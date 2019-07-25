namespace H3ml.Layout
{
    public enum box_type
    {
        box_block,
        box_line
    }

    public abstract class box
    {
        protected int _box_top;
        protected int _box_left;
        protected int _box_right;
        public box(int top, int left, int right)
        {
            _box_top = top;
            _box_left = left;
            _box_right = right;
        }

        public int bottom => _box_top + height;
        public int top => _box_top;
        public int right => _box_left + width;
        public int left => _box_left;

        public abstract box_type type { get; }
        public abstract int height { get; }
        public abstract int width { get; }
        public abstract void add_element(element el);
        public abstract bool can_hold(element el, white_space ws);
        public abstract void finish(bool last_box = false);
        public abstract bool isEmpty { get; }
        public abstract int baseline { get; }
        public abstract void get_elements(elements_vector els);
        public abstract int top_margin { get; }
        public abstract int bottom_margin { get; }
        public abstract void y_shift(int shift);
        public abstract void new_width(int left, int right, elements_vector els);
    }

    public class block_box : box
    {
        element _element;
        public block_box(int top, int left, int right) : base(top, left, right) { }

        public override box_type type => box_type.box_block;
        public override int height => _element.height;
        public override int width => _element.width;
        public override void add_element(element el) { _element = el; el._box = this; }
        public override bool can_hold(element el, white_space ws) => m_element != null || el.is_inline_box() ? false : true;
        public override void finish(bool last_box = false)
        {
            if (_element == null) return;
            _element.apply_relative_shift(_box_right - _box_left);
        }
        public override bool isEmpty => _element == null;
        public override int baseline => _element != null ? m_element.get_base_line() : 0;
        public override void get_elements(elements_vector els) => els.Add(m_element);
        public override int top_margin => _element != null && _element.collapse_top_margin ? _element._margins.top : 0;
        public override int bottom_margin => _element != null && _element.collapse_bottom_margin ? _element._margins.bottom : 0;
        public override void y_shift(int shift)
        {
            _box_top += shift;
            if (_element != null)
                _element._pos.y += shift;
        }
        public override void new_width(int left, int right, elements_vector els) { }
    }

    //////////////////////////////////////////////////////////////////////////

    public class line_box : box
    {
        elements_vector _items;
        int _height;
        int _width;
        int _line_height;
        font_metrics _font_metrics;
        int _baseline;
        text_align _text_align;

        public line_box(int top, int left, int right, int line_height, font_metrics fm, text_align align) : base(top, left, right)
        {
            _height = 0;
            _width = 0;
            _font_metrics = fm;
            _line_height = line_height;
            _baseline = 0;
            _text_align = align;
        }

        public override box_type type => box_type.box_line;
        public override int height => _height;
        public override int width => _width;
        public override void add_element(element el)
        {
            el._skip = false;
            el._box = 0;
            var add = true;
            if ((_items.empty() && el.is_white_space()) || el.is_break())
                el._skip = true;
            else if (el.is_white_space())
            {
                if (have_last_space())
                {
                    add = false;
                    el._skip = true;
                }
            }

            if (add)
            {
                el.m_box = this;
                _items.add(el);

                if (!el._skip)
                {
                    var el_shift_left = el.get_inline_shift_left();
                    var el_shift_right = el.get_inline_shift_right();

                    el._pos.x = _box_left + _width + el_shift_left + el.content_margins_left();
                    el._pos.y = _box_top + el.content_margins_top();
                    _width += el.width() + el_shift_left + el_shift_right;
                }
            }
        }

        public override bool can_hold(element el, white_space ws)
        {
            if (!el.is_inline_box()) return false;
            if (el.is_break()) return false;
            if (ws == white_space_nowrap || ws == white_space_pre) return true;
            if (_box_left + _width + el.width + el.get_inline_shift_left() + el.get_inline_shift_right() > _box_right) return false;
            return true;
        }

        public override void finish(bool last_box = false)
        {
            if (is_empty() || (!is_empty() && last_box && is_break_only()))
            {
                _height = 0;
                return;
            }

            for (var i = m_items.rbegin(); i != m_items.rend(); i++)
            {
                if (i.is_white_space() || i.is_break())
                {
                    if (!i._skip)
                    {
                        i._skip = true;
                        _width -= i.width;
                    }
                }
                else break;
            }

            var base_line = _font_metrics.base_line();
            var line_height = _line_height;

            var add_x = 0;
            switch (_text_align)
            {
                case text_align.text_align_right:
                    if (_width < (_box_right - _box_left))
                        add_x = (_box_right - _box_left) - _width;
                    break;
                case text_align.text_align_center:
                    if (_width < (_box_right - _box_left))
                        add_x = ((_box_right - _box_left) - _width) / 2;
                    break;
                default:
                    add_x = 0;
            }

            m_height = 0;
            // find line box baseline and line-height
            foreach (var el in _items)
            {
                if (el.get_display() == display_inline_text)
                {
                    font_metrics fm;
                    el.get_font(&fm);
                    base_line = std::max(base_line, fm.base_line());
                    line_height = std::max(line_height, el.line_height());
                    m_height = std::max(m_height, fm.height);
                }
                el.m_pos.x += add_x;
            }

            if (_height)
                base_line += (line_height - m_height) / 2;

            _height = line_height;

            var y1 = 0;
            var y2 = _height;

            foreach (var el in _items)
            {
                if (el.get_display() == display_inline_text)
                {
                    font_metrics fm;
                    el.get_font(&fm);
                    el.m_pos.y = m_height - base_line - fm.ascent;
                }
                else
                {
                    switch (el.get_vertical_align())
                    {
                        case va_super:
                        case va_sub:
                        case va_baseline: el._pos.y = _height - base_line - el.height + el.get_base_line() + el.content_margins_top(); break;
                        case va_top: el._pos.y = y1 + el.content_margins_top(); break;
                        case va_text_top: el._pos.y = m_height - base_line - m_font_metrics.ascent + el.content_margins_top(); break;
                        case va_middle: el._pos.y = m_height - base_line - m_font_metrics.x_height / 2 - el.height() / 2 + el.content_margins_top(); break;
                        case va_bottom: el._pos.y = y2 - el.height() + el.content_margins_top(); break;
                        case va_text_bottom: el._pos.y = m_height - base_line + m_font_metrics.descent - el.height() + el.content_margins_top(); break;
                    }
                    y1 = Math.Min(y1, el.top);
                    y2 = Math.Max(y2, el.bottom);
                }
            }

            css_offsets offsets;

            foreach (var el in _items)
            {
                el._pos.y -= y1;
                el._pos.y += _box_top;
                if (el.get_display() != display_inline_text)
                {
                    switch (el.get_vertical_align())
                    {
                        case va_top: el._pos.y = _box_top + el.content_margins_top; break;
                        case va_bottom: el._pos.y = _box_top + (y2 - y1) - el.height + el.content_margins_top; break;
                        case va_baseline: break; //TODO: process vertical align "baseline"
                        case va_middle: break; //TODO: process vertical align "middle"
                        case va_sub: break; //TODO: process vertical align "sub"
                        case va_super: break; //TODO: process vertical align "super"
                        case va_text_bottom: break; //TODO: process vertical align "text-bottom"
                        case va_text_top: break; //TODO: process vertical align "text-top"
                    }
                }

                el.apply_relative_shift(_box_right - _box_left);
            }
            _height = y2 - y1;
            _baseline = (base_line - y1) - (_height - line_height);
        }

        public override bool isEmpty
        {
            get
            {
                if (_items.empty()) return true;
                for (var i = m_items.rbegin(); i != m_items.rend(); i++)
                    if (!i._skip || i.is_break())
                        return false;
                return true;
            }
        }
        public override int baseline => _baseline;
        public override void get_elements(elements_vector els) { els.insert(els.begin(), _items.begin(), _items.end()); }

        public override int top_margin => 0;
        public override int bottom_margin => 0;
        public override void y_shift(int shift)
        {
            _box_top += shift;
            foreach (var el in _items)
                el._pos.y += shift;
        }

        public override void new_width(int left, int right, elements_vector els)
        {
            var add = left - m_box_left;
            if (add)
            {
                _box_left = left;
                _box_right = right;
                _width = 0;
                var remove_begin = _items.end();
                for (var i = _items.begin() + 1; i != _items.end(); i++)
                {
                    element el = i;

                    if (!el._skip)
                    {
                        if (_box_left + m_width + el.width + el.get_inline_shift_right() + el.get_inline_shift_left() > _box_right)
                        {
                            remove_begin = i;
                            break;
                        }
                        else
                        {
                            el._pos.x += add;
                            _width += el.width + el.get_inline_shift_right() + el.get_inline_shift_left();
                        }
                    }
                }
                if (remove_begin != _items.end())
                {
                    els.insert(els.begin(), remove_begin, m_items.end());
                    _items.erase(remove_begin, _items.end());

                    foreach (var el in els)
                        el._box = 0;
                }
            }
        }

        bool have_last_space()
        {
            var ret = false;
            for (var i = m_items.rbegin(); i != m_items.rend() && !ret; i++)
                if (i.is_white_space() || i.is_break()) ret = true;
                else break;
            return ret;
        }

        bool is_break_only()
        {
            if (_items.empty()) return true;

            if (_items.First().is_break())
            {
                foreach (var el in _items)
                    if (!el._skip)
                        return false;
                return true;
            }
            return false;
        }
    }
}
