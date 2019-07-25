using System.Collections.Generic;

namespace H3ml.Layout
{
    public struct css_text
    {
        public string text;
        public string baseurl;
        public string media;

        public css_text()
        {
        }
        public css_text(string txt, string url, string media_str)
        {
            text = txt ?? string.Empty;
            baseurl = url ?? string.Empty;
            media = media_str ?? string.Empty;
        }

        css_text(css_text val)
        {
            text = val.text;
            baseurl = val.baseurl;
            media = val.media;
        }
    }

    public struct stop_tags_t
    {
        public string tags;
        public string stop_parent;
    }

    public struct ommited_end_tags_t
    {
        public string tag;
        public string followed_tags;
    }

    public class document //: document
    {
        element _root;
        document_container _container;
        Dictionary<string, font> _fonts;
        IList<css_text> _css;
        css _styles;
        web_color _def_color;
        context _context;
        size _size;
        IList<position> _fixed_boxes;
        IList<media_query_list> _media_lists;
        element _over_element;
        IList<element> _tabular_elements;
        media_features _media;
        string _lang;
        string _culture;

        public document(document_container objContainer, context ctx)
        {
            _container = objContainer;
            _context = ctx;
        }
        public void Dispose()
        {
            _over_element = null;
            if (_container != null)
                foreach (var f in _fonts)
                    _container.delete_font(f.Value.font);
        }

        public document_container container => _container;
        public uint_ptr get_font(string name, int size, string weight, string style, string decoration, font_metrics fm)
        {
            if (!name || (name && !t_strcasecmp(name, _t("inherit"))))
            {
                name = m_container.get_default_font_name();
            }

            if (!size)
            {
                size = container().get_default_font_size();
            }

            tchar_t strSize[20];
            t_itoa(size, strSize, 20, 10);

            tstring key = name;
            key += _t(":");
            key += strSize;
            key += _t(":");
            key += weight;
            key += _t(":");
            key += style;
            key += _t(":");
            key += decoration;

            fonts_map::iterator el = m_fonts.find(key);

            if (el != m_fonts.end())
            {
                if (fm)
                {
                    *fm = el.second.metrics;
                }
                return el.second.font;
            }
            return add_font(name, size, weight, style, decoration, fm);
        }
        public int render(int max_width, render_type rt = render_all)
        {
            int ret = 0;
            if (m_root)
            {
                if (rt == render_fixed_only)
                {
                    m_fixed_boxes.clear();
                    m_root.render_positioned(rt);
                }
                else
                {
                    ret = m_root.render(0, 0, max_width);
                    if (m_root.fetch_positioned())
                    {
                        m_fixed_boxes.clear();
                        m_root.render_positioned(rt);
                    }
                    m_size.width = 0;
                    m_size.height = 0;
                    m_root.calc_document_size(m_size);
                }
            }
            return ret;
        }
        public void draw(uint_ptr hdc, int x, int y, position clip)
        {
            if (_root)
            {
                _root.draw(hdc, x, y, clip);
                _root.draw_stacking_context(hdc, x, y, clip, true);
            }
        }

