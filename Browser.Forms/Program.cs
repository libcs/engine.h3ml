using H3ml.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser.Forms
{
    static class Program
    {
        public static readonly string master_css;

        static Program()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Browser.Forms.master.css"))
            using (var reader = new StreamReader(stream))
                master_css = reader.ReadToEnd();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var html_context = new context();
            html_context.load_master_stylesheet(master_css);

            Application.Run(new BrowserForm(html_context));
        }
    }
}
