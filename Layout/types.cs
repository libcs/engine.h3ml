using System.ComponentModel;

namespace H3ml.Layout
{
    const uint font_decoration_none = 0x00;
    const uint font_decoration_underline = 0x01;
    const uint font_decoration_linethrough = 0x02;
    const uint font_decoration_overline = 0x04;

    public struct margins
    {
        public int left;
        public int right;
        public int top;
        public int bottom;

        public int width => left + right;
        public int height => top + bottom;
    }

    public struct size
    {
        public int width;
        public int height;
    }

    public struct position
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public position()
        {
            x = y = width = height = 0;
        }

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

        public static void operator +(margins t, margins mg)
        {
            t.x -= mg.left;
            t.y -= mg.top;
            t.width += mg.left + mg.right;
            t.height += mg.top + mg.bottom;
        }
        public static void operator -(margins t, margins mg)
        {
            t.x += mg.left;
            t.y += mg.top;
            t.width -= mg.left + mg.right;
            t.height -= mg.top + mg.bottom;
        }

        public void clear()
        {
            x = y = width = height = 0;
        }

        // public void operator=(size sz) //: sky
        //{
        //	width = sz.width;
        //	height = sz.height;
        //}

        public void move_to(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool does_intersect(position val)
        {
            if (!val) return true;
            return (left <= val.right && right >= val.left && bottom >= val.top && top <= val.bottom)
            || (val.left <= right && val.right >= left && val.bottom >= top && val.top <= bottom);
        }

        public bool empty => !width && !height;

        public bool is_point_inside(int x, int y) => x >= left && x <= right && y >= top && y <= bottom;
    }

    public struct font_metrics
    {
        public int height;
        public int ascent;
        public int descent;
        public int x_height;
        public bool draw_spaces;

        public font_metrics()
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
        public uint_ptr font;
        public font_metrics metrics;
    }

    public enum draw_flag
    {
        draw_root,
        draw_block,
        draw_floats,
        draw_inlines,
        draw_positioned,
    }

    public enum style_display
    {
        [DisplayName("none")] display_none,
        [DisplayName("block")] display_block,
        [DisplayName("inline")] display_inline,
        [DisplayName("inline-block")] display_inline_block,
        [DisplayName("inline-table")] display_inline_table,
        [DisplayName("list-item")] display_list_item,
        [DisplayName("table")] display_table,
        [DisplayName("table-caption")] display_table_caption,
        [DisplayName("table-cell")] display_table_cell,
        [DisplayName("table-column")] display_table_column,
        [DisplayName("table-column-group")] display_table_column_group,
        [DisplayName("table-footer-group")] display_table_footer_group,
        [DisplayName("table-header-group")] display_table_header_group,
        [DisplayName("table-row")] display_table_row,
        [DisplayName("table-row-group")] display_table_row_group,
        [DisplayName("inline-text")] display_inline_text,
    }

    public enum style_border
    {
        borderNope,
        borderNone,
        borderHidden,
        borderDotted,
        borderDashed,
        borderSolid,
        borderDouble
    }

    public enum font_size
    {
        [DisplayName("xx-small")] fontSize_xx_small,
        [DisplayName("x-small")] fontSize_x_small,
        [DisplayName("small")] fontSize_small,
        [DisplayName("medium")] fontSize_medium,
        [DisplayName("large")] fontSize_large,
        [DisplayName("x-large")] fontSize_x_large,
        [DisplayName("xx-large")] fontSize_xx_large,
        [DisplayName("smaller")] fontSize_smaller,
        [DisplayName("larger")] fontSize_larger,
    }

    public enum font_style
    {
        [DisplayName("normal")] fontStyleNormal,
        [DisplayName("italic")] fontStyleItalic
    }

    public enum font_variant
    {
        [DisplayName("normal")] font_variant_normal,
        [DisplayName("small-caps")] font_variant_italic
    }

    public enum font_weight
    {
        [DisplayName("normal")] fontWeightNormal,
        [DisplayName("bold")] fontWeightBold,
        [DisplayName("bolder")] fontWeightBolder,
        [DisplayName("lighter")] fontWeightLighter,
        [DisplayName("100")] fontWeight100,
        [DisplayName("200")] fontWeight200,
        [DisplayName("300")] fontWeight300,
        [DisplayName("400")] fontWeight400,
        [DisplayName("500")] fontWeight500,
        [DisplayName("600")] fontWeight600,
        [DisplayName("700")] fontWeight700
    }

