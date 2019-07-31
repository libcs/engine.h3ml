using System;

namespace H3ml.Layout
{
    public class el_before_after_base : html_tag
    {
        public el_before_after_base(document doc, bool before) : base(doc)
        {
            if (before) set_tagName("::before");
            else set_tagName("::after");
        }

        public override void add_style(style st)
        {
            add_style(st);
            var content = get_style_property("content", false, "");
            if (!string.IsNullOrEmpty(content))
            {
                var idx = html.value_index(content, types.content_property_string);
                if (idx < 0)
                {
                    var fnc = string.Empty;
                    var i = 0;
                    while (i < content.Length && i != -1)
                        if (content[i] == '"')
                        {
                            fnc = string.Empty;
                            i++;
                            var pos = content.IndexOf('"', i);
                            string txt;
                            if (pos == -1) { txt = content.Substring(i); i = -1; }
                            else { txt = content.Substring(i, pos - i); i = pos + 1; }
                            add_text(txt);
                        }
                        else if (content[i] == '(')
                        {
                            i++;
                            fnc = fnc.Trim().ToLowerInvariant();
                            var pos = content.IndexOf(')', i);
                            string args;
                            if (pos == -1) { args = content.Substring(i); i = -1; }
                            else { args = content.Substring(i, pos - i); i = pos + 1; }
                            add_function(fnc, args);
                            fnc = string.Empty;
                        }
                        else
                        {
                            fnc += content[i];
                            i++;
                        }
                }
            }
        }

        public override void apply_stylesheet(css stylesheet) { }

        void add_text(string txt)
        {
            string word = null;
            string esc = null;
            for (var i = 0; i < txt.Length; i++)
            {
                if (txt[i] == ' ' || txt[i] == '\t' || (txt[i] == '\\' && !string.IsNullOrEmpty(esc)))
                {
                    if (string.IsNullOrEmpty(esc))
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            appendChild(new el_text(word, get_document()));
                            word = string.Empty;
                        }
                        appendChild(new el_space(txt.Substring(i, 1), get_document()));
                    }
                    else
                    {
                        word += convert_escape(esc.Substring(1));
                        esc = string.Empty;
                        if (txt[i] == '\\')
                            esc += txt[i];
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(esc) || txt[i] == '\\') esc += txt[i];
                    else word += txt[i];
                }
            }
            if (!string.IsNullOrEmpty(esc))
                word += convert_escape(esc.Substring(1));
            if (!string.IsNullOrEmpty(word))
            {
                appendChild(new el_text(word, get_document()));
                word = string.Empty;
            }
        }

        void add_function(string fnc, string args)
        {
            var idx = html.value_index(fnc, "attr;counter;url");
            switch (idx)
            {
                // attr
                case 0:
                    {
                        var p_name = args.Trim().ToUpperInvariant();
                        var el_parent = parent();
                        if (el_parent != null)
                        {
                            var attr_value = el_parent.get_attr(p_name);
                            if (attr_value != null)
                                add_text(attr_value);
                        }
                    }
                    break;
                // counter
                case 1:
                    break;
                // url
                case 2:
                    {
                        var p_url = args.Trim();
                        if (!string.IsNullOrEmpty(p_url) && p_url[0] == '\'' || p_url[0] == '\"')
                            p_url = p_url.Substring(1);
                        if (!string.IsNullOrEmpty(p_url) && p_url[p_url.Length - 1] == '\'' || p_url[p_url.Length - 1] == '\"')
                            p_url.Remove(p_url.Length - 1);
                        if (!string.IsNullOrEmpty(p_url))
                        {
                            var el = new el_image(get_document());
                            el.set_attr("src", p_url);
                            el.set_attr("style", "display:inline-block");
                            el.set_tagName("img");
                            appendChild(el);
                            el.parse_attributes();
                        }
                    }
                    break;
            }
        }

        char convert_escape(string txt) => (char)Convert.ToInt64(txt, 16);
    }

    public class el_before : el_before_after_base
    {
        public el_before(document doc) : base(doc, true) { }
    }

    public class el_after : el_before_after_base
    {
        public el_after(document doc) : base(doc, false) { }
    }
}
