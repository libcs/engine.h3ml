using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using H3ml.Layout;
using H3ml.Layout.Containers;
using System.IO;

namespace Browser.Forms
{
    public partial class HtmlControl : UserControl
    {
        public HtmlControl()
        {
            InitializeComponent();
        }

        string _url;
        string _base_url;
        document _html;
        context _html_context;
        int _rendered_width;
        string _cursor;
        string _clicked_url;
        BrowserForm _browser;
        container_win _container;
        http_loader _http = new http_loader();

        public void set(context html_context, BrowserForm browser)
        {
            _browser = browser;
            _html_context = html_context;
            _container = new container_win(CreateGraphics);
        }

        public void open_page(string url)
        {
            _url = url;
            _base_url = url;
            load_text_file(url, out var html);
            _url = _http.url;
            _base_url = _http.url;
            _browser.set_url(_url);
            _html = document.createFromString(html, _container, _html_context);
            if (_html != null)
            {
                _rendered_width = Width;
                _html.render(_rendered_width);
                //set_size_request(_html.width, _html.height);
            }
            //queue_draw();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }


        void make_url(string url, string basepath, out string out_)
        {
            out_ = string.IsNullOrEmpty(basepath) ? !string.IsNullOrEmpty(_base_url) ? urljoin(_base_url, url) : url : urljoin(basepath, url);
        }

        void load_text_file(string url, out string out_)
        {
            var stream = _http.load_file(url);
            using (var r = new StreamReader(stream))
                out_ = r.ReadToEnd();
        }

        static string urljoin(string base_, string relative)
        {
            try { return new Uri(new Uri(base_), relative).ToString(); }
            catch { return relative; }
        }
    }
}
