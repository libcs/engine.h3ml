namespace H3ml.Layout
{
    public struct css_offsets
    {
        public css_length left;
        public css_length top;
        public css_length right;
        public css_length bottom;

        public css_offsets(css_offsets val)
        {
            left = val.left;
            top = val.top;
            right = val.right;
            bottom = val.bottom;
        }
    }
}
