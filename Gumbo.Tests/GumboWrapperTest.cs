using NUnit.Framework;
using System.Linq;

namespace Gumbo
{
    public class GumboWrapperTest
    {
        [Test]
        public void TestFirstAndLastTagsInEnum()
        {
            string testHtml = "<html><head><head><body><title></title><base></base><tt></tt><unknown123></unknown123></body></html>";
            using (var gumbo = new GumboWrapper(testHtml))
            {
                var list = gumbo.Document.Root.Children.OfType<ElementWrapper>().ToList();
                Assert.AreEqual(GumboTag.GUMBO_TAG_HEAD, list[0].Tag);
                Assert.AreEqual("<head>", list[0].OriginalTag);
                var body = list[1].Children.OfType<ElementWrapper>().ToList();
                Assert.AreEqual(GumboTag.GUMBO_TAG_TITLE, body[0].Tag);
                Assert.AreEqual(GumboTag.GUMBO_TAG_BASE, body[1].Tag);
                Assert.AreEqual(GumboTag.GUMBO_TAG_TT, body[2].Tag);
                Assert.AreEqual(GumboTag.GUMBO_TAG_UNKNOWN, body[3].Tag);
            }
        }

        [Test]
        public void TestHeadBody()
        {
            string testHtml = "<html><body class=\"gumbo\">привет!</body></html>";
            using (var gumbo = new GumboWrapper(testHtml))
            {
                var list = gumbo.Document.Root.Children.OfType<ElementWrapper>().ToList();
                Assert.AreEqual(GumboTag.GUMBO_TAG_HEAD, list[0].Tag);
                Assert.AreEqual(null, list[0].OriginalTag);
                Assert.AreEqual(GumboTag.GUMBO_TAG_BODY, list[1].Tag);
            }
        }

        [Test]
        public void TestAttributes()
        {
            string testHtml = "<html><body class=\"gumbo\">привет!</body></html>";
            using (var gumbo = new GumboWrapper(testHtml))
            {
                var list = gumbo.Document.Root.Children.OfType<ElementWrapper>().ToList();
                Assert.AreEqual("class", list[1].Attributes.First().Name);
                Assert.AreEqual("gumbo", list[1].Attributes.First().Value);
            }
        }
    }
}
