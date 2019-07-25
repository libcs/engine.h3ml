namespace H3ml.Layout
{
    //////////////////////////////////////////////////////////////////////////

    public struct selector_specificity
    {
        public int a;
        public int b;
        public int c;
        public int d;

        public selector_specificity(int va = 0, int vb = 0, int vc = 0, int vd = 0)
        {
            a = va;
            b = vb;
            c = vc;
            d = vd;
        }

        public static void operator +(selector_specificity t, selector_specificity val)
        {
            t.a += val.a;
            t.b += val.b;
            t.c += val.c;
            t.d += val.d;
        }

        //public bool operator ==(selector_specificity val) => a == val.a && b == val.b && c == val.c && d == val.d;
        //public bool operator !=(selector_specificity val) => a != val.a || b != val.b || c != val.c || d != val.d;

        //public bool operator >(selector_specificity val)
        //{
        //    if (a > val.a) return true;
        //    else if (a < val.a) return false;
        //    else
        //    {
        //        if (b > val.b) return true;
        //        else if (b < val.b) return false;
        //        else
        //        {
        //            if (c > val.c) return true;
        //            else if (c < val.c) return false;
        //            else
        //            {
        //                if (d > val.d) return true;
        //                else if (d < val.d) return false;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //public bool operator >=(selector_specificity val)
        //{
        //    if (this == val) return true;
        //    if (this > val) return true;
        //    return false;
        //}

        //public bool operator <=(selector_specificity val) => this > val ? false : true;
        //public bool operator <(selector_specificity val) => this <= val && this != val;
    }

    //////////////////////////////////////////////////////////////////////////

    public enum attr_select_condition
    {
        select_exists,
        select_equal,
        select_contain_str,
        select_start_str,
        select_end_str,
        select_pseudo_class,
        select_pseudo_element,
    }

    //////////////////////////////////////////////////////////////////////////

    public struct css_attribute_selector
    {
        public string attribute;
        public string val;
        public string_vector class_val;
        public attr_select_condition condition;

        public css_attribute_selector()
        {
            condition = select_exists;
        }
    }

    //////////////////////////////////////////////////////////////////////////

    public class css_element_selector
    {
        public string _tag;
        public List<css_attribute_selector> _attrs = new List<css_attribute_selector>();
        public void parse(string txt)
        {
            var el_end = txt.find_first_of(".#[:");
            m_tag = txt.substr(0, el_end);
            litehtml::lcase(m_tag);
            while (el_end != tstring::npos)
            {
                if (txt[el_end] == '.')
                {
                    css_attribute_selector attribute;

                    var pos = txt.find_first_of(".#[:", el_end + 1);
                    attribute.val = txt.substr(el_end + 1, pos - el_end - 1);
                    split_string(attribute.val, attribute.class_val, " ");
                    attribute.condition = select_equal;
                    attribute.attribute = "class";
                    _attrs.Add(attribute);
                    el_end = pos;
                }
                else if (txt[el_end] == ':')
                {
                    css_attribute_selector attribute;

                    if (txt[el_end + 1] == ':')
                    {
                        var pos = txt.find_first_of(".#[:", el_end + 2);
                        attribute.val = txt.substr(el_end + 2, pos - el_end - 2);
                        attribute.condition = select_pseudo_element;
                        litehtml::lcase(attribute.val);
                        attribute.attribute = "pseudo-el";
                        m_attrs.push_back(attribute);
                        el_end = pos;
                    }
                    else
                    {
                        tstring::size_type pos = txt.find_first_of(_t(".#[:("), el_end + 1);
                        if (pos != tstring::npos && txt.at(pos) == _t('('))
                        {
                            pos = find_close_bracket(txt, pos);
                            if (pos != tstring::npos)
                            {
                                pos++;
                            }
                            else
                            {
                                int iii = 0;
                                iii++;
                            }
                        }
                        if (pos != tstring::npos)
                        {
                            attribute.val = txt.substr(el_end + 1, pos - el_end - 1);
                        }
                        else
                        {
                            attribute.val = txt.substr(el_end + 1);
                        }
                        litehtml::lcase(attribute.val);
                        if (attribute.val == _t("after") || attribute.val == _t("before"))
                        {
                            attribute.condition = select_pseudo_element;
                        }
                        else
                        {
                            attribute.condition = select_pseudo_class;
                        }
                        attribute.attribute = _t("pseudo");
                        m_attrs.push_back(attribute);
                        el_end = pos;
                    }
                }
                else if (txt[el_end] == _t('#'))
                {
                    css_attribute_selector attribute;

                    tstring::size_type pos = txt.find_first_of(_t(".#[:"), el_end + 1);
                    attribute.val = txt.substr(el_end + 1, pos - el_end - 1);
                    attribute.condition = select_equal;
                    attribute.attribute = _t("id");
                    m_attrs.push_back(attribute);
                    el_end = pos;
                }
                else if (txt[el_end] == _t('['))
                {
                    css_attribute_selector attribute;

                    tstring::size_type pos = txt.find_first_of(_t("]~=|$*^"), el_end + 1);
                    tstring attr = txt.substr(el_end + 1, pos - el_end - 1);
                    trim(attr);
                    litehtml::lcase(attr);
                    if (pos != tstring::npos)
                    {
                        if (txt[pos] == _t(']'))
                        {
                            attribute.condition = select_exists;
                        }
                        else if (txt[pos] == _t('='))
                        {
                            attribute.condition = select_equal;
                            pos++;
                        }
                        else if (txt.substr(pos, 2) == _t("~="))
                        {
                            attribute.condition = select_contain_str;
                            pos += 2;
                        }
                        else if (txt.substr(pos, 2) == _t("|="))
                        {
                            attribute.condition = select_start_str;
                            pos += 2;
                        }
                        else if (txt.substr(pos, 2) == _t("^="))
                        {
                            attribute.condition = select_start_str;
                            pos += 2;
                        }
                        else if (txt.substr(pos, 2) == _t("$="))
                        {
                            attribute.condition = select_end_str;
                            pos += 2;
                        }
                        else if (txt.substr(pos, 2) == _t("*="))
                        {
                            attribute.condition = select_contain_str;
                            pos += 2;
                        }
                        else
                        {
                            attribute.condition = select_exists;
                            pos += 1;
                        }
                        pos = txt.find_first_not_of(_t(" \t"), pos);
                        if (pos != tstring::npos)
                        {
                            if (txt[pos] == _t('"'))
                            {
                                tstring::size_type pos2 = txt.find_first_of(_t("\""), pos + 1);
                                attribute.val = txt.substr(pos + 1, pos2 == tstring::npos ? pos2 : (pos2 - pos - 1));
                                pos = pos2 == tstring::npos ? pos2 : (pos2 + 1);
                            }
                            else if (txt[pos] == _t(']'))
                            {
                                pos++;
                            }
                            else
                            {
                                tstring::size_type pos2 = txt.find_first_of(_t("]"), pos + 1);
                                attribute.val = txt.substr(pos, pos2 == tstring::npos ? pos2 : (pos2 - pos));
                                trim(attribute.val);
                                pos = pos2 == tstring::npos ? pos2 : (pos2 + 1);
                            }
                        }
                    }
                    else
                    {
                        attribute.condition = select_exists;
                    }
                    attribute.attribute = attr;
                    m_attrs.push_back(attribute);
                    el_end = pos;
                }
                else
                {
                    el_end++;
                }
                el_end = txt.find_first_of(_t(".#[:"), el_end);
            }
        }

    }

    //////////////////////////////////////////////////////////////////////////

    enum css_combinator
    {
        combinator_descendant,
        combinator_child,
        combinator_adjacent_sibling,
        combinator_general_sibling
    };

    //////////////////////////////////////////////////////////////////////////

    class css_selector
    {
        public selector_specificity _specificity;
        public css_element_selector _right;
        public css_selector _left;
        public css_combinator _combinator;
        public style _style;
        public int _order;
        public media_query_list _media_query;

        public css_selector(media_query_list media)
        {
            _media_query = media;
            _combinator = combinator_descendant;
            _order = 0;
        }
        public css_selector(css_selector val)
        {
            _right = val._right;
            _left = val._left; //: shared?
            _combinator = val.m_combinator;
            _specificity = val._specificity;
            _order = val._order;
            _media_query = val._media_query;
        }

        public bool parse(string text)
        {
            if (text.empty())
            {
                return false;
            }
            string_vector tokens;
            split_string(text, tokens, _t(""), _t(" \t>+~"), _t("(["));

            if (tokens.empty())
            {
                return false;
            }

            tstring left;
            tstring right = tokens.back();
            tchar_t combinator = 0;

            tokens.pop_back();
            while (!tokens.empty() && (tokens.back() == _t(" ") || tokens.back() == _t("\t") || tokens.back() == _t("+") || tokens.back() == _t("~") || tokens.back() == _t(">")))
            {
                if (combinator == _t(' ') || combinator == 0)
                {
                    combinator = tokens.back()[0];
                }
                tokens.pop_back();
            }

            for (string_vector::const_iterator i = tokens.begin(); i != tokens.end(); i++)
            {
                left += *i;
            }

            trim(left);
            trim(right);

            if (right.empty())
            {
                return false;
            }

            m_right.parse(right);

            switch (combinator)
            {
                case _t('>'):
                    m_combinator = combinator_child;
                    break;
                case _t('+'):
                    m_combinator = combinator_adjacent_sibling;
                    break;
                case _t('~'):
                    m_combinator = combinator_general_sibling;
                    break;
                default:
                    m_combinator = combinator_descendant;
                    break;
            }

            m_left = 0;

            if (!left.empty())
            {
                m_left = std::make_shared<css_selector>(media_query_list::ptr(0));
                if (!m_left->parse(left))
                {
                    return false;
                }
            }

            return true;
        }
        public void calc_specificity()
        {
            if (!m_right.m_tag.empty() && m_right.m_tag != _t("*"))
            {
                m_specificity.d = 1;
            }
            for (css_attribute_selector::vector::iterator i = m_right.m_attrs.begin(); i != m_right.m_attrs.end(); i++)
            {
                if (i->attribute == _t("id"))
                {
                    m_specificity.b++;
                }
                else
                {
                    if (i->attribute == _t("class"))
                    {
                        m_specificity.c += (int)i->class_val.size();
                    }
                    else
                    {
                        m_specificity.c++;
                    }
                }
            }
            if (m_left)
            {
                m_left->calc_specificity();
                m_specificity += m_left->m_specificity;
            }
        }
        public bool is_media_valid => !m_media_query ? true : _media_query.is_used;
        public void add_media_to_doc(document doc)
        {
            if (m_media_query && doc)
            {
                doc->add_media_list(m_media_query);
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////

    public static bool operator >(css_selector v1, css_selector v2) => v1._specificity == v2._specificity ? v1._order > v2._order : v1._specificity > v2._specificity;

    public static bool operator <(css_selector v1, css_selector v2) => v1._specificity == v2._specificity ? v1._order < v2._order : v1._specificity < v2._specificity;

    //public static bool operator >(css_selector v1, css_selector v2) => v1 > v2;
    //public static bool operator <(css_selector v1, css_selector v2) => v1 < v2;

    //////////////////////////////////////////////////////////////////////////

    public class used_selector
    {
        public css_selector _selector;
        public bool _used;

        public used_selector(css_selector selector, bool used)
        {
            _used = used;
            _selector = selector;
        }
    }
}