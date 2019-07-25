namespace H3ml.Layout
{
    public class el_comment : element
    {
        string _text;

        public el_comment(document doc)
        {
            element = doc;
            _skip = true;
        }

        public override void get_text(ref string text)
        {
            text += _text;
        }

        public override void set_data(string data)
        {
            if (data != null)
                _text += data;
        }
    }
}
