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
    public class document_container
    {
        public virtual uint_ptr create_font(string faceName, int size, int weight, font_style italic, uint decoration, font_metrics fm);
        public virtual void delete_font(uint_ptr hFont);
        public virtual int text_width(string text, uint_ptr hFont);
        public virtual void draw_text(uint_ptr hdc, string text, uint_ptr hFont, web_color color, position pos);
        public virtual int pt_to_px(int pt);
        public virtual int get_default_font_size();
        public virtual string get_default_font_name();
        public virtual void draw_list_marker(uint_ptr hdc, list_marker marker);
        public virtual void load_image(string src, string baseurl, bool redraw_on_ready);
        public virtual void get_image_size(string src, string baseurl, size sz);
        public virtual void draw_background(uint_ptr hdc, background_paint bg);
        public virtual void draw_borders(uint_ptr hdc, borders borders, position draw_pos, bool root);

        public virtual void set_caption(string caption);
        public virtual void set_base_url(string base_url);
        public virtual void link(document doc, element el);
        public virtual void on_anchor_click(string url, element el);
        public virtual void set_cursor(string cursor);
        public virtual void transform_text(string text, text_transform tt);
        public virtual void import_css(string text, string url, string baseurl);
        public virtual void set_clip(position pos, border_radiuses bdr_radius, bool valid_x, bool valid_y);
        public virtual void del_clip();
        public virtual void get_client_rect(position client);
        public virtual element create_element(string tag_name, Dictionary<string, string> attributes, document doc);

        public virtual void get_media_features(media_features media);
        public virtual void get_language(string language, string culture);
        public virtual string resolve_color(string color) => "";
    }

    public static class html
    {
        public int value_index(string val, string strings, int defValue = -1, char delim = ';')
        {
            if (val.empty() || strings.empty() || !delim)
            {
                return defValue;
            }

            int idx = 0;
            var delim_start = 0;
            var delim_end = strings.find(delim, delim_start);
            var item_len = 0;
            while (true)
            {
                if (delim_end == tstring::npos)
                {
                    item_len = strings.length() - delim_start;
                }
                else
                {
                    item_len = delim_end - delim_start;
                }
                if (item_len == val.length())
                {
                    if (val == strings.substr(delim_start, item_len))
                    {
                        return idx;
                    }
                }
                idx++;
                delim_start = delim_end;
                if (delim_start == tstring::npos) break;
                delim_start++;
                if (delim_start == strings.length()) break;
                delim_end = strings.find(delim, delim_start);
            }
            return defValue;
        }
        public bool value_in_list(string val, string strings, char delim = ';')
        {
            var idx = value_index(val, strings, -1, delim);
            return idx >= 0 ? true : false;
        }
        public int find_close_bracket(string s, int off, char open_b = '(', char close_b = ')')
        {
            int cnt = 0;
            for (tstring::size_type i = off; i < s.length(); i++)
            {
                if (s[i] == open_b)
                {
                    cnt++;
                }
                else if (s[i] == close_b)
                {
                    cnt--;
                    if (!cnt)
                    {
                        return i;
                    }
                }
            }
            return tstring::npos;
        }

        public void split_string(string str, out IList<string> tokens, string delims, string delims_preserve = "", string quote = "\"")
        {
            if (str.empty() || (delims.empty() && delims_preserve.empty()))
            {
                return;
            }

            tstring all_delims = delims + delims_preserve + quote;

            tstring::size_type token_start = 0;
            tstring::size_type token_end = str.find_first_of(all_delims, token_start);
            tstring::size_type token_len = 0;
            tstring token;
            while (true)
            {
                while (token_end != tstring::npos && quote.find_first_of(str[token_end]) != tstring::npos)
                {
                    if (str[token_end] == _t('('))
                    {
                        token_end = find_close_bracket(str, token_end, _t('('), _t(')'));
                    }
                    else if (str[token_end] == _t('['))
                    {
                        token_end = find_close_bracket(str, token_end, _t('['), _t(']'));
                    }
                    else if (str[token_end] == _t('{'))
                    {
                        token_end = find_close_bracket(str, token_end, _t('{'), _t('}'));
                    }
                    else
                    {
                        token_end = str.find_first_of(str[token_end], token_end + 1);
                    }
                    if (token_end != tstring::npos)
                    {
                        token_end = str.find_first_of(all_delims, token_end + 1);
                    }
                }

                if (token_end == tstring::npos)
                {
                    token_len = tstring::npos;
                }
                else
                {
                    token_len = token_end - token_start;
                }

                token = str.substr(token_start, token_len);
                if (!token.empty())
                {
                    tokens.push_back(token);
                }
                if (token_end != tstring::npos && !delims_preserve.empty() && delims_preserve.find_first_of(str[token_end]) != tstring::npos)
                {
                    tokens.push_back(str.substr(token_end, 1));
                }

                token_start = token_end;
                if (token_start == tstring::npos) break;
                token_start++;
                if (token_start == str.length()) break;
                token_end = str.find_first_of(all_delims, token_start);
            }
        }
        public void join_string(string str, IList<string> tokens, string delims)
        {
            tstringstream ss;
            for (size_t i = 0; i < tokens.size(); ++i)
            {
                if (i != 0)
                    ss << delims;
                ss << tokens[i];
            }
            str = ss.str();
        }
    }
}