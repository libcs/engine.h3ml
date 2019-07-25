namespace H3ml.Layout
{
	public class el_style : element
	{
		IList<element>	_children;

		public el_style(document doc) => element = doc;

		public override void parse_attributes()
		{
			tstring text;

			for (auto& el : m_children)
			{
				el->get_text(text);
			}
			get_document()->add_stylesheet(text.c_str(), 0, get_attr(_t("media")));
		}

		public override bool appendChild(ptr el)
		{
			m_children.push_back(el);
			return true;
		}

		public override string get_tagName() = > "style";
	}
}
