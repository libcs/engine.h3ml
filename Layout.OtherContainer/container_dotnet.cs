using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace H3ml.Layout.Containers
{
    public class container_dotnet : Control, document_container
    {
        Dictionary<string, object> _images = new Dictionary<string, object>();
        List<position> _clips = new List<position>();
        Rectangle _hClipRect;

        public void Dispose() { }

        public object create_font(string faceName, int size, int weight, font_style italic, uint decoration, out font_metrics fm)
        {
            fm = default(font_metrics);
            var fonts = new List<string>();
            html.split_string(faceName, fonts, ",");
            fonts[0].Trim();
            var fontStyle = italic == font_style.italic ? FontStyle.Italic : FontStyle.Regular;
            if ((decoration & types.font_decoration_linethrough) != 0)
                fontStyle |= FontStyle.Strikeout;
            if ((decoration & types.font_decoration_underline) != 0)
                fontStyle |= FontStyle.Underline;
            return new Font(fonts[0], weight, fontStyle);
        }

        public void delete_font(object hFont) => ((Font)hFont).Dispose();

        public virtual int line_height(object hdc, object hFont) => (int)((Graphics)hdc).MeasureString("X", (Font)hFont).Height;

        public virtual int text_width(object hdc, string text, object hFont) => (int)((Graphics)hdc).MeasureString(text, (Font)hFont).Width;

        public void draw_text(object hdc, string text, object hFont, web_color color, position pos)
        {
            apply_clip((Graphics)hdc);
            var rcText = new RectangleF(pos.left, pos.top, pos.right, pos.bottom);
            var format = new StringFormat();
            using (var brush = new SolidBrush(Color.FromArgb(color.red, color.green, color.blue)))
                ((Graphics)hdc).DrawString(text, (Font)hFont, brush, rcText, format);
            release_clip((Graphics)hdc);
        }

        public virtual void fill_rect(object hdc, position pos, web_color color, css_border_radius radius)
        {
            apply_clip((Graphics)hdc);
            fill_rect(hdc, pos.x, pos.y, pos.width, pos.height, color, radius);
            release_clip((Graphics)hdc);
        }

        public virtual object get_temp_dc() => CreateGraphics();
        public virtual void release_temp_dc(object hdc) => ((Graphics)hdc).Dispose();


        public element create_element(string tag_name, Dictionary<string, string> attributes, document doc) => null;
        public void del_clip() { }

        public void draw_background(object hdc, background_paint bg) { }
        public void draw_borders(object hdc, borders borders, position draw_pos, bool root) { }
        public void draw_list_marker(object hdc, list_marker marker) { }
        public void get_client_rect(out position client) { client = default(position); }
        public string get_default_font_name() => null;
        public int get_default_font_size() => 0;
        public void get_image_size(string src, string baseurl, out size sz) { sz = default(size); }
        public void get_language(string language, out string culture) { culture = null; }
        public void get_media_features(media_features media) { }
        public void import_css(out string text, string url, string baseurl) { text = null; }
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

        void apply_clip(Graphics hdc)
        {
            //if (_hClipRgn != null)
            //{
            //    _hClipRgn.Dispose();
            //    _hClipRgn = null;
            //}
            hdc.ResetClip();
            if (_clips.Count != 0)
            {
                var ptView = hdc.RenderingOrigin;
                var clip_pos = _clips.Last();
                _hClipRect = new Rectangle(clip_pos.left - ptView.X, clip_pos.top - ptView.Y, clip_pos.right - ptView.X, clip_pos.bottom - ptView.Y);
                hdc.SetClip(_hClipRect);
            }
        }

        void release_clip(Graphics hdc)
        {
            hdc.ResetClip();
            //if (_hClipRgn != null)
            //{
            //    _hClipRgn.Dispose();
            //    _hClipRgn = null;
            //}
        }
    }
}
