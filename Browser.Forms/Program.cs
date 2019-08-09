using H3ml.Browser;
using H3ml.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser.Forms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var html_context = new context();
            html_context.load_master_stylesheet(Master.master_css);

            Application.Run(new BrowserForm(html_context));
        }
    }
}
