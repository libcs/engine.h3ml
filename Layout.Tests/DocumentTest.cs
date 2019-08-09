using NUnit.Framework;
using System.Collections.Generic;

namespace H3ml.Layout
{
    public class DocumentTest
    {
        [Test]
        public void AddFontTest()
        {
            var doc = new document(new container_test(), null);
            font_metrics fm;
            doc.get_font(null, 0, "normal", "normal", null, out fm);
            doc.get_font("inherit", 0, "normal", "normal", null, out fm);
            doc.get_font("Arial", 0, "bold", "normal", "underline", out fm);
            doc.get_font("Arial", 0, "bold", "normal", "line-through", out fm);
            doc.get_font("Arial", 0, "bold", "normal", "overline", out fm);
        }

        [Test]
        public void RenderTest()
        {
            var doc = document.createFromString("<html>Body</html>", new container_test(), new context());
            doc.render(100, render_type.fixed_only);
            doc.render(100, render_type.no_fixed);
            doc.render(100, render_type.all);
        }

        [Test]
        public void DrawTest()
        {
            var doc = document.createFromString("<html>Body</html>", new container_test(), new context());
            doc.draw(null, 0, 0, new position(0, 0, 100, 100));
        }

        [Test]
        public void CvtUnitsTest()
        {
            var doc = new document(new container_test(), null);
            doc.cvt_units("", 10, out var is_percent);
            var c = new css_length();
            c.fromString("10%"); doc.cvt_units(c, 10, 100);
            c.fromString("10em"); doc.cvt_units(c, 10, 100);
            c.fromString("10pt"); doc.cvt_units(c, 10, 100);
            c.fromString("10in"); doc.cvt_units(c, 10, 100);
            c.fromString("10cm"); doc.cvt_units(c, 10, 100);
            c.fromString("10mm"); doc.cvt_units(c, 10, 100);
            c.fromString("10vm"); doc.cvt_units(c, 10, 100);
            c.fromString("10vh"); doc.cvt_units(c, 10, 100);
            c.fromString("10vmin"); doc.cvt_units(c, 10, 100);
            c.fromString("10vmax"); doc.cvt_units(c, 10, 100);
            c.fromString("10"); doc.cvt_units(c, 10, 100);
        }

        [Test]
        public void MouseEventsTest()
        {
            var doc = new document(new container_test(), null);
            var redraw_boxes = new List<position>();
            doc.on_mouse_over(0, 0, 0, 0, redraw_boxes);
            doc.on_lbutton_down(0, 0, 0, 0, redraw_boxes);
            doc.on_lbutton_up(0, 0, 0, 0, redraw_boxes);
            doc.on_mouse_leave(redraw_boxes);
        }

        [Test]
        public void CreateElementTest()
        {
            var doc = new document(new container_test(), null);
            doc.create_element("container", new Dictionary<string, string> { { "", "" } });
            doc.create_element("br", new Dictionary<string, string> { { "", "" } });
            doc.create_element("p", new Dictionary<string, string> { { "", "" } });
            doc.create_element("img", new Dictionary<string, string> { { "", "" } });
            doc.create_element("table", new Dictionary<string, string> { { "", "" } });
            doc.create_element("td", new Dictionary<string, string> { { "", "" } });
            doc.create_element("th", new Dictionary<string, string> { { "", "" } });
            doc.create_element("link", new Dictionary<string, string> { { "", "" } });
            doc.create_element("title", new Dictionary<string, string> { { "", "" } });
            doc.create_element("a", new Dictionary<string, string> { { "", "" } });
            doc.create_element("tr", new Dictionary<string, string> { { "", "" } });
            doc.create_element("style", new Dictionary<string, string> { { "", "" } });
            doc.create_element("base", new Dictionary<string, string> { { "", "" } });
            doc.create_element("div", new Dictionary<string, string> { { "", "" } });
            doc.create_element("script", new Dictionary<string, string> { { "", "" } });
            doc.create_element("font", new Dictionary<string, string> { { "", "" } });
            doc.create_element("tag", new Dictionary<string, string> { { "", "" } });
        }

        [Test]
        public void DeviceChangeTest()
        {
            var doc = new document(new container_test(), null);
            doc.media_changed();
            doc.lang_changed();
        }

        [Test]
        public void ParseTest()
        {
            var container = new container_test();
            var ctx = new context();
            document.createFromString("", container, ctx);
        }
    }
}
