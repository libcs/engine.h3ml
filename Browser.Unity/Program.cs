using H3ml.Layout;

namespace H3ml.Browser
{
    public class Program
    {
        static readonly string master_css = Master.master_css;

        public static void Main()
        {
            var html_context = new context();
            html_context.load_master_stylesheet(master_css);
            //var win = new browser_window(html_context);
        }
    }
}