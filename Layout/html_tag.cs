using System;
using System.Collections.Generic;
using System.Linq;

namespace H3ml.Layout
{
    public struct line_context
    {
        public int calculatedTop;
        public int top;
        public int left;
        public int front; //:h3ml
        public int right;

        public int width => right - left;
        public void fix_top() => calculatedTop = top;
    }

    public class html_tag : element
    {
        protected List<box> _boxes = new List<box>();
        protected List<string> _class_values = new List<string>();
        protected string _tag;
        protected style _style = new style();
        protected Dictionary<string, string> _attrs = new Dictionary<string, string>();
        protected vertical_align _vertical_align;
        protected text_align _text_align;
        protected style_display _display;
        protected list_style_type _list_style_type;
        protected list_style_position _list_style_position;
        protected white_space _white_space;
        protected element_float _float;
        protected element_clear _clear;
        protected List<floated_box> _floats_left = new List<floated_box>();
        protected List<floated_box> _floats_right = new List<floated_box>();
        protected List<element> _positioned = new List<element>();
        protected background _bg = new background();
        protected element_position _el_position;
        protected int _line_height;
        protected bool _lh_predefined;
        protected List<string> _pseudo_classes = new List<string>();
        protected List<used_selector> _used_styles = new List<used_selector>();

        protected object _font;
        protected int _font_size;
        protected font_metrics _font_metrics;

        protected css_margins _css_margins;
        protected css_margins _css_padding;
        protected css_borders _css_borders;
        protected css_length _css_width;
        protected css_length _css_height;
        protected css_length _css_depth; //:h3ml
        protected css_length _css_min_width;
        protected css_length _css_min_height;
        protected css_length _css_min_depth; //:h3ml
        protected css_length _css_max_width;
        protected css_length _css_max_height;
        protected css_length _css_max_depth; //:h3ml
        protected css_offsets _css_offsets;
        protected css_length _css_text_indent;

        protected overflow _overflow;
        protected visibility _visibility;
        protected int _z_index;
        protected box_sizing _box_sizing;

        protected int_int_cache _cahe_line_left;
        protected int_int_cache _cahe_line_right;

        // data for table rendering
        protected table_grid _grid = new table_grid();
        protected css_length _css_border_spacing_x;
        protected css_length _css_border_spacing_y;
        protected css_length _css_border_spacing_z; //:h3ml
        protected int _border_spacing_x;
        protected int _border_spacing_y;
        protected int _border_spacing_z; //:h3ml
        protected border_collapse _border_collapse;

        public html_tag(document doc) : base(doc)
        {
            _box_sizing = box_sizing.content_box;
            _z_index = 0;
            _overflow = overflow.visible;
            _box = null;
            _text_align = text_align.left;
            _el_position = element_position.@static;
            _display = style_display.inline;
            _vertical_align = vertical_align.baseline;
            _list_style_type = list_style_type.none;
            _list_style_position = list_style_position.outside;
            _float = element_float.none;
            _clear = element_clear.none;
            _font = IntPtr.Zero;
            _font_size = 0;
            _white_space = white_space.normal;
            _lh_predefined = false;
            _line_height = 0;
            _visibility = visibility.visible;
            _border_spacing_x = 0;
            _border_spacing_y = 0;
            _border_spacing_z = 0; //:h3ml
            _border_collapse = border_collapse.separate;
        }

        /* render functions */

        public override int render(int x, int y, int z, int max_width, bool second_pass = false) => _display == style_display.table || _display == style_display.inline_table
            ? render_table(x, y, z, max_width, second_pass) //:h3ml
            : render_box(x, y, z, max_width, second_pass); //:h3ml

        public override int render_inline(element container, int max_width)
        {
            var ret_width = 0;
            var rw = 0;
            var ws = get_white_space;
            var skip_spaces = ws == white_space.normal || ws == white_space.nowrap || ws == white_space.pre_line;
            var was_space = false;
            foreach (var el in _children)
            {
                // skip spaces to make rendering a bit faster
                if (skip_spaces)
                    if (el.is_white_space())
                    {
                        if (was_space)
                        {
                            el.skip = true;
                            continue;
                        }
                        else was_space = true;
                    }
                    else was_space = false;
                rw = container.place_element(el, max_width);
                if (rw > ret_width)
                    ret_width = rw;
            }
            return ret_width;
        }
        public override int place_element(element el, int max_width)
        {
            if (el.get_display == style_display.none) return 0;
            if (el.get_display == style_display.inline)
                return el.render_inline(this, max_width);

            var el_position = el.get_element_position(out var junk);
            if (el_position == element_position.absolute || el_position == element_position.@fixed)
            {
                var line_top = 0;
                if (_boxes.Count != 0)
                {
                    if (_boxes.Last().get_type == box_type.line)
                    {
                        line_top = _boxes.Last().top;
                        if (!_boxes.Last().is_empty)
                            line_top += line_height;
                    }
                    else line_top = _boxes.Last().bottom;
                }
                el.render(0, line_top, 0, max_width); //:h3ml
                el._pos.x += el.content_margins_left;
                el._pos.y += el.content_margins_top;
                el._pos.z += el.content_margins_front; //:h3ml
                return 0;
            }

            var ret_width = 0;
            switch (el.get_float)
            {
                case element_float.left:
                    {
                        var line_top = 0;
                        if (_boxes.Count != 0)
                            line_top = _boxes.Last().get_type == box_type.line ? _boxes.Last().top : _boxes.Last().bottom;
                        line_top = get_cleared_top(el, line_top);
                        var line_left = 0;
                        var line_right = max_width;
                        get_line_left_right(line_top, max_width, ref line_left, ref line_right);

                        el.render(line_left, line_top, 0, line_right); //:h3ml

                        if (el.right > line_right)
                        {
                            var new_top = find_next_line_top(el.top, el.width, max_width);
                            el._pos.x = get_line_left(new_top) + el.content_margins_left;
                            el._pos.y = new_top + el.content_margins_top;
                            //el._pos.z = get_line_front(new_top) + el.content_margins_front; //:h3ml
                        }
                        add_float(el, 0, 0, 0);
                        ret_width = fix_line_width(max_width, element_float.left);
                        if (ret_width == 0)
                            ret_width = el.right;
                    }
                    break;
                case element_float.right:
                    {
                        var line_top = 0;
                        if (_boxes.Count != 0)
                            line_top = _boxes.Last().get_type == box_type.line ? _boxes.Last().top : _boxes.Last().bottom;
                        line_top = get_cleared_top(el, line_top);
                        var line_left = 0;
                        var line_right = max_width;
                        get_line_left_right(line_top, max_width, ref line_left, ref line_right);

                        el.render(0, line_top, 0, line_right); //:h3ml

                        if (line_left + el.width > line_right)
                        {
                            var new_top = find_next_line_top(el.top, el.width, max_width);
                            el._pos.x = get_line_right(new_top, max_width) - el.width + el.content_margins_left;
                            el._pos.y = new_top + el.content_margins_top;
                            //el._pos.z = get_line_back(new_top, max_width) - el.depth + el.content_margins_front; //:h3ml
                        }
                        else
                            el._pos.x = line_right - el.width + el.content_margins_left;
                        add_float(el, 0, 0, 0); //:h3ml
                        ret_width = fix_line_width(max_width, element_float.right);
                        if (ret_width == 0)
                        {
                            line_left = 0;
                            line_right = max_width;
                            get_line_left_right(line_top, max_width, ref line_left, ref line_right);
                            ret_width = ret_width + (max_width - line_right);
                        }
                    }
                    break;
                default:
                    {
                        var line_ctx = new line_context();
                        line_ctx.top = 0;
                        if (_boxes.Count != 0)
                            line_ctx.top = _boxes.Last().top;
                        line_ctx.left = 0;
                        line_ctx.right = max_width;
                        line_ctx.fix_top();
                        get_line_left_right(line_ctx.top, max_width, ref line_ctx.left, ref line_ctx.right);

                        switch (el.get_display)
                        {
                            case style_display.inline_block:
                                ret_width = el.render(line_ctx.left, line_ctx.top, line_ctx.front, line_ctx.right); //:h3ml
                                break;
                            case style_display.block:
                                if (el.is_replaced || el.is_floats_holder)
                                {
                                    var el_parent = el.parent();
                                    el._pos.width = el.get_css_width().calc_percent(line_ctx.right - line_ctx.left);
                                    el._pos.height = el.get_css_height().calc_percent(el_parent != null ? el_parent._pos.height : 0);
                                    el._pos.depth = el.get_css_depth().calc_percent(el_parent != null ? el_parent._pos.depth : 0); //:h3ml
                                }
                                el.calc_outlines(line_ctx.right - line_ctx.left);
                                break;
                            case style_display.inline_text:
                                {
                                    el.get_content_size(out var sz, line_ctx.right);
                                    el._pos.assignTo(sz);
                                }
                                break;
                            default: ret_width = 0; break;
                        }

                        var add_box = true;
                        if (_boxes.Count != 0)
                            if (_boxes.Last().can_hold(el, _white_space))
                                add_box = false;
                        if (add_box)
                            new_box(el, max_width, line_ctx);
                        else if (_boxes.Count != 0)
                            line_ctx.top = _boxes.Last().top;
                        if (line_ctx.top != line_ctx.calculatedTop)
                        {
                            line_ctx.left = 0;
                            line_ctx.right = max_width;
                            line_ctx.fix_top();
                            get_line_left_right(line_ctx.top, max_width, ref line_ctx.left, ref line_ctx.right);
                        }

                        if (!el.is_inline_box)
                        {
                            if (_boxes.Count == 1)
                            {
                                if (collapse_top_margin)
                                {
                                    var shift = el.margin_top;
                                    if (shift >= 0)
                                    {
                                        line_ctx.top -= shift;
                                        _boxes.Last().y_shift(-shift);
                                    }
                                }
                            }
                            else
                            {
                                var shift = 0;
                                var prev_margin = _boxes[_boxes.Count - 2].bottom_margin;
                                shift = prev_margin > el.margin_top ? el.margin_top : prev_margin;
                                if (shift >= 0)
                                {
                                    line_ctx.top -= shift;
                                    _boxes.Last().y_shift(-shift);
                                }
                            }
                        }

                        switch (el.get_display)
                        {
                            case style_display.table:
                            case style_display.list_item:
                                ret_width = el.render(line_ctx.left, line_ctx.top, line_ctx.front, line_ctx.width); //:h3ml
                                break;
                            case style_display.block:
                            case style_display.table_cell:
                            case style_display.table_caption:
                            case style_display.table_row:
                                ret_width = el.is_replaced || el.is_floats_holder
                                    ? el.render(line_ctx.left, line_ctx.top, line_ctx.front, line_ctx.width) + line_ctx.left + (max_width - line_ctx.right) //:h3ml
                                    : el.render(0, line_ctx.top, line_ctx.front, max_width); //:h3ml
                                break;
                            default:
                                ret_width = 0;
                                break;
                        }

                        _boxes.Last().add_element(el);
                        if (el.is_inline_box && !el.skip)
                            ret_width = el.right + (max_width - line_ctx.right);
                    }
                    break;
            }

            return ret_width;
        }

