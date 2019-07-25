using System.Collections.Generic;
using System.Linq;

namespace H3ml.Layout
{
    public class element //: public std::enable_shared_from_this<element>
    {
        protected element _parent; //: weak
        protected document _doc; //: weak
        protected box _box;
        protected elements_vector _children;
        protected position _pos;
        protected margins _margins;
        protected margins _padding;
        protected margins _borders;
        protected bool _skip;

        protected virtual void select_all(css_selector selector, elements_vector res) { }
        public element(document doc)
        {
            _doc = doc;
            _box = 0;
            _skip = false;
        }

        // returns refer to _pos member;
        public position get_position => _pos;

        public int left => _pos.left - margin_left - _padding.left - _borders.left;
        public int right => left + width();
        public int top => _pos.top - margin_top - _padding.top - _borders.top;
        public int bottom => top + height;
        public int height => _pos.height + margin_top + margin_bottom + _padding.height + _borders.height;
        public int width => _pos.width + margin_left + margin_right + _padding.width + _borders.width;

        public int content_margins_top => margin_top + _padding.top + _borders.top;
        public int content_margins_bottom => margin_bottom + _padding.bottom + _borders.bottom;
        public int content_margins_left => margin_left + _padding.left + _borders.left;
        public int content_margins_right => margin_right + _padding.right + _borders.right;
        public int content_margins_width => content_margins_left + content_margins_right;
        public int content_margins_height => content_margins_top + content_margins_bottom;

        public int margin_top => _margins.top;
        public int margin_bottom => _margins.bottom;
        public int margin_left => _margins.left;
        public int margin_right => _margins.right;
        public margins get_margins()
        {
            margins ret;
            ret.left = margin_left;
            ret.right = margin_right;
            ret.top = margin_top;
            ret.bottom = margin_bottom;
            return ret;
        }

        public int padding_top => _padding.top;
        public int padding_bottom => _padding.bottom;
        public int padding_left => _padding.left;
        public int padding_right => _padding.right;
        public margins get_paddings();

        public int border_top => _borders.top;
        public int border_bottom => _borders.bottom;
        public int border_left => _borders.left;
        public int border_right => _borders.right;
        public margins get_borders();

        public bool in_normal_flow => get_element_position != element_position_absolute && get_display() != display_none;
        public web_color get_color(string prop_name, bool inherited, web_color def_color = web_color())
        {
            var clrstr = get_style_property(prop_name, inherited, 0);
            return clrstr != null ? def_color : web_color.from_string(clrstr, get_document().container());
        }
        public bool is_inline_box()
        {
            style_display d = get_display();
            return d == display_inline ||
                d == display_inline_block ||
                d == display_inline_text;
        }
        public position get_placement()
        {
            var pos = _pos;
            var cur_el = parent();
            while (cur_el)
            {
                pos.x += cur_el.m_pos.x;
                pos.y += cur_el.m_pos.y;
                cur_el = cur_el.parent();
            }
            return pos;
        }
        public bool collapse_top_margin() => !_borders.top && !_padding.top && in_normal_flow() && get_float() == float_none && _margins.top >= 0 && have_parent();
        public bool collapse_bottom_margin() => !_borders.bottom && !_padding.bottom && in_normal_flow() && get_float() == float_none && _margins.bottom >= 0 && have_parent();

        public bool is_positioned => get_element_position > element_position_static;

        public bool skip
        {
            get => _skip;
            set => _skip = value;
        }
        public bool have_parent => !_parent.expired();
        public element parent => _parent.lock_();
        public void parent(element par) => _parent = par;
        public bool is_visible => !(_skip || get_display() == display_none || get_visibility() != visibility_visible);
        public int calc_width(int defVal)
        {
            css_length w = get_css_width();
            if (w.is_predefined())
                return defVal;
            if (w.units() == css_units_percentage)
            {
                var el_parent = parent();
                if (!el_parent)
                {
                    get_document().container().get_client_rect(out var client_pos);
                    return w.calc_percent(client_pos.width);
                }
                else
                {
                    var pw = el_parent.calc_width(defVal);
                    if (is_body())
                        pw -= content_margins_width();
                    return w.calc_percent(pw);
                }
            }
            return get_document().cvt_units(w, get_font_size());
        }
        public int get_inline_shift_left()
        {
            var ret = 0;
            var el_parent = parent();
            if (el_parent)
            {
                if (el_parent.get_display() == display_inline)
                {
                    var disp = get_display();
                    if (disp == display_inline_text || disp == display_inline_block)
                    {
                        var el = shared_from_this();
                        while (el_parent && el_parent.get_display() == display_inline)
                        {
                            if (el_parent.is_first_child_inline(el))
                                ret += el_parent.padding_left() + el_parent.border_left() + el_parent.margin_left();
                            el = el_parent;
                            el_parent = el_parent.parent();
                        }
                    }
                }
            }
            return ret;
        }
        public int get_inline_shift_right()
        {
            var ret = 0;
            var el_parent = parent();
            if (el_parent)
            {
                if (el_parent.get_display() == display_inline)
                {
                    var disp = get_display();
                    if (disp == display_inline_text || disp == display_inline_block)
                    {
                        var el = shared_from_this();
                        while (el_parent && el_parent.get_display() == display_inline)
                        {
                            if (el_parent.is_last_child_inline(el))
                                ret += el_parent.padding_right() + el_parent.border_right() + el_parent.margin_right();
                            el = el_parent;
                            el_parent = el_parent.parent();
                        }
                    }
                }
            }
            return ret;
        }
        public void apply_relative_shift(int parent_width)
        {
            if (get_element_position(out var offsets) == element_position_relative)
            {
                var parent_ptr = parent();
                if (!offsets.left.is_predefined())
                    _pos.x += offsets.left.calc_percent(parent_width);
                else if (!offsets.right.is_predefined())
                    _pos.x -= offsets.right.calc_percent(parent_width);
                if (!offsets.top.is_predefined())
                {
                    var h = 0;
                    if (offsets.top.units() == css_units_percentage)
                    {
                        var el_parent = parent();
                        if (el_parent)
                            el_parent.get_predefined_height(h);
                    }
                    _pos.y += offsets.top.calc_percent(h);
                }
                else if (!offsets.bottom.is_predefined())
                {
                    var h = 0;
                    if (offsets.top.units() == css_units_percentage)
                    {
                        var el_parent = parent();
                        if (el_parent)
                            el_parent.get_predefined_height(h);
                    }
                    _pos.y -= offsets.bottom.calc_percent(h);
                }
            }
        }

