using NUnit.Framework;

namespace H3ml.Layout
{
    public class CssTest
    {
        [Test]
        public void CssParseTest()
        {
            var doc = new document(new container_test(), null);
            var c = new css();
            c.parse_stylesheet("/*Comment*/", null, doc, null);
            c.parse_stylesheet("html { display: none }", null, doc, null);
            // https://www.w3schools.com/cssref/pr_import_rule.asp
            c.parse_stylesheet("@import \"navigation.css\"; /* Using a string */", null, doc, null);
            c.parse_stylesheet("@import url(\"navigation.css\"); /* Using a url */", null, doc, null);
            c.parse_stylesheet("@import \"navigation.css\"", null, doc, null);
            c.parse_stylesheet("@import \"printstyle.css\" print;", null, doc, null);
            c.parse_stylesheet("@import \"mobstyle.css\" screen and (max-width: 768px);", null, doc, null);
            // https://www.w3schools.com/cssref/css3_pr_mediaquery.asp
            c.parse_stylesheet("@media only screen and (max-width: 600px) { body { background-color: lightblue; } }", null, doc, null);
        }

        [Test]
        public void CssParseUrlTest()
        {
            css.parse_css_url("", out var url); Assert.IsEmpty(url);
            css.parse_css_url("value", out url); Assert.IsEmpty(url);
            css.parse_css_url("url()", out url); Assert.IsEmpty(url);
            css.parse_css_url("url(value)", out url); Assert.AreEqual(url, "value");
            css.parse_css_url("url('value')", out url); Assert.AreEqual(url, "value");
            css.parse_css_url("url(\"value\")", out url); Assert.AreEqual(url, "value");
        }

        [Test]
        public void CssLengthParseTest()
        {
            var length = new css_length();
            length.fromString("calc(todo)"); Assert.AreEqual(length.is_predefined, true); Assert.AreEqual(length.predef, 0); Assert.AreEqual(length.val, 0); Assert.AreEqual(length.units, css_units.none);
            length.fromString("top", "top;bottom", -1); Assert.AreEqual(length.is_predefined, true); Assert.AreEqual(length.predef, 0); Assert.AreEqual(length.val, 0); Assert.AreEqual(length.units, css_units.none);
            length.fromString("bottom", "top;bottom", -1); Assert.AreEqual(length.is_predefined, true); Assert.AreEqual(length.predef, 1); Assert.AreEqual(length.val, 0); Assert.AreEqual(length.units, css_units.none);
            length.fromString("bad", "top;bottom", -1); Assert.AreEqual(length.is_predefined, true); Assert.AreEqual(length.predef, -1); Assert.AreEqual(length.val, 0); Assert.AreEqual(length.units, css_units.none);
            length.fromString("123", "top;bottom", -1); Assert.AreEqual(length.is_predefined, false); Assert.AreEqual(length.predef, 0); Assert.AreEqual(length.val, 123); Assert.AreEqual(length.units, css_units.none);
            length.fromString("123px", "top;bottom", -1); Assert.AreEqual(length.is_predefined, false); Assert.AreEqual(length.predef, 0); Assert.AreEqual(length.val, 123); Assert.AreEqual(length.units, css_units.px);
        }

