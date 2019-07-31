namespace H3ml.Layout
{
    public class context
    {
        css _master_css;

        public void load_master_stylesheet(string str)
        {
            _master_css.parse_stylesheet(str, null, null, new media_query_list());
            _master_css.sort_selectors();
        }

        public css master_css => _master_css;
    }
}
