using System.Collections.Generic;

namespace H3ml.Layout
{
    public class el_style : element
    {
        new List<element> _children = new List<element>();

        public el_style(document doc) : base(doc) { }

        public override void parse_attributes()
        {
            var text = string.Empty;
            foreach (var el in _children)
                el.get_text(ref text);
            get_document().add_stylesheet(text, null, get_attr("media"));
        }

        public override bool appendChild(element el) { _children.Add(el); return true; }

        public override string get_tagName() => "style";
	}
}
