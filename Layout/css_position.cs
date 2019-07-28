namespace H3ml.Layout
{
    public struct css_position
    {
        public css_length x;
        public css_length y;
        public css_length width;
        public css_length height;

        public css_position(css_position val)
        {
            x = val.x;
            y = val.y;
            width = val.width;
            height = val.height;
        }
    }
}
