namespace H3ml.Layout
{
    public struct css_margins
    {
        public css_length left;
        public css_length right;
        public css_length top;
        public css_length bottom;
        public css_length front; //:h3ml
        public css_length back; //:h3ml

        public css_margins(css_margins val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
            front = val.front; //:h3ml
            back = val.back; //:h3ml
        }
    }
}
