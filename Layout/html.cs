using System;
using System.Collections.Generic;

namespace H3ml.Layout
{
    public struct list_marker
    {
        public string image;
        public string baseurl;
        public list_style_type marker_type;
        public web_color color;
        public position pos;
    }

    // call back interface to draw text, images and other elements
    public interface document_container : IDisposable
    {
        object create_font(string faceName, int size, int weight, font_style italic, uint decoration, out font_metrics fm);
        void delete_font(object hFont);
        int text_width(string text, object hFont);
        void draw_text(object hdc, string text, object hFont, web_color color, position pos);
        int pt_to_px(int pt);
        int get_default_font_size();
        string get_default_font_name();
        void draw_list_marker(object hdc, list_marker marker);
        void load_image(string src, string baseurl, bool redraw_on_ready);
        void get_image_size(string src, string baseurl, out size sz);
        void draw_background(object hdc, background_paint bg);
        void draw_borders(object hdc, borders borders, position draw_pos, bool root);

        void set_caption(string caption);
        void set_base_url(string base_url);
        void link(document doc, element el);
        void on_anchor_click(string url, element el);
        void set_cursor(string cursor);
        void transform_text(string text, text_transform tt);
        void import_css(out string text, string url, string baseurl);
        void set_clip(position pos, border_radiuses bdr_radius, bool valid_x, bool valid_y);
        void del_clip();
        void get_client_rect(out position client);
        element create_element(string tag_name, Dictionary<string, string> attributes, document doc);

        void get_media_features(media_features media);
        void get_language(string language, out string culture);
        string resolve_color(string color); // => "";
    }

    public static partial class html
    {
        public static int value_index(string val, string strings, int defValue = -1, char delim = ';')
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(strings) || delim == 0)
                return defValue;
            int idx = 0;
            var delim_start = 0;
            var delim_end = strings.IndexOf(delim, delim_start);
            var item_len = 0;
            while (true)
            {
                item_len = delim_end == -1 ? strings.Length - delim_start : delim_end - delim_start;
                if (item_len == val.Length && val == strings.Substring(delim_start, item_len))
                    return idx;
                idx++;
                delim_start = delim_end;
                if (delim_start == -1) break;
                delim_start++;
                if (delim_start == strings.Length) break;
                delim_end = strings.IndexOf(delim, delim_start);
            }
            return defValue;
        }
        public static bool value_in_list(string val, string strings, char delim = ';')
        {
            var idx = value_index(val, strings, -1, delim);
            return idx >= 0 ? true : false;
        }
        public static int find_close_bracket(string s, int off, char open_b = '(', char close_b = ')')
        {
            var cnt = 0;
            for (var i = off; i < s.Length; i++)
            {
                if (s[i] == open_b) cnt++;
                else if (s[i] == close_b)
                {
                    cnt--;
                    if (cnt == 0)
                        return i;
                }
            }
            return -1;
        }

        public static void split_string(string str, IList<string> tokens, string delims, string delims_preserve = "", string quote = "\"")
        {
            if (string.IsNullOrEmpty(str) || (string.IsNullOrEmpty(delims) && string.IsNullOrEmpty(delims_preserve)))
                return;
            var all_delims = (delims + delims_preserve + quote).ToCharArray();
            var token_start = 0;
            var token_end = str.IndexOfAny(all_delims, token_start);
            int token_len;
            string token;
            while (true)
            {
                while (token_end != -1 && quote.IndexOf(str[token_end]) != -1)
                {
                    if (str[token_end] == '(') token_end = find_close_bracket(str, token_end, '(', ')');
                    else if (str[token_end] == '[') token_end = find_close_bracket(str, token_end, '[', ']');
                    else if (str[token_end] == '{') token_end = find_close_bracket(str, token_end, '{', '}');
                    else token_end = str.IndexOf(str[token_end], token_end + 1);
                    if (token_end != -1) token_end = str.IndexOfAny(all_delims, token_end + 1);
                }
                token_len = token_end == -1 ? -1 : token_end - token_start;
                token = str.Substr(token_start, token_len);
                if (!string.IsNullOrEmpty(token))
                    tokens.Add(token);
                if (token_end != -1 && !string.IsNullOrEmpty(delims_preserve) && delims_preserve.IndexOf(str[token_end]) != -1)
                    tokens.Add(str.Substring(token_end, 1));
                token_start = token_end;
                if (token_start == -1) break;
                token_start++;
                if (token_start == str.Length) break;
                token_end = str.IndexOfAny(all_delims, token_start);
            }
        }

        //public static void join_string(string str, IList<string> tokens, string delims) => string.Join(delims, tokens);

        public static int FindFirstNotOf(this string source, string chars, int pos = 0)
        {
            if (source.Length != 0)
                for (var i = pos; i < source.Length; i++)
                    if (chars.IndexOf(source[i]) == -1) return i;
            return -1;
        }

        public static string Substr(this string source, int start, int end) => end < 0 ? source.Substring(start) : source.Substring(start, end);

        public static int FindFirstOf(this string source, char[] anyOf, int start) => start == -1 ? -1 : source.IndexOfAny(anyOf, start);
    }
}