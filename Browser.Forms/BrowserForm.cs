using H3ml.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser.Forms
{
    public partial class BrowserForm : Form
    {
        readonly int _widthAdjust;
        readonly int _heightAdjust;

        public BrowserForm(context ctx)
        {
            InitializeComponent();
            _widthAdjust = Width - html.Width;
            _heightAdjust = Height - html.Height;
            SetSize(840, 640);
            html.set(ctx, this);
            on_go_clicked(null, null);
        }

        void SetSize(int width, int height)
        {
            Width = width; Height = height;
            BrowserForm_Resize(null, null);
        }

        void on_go_clicked(object sender, EventArgs e) => html.open_page(address_bar.Text);

        void on_address_key_press(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                address_bar.Select(0, -1);
                on_go_clicked(null, null);
                e.Handled = true;
            }
        }

        public void open_url(string url)
        {
            address_bar.Text = url;
            html.open_page(url);
        }

        public void set_url(string url) => address_bar.Text = url;

        void BrowserForm_Resize(object sender, EventArgs e)
        {
            html.Width = Width - _widthAdjust;
            html.Height = Height - _heightAdjust;
        }
    }
}