        public web_color get_def_color => _def_color;
        public int cvt_units(string str, int fontSize, out bool is_percent)
        {
            is_percent = false;
            if (str == null) return 0;
            var val = new css_length();
            val.fromString(str);
            if (is_percent && val.units == css_units_percentage && !val.is_predefined)
                is_percent = true;
            return cvt_units(val, fontSize);
        }
        public int cvt_units(css_length val, int fontSize, int size = 0);
{
	if(val.is_predefined())
	{
		return 0;
	}
	int ret = 0;
	switch(val.units())
	{
	case css_units_percentage:
		ret = val.calc_percent(size);
		break;
	case css_units_em:
		ret = round_f(val.val() * fontSize);
        val.set_value((float) ret, css_units_px);
		break;
	case css_units_pt:
		ret = m_container.pt_to_px((int) val.val());
        val.set_value((float) ret, css_units_px);
		break;
	case css_units_in:
		ret = m_container.pt_to_px((int) (val.val() * 72));
		val.set_value((float) ret, css_units_px);
		break;
	case css_units_cm:
		ret = m_container.pt_to_px((int) (val.val() * 0.3937 * 72));
		val.set_value((float) ret, css_units_px);
		break;
	case css_units_mm:
		ret = m_container.pt_to_px((int) (val.val() * 0.3937 * 72) / 10);
		val.set_value((float) ret, css_units_px);
		break;
	case css_units_vw:
		ret = (int) ((double) m_media.width* (double) val.val() / 100.0);
		break;
	case css_units_vh:
		ret = (int) ((double) m_media.height* (double) val.val() / 100.0);
		break;
	case css_units_vmin:
		ret = (int) ((double) std::min(m_media.height, m_media.width) * (double) val.val() / 100.0);
		break;
	case css_units_vmax:
		ret = (int) ((double) std::max(m_media.height, m_media.width) * (double) val.val() / 100.0);
		break;
	default:
		ret = (int) val.val();
		break;
	}
	return ret;
}
public int width => _size.width;
public int height => _size.height;
public void add_stylesheet(string str, string baseurl, string media)
{
    if (str && str[0])
    {
        m_css.push_back(css_text(str, baseurl, media));
    }
}

public bool on_mouse_over(int x, int y, int client_x, int client_y, IList<position> redraw_boxes)
{
    if (!m_root)
    {
        return false;
    }

    element::ptr over_el = m_root.get_element_by_point(x, y, client_x, client_y);

    bool state_was_changed = false;

    if (over_el != m_over_element)
    {
        if (m_over_element)
        {
            if (m_over_element.on_mouse_leave())
            {
                state_was_changed = true;
            }
        }
        m_over_element = over_el;
    }

    const tchar_t* cursor = 0;

    if (m_over_element)
    {
        if (m_over_element.on_mouse_over())
        {
            state_was_changed = true;
        }
        cursor = m_over_element.get_cursor();
    }

    m_container.set_cursor(cursor ? cursor : _t("auto"));

    if (state_was_changed)
    {
        return m_root.find_styles_changes(redraw_boxes, 0, 0);
    }
    return false;
}

public bool on_lbutton_down(int x, int y, int client_x, int client_y, IList<position> redraw_boxes)
{
    if (!m_root)
    {
        return false;
    }

    element::ptr over_el = m_root.get_element_by_point(x, y, client_x, client_y);

    bool state_was_changed = false;

    if (over_el != m_over_element)
    {
        if (m_over_element)
        {
            if (m_over_element.on_mouse_leave())
            {
                state_was_changed = true;
            }
        }
        m_over_element = over_el;
        if (m_over_element)
        {
            if (m_over_element.on_mouse_over())
            {
                state_was_changed = true;
            }
        }
    }

    const tchar_t* cursor = 0;

    if (m_over_element)
    {
        if (m_over_element.on_lbutton_down())
        {
            state_was_changed = true;
        }
        cursor = m_over_element.get_cursor();
    }

    m_container.set_cursor(cursor ? cursor : _t("auto"));

    if (state_was_changed)
    {
        return m_root.find_styles_changes(redraw_boxes, 0, 0);
    }

    return false;
}

public bool on_lbutton_up(int x, int y, int client_x, int client_y, IList<position> redraw_boxes)
{
    if (!m_root)
    {
        return false;
    }
    if (m_over_element)
    {
        if (m_over_element.on_lbutton_up())
        {
            return m_root.find_styles_changes(redraw_boxes, 0, 0);
        }
    }
    return false;
}

public bool on_mouse_leave(ILIst<position> redraw_boxes)
{
    if (!m_root)
    {
        return false;
    }
    if (m_over_element)
    {
        if (m_over_element.on_mouse_leave())
        {
            return m_root.find_styles_changes(redraw_boxes, 0, 0);
        }
    }
    return false;
}

public element create_element(string tag_name, Dictionary<string, string> attributes)
{
    element::ptr newTag;
    document::ptr this_doc = shared_from_this();
    if (m_container)
    {
        newTag = m_container.create_element(tag_name, attributes, this_doc);
    }
    if (!newTag)
    {
        if (!t_strcmp(tag_name, _t("br")))
        {
            newTag = std::make_shared<litehtml::el_break>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("p")))
        {
            newTag = std::make_shared<litehtml::el_para>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("img")))
        {
            newTag = std::make_shared<litehtml::el_image>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("table")))
        {
            newTag = std::make_shared<litehtml::el_table>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("td")) || !t_strcmp(tag_name, _t("th")))
        {
            newTag = std::make_shared<litehtml::el_td>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("link")))
        {
            newTag = std::make_shared<litehtml::el_link>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("title")))
        {
            newTag = std::make_shared<litehtml::el_title>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("a")))
        {
            newTag = std::make_shared<litehtml::el_anchor>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("tr")))
        {
            newTag = std::make_shared<litehtml::el_tr>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("style")))
        {
            newTag = std::make_shared<litehtml::el_style>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("base")))
        {
            newTag = std::make_shared<litehtml::el_base>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("body")))
        {
            newTag = std::make_shared<litehtml::el_body>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("div")))
        {
            newTag = std::make_shared<litehtml::el_div>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("script")))
        {
            newTag = std::make_shared<litehtml::el_script>(this_doc);
        }
        else if (!t_strcmp(tag_name, _t("font")))
        {
            newTag = std::make_shared<litehtml::el_font>(this_doc);
        }
        else
        {
            newTag = std::make_shared<litehtml::html_tag>(this_doc);
        }
    }

    if (newTag)
    {
        newTag.set_tagName(tag_name);
        for (string_map::const_iterator iter = attributes.begin(); iter != attributes.end(); iter++)
        {
            newTag.set_attr(iter.first.c_str(), iter.second.c_str());
        }
    }

    return newTag;
}