        [Test]
        public void CssElementSelectorParseTest()
        {
            var selector = new css_element_selector();
            // https://www.w3schools.com/cssref/css_selectors.asp
            selector.parse(".class"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "class"); Assert.AreEqual(selector._attrs[0].attribute, "class");
            selector.parse(".class1.class2"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 2); Assert.AreEqual(selector._attrs[0].val, "class1"); Assert.AreEqual(selector._attrs[1].val, "class2"); Assert.AreEqual(selector._attrs[0].attribute, "class");
            selector.parse("#id"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "id"); Assert.AreEqual(selector._attrs[0].attribute, "id");
            selector.parse("*"); Assert.AreEqual(selector._tag, "*"); Assert.AreEqual(selector._attrs.Count, 0);
            selector.parse("element"); Assert.AreEqual(selector._tag, "element"); Assert.AreEqual(selector._attrs.Count, 0);
            selector.parse("[attribute]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.IsNull(selector._attrs[0].val); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.exists);
            selector.parse("[attribute=value]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "value"); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.equal);
            selector.parse("[attribute~=value]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "value"); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.contain_str);
            selector.parse("[attribute|=value]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "value"); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.start_str);
            selector.parse("[attribute^=value]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "value"); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.start_str);
            selector.parse("[attribute$=value]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "value"); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.end_str);
            selector.parse("[attribute*=value]"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "value"); Assert.AreEqual(selector._attrs[0].attribute, "attribute"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.contain_str);
            selector.parse(":active"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "active"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse("::after"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "after"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo-el"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_element);
            selector.parse("::before"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "before"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo-el"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_element);
            selector.parse(":checked"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "checked"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":default"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "default"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":disabled"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "disabled"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":empty"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "empty"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":enabled"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "enabled"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":first-child"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "first-child"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse("::first-letter"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "first-letter"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo-el"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_element);
            selector.parse("::first-line"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "first-line"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo-el"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_element);
            selector.parse(":first-of-type"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "first-of-type"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":focus"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "focus"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":hover"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "hover"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":in-range"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "in-range"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":indeterminate"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "indeterminate"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":invalid"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "invalid"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":lang(language)"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "lang(language)"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":last-child"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "last-child"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":last-of-type"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "last-of-type"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":link"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "link"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":not(selector)"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "not(selector)"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":nth-child(n)"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "nth-child(n)"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":nth-last-child(n)"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "nth-last-child(n)"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":nth-last-of-type(n)"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "nth-last-of-type(n)"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":nth-of-type(n)"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "nth-of-type(n)"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":only-of-type"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "only-of-type"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":only-child"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "only-child"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":optional"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "optional"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":out-of-range"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "out-of-range"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse("::placeholder"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "placeholder"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo-el"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_element);
            selector.parse(":read-only"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "read-only"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":read-write"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "read-write"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":required"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "required"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":root"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "root"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse("::selection"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "selection"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo-el"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_element);
            selector.parse(":target"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "target"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":valid"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "valid"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            selector.parse(":visited"); Assert.IsEmpty(selector._tag); Assert.AreEqual(selector._attrs.Count, 1); Assert.AreEqual(selector._attrs[0].val, "visited"); Assert.AreEqual(selector._attrs[0].attribute, "pseudo"); Assert.AreEqual(selector._attrs[0].condition, attr_select_condition.pseudo_class);
            // other
            selector.parse("tag:psudo#anchor"); Assert.AreEqual(selector._tag, "tag"); Assert.AreEqual(selector._attrs.Count, 2);
        }

        [Test]
        public void CssSelectorParseTest()
        {
            var selector = new css_selector();
            // https://www.w3schools.com/cssref/css_selectors.asp
            Assert.IsFalse(selector.parse(string.Empty));
            Assert.IsTrue(selector.parse("element")); Assert.AreEqual(selector._combinator, css_combinator.descendant); Assert.AreEqual(selector._right._tag, "element"); Assert.AreEqual(selector._right._attrs.Count, 0); Assert.IsNull(selector._left);
            //Assert.IsTrue(selector.parse("element,element"));  Assert.AreEqual(selector._combinator, css_combinator.descendant); Assert.AreEqual(selector._right._tag, "element"); Assert.AreEqual(selector._right._attrs.Count, 0);
            Assert.IsTrue(selector.parse(".class1 .class2")); Assert.AreEqual(selector._combinator, css_combinator.descendant); Assert.IsEmpty(selector._right._tag); Assert.AreEqual(selector._right._attrs.Count, 1); Assert.AreEqual(selector._left._right._attrs.Count, 1);
            Assert.IsTrue(selector.parse("element element")); Assert.AreEqual(selector._combinator, css_combinator.descendant); Assert.AreEqual(selector._right._tag, "element"); Assert.AreEqual(selector._right._attrs.Count, 0); Assert.AreEqual(selector._left._right._tag, "element");
            Assert.IsTrue(selector.parse("element>element")); Assert.AreEqual(selector._combinator, css_combinator.child); Assert.AreEqual(selector._right._tag, "element"); Assert.AreEqual(selector._right._attrs.Count, 0); Assert.AreEqual(selector._left._right._tag, "element");
            Assert.IsTrue(selector.parse("element+element")); Assert.AreEqual(selector._combinator, css_combinator.adjacent_sibling); Assert.AreEqual(selector._right._tag, "element"); Assert.AreEqual(selector._right._attrs.Count, 0); Assert.AreEqual(selector._left._right._tag, "element");
            Assert.IsTrue(selector.parse("element1~element2")); Assert.AreEqual(selector._combinator, css_combinator.general_sibling); Assert.AreEqual(selector._right._tag, "element2"); Assert.AreEqual(selector._right._attrs.Count, 0); Assert.AreEqual(selector._left._right._tag, "element1");
        }

        [Test]
        public void StyleAddTest()
        {
            var style = new style();
            style.add("border: 5px solid red; background-image: value", "base");
            style.add("border: 5px solid red!important; background-image: value", "base");
        }

        [Test]
        public void StyleAddPropertyTest()
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
            style.add_property("border-front", "5px solid red", null, false); //:h3ml
            style.add_property("border-back", "5px solid red", null, false); //:h3ml
            style.add_property("border-bottom-left-radius", "1", null, false);
            style.add_property("border-bottom-left-radius", "1 2", null, false);
            style.add_property("border-bottom-left-radius", "1 2 3", null, false); //:h3ml
            style.add_property("border-bottom-right-radius", "1", null, false);
            style.add_property("border-bottom-right-radius", "1 2", null, false);
            style.add_property("border-bottom-right-radius", "1 2 3", null, false); //:h3ml
            style.add_property("border-top-right-radius", "1", null, false);
            style.add_property("border-top-right-radius", "1 2", null, false);
            style.add_property("border-top-right-radius", "1 2 3", null, false); //:h3ml
            style.add_property("border-top-left-radius", "1", null, false);
            style.add_property("border-top-left-radius", "1 2", null, false);
            style.add_property("border-top-left-radius", "1 2 3", null, false); //:h3ml
            style.add_property("border-radius", "1", null, false);
            style.add_property("border-radius", "1 2", null, false);
            style.add_property("border-radius", "1 2 3", null, false); //:h3ml
            style.add_property("border-radius-x", "1", null, false);
            style.add_property("border-radius-x", "1 2", null, false);
            style.add_property("border-radius-x", "1 2 3", null, false);
            style.add_property("border-radius-x", "1 2 3 4", null, false);
            style.add_property("border-radius-y", "1", null, false);
            style.add_property("border-radius-y", "1 2", null, false);
            style.add_property("border-radius-y", "1 2 3", null, false);
            style.add_property("border-radius-y", "1 2 3 4", null, false);
            style.add_property("border-radius-z", "1", null, false); //:h3ml
            style.add_property("border-radius-z", "1 2", null, false); //:h3ml
            style.add_property("border-radius-z", "1 2 3", null, false); //:h3ml
            style.add_property("border-radius-z", "1 2 3 4", null, false); //:h3ml
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
            style.add_property("border-front", "TBD", null, false); //:h3ml
            style.add_property("border-front", "TBD", null, false); //:h3ml
            style.add_property("border-front", "TBD", null, false); //:h3ml
            style.add_property("border-front", "TBD", null, false); //:h3ml
            style.add_property("border-back", "TBD", null, false); //:h3ml
            style.add_property("border-back", "TBD", null, false); //:h3ml
            style.add_property("border-back", "TBD", null, false); //:h3ml
            style.add_property("border-back", "TBD", null, false); //:h3ml
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
