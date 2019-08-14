using NUnit.Framework;

namespace H3ml.Layout
{
    public class LayoutGlobalTest
    {
        [Test]
        public void Test()
        {
            var doc = document.createFromString("<html>Body</html>", new container_test(), new context());
            doc.render(500, render_type.all);
        }
    }
}