        public override bool fetch_positioned()
        {
            var ret = false;
            _positioned.Clear();
            element_position el_pos;
            foreach (var el in _children)
            {
                el_pos = el.get_element_position(out var junk);
                if (el_pos != element_position.@static)
                    add_positioned(el);
                if (!ret && (el_pos == element_position.absolute || el_pos == element_position.@fixed))
                    ret = true;
                if (el.fetch_positioned())
                    ret = true;
            }
            return ret;
        }
        public override void render_positioned(render_type rt = render_type.all)
        {
            get_document().container.get_client_rect(out var wnd_position);
            element_position el_position;
            bool process;
            foreach (var el in _positioned)
            {
                el_position = el.get_element_position(out var junk);
                process = false;
                if (el.get_display != style_display.none)
                {
                    if (el_position == element_position.absolute)
                    {
                        if (rt != render_type.fixed_only)
                            process = true;
                    }
                    else if (el_position == element_position.@fixed)
                    {
                        if (rt != render_type.no_fixed)
                            process = true;
                    }
                }

                if (process)
                {
                    var parent_height = 0;
                    var parent_width = 0;
                    var parent_depth = 0; //:h3ml
                    var client_x = 0;
                    var client_y = 0;
                    var client_z = 0; //:h3ml
                    if (el_position == element_position.@fixed)
                    {
                        parent_height = wnd_position.height;
                        parent_width = wnd_position.width;
                        parent_depth = wnd_position.depth; //:h3ml
                        client_x = wnd_position.left;
                        client_y = wnd_position.top;
                        client_z = wnd_position.depth; //:h3ml
                    }
                    else
                    {
                        var el_parent = el.parent();
                        if (el_parent != null)
                        {
                            parent_height = el_parent.height;
                            parent_width = el_parent.width;
                            parent_depth = el_parent.depth; //:h3ml
                        }
                    }

                    var css_left = el.get_css_left();
                    var css_right = el.get_css_right();
                    var css_top = el.get_css_top();
                    var css_bottom = el.get_css_bottom();
                    var css_front = el.get_css_front(); //:h3ml
                    var css_back = el.get_css_back(); //:h3ml

                    var need_render = false;

                    var el_w = el.get_css_width();
                    var el_h = el.get_css_height();
                    var el_d = el.get_css_depth(); //:h3ml

                    var new_width = -1;
                    var new_height = -1;
                    var new_depth = -1; //:h3ml
                    if (el_w.units == css_units.percentage && parent_width != 0)
                    {
                        new_width = el_w.calc_percent(parent_width);
                        if (el._pos.width != new_width)
                        {
                            need_render = true;
                            el._pos.width = new_width;
                        }
                    }
                    if (el_h.units == css_units.percentage && parent_height != 0)
                    {
                        new_height = el_h.calc_percent(parent_height);
                        if (el._pos.height != new_height)
                        {
                            need_render = true;
                            el._pos.height = new_height;
                        }
                    }
                    //:h3ml
                    if (el_h.units == css_units.percentage && parent_depth != 0) //:h3ml
                    {
                        new_depth = el_d.calc_percent(parent_depth); //:h3ml
                        if (el._pos.depth != new_depth) //:h3ml
                        {
                            need_render = true;
                            el._pos.depth = new_depth; //:h3ml
                        }
                    }

                    var cvt_x = false;
                    var cvt_y = false;
                    var cvt_z = false; //:h3ml
                    if (el_position == element_position.@fixed)
                    {
                        if (!css_left.is_predefined || !css_right.is_predefined)
                        {
                            if (!css_left.is_predefined && css_right.is_predefined) el._pos.x = css_left.calc_percent(parent_width) + el.content_margins_left;
                            else if (css_left.is_predefined && !css_right.is_predefined) el._pos.x = parent_width - css_right.calc_percent(parent_width) - el._pos.width - el.content_margins_right;
                            else
                            {
                                el._pos.x = css_left.calc_percent(parent_width) + el.content_margins_left;
                                el._pos.width = parent_width - css_left.calc_percent(parent_width) - css_right.calc_percent(parent_width) - (el.content_margins_left + el.content_margins_right);
                                need_render = true;
                            }
                        }
                        if (!css_top.is_predefined || !css_bottom.is_predefined)
                        {
                            if (!css_top.is_predefined && css_bottom.is_predefined) el._pos.y = css_top.calc_percent(parent_height) + el.content_margins_top;
                            else if (css_top.is_predefined && !css_bottom.is_predefined) el._pos.y = parent_height - css_bottom.calc_percent(parent_height) - el._pos.height - el.content_margins_bottom;
                            else
                            {
                                el._pos.y = css_top.calc_percent(parent_height) + el.content_margins_top;
                                el._pos.height = parent_height - css_top.calc_percent(parent_height) - css_bottom.calc_percent(parent_height) - (el.content_margins_top + el.content_margins_bottom);
                                need_render = true;
                            }
                        }
                        //:h3ml
                        if (!css_front.is_predefined || !css_back.is_predefined) //:h3ml
                        {
                            if (!css_front.is_predefined && css_back.is_predefined) el._pos.z = css_front.calc_percent(parent_depth) + el.content_margins_front; //:h3ml
                            else if (css_front.is_predefined && !css_back.is_predefined) el._pos.z = parent_depth - css_back.calc_percent(parent_depth) - el._pos.depth - el.content_margins_back; //:h3ml
                            else
                            {
                                el._pos.z = css_front.calc_percent(parent_depth) + el.content_margins_front; //:h3ml
                                el._pos.depth = parent_depth - css_front.calc_percent(parent_depth) - css_back.calc_percent(parent_depth) - (el.content_margins_front + el.content_margins_back); //:h3ml
                                need_render = true;
                            }
                        }
                    }
                    else
                    {
                        if (!css_left.is_predefined || !css_right.is_predefined)
                        {
                            if (!css_left.is_predefined && css_right.is_predefined) el._pos.x = css_left.calc_percent(parent_width) + el.content_margins_left - _padding.left;
                            else if (css_left.is_predefined && !css_right.is_predefined) el._pos.x = _pos.width + _padding.right - css_right.calc_percent(parent_width) - el._pos.width - el.content_margins_right;
                            else
                            {
                                el._pos.x = css_left.calc_percent(parent_width) + el.content_margins_left - _padding.left;
                                el._pos.width = _pos.width + _padding.left + _padding.right - css_left.calc_percent(parent_width) - css_right.calc_percent(parent_width) - (el.content_margins_left + el.content_margins_right);
                                if (new_width != -1)
                                {
                                    el._pos.x += (el._pos.width - new_width) / 2;
                                    el._pos.width = new_width;
                                }
                                need_render = true;
                            }
                            cvt_x = true;
                        }
                        if (!css_top.is_predefined || !css_bottom.is_predefined)
                        {
                            if (!css_top.is_predefined && css_bottom.is_predefined) el._pos.y = css_top.calc_percent(parent_height) + el.content_margins_top - _padding.top;
                            else if (css_top.is_predefined && !css_bottom.is_predefined) el._pos.y = _pos.height + _padding.bottom - css_bottom.calc_percent(parent_height) - el._pos.height - el.content_margins_bottom;
                            else
                            {
                                el._pos.y = css_top.calc_percent(parent_height) + el.content_margins_top - _padding.top;
                                el._pos.height = _pos.height + _padding.top + _padding.bottom - css_top.calc_percent(parent_height) - css_bottom.calc_percent(parent_height) - (el.content_margins_top + el.content_margins_bottom);
                                if (new_height != -1)
                                {
                                    el._pos.y += (el._pos.height - new_height) / 2;
                                    el._pos.height = new_height;
                                }
                                need_render = true;
                            }
                            cvt_y = true;
                        }
                        //:h3ml
                        if (!css_front.is_predefined || !css_back.is_predefined) //:h3ml
                        {
                            if (!css_front.is_predefined && css_back.is_predefined) el._pos.z = css_front.calc_percent(parent_depth) + el.content_margins_front - _padding.front; //:h3ml
                            else if (css_front.is_predefined && !css_back.is_predefined) el._pos.z = _pos.depth + _padding.bottom - css_back.calc_percent(parent_depth) - el._pos.depth - el.content_margins_back; //:h3ml
                            else
                            {
                                el._pos.z = css_front.calc_percent(parent_depth) + el.content_margins_front - _padding.front; //:h3ml
                                el._pos.depth = _pos.depth + _padding.top + _padding.bottom - css_front.calc_percent(parent_depth) - css_back.calc_percent(parent_depth) - (el.content_margins_front + el.content_margins_back); //:h3ml
                                if (new_depth != -1)
                                {
                                    el._pos.z += (el._pos.depth - new_depth) / 2; //:h3ml
                                    el._pos.depth = new_depth; //:h3ml
                                }
                                need_render = true;
                            }
                            cvt_z = true; //:h3ml
                        }
                    }

                    if (cvt_x || cvt_y || cvt_z) //:h3ml
                    {
                        var offset_x = 0;
                        var offset_y = 0;
                        var offset_z = 0; //:h3ml
                        var cur_el = el.parent();
                        var this_el = (element)this;
                        while (cur_el != null && cur_el != this_el)
                        {
                            offset_x += cur_el._pos.x;
                            offset_y += cur_el._pos.y;
                            offset_z += cur_el._pos.z; //:h3ml
                            cur_el = cur_el.parent();
                        }
                        if (cvt_x) el._pos.x -= offset_x;
                        if (cvt_y) el._pos.y -= offset_y;
                        if (cvt_z) el._pos.z -= offset_z; //:h3ml
                    }

                    if (need_render)
                    {
                        var pos = el._pos;
                        el.render(el.left, el.top, el.front, el.width, true);
                        el._pos = pos;
                    }

                    if (el_position == element_position.@fixed)
                    {
                        var fixed_pos = new position();
                        el.get_redraw_box(ref fixed_pos);
                        get_document().add_fixed_box(fixed_pos);
                    }
                }

                el.render_positioned();
            }

            if (_positioned.Count != 0)
                _positioned.Sort((left, right) => left.get_zindex < right.get_zindex ? 1 : 0);
        }

        public int new_box(element el, int max_width, line_context line_ctx)
        {
            line_ctx.top = get_cleared_top(el, finish_last_box());
            line_ctx.left = 0;
            line_ctx.front = 0; //:h3ml
            line_ctx.right = max_width;
            line_ctx.fix_top();
            get_line_left_right(line_ctx.top, max_width, ref line_ctx.left, ref line_ctx.right);

            if (el.is_inline_box || el.is_floats_holder)
                if (el.width > line_ctx.right - line_ctx.left)
                {
                    line_ctx.top = find_next_line_top(line_ctx.top, el.width, max_width);
                    line_ctx.left = 0;
                    line_ctx.front = 0; //:h3ml
                    line_ctx.right = max_width;
                    line_ctx.fix_top();
                    get_line_left_right(line_ctx.top, max_width, ref line_ctx.left, ref line_ctx.right);
                }

            var first_line_margin = 0;
            if (_boxes.Count == 0 && _list_style_type != list_style_type.none && _list_style_position == list_style_position.inside)
                first_line_margin = get_font_size;

            if (el.is_inline_box)
            {
                var text_indent = 0;
                if (_css_text_indent.val != 0)
                {
                    var line_box_found = false;
                    foreach (var iter in _boxes)
                        if (iter.get_type == box_type.line)
                        {
                            line_box_found = true;
                            break;
                        }
                    if (!line_box_found)
                        text_indent = _css_text_indent.calc_percent(max_width);
                }
                get_font(out var fm);
                _boxes.Add(new line_box(line_ctx.top, line_ctx.left + first_line_margin + text_indent, line_ctx.front, line_ctx.right, line_height, fm, _text_align)); //:h3ml
            }
            else _boxes.Add(new block_box(line_ctx.top, line_ctx.left, line_ctx.front, line_ctx.right));
            return line_ctx.top;
        }

        public int get_cleared_top(element el, int line_top)
        {
            switch (el.get_clear)
            {
                case element_clear.left:
                    {
                        var fh = get_left_floats_height();
                        if (fh != 0 && fh > line_top)
                            line_top = fh;
                    }
                    break;
                case element_clear.right:
                    {
                        var fh = get_right_floats_height();
                        if (fh != 0 && fh > line_top)
                            line_top = fh;
                    }
                    break;
                case element_clear.both:
                    {
                        var fh = get_floats_height();
                        if (fh != 0 && fh > line_top)
                            line_top = fh;
                    }
                    break;
                default:
                    if (el.get_float != element_float.none)
                    {
                        var fh = get_floats_height(el.get_float);
                        if (fh != 0 && fh > line_top)
                            line_top = fh;
                    }
                    break;
            }
            return line_top;
        }
        public int finish_last_box(bool end_of_render = false)
        {
            var line_top = 0;
            if (_boxes.Count != 0)
            {
                _boxes.Last().finish(end_of_render);
                if (_boxes.Last().is_empty)
                {
                    line_top = _boxes.Last().top;
                    _boxes.RemoveAt(_boxes.Count - 1);
                }
                if (_boxes.Count != 0)
                    line_top = _boxes.Last().bottom;
            }
            return line_top;
        }

        public override bool appendChild(element el)
        {
            if (el != null)
            {
                el.parent(this);
                _children.Add(el);
                return true;
            }
            return false;
        }
        public override bool removeChild(element el)
        {
            if (el != null && el.parent() == this)
            {
                el.parent(null);
                _children.Remove(el); //_children.erase(std::remove(m_children.begin(), m_children.end(), el), m_children.end());
                return true;
            }
            return false;
        }
        public override void clearRecursive()
        {
            foreach (var el in _children)
            {
                el.clearRecursive();
                el.parent(null);
            }
            _children.Clear();
        }
        public override string get_tagName() => _tag;
        public override void set_tagName(string tag) => _tag = tag.ToLowerInvariant();
        public override void set_data(string data) { }
        public override element_float get_float => _float;
        public override vertical_align get_vertical_align => _vertical_align;
        public override css_length get_css_left() => _css_offsets.left;
        public override css_length get_css_right() => _css_offsets.right;
        public override css_length get_css_top() => _css_offsets.top;
        public override css_length get_css_bottom() => _css_offsets.bottom;
        public override css_length get_css_width() => _css_width;
        public override css_offsets get_css_offsets() => _css_offsets;
        public override void set_css_width(css_length w) => _css_width = w;
        public override css_length get_css_height() => _css_height;
        public override element_clear get_clear => _clear;
        public override int get_children_count => _children.Count;
        public override element get_child(int idx) => _children[idx];
        public override element_position get_element_position(out css_offsets offsets)
        {
            if (_el_position != element_position.@static) offsets = _css_offsets;
            else offsets = default(css_offsets);
            return _el_position;
        }
        public override overflow get_overflow => _overflow;
        public override void set_attr(string name, string val)
        {
            if (name != null && val != null)
            {
                _attrs[name.ToLowerInvariant()] = val;
                if (string.Equals(name, "class", StringComparison.OrdinalIgnoreCase))
                {
                    _class_values.Clear();
                    html.split_string(val, _class_values, " ");
                }
            }
        }
        public override string get_attr(string name, string def = null) => _attrs.TryGetValue(name, out var attr) ? attr : def;
        public override void apply_stylesheet(css stylesheet)
        {
            remove_before_after();
            foreach (var sel in stylesheet.selectors())
            {
                var apply = select(sel, false);
                if (apply != select_result.no_match)
                {
                    var us = new used_selector(sel, false);
                    if (sel.is_media_valid)
                        if ((apply & select_result.match_pseudo_class) != 0)
                        {
                            if (select(sel, true) != 0)
                            {
                                if ((apply & select_result.match_with_after) != 0)
                                {
                                    var el = get_element_after();
                                    if (el != null)
                                        el.add_style(sel._style);
                                }
                                else if ((apply & select_result.match_with_before) != 0)
                                {
                                    var el = get_element_before();
                                    if (el != null)
                                        el.add_style(sel._style);
                                }
                                else
                                {
                                    add_style(sel._style);
                                    us._used = true;
                                }
                            }
                        }
                        else if ((apply & select_result.match_with_after) != 0)
                        {
                            var el = get_element_after();
                            if (el != null)
                                el.add_style(sel._style);
                        }
                        else if ((apply & select_result.match_with_before) != 0)
                        {
                            var el = get_element_before();
                            if (el != null)
                                el.add_style(sel._style);
                        }
                        else
                        {
                            add_style(sel._style);
                            us._used = true;
                        }
                    _used_styles.Add(us);
                }
            }
            foreach (var el in _children)
                if (el.get_display != style_display.inline_text)
                    el.apply_stylesheet(stylesheet);
        }
        public override void refresh_styles()
        {
            remove_before_after();
            foreach (var el in _children)
                if (el.get_display != style_display.inline_text)
                    el.refresh_styles();
            _style.clear();
            foreach (var usel in _used_styles)
            {
                usel._used = false;
                if (usel._selector.is_media_valid)
                {
                    var apply = select(usel._selector, false);
                    if (apply != select_result.no_match)
                        if ((apply & select_result.match_pseudo_class) != 0)
                        {
                            if (select(usel._selector, true) != 0)
                                if ((apply & select_result.match_with_after) != 0)
                                {
                                    var el = get_element_after();
                                    if (el != null)
                                        el.add_style(usel._selector._style);
                                }
                                else if ((apply & select_result.match_with_before) != 0)
                                {
                                    var el = get_element_before();
                                    if (el != null)
                                        el.add_style(usel._selector._style);
                                }
                                else
                                {
                                    add_style(usel._selector._style);
                                    usel._used = true;
                                }
                        }
                        else if ((apply & select_result.match_with_after) != 0)
                        {
                            var el = get_element_after();
                            if (el != null)
                                el.add_style(usel._selector._style);
                        }
                        else if ((apply & select_result.match_with_before) != 0)
                        {
                            var el = get_element_before();
                            if (el != null)
                                el.add_style(usel._selector._style);
                        }
                        else
                        {
                            add_style(usel._selector._style);
                            usel._used = true;
                        }
                }
            }
        }

        public override bool is_white_space() => false;
        public override bool is_body() => false;
        public override bool is_break() => false;
        public override int get_base_line()
        {
            if (is_replaced)
                return 0;
            var bl = 0;
            if (_boxes.Count != 0)
                bl = _boxes.Last().baseline + content_margins_bottom;
            return bl;
        }
        public override bool on_mouse_over()
        {
            var ret = false;
            var el = (element)this;
            while (el != null)
            {
                if (el.set_pseudo_class("hover", true)) ret = true;
                el = el.parent();
            }
            return ret;
        }
        public override bool on_mouse_leave()
        {
            var ret = false;
            var el = (element)this;
            while (el != null)
            {
                if (el.set_pseudo_class("hover", false)) ret = true;
                if (el.set_pseudo_class("active", false)) ret = true;
                el = el.parent();
            }
            return ret;
        }
        public override bool on_lbutton_down()
        {
            var ret = false;
            var el = (element)this;
            while (el != null)
            {
                if (el.set_pseudo_class("active", true)) ret = true;
                el = el.parent();
            }
            return ret;
        }
        public override bool on_lbutton_up()
        {
            var ret = false;
            var el = (element)this;
            while (el != null)
            {
                if (el.set_pseudo_class("active", false)) ret = true;
                el = el.parent();
            }
            on_click();
            return ret;
        }

        public override void on_click()
        {
            if (have_parent)
            {
                var el_parent = parent();
                if (el_parent != null)
                    el_parent.on_click();
            }
        }
        public override bool find_styles_changes(IList<position> redraw_boxes, int x, int y, int z)
        {
            if (_display == style_display.inline_text)
                return false;
            var ret = false;
            var apply = false;
            foreach (var iter in _used_styles)
                if (iter._selector.is_media_valid)
                {
                    var res = select(iter._selector, true);
                    if ((res == select_result.no_match && iter._used) || (res == select_result.match && !iter._used))
                    {
                        apply = true;
                        break;
                    }
                }
            if (apply)
            {
                if (_display == style_display.inline || _display == style_display.table_row)
                {
                    var boxes = new List<position>();
                    get_inline_boxes(boxes);
                    for (var i = 0; i < boxes.Count; i++)
                    {
                        var pos = boxes[i];
                        pos.x += x;
                        pos.y += y;
                        pos.z += z; //:h3ml
                        redraw_boxes.Add(pos);
                    }
                }
                else
                {
                    var pos = _pos;
                    if (_el_position != element_position.@fixed)
                    {
                        pos.x += x;
                        pos.y += y;
                        pos.z += z; //:h3ml
                    }
                    pos += _padding;
                    pos += _borders;
                    redraw_boxes.Add(pos);
                }

                ret = true;
                refresh_styles();
                parse_styles();
            }
            foreach (var el in _children)
                if (!el.skip)
                    if (_el_position != element_position.@fixed)
                    {
                        if (el.find_styles_changes(redraw_boxes, x + _pos.x, y + _pos.y, z + _pos.z)) //:h3ml
                            ret = true;
                    }
                    else
                    {
                        if (el.find_styles_changes(redraw_boxes, _pos.x, _pos.y, _pos.z)) //:h3ml
                            ret = true;
                    }
            return ret;
        }

