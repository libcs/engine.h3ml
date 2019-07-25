namespace H3ml.Layout
{
	public class el_script : element
	{
		string _text;
	
		public el_script(document doc) = > element = doc;
		

		public override void parse_attributes()
		{
			//TODO: pass script text to document container
		}
		public override bool appendChild(ptr el) 
		{
			el.get_text(m_text);
			return true;
		}

		public override string get_tagName() = > "script";
	}
}
