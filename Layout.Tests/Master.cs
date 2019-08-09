using System.IO;
using System.Reflection;

namespace H3ml.Layout
{
    public static class Master
    {
        public static readonly string master_css;

        static Master()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Layout.Tests.master.css"))
            using (var reader = new StreamReader(stream))
                master_css = reader.ReadToEnd();
        }
    }
}
