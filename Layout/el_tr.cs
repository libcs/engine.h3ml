using System.Collections.Generic;

namespace H3ml.Layout
{
	public class el_tr : html_tag
	{
		public el_tr(document doc) : base(doc) { }

        public override void parse_attributes()
		{
			var str = get_attr("align"); if (str != null) _style.add_property("text-align", str, null, false);
			str = get_attr("valign"); if (str != null) _style.add_property("vertical-align", str, null, false);
			str = get_attr("bgcolor"); if (str != null) _style.add_property("background-color", str, null, false);
			parse_attributes();
		}

		public override void get_inline_boxes(IList<position> boxes)
		{
			position pos;
			foreach (var el in _children)
				if (el.get_display == style_display.table_cell)
				{
					pos.x = el.left + el.margin_left;
					pos.y = el.top - _padding.top - _borders.top;
					pos.z = el.front - _padding.front - _borders.front;
					pos.width = el.right - pos.x - el.margin_right - el.margin_left;
					pos.height = el.height + _padding.top + _padding.bottom + _borders.top + _borders.bottom; //:h3ml
                    pos.depth = el.depth + _padding.front + _padding.back + _borders.front + _borders.back; //:h3ml
					boxes.Add(pos);
				}
		}
	}
}