        public override string get_cursor() => get_style_property("cursor", true, null);

        static readonly int[][] font_size_table =
        {
            new int[]{ 9,    9,     9,     9,    11,    14,    18},
            new int[]{ 9,    9,     9,    10,    12,    15,    20},
            new int[]{ 9,    9,     9,    11,    13,    17,    22},
            new int[]{ 9,    9,    10,    12,    14,    18,    24},
            new int[]{ 9,    9,    10,    13,    16,    20,    26},
            new int[]{ 9,    9,    11,    14,    17,    21,    28},
            new int[]{ 9,   10,    12,    15,    17,    23,    30},
            new int[]{ 9,   10,    13,    16,    18,    24,    32}
        };

        public override void init_font()
        {
            // initialize font size
            var str = get_style_property("font-size", false, null);
            var parent_sz = 0;
            var doc_font_size = get_document().container.get_default_font_size();
            var el_parent = parent();
            parent_sz = el_parent != null ? el_parent.get_font_size : doc_font_size;
            if (str == null)
                _font_size = parent_sz;
            else
            {
                _font_size = parent_sz;
                var sz = new css_length();
                sz.fromString(str, types.font_size_strings);
                if (sz.is_predefined)
                {
                    var idx_in_table = doc_font_size - 9;
                    if (idx_in_table >= 0 && idx_in_table <= 7)
                        _font_size = sz.predef >= (int)font_size.xx_small && sz.predef <= (int)font_size.xx_large ? font_size_table[idx_in_table][sz.predef] : doc_font_size;
                    else switch ((font_size)sz.predef)
                        {
                            case font_size.xx_small: _font_size = doc_font_size * 3 / 5; break;
                            case font_size.x_small: _font_size = doc_font_size * 3 / 4; break;
                            case font_size.small: _font_size = doc_font_size * 8 / 9; break;
                            case font_size.large: _font_size = doc_font_size * 6 / 5; break;
                            case font_size.x_large: _font_size = doc_font_size * 3 / 2; break;
                            case font_size.xx_large: _font_size = doc_font_size * 2; break;
                            default: _font_size = doc_font_size; break;
                        }
                }
                else
                {
                    if (sz.units == css_units.percentage) _font_size = sz.calc_percent(parent_sz);
                    else if (sz.units == css_units.none) _font_size = parent_sz;
                    else _font_size = get_document().cvt_units(sz, parent_sz);
                }
            }
            // initialize font
            var name = get_style_property("font-family", true, "inherit");
            var weight = get_style_property("font-weight", true, "normal");
            var style = get_style_property("font-style", true, "normal");
            var decoration = get_style_property("text-decoration", true, "none");
            _font = get_document().get_font(name, _font_size, weight, style, decoration, out _font_metrics);
        }

        public override bool set_pseudo_class(string pclass, bool add)
        {
            var ret = false;
            if (add)
            {
                if (!_pseudo_classes.Contains(pclass))
                {
                    _pseudo_classes.Add(pclass);
                    ret = true;
                }
            }
            else ret = _pseudo_classes.Remove(pclass);
            return ret;
        }

        public override bool set_class(string pclass, bool add)
        {
            var changed = false;
            var classes = new List<string>();
            html.split_string(pclass, classes, " ");
            if (add)
            {
                foreach (var _class in classes)
                    if (!_class_values.Contains(_class))
                    {
                        _class_values.Add(_class);
                        changed = true;
                    }
            }
            else
            {
                foreach (var _class in classes)
                    if (_class_values.Remove(_class))
                        changed = true;
            }
            if (changed)
            {
                var class_string = string.Join(" ", _class_values);
                set_attr("class", class_string);
                return true;
            }
            else return false;
        }
        public override bool is_replaced => false;
        public override int line_height => _line_height;
        public override white_space get_white_space => _white_space;
        public override style_display get_display => _display;
        public override visibility get_visibility => _visibility;
        public override void parse_styles(bool is_reparse = false)
        {
            var style = get_attr("style"); if (style != null) _style.add(style, null);
            init_font();
            var doc = get_document();
            _el_position = (element_position)html.value_index(get_style_property("position", false, "static"), types.element_position_strings, (int)element_position.@fixed);
            _text_align = (text_align)html.value_index(get_style_property("text-align", true, "left"), types.text_align_strings, (int)text_align.left);
            _overflow = (overflow)html.value_index(get_style_property("overflow", false, "visible"), types.overflow_strings, (int)overflow.visible);
            _white_space = (white_space)html.value_index(get_style_property("white-space", true, "normal"), types.white_space_strings, (int)white_space.normal);
            _display = (style_display)html.value_index(get_style_property("display", false, "inline"), types.style_display_strings, (int)style_display.inline);
            _visibility = (visibility)html.value_index(get_style_property("visibility", true, "visible"), types.visibility_strings, (int)visibility.visible);
            _box_sizing = (box_sizing)html.value_index(get_style_property("box-sizing", false, "content-box"), types.box_sizing_strings, (int)box_sizing.content_box);
            if (_el_position != element_position.@static)
            {
                var val = get_style_property("z-index", false, null);
                if (val != null) _z_index = int.Parse(val);
            }
            var va = get_style_property("vertical-align", true, "baseline");
            _vertical_align = (vertical_align)html.value_index(va, types.vertical_align_strings, (int)vertical_align.baseline);
            var fl = get_style_property("float", false, "none");
            _float = (element_float)html.value_index(fl, types.element_float_strings, (int)element_float.none);
            _clear = (element_clear)html.value_index(get_style_property("clear", false, "none"), types.element_clear_strings, (int)element_clear.none);

            if (_float != element_float.none)
            {
                // reset display in to block for floating elements
                if (_display != style_display.none)
                    _display = style_display.block;
            }
            else if (_display == style_display.table ||
                _display == style_display.table_caption ||
                _display == style_display.table_cell ||
                _display == style_display.table_column ||
                _display == style_display.table_column_group ||
                _display == style_display.table_footer_group ||
                _display == style_display.table_header_group ||
                _display == style_display.table_row ||
                _display == style_display.table_row_group)
                doc.add_tabular(this);
            // fix inline boxes with absolute/fixed positions
            else if (_display != style_display.none && is_inline_box)
                if (_el_position == element_position.absolute || _el_position == element_position.@fixed)
                    _display = style_display.block;

            _css_text_indent.fromString(get_style_property("text-indent", true, "0"), "0");
            _css_width.fromString(get_style_property("width", false, "auto"), "auto");
            _css_height.fromString(get_style_property("height", false, "auto"), "auto");
            _css_depth.fromString(get_style_property("depth", false, "auto"), "auto"); //:h3ml
            doc.cvt_units(_css_width, _font_size);
            doc.cvt_units(_css_height, _font_size);
            doc.cvt_units(_css_depth, _font_size);

            _css_min_width.fromString(get_style_property("min-width", false, "0"));
            _css_min_height.fromString(get_style_property("min-height", false, "0"));
            _css_min_depth.fromString(get_style_property("min-depth", false, "0")); //:h3ml
            _css_max_width.fromString(get_style_property("max-width", false, "none"), "none");
            _css_max_height.fromString(get_style_property("max-height", false, "none"), "none");
            _css_max_depth.fromString(get_style_property("max-depth", false, "none"), "none"); //:h3ml
            doc.cvt_units(_css_min_width, _font_size);
            doc.cvt_units(_css_min_height, _font_size);
            doc.cvt_units(_css_min_depth, _font_size);

            _css_offsets.left.fromString(get_style_property("left", false, "auto"), "auto");
            _css_offsets.right.fromString(get_style_property("right", false, "auto"), "auto");
            _css_offsets.top.fromString(get_style_property("top", false, "auto"), "auto");
            _css_offsets.bottom.fromString(get_style_property("bottom", false, "auto"), "auto");
            _css_offsets.front.fromString(get_style_property("front", false, "auto"), "auto"); //:h3ml
            _css_offsets.back.fromString(get_style_property("back", false, "auto"), "auto"); //:h3ml
            doc.cvt_units(_css_offsets.left, _font_size);
            doc.cvt_units(_css_offsets.right, _font_size);
            doc.cvt_units(_css_offsets.top, _font_size);
            doc.cvt_units(_css_offsets.bottom, _font_size);
            doc.cvt_units(_css_offsets.front, _font_size); //:h3ml
            doc.cvt_units(_css_offsets.back, _font_size); //:h3ml

            _css_margins.left.fromString(get_style_property("margin-left", false, "0"), "auto");
            _css_margins.right.fromString(get_style_property("margin-right", false, "0"), "auto");
            _css_margins.top.fromString(get_style_property("margin-top", false, "0"), "auto");
            _css_margins.bottom.fromString(get_style_property("margin-bottom", false, "0"), "auto");
            _css_margins.front.fromString(get_style_property("margin-front", false, "0"), "auto"); //:h3ml
            _css_margins.back.fromString(get_style_property("margin-back", false, "0"), "auto"); //:h3ml

            _css_padding.left.fromString(get_style_property("padding-left", false, "0"), "");
            _css_padding.right.fromString(get_style_property("padding-right", false, "0"), "");
            _css_padding.top.fromString(get_style_property("padding-top", false, "0"), "");
            _css_padding.bottom.fromString(get_style_property("padding-bottom", false, "0"), "");
            _css_padding.front.fromString(get_style_property("padding-front", false, "0"), ""); //:h3ml
            _css_padding.back.fromString(get_style_property("padding-back", false, "0"), ""); //:h3ml

            _css_borders.left.width.fromString(get_style_property("border-left-width", false, "medium"), types.border_width_strings);
            _css_borders.right.width.fromString(get_style_property("border-right-width", false, "medium"), types.border_width_strings);
            _css_borders.top.width.fromString(get_style_property("border-top-width", false, "medium"), types.border_width_strings);
            _css_borders.bottom.width.fromString(get_style_property("border-bottom-width", false, "medium"), types.border_width_strings);
            _css_borders.front.width.fromString(get_style_property("border-front-width", false, "medium"), types.border_width_strings); //:h3ml
            _css_borders.back.width.fromString(get_style_property("border-back-width", false, "medium"), types.border_width_strings); //:h3ml

            _css_borders.left.color = web_color.from_string(get_style_property("border-left-color", false, ""), doc.container);
            _css_borders.left.style = (border_style)html.value_index(get_style_property("border-left-style", false, "none"), types.border_style_strings, (int)border_style.none);

            _css_borders.right.color = web_color.from_string(get_style_property("border-right-color", false, ""), doc.container);
            _css_borders.right.style = (border_style)html.value_index(get_style_property("border-right-style", false, "none"), types.border_style_strings, (int)border_style.none);

            _css_borders.top.color = web_color.from_string(get_style_property("border-top-color", false, ""), doc.container);
            _css_borders.top.style = (border_style)html.value_index(get_style_property("border-top-style", false, "none"), types.border_style_strings, (int)border_style.none);

            _css_borders.bottom.color = web_color.from_string(get_style_property("border-bottom-color", false, ""), doc.container);
            _css_borders.bottom.style = (border_style)html.value_index(get_style_property("border-bottom-style", false, "none"), types.border_style_strings, (int)border_style.none);

            _css_borders.front.color = web_color.from_string(get_style_property("border-front-color", false, ""), doc.container); //:h3ml
            _css_borders.front.style = (border_style)html.value_index(get_style_property("border-front-style", false, "none"), types.border_style_strings, (int)border_style.none); //:h3ml

            _css_borders.back.color = web_color.from_string(get_style_property("border-back-color", false, ""), doc.container); //:h3ml
            _css_borders.back.style = (border_style)html.value_index(get_style_property("border-back-style", false, "none"), types.border_style_strings, (int)border_style.none); //:h3ml

            _css_borders.radius.top_left_x.fromString(get_style_property("border-top-left-radius-x", false, "0"));
            _css_borders.radius.top_left_y.fromString(get_style_property("border-top-left-radius-y", false, "0"));
            _css_borders.radius.top_left_z.fromString(get_style_property("border-top-left-radius-z", false, "0")); //:h3ml

            _css_borders.radius.top_right_x.fromString(get_style_property("border-top-right-radius-x", false, "0"));
            _css_borders.radius.top_right_y.fromString(get_style_property("border-top-right-radius-y", false, "0"));
            _css_borders.radius.top_right_z.fromString(get_style_property("border-top-right-radius-z", false, "0")); //:h3ml

            _css_borders.radius.bottom_right_x.fromString(get_style_property("border-bottom-right-radius-x", false, "0"));
            _css_borders.radius.bottom_right_y.fromString(get_style_property("border-bottom-right-radius-y", false, "0"));
            _css_borders.radius.bottom_right_z.fromString(get_style_property("border-bottom-right-radius-z", false, "0")); //:h3ml

            _css_borders.radius.bottom_left_x.fromString(get_style_property("border-bottom-left-radius-x", false, "0"));
            _css_borders.radius.bottom_left_y.fromString(get_style_property("border-bottom-left-radius-y", false, "0"));
            _css_borders.radius.bottom_left_z.fromString(get_style_property("border-bottom-left-radius-z", false, "0")); //:h3ml

            doc.cvt_units(_css_borders.radius.bottom_left_x, _font_size);
            doc.cvt_units(_css_borders.radius.bottom_left_y, _font_size);
            doc.cvt_units(_css_borders.radius.bottom_left_z, _font_size); //:h3ml
            doc.cvt_units(_css_borders.radius.bottom_right_x, _font_size);
            doc.cvt_units(_css_borders.radius.bottom_right_y, _font_size);
            doc.cvt_units(_css_borders.radius.bottom_right_z, _font_size); //:h3ml
            doc.cvt_units(_css_borders.radius.top_left_x, _font_size);
            doc.cvt_units(_css_borders.radius.top_left_y, _font_size);
            doc.cvt_units(_css_borders.radius.top_left_z, _font_size); //:h3ml
            doc.cvt_units(_css_borders.radius.top_right_x, _font_size);
            doc.cvt_units(_css_borders.radius.top_right_y, _font_size);
            doc.cvt_units(_css_borders.radius.top_right_z, _font_size); //:h3ml

            doc.cvt_units(_css_text_indent, _font_size);

            _margins.left = doc.cvt_units(_css_margins.left, _font_size);
            _margins.right = doc.cvt_units(_css_margins.right, _font_size);
            _margins.top = doc.cvt_units(_css_margins.top, _font_size);
            _margins.bottom = doc.cvt_units(_css_margins.bottom, _font_size);
            _margins.front = doc.cvt_units(_css_margins.front, _font_size); //:h3ml
            _margins.back = doc.cvt_units(_css_margins.back, _font_size); //:h3ml

            _padding.left = doc.cvt_units(_css_padding.left, _font_size);
            _padding.right = doc.cvt_units(_css_padding.right, _font_size);
            _padding.top = doc.cvt_units(_css_padding.top, _font_size);
            _padding.bottom = doc.cvt_units(_css_padding.bottom, _font_size);
            _padding.front = doc.cvt_units(_css_padding.front, _font_size); //:h3ml
            _padding.back = doc.cvt_units(_css_padding.back, _font_size); //:h3ml

            _borders.left = doc.cvt_units(_css_borders.left.width, _font_size);
            _borders.right = doc.cvt_units(_css_borders.right.width, _font_size);
            _borders.top = doc.cvt_units(_css_borders.top.width, _font_size);
            _borders.bottom = doc.cvt_units(_css_borders.bottom.width, _font_size);
            _borders.front = doc.cvt_units(_css_borders.front.width, _font_size); //:h3ml
            _borders.back = doc.cvt_units(_css_borders.back.width, _font_size); //:h3ml

            var line_height = new css_length();
            line_height.fromString(get_style_property("line-height", true, "normal"), "normal");
            if (line_height.is_predefined)
            {
                _line_height = _font_metrics.height;
                _lh_predefined = true;
            }
            else if (line_height.units == css_units.none)
            {
                _line_height = (int)(line_height.val * _font_size);
                _lh_predefined = false;
            }
            else
            {
                _line_height = doc.cvt_units(line_height, _font_size, _font_size);
                _lh_predefined = false;
            }

            if (_display == style_display.list_item)
            {
                var list_type = get_style_property("list-style-type", true, "disc");
                _list_style_type = (list_style_type)html.value_index(list_type, types.list_style_type_strings, (int)list_style_type.disc);

                var list_pos = get_style_property("list-style-position", true, "outside");
                _list_style_position = (list_style_position)html.value_index(list_pos, types.list_style_position_strings, (int)list_style_position.outside);

                var list_image = get_style_property("list-style-image", true, null);
                if (!string.IsNullOrEmpty(list_image))
                {
                    css.parse_css_url(list_image, out var url);
                    var list_image_baseurl = get_style_property("list-style-image-baseurl", true, null);
                    doc.container.load_image(url, list_image_baseurl, true);
                }
            }

            parse_background();
            if (!is_reparse)
                foreach (var el in _children)
                    el.parse_styles();
        }

