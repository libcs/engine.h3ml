namespace H3ml.Layout
{
    public class el_td : html_tag
    {
        public el_td(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            var str = get_attr("width"); if (str != null) _style.add_property("width", str, null, false);
            str = get_attr("background"); if (str != null) _style.add_property("background-image", $"url('{str}')", null, false);
            str = get_attr("align"); if (str != null) _style.add_property("text-align", str, null, false);
            str = get_attr("bgcolor"); if (str != null) _style.add_property("background-color", str, null, false);
            str = get_attr("valign"); if (str != null) _style.add_property("vertical-align", str, null, false);
            base.parse_attributes();
        }
    }
}
