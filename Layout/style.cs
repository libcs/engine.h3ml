using System.Collections.Generic;

namespace H3ml.Layout
{
    public class property_value
    {
        public string _value;
        public bool _important;

        public property_value()
        {
            _important = false;
        }
        public property_value(string val, bool imp)
        {
            _important = imp;
            _value = val;
        }
        public property_value(property_value val)
        {
            _value = val._value;
            _important = val._important;
        }
    }

    public class style
    {
        static readonly Dictionary<string, string> _valid_values = new Dictionary<string, string> { { "white-space", types.white_space_strings } };
        Dictionary<string, property_value> _properties;

        public style() => _properties = new Dictionary<string, property_value>();
        public style(style val) => _properties = val._properties;

        public void add(string txt, string baseurl) => parse(txt, baseurl);

        public void add_property(string name, string val, string baseurl, bool important)
        {
            if (name == null || val == null)
                return;

            // Add baseurl for background image 
            if (name == "background-image")
            {
                add_parsed_property(name, val, important);
                if (baseurl != null)
                    add_parsed_property("background-image-baseurl", baseurl, important);
            }
            else

            // Parse border spacing properties 
            if (name == "border-spacing")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count == 1)
                {
                    add_parsed_property("-litehtml-border-spacing-x", tokens[0], important);
                    add_parsed_property("-litehtml-border-spacing-y", tokens[0], important);
                }
                else if (tokens.Count >= 2)
                {
                    add_parsed_property("-litehtml-border-spacing-x", tokens[0], important);
                    add_parsed_property("-litehtml-border-spacing-y", tokens[1], important);
                }
            }
            else

            // Parse borders shorthand properties 
            if (name == "border")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ", "", "(");
                int idx;
                foreach (var tok in tokens)
                {
                    idx = html.value_index(tok, types.border_style_strings, -1);
                    if (idx >= 0)
                    {
                        add_property("border-left-style", tok, baseurl, important);
                        add_property("border-right-style", tok, baseurl, important);
                        add_property("border-top-style", tok, baseurl, important);
                        add_property("border-bottom-style", tok, baseurl, important);
                    }
                    else
                    {
                        if (char.IsDigit(tok[0]) || tok[0] == '.' || html.value_in_list(tok, "thin;medium;thick"))
                        {
                            add_property("border-left-width", tok, baseurl, important);
                            add_property("border-right-width", tok, baseurl, important);
                            add_property("border-top-width", tok, baseurl, important);
                            add_property("border-bottom-width", tok, baseurl, important);
                        }
                        else
                        {
                            add_property("border-left-color", tok, baseurl, important);
                            add_property("border-right-color", tok, baseurl, important);
                            add_property("border-top-color", tok, baseurl, important);
                            add_property("border-bottom-color", tok, baseurl, important);
                        }
                    }
                }
            }
            else if (name == "border-left" || name == "border-right" || name == "border-top" || name == "border-bottom")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ", "", "(");
                int idx;
                foreach (var tok in tokens)
                {
                    idx = html.value_index(tok, types.border_style_strings, -1);
                    add_property($"{name}-{(idx >= 0 ? "style" : web_color.is_color(tok) ? "color" : "width")}", tok, baseurl, important);
                }
            }
            else

