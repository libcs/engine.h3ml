namespace H3ml.Layout
{
    public class el_before_after_base : html_tag
    {
        public el_before_after_base(document doc, bool before)
        {
            html_tag = doc;
            if (before) set_tagName("::before");
            else set_tagName("::after");
        }

        public override void add_style(style st)
        {
            html_tag.add_style(st);

            var content = get_style_property("content", false, "");
            if (!content.empty())
            {
                var idx = value_index(content, content_property_string);
                if (idx < 0)
                {
                    string fnc;
                    var i = 0;
                    while (i < content.length() && i != -1)
                    {
                        if (content.at(i) == '"')
                        {
                            fnc.clear();
                            i++;
                            tstring::size_type pos = content.find('"', i);
                            tstring txt;
                            if (pos == -1)
                            {
                                txt = content.substr(i);
                                i = -1;
                            }
                            else
                            {
                                txt = content.substr(i, pos - i);
                                i = pos + 1;
                            }
                            add_text(txt);
                        }
                        else if (content.at(i) == '(')
                        {
                            i++;
                            fnc = fnc.Trim().ToLowerInvariant();
                            var pos = content.find(')', i);
                            string params_;
                            if (pos == -1)
                            {
                                params_ = content.substr(i);
                                i = -1;
                            }
                            else
                            {
                                params_ = content.substr(i, pos - i);
                                i = pos + 1;
                            }
                            add_function(fnc, params_);
                            fnc.clear();
                        }
                        else
                        {
                            fnc += content.at(i);
                            i++;
                        }
                    }
                }
            }
        }

        public override void apply_stylesheet(css stylesheet) { }

        void add_text(string txt)
        {
            tstring word;
            tstring esc;
            for (tstring::size_type i = 0; i < txt.length(); i++)
            {
                if ((txt.at(i) == _t(' ')) || (txt.at(i) == _t('\t')) || (txt.at(i) == _t('\\') && !esc.empty()))
                {
                    if (esc.empty())
                    {
                        if (!word.empty())
                        {
                            element::ptr el = std::make_shared<el_text>(word.c_str(), get_document());
                            appendChild(el);
                            word.clear();
                        }

                        element::ptr el = std::make_shared<el_space>(txt.substr(i, 1).c_str(), get_document());
                        appendChild(el);
                    }
                    else
                    {
                        word += convert_escape(esc.c_str() + 1);
                        esc.clear();
                        if (txt.at(i) == _t('\\'))
                        {
                            esc += txt.at(i);
                        }
                    }
                }
                else
                {
                    if (!esc.empty() || txt.at(i) == _t('\\'))
                    {
                        esc += txt.at(i);
                    }
                    else
                    {
                        word += txt.at(i);
                    }
                }
            }

            if (!esc.empty())
            {
                word += convert_escape(esc.c_str() + 1);
            }
            if (!word.empty())
            {
                element::ptr el = std::make_shared<el_text>(word.c_str(), get_document());
                appendChild(el);
                word.clear();
            }
        }
        void add_function(string fnc, string params)
        {
            int idx = value_index(fnc.c_str(), _t("attr;counter;url"));
            switch (idx)
            {
                // attr
                case 0:
                    {
                        tstring p_name = params;
                        trim(p_name);
                        lcase(p_name);
                        element::ptr el_parent = parent();
                        if (el_parent)
                        {
                            const tchar_t* attr_value = el_parent->get_attr(p_name.c_str());
                            if (attr_value)
                            {
                                add_text(attr_value);
                            }
                        }
                    }
                    break;
                // counter
                case 1:
                    break;
                // url
                case 2:
                    {
                        tstring p_url = params;
                        trim(p_url);
                        if (!p_url.empty())
                        {
                            if (p_url.at(0) == _t('\'') || p_url.at(0) == _t('\"'))
                            {
                                p_url.erase(0, 1);
                            }
                        }
                        if (!p_url.empty())
                        {
                            if (p_url.at(p_url.length() - 1) == _t('\'') || p_url.at(p_url.length() - 1) == _t('\"'))
                            {
                                p_url.erase(p_url.length() - 1, 1);
                            }
                        }
                        if (!p_url.empty())
                        {
                            element::ptr el = std::make_shared<el_image>(get_document());
                            el->set_attr(_t("src"), p_url.c_str());
                            el->set_attr(_t("style"), _t("display:inline-block"));
                            el->set_tagName(_t("img"));
                            appendChild(el);
                            el->parse_attributes();
                        }
                    }
                    break;
            }
        }
        char convert_escape(string txt)
        {
            tchar_t* sss = 0;
            return (tchar_t)t_strtol(txt, &sss, 16);
        }
    }

    public class el_before : el_before_after_base
    {
        public el_before(document doc) : base(doc, true) { }
    }

    public class el_after : el_before_after_base
    {
        public el_after(document& doc) : base(doc, false) { }
    }
}