public element root => _root;
public void get_fixed_boxes(out IList<position> fixed_boxes) => fixed_boxes = _fixed_boxes;
public void add_fixed_box(position pos) => _fixed_boxes.Add(pos);
public void add_media_list(media_query_list list)
{
    if (list)
    {
        if (std::find(m_media_lists.begin(), m_media_lists.end(), list) == m_media_lists.end())
        {
            m_media_lists.push_back(list);
        }
    }
}

public bool media_changed()
{
    if (!m_media_lists.empty())
    {
        container().get_media_features(m_media);
        if (update_media_lists(m_media))
        {
            m_root.refresh_styles();
            m_root.parse_styles();
            return true;
        }
    }
    return false;
}

public bool lang_changed()
{
    if (!m_media_lists.empty())
    {
        tstring culture;
        container().get_language(m_lang, culture);
        if (!culture.empty())
        {
            m_culture = m_lang + _t('-') + culture;
        }
        else
        {
            m_culture.clear();
        }
        m_root.refresh_styles();
        m_root.parse_styles();
        return true;
    }
    return false;
}

public bool match_lang(string lang) => lang == _lang || lang == _culture;
public void add_tabular(element el) => _tabular_elements.Add(el);
public element get_over_element => _over_element;

public static document createFromString(string str, document_container objPainter, context ctx, css user_styles = 0) => createFromUTF8(litehtml_to_utf8(str), objPainter, ctx, user_styles);
public static document createFromUTF8(string str, document_container objPainter, context ctx, css user_styles = 0)
{
    // parse document into GumboOutput
    GumboOutput* output = gumbo_parse((const char*) str);

    // Create litehtml::document
    litehtml::document::ptr doc = std::make_shared<litehtml::document>(objPainter, ctx);

    // Create litehtml::elements.
    elements_vector root_elements;
    doc.create_node(output.root, root_elements);
    if (!root_elements.empty())
    {
        doc.m_root = root_elements.back();
    }
    // Destroy GumboOutput
    gumbo_destroy_output(&kGumboDefaultOptions, output);

    // Let's process created elements tree
    if (doc.m_root)
    {
        doc.container().get_media_features(doc.m_media);

        // apply master CSS
        doc.m_root.apply_stylesheet(ctx.master_css());

        // parse elements attributes
        doc.m_root.parse_attributes();

        // parse style sheets linked in document
        media_query_list::ptr media;
        for (css_text::vector::iterator css = doc.m_css.begin(); css != doc.m_css.end(); css++)
        {
            if (!css.media.empty())
            {
                media = media_query_list::create_from_string(css.media, doc);
            }
            else
            {
                media = 0;
            }
            doc.m_styles.parse_stylesheet(css.text.c_str(), css.baseurl.c_str(), doc, media);
        }
        // Sort css selectors using CSS rules.
        doc.m_styles.sort_selectors();

        // get current media features
        if (!doc.m_media_lists.empty())
        {
            doc.update_media_lists(doc.m_media);
        }

        // Apply parsed styles.
        doc.m_root.apply_stylesheet(doc.m_styles);

        // Apply user styles if any
        if (user_styles)
        {
            doc.m_root.apply_stylesheet(*user_styles);
        }

        // Parse applied styles in the elements
        doc.m_root.parse_styles();

        // Now the m_tabular_elements is filled with tabular elements.
        // We have to check the tabular elements for missing table elements 
        // and create the anonymous boxes in visual table layout
        doc.fix_tables_layout();

        // Fanaly initialize elements
        doc.m_root.init();
    }

    return doc;
}

