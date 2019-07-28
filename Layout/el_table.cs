namespace H3ml.Layout
{
    public struct col_info
    {
        public int width;
        public bool is_auto;
    }

    public class el_table : html_tag
    {
        public el_table(document doc) : base(doc)
        {
            _border_spacing_x = 0;
            _border_spacing_y = 0;
            _border_collapse = border_collapse.separate;
        }

        public override bool appendChild(element el)
        {
            if (el == null) return false;
            return el.get_tagName() == "tbody" || el.get_tagName() == "thead" || el.get_tagName() == "tfoot" ? appendChild(el) : false;
        }

        public override void parse_styles(bool is_reparse = false)
        {
            parse_styles(is_reparse);
            _border_collapse = (border_collapse)html.value_index(get_style_property("border-collapse", true, "separate"), types.border_collapse_strings, (int)border_collapse.separate);
            if (_border_collapse == border_collapse.separate)
            {
                _css_border_spacing_x.fromString(get_style_property("-litehtml-border-spacing-x", true, "0px"));
                _css_border_spacing_y.fromString(get_style_property("-litehtml-border-spacing-y", true, "0px"));
                var fntsz = get_font_size();
                var doc = get_document();
                _border_spacing_x = doc.cvt_units(_css_border_spacing_x, fntsz);
                _border_spacing_y = doc.cvt_units(_css_border_spacing_y, fntsz);
            }
            else
            {
                _border_spacing_x = 0;
                _border_spacing_y = 0;
                _padding.bottom = 0;
                _padding.top = 0;
                _padding.left = 0;
                _padding.right = 0;
                _css_padding.bottom.set_value(0, css_units.px);
                _css_padding.top.set_value(0, css_units.px);
                _css_padding.left.set_value(0, css_units.px);
                _css_padding.right.set_value(0, css_units.px);
            }
        }

        public override void parse_attributes()
        {
            var str = get_attr("width"); if (str != null) _style.add_property("width", str, null, false);
            str = get_attr("align"); if (str != null)
            {
                var align = html.value_index(str, "left;center;right");
                switch (align)
                {
                    case 1: _style.add_property("margin-left", "auto", null, false); _style.add_property("margin-right", "auto", null, false); break;
                    case 2: _style.add_property("margin-left", "auto", null, false); _style.add_property("margin-right", "0", null, false); break;
                }
            }
            str = get_attr("cellspacing"); if (str != null) _style.add_property("border-spacing", $"{str} {str}", null, false);
            str = get_attr("border"); if (str != null) _style.add_property("border-width", str, null, false);
            str = get_attr("bgcolor"); if (str != null) _style.add_property("background-color", str, null, false);
            parse_attributes();
        }
    }
}
