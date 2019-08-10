namespace H3ml.Layout
{
    public struct css_position
    {
        public css_length x;
        public css_length y;
        public css_length z; //:h3ml
        public css_length width;
        public css_length height;
        public css_length depth; //:h3ml

        public css_position(css_position val)
        {
            x = val.x;
            y = val.y;
            z = val.z; //:h3ml
            width = val.width;
            height = val.height;
            depth = val.depth;
        }
    }
}
