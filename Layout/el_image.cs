using System;

namespace H3ml.Layout
{
    public class el_image : html_tag
    {
        string _src;

        public el_image(document doc) : base(doc) => _display = style_display.inline_block;

        public override int line_height => height;
        public override bool is_replaced => true;
        public override int render(int x, int y, int max_width, bool second_pass = false)
        {
            var parent_width = max_width;
            calc_outlines(parent_width);
            _pos.move_to(x, y);
            var doc = get_document();
            doc.container.get_image_size(_src, null, out var sz);
            _pos.width = sz.width;
            _pos.height = sz.height;
            if (_css_height.is_predefined && _css_width.is_predefined)
            {
                _pos.height = sz.height;
                _pos.width = sz.width;
                // check for max-height
                if (!_css_max_width.is_predefined)
                {
                    var max_width2 = doc.cvt_units(_css_max_width, _font_size, parent_width);
                    if (_pos.width > max_width2)
                        _pos.width = max_width2;
                    _pos.height = sz.width != 0 ? (int)(_pos.width * (float)sz.height / sz.width) : sz.height;
                }

                // check for max-height
                if (!_css_max_height.is_predefined)
                {
                    var max_height = doc.cvt_units(_css_max_height, _font_size);
                    if (_pos.height > max_height)
                        _pos.height = max_height;
                    _pos.width = sz.height != 0 ? (int)(_pos.height * (float)sz.width / sz.height) : sz.width;
                }
            }
            else if (!_css_height.is_predefined && _css_width.is_predefined)
            {
                if (!get_predefined_height(_pos.height))
                    _pos.height = (int)_css_height.val;
                // check for max-height
                if (!_css_max_height.is_predefined)
                {
                    var max_height = doc.cvt_units(_css_max_height, _font_size);
                    if (_pos.height > max_height)
                        _pos.height = max_height;
                }
                _pos.width = sz.height != 0 ? (int)(_pos.height * (float)sz.width / sz.height) : sz.width;
            }
            else if (_css_height.is_predefined && !_css_width.is_predefined)
            {
                _pos.width = _css_width.calc_percent(parent_width);
                // check for max-width
                if (!_css_max_width.is_predefined)
                {
                    var max_width2 = doc.cvt_units(_css_max_width, _font_size, parent_width);
                    if (_pos.width > max_width2)
                        _pos.width = max_width2;
                }
                _pos.height = sz.width != 0 ? (int)(_pos.width * (float)sz.height / sz.width) : sz.height;
            }
            else
            {
                _pos.width = _css_width.calc_percent(parent_width);
                _pos.height = 0;
                if (!get_predefined_height(_pos.height))
                    _pos.height = (int)_css_height.val;

                // check for max-height
                if (!_css_max_height.is_predefined)
                {
                    var max_height = doc.cvt_units(_css_max_height, _font_size);
                    if (_pos.height > max_height)
                        _pos.height = max_height;
                }

                // check for max-height
                if (!_css_max_width.is_predefined)
                {
                    var max_width2 = doc.cvt_units(_css_max_width, _font_size, parent_width);
                    if (_pos.width > max_width2)
                        _pos.width = max_width2;
                }
            }
            calc_auto_margins(parent_width);
            _pos.x += content_margins_left;
            _pos.y += content_margins_top;
            return _pos.width + content_margins_left + content_margins_right;
        }

        public override void parse_attributes()
        {
            _src = get_attr("src", "");
            var attr_height = get_attr("height"); if (attr_height != null) _style.add_property("height", attr_height, null, false);
            var attr_width = get_attr("width"); if (attr_width != null) _style.add_property("width", attr_width, null, false);
        }

        public override void parse_styles(bool is_reparse = false)
        {
            parse_styles(is_reparse);
            if (!string.IsNullOrEmpty(_src))
                get_document().container.load_image(_src, null, !_css_height.is_predefined && !_css_width.is_predefined);
        }

        public override void draw(object hdc, int x, int y, position clip)
        {
            var pos = _pos;
            pos.x += x;
            pos.y += y;
            var el_pos = pos;
            el_pos += _padding;
            el_pos += _borders;

            // draw standard background here
            if (el_pos.does_intersect(clip))
            {
                var bg = get_background();
                if (bg != null)
                {
                    var bg_paint = new background_paint();
                    init_background_paint(pos, bg_paint, bg);
                    get_document().container.draw_background(hdc, bg_paint);
                }
            }

            // draw image as background
            if (pos.does_intersect(clip))
            {
                if (pos.width > 0 && pos.height > 0)
                {
                    var bg = new background_paint();
                    bg.image = _src;
                    bg.clip_box = pos;
                    bg.origin_box = pos;
                    bg.border_box = pos;
                    bg.border_box += _padding;
                    bg.border_box += _borders;
                    bg.repeat = background_repeat.no_repeat;
                    bg.image_size.width = pos.width;
                    bg.image_size.height = pos.height;
                    bg.border_radius = _css_borders.radius.calc_percents(bg.border_box.width, bg.border_box.height);
                    bg.position_x = pos.x;
                    bg.position_y = pos.y;
                    get_document().container.draw_background(hdc, bg);
                }
            }

            // draw borders
            if (el_pos.does_intersect(clip))
            {
                var border_box = pos;
                border_box += _padding;
                border_box += _borders;
                var bdr = new borders(_css_borders);
                bdr.radius = _css_borders.radius.calc_percents(border_box.width, border_box.height);
                get_document().container.draw_borders(hdc, bdr, border_box, !have_parent);
            }
        }

        public override void get_content_size(out size sz, int max_width) => get_document().container.get_image_size(_src, null, out sz);
    }
}
