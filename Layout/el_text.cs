using System;

namespace H3ml.Layout
{
    public class el_text : element
    {
        protected string _text;
        protected string _transformed_text;
        protected size _size;
        protected text_transform _text_transform;
        protected bool _use_transformed;
        protected bool _draw_spaces;

        public el_text(string text, document doc) : base(doc)
        {
            if (text != null)
                _text = text;
            _text_transform = text_transform.none;
            _use_transformed = false;
            _draw_spaces = true;
        }

        public override void get_text(ref string text) => text += _text;

        public override string get_style_property(string name, bool inherited, string def = null)
        {
            if (inherited)
            {
                var el_parent = parent();
                if (el_parent != null)
                    return el_parent.get_style_property(name, inherited, def);
            }
            return def;
        }

        public override void parse_styles(bool is_reparse)
        {
            _text_transform = (text_transform)html.value_index(get_style_property("text-transform", true, "none"), types.text_transform_strings, (int)text_transform.none);
            if (_text_transform != text_transform.none)
            {
                _transformed_text = _text;
                _use_transformed = true;
                get_document().container.transform_text(_transformed_text, _text_transform);
            }

            if (is_white_space()) { _transformed_text = " "; _use_transformed = true; }
            else
            {
                if (_text == "\t") { _transformed_text = "    "; _use_transformed = true; }
                if (_text == "\n" || _text == "\r") { _transformed_text = ""; _use_transformed = true; }
            }

            object font = null; font_metrics fm;
            var el_parent = parent();
            if (el_parent != null)
                font = el_parent.get_font(out fm);
            else fm = new font_metrics();
            if (is_break())
            {
                _size.height = 0;
                _size.width = 0;
            }
            else
            {
                _size.height = fm.height;
                _size.width = get_document().container.text_width(_use_transformed ? _transformed_text : _text, font);
            }
            _draw_spaces = fm.draw_spaces;
        }

        public override int get_base_line()
        {
            var el_parent = parent();
            return el_parent != null ? el_parent.get_base_line() : 0;
        }

        public override void draw(object hdc, int x, int y, position clip)
        {
            if (is_white_space() && !_draw_spaces)
                return;
            var pos = _pos;
            pos.x += x;
            pos.y += y;
            if (pos.does_intersect(clip))
            {
                var el_parent = parent();
                if (el_parent != null)
                {
                    var doc = get_document();
                    var font = el_parent.get_font(out var junk);
                    var color = el_parent.get_color("color", true, doc.get_def_color);
                    doc.container.draw_text(hdc, _use_transformed ? _transformed_text : _text, font, color, pos);
                }
            }
        }

        public override int line_height => parent()?.line_height ?? 0;

        public override object get_font(out font_metrics fm)
        {
            var el_parent = parent();
            if (el_parent != null) return el_parent.get_font(out fm);
            fm = default(font_metrics);
            return IntPtr.Zero;
        }

        public override style_display get_display => style_display.inline_text;

        public override white_space get_white_space => parent()?.get_white_space ?? white_space.normal;

        public override element_position get_element_position(out css_offsets offsets)
        {
            var p = parent();
            while (p != null && p.get_display == style_display.inline)
            {
                if (p.get_element_position(out offsets) == element_position.relative)
                {
                    offsets = p.get_css_offsets();
                    return element_position.relative;
                }
                p = p.parent();
            }
            offsets = default(css_offsets);
            return element_position.@static;
        }

        public override css_offsets get_css_offsets()
        {
            var p = parent();
            while (p != null && p.get_display == style_display.inline)
            {
                if (p.get_element_position(out var junk) == element_position.relative)
                    return p.get_css_offsets();
                p = p.parent();
            }
            return new css_offsets();
        }

        public override void get_content_size(out size sz, int max_width) => sz = _size;
    }
}
