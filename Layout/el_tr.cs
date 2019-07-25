namespace H3ml.Layout
{
	public class el_tr : html_tag
	{
		public el_tr(document doc) : base(doc) { }

        public override void parse_attributes()
		{
			const tchar_t* str = get_attr(_t("align"));
			if (str)
			{
				m_style.add_property(_t("text-align"), str, 0, false);
			}
			str = get_attr(_t("valign"));
			if (str)
			{
				m_style.add_property(_t("vertical-align"), str, 0, false);
			}
			str = get_attr(_t("bgcolor"));
			if (str)
			{
				m_style.add_property(_t("background-color"), str, 0, false);
			}
			html_tag::parse_attributes();
		}

		public override void get_inline_boxes(IList<position> boxes)
		{
			position pos;
			for (auto& el : m_children)
			{
				if (el->get_display() == display_table_cell)
				{
					pos.x = el->left() + el->margin_left();
					pos.y = el->top() - m_padding.top - m_borders.top;

					pos.width = el->right() - pos.x - el->margin_right() - el->margin_left();
					pos.height = el->height() + m_padding.top + m_padding.bottom + m_borders.top + m_borders.bottom;

					boxes.push_back(pos);
				}
			}
		}

	}
}
