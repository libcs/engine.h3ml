namespace H3ml.Browser
{
    public class main
    {
        string master_css = null;

        public int main()
        {
            var app = Application.create("litehtml.browser");

            var html_context = new context();
            html_context.load_master_stylesheet(master_css);

            var win = new browser_window(html_context);

            return app.run(win);
        }
    }
}