        public document get_document() => _doc.lock_();

        public virtual IList<element> select_all(string selector) => Enumerable.Empty<element>();
        public virtual IList<element> select_all(css_selector selector) => Enumerable.Empty<element>();

        public virtual element select_one(string selector) => null;
        public virtual element select_one(css_selector selector) => null;

        public virtual int render(int x, int y, int max_width, bool second_pass = false) => 0;
        public virtual int render_inline(ptr container, int max_width) => 0;
        public virtual int place_element(ptr el, int max_width) => 0;
        public virtual void calc_outlines(int parent_width) { }
        public virtual void calc_auto_margins(int parent_width) { }
        public virtual void apply_vertical_align() { }
        public virtual bool fetch_positioned() => false;
        public virtual void render_positioned(render_type rt = render_type.render_all) { }

        public virtual bool appendChild(ptr el) => false;

        public virtual bool removeChild(ptr el) => false;
        public virtual void clearRecursive() { }

        public virtual string get_tagName() => string.Empty;
        public virtual void set_tagName(string tag) { }
        public virtual void set_data(string data) { }
        public virtual element_float get_float() => float_none;
        public virtual vertical_align get_vertical_align() => va_baseline;
        public virtual element_clear get_clear() => clear_none;
        public virtual int get_children_count() => 0;
        public virtual element get_child(int idx) => null;
        public virtual overflow get_overflow() => overflow.overflow_visible;

        public virtual css_length get_css_left() => css_length();
        public virtual css_length get_css_right() => css_length();
        public virtual css_length get_css_top() => css_length();
        public virtual css_length get_css_bottom() => css_length();
        public virtual css_offsets get_css_offsets() => css_offsets();
        public virtual css_length get_css_width() => css_length();
        public virtual void set_css_width(css_length w) { }
        public virtual css_length get_css_height() => css_length();

        public virtual void set_attr(string name, string val) { }
        public virtual string get_attr(string name, string def = null) => def;
        public virtual void apply_stylesheet(css stylesheet) { }
        public virtual void refresh_styles() { }
        public virtual bool is_white_space() => false;
        public virtual bool is_body() => false;
        public virtual bool is_break() => false;
        public virtual int get_base_line() => 0;
        public virtual bool on_mouse_over() => false;
        public virtual bool on_mouse_leave() => false;
        public virtual bool on_lbutton_down() => false;
        public virtual bool on_lbutton_up() => false;
        public virtual void on_click() { }
        public virtual bool find_styles_changes(IList<position> redraw_boxes, int x, int y) => false;
        public virtual string get_cursor() => 0;
        public virtual void init_font() { }
        public virtual bool is_point_inside(int x, int y)
        {
            if (get_display() != display_inline && get_display() != display_table_row)
            {
                var pos = _pos;
                pos += _padding;
                pos += _borders;
                return pos.is_point_inside(x, y));
            }
            else
            {
                get_inline_boxes(out var boxes);
                foreach (var box in boxes)
                    if (box.is_point_inside(x, y))
                        return true;
            }
            return false;
        }

