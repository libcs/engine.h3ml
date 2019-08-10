using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace H3ml.Layout
{
    public class ContextTest
    {
        public static readonly string master_css;

        static ContextTest()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Layout.Tests.master.css"))
            using (var reader = new StreamReader(stream))
                master_css = reader.ReadToEnd();
        }

        [Test]
        public void Test()
        {
            var html_context = new context();
            html_context.load_master_stylesheet(master_css);
        }
    }
}
