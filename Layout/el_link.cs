namespace H3ml.Layout
{
    public class el_link : html_tag
    {
        public el_link(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            var processed = false;
            var doc = get_document();
            var rel = get_attr("rel");
            if (rel != null && rel == "stylesheet")
            {
                var media = get_attr("media");
                var href = get_attr("href");
                if (!string.IsNullOrEmpty(href))
                {
                    var css_baseurl = string.Empty;
                    doc.container.import_css(out var css_text, href, ref css_baseurl);
                    if (!string.IsNullOrEmpty(css_text))
                    {
                        doc.add_stylesheet(css_text, css_baseurl, media);
                        processed = true;
                    }
                }
            }
            if (!processed)
                doc.container.link(doc, this);
        }
    }
}