        public override void draw(object hdc, int x, int y, int z, position clip)
        {
            var pos = _pos;
            pos.x += x;
            pos.y += y;
            pos.z += z; //:h3ml
            draw_background(hdc, x, y, z, clip); //:h3ml
            if (_display == style_display.list_item && _list_style_type != list_style_type.none)
            {
                if (_overflow > overflow.visible)
                {
                    var border_box = pos;
                    border_box += _padding;
                    border_box += _borders;
                    var bdr_radius = _css_borders.radius.calc_percents(border_box.width, border_box.height, border_box.depth); //:h3ml
                    bdr_radius -= _borders;
                    bdr_radius -= _padding;
                    get_document().container.set_clip(pos, bdr_radius, true, true);
                }
                draw_list_marker(hdc, pos);
                if (_overflow > overflow.visible)
                    get_document().container.del_clip();
            }
        }

        public override void draw_background(object hdc, int x, int y, int z, position clip)
        {
            var pos = _pos;
            pos.x += x;
            pos.y += y;
            pos.z += z; //:h3ml
            var el_pos = pos;
            el_pos += _padding;
            el_pos += _borders;
            if (_display != style_display.inline && _display != style_display.table_row)
            {
                if (el_pos.does_intersect(clip))
                {
                    var bg = get_background();
                    if (bg != null)
                    {
                        var bg_paint = new background_paint();
                        init_background_paint(pos, bg_paint, bg);
                        get_document().container.draw_background(hdc, bg_paint);
                    }
                    var border_box = pos;
                    border_box += _padding;
                    border_box += _borders;
                    var bdr = new borders(_css_borders);
                    bdr.radius = _css_borders.radius.calc_percents(border_box.width, border_box.height, border_box.depth); //:h3ml
                    get_document().container.draw_borders(hdc, bdr, border_box, !have_parent);
                }
            }
            else
            {
                var bg = get_background();
                var boxes = new List<position>();
                get_inline_boxes(boxes);

                var bg_paint = new background_paint();
                position content_box;

                for (var i = 0; i < boxes.Count; i++)
                {
                    var box = boxes[i];
                    box.x += x;
                    box.y += y;
                    box.z += z; //:h3ml

                    if (box.does_intersect(clip))
                    {
                        content_box = box;
                        content_box -= _borders;
                        content_box -= _padding;
                        if (bg != null)
                            init_background_paint(content_box, bg_paint, bg);

                        var bdr = new css_borders();

                        // set left borders radius for the first box
                        if (i == 0)
                        {
                            bdr.radius.bottom_left_x = _css_borders.radius.bottom_left_x;
                            bdr.radius.bottom_left_y = _css_borders.radius.bottom_left_y;
                            bdr.radius.bottom_left_z = _css_borders.radius.bottom_left_z; //:h3ml
                            bdr.radius.top_left_x = _css_borders.radius.top_left_x;
                            bdr.radius.top_left_y = _css_borders.radius.top_left_y;
                            bdr.radius.top_left_z = _css_borders.radius.top_left_z; //:h3ml
                        }

                        // set right borders radius for the last box
                        if (i == boxes.Count - 1)
                        {
                            bdr.radius.bottom_right_x = _css_borders.radius.bottom_right_x;
                            bdr.radius.bottom_right_y = _css_borders.radius.bottom_right_y;
                            bdr.radius.bottom_right_z = _css_borders.radius.bottom_right_z; //:h3ml
                            bdr.radius.top_right_x = _css_borders.radius.top_right_x;
                            bdr.radius.top_right_y = _css_borders.radius.top_right_y;
                            bdr.radius.top_right_z = _css_borders.radius.top_right_z; //:h3ml
                        }

                        bdr.top = _css_borders.top;
                        bdr.bottom = _css_borders.bottom;
                        if (i == 0)
                            bdr.left = _css_borders.left;
                        if (i == boxes.Count - 1)
                            bdr.right = _css_borders.right;

                        if (bg != null)
                        {
                            bg_paint.border_radius = bdr.radius.calc_percents(bg_paint.border_box.width, bg_paint.border_box.width, bg_paint.border_box.depth); //:h3ml
                            get_document().container.draw_background(hdc, bg_paint);
                        }
                        var b = new borders(bdr);
                        b.radius = bdr.radius.calc_percents(box.width, box.height, box.depth); //:h3ml
                        get_document().container.draw_borders(hdc, b, box, false);
                    }
                }
            }
        }

        public override string get_style_property(string name, bool inherited, string def = null)
        {
            var ret = _style.get_property(name);
            var el_parent = parent();
            if (el_parent != null)
                if ((ret != null && string.Equals(ret, "inherit", StringComparison.OrdinalIgnoreCase)) || (ret == null && inherited))
                    ret = el_parent.get_style_property(name, inherited, def);
            if (ret == null)
                ret = def;
            return ret;
        }

        public override object get_font(out font_metrics fm)
        {
            fm = _font_metrics;
            return _font;
        }

        public override int get_font_size => _font_size;

        public IList<element> children() => _children;

        public override void calc_outlines(int parent_width)
        {
            _padding.left = _css_padding.left.calc_percent(parent_width);
            _padding.right = _css_padding.right.calc_percent(parent_width);
            _borders.left = _css_borders.left.width.calc_percent(parent_width);
            _borders.right = _css_borders.right.width.calc_percent(parent_width);
            _margins.left = _css_margins.left.calc_percent(parent_width);
            _margins.right = _css_margins.right.calc_percent(parent_width);

            _margins.top = _css_margins.top.calc_percent(parent_width);
            _margins.bottom = _css_margins.bottom.calc_percent(parent_width);
            _padding.top = _css_padding.top.calc_percent(parent_width);
            _padding.bottom = _css_padding.bottom.calc_percent(parent_width);

            _margins.front = _css_margins.front.calc_percent(parent_width); //:h3ml
            _margins.back = _css_margins.back.calc_percent(parent_width); //:h3ml
            _padding.front = _css_padding.front.calc_percent(parent_width); //:h3ml
            _padding.back = _css_padding.back.calc_percent(parent_width); //:h3ml
        }

        public override void calc_auto_margins(int parent_width)
        {
            if (get_element_position(out var junk) != element_position.absolute && (_display == style_display.block || _display == style_display.table))
            {
                if (_css_margins.left.is_predefined && _css_margins.right.is_predefined)
                {
                    var el_width = _pos.width + _borders.left + _borders.right + _padding.left + _padding.right;
                    if (el_width <= parent_width)
                    {
                        _margins.left = (parent_width - el_width) / 2;
                        _margins.right = (parent_width - el_width) - _margins.left;
                    }
                    else
                    {
                        _margins.left = 0;
                        _margins.right = 0;
                    }
                }
                else if (_css_margins.left.is_predefined && !_css_margins.right.is_predefined)
                {
                    var el_width = _pos.width + _borders.left + _borders.right + _padding.left + _padding.right + _margins.right;
                    _margins.left = parent_width - el_width;
                    if (_margins.left < 0) _margins.left = 0;
                }
                else if (!_css_margins.left.is_predefined && _css_margins.right.is_predefined)
                {
                    var el_width = _pos.width + _borders.left + _borders.right + _padding.left + _padding.right + _margins.left;
                    _margins.right = parent_width - el_width;
                    if (_margins.right < 0) _margins.right = 0;
                }
            }
        }

        public override select_result select(css_selector selector, bool apply_pseudo = true)
        {
            var right_res = select(selector._right, apply_pseudo);
            if (right_res == select_result.no_match)
                return select_result.no_match;
            var el_parent = parent();
            if (selector._left != null)
            {
                if (el_parent == null)
                    return select_result.no_match;
                switch (selector._combinator)
                {
                    case css_combinator.descendant:
                        {
                            var res = find_ancestor(selector._left, out var is_pseudo, apply_pseudo);
                            if (res == null)
                                return select_result.no_match;
                            else if (is_pseudo)
                                right_res |= select_result.match_pseudo_class;
                        }
                        break;
                    case css_combinator.child:
                        {
                            var res = el_parent.select(selector._left, apply_pseudo);
                            if (res == select_result.no_match)
                                return select_result.no_match;
                            else if (right_res != select_result.match_pseudo_class)
                                right_res |= res;
                        }
                        break;
                    case css_combinator.adjacent_sibling:
                        {
                            var res = el_parent.find_adjacent_sibling(this, selector._left, out var is_pseudo, apply_pseudo);
                            if (res == null)
                                return select_result.no_match;
                            else if (is_pseudo)
                                right_res |= select_result.match_pseudo_class;
                        }
                        break;
                    case css_combinator.general_sibling:
                        {
                            var res = el_parent.find_sibling(this, selector._left, out var is_pseudo, apply_pseudo);
                            if (res == null)
                                return select_result.no_match;
                            else if (is_pseudo)
                                right_res |= select_result.match_pseudo_class;
                        }
                        break;
                    default: right_res = select_result.no_match; break;
                }
            }
            return right_res;
        }

        public override select_result select(css_element_selector selector, bool apply_pseudo = true)
        {
            if (!string.IsNullOrEmpty(selector._tag) && selector._tag != "*")
                if (selector._tag != _tag)
                    return select_result.no_match;
            var res = select_result.match;
            var el_parent = parent();
            foreach (var i in selector._attrs)
            {
                var attr_value = get_attr(i.attribute);
                switch (i.condition)
                {
                    case attr_select_condition.exists:
                        if (attr_value == null) return select_result.no_match;
                        break;
                    case attr_select_condition.equal:
                        if (attr_value == null) return select_result.no_match;
                        else
                        {
                            if (i.attribute == "class")
                            {
                                var tokens1 = _class_values;
                                var tokens2 = i.class_val;
                                var found = true;
                                foreach (var str1 in tokens2)
                                {
                                    var f = false;
                                    foreach (var str2 in tokens1)
                                        if (string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase))
                                        {
                                            f = true;
                                            break;
                                        }
                                    if (!f)
                                    {
                                        found = false;
                                        break;
                                    }
                                }
                                if (!found) return select_result.no_match;
                            }
                            else if (!string.Equals(i.val, attr_value, StringComparison.OrdinalIgnoreCase)) return select_result.no_match;
                        }
                        break;
                    case attr_select_condition.contain_str:
                        if (attr_value == null) return select_result.no_match;
                        else if (attr_value != i.val) return select_result.no_match;
                        break;
                    case attr_select_condition.start_str:
                        if (attr_value == null) return select_result.no_match;
                        else if (!attr_value.StartsWith(i.val)) return select_result.no_match;
                        break;
                    case attr_select_condition.end_str:
                        if (attr_value == null) return select_result.no_match;
                        else if (!attr_value.StartsWith(i.val))
                        {
                            var sIdx = attr_value.Length - i.val.Length - 1;
                            if (sIdx < 0) return select_result.no_match;
                            var s = attr_value.Substring(sIdx);
                            if (i.val != s) return select_result.no_match;
                        }
                        break;
                    case attr_select_condition.pseudo_element:
                        if (i.val == "after") res |= select_result.match_with_after;
                        else if (i.val == "before") res |= select_result.match_with_before;
                        else return select_result.no_match;
                        break;
                    case attr_select_condition.pseudo_class:
                        if (apply_pseudo)
                        {
                            if (el_parent == null) return select_result.no_match;
                            var begin = i.val.IndexOf('(');
                            var end = begin == -1 ? -1 : html.find_close_bracket(i.val, begin);
                            var selector_param = begin != -1 && end != -1 ? i.val.Substring(begin + 1, end - begin - 1) : string.Empty;
                            var selector_name = begin != -1 ? i.val.Substring(0, begin).Trim() : i.val;
                            var selector2 = (pseudo_class)html.value_index(selector_name, types.pseudo_class_strings);
                            switch (selector2)
                            {
                                case pseudo_class.only_child: if (!el_parent.is_only_child(this, false)) return select_result.no_match; break;
                                case pseudo_class.only_of_type: if (!el_parent.is_only_child(this, true)) return select_result.no_match; break;
                                case pseudo_class.first_child: if (!el_parent.is_nth_child(this, 0, 1, false)) return select_result.no_match; break;
                                case pseudo_class.first_of_type: if (!el_parent.is_nth_child(this, 0, 1, true)) return select_result.no_match; break;
                                case pseudo_class.last_child: if (!el_parent.is_nth_last_child(this, 0, 1, false)) return select_result.no_match; break;
                                case pseudo_class.last_of_type: if (!el_parent.is_nth_last_child(this, 0, 1, true)) return select_result.no_match; break;
                                case pseudo_class.nth_child:
                                case pseudo_class.nth_of_type:
                                case pseudo_class.nth_last_child:
                                case pseudo_class.nth_last_of_type:
                                    {
                                        if (string.IsNullOrEmpty(selector_param)) return select_result.no_match;
                                        parse_nth_child_params(selector_param, out var num, out var off);
                                        if (num == 0 && off == 0) return select_result.no_match;
                                        switch (selector2)
                                        {
                                            case pseudo_class.nth_child: if (!el_parent.is_nth_child(this, num, off, false)) return select_result.no_match; break;
                                            case pseudo_class.nth_of_type: if (!el_parent.is_nth_child(this, num, off, true)) return select_result.no_match; break;
                                            case pseudo_class.nth_last_child: if (!el_parent.is_nth_last_child(this, num, off, false)) return select_result.no_match; break;
                                            case pseudo_class.nth_last_of_type: if (!el_parent.is_nth_last_child(this, num, off, true)) return select_result.no_match; break;
                                        }
                                    }
                                    break;
                                case pseudo_class.not:
                                    {
                                        var sel = new css_element_selector();
                                        sel.parse(selector_param);
                                        if (select(sel, apply_pseudo) != 0) return select_result.no_match;
                                    }
                                    break;
                                case pseudo_class.lang: if (!get_document().match_lang(selector_param.Trim())) return select_result.no_match; break;
                                default: if (!_pseudo_classes.Contains(i.val)) return select_result.no_match; break;
                            }
                        }
                        else res |= select_result.match_pseudo_class;
                        break;
                }
            }
            return res;
        }

        public override IList<element> select_all(string selector)
        {
            var sel = new css_selector((media_query_list)null);
            sel.parse(selector);
            return select_all(sel);
        }

        public override IList<element> select_all(css_selector selector)
        {
            var res = new List<element>();
            select_all(selector, res);
            return res;
        }

        protected override void select_all(css_selector selector, IList<element> res)
        {
            if (select(selector) != 0)
                res.Add(this);
            foreach (html_tag el in _children)
                el.select_all(selector, res);
        }

