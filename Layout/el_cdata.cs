namespace H3ml.Layout
{
    public class el_cdata : element
    {
        string _text;

        public el_cdata(document doc) : base(doc) => _skip = true;

        public override void get_text(ref string text) => text += _text;

        public override void set_data(string data)
        {
            if (data != null)
                _text += data;
        }
    }
}
