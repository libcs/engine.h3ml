namespace H3ml.Layout
{
    public class el_anchor : html_tag
    {
        public el_anchor(document doc) : base(doc) { }

        public override void on_click()
        {
            var href = get_attr("href"); if (href != null) get_document().container.on_anchor_click(href, this);
        }

        public override void apply_stylesheet(css stylesheet)
        {
            if (get_attr("href") != null) _pseudo_classes.Add("link");
            apply_stylesheet(stylesheet);
        }
    }
}
