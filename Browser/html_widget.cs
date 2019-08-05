using H3ml.Layout;
using System;
using System.Collections.Generic;

namespace H3ml.Browser
{
    public class html_widget : DrawingArea, container_linux
    {
        string _url;
        string _base_url;
        document _html;
        context _html_context;
        int _rendered_width;
        string _cursor;
        string _clicked_url;
        browser_window _browser;
        http_loader _http;

        public html_widget(context html_context, browser_window browser)
        {
            _browser = browser;
            _rendered_width = 0;
            _html_context = html_context;
            _html = NULL;
            add_events(POINTER_MOTION_MASK | BUTTON_PRESS_MASK | BUTTON_RELEASE_MASK);
        }

        public void open_page(string url)
        {
            _url = url;
            _base_url = url;

            string html;
            load_text_file(url, html);
            _url = _http.get_url();
            _base_url = _http.get_url();
            _browser.set_url(_url);
            _html = document.createFromString(html, this, _html_context);
            if (_html != null)
            {
                _rendered_width = get_parent_allocation().get_width();
                _html->render(_rendered_width);
                set_size_request(_html.width(), _html.height());
            }
            queue_draw();
        }

        public void update_cursor()
        {
            var cursType = _cursor == "pointer" ? HAND1 : ARROW;
            if (cursType == ARROW) get_window().set_cursor();
            else get_window().set_cursor(Cursor.create(cursType));
        }

        public void on_parent_size_allocate(Allocation allocation)
        {
            if (_html != null && _rendered_width != allocation.get_width())
            {
                _rendered_width = allocation.get_width();
                _html->media_changed();
                _html->render(_rendered_width);
                set_size_request(_html->width(), _html->height());
                queue_draw();
            }
        }

        protected virtual bool on_draw(CairoContext cr)
        {
            position pos;
            GdkRectangle rect;
            gdk_cairo_get_clip_rectangle(cr.cobj(), rect);
            pos.width = rect.width;
            pos.height = rect.height;
            pos.x = rect.x;
            pos.y = rect.y;
            cr.rectangle(0, 0, get_allocated_width(), get_allocated_height());
            cr.set_source_rgb(1, 1, 1);
            cr.fill();
            if (_html != null)
                _html.draw((IntPtr)cr.cobj(), 0, 0, pos);
            return true;
        }

        protected virtual void get_client_rect(position client)
        {
            client.width = get_parent().get_allocated_width();
            client.height = get_parent().get_allocated_height();
            client.x = 0;
            client.y = 0;
        }

        protected virtual void on_anchor_click(string url, element el)
        {
            if (url != null)
                make_url(url, _base_url, out _clicked_url);
        }

        protected virtual void set_cursor(string cursor)
        {
            if (cursor != null && _cursor != cursor)
            {
                _cursor = cursor;
                update_cursor();
            }
        }

        protected virtual void import_css(string text, string url, string baseurl)
        {
            make_url(url, baseurl, out var css_url);
            load_text_file(css_url, text);
            if (!string.IsNullOrEmpty(text))
                baseurl = css_url;
        }

        protected virtual void set_caption(string caption)
        {
            if (get_parent_window())
                get_parent_window().set_title(caption);
        }

        protected virtual void set_base_url(string base_url)
        {
            _base_url = base_url != null ? urljoin(_url, base_url) : _url;
        }

        protected virtual Pixbuf get_image(string url, bool redraw_on_ready)
        {
            var stream = _http.load_file(url);
            var ptr = Pixbuf.create_from_stream(stream);
            return ptr;
        }

        protected virtual void make_url(string url, string basepath, out string out_)
        {
            out_ = string.IsNullOrEmpty(basepath) ? !string.IsNullOrEmpty(_base_url) ? urljoin(_base_url, url) : url : urljoin(basepath, url);
        }

        protected virtual bool on_button_press_event(GdkEventButton event_)
        {
            if (_html != null)
            {
                var redraw_boxes = new List<position>();
                if (_html.on_lbutton_down((int)event_.x, (int)event_.y, (int)event_.x, (int)event_.y, redraw_boxes))
                    foreach (var pos in redraw_boxes)
                        queue_draw_area(pos.x, pos.y, pos.width, pos.height);
            }
            return true;
        }

        protected virtual bool on_button_release_event(GdkEventButton event_)
        {
            if (_html != null)
            {
                var redraw_boxes = new List<position>();
                _clicked_url.clear();
                if (_html.on_lbutton_up((int)event_.x, (int)event_.y, (int)event_.x, (int)event_.y, redraw_boxes))
                    foreach (var pos in redraw_boxes)
                        queue_draw_area(pos.x, pos.y, pos.width, pos.height);
                if (!string.IsNullOrEmpty(_clicked_url))
                    _browser.open_url(_clicked_url);
            }
            return true;
        }

        protected virtual bool on_motion_notify_event(GdkEventMotion event_)
        {
            if (_html != null)
            {
                var redraw_boxes = new List<position>();
                if (_html.on_mouse_over((int)event_.x, (int)event_.y, (int)event_.x, (int)event_.y, redraw_boxes))
                    foreach (var pos in redraw_boxes)
                        queue_draw_area(pos.x, pos.y, pos.width, pos.height);
            }
            return true;
        }

        protected virtual void on_parent_changed(Widget previous_parent)
        {
            var viewport = get_parent();
            if (viewport != null)
                viewport.signal_size_allocate(on_parent_size_allocate);
        }

        void load_text_file(string url, string out_)
        {
            out_ = string.Empty;
            var stream = _http.load_file(url);
            int sz;
            var buff = new char[1025];
            while ((sz = stream.Read(buff, 0, 1024)) > 0)
            {
                buff[sz] = 0;
                out_ += buff;
            }
        }

        Allocation get_parent_allocation()
        {
            var parent = get_parent();
            return parent.get_allocation();
        }

        static string urljoin(string base_, string relative)
        {
            try { return new Uri(new Uri(base_), relative).ToString(); }
            catch { return relative; }
        }
    }
}