using System;
using System.ComponentModel;
using System.Diagnostics;

namespace H3ml.Layout
{
    public partial class types
    {
        public const uint font_decoration_none = 0x00;
        public const uint font_decoration_underline = 0x01;
        public const uint font_decoration_linethrough = 0x02;
        public const uint font_decoration_overline = 0x04;
    }

    [DebuggerDisplay("margins: {left} {right} {top} {bottom}")]
    public struct margins
    {
        public int left;
        public int right;
        public int top;
        public int bottom;

        public int width => left + right;
        public int height => top + bottom;
    }

    [DebuggerDisplay("size: {width} {height}")]
    public struct size
    {
        public int width;
        public int height;
    }

    [DebuggerDisplay("position: {x},{y} {width},{height}")]
    public struct position
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public position(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public int right => x + width;
        public int bottom => y + height;
        public int left => x;
        public int top => y;

        public static position operator +(position t, margins mg)
        {
            t.x -= mg.left;
            t.y -= mg.top;
            t.width += mg.left + mg.right;
            t.height += mg.top + mg.bottom;
            return t;
        }
        public static position operator -(position t, margins mg)
        {
            t.x += mg.left;
            t.y += mg.top;
            t.width -= mg.left + mg.right;
            t.height -= mg.top + mg.bottom;
            return t;
        }

        public void clear() => x = y = width = height = 0;

        public void assignTo(size sz)
        {
        	width = sz.width;
        	height = sz.height;
        }

        public void move_to(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool does_intersect(position val)
        {
            //if (val == null) return true;
            return (left <= val.right && right >= val.left && bottom >= val.top && top <= val.bottom)
            || (val.left <= right && val.right >= left && val.bottom >= top && val.top <= bottom);
        }

        public bool empty => width == 0 && height == 0;

        public bool is_point_inside(int x, int y) => x >= left && x <= right && y >= top && y <= bottom;
    }

    [DebuggerDisplay("font_metrics: {height}")]
    public struct font_metrics
    {
        public int height;
        public int ascent;
        public int descent;
        public int x_height;
        public bool draw_spaces;

        public font_metrics(Exception o = null)
        {
            height = 0;
            ascent = 0;
            descent = 0;
            x_height = 0;
            draw_spaces = true;
        }
        public int base_line => descent;
    }

    public struct font_item
    {
        public object font;
        public font_metrics metrics;
    }

    public enum draw_flag
    {
        root,
        block,
        floats,
        inlines,
        positioned,
    }

    partial class types
    {
        public const string style_display_strings = "none;block;inline;inline-block;inline-table;list-item;table;table-caption;table-cell;table-column;table-column-group;table-footer-group;table-header-group;table-row;table-row-group;inline-text";
    }
    public enum style_display
    {
        [Description("none")] none,
        [Description("block")] block,
        [Description("inline")] inline,
        [Description("inline-block")] inline_block,
        [Description("inline-table")] inline_table,
        [Description("list-item")] list_item,
        [Description("table")] table,
        [Description("table-caption")] table_caption,
        [Description("table-cell")] table_cell,
        [Description("table-column")] table_column,
        [Description("table-column-group")] table_column_group,
        [Description("table-footer-group")] table_footer_group,
        [Description("table-header-group")] table_header_group,
        [Description("table-row")] table_row,
        [Description("table-row-group")] table_row_group,
        [Description("inline-text")] inline_text,
    }

    public enum style_border
    {
        nope,
        none,
        hidden,
        dotted,
        dashed,
        solid,
        @double
    }

    partial class types
    {
        public const string font_size_strings = "xx-small;x-small;small;medium;large;x-large;xx-large;smaller;larger";
    }
    public enum font_size
    {
        [Description("xx-small")] xx_small,
        [Description("x-small")] x_small,
        [Description("small")] small,
        [Description("medium")] medium,
        [Description("large")] large,
        [Description("x-large")] x_large,
        [Description("xx-large")] xx_large,
        [Description("smaller")] smaller,
        [Description("larger")] larger,
    }

    partial class types
    {
        public const string font_style_strings = "normal;italic";
    }
    public enum font_style
    {
        [Description("normal")] normal,
        [Description("italic")] italic
    }

    partial class types
    {
        public const string font_variant_strings = "normal;small-caps";
    }
    public enum font_variant
    {
        [Description("normal")] normal,
        [Description("small-caps")] italic
    }

    partial class types
    {
        public const string font_weight_strings = "normal;bold;bolder;lighter;100;200;300;400;500;600;700";
    }
    public enum font_weight
    {
        [Description("normal")] normal,
        [Description("bold")] bold,
        [Description("bolder")] bolder,
        [Description("lighter")] lighter,
        [Description("100")] w100,
        [Description("200")] w200,
        [Description("300")] w300,
        [Description("400")] w400,
        [Description("500")] w500,
        [Description("600")] w600,
        [Description("700")] w700
    }

    partial class types
    {
        public const string list_style_type_strings = "none;circle;disc;square;armenian;cjk-ideographic;decimal;decimal-leading-zero;georgian;hebrew;hiragana;hiragana-iroha;katakana;katakana-iroha;lower-alpha;lower-greek;lower-latin;lower-roman;upper-alpha;upper-latin;upper-roman";
    }
    public enum list_style_type
    {
        [Description("none")] none,
        [Description("circle")] circle,
        [Description("disc")] disc,
        [Description("square")] square,
        [Description("armenian")] armenian,
        [Description("cjk-ideographic")] cjk_ideographic,
        [Description("decimal")] @decimal,
        [Description("decimal-leading-zero")] decimal_leading_zero,
        [Description("georgian")] georgian,
        [Description("hebrew")] hebrew,
        [Description("hiragana")] hiragana,
        [Description("hiragana-iroha")] hiragana_iroha,
        [Description("katakana")] katakana,
        [Description("katakana-iroha")] katakana_iroha,
        [Description("lower-alpha")] lower_alpha,
        [Description("lower-greek")] lower_greek,
        [Description("lower-latin")] lower_latin,
        [Description("lower-roman")] lower_roman,
        [Description("upper-alpha")] upper_alpha,
        [Description("upper-latin")] upper_latin,
        [Description("upper-roman")] upper_roman,
    }

    partial class types
    {
        public const string list_style_position_strings = "inside;outside";
    }
    public enum list_style_position
    {
        [Description("inside")] inside,
        [Description("outside")] outside
    }

    partial class types
    {
        public const string vertical_align_strings = "baseline;sub;super;top;text-top;middle;bottom;text-bottom";
    }
    public enum vertical_align
    {
        [Description("baseline")] baseline,
        [Description("sub")] sub,
        [Description("super")] super,
        [Description("top")] top,
        [Description("text-top")] text_top,
        [Description("middle")] middle,
        [Description("bottom")] bottom,
        [Description("text-bottom")] text_bottom
    }

    partial class types
    {
        public const string border_width_strings = "thin;medium;thick";
    }
    public enum border_width
    {
        [Description("thin")] thin,
        [Description("medium")] medium,
        [Description("thick")] thick
    }

    partial class types
    {
        public const string border_style_strings = "none;hidden;dotted;dashed;solid;double;groove;ridge;inset;outset";
    }
    public enum border_style
    {
        [Description("none")] none,
        [Description("hidden")] hidden,
        [Description("dotted")] dotted,
        [Description("dashed")] dashed,
        [Description("solid")] solid,
        [Description("double")] @double,
        [Description("groove")] groove,
        [Description("ridge")] ridge,
        [Description("inset")] inset,
        [Description("outset")] outset
    }

    partial class types
    {
        public const string element_float_strings = "none;left;right";
    }
    public enum element_float
    {
        [Description("none")] none,
        [Description("left")] left,
        [Description("right")] right
    }

    partial class types
    {
        public const string element_clear_strings = "none;left;right;both";
    }
    public enum element_clear
    {
        [Description("none")] none,
        [Description("left")] left,
        [Description("right")] right,
        [Description("both")] both
    }

    partial class types
    {
        public const string css_units_strings = "none;%;in;cm;mm;em;ex;pt;pc;px;dpi;dpcm;vw;vh;vmin;vmax";
    }
    public enum css_units
    {
        [Description("none")] none,
        [Description("%")] percentage,
        [Description("in")] @in,
        [Description("cm")] cm,
        [Description("mm")] mm,
        [Description("em")] em,
        [Description("ex")] ex,
        [Description("pt")] pt,
        [Description("pc")] pc,
        [Description("px")] px,
        [Description("dpi")] dpi,
        [Description("dpcm")] dpcm,
        [Description("vw")] vw,
        [Description("vh")] vh,
        [Description("vmin")] vmin,
        [Description("vmax")] vmax,
    }

    partial class types
    {
        public const string background_attachment_strings = "scroll;fixed";
    }
    public enum background_attachment
    {
        [Description("scroll")] scroll,
        [Description("fixed")] @fixed
    }

    partial class types
    {
        public const string background_repeat_strings = "repeat;repeat-x;repeat-y;no-repeat";
    }
    public enum background_repeat
    {
        [Description("repeat")] repeat,
        [Description("repeat-x")] repeat_x,
        [Description("repeat-y")] repeat_y,
        [Description("no-repeat")] no_repeat
    }

    partial class types
    {
        public const string background_box_strings = "border-box;padding-box;content-box";
    }
    public enum background_box
    {
        [Description("border-box")] border_box,
        [Description("padding-box")] padding_box,
        [Description("content-box")] content_box
    }

    partial class types
    {
        public const string element_position_strings = "static;relative;absolute;fixed";
    }
    public enum element_position
    {
        [Description("static")] @static,
        [Description("relative")] relative,
        [Description("absolute")] absolute,
        [Description("fixed")] @fixed,
    }

    partial class types
    {
        public const string text_align_strings = "left;right;center;justify";
    }
    public enum text_align
    {
        [Description("left")] left,
        [Description("right")] right,
        [Description("center")] center,
        [Description("justify")] justify
    }

    partial class types
    {
        public const string text_transform_strings = "none;capitalize;uppercase;lowercase";
    }
    public enum text_transform
    {
        [Description("none")] none,
        [Description("capitalize")] capitalize,
        [Description("uppercase")] uppercase,
        [Description("lowercase")] lowercase
    }

    partial class types
    {
        public const string white_space_strings = "normal;nowrap;pre;pre-line;pre-wrap";
    }
    public enum white_space
    {
        [Description("normal")] normal,
        [Description("nowrap")] nowrap,
        [Description("pre")] pre,
        [Description("pre-line")] pre_line,
        [Description("pre-wrap")] pre_wrap
    }

    partial class types
    {
        public const string overflow_strings = "visible;hidden;scroll;auto;no-display;no-content";
    }
    public enum overflow
    {
        [Description("visible")] visible,
        [Description("hidden")] hidden,
        [Description("scroll")] scroll,
        [Description("auto")] auto,
        [Description("no-display")] no_display,
        [Description("no-content")] no_content
    }

    partial class types
    {
        public const string background_size_strings = "auto;cover;contain";
    }
    public enum background_size
    {
        [Description("auto")] auto,
        [Description("cover")] cover,
        [Description("contain")] contain,
    }

    partial class types
    {
        public const string visibility_strings = "visible;hidden;collapse";
    }
    public enum visibility
    {
        [Description("visible")] visible,
        [Description("hidden")] hidden,
        [Description("collapse")] collapse,
    }

    partial class types
    {
        public const string border_collapse_strings = "collapse;separate";
    }
    public enum border_collapse
    {
        [Description("collapse")] collapse,
        [Description("separate")] separate,
    }

    partial class types
    {
        public const string pseudo_class_strings = "only-child;only-of-type;first-child;first-of-type;last-child;last-of-type;nth-child;nth-of-type;nth-last-child;nth-last-of-type;not;lang";
    }
    public enum pseudo_class
    {
        [Description("only-child")] only_child,
        [Description("only-of-type")] only_of_type,
        [Description("first-child")] first_child,
        [Description("first-of-type")] first_of_type,
        [Description("last-child")] last_child,
        [Description("last-of-type")] last_of_type,
        [Description("nth-child")] nth_child,
        [Description("nth-of-type")] nth_of_type,
        [Description("nth-last-child")] nth_last_child,
        [Description("nth-last-of-type")] nth_last_of_type,
        [Description("not")] not,
        [Description("lang")] lang,
    }

    partial class types
    {
        public const string content_property_string = "none;normal;open-quote;close-quote;no-open-quote;no-close-quote";
    }
    public enum content_property
    {
        [Description("none")] none,
        [Description("normal")] normal,
        [Description("open-quote")] open_quote,
        [Description("close-quote")] close_quote,
        [Description("no-open-quote")] no_open_quote,
        [Description("no-close-quote")] no_close_quote,
    }

    public struct floated_box
    {
        public position pos;
        public element_float float_side;
        public element_clear clear_floats;
        public element el;

        //public floated_box() { }
        public floated_box(floated_box val)
        {
            pos = val.pos;
            float_side = val.float_side;
            clear_floats = val.clear_floats;
            el = val.el;
        }
    }

    public struct int_int_cache
    {
        public int hash;
        public int val;
        public bool is_valid;
        public bool is_default;

        public void invalidate()
        {
            is_valid = false;
            is_default = false;
        }

        public void set_value(int vHash, int vVal)
        {
            hash = vHash;
            val = vVal;
            is_valid = true;
        }
    }

    [Flags]
    public enum select_result
    {
        no_match = 0x00,
        match = 0x01,
        match_pseudo_class = 0x02,
        match_with_before = 0x10,
        match_with_after = 0x20,
    }

    [DebuggerDisplay("def_value: {_val}")]
    public class def_value<T>
    {
        T _val;
        bool _is_default;

        public def_value(T def_val)
        {
            _is_default = true;
            _val = def_val;
        }
        public void reset(T def_val)
        {
            _is_default = true;
            _val = def_val;
        }
        public bool is_default => _is_default;
        public T assignTo(T new_val)
        {
            _val = new_val;
            _is_default = false;
            return _val;
        }
        public T val => _val;
    }

    partial class types
    {
        public const string media_orientation_strings = "portrait;landscape";
    }
    public enum media_orientation
    {
        [Description("portrait")] portrait,
        [Description("landscape")] landscape,
    }

    partial class types
    {
        public const string media_feature_strings = "none;width;min-width;max-width;height;min-height;max-height;device-width;min-device-width;max-device-width;device-height;min-device-height;max-device-height;orientation;aspect-ratio;min-aspect-ratio;max-aspect-ratio;device-aspect-ratio;min-device-aspect-ratio;max-device-aspect-ratio;color;min-color;max-color;color-index;min-color-index;max-color-index;monochrome;min-monochrome;max-monochrome;resolution;min-resolution;max-resolution";
    }
    public enum media_feature
    {
        [Description("none")] none,

        [Description("width")] width,
        [Description("min-width")] min_width,
        [Description("max-width")] max_width,

        [Description("height")] height,
        [Description("min-height")] min_height,
        [Description("max-height")] max_height,

        [Description("device-width")] device_width,
        [Description("min-device-width")] min_device_width,
        [Description("max-device-width")] max_device_width,

        [Description("device-height")] device_height,
        [Description("min-device-height")] min_device_height,
        [Description("max-device-height")] max_device_height,

        [Description("orientation")] orientation,

        [Description("aspect-ratio")] aspect_ratio,
        [Description("min-aspect-ratio")] min_aspect_ratio,
        [Description("max-aspect-ratio")] max_aspect_ratio,

        [Description("device-aspect-ratio")] device_aspect_ratio,
        [Description("min-device-aspect-ratio")] min_device_aspect_ratio,
        [Description("max-device-aspect-ratio")] max_device_aspect_ratio,

        [Description("color")] color,
        [Description("min-color")] min_color,
        [Description("max-color")] max_color,

        [Description("color-index")] color_index,
        [Description("min-color-index")] min_color_index,
        [Description("max-color-index")] max_color_index,

        [Description("monochrome")] monochrome,
        [Description("min-monochrome")] min_monochrome,
        [Description("max-monochrome")] max_monochrome,

        [Description("resolution")] resolution,
        [Description("min-resolution")] min_resolution,
        [Description("max-resolution")] max_resolution,
    }

    partial class types
    {
        public const string box_sizing_strings = "content-box;border-box";
    }
    public enum box_sizing
    {
        [Description("content-box")] content_box,
        [Description("border-box")] border_box,
    }

    partial class types
    {
        public const string media_type_strings = "none;all;screen;print;braille;embossed;handheld;projection;speech;tty;tv";
    }
    public enum media_type
    {
        [Description("none")] none,
        [Description("all")] all,
        [Description("screen")] screen,
        [Description("print")] print,
        [Description("braille")] braille,
        [Description("embossed")] embossed,
        [Description("handheld")] handheld,
        [Description("projection")] projection,
        [Description("speech")] speech,
        [Description("tty")] tty,
        [Description("tv")] tv,
    }

    public struct media_features
    {
        public media_type type;
        public int width;          // (pixels) For continuous media, this is the width of the viewport including the size of a rendered scroll bar (if any). For paged media, this is the width of the page box.
        public int height;         // (pixels) The height of the targeted display area of the output device. For continuous media, this is the height of the viewport including the size of a rendered scroll bar (if any). For paged media, this is the height of the page box.
        public int device_width;   // (pixels) The width of the rendering surface of the output device. For continuous media, this is the width of the screen. For paged media, this is the width of the page sheet size.
        public int device_height;  // (pixels) The height of the rendering surface of the output device. For continuous media, this is the height of the screen. For paged media, this is the height of the page sheet size.
        public int color;          // The number of bits per color component of the output device. If the device is not a color device, the value is zero.
        public int color_index;    // The number of entries in the color lookup table of the output device. If the device does not use a color lookup table, the value is zero.
        public int monochrome;     // The number of bits per pixel in a monochrome frame buffer. If the device is not a monochrome device, the output device value will be 0.
        public int resolution;     // The resolution of the output device (in DPI)
    }

    public enum render_type
    {
        all,
        no_fixed,
        fixed_only,
    }

    public partial class types
    {
        // List of the Void Elements (can't have any contents)
        public static readonly string[] void_elements = new[] { "area", "base", "br", "col", "command", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };
    }
}
