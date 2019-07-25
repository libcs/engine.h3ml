namespace H3ml.Layout
{
    public struct css_border
    {
        public css_length width;
        public border_style style;
        public web_color color;

        public css_border()
        {
            style = border_style_none;
        }

        public css_border(css_border val)
        {
            width = val.width;
            style = val.style;
            color = val.color;
        }

        //css_border operator=(css_border val)
        //{
        //    width = val.width;
        //    style = val.style;
        //    color = val.color;
        //    return this;
        //}
    }

    public struct border
    {
        public int width;
        public border_style style;
        public web_color color;

        public border()
        {
            width = 0;
        }
        public border(border val)
        {
            width = val.width;
            style = val.style;
            color = val.color;
        }
        public border(css_border val)
        {
            width = (int)val.width.val();
            style = val.style;
            color = val.color;
        }
        //public border operator=(border val)
        //{
        //    width = val.width;
        //    style = val.style;
        //    color = val.color;
        //    return this;
        //}
        //public border operator=(css_border val)
        //{
        //    width = (int)val.width.val();
        //    style = val.style;
        //    color = val.color;
        //    return this;
        //}
    }

    public struct border_radiuses
    {
        public int top_left_x;
        public int top_left_y;

        public int top_right_x;
        public int top_right_y;

        public int bottom_right_x;
        public int bottom_right_y;

        public int bottom_left_x;
        public int bottom_left_y;

        public border_radiuses()
        {
            top_left_x = 0;
            top_left_y = 0;
            top_right_x = 0;
            top_right_y = 0;
            bottom_right_x = 0;
            bottom_right_y = 0;
            bottom_left_x = 0;
            bottom_left_y = 0;
        }
        public border_radiuses(border_radiuses val)
        {
            top_left_x = val.top_left_x;
            top_left_y = val.top_left_y;
            top_right_x = val.top_right_x;
            top_right_y = val.top_right_y;
            bottom_right_x = val.bottom_right_x;
            bottom_right_y = val.bottom_right_y;
            bottom_left_x = val.bottom_left_x;
            bottom_left_y = val.bottom_left_y;
        }
        //public border_radiuses operator = (border_radiuses val)
        //{
        //    top_left_x = val.top_left_x;
        //    top_left_y = val.top_left_y;
        //    top_right_x = val.top_right_x;
        //    top_right_y = val.top_right_y;
        //    bottom_right_x = val.bottom_right_x;
        //    bottom_right_y = val.bottom_right_y;
        //    bottom_left_x = val.bottom_left_x;
        //    bottom_left_y = val.bottom_left_y;
        //    return this;
        //}
        public static border_radiuses operator +(border_radiuses t, margins mg)
        {
            t.top_left_x += mg.left;
            t.top_left_y += mg.top;
            t.top_right_x += mg.right;
            t.top_right_y += mg.top;
            t.bottom_right_x += mg.right;
            t.bottom_right_y += mg.bottom;
            t.bottom_left_x += mg.left;
            t.bottom_left_y += mg.bottom;
            t.fix_values();
            return t;
        }
        public static border_radiuses operator -(border_radiuses t, margins mg)
        {
            t.top_left_x -= mg.left;
            t.top_left_y -= mg.top;
            t.top_right_x -= mg.right;
            t.top_right_y -= mg.top;
            t.bottom_right_x -= mg.right;
            t.bottom_right_y -= mg.bottom;
            t.bottom_left_x -= mg.left;
            t.bottom_left_y -= mg.bottom;
            t.fix_values();
            return t;
        }
        void fix_values()
        {
            if (top_left_x < 0) top_left_x = 0;
            if (top_left_y < 0) top_left_y = 0;
            if (top_right_x < 0) top_right_x = 0;
            if (bottom_right_x < 0) bottom_right_x = 0;
            if (bottom_right_y < 0) bottom_right_y = 0;
            if (bottom_left_x < 0) bottom_left_x = 0;
            if (bottom_left_y < 0) bottom_left_y = 0;
        }
    }

    public struct css_border_radius
    {
        public css_length top_left_x;
        public css_length top_left_y;

        public css_length top_right_x;
        public css_length top_right_y;

        public css_length bottom_right_x;
        public css_length bottom_right_y;

        public css_length bottom_left_x;
        public css_length bottom_left_y;

        public css_border_radius() { }
        public css_border_radius(css_border_radius val)
        {
            top_left_x = val.top_left_x;
            top_left_y = val.top_left_y;
            top_right_x = val.top_right_x;
            top_right_y = val.top_right_y;
            bottom_left_x = val.bottom_left_x;
            bottom_left_y = val.bottom_left_y;
            bottom_right_x = val.bottom_right_x;
            bottom_right_y = val.bottom_right_y;
        }

        //public css_border_radius operator=(css_border_radius val)
        //{
        //    top_left_x = val.top_left_x;
        //    top_left_y = val.top_left_y;
        //    top_right_x = val.top_right_x;
        //    top_right_y = val.top_right_y;
        //    bottom_left_x = val.bottom_left_x;
        //    bottom_left_y = val.bottom_left_y;
        //    bottom_right_x = val.bottom_right_x;
        //    bottom_right_y = val.bottom_right_y;
        //    return this;
        //}
        border_radiuses calc_percents(int width, int height)
        {
            border_radiuses ret;
            ret.bottom_left_x = bottom_left_x.calc_percent(width);
            ret.bottom_left_y = bottom_left_y.calc_percent(height);
            ret.top_left_x = top_left_x.calc_percent(width);
            ret.top_left_y = top_left_y.calc_percent(height);
            ret.top_right_x = top_right_x.calc_percent(width);
            ret.top_right_y = top_right_y.calc_percent(height);
            ret.bottom_right_x = bottom_right_x.calc_percent(width);
            ret.bottom_right_y = bottom_right_y.calc_percent(height);
            return ret;
        }
    }

    public struct css_borders
    {
        public css_border left;
        public css_border top;
        public css_border right;
        public css_border bottom;
        public css_border_radius radius;

        public css_borders() { }
        public css_borders(css_borders val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
            radius = val.radius;
        }

        //public css_borders operator=(css_borders val)
        //{
        //    left = val.left;
        //    right = val.right;
        //    top = val.top;
        //    bottom = val.bottom;
        //    radius = val.radius;
        //    return this;
        //}
    }

    public struct borders
    {
        public border left;
        public border top;
        public border right;
        public border bottom;
        public border_radiuses radius;

        public borders() { }
        public borders(borders val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
            radius = val.radius;
        }

        public borders(css_borders val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
        }

        //public borders operator=(borders val)
        //{
        //    left = val.left;
        //    right = val.right;
        //    top = val.top;
        //    bottom = val.bottom;
        //    radius = val.radius;
        //    return this;
        //}

        //public borders operator=(css_borders val)
        //{
        //    left = val.left;
        //    right = val.right;
        //    top = val.top;
        //    bottom = val.bottom;
        //    return this;
        //}
    }
}