    public enum list_style_type
    {
        [DisplayName("none")] list_style_type_none,
        [DisplayName("circle")] list_style_type_circle,
        [DisplayName("disc")] list_style_type_disc,
        [DisplayName("square")] list_style_type_square,
        [DisplayName("armenian")] list_style_type_armenian,
        [DisplayName("cjk-ideographic")] list_style_type_cjk_ideographic,
        [DisplayName("decimal")] list_style_type_decimal,
        [DisplayName("decimal-leading-zero")] list_style_type_decimal_leading_zero,
        [DisplayName("georgian")] list_style_type_georgian,
        [DisplayName("hebrew")] list_style_type_hebrew,
        [DisplayName("hiragana")] list_style_type_hiragana,
        [DisplayName("hiragana-iroha")] list_style_type_hiragana_iroha,
        [DisplayName("katakana")] list_style_type_katakana,
        [DisplayName("katakana-iroha")] list_style_type_katakana_iroha,
        [DisplayName("lower-alpha")] list_style_type_lower_alpha,
        [DisplayName("lower-greek")] list_style_type_lower_greek,
        [DisplayName("lower-latin")] list_style_type_lower_latin,
        [DisplayName("lower-roman")] list_style_type_lower_roman,
        [DisplayName("upper-alpha")] list_style_type_upper_alpha,
        [DisplayName("upper-latin")] list_style_type_upper_latin,
        [DisplayName("upper-roman")] list_style_type_upper_roman,
    }

    public enum list_style_position
    {
        [DisplayName("inside")] list_style_position_inside,
        [DisplayName("outside")] list_style_position_outside
    }

    public enum vertical_align
    {
        [DisplayName("baseline")] va_baseline,
        [DisplayName("sub")] va_sub,
        [DisplayName("super")] va_super,
        [DisplayName("top")] va_top,
        [DisplayName("text-top")] va_text_top,
        [DisplayName("middle")] va_middle,
        [DisplayName("bottom")] va_bottom,
        [DisplayName("text-bottom")] va_text_bottom
    }

    public enum border_width
    {
        [DisplayName("thin")] border_width_thin,
        [DisplayName("medium")] border_width_medium,
        [DisplayName("thick")] border_width_thick
    }

    public enum border_style
    {
        [DisplayName("none")] border_style_none,
        [DisplayName("hidden")] border_style_hidden,
        [DisplayName("dotted")] border_style_dotted,
        [DisplayName("dashed")] border_style_dashed,
        [DisplayName("solid")] border_style_solid,
        [DisplayName("double")] border_style_double,
        [DisplayName("groove")] border_style_groove,
        [DisplayName("ridge")] border_style_ridge,
        [DisplayName("inset")] border_style_inset,
        [DisplayName("outset")] border_style_outset
    }

    public enum element_float
    {
        [DisplayName("none")] float_none,
        [DisplayName("left")] float_left,
        [DisplayName("right")] float_right
    }

    public enum element_clear
    {
        [DisplayName("none")] clear_none,
        [DisplayName("left")] clear_left,
        [DisplayName("right")] clear_right,
        [DisplayName("both")] clear_both
    }

    public enum css_units
    {
        [DisplayName("none")] css_units_none,
        [DisplayName("%")] css_units_percentage,
        [DisplayName("in")] css_units_in,
        [DisplayName("cm")] css_units_cm,
        [DisplayName("mm")] css_units_mm,
        [DisplayName("em")] css_units_em,
        [DisplayName("ex")] css_units_ex,
        [DisplayName("pt")] css_units_pt,
        [DisplayName("pc")] css_units_pc,
        [DisplayName("px")] css_units_px,
        [DisplayName("dpi")] css_units_dpi,
        [DisplayName("dpcm")] css_units_dpcm,
        [DisplayName("vw")] css_units_vw,
        [DisplayName("vh")] css_units_vh,
        [DisplayName("vmin")] css_units_vmin,
        [DisplayName("vmax")] css_units_vmax,
    }

    public enum background_attachment
    {
        [DisplayName("scroll")] background_attachment_scroll,
        [DisplayName("fixed")] background_attachment_fixed
    }

    public enum background_repeat
    {
        [DisplayName("repeat")] background_repeat_repeat,
        [DisplayName("repeat-x")] background_repeat_repeat_x,
        [DisplayName("repeat-y")] background_repeat_repeat_y,
        [DisplayName("no-repeat")] background_repeat_no_repeat
    }

