using System.Collections.Generic;

namespace H3ml.Layout
{
    public class el_style : element
    {
        //IList<element> _children;

        public el_style(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            string text;
            foreach (var el in _children)
                el.get_text(ref text);
            get_document().add_stylesheet(text, 0, get_attr("media"));
        }

        public override bool appendChild(ptr el)
        {
            _children.Add(el);
            return true;
        }

        public override string get_tagName() => "style";
	}
}
