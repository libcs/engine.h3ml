namespace H3ml.Layout
{
    public class el_link : html_tag
    {
        public el_link(document doc) : base(doc) { }

        protected override void parse_attributes()
        {
            var processed = false;

            var doc = get_document();

            var rel = get_attr("rel");
            if (rel != null && rel == "stylesheet")
            {
                var media = get_attr("media");
                var href = get_attr("href");
                if (href && href[0])
                {
                    tstring css_text;
                    tstring css_baseurl;
                    doc->container()->import_css(css_text, href, css_baseurl);
                    if (!css_text.empty())
                    {
                        doc->add_stylesheet(css_text.c_str(), css_baseurl.c_str(), media);
                        processed = true;
                    }
                }
            }

            if (!processed)
                doc->container()->link(doc, shared_from_this());
        }
    }
}
