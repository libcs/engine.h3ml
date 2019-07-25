namespace H3ml.Layout
{
    public class el_break : html_tag
    {
        public el_break(document doc) : base(doc) { }

        public override bool is_break() => true;
    }
}
