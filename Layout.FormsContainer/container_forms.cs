using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace H3ml.Layout.Containers
{
    public class container_form : UserControl, Idocument_container
    {
        readonly Dictionary<string, object> _images = new Dictionary<string, object>();
        readonly List<position> _clips = new List<position>();
        Rectangle _hClipRect;

        //public object get_temp_dc() => CreateGraphics(); // _getDC()
        //public void release_temp_dc(object hdc) => ((Graphics)hdc).Dispose();
        //public void fill_rect(object hdc, position pos, web_color color, css_border_radius radius)
        //{
        //    var gdi = (Graphics)hdc;
        //    apply_clip(gdi);
        //    fill_rect(gdi, pos.x, pos.y, pos.width, pos.height, color, radius);
        //    release_clip(gdi);
        //}

        public virtual void get_client_rect(out position client) => throw new NotImplementedException();
        public virtual void import_css(out string text, string url, string baseurl) => throw new NotImplementedException();
        public virtual void on_anchor_click(string url, element el) => throw new NotImplementedException();
        public string resolve_color(string color) => null;
        public virtual void set_base_url(string base_url) => throw new NotImplementedException();
        public virtual void set_caption(string caption) => throw new NotImplementedException();
        public virtual void set_cursor(string cursor) => throw new NotImplementedException();
        protected virtual object get_image(string url) => throw new NotImplementedException();

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

        //public int line_height(object hdc, object hFont) => ((Font)hFont).FontFamily.GetEmHeight(FontStyle.Regular);
        //public int get_text_base_line(object hdc, object hFont) => ((Font)hFont).FontFamily.GetCellDescent(FontStyle.Regular);
        public int text_width(string text, object hFont) => TextRenderer.MeasureText(text, (Font)hFont).Width;  //(int)((Graphics)hdc).MeasureString(text, (Font)hFont).Width;

        public void draw_text(object hdc, string text, object hFont, web_color color, position pos)
        {
            // https://stackoverflow.com/questions/6391911/c-sharp-winforms-anyone-know-of-a-c-sharp-gdi-library-not-slow-gdi
            apply_clip((Graphics)hdc);
            var rcText = new Rectangle(pos.left, pos.top, pos.right, pos.bottom);
            TextRenderer.DrawText((Graphics)hdc, text, (Font)hFont, rcText, Color.FromArgb(color.red, color.green, color.blue));
            //var rcText = new RectangleF(pos.left, pos.top, pos.right, pos.bottom);
            //var format = new StringFormat();
            //using (var brush = new SolidBrush(Color.FromArgb(color.red, color.green, color.blue)))
            //    ((Graphics)hdc).DrawString(text, (Font)hFont, brush, rcText, format);
            release_clip((Graphics)hdc);
        }

        public int pt_to_px(int pt)
        {
            using (var dc = CreateGraphics())
                return (int)(pt * dc.DpiY / 72);
        }

        public int get_default_font_size() => 16;

        public void draw_list_marker(object hdc, list_marker marker)
        {
            var gdi = (Graphics)hdc;
            apply_clip(gdi);
            if (!string.IsNullOrEmpty(marker.image))
            {
                make_url(marker.image, marker.baseurl, out var url);
                if (_images.TryGetValue(url, out var img) && img is Bitmap bmp)
                    draw_bmp(gdi, bmp, marker.pos);
            }
            else
                switch (marker.marker_type)
                {
                    case list_style_type.circle:
                        {
                            draw_ellipse(gdi, marker.pos.x, marker.pos.y, marker.pos.width, marker.pos.height, marker.color, 1);
                        }
                        break;
                    case list_style_type.disc:
                        {
                            fill_ellipse(gdi, marker.pos.x, marker.pos.y, marker.pos.width, marker.pos.height, marker.color);
                        }
                        break;
                    case list_style_type.square:
                        {
                            fill_rect(gdi, marker.pos.x, marker.pos.y, marker.pos.width, marker.pos.height, marker.color, new css_border_radius());
                        }
                        break;
                }
            release_clip(gdi);
        }

        public void load_image(string src, string baseurl, bool redraw_on_ready)
        {
            make_url(src, baseurl, out var url);
            if (!_images.ContainsKey(url))
            {
                var img = get_image(url);
                if (img != null)
                    _images[url] = img;
            }
        }

        public void get_image_size(string src, string baseurl, out size sz)
        {
            sz = new size();
            make_url(src, baseurl, out var url);
            if (_images.TryGetValue(url, out var img) && img is Bitmap bmp)
            {
                sz.width = bmp.Width;
                sz.height = bmp.Height;
            }
        }

        public void draw_image(object hdc, string src, string baseurl, position pos)
        {
            var gdi = (Graphics)hdc;
            apply_clip(gdi);
            make_url(src, baseurl, out var url);
            if (_images.TryGetValue(url, out var img) && img is Bitmap bmp)
                draw_bmp(gdi, bmp, pos);
            release_clip(gdi);
        }

        void draw_bmp(Graphics hdc, Bitmap bmp, position pos)
        {
            hdc.InterpolationMode = InterpolationMode.NearestNeighbor;
            hdc.PixelOffsetMode = PixelOffsetMode.Half;
            hdc.DrawImage(bmp, pos.x, pos.y, pos.width, pos.height);
        }

        public void draw_background(object hdc, background_paint bg)
        {
            var gdi = (Graphics)hdc;
            apply_clip(gdi);
            make_url(bg.image, bg.baseurl, out var url);
            if (_images.TryGetValue(url, out var img) && img is Bitmap bmp)
                draw_img_bg(gdi, bmp, bg);
            release_clip(gdi);
        }

        protected void draw_img_bg(Graphics hdc, Bitmap bmp, background_paint bg)
        {
            var bmp_Width = bmp.Width;
            var bmp_Height = bmp.Height;
            hdc.InterpolationMode = InterpolationMode.NearestNeighbor;
            hdc.PixelOffsetMode = PixelOffsetMode.Half;
            var rect = new Rectangle(bg.clip_box.left, bg.clip_box.top, bg.clip_box.width, bg.clip_box.height);
            hdc.SetClip(rect);
            switch (bg.repeat)
            {
                case background_repeat.no_repeat:
                    {
                        hdc.DrawImage(bmp, bg.position_x, bg.position_y, bmp_Width, bmp_Height);
                    }
                    break;
                case background_repeat.repeat_x:
                    {
                        //        var bmp = new CachedBitmap(bgbmp, hdc);
                        //        for (var x = pos.left; x < pos.right; x += bgbmp_Width)
                        //            hdc.DrawCachedBitmap(bmp, x, pos.top);
                        //        for (var x = pos.left - bgbmp_Width; x + bgbmp_Width > draw_pos.left; x -= bgbmp_Width)
                        //            hdc.DrawCachedBitmap(&bmp, x, pos.top);
                    }
                    break;
                case background_repeat.repeat_y:
                    {
                        //        var bmp = new CachedBitmap(bgbmp, hdc);
                        //        for (var y = pos.top; y < pos.bottom; y += bgbmp_Height)
                        //            hdc.DrawCachedBitmap(bmp, pos.left, y);
                        //        for (var y = pos.top - bgbmp_Height; y + bgbmp_Height > draw_pos.top; y -= bgbmp_Height)
                        //            hdc.DrawCachedBitmap(&bmp, pos.left, y);
                    }
                    break;
                case background_repeat.repeat:
                    {
                        //        var bmp = new CachedBitmap(bgbmp, hdc);
                        //        if (bgbmp_Height >= 0)
                        //            for (var x = pos.left; x < pos.right; x += bgbmp_Width)
                        //                for (var y = pos.top; y < pos.bottom; y += bgbmp_Height)
                        //                    hdc.DrawCachedBitmap(&bmp, x, y);
                    }
                    break;
            }
        }

        protected virtual void make_url(string url, string basepath, out string urlout) => urlout = url;

        public void draw_borders(object hdc, borders borders, position draw_pos, bool root = false)
        {
            var gdi = (Graphics)hdc;
            apply_clip(gdi);
            // draw left border
            if (borders.left.width != 0 && borders.left.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.left.color.red, borders.left.color.green, borders.left.color.blue)))
                    for (var x = 0; x < borders.left.width; x++)
                        gdi.DrawLine(pen, new Point(draw_pos.left + x, draw_pos.top), new Point(draw_pos.left + x, draw_pos.bottom));
            // draw right border
            if (borders.right.width != 0 && borders.right.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.right.color.red, borders.right.color.green, borders.right.color.blue)))
                    for (var x = 0; x < borders.right.width; x++)
                        gdi.DrawLine(pen, new Point(draw_pos.right - x - 1, draw_pos.top), new Point(draw_pos.right - x - 1, draw_pos.bottom));
            // draw top border
            if (borders.top.width != 0 && borders.top.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.top.color.red, borders.top.color.green, borders.top.color.blue)))
                    for (var y = 0; y < borders.top.width; y++)
                        gdi.DrawLine(pen, new Point(draw_pos.left, draw_pos.top + y), new Point(draw_pos.right, draw_pos.top + y));
            // draw bottom border
            if (borders.bottom.width != 0 && borders.bottom.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.bottom.color.red, borders.bottom.color.green, borders.bottom.color.blue)))
                    for (var y = 0; y < borders.bottom.width; y++)
                        gdi.DrawLine(pen, new Point(draw_pos.left, draw_pos.bottom - y - 1), new Point(draw_pos.right, draw_pos.bottom - y - 1));
            release_clip(gdi);
        }

        public void transform_text(string text, text_transform tt) { }

        public void set_clip(position pos, border_radiuses bdr_radius, bool valid_x, bool valid_y)
        {
            var clip_pos = pos;
            get_client_rect(out var client_pos);
            if (!valid_x)
            {
                clip_pos.x = client_pos.x;
                clip_pos.width = client_pos.width;
            }
            if (!valid_y)
            {
                clip_pos.y = client_pos.y;
                clip_pos.height = client_pos.height;
            }
            _clips.Add(clip_pos);
        }

        public void del_clip()
        {
            if (_clips.Count != 0)
            {
                _clips.RemoveAt(_clips.Count - 1);
                //if (_clips.Count != 0)
                //    var clip_pos = _clips.Back();
            }
        }

        protected void apply_clip(Graphics hdc)
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
                var clip_pos = _clips.Back();
                _hClipRect = new Rectangle(clip_pos.left - ptView.X, clip_pos.top - ptView.Y, clip_pos.right - ptView.X, clip_pos.bottom - ptView.Y);
                hdc.SetClip(_hClipRect);
            }
        }

        protected void release_clip(Graphics hdc)
        {
            hdc.ResetClip();
            //if (_hClipRgn != null)
            //{
            //    _hClipRgn.Dispose();
            //    _hClipRgn = null;
            //}
        }

        protected void fill_rect(Graphics hdc, int x, int y, int width, int height, web_color color, css_border_radius radius)
        {
            var brush = new SolidBrush(Color.FromArgb(color.alpha, color.red, color.green, color.blue));
            hdc.FillRectangle(brush, x, y, width, height);
        }

        protected void draw_ellipse(Graphics hdc, int x, int y, int width, int height, web_color color, int line_width)
        {
            hdc.CompositingQuality = CompositingQuality.HighQuality;
            hdc.SmoothingMode = SmoothingMode.AntiAlias;
            var pen = new Pen(Color.FromArgb(color.alpha, color.red, color.green, color.blue));
            hdc.DrawEllipse(pen, x, y, width, height);
        }

        protected void fill_ellipse(Graphics hdc, int x, int y, int width, int height, web_color color)
        {
            hdc.CompositingQuality = CompositingQuality.HighQuality;
            hdc.SmoothingMode = SmoothingMode.AntiAlias;
            var brush = new SolidBrush(Color.FromArgb(color.alpha, color.red, color.green, color.blue));
            hdc.FillEllipse(brush, x, y, width, height);
        }

        protected void clear_images()
        {
            foreach (var i in _images)
                ((Bitmap)i.Value).Dispose();
            _images.Clear();
        }

        public string get_default_font_name() => "Times New Roman";

        public element create_element(string tag_name, Dictionary<string, string> attributes, document doc) => null;

        //void container_linux::rounded_rectangle(cairo_t* cr, const litehtml::position &pos, const litehtml::border_radiuses &radius )
        //void container_linux::draw_pixbuf(cairo_t* cr, const Glib::RefPtr<Gdk::Pixbuf>& bmp, int x, int y, int cx, int cy)
        //cairo_surface_t* container_linux::surface_from_pixbuf(const Glib::RefPtr<Gdk::Pixbuf>& bmp)

        public void get_media_features(media_features media)
        {
            get_client_rect(out var client);
            media.type = media_type.screen;
            media.width = client.width;
            media.height = client.height;
            var screen = Screen.FromControl(this);
            var screenBounds = screen.Bounds;
            media.device_width = screenBounds.Width;
            media.device_height = screenBounds.Height;
            media.color = 8;
            media.monochrome = 0;
            media.color_index = 256;
            media.resolution = 96;
        }

        public void get_language(out string language, out string culture) { language = "en"; culture = string.Empty; }

        public void link(document doc, element el) { }

        protected static string urljoin(string base_, string relative)
        {
            try { return new Uri(new Uri(base_), relative).ToString(); }
            catch { return relative; }
        }
    }
}
