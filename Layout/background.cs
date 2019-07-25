namespace H3ml.Layout
{
    public class background
    {
        public string _image;
        public string _baseurl;
        public web_color _color;
        public background_attachment _attachment;
        public css_position _position;
        public background_repeat _repeat;
        public background_box _clip;
        public background_box _origin;
        public css_border_radius _radius;

        public background()
        {
            _attachment = background_attachment_scroll;
            _repeat = background_repeat_repeat;
            _clip = background_box_border;
            _origin = background_box_padding;
            _color.alpha = 0;
            _color.red = 0;
            _color.green = 0;
            _color.blue = 0;
        }

        public background(background val)
        {
            _image = val._image;
            _baseurl = val._baseurl;
            _color = val._color;
            _attachment = val._attachment;
            _position = val._position;
            _repeat = val._repeat;
            _clip = val._clip;
            _origin = val._origin;
        }

        //public background operator=(background val)
        //{
        //    _image = val._image;
        //    _baseurl = val._baseurl;
        //    _color = val._color;
        //    _attachment = val._attachment;
        //    _position = val._position;
        //    _repeat = val._repeat;
        //    _clip = val._clip;
        //    _origin = val._origin;
        //    return this;
        //}
    }

    public class background_paint
    {
        public string image;
        public string baseurl;
        public background_attachment attachment;
        public background_repeat repeat;
        public web_color color;
        public position clip_box;
        public position origin_box;
        public position border_box;
        public border_radiuses border_radius;
        public size image_size;
        public int position_x;
        public int position_y;
        public bool is_root;

        public background_paint()
        {
            color = new web_color(0, 0, 0, 0);
            position_x = 0;
            position_y = 0;
            attachment = background_attachment_scroll;
            repeat = background_repeat_repeat;
            is_root = false;
        }
        public background_paint(background_paint val)
        {
            image = val.image;
            baseurl = val.baseurl;
            attachment = val.attachment;
            repeat = val.repeat;
            color = val.color;
            clip_box = val.clip_box;
            origin_box = val.origin_box;
            border_box = val.border_box;
            border_radius = val.border_radius;
            image_size = val.image_size;
            position_x = val.position_x;
            position_y = val.position_y;
            is_root = val.is_root;
        }
        //public void operator=(background val)
        //{
        //    attachment = val._attachment;
        //    baseurl = val._baseurl;
        //    image = val._image;
        //    repeat = val._repeat;
        //    color = val._color;
        //}
    }
}