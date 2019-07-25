namespace H3ml.Layout
{
	public class el_space : el_text
	{
	public el_space(string text, document doc) : base(text, doc) { }
 

		public override bool is_white_space()
{
	white_space ws = get_white_space();
	if (ws == white_space_normal ||
		ws == white_space_nowrap ||
		ws == white_space_pre_line)
	{
		return true;
	}
	return false;
}

		public override bool is_break()
{
	white_space ws = get_white_space();
	if (ws == white_space_pre ||
		ws == white_space_pre_line ||
		ws == white_space_pre_wrap)
	{
		if (m_text == _t("\n"))
		{
			return true;
		}
	}
	return false;
}

	}
}
