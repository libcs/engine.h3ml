namespace H3ml.Layout
{
    public class context
    {
        public void load_master_stylesheet(string str)
        {
            master_css.parse_stylesheet(str, null, null, null);
            master_css.sort_selectors();
        }

        public css master_css { get; } = new css();
    }
}
