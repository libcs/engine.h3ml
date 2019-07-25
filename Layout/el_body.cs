namespace H3ml.Layout
{
    public class el_body : html_tag
    {
        public el_body(document doc) : base(doc) { }

        public override bool is_body() => true;
    }
}