uint_ptr add_font(string name, int size, string weight, string style, string decoration, font_metrics fm)
{
    uint_ptr ret = 0;

    if (!name || (name && !t_strcasecmp(name, _t("inherit"))))
    {
        name = m_container.get_default_font_name();
    }

    if (!size)
    {
        size = container().get_default_font_size();
    }

    tchar_t strSize[20];
    t_itoa(size, strSize, 20, 10);

    tstring key = name;
    key += _t(":");
    key += strSize;
    key += _t(":");
    key += weight;
    key += _t(":");
    key += style;
    key += _t(":");
    key += decoration;

    if (m_fonts.find(key) == m_fonts.end())
    {
        font_style fs = (font_style)value_index(style, font_style_strings, fontStyleNormal);
        int fw = value_index(weight, font_weight_strings, -1);
        if (fw >= 0)
        {
            switch (fw)
            {
                case litehtml::fontWeightBold:
                    fw = 700;
                    break;
                case litehtml::fontWeightBolder:
                    fw = 600;
                    break;
                case litehtml::fontWeightLighter:
                    fw = 300;
                    break;
                default:
                    fw = 400;
                    break;
            }
        }
        else
        {
            fw = t_atoi(weight);
            if (fw < 100)
            {
                fw = 400;
            }
        }

        unsigned int decor = 0;

        if (decoration)
        {
            std::vector<tstring> tokens;
            split_string(decoration, tokens, _t(" "));
            for (std::vector<tstring>::iterator i = tokens.begin(); i != tokens.end(); i++)
            {
                if (!t_strcasecmp(i.c_str(), _t("underline")))
                {
                    decor |= font_decoration_underline;
                }
                else if (!t_strcasecmp(i.c_str(), _t("line-through")))
                {
                    decor |= font_decoration_linethrough;
                }
                else if (!t_strcasecmp(i.c_str(), _t("overline")))
                {
                    decor |= font_decoration_overline;
                }
            }
        }

        font_item fi = { 0 };

        fi.font = m_container.create_font(name, size, fw, fs, decor, &fi.metrics);
        m_fonts[key] = fi;
        ret = fi.font;
        if (fm)
        {
            *fm = fi.metrics;
        }
    }
    return ret;
}