        public override element select_one(string selector)
        {
            var sel = new css_selector((media_query_list)null);
            sel.parse(selector);
            return select_one(sel);
        }

        public override element select_one(css_selector selector)
        {
            if (select(selector) != 0)
                return this;
            foreach (var el in _children)
            {
                var res = el.select_one(selector);
                if (res != null)
                    return res;
            }
            return null;
        }

        public override element find_ancestor(css_selector selector, out bool is_pseudo, bool apply_pseudo = true)
        {
            var el_parent = parent();
            if (el_parent == null)
            {
                is_pseudo = false;
                return null;
            }
            var res = el_parent.select(selector, apply_pseudo);
            if (res != select_result.no_match)
            {
                is_pseudo = (res & select_result.match_pseudo_class) != 0;
                return el_parent;
            }
            return el_parent.find_ancestor(selector, out is_pseudo, apply_pseudo);
        }

        public override element find_adjacent_sibling(element el, css_selector selector, out bool is_pseudo, bool apply_pseudo = true)
        {
            is_pseudo = false;
            element ret = null;
            foreach (var e in _children)
                if (e.get_display != style_display.inline_text)
                {
                    if (e == el)
                    {
                        if (ret != null)
                        {
                            var res = ret.select(selector, apply_pseudo);
                            if (res != select_result.no_match)
                            {
                                is_pseudo = (res & select_result.match_pseudo_class) != 0;
                                return ret;
                            }
                        }
                        return null;
                    }
                    else ret = e;
                }
            return null;
        }

        public override element find_sibling(element el, css_selector selector, out bool is_pseudo, bool apply_pseudo = true)
        {
            is_pseudo = false;
            element ret = null;
            foreach (var e in _children)
                if (e.get_display != style_display.inline_text)
                {
                    if (e == el)
                        return ret;
                    else if (ret == null)
                    {
                        var res = e.select(selector, apply_pseudo);
                        if (res != select_result.no_match)
                        {
                            is_pseudo = (res & select_result.match_pseudo_class) != 0;
                            ret = e;
                        }
                    }
                }
            return null;
        }

        public override void get_text(ref string text)
        {
            foreach (var el in _children)
                el.get_text(ref text);
        }

        public override void parse_attributes()
        {
            foreach (var el in _children)
                el.parse_attributes();
        }

        public override bool is_first_child_inline(element el)
        {
            if (_children.Count != 0)
                foreach (var this_el in _children)
                    if (!this_el.is_white_space())
                    {
                        if (el == this_el)
                            return true;
                        if (this_el.get_display == style_display.inline)
                        {
                            if (this_el.have_inline_child())
                                return false;
                        }
                        else return false;
                    }
            return false;
        }

        public override bool is_last_child_inline(element el)
        {
            if (_children.Count != 0)
                foreach (var this_el in _children)
                    if (!this_el.is_white_space())
                    {
                        if (el == this_el)
                            return true;
                        if (this_el.get_display == style_display.inline)
                        {
                            if (this_el.have_inline_child())
                                return false;
                        }
                        else return false;
                    }
            return false;
        }

        public override bool have_inline_child()
        {
            if (_children.Count != 0)
                foreach (var el in _children)
                    if (!el.is_white_space())
                        return true;
            return false;
        }

        public override void get_content_size(out size sz, int max_width)
        {
            sz.height = 0;
            sz.width = _display == style_display.block ? max_width : 0;
            sz.depth = 0;
        }

        public override void init()
        {
            if (_display == style_display.table || _display == style_display.inline_table)
            {
                if (_grid != null) _grid.clear();
                else _grid = new table_grid();
                var table_selector = new go_inside_table();
                var row_selector = new table_rows_selector();
                var cell_selector = new table_cells_selector();
                var row_iter = new elements_iterator(this, table_selector, row_selector);
                var row = row_iter.next(false);
                while (row != null)
                {
                    _grid.begin_row(row);
                    var cell_iter = new elements_iterator(row, table_selector, cell_selector);
                    var cell = cell_iter.next();
                    while (cell != null)
                    {
                        _grid.add_cell(cell);
                        cell = cell_iter.next(false);
                    }
                    row = row_iter.next(false);
                }
                _grid.finish();
            }
            foreach (var el in _children)
                el.init();
        }

        public override void get_inline_boxes(IList<position> boxes)
        {
            box old_box = null;
            var pos = new position();
            foreach (var el in _children)
                if (!el.skip)
                    if (el._box != null)
                    {
                        if (el._box != old_box)
                        {
                            if (old_box != null)
                            {
                                if (boxes.Count == 0)
                                {
                                    pos.x -= _padding.left + _borders.left;
                                    pos.width += _padding.left + _borders.left;
                                }
                                boxes.Add(pos);
                            }
                            old_box = el._box;
                            pos.x = el.left + el.margin_left;
                            pos.y = el.top - _padding.top - _borders.top;
                            pos.z = el.front - _padding.front - _borders.front; //:h3ml
                            pos.width = 0;
                            pos.height = 0;
                            pos.depth = 0; //:h3ml
                        }
                        pos.width = el.right - pos.x - el.margin_right - el.margin_left;
                        pos.height = Math.Max(pos.height, el.height + _padding.top + _padding.bottom + _borders.top + _borders.bottom);
                        pos.depth = Math.Max(pos.depth, el.depth + _padding.front + _padding.back + _borders.front + _borders.back); //:h3ml
                    }
                    else if (el.get_display == style_display.inline)
                    {
                        var sub_boxes = new List<position>();
                        el.get_inline_boxes(sub_boxes);
                        if (sub_boxes.Count != 0)
                        {
                            var p = sub_boxes[sub_boxes.Count - 1]; p.width += el.margin_right; sub_boxes[sub_boxes.Count - 1] = p;
                            if (boxes.Count == 0)
                                if (_padding.left + _borders.left > 0)
                                {
                                    var padding_box = sub_boxes.First();
                                    padding_box.x -= _padding.left + _borders.left + el.margin_left;
                                    padding_box.width = _padding.left + _borders.left + el.margin_left;
                                    boxes.Add(padding_box);
                                }
                            p = sub_boxes[sub_boxes.Count - 1]; p.width += el.margin_right; sub_boxes[sub_boxes.Count - 1] = p;
                            ((List<position>)boxes).AddRange(sub_boxes);
                        }

                    }
            if (pos.width != 0 || pos.height != 0 || pos.depth != 0) //:h3ml
            {
                if (boxes.Count == 0)
                {
                    pos.x -= _padding.left + _borders.left;
                    pos.width += _padding.left + _borders.left;
                }
                boxes.Add(pos);
            }
            if (boxes.Count != 0)
                if (_padding.right + _borders.right > 0)
                {
                    var p = boxes[boxes.Count - 1]; p.width += _padding.right + _borders.right; boxes[boxes.Count - 1] = p;
                }
        }

        public override bool is_floats_holder =>
            _display == style_display.inline_block ||
            _display == style_display.table_cell ||
            !have_parent ||
            is_body() ||
            _float != element_float.none ||
            _el_position == element_position.absolute ||
            _el_position == element_position.@fixed ||
            _overflow > overflow.visible;

        public override int get_floats_height(element_float el_float = element_float.none)
        {
            if (is_floats_holder)
            {
                var h = 0;
                var process = false;
                foreach (var fb in _floats_left)
                {
                    process = false;
                    switch (el_float)
                    {
                        case element_float.none: process = true; break;
                        case element_float.left: if (fb.clear_floats == element_clear.left || fb.clear_floats == element_clear.both) process = true; break;
                        case element_float.right: if (fb.clear_floats == element_clear.right || fb.clear_floats == element_clear.both) process = true; break;
                    }
                    if (process)
                        h = el_float == element_float.none ? Math.Max(h, fb.pos.bottom) : Math.Max(h, fb.pos.top);
                }
                foreach (var fb in _floats_right)
                {
                    process = false;
                    switch (el_float)
                    {
                        case element_float.none: process = true; break;
                        case element_float.left: if (fb.clear_floats == element_clear.left || fb.clear_floats == element_clear.both) process = true; break;
                        case element_float.right: if (fb.clear_floats == element_clear.right || fb.clear_floats == element_clear.both) process = true; break;
                    }
                    if (process)
                        h = el_float == element_float.none ? Math.Max(h, fb.pos.bottom) : Math.Max(h, fb.pos.top);
                }
                return h;
            }
            var el_parent = parent();
            if (el_parent != null)
            {
                var h = el_parent.get_floats_height(el_float);
                return h - _pos.y;
            }
            return 0;
        }

        public override int get_left_floats_height()
        {
            if (is_floats_holder)
            {
                var h = 0;
                if (_floats_left.Count != 0)
                    foreach (var fb in _floats_left)
                        h = Math.Max(h, fb.pos.bottom);
                return h;
            }
            var el_parent = parent();
            if (el_parent != null)
            {
                var h = el_parent.get_left_floats_height();
                return h - _pos.y;
            }
            return 0;
        }

        public override int get_right_floats_height()
        {
            if (is_floats_holder)
            {
                var h = 0;
                if (_floats_right.Count != 0)
                    foreach (var fb in _floats_right)
                        h = Math.Max(h, fb.pos.bottom);
                return h;
            }
            var el_parent = parent();
            if (el_parent != null)
            {
                var h = el_parent.get_right_floats_height();
                return h - _pos.y;
            }
            return 0;
        }

        public override int get_line_left(int y)
        {
            if (is_floats_holder)
            {
                if (_cahe_line_left.is_valid && _cahe_line_left.hash == y)
                    return _cahe_line_left.val;
                var w = 0;
                foreach (var fb in _floats_left)
                    if (y >= fb.pos.top && y < fb.pos.bottom)
                    {
                        w = Math.Max(w, fb.pos.right);
                        if (w < fb.pos.right)
                            break;
                    }
                _cahe_line_left.set_value(y, w);
                return w;
            }
            var el_parent = parent();
            if (el_parent != null)
            {
                var w = el_parent.get_line_left(y + _pos.y);
                if (w < 0)
                    w = 0;
                return w - (w != 0 ? _pos.x : 0);
            }
            return 0;
        }

        public override int get_line_right(int y, int def_right)
        {
            if (is_floats_holder)
            {
                if (_cahe_line_right.is_valid && _cahe_line_right.hash == y)
                    return _cahe_line_right.is_default ? def_right : Math.Min(_cahe_line_right.val, def_right);
                var w = def_right;
                _cahe_line_right.is_default = true;
                foreach (var fb in _floats_right)
                    if (y >= fb.pos.top && y < fb.pos.bottom)
                    {
                        w = Math.Min(w, fb.pos.left);
                        _cahe_line_right.is_default = false;
                        if (w > fb.pos.left)
                            break;
                    }
                _cahe_line_right.set_value(y, w);
                return w;
            }
            var el_parent = parent();
            if (el_parent != null)
            {
                var w = el_parent.get_line_right(y + _pos.y, def_right + _pos.x);
                return w - _pos.x;
            }
            return 0;
        }

        public override void get_line_left_right(int y, int def_right, ref int ln_left, ref int ln_right)
        {
            if (is_floats_holder)
            {
                ln_left = get_line_left(y);
                ln_right = get_line_right(y, def_right);
            }
            else
            {
                var el_parent = parent();
                if (el_parent != null)
                    el_parent.get_line_left_right(y + _pos.y, def_right + _pos.x, ref ln_left, ref ln_right);
                ln_right -= _pos.x;
                ln_left -= _pos.x;
                if (ln_left < 0)
                    ln_left = 0;
            }
        }

        public override void add_float(element el, int x, int y, int z)
        {
            if (is_floats_holder)
            {
                floated_box fb;
                fb.pos.x = el.left + x;
                fb.pos.y = el.top + y;
                fb.pos.z = el.front + z; //:h3ml
                fb.pos.width = el.width;
                fb.pos.height = el.height;
                fb.pos.depth = el.depth; //:h3ml
                fb.float_side = el.get_float;
                fb.clear_floats = el.get_clear;
                fb.el = el;
                if (fb.float_side == element_float.left)
                {
                    if (_floats_left.Count == 0)
                        _floats_left.Add(fb);
                    else
                    {
                        var inserted = false;
                        for (var ii = 0; ii < _floats_left.Count; ii++)
                        {
                            var i = _floats_left[ii];
                            if (fb.pos.right > i.pos.right)
                            {
                                _floats_left.Insert(ii, fb);
                                inserted = true;
                                break;
                            }
                        }
                        if (!inserted)
                            _floats_left.Add(fb);
                    }
                    _cahe_line_left.invalidate();
                }
                else if (fb.float_side == element_float.right)
                {
                    if (_floats_right.Count == 0)
                        _floats_right.Add(fb);
                    else
                    {
                        var inserted = false;
                        for (var ii = 0; ii < _floats_right.Count; ii++)
                        {
                            var i = _floats_right[ii];
                            if (fb.pos.left < i.pos.left)
                            {
                                _floats_right.Insert(ii, fb);
                                inserted = true;
                                break;
                            }
                        }
                        if (!inserted)
                            _floats_right.Add(fb);
                    }
                    _cahe_line_right.invalidate();
                }
            }
            else
            {
                var el_parent = parent();
                if (el_parent != null)
                    el_parent.add_float(el, x + _pos.x, y + _pos.y, z + _pos.z); //:h3ml
            }
        }

        public override void update_floats(int dy, element parent)
        {
            if (is_floats_holder)
            {
                var reset_cache = false;
                for (var fbI = _floats_left.Count - 1; fbI >= 0; fbI--)
                {
                    var fb = _floats_left[fbI];
                    if (fb.el.is_ancestor(parent))
                    {
                        reset_cache = true;
                        fb.pos.y += dy;
                    }
                }
                if (reset_cache)
                    _cahe_line_left.invalidate();
                reset_cache = false;
                for (var fbI = _floats_right.Count - 1; fbI >= 0; fbI--)
                {
                    var fb = _floats_right[fbI];
                    if (fb.el.is_ancestor(parent))
                    {
                        reset_cache = true;
                        fb.pos.y += dy;
                    }
                }
                if (reset_cache)
                    _cahe_line_right.invalidate();
            }
            else
            {
                var el_parent = this.parent();
                if (el_parent != null)
                    el_parent.update_floats(dy, parent);
            }
        }

        public override void add_positioned(element el)
        {
            if (_el_position != element_position.@static || !have_parent)
                _positioned.Add(el);
            else
            {
                var el_parent = parent();
                if (el_parent != null)
                    el_parent.add_positioned(el);
            }
        }

        public override int find_next_line_top(int top, int width, int def_right)
        {
            if (is_floats_holder)
            {
                var new_top = top;
                var points = new List<int>();
                foreach (var fb in _floats_left)
                {
                    if (fb.pos.top >= top)
                        if (!points.Contains(fb.pos.top))
                            points.Add(fb.pos.top);
                    if (fb.pos.bottom >= top)
                        if (!points.Contains(fb.pos.bottom))
                            points.Add(fb.pos.bottom);
                }
                foreach (var fb in _floats_right)
                {
                    if (fb.pos.top >= top)
                        if (!points.Contains(fb.pos.top))
                            points.Add(fb.pos.top);
                    if (fb.pos.bottom >= top)
                        if (!points.Contains(fb.pos.bottom))
                            points.Add(fb.pos.bottom);
                }
                if (points.Count != 0)
                {
                    points.Sort(); // SKY: check was less
                    new_top = points.Last();
                    foreach (var pt in points)
                    {
                        var pos_left = 0;
                        var pos_right = def_right;
                        get_line_left_right(pt, def_right, ref pos_left, ref pos_right);
                        if (pos_right - pos_left >= width)
                        {
                            new_top = pt;
                            break;
                        }
                    }
                }
                return new_top;
            }
            var el_parent = parent();
            if (el_parent != null)
            {
                var new_top = el_parent.find_next_line_top(top + _pos.y, width, def_right + _pos.x);
                return new_top - _pos.y;
            }
            return 0;
        }

