namespace H3ml.Layout
{
    public class el_para : html_tag
    {
        public el_para(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            var str = get_attr("align"); if (str != null) _style.add_property("text-align", str, null, false);
            parse_attributes();
        }
    }
}