namespace H3ml.Layout
{
	public class el_div : html_tag
	{
		public el_div(document doc) = > html_tag = doc;

		public override void parse_attributes()
		{
			const tchar_t* str = get_attr(_t("align"));
			if (str)
			{
				m_style.add_property(_t("text-align"), str, 0, false);
			}
			html_tag::parse_attributes();
		}
	}
}