        public virtual bool set_pseudo_class(string pclass, bool add) => false;
        public virtual bool set_class(string pclass, bool add) => false;
        public virtual bool is_replaced() => false;
        public virtual int line_height() => 0;
        public virtual white_space get_white_space() => white_space_normal;
        public virtual style_display get_display() => display_none;
        public virtual visibility get_visibility() => visibility.visibility_visible;
        public virtual element_position get_element_position(css_offsets offsets = null) => element_position_static;
        public virtual void get_inline_boxes(IList<position> boxes) { }
        public virtual void parse_styles(bool is_reparse = false) { }
        public virtual void draw(uint_ptr hdc, int x, int y, position clip) { }
        public virtual void draw_background(uint_ptr hdc, int x, int y, position clip) { }
        public virtual string get_style_property(string name, bool inherited, string def = null) => 0;
        public virtual uint_ptr get_font(font_metrics fm = null) => 0;
        public virtual int get_font_size() => 0;
        public virtual void get_text(out string text) { text = null; }
        public virtual void parse_attributes() { }
        public virtual int select(css_selector selector, bool apply_pseudo = true) => select_no_match;
        public virtual int select(css_element_selector selector, bool apply_pseudo = true) => select_no_match;
        public virtual element find_ancestor(css_selector selector, bool apply_pseudo = true, out bool is_pseudo = 0) => 0;
        public virtual bool is_ancestor(ptr el)
        {
            var el_parent = parent();
            while (el_parent && el_parent != el)
                el_parent = el_parent.parent();
            return el_parent != null;
        }

        public virtual element find_adjacent_sibling(element el, css_selector selector, bool apply_pseudo = true, out bool is_pseudo = 0) => null;
        public virtual element find_sibling(element el, css_selector selector, bool apply_pseudo = true, out bool is_pseudo = 0) => null;
        public virtual bool is_first_child_inline(element el) => false;
        public virtual bool is_last_child_inline(element el) => false;
        public virtual bool have_inline_child() => false;
        public virtual void get_content_size(size sz, int max_width) { }
        public virtual void init() { }
        public virtual bool is_floats_holder() => false;
        public virtual int get_floats_height(element_float el_float = float_none) => 0;
        public virtual int get_left_floats_height() => 0;
        public virtual int get_right_floats_height() => 0;
        public virtual int get_line_left(int y) => 0;
        public virtual int get_line_right(int y, int def_right) => def_right;
        public virtual void get_line_left_right(int y, int def_right, ref int ln_left, ref int ln_right) { };
        public virtual void add_float(ptr el, int x, int y) { }
        public virtual void update_floats(int dy, ptr parent) { }
        public virtual void add_positioned(ptr el) { }
        public virtual int find_next_line_top(int top, int width, int def_right) => 0;
        public virtual int get_zindex() => 0;
        public virtual void draw_stacking_context(uint_ptr hdc, int x, int y, position clip, bool with_positioned) { }
        public virtual void draw_children(uint_ptr hdc, int x, int y, position clip, draw_flag flag, int zindex) { }
        public virtual bool is_nth_child(element el, int num, int off, bool of_type) => false;
        public virtual bool is_nth_last_child(element el, int num, int off, bool of_type) => false;
        public virtual bool is_only_child(element el, bool of_type) => false;
        public virtual bool get_predefined_height(int p_height)
        {
            var h = get_css_height();
            if (h.is_predefined())
            {
                p_height = m_pos.height;
                return false;
            }
            if (h.units() == css_units_percentage)
            {
                var el_parent = parent();
                if (!el_parent)
                {
                    get_document().container().get_client_rect(out var client_pos);
                    p_height = h.calc_percent(client_pos.height);
                    return true;
                }
                else
                {
                    var ph = 0;
                    if (el_parent.get_predefined_height(ph))
                    {
                        p_height = h.calc_percent(ph);
                        if (is_body())
                            p_height -= content_margins_height();
                        return true;
                    }
                    else
                    {
                        p_height = m_pos.height;
                        return false;
                    }
                }
            }
            p_height = get_document().cvt_units(h, get_font_size());
            return true;
        }
        public virtual void calc_document_size(size sz, int x = 0, int y = 0)
        {
            if (is_visible())
            {
                sz.width = Math.Max(sz.width, x + right());
                sz.height = Math.Max(sz.height, y + bottom());
            }
        }
        public virtual void get_redraw_box(position pos, int x = 0, int y = 0)
        {
            if (is_visible())
            {
                var p_left = Math.Min(pos.left(), x + _pos.left() - _padding.left - _borders.left);
                var p_right = Math.Max(pos.right(), x + _pos.right() + _padding.left + _borders.left);
                var p_top = Math.Min(pos.top(), y + _pos.top() - _padding.top - _borders.top);
                var p_bottom = Math.Max(pos.bottom(), y + _pos.bottom() + _padding.bottom + _borders.bottom);

                pos.x = p_left;
                pos.y = p_top;
                pos.width = p_right - p_left;
                pos.height = p_bottom - p_top;
            }
        }

        public virtual void add_style(style st) { }
        public virtual element get_element_by_point(int x, int y, int client_x, int client_y) => null;
        public virtual element get_child_by_point(int x, int y, int client_x, int client_y, draw_flag flag, int zindex) => null;
        public virtual background get_background(bool own_only = false) => null;
    }
}
