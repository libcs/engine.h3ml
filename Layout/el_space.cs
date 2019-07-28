namespace H3ml.Layout
{
    public class el_space : el_text
    {
        public el_space(string text, document doc) : base(text, doc) { }

        public override bool is_white_space()
        {
            var ws = get_white_space();
            return ws == white_space.normal || ws == white_space.nowrap || ws == white_space.pre_line;
        }

        public override bool is_break()
        {
            var ws = get_white_space();
            return ws == white_space.pre || ws == white_space.pre_line || ws == white_space.pre_wrap && _text == "\n";
        }
    }
}
