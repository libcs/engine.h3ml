namespace H3ml.Browser
{
    public class browser_window : Window
    {
        public browser_window(context html_context)
        {
            _html = new html_widget(html_context, this);
            _go_button("_Go", true);

            set_title("litehtml");

            add(_vbox);
            _vbox.show();

            _vbox.pack_start(_hbox, PACK_SHRINK);
            _hbox.show();

            _hbox.pack_start(_address_bar, PACK_EXPAND_WIDGET);
            _address_bar.show();
            _address_bar.set_text("http://www.litehtml.com/");

            _address_bar.add_events(KEY_PRESS_MASK);
            _address_bar.signal_key_press_event(on_address_key_press);

            _go_button.signal_clicked(on_go_clicked);

            _hbox.pack_start(_go_button, PACK_SHRINK);
            _go_button.show();

            _vbox.pack_start(_scrolled_wnd, PACK_EXPAND_WIDGET);
            _scrolled_wnd.show();

            _scrolled_wnd.add(_html);
            _html.show();

            set_default_size(800, 600);
        }

        public void open_url(string url)
        {
            _address_bar.set_text(url);
            _html.open_page(url);
        }

        public void set_url(string url)
        {
            _address_bar.set_text(url);
        }

        void on_go_clicked()
        {
            var url = _address_bar.get_text();
            _html.open_page(url);
        }

        bool on_address_key_press(GdkEventKey event_)
        {
            if (event_.keyval == GDK_KEY_Return)

            {
                _address_bar.select_region(0, -1);
                on_go_clicked();
                return true;
            }
            return false;
        }

        protected html_widget _html;
        protected Entry _address_bar;
        protected Button _go_button;
        protected VBox _vbox;
        protected HBox _hbox;
        protected ScrolledWindow _scrolled_wnd;
    }
}