    public enum background_box
    {
        [DisplayName("border-box")] background_box_border,
        [DisplayName("padding-box")] background_box_padding,
        [DisplayName("content-box")] background_box_content
    }

    public enum element_position
    {
        [DisplayName("static")] element_position_static,
        [DisplayName("relative")] element_position_relative,
        [DisplayName("absolute")] element_position_absolute,
        [DisplayName("fixed")] element_position_fixed,
    }

    public enum text_align
    {
        [DisplayName("left")] text_align_left,
        [DisplayName("right")] text_align_right,
        [DisplayName("center")] text_align_center,
        [DisplayName("justify")] text_align_justify
    }

    public enum text_transform
    {
        [DisplayName("none")] text_transform_none,
        [DisplayName("capitalize")] text_transform_capitalize,
        [DisplayName("uppercase")] text_transform_uppercase,
        [DisplayName("lowercase")] text_transform_lowercase
    }

    public enum white_space
    {
        [DisplayName("normal")] white_space_normal,
        [DisplayName("nowrap")] white_space_nowrap,
        [DisplayName("pre")] white_space_pre,
        [DisplayName("pre-line")] white_space_pre_line,
        [DisplayName("pre-wrap")] white_space_pre_wrap
    }

    public enum overflow
    {
        [DisplayName("visible")] overflow_visible,
        [DisplayName("hidden")] overflow_hidden,
        [DisplayName("scroll")] overflow_scroll,
        [DisplayName("auto")] overflow_auto,
        [DisplayName("no-display")] overflow_no_display,
        [DisplayName("no-content")] overflow_no_content
    }

    public enum background_size
    {
        [DisplayName("auto")] background_size_auto,
        [DisplayName("cover")] background_size_cover,
        [DisplayName("contain")] background_size_contain,
    }

    public enum visibility
    {
        [DisplayName("visible")] visibility_visible,
        [DisplayName("hidden")] visibility_hidden,
        [DisplayName("collapse")] visibility_collapse,
    }

    public enum border_collapse
    {
        [DisplayName("collapse")] border_collapse_collapse,
        [DisplayName("separate")] border_collapse_separate,
    }

    public enum pseudo_class
    {
        [DisplayName("only-child")] pseudo_class_only_child,
        [DisplayName("only-of-type")] pseudo_class_only_of_type,
        [DisplayName("first-child")] pseudo_class_first_child,
        [DisplayName("first-of-type")] pseudo_class_first_of_type,
        [DisplayName("last-child")] pseudo_class_last_child,
        [DisplayName("last-of-type")] pseudo_class_last_of_type,
        [DisplayName("nth-child")] pseudo_class_nth_child,
        [DisplayName("nth-of-type")] pseudo_class_nth_of_type,
        [DisplayName("nth-last-child")] pseudo_class_nth_last_child,
        [DisplayName("nth-last-of-type")] pseudo_class_nth_last_of_type,
        [DisplayName("not")] pseudo_class_not,
        [DisplayName("lang")] pseudo_class_lang,
    }

    public enum content_property
    {
        [DisplayName("none")] content_property_none,
        [DisplayName("normal")] content_property_normal,
        [DisplayName("open-quote")] content_property_open_quote,
        [DisplayName("close-quote")] content_property_close_quote,
        [DisplayName("no-open-quote")] content_property_no_open_quote,
        [DisplayName("no-close-quote")] content_property_no_close_quote,
    }

    public struct floated_box
    {
        public position pos;
        public element_float float_side;
        public element_clear clear_floats;
        public element el;

        public floated_box() { }
        public floated_box(floated_box val)
        {
            pos = val.pos;
            float_side = val.float_side;
            clear_floats = val.clear_floats;
            el = val.el;
        }
        //floated_box operator=(floated_box val)
        //{
        //    pos = val.pos;
        //    float_side = val.float_side;
        //    clear_floats = val.clear_floats;
        //    el = val.el;
        //    return this;
        //}
        //floated_box(floated_box val)
        //{
        //    pos = val.pos;
        //    float_side = val.float_side;
        //    clear_floats = val.clear_floats;
        //    el = val.el;
        //}
        //void operator=(floated_box val)
        //{
        //    pos = val.pos;
        //    float_side = val.float_side;
        //    clear_floats = val.clear_floats;
        //    el = val.el;
        //}
    }

