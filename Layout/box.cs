using System;
using System.Linq;
using System.Collections.Generic;

namespace H3ml.Layout
{
    public enum box_type
    {
        block,
        line
    }

    public abstract class box
    {
        protected int _box_top;
        protected int _box_left;
        protected int _box_front; //:h3ml
        protected int _box_right;

        public box(int top, int left, int front, int right)
        {
            _box_top = top;
            _box_left = left;
            _box_front = front; //:h3ml
            _box_right = right;
        }

        public int bottom => _box_top + height;
        public int top => _box_top;
        public int right => _box_left + width;
        public int left => _box_left;
        public int front => _box_front; //:h3ml
        public int back => _box_front + depth; //:h3ml

        public abstract box_type get_type { get; }
        public abstract int height { get; }
        public abstract int width { get; }
        public abstract int depth { get; } //:h3ml
        public abstract void add_element(element el);
        public abstract bool can_hold(element el, white_space ws);
        public abstract void finish(bool last_box = false);
        public abstract bool is_empty { get; }
        public abstract int baseline { get; }
        public abstract void get_elements(IList<element> els);
        public abstract int top_margin { get; }
        public abstract int bottom_margin { get; }
        public abstract void y_shift(int shift);
        public abstract void new_width(int left, int right, IList<element> els);
    }

    public class block_box : box
    {
        element _element;
        public block_box(int top, int left, int front, int right) : base(top, left, front, right) { } //:h3ml

        public override box_type get_type => box_type.block;
        public override int height => _element.height;
        public override int width => _element.width;
        public override int depth => _element.depth; //:h3ml
        public override void add_element(element el) { _element = el; el._box = this; }
        public override bool can_hold(element el, white_space ws) => _element != null || el.is_inline_box ? false : true;
        public override void finish(bool last_box = false)
        {
            if (_element == null) return;
            _element.apply_relative_shift(_box_right - _box_left);
        }
        public override bool is_empty => _element == null;
        public override int baseline => _element != null ? _element.get_base_line() : 0;
        public override void get_elements(IList<element> els) => els.Add(_element);
        public override int top_margin => _element != null && _element.collapse_top_margin ? _element._margins.top : 0;
        public override int bottom_margin => _element != null && _element.collapse_bottom_margin ? _element._margins.bottom : 0;
        public override void y_shift(int shift)
        {
            _box_top += shift;
            if (_element != null)
                _element._pos.y += shift;
        }
        public override void new_width(int left, int right, IList<element> els) { }
    }

    //////////////////////////////////////////////////////////////////////////

    public class line_box : box
    {
        List<element> _items = new List<element>();
        int _height;
        int _width;
        int _depth; //:h3ml
        int _line_height;
        font_metrics _font_metrics;
        int _baseline;
        text_align _text_align;

        public line_box(int top, int left, int front, int right, int line_height, font_metrics fm, text_align align) : base(top, left, front, right) //:h3ml
        {
            _height = 0;
            _width = 0;
            _depth = 0; //:h3ml
            _font_metrics = fm;
            _line_height = line_height;
            _baseline = 0;
            _text_align = align;
        }

        public override box_type get_type => box_type.line;
        public override int height => _height;
        public override int width => _width;
        public override int depth => _depth; //:h3ml
        public override void add_element(element el)
        {
            el._skip = false;
            el._box = null;
            var add = true;
            if ((_items.Count == 0 && el.is_white_space()) || el.is_break())
                el._skip = true;
            else if (el.is_white_space())
                if (have_last_space())
                {
                    add = false;
                    el._skip = true;
                }
            if (add)
            {
                el._box = this;
                _items.Add(el);
                if (!el._skip)
                {
                    var el_shift_left = el.get_inline_shift_left();
                    var el_shift_right = el.get_inline_shift_right();
                    el._pos.x = _box_left + _width + el_shift_left + el.content_margins_left;
                    el._pos.y = _box_top + el.content_margins_top;
                    el._pos.z = _box_front + el.content_margins_front; //:h3ml
                    _width += el.width + el_shift_left + el_shift_right;
                    //_depth += el.depth + el_shift_front + el_shift_back; //:h3ml
                }
            }
        }

        public override bool can_hold(element el, white_space ws)
        {
            if (!el.is_inline_box) return false;
            if (el.is_break()) return false;
            if (ws == white_space.nowrap || ws == white_space.pre) return true;
            if (_box_left + _width + el.width + el.get_inline_shift_left() + el.get_inline_shift_right() > _box_right) return false;
            return true;
        }