        public override void apply_vertical_align()
        {
            if (_boxes.Count != 0)
            {
                var add = 0;
                var content_height = _boxes.Last().bottom;
                if (_pos.height > content_height)
                {
                    switch (_vertical_align)
                    {
                        case vertical_align.middle: add = (_pos.height - content_height) / 2; break;
                        case vertical_align.bottom: add = _pos.height - content_height; break;
                        default: add = 0; break;
                    }
                }
                if (add != 0)
                    for (var i = 0; i < _boxes.Count; i++)
                        _boxes[i].y_shift(add);
            }
        }

        public override void draw_children(object hdc, int x, int y, int z, position clip, draw_flag flag, int zindex)
        {
            if (_display == style_display.table || _display == style_display.inline_table) draw_children_table(hdc, x, y, z, clip, flag, zindex); //:h3ml
            else draw_children_box(hdc, x, y, z, clip, flag, zindex); //:h3ml
        }

        public override int get_zindex => _z_index;

        public override void draw_stacking_context(object hdc, int x, int y, int z, position clip, bool with_positioned)
        {
            if (!is_visible) return;
            var zindexes = new HashSet<int>();
            if (with_positioned)
            {
                foreach (var i in _positioned)
                    zindexes.Add(i.get_zindex);
                foreach (var idx in zindexes)
                    if (idx < 0)
                        draw_children(hdc, x, y, z, clip, draw_flag.positioned, idx); //:h3ml
            }
            draw_children(hdc, x, y, z, clip, draw_flag.block, 0); //:h3ml
            draw_children(hdc, x, y, z, clip, draw_flag.floats, 0); //:h3ml
            draw_children(hdc, x, y, z, clip, draw_flag.inlines, 0); //:h3ml
            if (with_positioned)
            {
                foreach (var idx in zindexes)
                    if (idx == 0)
                        draw_children(hdc, x, y, z, clip, draw_flag.positioned, idx); //:h3ml
                foreach (var idx in zindexes)
                    if (idx > 0)
                        draw_children(hdc, x, y, z, clip, draw_flag.positioned, idx); //:h3ml
            }
        }

        public override void calc_document_size(ref size sz, int x = 0, int y = 0, int z = 0)
        {
            if (is_visible && _el_position != element_position.@fixed)
            {
                base.calc_document_size(ref sz, x, y, z); //:h3ml
                if (_overflow == overflow.visible)
                    foreach (var el in _children)
                        el.calc_document_size(ref sz, x + _pos.x, y + _pos.y, z + _pos.z); //:h3ml
                // root element (<html>) must to cover entire window
                if (!have_parent)
                {
                    get_document().container.get_client_rect(out var client_pos);
                    _pos.height = Math.Max(sz.height, client_pos.height) - content_margins_top - content_margins_bottom;
                    _pos.width = Math.Max(sz.width, client_pos.width) - content_margins_left - content_margins_right;
                    _pos.depth = Math.Max(sz.depth, client_pos.depth) - content_margins_front - content_margins_back; //:h3ml
                }
            }
        }

        public override void get_redraw_box(ref position pos, int x = 0, int y = 0, int z = 0)
        {
            if (is_visible)
            {
                get_redraw_box(ref pos, x, y, z); //:h3ml
                if (_overflow == overflow.visible)
                    foreach (var el in _children)
                        if (el.get_element_position(out var junk) != element_position.@fixed)
                            el.get_redraw_box(ref pos, x + _pos.x, y + _pos.y, z + _pos.z); //:h3ml
            }
        }

        public override void add_style(style st) => _style.combine(st);

        public override element get_element_by_point(int x, int y, int z, int client_x, int client_y, int client_z)
        {
            if (!is_visible) return null;
            element ret = null;
            var zindexes = new HashSet<int>();
            foreach (var i in _positioned)
                zindexes.Add(i.get_zindex);
            foreach (var idx in zindexes)
                if (idx > 0)
                {
                    ret = get_child_by_point(x, y, z, client_x, client_y, client_z, draw_flag.positioned, idx); //:h3ml
                    if (ret != null) break;
                }
            if (ret != null) return ret;

            foreach (var idx in zindexes)
            {
                if (idx == 0)
                {
                    ret = get_child_by_point(x, y, z, client_x, client_y, client_z, draw_flag.positioned, idx); //:h3ml
                    if (ret != null) break;
                }
            }
            if (ret != null) return ret;

            ret = get_child_by_point(x, y, z, client_x, client_y, client_z, draw_flag.inlines, 0); //:h3ml
            if (ret != null) return ret;

            ret = get_child_by_point(x, y, z, client_x, client_y, client_z, draw_flag.floats, 0); //:h3ml
            if (ret != null) return ret;

            ret = get_child_by_point(x, y, z, client_x, client_y, client_z, draw_flag.block, 0); //:h3ml
            if (ret != null) return ret;

            foreach (var idx in zindexes)
                if (idx < 0)
                {
                    ret = get_child_by_point(x, y, z, client_x, client_y, client_z, draw_flag.positioned, idx); //:h3ml
                    if (ret != null) break;
                }
            if (ret != null) return ret;

            if (_el_position == element_position.@fixed)
            {
                if (is_point_inside(client_x, client_y, client_z)) //:h3ml
                    ret = this;
            }
            else
            {
                if (is_point_inside(x, y, z)) //:h3ml
                    ret = this;
            }
            return ret;
        }

        public override element get_child_by_point(int x, int y, int z, int client_x, int client_y, int client_z, draw_flag flag, int zindex)
        {
            element ret = null;
            if (_overflow > overflow.visible)
                if (!_pos.is_point_inside(x, y, z)) //:h3ml
                    return ret;
            var pos = _pos;
            pos.x = x - pos.x;
            pos.y = y - pos.y;
            pos.z = z - pos.z; //:h3ml
            for (var ii = _children.Count - 1; ii >= 0 && ret == null; ii++)
            {
                var i = _children[ii];
                var el = i;
                if (el.is_visible && el.get_display != style_display.inline_text)
                {
                    switch (flag)
                    {
                        case draw_flag.positioned:
                            if (el.is_positioned && el.get_zindex == zindex)
                            {
                                if (el.get_element_position(out var junk) == element_position.@fixed)
                                {
                                    ret = el.get_element_by_point(client_x, client_y, client_z, client_x, client_y, client_z); //:h3ml
                                    if (ret == null && i.is_point_inside(client_x, client_y, client_z)) //:h3ml
                                        ret = i;
                                }
                                else
                                {
                                    ret = el.get_element_by_point(pos.x, pos.y, pos.z, client_x, client_y, client_z); //:h3ml
                                    if (ret == null && i.is_point_inside(pos.x, pos.y, pos.z)) //:h3ml
                                        ret = i;
                                }
                                el = null;
                            }
                            break;
                        case draw_flag.block:
                            if (!el.is_inline_box && el.get_float == element_float.none && !el.is_positioned)
                                if (el.is_point_inside(pos.x, pos.y, pos.z)) //:h3ml
                                    ret = el;
                            break;
                        case draw_flag.floats:
                            if (el.get_float != element_float.none && !el.is_positioned)
                            {
                                ret = el.get_element_by_point(pos.x, pos.y, pos.z, client_x, client_y, client_z); //:h3ml
                                if (ret == null && i.is_point_inside(pos.x, pos.y, pos.z)) //:h3ml
                                    ret = i;
                                el = null;
                            }
                            break;
                        case draw_flag.inlines:
                            if (el.is_inline_box && el.get_float == element_float.none && !el.is_positioned)
                            {
                                if (el.get_display == style_display.inline_block)
                                {
                                    ret = el.get_element_by_point(pos.x, pos.y, pos.z, client_x, client_y, client_z); //:h3ml
                                    el = null;
                                }
                                if (ret == null && i.is_point_inside(pos.x, pos.y, pos.z)) //:h3ml
                                    ret = i;
                            }
                            break;
                        default: break;
                    }

                    if (el != null && !el.is_positioned)
                    {
                        if (flag == draw_flag.positioned)
                        {
                            var child = el.get_child_by_point(pos.x, pos.y, pos.z, client_x, client_y, client_z, flag, zindex); //:h3ml
                            if (child != null)
                                ret = child;
                        }
                        else if (el.get_float == element_float.none && el.get_display != style_display.inline_block)
                        {
                            var child = el.get_child_by_point(pos.x, pos.y, pos.z, client_x, client_y, client_z, flag, zindex); //:h3ml
                            if (child != null)
                                ret = child;
                        }
                    }
                }
            }
            return ret;
        }

        public override bool is_nth_child(element el, int num, int off, bool of_type)
        {
            var idx = 1;
            foreach (var child in _children)
                if (child.get_display != style_display.inline_text)
                {
                    if (!of_type || (of_type && el.get_tagName() == child.get_tagName()))
                    {
                        if (el == child)
                        {
                            if (num != 0)
                            {
                                if ((idx - off) >= 0 && (idx - off) % num == 0)
                                    return true;
                            }
                            else if (idx == off)
                                return true;
                            return false;
                        }
                        idx++;
                    }
                    if (el == child) break;
                }
            return false;
        }

        public override bool is_nth_last_child(element el, int num, int off, bool of_type)
        {
            var idx = 1;
            for (var childi = _children.Count - 1; childi >= 0; childi++)
            {
                var child = _children[childi];
                if (child.get_display != style_display.inline_text)
                {
                    if (!of_type || (of_type && el.get_tagName() == child.get_tagName()))
                    {
                        if (el == child)
                        {
                            if (num != 0)
                            {
                                if ((idx - off) >= 0 && (idx - off) % num == 0)
                                    return true;
                            }
                            else if (idx == off)
                                return true;
                            return false;
                        }
                        idx++;
                    }
                    if (el == child) break;
                }
            }
            return false;
        }

        public override bool is_only_child(element el, bool of_type)
        {
            var child_count = 0;
            foreach (var child in _children)
                if (child.get_display != style_display.inline_text)
                {
                    if (!of_type || (of_type && el.get_tagName() == child.get_tagName()))
                        child_count++;
                    if (child_count > 1) break;
                }
            if (child_count > 1)
                return false;
            return true;
        }

        public override background get_background(bool own_only = false)
        {
            if (own_only)
            {
                // return own background with check for empty one
                if (string.IsNullOrEmpty(_bg._image) && _bg._color.alpha == 0)
                    return null;
                return _bg;
            }

            if (string.IsNullOrEmpty(_bg._image) && _bg._color.alpha == 0)
            {
                // if this is root element (<html>) try to get background from body
                if (!have_parent)
                    foreach (var el in _children)
                        if (el.is_body())
                            // return own body background
                            return el.get_background(true);
                return null;
            }

            if (is_body())
            {
                var el_parent = parent();
                if (el_parent != null)
                    if (el_parent.get_background(true) == null)
                        // parent of body will draw background for body
                        return null;
            }
            return _bg;
        }

        protected void draw_children_box(object hdc, int x, int y, int z, position clip, draw_flag flag, int zindex) //:h3ml
        {
            var pos = _pos;
            pos.x += x;
            pos.y += y;
            pos.z += z; //:h3ml
            var doc = get_document();
            if (_overflow > overflow.visible)
            {
                var border_box = pos;
                border_box += _padding;
                border_box += _borders;
                var bdr_radius = _css_borders.radius.calc_percents(border_box.width, border_box.height, border_box.depth); //:h3ml
                bdr_radius -= _borders;
                bdr_radius -= _padding;
                doc.container.set_clip(pos, bdr_radius, true, true);
            }

            doc.container.get_client_rect(out var browser_wnd);

            element el;
            foreach (var item in _children)
            {
                el = item;
                if (el.is_visible)
                {
                    switch (flag)
                    {
                        case draw_flag.positioned:
                            if (el.is_positioned && el.get_zindex == zindex)
                            {
                                if (el.get_element_position(out var junk) == element_position.@fixed)
                                {
                                    el.draw(hdc, browser_wnd.x, browser_wnd.y, browser_wnd.z, clip); //:h3ml
                                    el.draw_stacking_context(hdc, browser_wnd.x, browser_wnd.y, browser_wnd.z, clip, true); //:h3ml
                                }
                                else
                                {
                                    el.draw(hdc, pos.x, pos.y, pos.z, clip); //:h3ml
                                    el.draw_stacking_context(hdc, pos.x, pos.y, pos.z, clip, true); //:h3ml
                                }
                                el = null;
                            }
                            break;
                        case draw_flag.block:
                            if (!el.is_inline_box && el.get_float == element_float.none && !el.is_positioned)
                                el.draw(hdc, pos.x, pos.y, pos.z, clip); //:h3ml
                            break;
                        case draw_flag.floats:
                            if (el.get_float != element_float.none && !el.is_positioned)
                            {
                                el.draw(hdc, pos.x, pos.y, pos.z, clip); //:h3ml
                                el.draw_stacking_context(hdc, pos.x, pos.y, pos.z, clip, false); //:h3ml
                                el = null;
                            }
                            break;
                        case draw_flag.inlines:
                            if (el.is_inline_box && el.get_float == element_float.none && !el.is_positioned)
                            {
                                el.draw(hdc, pos.x, pos.y, pos.z, clip); //:h3ml
                                if (el.get_display == style_display.inline_block)
                                {
                                    el.draw_stacking_context(hdc, pos.x, pos.y, pos.z, clip, false); //:h3ml
                                    el = null;
                                }
                            }
                            break;
                        default: break;
                    }

                    if (el != null)
                        if (flag == draw_flag.positioned)
                        {
                            if (!el.is_positioned)
                                el.draw_children(hdc, pos.x, pos.y, pos.z, clip, flag, zindex); //:h3ml
                        }
                        else
                        {
                            if (el.get_float == element_float.none && el.get_display != style_display.inline_block && !el.is_positioned)
                                el.draw_children(hdc, pos.x, pos.y, pos.z, clip, flag, zindex); //:h3ml
                        }
                }
            }

            if (_overflow > overflow.visible)
                doc.container.del_clip();
        }

        protected void draw_children_table(object hdc, int x, int y, int z, position clip, draw_flag flag, int zindex) //:h3ml
        {
            if (_grid == null) return;
            var pos = _pos;
            pos.x += x;
            pos.y += y;
            pos.z += z; //:h3ml
            for (var row = 0; row < _grid.rows_count; row++)
            {
                if (flag == draw_flag.block)
                    _grid.row(row).el_row.draw_background(hdc, pos.x, pos.y, pos.z, clip); //:h3ml
                for (var col = 0; col < _grid.cols_count; col++)
                {
                    var cell = _grid.cell(col, row);
                    if (cell.el != null)
                    {
                        if (flag == draw_flag.block)
                            cell.el.draw(hdc, pos.x, pos.y, pos.z, clip); //:h3ml
                        cell.el.draw_children(hdc, pos.x, pos.y, pos.z, clip, flag, zindex); //:h3ml
                    }
                }
            }
        }