    public struct int_int_cache
    {
        public int hash;
        public int val;
        public bool is_valid;
        public bool is_default;

        public int_int_cache()
        {
            hash = 0;
            val = 0;
            is_valid = false;
            is_default = false;
        }

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

    public enum select_result
    {
        select_no_match = 0x00,
        select_match = 0x01,
        select_match_pseudo_class = 0x02,
        select_match_with_before = 0x10,
        select_match_with_after = 0x20,
    }

    public class def_value<T>
    {
        T _val;
        bool _isDefault;

        public def_value(T def_val)
        {
            _isDefault = true;
            _val = def_val;
        }
        public void reset(T def_val)
        {
            _isDefault = true;
            _val = def_val;
        }
        public bool is_default => _isDefault;
        //public T operator=(T new_val)
        //{
        //    m_val = new_val;
        //    m_is_default = false;
        //    return m_val;
        //}
        //public operator T() => m_val;
    }

    public enum media_orientation
    {
        [DisplayName("portrait")] media_orientation_portrait,
        [DisplayName("landscape")] media_orientation_landscape,
    }

    public enum media_feature
    {
        [DisplayName("none")] media_feature_none,

        [DisplayName("width")] media_feature_width,
        [DisplayName("min-width")] media_feature_min_width,
        [DisplayName("max-width")] media_feature_max_width,

        [DisplayName("height")] media_feature_height,
        [DisplayName("min-height")] media_feature_min_height,
        [DisplayName("max-height")] media_feature_max_height,

        [DisplayName("device-width")] media_feature_device_width,
        [DisplayName("min-device-width")] media_feature_min_device_width,
        [DisplayName("max-device-width")] media_feature_max_device_width,

        [DisplayName("device-height")] media_feature_device_height,
        [DisplayName("min-device-height")] media_feature_min_device_height,
        [DisplayName("max-device-height")] media_feature_max_device_height,

        [DisplayName("orientation")] media_feature_orientation,

        [DisplayName("aspect-ratio")] media_feature_aspect_ratio,
        [DisplayName("min-aspect-ratio")] media_feature_min_aspect_ratio,
        [DisplayName("max-aspect-ratio")] media_feature_max_aspect_ratio,

        [DisplayName("device-aspect-ratio")] media_feature_device_aspect_ratio,
        [DisplayName("min-device-aspect-ratio")] media_feature_min_device_aspect_ratio,
        [DisplayName("max-device-aspect-ratio")] media_feature_max_device_aspect_ratio,

        [DisplayName("color")] media_feature_color,
        [DisplayName("min-color")] media_feature_min_color,
        [DisplayName("max-color")] media_feature_max_color,

        [DisplayName("color-index")] media_feature_color_index,
        [DisplayName("min-color-index")] media_feature_min_color_index,
        [DisplayName("max-color-index")] media_feature_max_color_index,

        [DisplayName("monochrome")] media_feature_monochrome,
        [DisplayName("min-monochrome")] media_feature_min_monochrome,
        [DisplayName("max-monochrome")] media_feature_max_monochrome,

        [DisplayName("resolution")] media_feature_resolution,
        [DisplayName("min-resolution")] media_feature_min_resolution,
        [DisplayName("max-resolution")] media_feature_max_resolution,
    }

    public enum box_sizing
    {
        [DisplayName("content-box")] box_sizing_content_box,
        [DisplayName("border-box")] box_sizing_border_box,
    }

    public enum media_type
    {
        [DisplayName("none")] media_type_none,
        [DisplayName("all")] media_type_all,
        [DisplayName("screen")] media_type_screen,
        [DisplayName("print")] media_type_print,
        [DisplayName("braille")] media_type_braille,
        [DisplayName("embossed")] media_type_embossed,
        [DisplayName("handheld")] media_type_handheld,
        [DisplayName("projection")] media_type_projection,
        [DisplayName("speech")] media_type_speech,
        [DisplayName("tty")] media_type_tty,
        [DisplayName("tv")] media_type_tv,
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
        render_all,
        render_no_fixed,
        render_fixed_only,
    }

    // List of the Void Elements (can't have any contents)
    public const string[] void_elements = new[] { "area", "base", "br", "col", "command", "embed", "hr", "img", "input", "keygen", "link", "meta", "param", "source", "track", "wbr" };
}
