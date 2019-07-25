namespace H3ml.Layout
{
	public class el_td : html_tag
	{
		public el_td(document doc) = > html_tag = doc;

		public override void parse_attributes()
		{
			const tchar_t* str = get_attr(_t("width"));
			if (str)
			{
				m_style.add_property(_t("width"), str, 0, false);
			}
			str = get_attr(_t("background"));
			if (str)
			{
				tstring url = _t("url('");
				url += str;
				url += _t("')");
				m_style.add_property(_t("background-image"), url.c_str(), 0, false);
			}
			str = get_attr(_t("align"));
			if (str)
			{
				m_style.add_property(_t("text-align"), str, 0, false);
			}

			str = get_attr(_t("bgcolor"));
			if (str)
			{
				m_style.add_property(_t("background-color"), str, 0, false);
			}

			str = get_attr(_t("valign"));
			if (str)
			{
				m_style.add_property(_t("vertical-align"), str, 0, false);
			}
			html_tag::parse_attributes();
		}
	}
}