        protected int render_box(int x, int y, int z, int max_width, bool second_pass = false) //:h3ml
        {
            var parent_width = max_width;
            calc_outlines(parent_width);
            _pos.clear();
            _pos.move_to(x, y, z); //:h3ml
            _pos.x += content_margins_left;
            _pos.y += content_margins_top;
            _pos.z += content_margins_front; //:h3ml
            var ret_width = 0;

            var block_width = new def_value<int>(0);
            if (_display != style_display.table_cell && !_css_width.is_predefined)
            {
                var w = calc_width(parent_width);
                if (_box_sizing == box_sizing.border_box)
                    w -= _padding.width + _borders.width;
                ret_width = max_width = w;
                block_width.assignTo(w);
            }
            else
            {
                if (max_width != 0)
                    max_width -= content_margins_left + content_margins_right;
            }

            // check for max-width (on the first pass only)
            if (!_css_max_width.is_predefined && !second_pass)
            {
                var mw = get_document().cvt_units(_css_max_width, _font_size, parent_width);
                if (_box_sizing == box_sizing.border_box)
                    mw -= _padding.left + _borders.left + _padding.right + _borders.right;
                if (max_width > mw)
                    max_width = mw;
            }

            _floats_left.Clear();
            _floats_right.Clear();
            _boxes.Clear();
            _cahe_line_left.invalidate();
            _cahe_line_right.invalidate();

            _pos.height = 0;
            if (get_predefined_height(out var block_height))
                _pos.height = block_height;

            //:h3ml
            _pos.depth = 0; //:h3ml
            if (get_predefined_depth(out var block_depth)) //:h3ml
                _pos.depth = block_depth; //:h3ml

            var ws = get_white_space;
            var skip_spaces = ws == white_space.normal || ws == white_space.nowrap || ws == white_space.pre_line;
            var was_space = false;

            element_position el_position;
            foreach (var el in _children)
            {
                // we don't need process absolute and fixed positioned element on the second pass
                if (second_pass)
                {
                    el_position = el.get_element_position(out var junk);
                    if (el_position == element_position.absolute || el_position == element_position.@fixed) continue;
                }

                // skip spaces to make rendering a bit faster
                if (skip_spaces)
                    if (el.is_white_space())
                    {
                        if (was_space)
                        {
                            el.skip = true;
                            continue;
                        }
                        else
                            was_space = true;
                    }
                    else
                        was_space = false;

                // place element into rendering flow
                var rw = place_element(el, max_width);
                if (rw > ret_width)
                    ret_width = rw;
            }

            finish_last_box(true);

            _pos.width = block_width.is_default && is_inline_box ? ret_width : max_width;
            calc_auto_margins(parent_width);

            if (_boxes.Count != 0)
                if (collapse_top_margin)
                {
                    var old_top = _margins.top;
                    _margins.top = Math.Max(_boxes.First().top_margin, _margins.top);
                    if (_margins.top != old_top)
                        update_floats(_margins.top - old_top, this);
                }
            if (collapse_bottom_margin)
            {
                _margins.bottom = Math.Max(_boxes.Last().bottom_margin, _margins.bottom);
                _pos.height = _boxes.Last().bottom - _boxes.Last().bottom_margin;
            }
            else
                _pos.height = _boxes.Last().bottom;

            // add the floats height to the block height
            if (is_floats_holder)
            {
                var floats_height = get_floats_height();
                if (floats_height > _pos.height)
                    _pos.height = floats_height;
            }

            // calculate the final position
            _pos.move_to(x, y, z); //:h3ml
            _pos.x += content_margins_left;
            _pos.y += content_margins_top;
            _pos.z += content_margins_front; //:h3ml
            if (get_predefined_height(out block_height))
                _pos.height = block_height;
            if (get_predefined_depth(out block_depth)) //:h3ml
                _pos.depth = block_depth; //:h3ml

            var min_height = 0;
            if (!_css_min_height.is_predefined && _css_min_height.units == css_units.percentage)
            {
                var el_parent = parent();
                if (el_parent != null && el_parent.get_predefined_height(out block_height))
                    min_height = _css_min_height.calc_percent(block_height);
            }
            else
                min_height = (int)_css_min_height.val;
            if (min_height != 0 && _box_sizing == box_sizing.border_box)
            {
                min_height -= _padding.top + _borders.top + _padding.bottom + _borders.bottom;
                if (min_height < 0) min_height = 0;
            }

            //:h3ml
            var min_depth = 0; //:h3ml
            if (!_css_min_depth.is_predefined && _css_min_depth.units == css_units.percentage) //:h3ml
            {
                var el_parent = parent(); //:h3ml
                if (el_parent != null && el_parent.get_predefined_depth(out block_depth)) //:h3ml
                    min_depth = _css_min_depth.calc_percent(block_depth); //:h3ml
            }
            else
                min_depth = (int)_css_min_depth.val; //:h3ml
            if (min_depth != 0 && _box_sizing == box_sizing.border_box) //:h3ml
            {
                min_depth -= _padding.front + _borders.front + _padding.back + _borders.back; //:h3ml
                if (min_depth < 0) min_depth = 0; //:h3ml
            }

            if (_display == style_display.list_item)
            {
                var list_image = get_style_property("list-style-image", true, null);
                if (list_image != null)
                {
                    css.parse_css_url(list_image, out var url);
                    var list_image_baseurl = get_style_property("list-style-image-baseurl", true, null);
                    get_document().container.get_image_size(url, list_image_baseurl, out var sz);
                    if (min_height < sz.height)
                        min_height = sz.height;
                    if (min_depth < sz.depth) //:h3ml
                        min_depth = sz.depth; //:h3ml
                }
            }

            if (min_height > _pos.height)
                _pos.height = min_height;
            if (min_depth > _pos.depth) //:h3ml
                _pos.depth = min_depth; //:h3ml

            var min_width = _css_min_width.calc_percent(parent_width);
            if (min_width != 0 && _box_sizing == box_sizing.border_box)
            {
                min_width -= _padding.left + _borders.left + _padding.right + _borders.right;
                if (min_width < 0) min_width = 0;
            }
            if (min_width != 0)
            {
                if (min_width > _pos.width)
                    _pos.width = min_width;
                if (min_width > ret_width)
                    ret_width = min_width;
            }

            ret_width += content_margins_left + content_margins_right;

            // re-render with new width
            if (ret_width < max_width && !second_pass && have_parent)
                if (_display == style_display.inline_block || _css_width.is_predefined && (_float != element_float.none || _display == style_display.table || _el_position == element_position.absolute || _el_position == element_position.@fixed))
                {
                    render(x, y, z, ret_width, true); //:h3ml
                    _pos.width = ret_width - (content_margins_left + content_margins_right);
                }

            if (is_floats_holder && !second_pass)
                foreach (var fb in _floats_left)
                    fb.el.apply_relative_shift(fb.el.parent().calc_width(_pos.width));

            return ret_width;
        }

        protected int render_table(int x, int y, int z, int max_width, bool second_pass = false) //:h3ml
        {
            if (_grid == null) return 0;

            var parent_width = max_width;

            calc_outlines(parent_width);

            _pos.clear();
            _pos.move_to(x, y, z); //:h3ml
            _pos.x += content_margins_left;
            _pos.y += content_margins_top;

            var block_width = new def_value<int>(0);
            if (!_css_width.is_predefined)
                block_width.assignTo(max_width = calc_width(parent_width) - _padding.width - _borders.width);
            else if (max_width != 0)
                max_width -= content_margins_left + content_margins_right;

            // Calculate table spacing
            var table_width_spacing = 0;
            if (_border_collapse == border_collapse.separate)
                table_width_spacing = _border_spacing_x * (_grid.cols_count + 1);
            else
            {
                table_width_spacing = 0;
                if (_grid.cols_count != 0)
                {
                    table_width_spacing -= Math.Min(border_left, _grid.column(0).border_left);
                    table_width_spacing -= Math.Min(border_right, _grid.column(_grid.cols_count - 1).border_right);
                }
                for (var col = 1; col < _grid.cols_count; col++)
                    table_width_spacing -= Math.Min(_grid.column(col).border_left, _grid.column(col - 1).border_right);
            }


            // Calculate the minimum content width (MCW) of each cell: the formatted content may span any number of lines but may not overflow the cell box. 
            // If the specified 'width' (W) of the cell is greater than MCW, W is the minimum cell width. A value of 'auto' means that MCW is the minimum 
            // cell width.
            // 
            // Also, calculate the "maximum" cell width of each cell: formatting the content without breaking lines other than where explicit line breaks occur.
            if (_grid.cols_count == 1 && !block_width.is_default)
            {
                for (var row = 0; row < _grid.rows_count; row++)
                {
                    var cell = _grid.cell(0, row);
                    if (cell != null && cell.el != null)
                    {
                        cell.min_width = cell.max_width = cell.el.render(0, 0, 0, max_width - table_width_spacing); //:h3ml
                        cell.el._pos.width = cell.min_width - cell.el.content_margins_left - cell.el.content_margins_right;
                    }
                }
            }
            else
            {
                for (var row = 0; row < _grid.rows_count; row++)
                    for (var col = 0; col < _grid.cols_count; col++)
                    {
                        var cell = _grid.cell(col, row);
                        if (cell != null && cell.el != null)
                            if (!_grid.column(col).css_width.is_predefined && _grid.column(col).css_width.units != css_units.percentage)
                            {
                                var css_w = _grid.column(col).css_width.calc_percent(block_width.val);
                                var el_w = cell.el.render(0, 0, 0, css_w); //:h3ml
                                cell.min_width = cell.max_width = Math.Max(css_w, el_w);
                                cell.el._pos.width = cell.min_width - cell.el.content_margins_left - cell.el.content_margins_right;
                            }
                            else
                            {
                                // calculate minimum content width
                                cell.min_width = cell.el.render(0, 0, 0, 1); //:h3ml
                                // calculate maximum content width
                                cell.max_width = cell.el.render(0, 0, 0, max_width - table_width_spacing); //:h3ml
                            }
                    }
            }

            // For each column, determine a maximum and minimum column width from the cells that span only that column. 
            // The minimum is that required by the cell with the largest minimum cell width (or the column 'width', whichever is larger). 
            // The maximum is that required by the cell with the largest maximum cell width (or the column 'width', whichever is larger).
            for (var col = 0; col < _grid.cols_count; col++)
            {
                _grid.column(col).max_width = 0;
                _grid.column(col).min_width = 0;
                for (var row = 0; row < _grid.rows_count; row++)
                    if (_grid.cell(col, row).colspan <= 1)
                    {
                        _grid.column(col).max_width = Math.Max(_grid.column(col).max_width, _grid.cell(col, row).max_width);
                        _grid.column(col).min_width = Math.Max(_grid.column(col).min_width, _grid.cell(col, row).min_width);
                    }
            }

            // For each cell that spans more than one column, increase the minimum widths of the columns it spans so that together, 
            // they are at least as wide as the cell. Do the same for the maximum widths. 
            // If possible, widen all spanned columns by approximately the same amount.
            for (var col = 0; col < _grid.cols_count; col++)
                for (var row = 0; row < _grid.rows_count; row++)
                    if (_grid.cell(col, row).colspan > 1)
                    {
                        var max_total_width = _grid.column(col).max_width;
                        var min_total_width = _grid.column(col).min_width;
                        for (var col2 = col + 1; col2 < col + _grid.cell(col, row).colspan; col2++)
                        {
                            max_total_width += _grid.column(col2).max_width;
                            min_total_width += _grid.column(col2).min_width;
                        }
                        if (min_total_width < _grid.cell(col, row).min_width)
                            _grid.distribute_min_width(_grid.cell(col, row).min_width - min_total_width, col, col + _grid.cell(col, row).colspan - 1);
                        if (max_total_width < _grid.cell(col, row).max_width)
                            _grid.distribute_max_width(_grid.cell(col, row).max_width - max_total_width, col, col + _grid.cell(col, row).colspan - 1);
                    }

            // If the 'table' or 'inline-table' element's 'width' property has a computed value (W) other than 'auto', the used width is the 
            // greater of W, CAPMIN, and the minimum width required by all the columns plus cell spacing or borders (MIN). 
            // If the used width is greater than MIN, the extra width should be distributed over the columns.
            //
            // If the 'table' or 'inline-table' element has 'width: auto', the used width is the greater of the table's containing block width, 
            // CAPMIN, and MIN. However, if either CAPMIN or the maximum width required by the columns plus cell spacing or borders (MAX) is 
            // less than that of the containing block, use max(MAX, CAPMIN).
            var table_width = 0;
            var min_table_width = 0;
            var max_table_width = 0;

            if (!block_width.is_default)
                table_width = _grid.calc_table_width(block_width.val - table_width_spacing, false, min_table_width, max_table_width);
            else
                table_width = _grid.calc_table_width(max_width - table_width_spacing, true, min_table_width, max_table_width);

            min_table_width += table_width_spacing;
            max_table_width += table_width_spacing;
            table_width += table_width_spacing;
            _grid.calc_horizontal_positions(_borders, _border_collapse, _border_spacing_x);

            var row_span_found = false;

            // render cells with computed width
            for (var row = 0; row < _grid.rows_count; row++)
            {
                _grid.row(row).height = 0;
                for (var col = 0; col < _grid.cols_count; col++)
                {
                    var cell = _grid.cell(col, row);
                    if (cell.el != null)
                    {
                        var span_col = col + cell.colspan - 1;
                        if (span_col >= _grid.cols_count)
                            span_col = _grid.cols_count - 1;
                        var cell_width = _grid.column(span_col).right - _grid.column(col).left;

                        if (cell.el._pos.width != cell_width - cell.el.content_margins_left - cell.el.content_margins_right)
                        {
                            cell.el.render(_grid.column(col).left, 0, 0, cell_width); //:h3ml
                            cell.el._pos.width = cell_width - cell.el.content_margins_left - cell.el.content_margins_right;
                        }
                        else
                            cell.el._pos.x = _grid.column(col).left + cell.el.content_margins_left;

                        if (cell.rowspan <= 1)
                            _grid.row(row).height = Math.Max(_grid.row(row).height, cell.el.height);
                        else
                            row_span_found = true;
                    }
                }
            }

            if (row_span_found)
                for (var col = 0; col < _grid.cols_count; col++)
                    for (var row = 0; row < _grid.rows_count; row++)
                    {
                        var cell = _grid.cell(col, row);
                        if (cell.el != null)
                        {
                            var span_row = row + cell.rowspan - 1;
                            if (span_row >= _grid.rows_count)
                                span_row = _grid.rows_count - 1;
                            if (span_row != row)
                            {
                                var h = 0;
                                for (var i = row; i <= span_row; i++)
                                    h += _grid.row(i).height;
                                if (h < cell.el.height)
                                    _grid.row(span_row).height += cell.el.height - h;
                            }
                        }
                    }

            // Calculate vertical table spacing
            var table_height_spacing = 0;
            if (_border_collapse == border_collapse.separate)
                table_height_spacing = _border_spacing_y * (_grid.rows_count + 1);
            else
            {
                table_height_spacing = 0;
                if (_grid.rows_count != 0)
                {
                    table_height_spacing -= Math.Min(border_top, _grid.row(0).border_top);
                    table_height_spacing -= Math.Min(border_bottom, _grid.row(_grid.rows_count - 1).border_bottom);
                }
                for (var row = 1; row < _grid.rows_count; row++)
                    table_height_spacing -= Math.Min(_grid.row(row).border_top, _grid.row(row - 1).border_bottom);
            }

            // calculate block height
            if (get_predefined_height(out var block_height))
                block_height -= _padding.height + _borders.height;

            // calculate minimum height from m_css_min_height
            var min_height = 0;
            if (!_css_min_height.is_predefined && _css_min_height.units == css_units.percentage)
            {
                var el_parent = parent();
                if (el_parent != null)
                {
                    var parent_height = 0;
                    if (el_parent.get_predefined_height(out parent_height))
                        min_height = _css_min_height.calc_percent(parent_height);
                }
            }
            else min_height = (int)_css_min_height.val;

            //var extra_row_height = 0;
            var minimum_table_height = Math.Max(block_height, min_height);

            _grid.calc_rows_height(minimum_table_height - table_height_spacing, _border_spacing_y);
            _grid.calc_vertical_positions(_borders, _border_collapse, _border_spacing_y);

            var table_height = 0;
            var table_depth = 0; //:h3ml

            // place cells vertically
            for (var col = 0; col < _grid.cols_count; col++)
                for (var row = 0; row < _grid.rows_count; row++)
                {
                    var cell = _grid.cell(col, row);
                    if (cell.el != null)
                    {
                        var span_row = row + cell.rowspan - 1;
                        if (span_row >= _grid.rows_count)
                            span_row = _grid.rows_count - 1;
                        cell.el._pos.y = _grid.row(row).top + cell.el.content_margins_top;
                        cell.el._pos.height = _grid.row(span_row).bottom - _grid.row(row).top - cell.el.content_margins_top - cell.el.content_margins_bottom;
                        table_height = Math.Max(table_height, _grid.row(span_row).bottom);
                        cell.el.apply_vertical_align();
                    }
                }

            if (_border_collapse == border_collapse.collapse)
            {
                if (_grid.rows_count != 0)
                    table_height -= Math.Min(border_bottom, _grid.row(_grid.rows_count - 1).border_bottom);
            }
            else
                table_height += _border_spacing_y;

            _pos.width = table_width;

            calc_auto_margins(parent_width);

            _pos.move_to(x, y, z); //:h3ml
            _pos.x += content_margins_left;
            _pos.y += content_margins_top;
            _pos.z += content_margins_front;//:h3ml
            _pos.width = table_width;
            _pos.height = table_height;
            _pos.depth = table_depth;

            return max_table_width;
        }

