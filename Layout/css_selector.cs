using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H3ml.Layout
{
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

        public static selector_specificity operator +(selector_specificity t, selector_specificity val)
        {
            t.a += val.a;
            t.b += val.b;
            t.c += val.c;
            t.d += val.d;
            return t;
        }

        public static bool operator ==(selector_specificity t, selector_specificity val) => t.a == val.a && t.b == val.b && t.c == val.c && t.d == val.d;
        public static bool operator !=(selector_specificity t, selector_specificity val) => t.a != val.a || t.b != val.b || t.c != val.c || t.d != val.d;
        public override bool Equals(object obj) => obj is selector_specificity val ? this == val : base.Equals(obj);

        public static bool operator >(selector_specificity t, selector_specificity val)
        {
            if (t.a > val.a) return true;
            else if (t.a < val.a) return false;
            else
            {
                if (t.b > val.b) return true;
                else if (t.b < val.b) return false;
                else
                {
                    if (t.c > val.c) return true;
                    else if (t.c < val.c) return false;
                    else
                    {
                        if (t.d > val.d) return true;
                        else if (t.d < val.d) return false;
                    }
                }
            }
            return false;
        }
        public static bool operator <(selector_specificity t, selector_specificity val) => !(t > val) && t != val;

        //public bool operator >=(selector_specificity val)
        //{
        //    if (this == val) return true;
        //    if (this > val) return true;
        //    return false;
        //}
        //public bool operator <=(selector_specificity val) => this > val ? false : true;
    }

    //////////////////////////////////////////////////////////////////////////

    public enum attr_select_condition
    {
        exists,
        equal,
        contain_str,
        start_str,
        end_str,
        pseudo_class,
        pseudo_element,
    }

    //////////////////////////////////////////////////////////////////////////

    public class css_attribute_selector
    {
        public string attribute;
        public string val;
        public List<string> class_val = new List<string>();
        public attr_select_condition condition = attr_select_condition.exists;
    }

    //////////////////////////////////////////////////////////////////////////

    public class css_element_selector
    {
        static readonly char[] delims1 = ".#[:".ToCharArray();
        static readonly char[] delims2 = ".#[:(".ToCharArray();
        static readonly char[] delims3 = "]~=|$*^".ToCharArray();
        public string _tag;
        public List<css_attribute_selector> _attrs = new List<css_attribute_selector>();

        public void parse(string txt)
        {
            var el_end = txt.IndexOfAny(delims1);
            _tag = txt.Substring(0, el_end).ToLowerInvariant();
            while (el_end != -1)
            {
                if (txt[el_end] == '.')
                {
                    var attribute = new css_attribute_selector();
                    var pos = txt.IndexOfAny(delims1, el_end + 1);
                    attribute.val = txt.Substring(el_end + 1, pos - el_end - 1);
                    html.split_string(attribute.val, attribute.class_val, " ");
                    attribute.condition = attr_select_condition.equal;
                    attribute.attribute = "class";
                    _attrs.Add(attribute);
                    el_end = pos;
                }
                else if (txt[el_end] == ':')
                {
                    var attribute = new css_attribute_selector();
                    if (txt[el_end + 1] == ':')
                    {
                        var pos = txt.IndexOfAny(delims1, el_end + 2);
                        attribute.val = txt.Substring(el_end + 2, pos - el_end - 2).ToLowerInvariant();
                        attribute.condition = attr_select_condition.pseudo_element;
                        attribute.attribute = "pseudo-el";
                        _attrs.Add(attribute);
                        el_end = pos;
                    }
                    else
                    {
                        var pos = txt.IndexOfAny(delims2, el_end + 1);
                        if (pos != -1 && txt[pos] == '(')
                        {
                            pos = html.find_close_bracket(txt, pos);
                            if (pos != -1)
                                pos++;
                        }
                        attribute.val = (pos != -1 ? txt.Substring(el_end + 1, pos - el_end - 1) : txt.Substring(el_end + 1)).ToLowerInvariant();
                        attribute.condition = attribute.val == "after" || attribute.val == "before"
                            ? attr_select_condition.pseudo_element
                            : attr_select_condition.pseudo_class;
                        attribute.attribute = "pseudo";
                        _attrs.Add(attribute);
                        el_end = pos;
                    }
                }
                else if (txt[el_end] == '#')
                {
                    var attribute = new css_attribute_selector();
                    var pos = txt.IndexOfAny(delims1, el_end + 1);
                    attribute.val = txt.Substring(el_end + 1, pos - el_end - 1);
                    attribute.condition = attr_select_condition.equal;
                    attribute.attribute = "id";
                    _attrs.Add(attribute);
                    el_end = pos;
                }
                else if (txt[el_end] == '[')
                {
                    var attribute = new css_attribute_selector();
                    var pos = txt.IndexOfAny(delims3, el_end + 1);
                    var attr = txt.Substring(el_end + 1, pos - el_end - 1).Trim().ToLowerInvariant();
                    if (pos != -1)
                    {
                        if (txt[pos] == ']') attribute.condition = attr_select_condition.exists;
                        else if (txt[pos] == '=') { attribute.condition = attr_select_condition.equal; pos++; }
                        else if (txt.Substring(pos, 2) == "~=") { attribute.condition = attr_select_condition.contain_str; pos += 2; }
                        else if (txt.Substring(pos, 2) == "|=") { attribute.condition = attr_select_condition.start_str; pos += 2; }
                        else if (txt.Substring(pos, 2) == "^=") { attribute.condition = attr_select_condition.start_str; pos += 2; }
                        else if (txt.Substring(pos, 2) == "$=") { attribute.condition = attr_select_condition.end_str; pos += 2; }
                        else if (txt.Substring(pos, 2) == "*=") { attribute.condition = attr_select_condition.contain_str; pos += 2; }
                        else { attribute.condition = attr_select_condition.exists; pos += 1; }
                        pos = txt.FindFirstNotOf(" \t", pos);
                        if (pos != -1)
                        {
                            if (txt[pos] == '"')
                            {
                                var pos2 = txt.IndexOf("\"", pos + 1);
                                attribute.val = txt.Substring(pos + 1, pos2 == -1 ? pos2 : (pos2 - pos - 1));
                                pos = pos2 == -1 ? pos2 : (pos2 + 1);
                            }
                            else if (txt[pos] == ']') pos++;
                            else
                            {
                                var pos2 = txt.IndexOf("]", pos + 1);
                                attribute.val = txt.Substring(pos, pos2 == -1 ? pos2 : (pos2 - pos)).Trim();
                                pos = pos2 == -1 ? pos2 : (pos2 + 1);
                            }
                        }
                    }
                    else attribute.condition = attr_select_condition.exists;
                    attribute.attribute = attr;
                    _attrs.Add(attribute);
                    el_end = pos;
                }
                else el_end++;
                el_end = txt.IndexOfAny(delims1, el_end);
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////

    public enum css_combinator
    {
        descendant,
        child,
        adjacent_sibling,
        general_sibling
    }

    //////////////////////////////////////////////////////////////////////////

    public class css_selector
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
            _combinator = css_combinator.descendant;
            _order = 0;
        }
        public css_selector(css_selector val)
        {
            _right = val._right;
            _left = val._left; //: shared?
            _combinator = val._combinator;
            _specificity = val._specificity;
            _order = val._order;
            _media_query = val._media_query;
        }

        public bool parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            var tokens = new List<string>();
            html.split_string(text, tokens, "", " \t>+~", "([");
            if (tokens.Count == 0)
                return false;

            var right = tokens[tokens.Count - 1].Trim();
            var combinator = '\0';
            tokens.RemoveAt(tokens.Count - 1);
            string lastToken;
            while (tokens.Count != 0 && ((lastToken = tokens[tokens.Count - 1]) == " " || lastToken == "\t" || lastToken == "+" || lastToken == "~" || lastToken == ">"))
            {
                if (combinator == ' ' || combinator == 0)
                    combinator = lastToken[0];
                tokens.RemoveAt(tokens.Count - 1);
            }
            var left = tokens.Aggregate(new StringBuilder(), (a, b) => a.Append(b)).ToString().Trim();
            if (string.IsNullOrEmpty(right))
                return false;

            _right.parse(right);
            switch (combinator)
            {
                case '>': _combinator = css_combinator.child; break;
                case '+': _combinator = css_combinator.adjacent_sibling; break;
                case '~': _combinator = css_combinator.general_sibling; break;
                default: _combinator = css_combinator.descendant; break;
            }

            _left = null;
            if (!string.IsNullOrEmpty(left))
            {
                _left = new css_selector((media_query_list)null);
                if (!_left.parse(left))
                    return false;
            }
            return true;
        }

        public void calc_specificity()
        {
            if (!string.IsNullOrEmpty(_right._tag) && _right._tag != "*")
                _specificity.d = 1;
            foreach (var i in _right._attrs)
            {
                if (i.attribute == "id") _specificity.b++;
                else if (i.attribute == "class") _specificity.c += i.class_val.Count;
                else _specificity.c++;
            }
            if (_left != null)
            {
                _left.calc_specificity();
                _specificity += _left._specificity;
            }
        }

        public bool is_media_valid => _media_query == null ? true : _media_query.is_used;

        public void add_media_to_doc(document doc)
        {
            if (_media_query != null && doc != null)
                doc.add_media_list(_media_query);
        }

        public static bool operator >(css_selector v1, css_selector v2) => v1._specificity == v2._specificity ? v1._order > v2._order : v1._specificity > v2._specificity;
        public static bool operator <(css_selector v1, css_selector v2) => v1._specificity == v2._specificity ? v1._order < v2._order : v1._specificity < v2._specificity;
    }

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