namespace H3ml.Layout
{
    public class el_base : html_tag
    {
        public el_base(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            get_document().container.set_base_url(get_attr("href"));
        }
    }
}
