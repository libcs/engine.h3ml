using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace H3ml.Layout
{
    [DebuggerDisplay("css:{_selectors.Count}")]
    public class css
    {
        List<css_selector> _selectors = new List<css_selector>();

        public IList<css_selector> selectors() => _selectors;

        public void clear() => _selectors.Clear();

        readonly char[] delims1 = "{;".ToCharArray();

        public void parse_stylesheet(string str, string baseurl, document doc, media_query_list media)
        {
            var text = str;

            // remove comments
            var c_start = text.IndexOf("/*");
            while (c_start != -1)
            {
                var c_end = text.IndexOf("*/", c_start + 2);
                text = text.Substring(0, c_start) + text.Substring(c_end + 2);
                c_start = text.IndexOf("/*");
            }

            var pos = text.FindFirstNotOf(" \n\r\t");
            while (pos != -1)
            {
                while (pos != -1 && text[pos] == '@')
                {
                    var sPos = pos;
                    pos = text.IndexOfAny(delims1, pos);
                    if (pos != -1 && text[pos] == '{') pos = html.find_close_bracket(text, pos, '{', '}');
                    parse_atrule(pos != -1 ? text.Substring(sPos, pos - sPos + 1) : text.Substring(sPos), baseurl, doc, media);
                    if (pos != -1) pos = text.FindFirstNotOf(" \n\r\t", pos + 1);
                }
                if (pos == -1)
                    break;
                var style_start = text.IndexOf("{", pos);
                var style_end = text.IndexOf("}", pos);
                if (style_start != -1 && style_end != -1)
                {
                    var st = new style();
                    st.add(text.Substring(style_start + 1, style_end - style_start - 1), baseurl);
                    parse_selectors(text.Substring(pos, style_start - pos), st, media);
                    if (media != null && doc != null)
                        doc.add_media_list(media);
                    pos = style_end + 1;
                }
                else pos = -1;
                if (pos != -1) pos = text.FindFirstNotOf(" \n\r\t", pos);
            }
        }

        public void sort_selectors() => _selectors.Sort((v1, v2) => v1 < v2 ? 1 : 0);

        public static void parse_css_url(string str, out string url)
        {
            url = string.Empty;
            var pos1 = str.IndexOf('(');
            var pos2 = str.IndexOf(')');
            if (pos1 != -1 && pos2 != -1)
            {
                url = str.Substring(pos1 + 1, pos2 - pos1 - 1);
                if (url.Length != 0)
                {
                    if (url[0] == '\'' || url[0] == '"')
                        url = url.Substring(1);
                }
                if (url.Length != 0)
                    if (url[url.Length - 1] == '\'' || url[url.Length - 1] == '"')
                        url = url.Remove(url.Length - 1);
            }
        }

        void parse_atrule(string text, string baseurl, document doc, media_query_list media)
        {
            if (text.StartsWith("@import"))
            {
                var sPos = 7;
                var iStr = text.Substring(sPos);
                if (iStr[iStr.Length - 1] == ';')
                    iStr = iStr.Remove(iStr.Length - 1);
                iStr = iStr.Trim();
                var tokens = new List<string>();
                html.split_string(iStr, tokens, " ", "", "(\"");
                if (tokens.Count != 0)
                {
                    parse_css_url(tokens.First(), out var url);
                    if (string.IsNullOrEmpty(url))
                        url = tokens.First();
                    tokens.RemoveAt(0);
                    if (doc != null)
                    {
                        var doc_cont = doc.container;
                        if (doc_cont != null)
                        {
                            var css_baseurl = baseurl ?? string.Empty;
                            doc_cont.import_css(out var css_text, url, css_baseurl);
                            if (!string.IsNullOrEmpty(css_text))
                            {
                                var new_media = media;
                                if (tokens.Count != 0)
                                {
                                    var media_str = string.Join(" ", tokens);
                                    new_media = media_query_list.create_from_string(media_str, doc);
                                    if (new_media == null)
                                        new_media = media;
                                }
                                parse_stylesheet(css_text, css_baseurl, doc, new_media);
                            }
                        }
                    }
                }
            }
            else if (text.StartsWith("@media"))
            {
                var b1 = text.IndexOf('{');
                var b2 = text.LastIndexOf('}');
                if (b1 != -1)
                {
                    var media_type = text.Substring(6, b1 - 6).Trim();
                    var new_media = media_query_list.create_from_string(media_type, doc);
                    var media_style = b2 != -1 ? text.Substring(b1 + 1, b2 - b1 - 1) : text.Substring(b1 + 1);
                    parse_stylesheet(media_style, baseurl, doc, new_media);
                }
            }
        }

        void add_selector(css_selector selector)
        {
            selector._order = _selectors.Count;
            _selectors.Add(selector);
        }

        bool parse_selectors(string txt, style styles, media_query_list media)
        {
            var tokens = new List<string>();
            html.split_string(txt.Trim(), tokens, ",");
            var added_something = false;
            foreach (var tok in tokens)
            {
                var selector = new css_selector(media);
                selector._style = styles;
                if (selector.parse(tok.Trim()))
                {
                    selector.calc_specificity();
                    add_selector(selector);
                    added_something = true;
                }
            }
            return added_something;
        }
    }
}