void create_node(GumboNode node, IList<elements> elements)
{
    switch (node.type)
    {
        case GUMBO_NODE_ELEMENT:
            {
                string_map attrs;
                GumboAttribute* attr;
                for (unsigned int i = 0; i < node.v.element.attributes.length; i++)
                {
                    attr = (GumboAttribute*)node.v.element.attributes.data[i];
                    attrs[tstring(litehtml_from_utf8(attr.name))] = litehtml_from_utf8(attr.value);
                }


                element::ptr ret;
                const char* tag = gumbo_normalized_tagname(node.v.element.tag);
                if (tag[0])
                {
                    ret = create_element(litehtml_from_utf8(tag), attrs);
                }
                else
                {
                    if (node.v.element.original_tag.data && node.v.element.original_tag.length)
                    {
                        std::string strA;
                        gumbo_tag_from_original_text(&node.v.element.original_tag);
                        strA.append(node.v.element.original_tag.data, node.v.element.original_tag.length);
                        ret = create_element(litehtml_from_utf8(strA.c_str()), attrs);
                    }
                }
                if (ret)
                {
                    elements_vector child;
                    for (unsigned int i = 0; i < node.v.element.children.length; i++)
                    {
                        child.clear();
                        create_node(static_cast<GumboNode*>(node.v.element.children.data[i]), child);
                        std::for_each(child.begin(), child.end(),
    
                            [&ret](element::ptr & el)
    
                        {
                            ret.appendChild(el);
                        }
					);
                }
                elements.push_back(ret);
            }
    }
    break;
	case GUMBO_NODE_TEXT:
    {
        std::wstring str;
        std::wstring str_in = (const wchar_t*) (utf8_to_wchar(node.v.text.text));
        ucode_t c;
        for (size_t i = 0; i < str_in.length(); i++)
        {
            c = (ucode_t)str_in[i];
            if (c <= ' ' && (c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\f'))
            {
                if (!str.empty())
                {
                    elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                    str.clear();
                }
                str += c;
                elements.push_back(std::make_shared<el_space>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                str.clear();
            }
            // CJK character range
            else if (c >= 0x4E00 && c <= 0x9FCC)
            {
                if (!str.empty())
                {
                    elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                    str.clear();
                }
                str += c;
                elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                str.clear();
            }
            else
            {
                str += c;
            }
        }
        if (!str.empty())
        {
            elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
        }
    }
    break;
	case GUMBO_NODE_CDATA:
    {
        element::ptr ret = std::make_shared<el_cdata>(shared_from_this());
        ret.set_data(litehtml_from_utf8(node.v.text.text));
        elements.push_back(ret);
    }
    break;
	case GUMBO_NODE_COMMENT:
    {
        element::ptr ret = std::make_shared<el_comment>(shared_from_this());
        ret.set_data(litehtml_from_utf8(node.v.text.text));
        elements.push_back(ret);
    }
    break;
	case GUMBO_NODE_WHITESPACE:
    {
        tstring str = litehtml_from_utf8(node.v.text.text);
        for (size_t i = 0; i < str.length(); i++)
        {
            elements.push_back(std::make_shared<el_space>(str.substr(i, 1).c_str(), shared_from_this()));
        }
    }
    break;
    default:
		break;
}
}

bool update_media_lists(media_features features)
{
    bool update_styles = false;
    for (media_query_list::vector::iterator iter = m_media_lists.begin(); iter != m_media_lists.end(); iter++)
    {
        if ((*iter).apply_media_features(features))
        {
            update_styles = true;
        }
    }
    return update_styles;
}

void fix_tables_layout()
{
    size_t i = 0;
    while (i < m_tabular_elements.size())
    {
        element::ptr el_ptr = m_tabular_elements[i];

        switch (el_ptr.get_display())
        {
            case display_inline_table:
            case display_table:
                fix_table_children(el_ptr, display_table_row_group, _t("table-row-group"));
                break;
            case display_table_footer_group:
            case display_table_row_group:
            case display_table_header_group:
                fix_table_parent(el_ptr, display_table, _t("table"));
                fix_table_children(el_ptr, display_table_row, _t("table-row"));
                break;
            case display_table_row:
                fix_table_parent(el_ptr, display_table_row_group, _t("table-row-group"));
                fix_table_children(el_ptr, display_table_cell, _t("table-cell"));
                break;
            case display_table_cell:
                fix_table_parent(el_ptr, display_table_row, _t("table-row"));
                break;
            // TODO: make table layout fix for table-caption, table-column etc. elements
            case display_table_caption:
            case display_table_column:
            case display_table_column_group:
            default:
                break;
        }
        i++;
    }
}
void fix_table_children(element el_ptr, style_display disp, string disp_str)
{
    elements_vector tmp;
    elements_vector::iterator first_iter = el_ptr.m_children.begin();
    elements_vector::iterator cur_iter = el_ptr.m_children.begin();

    auto flush_elements = [&]()

    {
        element::ptr annon_tag = std::make_shared<html_tag>(shared_from_this());
        style st;
        st.add_property(_t("display"), disp_str, 0, false);
        annon_tag.add_style(st);
        annon_tag.parent(el_ptr);
        annon_tag.parse_styles();
        std::for_each(tmp.begin(), tmp.end(),

            [&annon_tag](element::ptr & el)

            {
            annon_tag.appendChild(el);
        }
		);
        first_iter = el_ptr.m_children.insert(first_iter, annon_tag);
        cur_iter = first_iter + 1;
        while (cur_iter != el_ptr.m_children.end() && (*cur_iter).parent() != el_ptr)
        {
            cur_iter = el_ptr.m_children.erase(cur_iter);
        }
        first_iter = cur_iter;
        tmp.clear();
    };

    while (cur_iter != el_ptr.m_children.end())
    {
        if ((*cur_iter).get_display() != disp)
        {
            if (!(*cur_iter).is_white_space() || ((*cur_iter).is_white_space() && !tmp.empty()))
            {
                if (tmp.empty())
                {
                    first_iter = cur_iter;
                }
                tmp.push_back((*cur_iter));
            }
            cur_iter++;
        }
        else if (!tmp.empty())
        {
            flush_elements();
        }
        else
        {
            cur_iter++;
        }
    }
    if (!tmp.empty())
    {
        flush_elements();
    }
}
void fix_table_parent(element el_ptr, style_display disp, string disp_str)
{
    element::ptr parent = el_ptr.parent();

    if (parent.get_display() != disp)
    {
        elements_vector::iterator this_element = std::find_if(parent.m_children.begin(), parent.m_children.end(),

            [&](element::ptr & el)

            {
            if (el == el_ptr)
            {
                return true;
            }
            return false;
        }
		);
        if (this_element != parent.m_children.end())
        {
            style_display el_disp = el_ptr.get_display();
            elements_vector::iterator first = this_element;
            elements_vector::iterator last = this_element;
            elements_vector::iterator cur = this_element;

            // find first element with same display
            while (true)
            {
                if (cur == parent.m_children.begin()) break;
                cur--;
                if ((*cur).is_white_space() || (*cur).get_display() == el_disp)
                {
                    first = cur;
                }
                else
                {
                    break;
                }
            }

            // find last element with same display
            cur = this_element;
            while (true)
            {
                cur++;
                if (cur == parent.m_children.end()) break;

                if ((*cur).is_white_space() || (*cur).get_display() == el_disp)
                {
                    last = cur;
                }
                else
                {
                    break;
                }
            }

            // extract elements with the same display and wrap them with anonymous object
            element::ptr annon_tag = std::make_shared<html_tag>(shared_from_this());
            style st;
            st.add_property(_t("display"), disp_str, 0, false);
            annon_tag.add_style(st);
            annon_tag.parent(parent);
            annon_tag.parse_styles();
            std::for_each(first, last + 1,

                [&annon_tag](element::ptr & el)

                {
                annon_tag.appendChild(el);
            }
			);
            first = parent.m_children.erase(first, last + 1);
            parent.m_children.insert(first, annon_tag);
        }
    }
}

    }
}