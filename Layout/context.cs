namespace H3ml.Layout
{
    public class context
    {
        css _master_css;

        public void load_master_stylesheet(string str)
        {
            //media_query_list media;
            //_master_css.parse_stylesheet(str, 0, std::shared_ptr<litehtml::document>(), media_query_list::ptr());
            _master_css.sort_selectors();
        }

        public css master_css => _master_css;
    }
}
