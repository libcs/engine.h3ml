namespace H3ml.Layout
{
    public struct css_margins
    {
        public css_length left;
        public css_length right;
        public css_length top;
        public css_length bottom;

        public css_margins() { }
        public css_margins(css_margins val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
        }

        //public css_margins operator=(css_margins val)
        //{
        //    left = val.left;
        //    right = val.right;
        //    top = val.top;
        //    bottom = val.bottom;
        //    return this;
        //}
    }
}
