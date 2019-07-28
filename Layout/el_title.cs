namespace H3ml.Layout
{
    public class el_title : html_tag
    {
        public el_title(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            var text = string.Empty;
            get_text(ref text);
            get_document().container.set_caption(text);
        }
    }
}
