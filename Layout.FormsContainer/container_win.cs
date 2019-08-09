using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace H3ml.Layout.Containers
{
    public class container_win : UserControl, document_container
    {
        Dictionary<string, object> _images = new Dictionary<string, object>();
        List<position> _clips = new List<position>();
        Rectangle _hClipRect;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

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

        public int line_height(object hdc, object hFont) => ((Font)hFont).FontFamily.GetEmHeight(FontStyle.Regular);

        public int get_text_base_line(object hdc, object hFont) => ((Font)hFont).FontFamily.GetCellDescent(FontStyle.Regular);

        public int text_width(object hdc, string text, object hFont) => TextRenderer.MeasureText(text, (Font)hFont).Width;  //(int)((Graphics)hdc).MeasureString(text, (Font)hFont).Width;

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

        public void fill_rect(object hdc, position pos, web_color color, css_border_radius radius)
        {
            apply_clip((Graphics)hdc);
            fill_rect((Graphics)hdc, pos.x, pos.y, pos.width, pos.height, color, radius);
            release_clip((Graphics)hdc);
        }

        public object get_temp_dc() => CreateGraphics();

        public void release_temp_dc(object hdc) => ((Graphics)hdc).Dispose();

        public int pt_to_px(int pt)
        {
            using (var dc = CreateGraphics())
                return (int)(pt * dc.DpiY / 72);
        }

        public void draw_list_marker(object hdc, list_style_type marker_type, int x, int y, int height, web_color color)
        {
            apply_clip((Graphics)hdc);
            var top_margin = height / 3;
            var draw_x = x;
            var draw_y = y + top_margin;
            var draw_width = height - top_margin * 2;
            var draw_height = height - top_margin * 2;
            switch (marker_type)
            {
                case list_style_type.circle:
                    {
                        draw_ellipse((Graphics)hdc, draw_x, draw_y, draw_width, draw_height, color, 1);
                    }
                    break;
                case list_style_type.disc:
                    {
                        fill_ellipse((Graphics)hdc, draw_x, draw_y, draw_width, draw_height, color);
                    }
                    break;
                case list_style_type.square:
                    {
                        fill_rect((Graphics)hdc, draw_x, draw_y, draw_width, draw_height, color, new css_border_radius());
                    }
                    break;
            }
            release_clip((Graphics)hdc);
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
            sz = default(size);
            make_url(src, baseurl, out var url);
            if (_images.TryGetValue(url, out var img))
                get_img_size(img, out sz);
        }

        public void draw_image(object hdc, string src, string baseurl, position pos)
        {
            apply_clip((Graphics)hdc);
            make_url(src, baseurl, out var url);
            if (_images.TryGetValue(url, out var img))
                draw_img((Graphics)hdc, img, pos);
            release_clip((Graphics)hdc);
        }

        public void draw_background(object hdc, string image, string baseurl, position draw_pos, css_position bg_pos, background_repeat repeat, background_attachment attachment)
        {
            apply_clip((Graphics)hdc);
            make_url(image, baseurl, out var url);
            if (_images.TryGetValue(url, out var img))
            {
                get_img_size(img, out var img_sz);
                var pos = draw_pos;
                if (bg_pos.x.units != css_units.percentage)
                    pos.x += (int)bg_pos.x.val;
                else
                    pos.x += (int)((draw_pos.width - img_sz.width) * bg_pos.x.val / 100.0);
                if (bg_pos.y.units != css_units.percentage)
                    pos.y += (int)bg_pos.y.val;
                else
                    pos.y += (int)((draw_pos.height - img_sz.height) * bg_pos.y.val / 100.0);
                draw_img_bg((Graphics)hdc, img, draw_pos, pos, repeat, attachment);
            }
            release_clip((Graphics)hdc);
        }

        public int get_default_font_size() => 16;

        public void set_clip(position pos, bool valid_x, bool valid_y)
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
                //    var clip_pos = _clips.Last();
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
                var clip_pos = _clips.Last();
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

        protected void clear_images()
        {
            foreach (var i in _images)
                free_image(i);
            _images.Clear();
        }

        protected void make_url(string url, string basepath, out string urlout) { urlout = null; }

        protected object get_image(string url) => null;
        //protected void get_client_rect(position client) { }

        /////// container_gdi ///////

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

        protected void fill_rect(Graphics hdc, int x, int y, int width, int height, web_color color, css_border_radius radius)
        {
            var brush = new SolidBrush(Color.FromArgb(color.alpha, color.red, color.green, color.blue));
            hdc.FillRectangle(brush, x, y, width, height);
        }

        protected void get_img_size(object img, out size sz)
        {
            sz = new size();
            var bmp = (Bitmap)img;
            if (bmp != null)
            {
                sz.width = bmp.Width;
                sz.height = bmp.Height;
            }
        }

        protected void draw_img(Graphics hdc, object img, position pos)
        {
            var bmp = (Bitmap)img;
            if (bmp != null)
            {
                hdc.InterpolationMode = InterpolationMode.NearestNeighbor;
                hdc.PixelOffsetMode = PixelOffsetMode.Half;
                hdc.DrawImage(bmp, pos.x, pos.y, pos.width, pos.height);
            }
        }

        protected void free_image(object img)
        {
            var bmp = (Bitmap)img;
            if (bmp != null)
                bmp.Dispose();
        }

        protected void draw_img_bg(Graphics hdc, object img, position draw_pos, position pos, background_repeat repeat, background_attachment attachment)
        {
            var bgbmp = (Bitmap)img;
            //var img_width = bgbmp.Width;
            //var img_height = bgbmp.Height;
            hdc.InterpolationMode = InterpolationMode.NearestNeighbor;
            hdc.PixelOffsetMode = PixelOffsetMode.Half;
            var rect = new Rectangle(draw_pos.left, draw_pos.top, draw_pos.width, draw_pos.height);
            hdc.SetClip(rect);
            switch (repeat)
            {
                case background_repeat.no_repeat:
                    {
                        hdc.DrawImage(bgbmp, pos.x, pos.y, bgbmp.Width, bgbmp.Height);
                    }
                    break;
                case background_repeat.repeat_x:
                    {
                        //        var bmp = new CachedBitmap(bgbmp, hdc);
                        //        for (var x = pos.left; x < pos.right; x += bgbmp.Width)
                        //            hdc.DrawCachedBitmap(bmp, x, pos.top);
                        //        for (var x = pos.left - bgbmp.Width; x + bgbmp.Width > draw_pos.left; x -= bgbmp.Width)
                        //            hdc.DrawCachedBitmap(&bmp, x, pos.top);
                    }
                    break;
                case background_repeat.repeat_y:
                    {
                        //        var bmp = new CachedBitmap(bgbmp, hdc);
                        //        for (var y = pos.top; y < pos.bottom; y += bgbmp.Height)
                        //            hdc.DrawCachedBitmap(bmp, pos.left, y);
                        //        for (var y = pos.top - bgbmp.Height; y + bgbmp.Height > draw_pos.top; y -= bgbmp.Height)
                        //            hdc.DrawCachedBitmap(&bmp, pos.left, y);
                    }
                    break;
                case background_repeat.repeat:
                    {
                        //        var bmp = new CachedBitmap(bgbmp, hdc);
                        //        if (bgbmp.Height >= 0)
                        //            for (var x = pos.left; x < pos.right; x += bgbmp.Width)
                        //                for (var y = pos.top; y < pos.bottom; y += bgbmp.Height)
                        //                    hdc.DrawCachedBitmap(&bmp, x, y);
                    }
                    break;
            }
        }

        public void draw_borders(object hdc, css_borders borders, position draw_pos, bool root = false)
        {
            var gdi = (Graphics)hdc;
            apply_clip(gdi);
            // draw left border
            if (borders.left.width.val != 0 && borders.left.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.left.color.red, borders.left.color.green, borders.left.color.blue)))
                    for (var x = 0; x < borders.left.width.val; x++)
                        gdi.DrawLine(pen, new Point(draw_pos.left + x, draw_pos.top), new Point(draw_pos.left + x, draw_pos.bottom));
            // draw right border
            if (borders.right.width.val != 0 && borders.right.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.right.color.red, borders.right.color.green, borders.right.color.blue)))
                    for (var x = 0; x < borders.right.width.val; x++)
                        gdi.DrawLine(pen, new Point(draw_pos.right - x - 1, draw_pos.top), new Point(draw_pos.right - x - 1, draw_pos.bottom));
            // draw top border
            if (borders.top.width.val != 0 && borders.top.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.top.color.red, borders.top.color.green, borders.top.color.blue)))
                    for (var y = 0; y < borders.top.width.val; y++)
                        gdi.DrawLine(pen, new Point(draw_pos.left, draw_pos.top + y), new Point(draw_pos.right, draw_pos.top + y));
            // draw bottom border
            if (borders.bottom.width.val != 0 && borders.bottom.style > border_style.hidden)
                using (var pen = new Pen(Color.FromArgb(borders.bottom.color.red, borders.bottom.color.green, borders.bottom.color.blue)))
                    for (var y = 0; y < borders.bottom.width.val; y++)
                        gdi.DrawLine(pen, new Point(draw_pos.left, draw_pos.bottom - y - 1), new Point(draw_pos.right, draw_pos.bottom - y - 1));
            release_clip(gdi);
        }

        // LEFT OVER //

        public element create_element(string tag_name, Dictionary<string, string> attributes, document doc) => null;
        public void draw_background(object hdc, background_paint bg) { }
        public virtual void draw_borders(object hdc, borders borders, position draw_pos, bool root) { }
        public void draw_list_marker(object hdc, list_marker marker) { }
        public void get_client_rect(out position client) { client = default(position); }
        public string get_default_font_name() => null;
        public void get_language(string language, out string culture) { culture = null; }
        public void get_media_features(media_features media) { }
        public void import_css(out string text, string url, string baseurl) { text = null; }
        public void link(document doc, element el) { }
        public void on_anchor_click(string url, element el) { }
        public string resolve_color(string color) => null;
        public void set_base_url(string base_url) { }
        public void set_caption(string caption) { }
        public void set_clip(position pos, border_radiuses bdr_radius, bool valid_x, bool valid_y) { }
        public void set_cursor(string cursor) { }
        public int text_width(string text, object hFont) => 0;
        public void transform_text(string text, text_transform tt) { }
    }
}
