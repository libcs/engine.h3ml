using NUnit.Framework;

namespace H3ml.Layout
{
    public class WebColorTest
    {
        [Test]
        public void WebColorParseTest()
        {
            var callback = new container_test();
            web_color c;
            c = web_color.from_string("", callback); Assert.AreEqual(c.red, 0); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 0);
            c = web_color.from_string("#f0f", callback); Assert.AreEqual(c.red, 255); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 255);
            c = web_color.from_string("#ff00ff", callback); Assert.AreEqual(c.red, 255); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 255);
            c = web_color.from_string("rgb()", callback); Assert.AreEqual(c.red, 0); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 0);
            c = web_color.from_string("rgb(255,0,255)", callback); Assert.AreEqual(c.red, 255); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 255);
            c = web_color.from_string("red", callback); Assert.AreEqual(c.red, 255); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 0);
            c = web_color.from_string("unknown", callback); Assert.AreEqual(c.red, 0); Assert.AreEqual(c.green, 0); Assert.AreEqual(c.blue, 0);
        }
    }
}
