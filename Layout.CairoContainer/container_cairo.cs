using Cairo;
using System;
using System.Collections.Generic;

namespace H3ml.Layout.Containers
{
    public class container_cairo : Idocument_container, IDisposable
    {
        class cairo_font
        {
            public FontFace font;
            public int size;
            public bool underline;
            public bool strikeout;
        }

        ImageSurface _temp_surface;
        Context _temp_cr;
        //images_map m_images;
        //List<cairo_clip_box> _clips;

        public container_cairo()
        {
            _temp_surface = new ImageSurface(Format.Argb32, 2, 2);
            _temp_cr = new Context(_temp_surface);
        }

        public void Dispose()
        {
            //clear_images();
            _temp_surface.Dispose();
            _temp_cr.Dispose();
        }

        public element create_element(string tag_name, Dictionary<string, string> attributes, document doc) => null;

        public object create_font(string faceName, int size, int weight, font_style italic, uint decoration, out font_metrics fm)
        {
            fm = new font_metrics();
            var fonts = new List<string>();
            html.split_string(faceName, fonts, ",");
            fonts[0].Trim();

            FontFace fnt = null;

            //using (var pattern = FcPatternCreate())
            //{
            //    var found = false;
            //    foreach (var i in fonts)
            //        if (FcPatternAddString(pattern, FC_FAMILY, i))
            //        {
            //            found = true;
            //            break;
            //        }
            //    if (found)
            //    {
            //        FcPatternAddInteger(pattern, FC_SLANT, italic == font_style.italic ? FontSlant.Italic : FontSlant.Normal);
            //        var fc_weight = FontWeight.Normal;
            //        if (weight >= 0 && weight < 150) fc_weight = FC_WEIGHT_THIN;
            //        else if (weight >= 150 && weight < 250) fc_weight = FC_WEIGHT_EXTRALIGHT;
            //        else if (weight >= 250 && weight < 350) fc_weight = FC_WEIGHT_LIGHT;
            //        else if (weight >= 350 && weight < 450) fc_weight = FC_WEIGHT_NORMAL;
            //        else if (weight >= 450 && weight < 550) fc_weight = FC_WEIGHT_MEDIUM;
            //        else if (weight >= 550 && weight < 650) fc_weight = FC_WEIGHT_SEMIBOLD;
            //        else if (weight >= 650 && weight < 750) fc_weight = FC_WEIGHT_BOLD;
            //        else if (weight >= 750 && weight < 850) fc_weight = FC_WEIGHT_EXTRABOLD;
            //        else if (weight >= 950) fc_weight = FC_WEIGHT_BLACK;
            //        FcPatternAddInteger(pattern, FC_WEIGHT, fc_weight);
            //        fnt = cairo_ft_font_face_create_for_pattern(pattern);
            //    }
            //}

            cairo_font ret = null;
            if (fnt != null)
            {
                _temp_cr.Save();
                _temp_cr.SetContextFontFace(fnt);
                _temp_cr.SetFontSize(size);
                var ext = _temp_cr.FontExtents;
                var tex = _temp_cr.TextExtents("x");

                fm.ascent = (int)ext.Ascent;
                fm.descent = (int)ext.Descent;
                fm.height = (int)(ext.Ascent + ext.Descent);
                fm.x_height = (int)tex.Height;
                _temp_cr.Restore();

                ret = new cairo_font
                {
                    font = fnt,
                    size = size,
                    strikeout = (decoration & types.font_decoration_linethrough) != 0,
                    underline = (decoration & types.font_decoration_underline) != 0
                };
            }
            return ret;
        }

        public void delete_font(object hFont)
        {
            var fnt = (cairo_font)hFont;
            if (fnt != null)
                fnt.font.Dispose();
        }

        public void del_clip() { }
        public void draw_background(object hdc, background_paint bg) { }
        public void draw_borders(object hdc, borders borders, position draw_pos, bool root) { }
        public void draw_list_marker(object hdc, list_marker marker) { }
        public void draw_text(object hdc, string text, object hFont, web_color color, position pos) { }
        public void get_client_rect(out position client) { client = default(position); }
        public string get_default_font_name() => null;
        public int get_default_font_size() => 0;
        public void get_image_size(string src, string baseurl, out size sz) { sz = default(size); }
        public void get_language(out string language, out string culture) { language = "en"; culture = string.Empty; }
        public void get_media_features(media_features media) { }
        public void import_css(out string text, string url, ref string baseurl) { text = null; }
        public void link(document doc, element el) { }
        public void load_image(string src, string baseurl, bool redraw_on_ready) { }
        public void on_anchor_click(string url, element el) { }
        public int pt_to_px(int pt) => 0;
        public string resolve_color(string color) => null;
        public void set_base_url(string base_url) { }
        public void set_caption(string caption) { }
        public void set_clip(position pos, border_radiuses bdr_radius, bool valid_x, bool valid_y) { }
        public void set_cursor(string cursor) { }
        public int text_width(string text, object hFont) => 0;
        public void transform_text(string text, text_transform tt) { }
    }
}
