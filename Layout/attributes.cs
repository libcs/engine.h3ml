using System;

namespace H3ml.Layout
{
    public struct attr_color
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbAlpha;

        public attr_color(Exception o = null)
        {
            rgbAlpha = 255;
            rgbBlue = 0;
            rgbGreen = 0;
            rgbRed = 0;
        }
    }

    public struct attr_border
    {
        public style_border border;
        public int width;
        public attr_color color;

        public attr_border(Exception o = null)
        {
            border = style_border.none;
            width = 0;
            color = default(attr_color);
        }
    }
}