            // Parse border radius shorthand properties 
            if (name == "border-bottom-left-radius")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count >= 2)
                {
                    add_property("border-bottom-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-left-radius-y", tokens[1], baseurl, important);
                }
                else if (tokens.Count == 1)
                {
                    add_property("border-bottom-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-left-radius-y", tokens[0], baseurl, important);
                }
            }
            else if (name == "border-bottom-right-radius")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count >= 2)
                {
                    add_property("border-bottom-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-right-radius-y", tokens[1], baseurl, important);
                }
                else if (tokens.Count == 1)
                {
                    add_property("border-bottom-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-right-radius-y", tokens[0], baseurl, important);
                }

            }
            else if (name == "border-top-right-radius")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count >= 2)
                {
                    add_property("border-top-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-y", tokens[1], baseurl, important);
                }
                else if (tokens.Count == 1)
                {
                    add_property("border-top-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-y", tokens[0], baseurl, important);
                }

            }
            else if (name == "border-top-left-radius")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count >= 2)
                {
                    add_property("border-top-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-left-radius-y", tokens[1], baseurl, important);
                }
                else if (tokens.Count == 1)
                {
                    add_property("border-top-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-left-radius-y", tokens[0], baseurl, important);
                }
            }
            else

            // Parse border-radius shorthand properties 
            if (name == "border-radius")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, "/");
                if (tokens.Count == 1)
                {
                    add_property("border-radius-x", tokens[0], baseurl, important);
                    add_property("border-radius-y", tokens[0], baseurl, important);
                }
                else if (tokens.Count >= 2)
                {
                    add_property("border-radius-x", tokens[0], baseurl, important);
                    add_property("border-radius-y", tokens[1], baseurl, important);
                }
            }
            else if (name == "border-radius-x")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count == 1)
                {
                    add_property("border-top-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-left-radius-x", tokens[0], baseurl, important);
                }
                else if (tokens.Count == 2)
                {
                    add_property("border-top-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-x", tokens[1], baseurl, important);
                    add_property("border-bottom-right-radius-x", tokens[0], baseurl, important);
                    add_property("border-bottom-left-radius-x", tokens[1], baseurl, important);
                }
                else if (tokens.Count == 3)
                {
                    add_property("border-top-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-x", tokens[1], baseurl, important);
                    add_property("border-bottom-right-radius-x", tokens[2], baseurl, important);
                    add_property("border-bottom-left-radius-x", tokens[1], baseurl, important);
                }
                else if (tokens.Count >= 4)
                {
                    add_property("border-top-left-radius-x", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-x", tokens[1], baseurl, important);
                    add_property("border-bottom-right-radius-x", tokens[2], baseurl, important);
                    add_property("border-bottom-left-radius-x", tokens[3], baseurl, important);
                }
            }
            else if (name == "border-radius-y")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count == 1)
                {
                    add_property("border-top-left-radius-y", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-y", tokens[0], baseurl, important);
                    add_property("border-bottom-right-radius-y", tokens[0], baseurl, important);
                    add_property("border-bottom-left-radius-y", tokens[0], baseurl, important);
                }
                else if (tokens.Count == 2)
                {
                    add_property("border-top-left-radius-y", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-y", tokens[1], baseurl, important);
                    add_property("border-bottom-right-radius-y", tokens[0], baseurl, important);
                    add_property("border-bottom-left-radius-y", tokens[1], baseurl, important);
                }
                else if (tokens.Count == 3)
                {
                    add_property("border-top-left-radius-y", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-y", tokens[1], baseurl, important);
                    add_property("border-bottom-right-radius-y", tokens[2], baseurl, important);
                    add_property("border-bottom-left-radius-y", tokens[1], baseurl, important);
                }
                else if (tokens.Count >= 4)
                {
                    add_property("border-top-left-radius-y", tokens[0], baseurl, important);
                    add_property("border-top-right-radius-y", tokens[1], baseurl, important);
                    add_property("border-bottom-right-radius-y", tokens[2], baseurl, important);
                    add_property("border-bottom-left-radius-y", tokens[3], baseurl, important);
                }
            }
            else

            // Parse list-style shorthand properties 
            if (name == "list-style")
            {
                add_parsed_property("list-style-type", "disc", important);
                add_parsed_property("list-style-position", "outside", important);
                add_parsed_property("list-style-image", "", important);
                add_parsed_property("list-style-image-baseurl", "", important);

                var tokens = new List<string>();
                html.split_string(val, tokens, " ", "", "(");
                foreach (var tok in tokens)
                {
                    var idx = html.value_index(tok, types.list_style_type_strings, -1);
                    if (idx >= 0)
                        add_parsed_property("list-style-type", tok, important);
                    else
                    {
                        idx = html.value_index(tok, types.list_style_position_strings, -1);
                        if (idx >= 0)
                            add_parsed_property("list-style-position", tok, important);
                        else if (val.StartsWith("url"))
                        {
                            add_parsed_property("list-style-image", tok, important);
                            if (baseurl != null)
                                add_parsed_property("list-style-image-baseurl", baseurl, important);
                        }
                    }
                }
            }
            else

            // Add baseurl for background image 
            if (name == "list-style-image")
            {
                add_parsed_property(name, val, important);
                if (baseurl != null)
                    add_parsed_property("list-style-image-baseurl", baseurl, important);
            }
            else

            // Parse background shorthand properties 
            if (name == "background")
                parse_short_background(val, baseurl, important);
            else

            // Parse margin and padding shorthand properties 
            if (name == "margin" || name == "padding")
            {
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count >= 4)
                {
                    add_parsed_property($"{name}-top", tokens[0], important);
                    add_parsed_property($"{name}-right", tokens[1], important);
                    add_parsed_property($"{name}-bottom", tokens[2], important);
                    add_parsed_property($"{name}-left", tokens[3], important);
                }
                else if (tokens.Count == 3)
                {
                    add_parsed_property($"{name}-top", tokens[0], important);
                    add_parsed_property($"{name}-right", tokens[1], important);
                    add_parsed_property($"{name}-left", tokens[1], important);
                    add_parsed_property($"{name}-bottom", tokens[2], important);
                }
                else if (tokens.Count == 2)
                {
                    add_parsed_property($"{name}-top", tokens[0], important);
                    add_parsed_property($"{name}-bottom", tokens[0], important);
                    add_parsed_property($"{name}-right", tokens[1], important);
                    add_parsed_property($"{name}-left", tokens[1], important);
                }
                else if (tokens.Count == 1)
                {
                    add_parsed_property($"{name}-top", tokens[0], important);
                    add_parsed_property($"{name}-bottom", tokens[0], important);
                    add_parsed_property($"{name}-right", tokens[0], important);
                    add_parsed_property($"{name}-left", tokens[0], important);
                }
            }
            else

            // Parse border-* shorthand properties 
            if (name == "border-left" || name == "border-right" || name == "border-top" || name == "border-bottom")
                parse_short_border(name, val, important);
            else

            // Parse border-width/style/color shorthand properties 
            if (name == "border-width" || name == "border-style" || name == "border-color")
            {
                var nametokens = new List<string>();
                html.split_string(name, nametokens, "-");
                var tokens = new List<string>();
                html.split_string(val, tokens, " ");
                if (tokens.Count >= 4)
                {
                    add_parsed_property($"{nametokens[0]}-top-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-right-{nametokens[1]}", tokens[1], important);
                    add_parsed_property($"{nametokens[0]}-bottom-{nametokens[1]}", tokens[2], important);
                    add_parsed_property($"{nametokens[0]}-left-{nametokens[1]}", tokens[3], important);
                }
                else if (tokens.Count == 3)
                {
                    add_parsed_property($"{nametokens[0]}-top-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-right-{nametokens[1]}", tokens[1], important);
                    add_parsed_property($"{nametokens[0]}-left-{nametokens[1]}", tokens[1], important);
                    add_parsed_property($"{nametokens[0]}-bottom-{nametokens[1]}", tokens[2], important);
                }
                else if (tokens.Count == 2)
                {
                    add_parsed_property($"{nametokens[0]}-top-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-bottom-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-right-{nametokens[1]}", tokens[1], important);
                    add_parsed_property($"{nametokens[0]}-left-{nametokens[1]}", tokens[1], important);
                }
                else if (tokens.Count == 1)
                {
                    add_parsed_property($"{nametokens[0]}-top-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-bottom-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-right-{nametokens[1]}", tokens[0], important);
                    add_parsed_property($"{nametokens[0]}-left-{nametokens[1]}", tokens[0], important);
                }
            }
            else

            // Parse font shorthand properties 
            if (name == "font")
                parse_short_font(val, important);
            else
                add_parsed_property(name, val, important);
        }

        public string get_property(string name) => name != null && _properties.TryGetValue(name, out var f) ? f._value : null;

        public void combine(style src)
        {
            foreach (var i in src._properties)
                add_parsed_property(i.Key, i.Value._value, i.Value._important);
        }

        public void clear() => _properties.Clear();

        void parse_property(string txt, string baseurl)
        {
            var pos = txt.IndexOf(":");
            if (pos != -1)
            {
                var name = txt.Substring(0, pos).Trim().ToLowerInvariant();
                var val = txt.Substring(pos + 1).Trim();
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                {
                    var vals = new List<string>();
                    html.split_string(val, vals, "!");
                    if (vals.Count == 1)
                        add_property(name, val, baseurl, false);
                    else if (vals.Count > 1)
                        add_property(name, vals[0].Trim(), baseurl, vals[1].ToLowerInvariant() == "important");
                }
            }
        }

        void parse(string txt, string baseurl)
        {
            var properties = new List<string>();
            html.split_string(txt, properties, ";", "", "\"'");
            foreach (var i in properties)
                parse_property(i, baseurl);
        }

        void parse_short_border(string prefix, string val, bool important)
        {
            var tokens = new List<string>();
            html.split_string(val, tokens, " ", "", "(");
            if (tokens.Count >= 3)
            {
                add_parsed_property($"{prefix}-width", tokens[0], important);
                add_parsed_property($"{prefix}-style", tokens[1], important);
                add_parsed_property($"{prefix}-color", tokens[2], important);
            }
            else if (tokens.Count == 2)
            {
                if (char.IsDigit(tokens[0][0]) || html.value_index(val, types.border_width_strings) >= 0)
                {
                    add_parsed_property($"{prefix}-width", tokens[0], important);
                    add_parsed_property($"{prefix}-style", tokens[1], important);
                }
                else
                {
                    add_parsed_property($"{prefix}-style", tokens[0], important);
                    add_parsed_property($"{prefix}-color", tokens[1], important);
                }
            }
        }

        void parse_short_background(string val, string baseurl, bool important)
        {
            add_parsed_property("background-color", "transparent", important);
            add_parsed_property("background-image", string.Empty, important);
            add_parsed_property("background-image-baseurl", string.Empty, important);
            add_parsed_property("background-repeat", "repeat", important);
            add_parsed_property("background-origin", "padding-box", important);
            add_parsed_property("background-clip", "border-box", important);
            add_parsed_property("background-attachment", "scroll", important);

            if (val == "none")
                return;

            var tokens = new List<string>();
            html.split_string(val, tokens, " ", "", "(");
            var origin_found = false;
            foreach (var tok in tokens)
            {
                if (tok.StartsWith("url"))
                {
                    add_parsed_property("background-image", tok, important);
                    if (baseurl != null)
                        add_parsed_property("background-image-baseurl", baseurl, important);
                }
                else if (html.value_in_list(tok, types.background_repeat_strings))
                    add_parsed_property("background-repeat", tok, important);
                else if (html.value_in_list(tok, types.background_attachment_strings))
                    add_parsed_property("background-attachment", tok, important);
                else if (html.value_in_list(tok, types.background_box_strings))
                {
                    if (!origin_found) { add_parsed_property("background-origin", tok, important); origin_found = true; }
                    else add_parsed_property("background-clip", tok, important);
                }
                else if (html.value_in_list(tok, "left;right;top;bottom;center") || char.IsDigit(tok[0]) || tok[0] == '-' || tok[0] == '.' || tok[0] == '+')
                {
                    if (_properties.TryGetValue("background-position", out var prop)) prop._value = $"{prop._value} {tok}";
                    else add_parsed_property("background-position", tok, important);
                }
                else if (web_color.is_color(tok))
                    add_parsed_property("background-color", tok, important);
            }
        }

        void parse_short_font(string val, bool important)
        {
            add_parsed_property("font-style", "normal", important);
            add_parsed_property("font-variant", "normal", important);
            add_parsed_property("font-weight", "normal", important);
            add_parsed_property("font-size", "medium", important);
            add_parsed_property("line-height", "normal", important);

            var tokens = new List<string>();
            html.split_string(val, tokens, " ", "", "\"");

            var idx = 0;
            var was_normal = false;
            var is_family = false;
            var font_family = string.Empty;
            foreach (var tok in tokens)
            {
                idx = html.value_index(tok, types.font_style_strings);
                if (!is_family)
                {
                    if (idx >= 0)
                    {
                        if (idx == 0 && !was_normal)
                        {
                            add_parsed_property("font-weight", tok, important);
                            add_parsed_property("font-variant", tok, important);
                            add_parsed_property("font-style", tok, important);
                        }
                        else
                            add_parsed_property("font-style", tok, important);
                    }
                    else
                    {
                        if (html.value_in_list(tok, types.font_weight_strings))
                            add_parsed_property("font-weight", tok, important);
                        else
                        {
                            if (html.value_in_list(tok, types.font_variant_strings))
                                add_parsed_property("font-variant", tok, important);
                            else if (char.IsDigit(tok[0]))
                            {
                                var szlh = new List<string>();
                                html.split_string(tok, szlh, "/");
                                if (szlh.Count == 1)
                                    add_parsed_property("font-size", szlh[0], important);
                                else if (szlh.Count >= 2)
                                {
                                    add_parsed_property("font-size", szlh[0], important);
                                    add_parsed_property("line-height", szlh[1], important);
                                }
                            }
                            else
                            {
                                is_family = true;
                                font_family += tok;
                            }
                        }
                    }
                }
                else
                    font_family += tok;
            }
            add_parsed_property("font-family", font_family, important);
        }

        void add_parsed_property(string name, string val, bool important)
        {
            var is_valid = !_valid_values.TryGetValue(name, out var vals) || html.value_in_list(val, vals);
            if (is_valid)
            {
                if (_properties.TryGetValue(name, out var prop))
                {
                    if (!prop._important || (important && prop._important))
                    {
                        prop._value = val;
                        prop._important = important;
                    }
                }
                else _properties[name] = new property_value(val, important);
            }
        }

        void remove_property(string name, bool important)
        {
            if (_properties.TryGetValue(name, out var prop))
                if (!prop._important || (important && prop._important))
                    _properties.Remove(name);
        }
    }
}
