using System;
using System.Collections.Generic;

namespace H3ml.Layout
{
    public struct def_color
    {
        public string name;
        public string rgb;
    }

    public struct web_color
    {
        static def_color[] g_def_colors =
        {
            new def_color{name="transparent",rgb="rgba(0, 0, 0, 0)"},
            new def_color{name="AliceBlue",rgb="#F0F8FF"},
            new def_color{name="AntiqueWhite",rgb="#FAEBD7"},
            new def_color{name="Aqua",rgb="#00FFFF"},
            new def_color{name="Aquamarine",rgb="#7FFFD4"},
            new def_color{name="Azure",rgb="#F0FFFF"},
            new def_color{name="Beige",rgb="#F5F5DC"},
            new def_color{name="Bisque",rgb="#FFE4C4"},
            new def_color{name="Black",rgb="#000000"},
            new def_color{name="BlanchedAlmond",rgb="#FFEBCD"},
            new def_color{name="Blue",rgb="#0000FF"},
            new def_color{name="BlueViolet",rgb="#8A2BE2"},
            new def_color{name="Brown",rgb="#A52A2A"},
            new def_color{name="BurlyWood",rgb="#DEB887"},
            new def_color{name="CadetBlue",rgb="#5F9EA0"},
            new def_color{name="Chartreuse",rgb="#7FFF00"},
            new def_color{name="Chocolate",rgb="#D2691E"},
            new def_color{name="Coral",rgb="#FF7F50"},
            new def_color{name="CornflowerBlue",rgb="#6495ED"},
            new def_color{name="Cornsilk",rgb="#FFF8DC"},
            new def_color{name="Crimson",rgb="#DC143C"},
            new def_color{name="Cyan",rgb="#00FFFF"},
            new def_color{name="DarkBlue",rgb="#00008B"},
            new def_color{name="DarkCyan",rgb="#008B8B"},
            new def_color{name="DarkGoldenRod",rgb="#B8860B"},
            new def_color{name="DarkGray",rgb="#A9A9A9"},
            new def_color{name="DarkGrey",rgb="#A9A9A9"},
            new def_color{name="DarkGreen",rgb="#006400"},
            new def_color{name="DarkKhaki",rgb="#BDB76B"},
            new def_color{name="DarkMagenta",rgb="#8B008B"},
            new def_color{name="DarkOliveGreen",rgb="#556B2F"},
            new def_color{name="Darkorange",rgb="#FF8C00"},
            new def_color{name="DarkOrchid",rgb="#9932CC"},
            new def_color{name="DarkRed",rgb="#8B0000"},
            new def_color{name="DarkSalmon",rgb="#E9967A"},
            new def_color{name="DarkSeaGreen",rgb="#8FBC8F"},
            new def_color{name="DarkSlateBlue",rgb="#483D8B"},
            new def_color{name="DarkSlateGray",rgb="#2F4F4F"},
            new def_color{name="DarkSlateGrey",rgb="#2F4F4F"},
            new def_color{name="DarkTurquoise",rgb="#00CED1"},
            new def_color{name="DarkViolet",rgb="#9400D3"},
            new def_color{name="DeepPink",rgb="#FF1493"},
            new def_color{name="DeepSkyBlue",rgb="#00BFFF"},
            new def_color{name="DimGray",rgb="#696969"},
            new def_color{name="DimGrey",rgb="#696969"},
            new def_color{name="DodgerBlue",rgb="#1E90FF"},
            new def_color{name="FireBrick",rgb="#B22222"},
            new def_color{name="FloralWhite",rgb="#FFFAF0"},
            new def_color{name="ForestGreen",rgb="#228B22"},
            new def_color{name="Fuchsia",rgb="#FF00FF"},
            new def_color{name="Gainsboro",rgb="#DCDCDC"},
            new def_color{name="GhostWhite",rgb="#F8F8FF"},
            new def_color{name="Gold",rgb="#FFD700"},
            new def_color{name="GoldenRod",rgb="#DAA520"},
            new def_color{name="Gray",rgb="#808080"},
            new def_color{name="Grey",rgb="#808080"},
            new def_color{name="Green",rgb="#008000"},
            new def_color{name="GreenYellow",rgb="#ADFF2F"},
            new def_color{name="HoneyDew",rgb="#F0FFF0"},
            new def_color{name="HotPink",rgb="#FF69B4"},
            new def_color{name="Ivory",rgb="#FFFFF0"},
            new def_color{name="Khaki",rgb="#F0E68C"},
            new def_color{name="Lavender",rgb="#E6E6FA"},
            new def_color{name="LavenderBlush",rgb="#FFF0F5"},
            new def_color{name="LawnGreen",rgb="#7CFC00"},
            new def_color{name="LemonChiffon",rgb="#FFFACD"},
            new def_color{name="LightBlue",rgb="#ADD8E6"},
            new def_color{name="LightCoral",rgb="#F08080"},
            new def_color{name="LightCyan",rgb="#E0FFFF"},
            new def_color{name="LightGoldenRodYellow",rgb="#FAFAD2"},
            new def_color{name="LightGray",rgb="#D3D3D3"},
            new def_color{name="LightGrey",rgb="#D3D3D3"},
            new def_color{name="LightGreen",rgb="#90EE90"},
            new def_color{name="LightPink",rgb="#FFB6C1"},
            new def_color{name="LightSalmon",rgb="#FFA07A"},
            new def_color{name="LightSeaGreen",rgb="#20B2AA"},
            new def_color{name="LightSkyBlue",rgb="#87CEFA"},
            new def_color{name="LightSlateGray",rgb="#778899"},
            new def_color{name="LightSlateGrey",rgb="#778899"},
            new def_color{name="LightSteelBlue",rgb="#B0C4DE"},
            new def_color{name="LightYellow",rgb="#FFFFE0"},
            new def_color{name="Lime",rgb="#00FF00"},
            new def_color{name="LimeGreen",rgb="#32CD32"},
            new def_color{name="Linen",rgb="#FAF0E6"},
            new def_color{name="Magenta",rgb="#FF00FF"},
            new def_color{name="Maroon",rgb="#800000"},
            new def_color{name="MediumAquaMarine",rgb="#66CDAA"},
            new def_color{name="MediumBlue",rgb="#0000CD"},
            new def_color{name="MediumOrchid",rgb="#BA55D3"},
            new def_color{name="MediumPurple",rgb="#9370D8"},
            new def_color{name="MediumSeaGreen",rgb="#3CB371"},
            new def_color{name="MediumSlateBlue",rgb="#7B68EE"},
            new def_color{name="MediumSpringGreen",rgb="#00FA9A"},
            new def_color{name="MediumTurquoise",rgb="#48D1CC"},
            new def_color{name="MediumVioletRed",rgb="#C71585"},
            new def_color{name="MidnightBlue",rgb="#191970"},
            new def_color{name="MintCream",rgb="#F5FFFA"},
            new def_color{name="MistyRose",rgb="#FFE4E1"},
            new def_color{name="Moccasin",rgb="#FFE4B5"},
            new def_color{name="NavajoWhite",rgb="#FFDEAD"},
            new def_color{name="Navy",rgb="#000080"},
            new def_color{name="OldLace",rgb="#FDF5E6"},
            new def_color{name="Olive",rgb="#808000"},
            new def_color{name="OliveDrab",rgb="#6B8E23"},
            new def_color{name="Orange",rgb="#FFA500"},
            new def_color{name="OrangeRed",rgb="#FF4500"},
            new def_color{name="Orchid",rgb="#DA70D6"},
            new def_color{name="PaleGoldenRod",rgb="#EEE8AA"},
            new def_color{name="PaleGreen",rgb="#98FB98"},
            new def_color{name="PaleTurquoise",rgb="#AFEEEE"},
            new def_color{name="PaleVioletRed",rgb="#D87093"},
            new def_color{name="PapayaWhip",rgb="#FFEFD5"},
            new def_color{name="PeachPuff",rgb="#FFDAB9"},
            new def_color{name="Peru",rgb="#CD853F"},
            new def_color{name="Pink",rgb="#FFC0CB"},
            new def_color{name="Plum",rgb="#DDA0DD"},
            new def_color{name="PowderBlue",rgb="#B0E0E6"},
            new def_color{name="Purple",rgb="#800080"},
            new def_color{name="Red",rgb="#FF0000"},
            new def_color{name="RosyBrown",rgb="#BC8F8F"},
            new def_color{name="RoyalBlue",rgb="#4169E1"},
            new def_color{name="SaddleBrown",rgb="#8B4513"},
            new def_color{name="Salmon",rgb="#FA8072"},
            new def_color{name="SandyBrown",rgb="#F4A460"},
            new def_color{name="SeaGreen",rgb="#2E8B57"},
            new def_color{name="SeaShell",rgb="#FFF5EE"},
            new def_color{name="Sienna",rgb="#A0522D"},
            new def_color{name="Silver",rgb="#C0C0C0"},
            new def_color{name="SkyBlue",rgb="#87CEEB"},
            new def_color{name="SlateBlue",rgb="#6A5ACD"},
            new def_color{name="SlateGray",rgb="#708090"},
            new def_color{name="SlateGrey",rgb="#708090"},
            new def_color{name="Snow",rgb="#FFFAFA"},
            new def_color{name="SpringGreen",rgb="#00FF7F"},
            new def_color{name="SteelBlue",rgb="#4682B4"},
            new def_color{name="Tan",rgb="#D2B48C"},
            new def_color{name="Teal",rgb="#008080"},
            new def_color{name="Thistle",rgb="#D8BFD8"},
            new def_color{name="Tomato",rgb="#FF6347"},
            new def_color{name="Turquoise",rgb="#40E0D0"},
            new def_color{name="Violet",rgb="#EE82EE"},
            new def_color{name="Wheat",rgb="#F5DEB3"},
            new def_color{name="White",rgb="#FFFFFF"},
            new def_color{name="WhiteSmoke",rgb="#F5F5F5"},
            new def_color{name="Yellow",rgb="#FFFF00"},
            new def_color{name="YellowGreen",rgb="#9ACD32"},
            new def_color{name=null,rgb=null}
        };

        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;

