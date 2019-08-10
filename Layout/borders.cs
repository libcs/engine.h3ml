namespace H3ml.Layout
{
    public struct css_border
    {
        public css_length width;
        public border_style style;
        public web_color color;

        public css_border(css_border val)
        {
            width = val.width;
            style = val.style;
            color = val.color;
        }
    }

    public struct border
    {
        public int width;
        public border_style style;
        public web_color color;

        public border(border val)
        {
            width = val.width;
            style = val.style;
            color = val.color;
        }
        public border(css_border val)
        {
            width = (int)val.width.val;
            style = val.style;
            color = val.color;
        }
    }

    public struct border_radiuses
    {
        public int top_left_x;
        public int top_left_y;
        public int top_left_z; //:h3ml
        public int top_right_x;
        public int top_right_y;
        public int top_right_z; //:h3ml
        public int bottom_right_x;
        public int bottom_right_y;
        public int bottom_right_z; //:h3ml
        public int bottom_left_x;
        public int bottom_left_y;
        public int bottom_left_z; //:h3ml

        public border_radiuses(border_radiuses val)
        {
            top_left_x = val.top_left_x;
            top_left_y = val.top_left_y;
            top_left_z = val.top_left_z; //:h3ml
            top_right_x = val.top_right_x;
            top_right_y = val.top_right_y;
            top_right_z = val.top_right_z; //:h3ml
            bottom_right_x = val.bottom_right_x;
            bottom_right_y = val.bottom_right_y;
            bottom_right_z = val.bottom_right_z; //:h3ml
            bottom_left_x = val.bottom_left_x;
            bottom_left_y = val.bottom_left_y;
            bottom_left_z = val.bottom_left_z; //:h3ml
        }
        public static border_radiuses operator +(border_radiuses t, margins mg)
        {
            t.top_left_x += mg.left;
            t.top_left_y += mg.top;
            t.top_left_z += mg.front; //:h3ml
            t.top_right_x += mg.right;
            t.top_right_y += mg.top;
            t.top_right_z += mg.front; //:h3ml
            t.bottom_right_x += mg.right;
            t.bottom_right_y += mg.bottom;
            t.bottom_right_z += mg.back; //:h3ml
            t.bottom_left_x += mg.left;
            t.bottom_left_y += mg.bottom;
            t.bottom_left_z += mg.back; //:h3ml
            t.fix_values();
            return t;
        }
        public static border_radiuses operator -(border_radiuses t, margins mg)
        {
            t.top_left_x -= mg.left;
            t.top_left_y -= mg.top;
            t.top_left_z -= mg.front; //:h3ml
            t.top_right_x -= mg.right;
            t.top_right_y -= mg.top;
            t.top_right_z -= mg.front; //:h3ml
            t.bottom_right_x -= mg.right;
            t.bottom_right_y -= mg.bottom;
            t.bottom_right_z -= mg.back; //:h3ml
            t.bottom_left_x -= mg.left;
            t.bottom_left_y -= mg.bottom;
            t.bottom_left_z -= mg.back; //:h3ml
            t.fix_values();
            return t;
        }
        void fix_values()
        {
            if (top_left_x < 0) top_left_x = 0;
            if (top_left_y < 0) top_left_y = 0;
            if (top_left_z < 0) top_left_z = 0; //:h3ml
            if (top_right_x < 0) top_right_x = 0;
            if (top_right_y < 0) top_right_y = 0;
            if (top_right_z < 0) top_right_z = 0; //:h3ml
            if (bottom_right_x < 0) bottom_right_x = 0;
            if (bottom_right_y < 0) bottom_right_y = 0;
            if (bottom_right_z < 0) bottom_right_z = 0; //:h3ml
            if (bottom_left_x < 0) bottom_left_x = 0;
            if (bottom_left_y < 0) bottom_left_y = 0;
            if (bottom_left_z < 0) bottom_left_z = 0; //:h3ml
        }
    }

    public struct css_border_radius
    {
        public css_length top_left_x;
        public css_length top_left_y;
        public css_length top_left_z; //:h3ml
        public css_length top_right_x;
        public css_length top_right_y;
        public css_length top_right_z; //:h3ml
        public css_length bottom_right_x;
        public css_length bottom_right_y;
        public css_length bottom_right_z; //:h3ml
        public css_length bottom_left_x;
        public css_length bottom_left_y;
        public css_length bottom_left_z; //:h3ml

        public css_border_radius(css_border_radius val)
        {
            top_left_x = val.top_left_x;
            top_left_y = val.top_left_y;
            top_left_z = val.top_left_z; //:h3ml
            top_right_x = val.top_right_x;
            top_right_y = val.top_right_y;
            top_right_z = val.top_right_z; //:h3ml
            bottom_left_x = val.bottom_left_x;
            bottom_left_y = val.bottom_left_y;
            bottom_left_z = val.bottom_left_z; //:h3ml
            bottom_right_x = val.bottom_right_x;
            bottom_right_y = val.bottom_right_y;
            bottom_right_z = val.bottom_right_z; //:h3ml
        }

        public border_radiuses calc_percents(int width, int height, int depth) //:h3ml
        {
            border_radiuses ret;
            ret.bottom_left_x = bottom_left_x.calc_percent(width);
            ret.bottom_left_y = bottom_left_y.calc_percent(height);
            ret.bottom_left_z = bottom_left_z.calc_percent(depth); //:h3ml
            ret.top_left_x = top_left_x.calc_percent(width);
            ret.top_left_y = top_left_y.calc_percent(height);
            ret.top_left_z = top_left_z.calc_percent(depth); //:h3ml
            ret.top_right_x = top_right_x.calc_percent(width);
            ret.top_right_y = top_right_y.calc_percent(height);
            ret.top_right_z = top_right_z.calc_percent(depth); //:h3ml
            ret.bottom_right_x = bottom_right_x.calc_percent(width);
            ret.bottom_right_y = bottom_right_y.calc_percent(height);
            ret.bottom_right_z = bottom_right_z.calc_percent(depth); //:h3ml
            return ret;
        }
    }

    public struct css_borders
    {
        public css_border left;
        public css_border top;
        public css_border right;
        public css_border bottom;
        public css_border front; //:h3ml
        public css_border back; //:h3ml
        public css_border_radius radius;

        public css_borders(css_borders val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
            front = val.front; //:h3ml
            back = val.back; //:h3ml
            radius = val.radius;
        }
    }

    public struct borders
    {
        public border left;
        public border top;
        public border right;
        public border bottom;
        public border front; //:h3ml
        public border back; //:h3ml
        public border_radiuses radius;

        public borders(borders val)
        {
            left = val.left;
            right = val.right;
            top = val.top;
            bottom = val.bottom;
            front = val.front; //:h3ml
            back = val.back; //:h3ml
            radius = val.radius;
        }
        public borders(css_borders val)
        {
            left = new border(val.left);
            right = new border(val.right);
            top = new border(val.top);
            bottom = new border(val.bottom);
            front = new border(val.front); //:h3ml
            back = new border(val.back); //:h3ml
            radius = default(border_radiuses);
        }
    }
}
