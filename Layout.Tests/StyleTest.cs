using H3ml.Layout;
using NUnit.Framework;

namespace H3ml
{
    public class StyleTest
    {
        [Test]
        public void AddTest()
        {
            var style = new style();
            style.add("border: 5px solid red; background-image: value", "base");
            style.add("border: 5px solid red!important; background-image: value", "base");
        }

        [Test]
        public void AddPropertyTest()
        {
            var style = new style();
            style.add_property("background-image", "value", "base", false);
            style.add_property("border-spacing", "1", null, false);
            style.add_property("border-spacing", "1 2", null, false);
            style.add_property("border", "5px solid red", null, false);
            style.add_property("border-left", "5px solid red", null, false);
            style.add_property("border-right", "5px solid red", null, false);
            style.add_property("border-top", "5px solid red", null, false);
            style.add_property("border-bottom", "5px solid red", null, false);
            style.add_property("border-bottom-left-radius", "1", null, false);
            style.add_property("border-bottom-left-radius", "1 2", null, false);
            style.add_property("border-bottom-right-radius", "1", null, false);
            style.add_property("border-bottom-right-radius", "1 2", null, false);
            style.add_property("border-top-right-radius", "1", null, false);
            style.add_property("border-top-right-radius", "1 2", null, false);
            style.add_property("border-top-left-radius", "1", null, false);
            style.add_property("border-top-left-radius", "1 2", null, false);
            style.add_property("border-radius", "1", null, false);
            style.add_property("border-radius", "1 2", null, false);
            style.add_property("border-radius-x", "1", null, false);
            style.add_property("border-radius-x", "1 2", null, false);
            style.add_property("border-radius-x", "1 2 3", null, false);
            style.add_property("border-radius-x", "1 2 3 4", null, false);
            style.add_property("border-radius-y", "1", null, false);
            style.add_property("border-radius-y", "1 2", null, false);
            style.add_property("border-radius-y", "1 2 3", null, false);
            style.add_property("border-radius-y", "1 2 3 4", null, false);
            style.add_property("list-style-image", "value", "base", false);
            style.add_property("background", "url(value)", "base", false);
            style.add_property("background", "repeat", null, false);
            style.add_property("background", "fixed", null, false);
            style.add_property("background", "border-box", null, false);
            style.add_property("background", "border-box border-box", null, false);
            style.add_property("background", "left", null, false);
            style.add_property("background", "1", null, false);
            style.add_property("background", "-1", null, false);
            style.add_property("background", "-1", null, false);
            style.add_property("background", "+1", null, false);
            style.add_property("background", "left 1", null, false);
            style.add_property("background", "red", null, false);
            style.add_property("margin", "1", null, false);
            style.add_property("margin", "1 2", null, false);
            style.add_property("margin", "1 2 3", null, false);
            style.add_property("margin", "1 2 3 4", null, false);
            style.add_property("padding", "1", null, false);
            style.add_property("padding", "1 2", null, false);
            style.add_property("padding", "1 2 3", null, false);
            style.add_property("padding", "1 2 3 4", null, false);
            style.add_property("border-left", "TBD", null, false);
            style.add_property("border-left", "TBD", null, false);
            style.add_property("border-left", "TBD", null, false);
            style.add_property("border-left", "TBD", null, false);
            style.add_property("border-right", "TBD", null, false);
            style.add_property("border-right", "TBD", null, false);
            style.add_property("border-right", "TBD", null, false);
            style.add_property("border-right", "TBD", null, false);
            style.add_property("border-top", "TBD", null, false);
            style.add_property("border-top", "TBD", null, false);
            style.add_property("border-top", "TBD", null, false);
            style.add_property("border-top", "TBD", null, false);
            style.add_property("border-bottom", "TBD", null, false);
            style.add_property("border-bottom", "TBD", null, false);
            style.add_property("border-bottom", "TBD", null, false);
            style.add_property("border-bottom", "TBD", null, false);
            style.add_property("border-width", "1", null, false);
            style.add_property("border-width", "1 2", null, false);
            style.add_property("border-width", "1 2 3", null, false);
            style.add_property("border-width", "1 2 3 4", null, false);
            style.add_property("border-style", "1", null, false);
            style.add_property("border-style", "1 2", null, false);
            style.add_property("border-style", "1 2 3", null, false);
            style.add_property("border-style", "1 2 3 4", null, false);
            style.add_property("border-color", "1", null, false);
            style.add_property("border-color", "1 2", null, false);
            style.add_property("border-color", "1 2 3", null, false);
            style.add_property("border-color", "1 2 3 4", null, false);
            style.add_property("font", "TBD", null, false);
            style.add_property("font", "TBD", null, false);
            style.add_property("font", "TBD", null, false);
            style.add_property("font", "TBD", null, false);
            style.add_property("unknown", "value", null, false);
        }
    }
}