        public web_color(byte r, byte g, byte b, byte a = 255)
        {
            blue = b;
            green = g;
            red = r;
            alpha = a;
        }

        public web_color(Exception o = null)
        {
            blue = 0;
            green = 0;
            red = 0;
            alpha = 0xFF;
        }

        public web_color(web_color val)
        {
            blue = val.blue;
            green = val.green;
            red = val.red;
            alpha = val.alpha;
        }

        public static web_color from_string(string str, document_container callback)
        {
            if (str == null || str[0] == 0)
                return new web_color(0, 0, 0);
            if (str[0] == '#')
            {
                var red = string.Empty;
                var green = string.Empty;
                var blue = string.Empty;
                if (str.Length - 1 == 3)
                {
                    red += str[1];
                    red += str[1];
                    green += str[2];
                    green += str[2];
                    blue += str[3];
                    blue += str[3];
                }
                else if (str.Length - 1 == 6)
                {
                    red += str[1];
                    red += str[2];
                    green += str[3];
                    green += str[4];
                    blue += str[5];
                    blue += str[6];
                }
                return new web_color
                {
                    red = (byte)Convert.ToInt64(red, 16),
                    green = (byte)Convert.ToInt64(green, 16),
                    blue = (byte)Convert.ToInt64(blue, 16)
                };
            }
            else if (str.StartsWith("rgb"))
            {
                var s = str;
                var pos = s.IndexOf("(");
                if (pos != -1)
                    s = s.Substring(pos + 1);
                pos = s.IndexOf(")");
                if (pos != -1)
                    s = s.Remove(pos, s.Length - pos);
                var tokens = new List<string>();
                html.split_string(s, tokens, ", \t");
                var clr = new web_color();
                if (tokens.Count >= 1) clr.red = (byte)int.Parse(tokens[0]);
                if (tokens.Count >= 2) clr.green = (byte)int.Parse(tokens[1]);
                if (tokens.Count >= 3) clr.blue = (byte)int.Parse(tokens[2]);
                if (tokens.Count >= 4) clr.alpha = (byte)(double.Parse(tokens[3]) * 255.0);
                return clr;
            }
            else
            {
                var rgb = resolve_name(str, callback);
                if (!string.IsNullOrEmpty(rgb))
                    return from_string(rgb, callback);
            }
            return new web_color(0, 0, 0);
        }

        public static string resolve_name(string name, document_container callback)
        {
            for (var i = 0; g_def_colors[i].name != null; i++)
                if (string.Equals(name, g_def_colors[i].name, StringComparison.OrdinalIgnoreCase))
                    return g_def_colors[i].rgb;
            if (callback != null)
                return callback.resolve_color(name);
            return string.Empty;
        }

        public static bool is_color(string str) => str.StartsWith("rgb", StringComparison.OrdinalIgnoreCase) || str[0] == '#' ? true : !char.IsDigit(str[0]) && str[0] != '.';
    }
}