        public override void finish(bool last_box = false)
        {
            if (is_empty || (!is_empty && last_box && is_break_only()))
            {
                _height = 0;
                return;
            }

            foreach (var i in _items)
                if (i.is_white_space() || i.is_break())
                {
                    if (!i._skip)
                    {
                        i._skip = true;
                        _width -= i.width;
                    }
                }
                else break;

            var base_line = _font_metrics.base_line;
            var line_height = _line_height;

            var add_x = 0;
            switch (_text_align)
            {
                case text_align.right:
                    if (_width < (_box_right - _box_left))
                        add_x = _box_right - _box_left - _width;
                    break;
                case text_align.center:
                    if (_width < (_box_right - _box_left))
                        add_x = (_box_right - _box_left - _width) / 2;
                    break;
                default:
                    add_x = 0;
                    break;
            }

            _height = 0;
            // find line box baseline and line-height
            foreach (var el in _items)
            {
                if (el.get_display == style_display.inline_text)
                {
                    el.get_font(out var fm);
                    base_line = Math.Max(base_line, fm.base_line);
                    line_height = Math.Max(line_height, el.line_height);
                    _height = Math.Max(_height, fm.height);
                }
                el._pos.x += add_x;
            }

            if (_height != 0)
                base_line += (line_height - _height) / 2;

            _height = line_height;

            var y1 = 0;
            var y2 = _height;

            foreach (var el in _items)
                if (el.get_display == style_display.inline_text)
                {
                    el.get_font(out var fm);
                    el._pos.y = _height - base_line - fm.ascent;
                }
                else
                {
                    switch (el.get_vertical_align)
                    {
                        case vertical_align.super:
                        case vertical_align.sub:
                        case vertical_align.baseline: el._pos.y = _height - base_line - el.height + el.get_base_line() + el.content_margins_top; break;
                        case vertical_align.top: el._pos.y = y1 + el.content_margins_top; break;
                        case vertical_align.text_top: el._pos.y = _height - base_line - _font_metrics.ascent + el.content_margins_top; break;
                        case vertical_align.middle: el._pos.y = _height - base_line - _font_metrics.x_height / 2 - el.height / 2 + el.content_margins_top; break;
                        case vertical_align.bottom: el._pos.y = y2 - el.height + el.content_margins_top; break;
                        case vertical_align.text_bottom: el._pos.y = _height - base_line + _font_metrics.descent - el.height + el.content_margins_top; break;
                    }
                    y1 = Math.Min(y1, el.top);
                    y2 = Math.Max(y2, el.bottom);
                }

            //css_offsets offsets;
            foreach (var el in _items)
            {
                el._pos.y -= y1;
                el._pos.y += _box_top;
                if (el.get_display != style_display.inline_text)
                    switch (el.get_vertical_align)
                    {
                        case vertical_align.top: el._pos.y = _box_top + el.content_margins_top; break;
                        case vertical_align.bottom: el._pos.y = _box_top + (y2 - y1) - el.height + el.content_margins_top; break;
                        case vertical_align.baseline: break; //TODO: process vertical align "baseline"
                        case vertical_align.middle: break; //TODO: process vertical align "middle"
                        case vertical_align.sub: break; //TODO: process vertical align "sub"
                        case vertical_align.super: break; //TODO: process vertical align "super"
                        case vertical_align.text_bottom: break; //TODO: process vertical align "text-bottom"
                        case vertical_align.text_top: break; //TODO: process vertical align "text-top"
                    }
                el.apply_relative_shift(_box_right - _box_left);
            }
            _height = y2 - y1;
            _baseline = base_line - y1 - (_height - line_height);
        }

        public override bool is_empty
        {
            get
            {
                if (_items.Count == 0) return true;
                foreach (var i in _items)
                    if (!i._skip || i.is_break())
                        return false;
                return true;
            }
        }
        public override int baseline => _baseline;
        public override void get_elements(IList<element> els) => ((List<element>)els).AddRange(_items);

        public override int top_margin => 0;
        public override int bottom_margin => 0;
        public override void y_shift(int shift)
        {
            _box_top += shift;
            foreach (var el in _items)
                el._pos.y += shift;
        }

        public override void new_width(int left, int right, IList<element> els)
        {
            var add = left - _box_left;
            if (add != 0)
            {
                _box_left = left;
                _box_right = right;
                _width = 0;
                var remove_begin = _items.Count;
                for (var i = 2; i != _items.Count; i++)
                {
                    var el = _items[i - 1];
                    if (!el._skip)
                    {
                        if (_box_left + _width + el.width + el.get_inline_shift_right() + el.get_inline_shift_left() > _box_right)
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
                if (remove_begin != _items.Count)
                {
                    ((List<element>)els).AddRange(_items.Skip(remove_begin));
                    _items.RemoveRange(remove_begin + 1, _items.Count - remove_begin);
                    foreach (var el in els)
                        el._box = null;
                }
            }
        }

        bool have_last_space()
        {
            foreach (var i in _items)
                if (i.is_white_space() || i.is_break())
                    return true;
                else break;
            return false;
        }

        bool is_break_only()
        {
            if (_items.Count == 0) return true;
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