        protected int fix_line_width(int max_width, element_float flt)
        {
            var ret_width = 0;
            if (_boxes.Count != 0)
            {
                var els = new List<element>();
                _boxes.Last().get_elements(els);
                var was_cleared = false;
                if (els.Count != 0 && els.First().get_clear != element_clear.none)
                {
                    if (els.First().get_clear == element_clear.both)
                        was_cleared = true;
                    else if ((flt == element_float.left && els.First().get_clear == element_clear.left) || (flt == element_float.right && els.First().get_clear == element_clear.right))
                        was_cleared = true;
                }
                if (!was_cleared)
                {
                    _boxes.RemoveAt(_boxes.Count - 1);
                    foreach (var i in els)
                    {
                        var rw = place_element(i, max_width);
                        if (rw > ret_width)
                            ret_width = rw;
                    }
                }
                else
                {
                    var line_top = _boxes.Last().get_type == box_type.line ? _boxes.Last().top : _boxes.Last().bottom;
                    var line_left = 0;
                    var line_right = max_width;
                    get_line_left_right(line_top, max_width, ref line_left, ref line_right);
                    if (_boxes.Last().get_type == box_type.line)
                    {
                        if (_boxes.Count == 1 && _list_style_type != list_style_type.none && _list_style_position == list_style_position.inside)
                            line_left += get_font_size;
                        if (_css_text_indent.val != 0)
                        {
                            var line_box_found = false;
                            foreach (var iter in _boxes)
                                if (iter.get_type == box_type.line)
                                {
                                    line_box_found = true;
                                    break;
                                }
                            if (!line_box_found)
                                line_left += _css_text_indent.calc_percent(max_width);
                        }
                    }
                    var els2 = new List<element>();
                    _boxes.Last().new_width(line_left, line_right, els2);
                    foreach (var el in els2)
                    {
                        var rw = place_element(el, max_width);
                        if (rw > ret_width)
                            ret_width = rw;
                    }
                }
            }
            return ret_width;
        }

        protected void parse_background()
        {
            // parse background-color
            _bg._color = get_color("background-color", false, new web_color(0, 0, 0, 0));

            // parse background-position
            var str = get_style_property("background-position", false, "0% 0%");
            if (str != null)
            {
                var res = new List<string>();
                html.split_string(str, res, " \t");
                if (res.Count != 0)
                {
                    if (res.Count == 1)
                    {
                        if (html.value_in_list(res[0], "left;right;center")) { _bg._position.x.fromString(res[0], "left;right;center"); _bg._position.y.set_value(50, css_units.percentage); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                        else if (html.value_in_list(res[0], "top;bottom;center")) { _bg._position.y.fromString(res[0], "top;bottom;center"); _bg._position.x.set_value(50, css_units.percentage); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                        else if (html.value_in_list(res[0], "front;back;center")) { _bg._position.z.fromString(res[0], "front;back;center"); _bg._position.x.set_value(50, css_units.percentage); _bg._position.y.set_value(50, css_units.percentage); } //:h3ml
                        else { _bg._position.x.fromString(res[0], "left;right;center"); _bg._position.y.set_value(50, css_units.percentage); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                    }
                    else
                    {
                        if (html.value_in_list(res[0], "left;right")) { _bg._position.x.fromString(res[0], "left;right;center"); _bg._position.y.fromString(res[1], "top;bottom;center"); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                        else if (html.value_in_list(res[0], "top;bottom")) { _bg._position.x.fromString(res[1], "left;right;center"); _bg._position.y.fromString(res[0], "top;bottom;center"); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                        else if (html.value_in_list(res[1], "left;right")) { _bg._position.x.fromString(res[1], "left;right;center"); _bg._position.y.fromString(res[0], "top;bottom;center"); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                        else if (html.value_in_list(res[1], "top;bottom")) { _bg._position.x.fromString(res[0], "left;right;center"); _bg._position.y.fromString(res[1], "top;bottom;center"); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                        else { _bg._position.x.fromString(res[0], "left;right;center"); _bg._position.y.fromString(res[1], "top;bottom;center"); _bg._position.z.set_value(50, css_units.percentage); } //:h3ml
                    }

                    if (_bg._position.x.is_predefined)
                        switch (_bg._position.x.predef)
                        {
                            case 0: _bg._position.x.set_value(0, css_units.percentage); break;
                            case 1: _bg._position.x.set_value(100, css_units.percentage); break;
                            case 2: _bg._position.x.set_value(50, css_units.percentage); break;
                        }
                    if (_bg._position.y.is_predefined)
                        switch (_bg._position.y.predef)
                        {
                            case 0: _bg._position.y.set_value(0, css_units.percentage); break;
                            case 1: _bg._position.y.set_value(100, css_units.percentage); break;
                            case 2: _bg._position.y.set_value(50, css_units.percentage); break;
                        }
                    //:h3ml
                    if (_bg._position.z.is_predefined) //:h3ml
                        switch (_bg._position.z.predef) //:h3ml
                        {
                            case 0: _bg._position.z.set_value(0, css_units.percentage); break; //:h3ml
                            case 1: _bg._position.z.set_value(100, css_units.percentage); break; //:h3ml
                            case 2: _bg._position.z.set_value(50, css_units.percentage); break; //:h3ml
                        }
                }
                else { _bg._position.x.set_value(0, css_units.percentage); _bg._position.y.set_value(0, css_units.percentage); _bg._position.z.set_value(0, css_units.percentage); } //:h3ml
            }
            else { _bg._position.y.set_value(0, css_units.percentage); _bg._position.x.set_value(0, css_units.percentage); _bg._position.z.set_value(0, css_units.percentage); } //:h3ml

            str = get_style_property("background-size", false, "auto");
            if (str != null)
            {
                var res = new List<string>();
                html.split_string(str, res, " \t");
                if (res.Count != 0)
                {
                    _bg._position.width.fromString(res[0], types.background_size_strings);
                    if (res.Count >= 1) _bg._position.height.fromString(res[1], types.background_size_strings);
                    else _bg._position.height.predef = (int)background_size.auto;
                    if (res.Count >= 2) _bg._position.depth.fromString(res[2], types.background_size_strings); //:h3ml
                    else _bg._position.depth.predef = (int)background_size.auto; //:h3ml
                }
                else
                {
                    _bg._position.width.predef = (int)background_size.auto;
                    _bg._position.height.predef = (int)background_size.auto;
                    _bg._position.depth.predef = (int)background_size.auto; //:h3ml
                }
            }

            var doc = get_document();
            doc.cvt_units(_bg._position.x, _font_size);
            doc.cvt_units(_bg._position.y, _font_size);
            doc.cvt_units(_bg._position.z, _font_size); //:h3ml
            doc.cvt_units(_bg._position.width, _font_size);
            doc.cvt_units(_bg._position.height, _font_size);
            doc.cvt_units(_bg._position.depth, _font_size); //:h3ml

            // parse background_attachment
            _bg._attachment = (background_attachment)html.value_index(get_style_property("background-attachment", false, "scroll"), types.background_attachment_strings, (int)background_attachment.scroll);
            // parse background_attachment
            _bg._repeat = (background_repeat)html.value_index(get_style_property("background-repeat", false, "repeat"), types.background_repeat_strings, (int)background_repeat.repeat);
            // parse background_clip
            _bg._clip = (background_box)html.value_index(get_style_property("background-clip", false, "border-box"), types.background_box_strings, (int)background_box.border_box);
            // parse background_origin
            _bg._origin = (background_box)html.value_index(get_style_property("background-origin", false, "padding-box"), types.background_box_strings, (int)background_box.content_box);

            // parse background-image
            css.parse_css_url(get_style_property("background-image", false, ""), out _bg._image);
            _bg._baseurl = get_style_property("background-image-baseurl", false, "");

            if (!string.IsNullOrEmpty(_bg._image))
                doc.container.load_image(_bg._image, string.IsNullOrEmpty(_bg._baseurl) ? null : _bg._baseurl, true);
        }

        protected void init_background_paint(position pos, background_paint bg_paint, background bg)
        {
            if (bg == null) return;
            bg_paint.assignTo(bg);
            var content_box = pos;
            var padding_box = pos;
            padding_box += _padding;
            var border_box = padding_box;
            border_box += _borders;
            switch (bg._clip)
            {
                case background_box.padding_box: bg_paint.clip_box = padding_box; break;
                case background_box.content_box: bg_paint.clip_box = content_box; break;
                default: bg_paint.clip_box = border_box; break;
            }
            switch (bg._origin)
            {
                case background_box.border_box: bg_paint.origin_box = border_box; break;
                case background_box.content_box: bg_paint.origin_box = content_box; break;
                default: bg_paint.origin_box = padding_box; break;
            }
            if (!string.IsNullOrEmpty(bg_paint.image))
            {
                get_document().container.get_image_size(bg_paint.image, bg_paint.baseurl, out bg_paint.image_size);
                if (bg_paint.image_size.width != 0 && bg_paint.image_size.height != 0)
                {
                    var img_new_sz = bg_paint.image_size;
                    var img_ar_width = bg_paint.image_size.width / (double)bg_paint.image_size.height;
                    var img_ar_height = bg_paint.image_size.height / (double)bg_paint.image_size.width;
                    if (bg._position.width.is_predefined)
                        switch ((background_size)bg._position.width.predef)
                        {
                            case background_size.contain:
                                if ((int)(bg_paint.origin_box.width * img_ar_height) <= bg_paint.origin_box.height)
                                {
                                    img_new_sz.width = bg_paint.origin_box.width;
                                    img_new_sz.height = (int)(bg_paint.origin_box.width * img_ar_height);
                                    img_new_sz.depth = 0; //:h3ml
                                }
                                else
                                {
                                    img_new_sz.height = bg_paint.origin_box.height;
                                    img_new_sz.width = (int)(bg_paint.origin_box.height * img_ar_width);
                                    img_new_sz.depth = 0; //:h3ml
                                }
                                break;
                            case background_size.cover:
                                if ((int)(bg_paint.origin_box.width * img_ar_height) >= bg_paint.origin_box.height)
                                {
                                    img_new_sz.width = bg_paint.origin_box.width;
                                    img_new_sz.height = (int)(bg_paint.origin_box.width * img_ar_height);
                                    img_new_sz.depth = 0; //:h3ml
                                }
                                else
                                {
                                    img_new_sz.height = bg_paint.origin_box.height;
                                    img_new_sz.width = (int)(bg_paint.origin_box.height * img_ar_width);
                                    img_new_sz.depth = 0; //:h3ml
                                }
                                break;
                            case background_size.auto:
                                if (!bg._position.height.is_predefined)
                                {
                                    img_new_sz.height = bg._position.height.calc_percent(bg_paint.origin_box.height);
                                    img_new_sz.width = (int)(img_new_sz.height * img_ar_width);
                                    img_new_sz.depth = 0; //:h3ml
                                }
                                break;
                        }
                    else
                    {
                        img_new_sz.width = bg._position.width.calc_percent(bg_paint.origin_box.width);
                        img_new_sz.height = bg._position.height.is_predefined
                            ? (int)(img_new_sz.width * img_ar_height)
                            : bg._position.height.calc_percent(bg_paint.origin_box.height);
                        img_new_sz.depth = 0; //:h3ml
                    }

                    bg_paint.image_size = img_new_sz;
                    bg_paint.position_x = bg_paint.origin_box.x + bg._position.x.calc_percent(bg_paint.origin_box.width - bg_paint.image_size.width);
                    bg_paint.position_y = bg_paint.origin_box.y + bg._position.y.calc_percent(bg_paint.origin_box.height - bg_paint.image_size.height);
                    bg_paint.position_z = bg_paint.origin_box.z + bg._position.z.calc_percent(bg_paint.origin_box.depth - bg_paint.image_size.depth); //:h3ml
                }
            }
            bg_paint.border_radius = _css_borders.radius.calc_percents(border_box.width, border_box.height, border_box.depth); //:h3ml
            bg_paint.border_box = border_box;
            bg_paint.is_root = !have_parent;
        }

        protected void draw_list_marker(object hdc, position pos)
        {
            var lm = new list_marker();
            var list_image = get_style_property("list-style-image", true, null);
            var img_size = new size();
            if (list_image != null)
            {
                css.parse_css_url(list_image, out lm.image);
                lm.baseurl = get_style_property("list-style-image-baseurl", true, null);
                get_document().container.get_image_size(lm.image, lm.baseurl, out img_size);
            }
            else lm.baseurl = null;

            var ln_height = line_height;
            var sz_font = get_font_size;
            lm.pos.x = pos.x;
            lm.pos.width = sz_font - sz_font * 2 / 3;
            lm.pos.height = sz_font - sz_font * 2 / 3;
            lm.pos.depth = 0; //:h3ml
            lm.pos.y = pos.y + ln_height / 2 - lm.pos.height / 2;
            lm.pos.z = pos.z; //:h3ml

            if (img_size.width != 0 && img_size.height != 0)
            {
                if (lm.pos.y + img_size.height > pos.y + pos.height)
                    lm.pos.y = pos.y + pos.height - img_size.height;
                if (img_size.width > lm.pos.width)
                    lm.pos.x -= img_size.width - lm.pos.width;
                if (lm.pos.z + img_size.depth > pos.z + pos.depth) //:h3ml
                    lm.pos.z = pos.z + pos.depth - img_size.depth; //:h3ml
                lm.pos.width = img_size.width;
                lm.pos.height = img_size.height;
                lm.pos.depth = img_size.depth; //:h3ml
            }
            if (_list_style_position == list_style_position.outside)
                lm.pos.x -= sz_font;
            lm.color = get_color("color", true, new web_color(0, 0, 0));
            lm.marker_type = _list_style_type;
            get_document().container.draw_list_marker(hdc, lm);
        }

        protected void parse_nth_child_params(string param, out int num, out int off)
        {
            if (param == "odd") { num = 2; off = 1; }
            else if (param == "even") { num = 2; off = 0; }
            else
            {
                var tokens = new List<string>();
                html.split_string(param, tokens, " n", "n");
                var s_num = string.Empty;
                var s_int = string.Empty;
                foreach (var tok in tokens)
                {
                    if (tok == "n")
                    {
                        s_num = s_int;
                        s_int = string.Empty;
                    }
                    else s_int += tok;
                }
                var s_off = s_int;
                num = int.Parse(s_num);
                off = int.Parse(s_off);
            }
        }

        protected void remove_before_after()
        {
            if (_children.Count != 0)
            {
                if (_children.First().get_tagName() == "::before")
                    _children.RemoveAt(0);
            }
            if (_children.Count != 0)
            {
                if (_children.Last().get_tagName() == "::after")
                    _children.RemoveAt(_children.Count - 1);
            }
        }

        protected element get_element_before()
        {
            if (_children.Count != 0)
                if (_children.First().get_tagName() == "::before")
                    return _children.First();
            var el = new el_before(get_document());
            el.parent(this);
            _children.Insert(0, el);
            return el;
        }

        protected element get_element_after()
        {
            if (_children.Count != 0)
                if (_children.First().get_tagName() == "::after")
                    return _children.Last();
            var el = new el_after(get_document());
            appendChild(el);
            return el;
        }
    }
}
