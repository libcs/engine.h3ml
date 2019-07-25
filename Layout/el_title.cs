namespace H3ml.Layout
{
	public class el_title : html_tag
	{
		public el_title(document doc) = > html_tag = doc;

		protected override void parse_attributes()
		{
			string text;
			get_text(text);
			get_document().container().set_caption(text);
		}
	}
}
