namespace H3ml.Layout
{
    public class el_font : html_tag
    {
        public el_font(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            var str = get_attr("color"); if (str != null) _style.add_property("color", str, null, false);
            str = get_attr("face");
            if (str != null) _style.add_property("font-face", str, null, false);
            str = get_attr("size"); if (str != null)
            {
                var sz = int.Parse(str);
                if (sz <= 1) _style.add_property("font-size", "x-small", null, false);
                else if (sz >= 6) _style.add_property("font-size", "xx-large", null, false);
                else switch (sz)
                {
                    case 2: _style.add_property("font-size", "small", null, false); break;
                    case 3: _style.add_property("font-size", "medium", null, false); break;
                    case 4: _style.add_property("font-size", "large", null, false); break;
                    case 5: _style.add_property("font-size", "x-large", null, false); break;
                }
            }
            base.parse_attributes();
        }
    }
}
