using Gumbo;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace H3ml.Layout
{
    public struct css_text
    {
        public string text;
        public string baseurl;
        public string media;

        public css_text(string txt, string url, string media_str)
        {
            text = txt ?? string.Empty;
            baseurl = url ?? string.Empty;
            media = media_str ?? string.Empty;
        }

        public css_text(css_text val)
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

    public class document
    {
        element _root;
        Idocument_container _container;
        Dictionary<string, font_item> _fonts = new Dictionary<string, font_item>();
        List<css_text> _css = new List<css_text>();
        css _styles = new css();
        web_color _def_color;
        context _context;
        size _size;
        List<position> _fixed_boxes = new List<position>();
        List<media_query_list> _media_lists = new List<media_query_list>();
        element _over_element;
        List<element> _tabular_elements = new List<element>();
        media_features _media;
        string _lang;
        string _culture;

        public document(Idocument_container objContainer, context ctx)
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

        public Idocument_container container => _container;

        public object get_font(string name, int size, string weight, string style, string decoration, out font_metrics fm)
        {
            if (name == null || string.Equals(name, "inherit", StringComparison.OrdinalIgnoreCase))
                name = _container.get_default_font_name();
            if (size == 0)
                size = container.get_default_font_size();
            var key = $"{name}:{size}:{weight}:{style}:{decoration}";
            if (_fonts.TryGetValue(key, out var el))
            {
                fm = el.metrics;
                return el.font;
            }
            return add_font(name, size, weight, style, decoration, out fm);
        }

        public int render(int max_width, render_type rt = render_type.all)
        {
            var ret = 0;
            if (_root != null)
            {
                if (rt == render_type.fixed_only)
                {
                    _fixed_boxes.Clear();
                    _root.render_positioned(rt);
                }
                else
                {
                    ret = _root.render(0, 0, 0, max_width); //:h3ml
                    if (_root.fetch_positioned())
                    {
                        _fixed_boxes.Clear();
                        _root.render_positioned(rt);
                    }
                    _size.width = 0;
                    _size.height = 0;
                    _size.depth = 0; //:h3ml
                    _root.calc_document_size(ref _size);
                }
            }
            return ret;
        }

        public void draw(object hdc, int x, int y, int z, position clip) //:h3ml
        {
            if (_root != null)
            {
                _root.draw(hdc, x, y, z, clip); //:h3ml
                _root.draw_stacking_context(hdc, x, y, z, clip, true); //:h3ml
            }
        }

        public web_color get_def_color => _def_color;

        public int cvt_units(string str, int fontSize, out bool is_percent)
        {
            is_percent = false;
            if (str == null) return 0;
            var val = new css_length();
            val.fromString(str);
            if (is_percent && val.units == css_units.percentage && !val.is_predefined)
                is_percent = true;
            return cvt_units(val, fontSize);
        }

        public int cvt_units(css_length val, int fontSize, int size = 0)
        {
            if (val.is_predefined)
                return 0;
            var ret = 0;
            switch (val.units)
            {
                case css_units.percentage: ret = val.calc_percent(size); break;
                case css_units.em: ret = (int)Math.Round(val.val * fontSize); val.set_value(ret, css_units.px); break;
                case css_units.pt: ret = _container.pt_to_px((int)val.val); val.set_value(ret, css_units.px); break;
                case css_units.@in: ret = _container.pt_to_px((int)(val.val * 72)); val.set_value(ret, css_units.px); break;
                case css_units.cm: ret = _container.pt_to_px((int)(val.val * 0.3937 * 72)); val.set_value(ret, css_units.px); break;
                case css_units.mm: ret = _container.pt_to_px((int)(val.val * 0.3937 * 72) / 10); val.set_value(ret, css_units.px); break;
                case css_units.vw: ret = (int)(_media.width * (double)val.val / 100.0); break;
                case css_units.vh: ret = (int)(_media.height * (double)val.val / 100.0); break;
                case css_units.vmin: ret = (int)(Math.Min(_media.height, _media.width) * (double)val.val / 100.0); break;
                case css_units.vmax: ret = (int)(Math.Max(_media.height, _media.width) * (double)val.val / 100.0); break;
                default: ret = (int)val.val; break;
            }
            return ret;
        }

        public int width => _size.width;
        public int height => _size.height;
        public int depth => _size.depth; //:h3ml

        public void add_stylesheet(string str, string baseurl, string media)
        {
            if (!string.IsNullOrEmpty(str))
                _css.Add(new css_text(str, baseurl, media));
        }

        public bool on_mouse_over(int x, int y, int z, int client_x, int client_y, int client_z, IList<position> redraw_boxes) //:h3ml
        {
            if (_root == null)
                return false;
            var over_el = _root.get_element_by_point(x, y, z, client_x, client_y, client_z); //:h3ml
            var state_was_changed = false;
            if (over_el != _over_element)
            {
                if (_over_element != null)
                    if (_over_element.on_mouse_leave())
                        state_was_changed = true;
                _over_element = over_el;
            }
            string cursor = null;
            if (_over_element != null)
            {
                if (_over_element.on_mouse_over())
                    state_was_changed = true;
                cursor = _over_element.get_cursor();
            }
            _container.set_cursor(cursor ?? "auto");
            return state_was_changed ? _root.find_styles_changes(redraw_boxes, 0, 0, 0) : false; //:h3ml
        }

        public bool on_lbutton_down(int x, int y, int z, int client_x, int client_y, int client_z, IList<position> redraw_boxes) //:h3ml
        {
            if (_root == null)
                return false;
            var over_el = _root.get_element_by_point(x, y, z, client_x, client_y, client_z); //:h3ml
            var state_was_changed = false;
            if (over_el != _over_element)
            {
                if (_over_element != null)
                    if (_over_element.on_mouse_leave())
                        state_was_changed = true;
                _over_element = over_el;
                if (_over_element != null)
                    if (_over_element.on_mouse_over())
                        state_was_changed = true;
            }
            string cursor = null;
            if (_over_element != null)
            {
                if (_over_element.on_lbutton_down())
                    state_was_changed = true;
                cursor = _over_element.get_cursor();
            }
            _container.set_cursor(cursor ?? "auto");
            return state_was_changed ? _root.find_styles_changes(redraw_boxes, 0, 0, 0) : false; //:h3ml
        }

        public bool on_lbutton_up(int x, int y, int z, int client_x, int client_y, int client_z, IList<position> redraw_boxes) //:h3ml
        {
            if (_root == null)
                return false;
            if (_over_element != null)
                if (_over_element.on_lbutton_up())
                    return _root.find_styles_changes(redraw_boxes, 0, 0, 0); //:h3ml
            return false;
        }

        public bool on_mouse_leave(IList<position> redraw_boxes)
        {
            if (_root == null)
                return false;
            if (_over_element != null)
                if (_over_element.on_mouse_leave())
                    return _root.find_styles_changes(redraw_boxes, 0, 0, 0); //:h3ml
            return false;
        }

        public element create_element(string tag_name, Dictionary<string, string> attributes)
        {
            element newTag = null;
            var this_doc = this;
            if (_container != null)
                newTag = _container.create_element(tag_name, attributes, this_doc);
            if (newTag == null)
            {
                if (tag_name == "br") newTag = new el_break(this_doc);
                else if (tag_name == "p") newTag = new el_para(this_doc);
                else if (tag_name == "img") newTag = new el_image(this_doc);
                else if (tag_name == "table") newTag = new el_table(this_doc);
                else if (tag_name == "td" || tag_name == "th") newTag = new el_td(this_doc);
                else if (tag_name == "link") newTag = new el_link(this_doc);
                else if (tag_name == "title") newTag = new el_title(this_doc);
                else if (tag_name == "a") newTag = new el_anchor(this_doc);
                else if (tag_name == "tr") newTag = new el_tr(this_doc);
                else if (tag_name == "style") newTag = new el_style(this_doc);
                else if (tag_name == "base") newTag = new el_base(this_doc);
                else if (tag_name == "body") newTag = new el_body(this_doc);
                else if (tag_name == "div") newTag = new el_div(this_doc);
                else if (tag_name == "script") newTag = new el_script(this_doc);
                else if (tag_name == "font") newTag = new el_font(this_doc);
                else newTag = new html_tag(this_doc);
            }
            if (newTag != null)
            {
                newTag.set_tagName(tag_name);
                foreach (var iter in attributes)
                    newTag.set_attr(iter.Key, iter.Value);
            }
            return newTag;
        }

        public element root => _root;

        public void get_fixed_boxes(out IList<position> fixed_boxes) => fixed_boxes = _fixed_boxes;

        public void add_fixed_box(position pos) => _fixed_boxes.Add(pos);

        public void add_media_list(media_query_list list)
        {
            if (list != null && !_media_lists.Contains(list))
                _media_lists.Add(list);
        }

        public bool media_changed()
        {
            if (_media_lists.Count != 0)
            {
                container.get_media_features(_media);
                if (update_media_lists(_media))
                {
                    _root.refresh_styles();
                    _root.parse_styles();
                    return true;
                }
            }
            return false;
        }

        public bool lang_changed()
        {
            if (_media_lists.Count != 0)
            {
                container.get_language(_lang, out var culture);
                _culture = !string.IsNullOrEmpty(culture) ? $"{_lang}-{culture}" : string.Empty;
                _root.refresh_styles();
                _root.parse_styles();
                return true;
            }
            return false;
        }

        public bool match_lang(string lang) => lang == _lang || lang == _culture;

        public void add_tabular(element el) => _tabular_elements.Add(el);

        public element get_over_element => _over_element;

        public static document createFromString(string str, Idocument_container objPainter, context ctx, css user_styles = null) => createFromUTF8(Encoding.UTF8.GetString(Encoding.Default.GetBytes(str)), objPainter, ctx, user_styles);
        public static document createFromUTF8(string str, Idocument_container objPainter, context ctx, css user_styles = null)
        {
            var doc = new document(objPainter, ctx); // Create litehtml::document
            var root_elements = new List<element>();
            using (var gumbo = new GumboWrapper(str))  // parse document into GumboOutput
                doc.create_node(gumbo.Document.Root, root_elements); // Create litehtml::elements.
            if (root_elements.Count != 0)
                doc._root = root_elements.Last();
            // Let's process created elements tree
            if (doc._root != null)
            {
                doc.container.get_media_features(doc._media);
                doc._root.apply_stylesheet(ctx.master_css); // apply master CSS
                doc._root.parse_attributes(); // parse elements attributes
                foreach (var css in doc._css) // parse style sheets linked in document
                    doc._styles.parse_stylesheet(css.text, css.baseurl, doc, !string.IsNullOrEmpty(css.media) ? media_query_list.create_from_string(css.media, doc) : null);
                doc._styles.sort_selectors(); // Sort css selectors using CSS rules.
                if (doc._media_lists.Count != 0) doc.update_media_lists(doc._media); // get current media features
                doc._root.apply_stylesheet(doc._styles); // Apply parsed styles.
                if (user_styles != null) doc._root.apply_stylesheet(user_styles); // Apply user styles if any
                doc._root.parse_styles(); // Parse applied styles in the elements
                doc.fix_tables_layout(); // Now the _tabular_elements is filled with tabular elements. We have to check the tabular elements for missing table elements and create the anonymous boxes in visual table layout
                doc._root.init(); // Fanaly initialize elements
            }
            return doc;
        }

        object add_font(string name, int size, string weight, string style, string decoration, out font_metrics fm)
        {
            fm = default(font_metrics);
            object ret = null;
            if (name == null || string.Equals(name, "inherit", StringComparison.OrdinalIgnoreCase))
                name = _container.get_default_font_name();
            if (size == 0)
                size = container.get_default_font_size();
            var key = $"{name}:{size}:{weight}:{style}:{decoration}";
            if (!_fonts.ContainsKey(key))
            {
                var fs = (font_style)html.value_index(style, types.font_style_strings, (int)font_style.normal);
                var fw = html.value_index(weight, types.font_weight_strings, -1);
                if (fw >= 0)
                    switch ((font_weight)fw)
                    {
                        case font_weight.bold: fw = (int)font_weight.w700; break;
                        case font_weight.bolder: fw = (int)font_weight.w600; break;
                        case font_weight.lighter: fw = (int)font_weight.w300; break;
                        default: fw = (int)font_weight.w400; break;
                    }
                else
                {
                    fw = int.Parse(weight);
                    if (fw < 100) fw = (int)font_weight.w400;
                }
                var decor = 0U;
                if (decoration != null)
                {
                    var tokens = new List<string>();
                    html.split_string(decoration, tokens, " ");
                    foreach (var i in tokens)
                    {
                        if (string.Equals(i, "underline", StringComparison.OrdinalIgnoreCase)) decor |= types.font_decoration_underline;
                        else if (string.Equals(i, "line-through", StringComparison.OrdinalIgnoreCase)) decor |= types.font_decoration_linethrough;
                        else if (string.Equals(i, "overline", StringComparison.OrdinalIgnoreCase)) decor |= types.font_decoration_overline;
                    }
                }
                var fi = new font_item();
                fi.font = _container.create_font(name, size, fw, fs, decor, out fi.metrics);
                _fonts[key] = fi;
                ret = fi.font;
                fm = fi.metrics;
            }
            return ret;
        }

        void create_node(NodeWrapper node, List<element> elements)
        {
            switch (node)
            {
                case ElementWrapper elementNode when node.Type == GumboNodeType.GUMBO_NODE_ELEMENT:
                    {
                        var attrs = new Dictionary<string, string>();
                        foreach (var attr in elementNode.Attributes)
                            attrs[attr.Name] = attr.Value;
                        element ret = null;
                        var tag = elementNode.OriginalTagName;
                        if (tag != null) ret = create_element(tag, attrs);
                        else if (elementNode.OriginalTag != null) ret = create_element(elementNode.OriginalTag, attrs);
                        if (ret != null)
                        {
                            var children = new List<element>();
                            foreach (var child in elementNode.Children)
                            {
                                children.Clear();
                                create_node(child, children);
                                foreach (var i in children)
                                    ret.appendChild(i);
                            }
                            elements.Add(ret);
                        }
                    }
                    break;
                case TextWrapper textNode when node.Type == GumboNodeType.GUMBO_NODE_TEXT:
                    {
                        elements.Add(new el_space(textNode.Value, this));
                        //std::wstring str;
                        //var str_in = textNode.Value;
                        //ucode_t c;
                        //for (size_t i = 0; i < str_in.length(); i++)
                        //{
                        //    c = (ucode_t)str_in[i];
                        //    if (c <= ' ' && (c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\f'))
                        //    {
                        //        if (!str.empty())
                        //        {
                        //            elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                        //            str.clear();
                        //        }
                        //        str += c;
                        //        elements.push_back(std::make_shared<el_space>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                        //        str.clear();
                        //    }
                        //    // CJK character range
                        //    else if (c >= 0x4E00 && c <= 0x9FCC)
                        //    {
                        //        if (!str.empty())
                        //        {
                        //            elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                        //            str.clear();
                        //        }
                        //        str += c;
                        //        elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                        //        str.clear();
                        //    }
                        //    else
                        //    {
                        //        str += c;
                        //    }
                        //}
                        //if (!str.empty())
                        //{
                        //    elements.push_back(std::make_shared<el_text>(litehtml_from_wchar(str.c_str()), shared_from_this()));
                        //}
                    }
                    break;
                case TextWrapper textNode when node.Type == GumboNodeType.GUMBO_NODE_CDATA:
                    {
                        var ret = new el_cdata(this);
                        ret.set_data(textNode.Value);
                        elements.Add(ret);
                    }
                    break;
                case TextWrapper textNode when node.Type == GumboNodeType.GUMBO_NODE_COMMENT:
                    {
                        var ret = new el_comment(this);
                        ret.set_data(textNode.Value);
                        elements.Add(ret);
                    }
                    break;
                case TextWrapper textNode when node.Type == GumboNodeType.GUMBO_NODE_WHITESPACE:
                    {
                        var str = textNode.Value;
                        for (var i = 0; i < str.Length; i++)
                            elements.Add(new el_space(str.Substring(i, 1), this));
                    }
                    break;
                default: break;
            }
        }

        bool update_media_lists(media_features features)
        {
            var update_styles = false;
            foreach (var iter in _media_lists)
                if (iter.apply_media_features(features))
                    update_styles = true;
            return update_styles;
        }

        void fix_tables_layout()
        {
            var i = 0;
            while (i < _tabular_elements.Count)
            {
                var el_ptr = _tabular_elements[i];
                switch (el_ptr.get_display)
                {
                    case style_display.inline_table:
                    case style_display.table: fix_table_children(el_ptr, style_display.table_row_group, "table-row-group"); break;
                    case style_display.table_footer_group:
                    case style_display.table_row_group:
                    case style_display.table_header_group: fix_table_parent(el_ptr, style_display.table, "table"); fix_table_children(el_ptr, style_display.table_row, "table-row"); break;
                    case style_display.table_row: fix_table_parent(el_ptr, style_display.table_row_group, "table-row-group"); fix_table_children(el_ptr, style_display.table_cell, "table-cell"); break;
                    case style_display.table_cell: fix_table_parent(el_ptr, style_display.table_row, "table-row"); break;
                    // TODO: make table layout fix for table-caption, table-column etc. elements
                    case style_display.table_caption:
                    case style_display.table_column:
                    case style_display.table_column_group:
                    default: break;
                }
                i++;
            }
        }

        void fix_table_children(element el_ptr, style_display disp, string disp_str)
        {
            //          IList<element> tmp;
            //          var first_iter = el_ptr._children.begin();
            //          var cur_iter = el_ptr._children.begin();
            //          auto flush_elements = [&]()
            //  {
            //              element::ptr annon_tag = std::make_shared<html_tag>(shared_from_this());
            //              style st;
            //              st.add_property(_t("display"), disp_str, 0, false);
            //              annon_tag.add_style(st);
            //              annon_tag.parent(el_ptr);
            //              annon_tag.parse_styles();
            //              std::for_each(tmp.begin(), tmp.end(),
            //                  [&annon_tag](element::ptr & el)
            //          {
            //                  annon_tag.appendChild(el);
            //              }
            //);
            //              first_iter = el_ptr.m_children.insert(first_iter, annon_tag);
            //              cur_iter = first_iter + 1;
            //              while (cur_iter != el_ptr.m_children.end() && (*cur_iter).parent() != el_ptr)
            //              {
            //                  cur_iter = el_ptr.m_children.erase(cur_iter);
            //              }
            //              first_iter = cur_iter;
            //              tmp.clear();
            //          };
            //          while (cur_iter != el_ptr.m_children.end())
            //          {
            //              if ((*cur_iter).get_display() != disp)
            //              {
            //                  if (!(*cur_iter).is_white_space() || ((*cur_iter).is_white_space() && !tmp.empty()))
            //                  {
            //                      if (tmp.empty())
            //                      {
            //                          first_iter = cur_iter;
            //                      }
            //                      tmp.push_back((*cur_iter));
            //                  }
            //                  cur_iter++;
            //              }
            //              else if (!tmp.empty())
            //              {
            //                  flush_elements();
            //              }
            //              else
            //              {
            //                  cur_iter++;
            //              }
            //          }
            //          if (!tmp.empty())
            //          {
            //              flush_elements();
            //          }
        }

        void fix_table_parent(element el_ptr, style_display disp, string disp_str)
        {
            //          element::ptr parent = el_ptr.parent();
            //          if (parent.get_display() != disp)
            //          {
            //              IList<element>::iterator this_element = std::find_if(parent.m_children.begin(), parent.m_children.end(),
            //                  [&](element::ptr & el)
            //          {
            //                  if (el == el_ptr)
            //                  {
            //                      return true;
            //                  }
            //                  return false;
            //              }
            //);
            //              if (this_element != parent.m_children.end())
            //              {
            //                  style_display el_disp = el_ptr.get_display();
            //                  IList<element>::iterator first = this_element;
            //                  IList<element>::iterator last = this_element;
            //                  IList<element>::iterator cur = this_element;
            //                  // find first element with same display
            //                  while (true)
            //                  {
            //                      if (cur == parent.m_children.begin()) break;
            //                      cur--;
            //                      if ((*cur).is_white_space() || (*cur).get_display() == el_disp)
            //                      {
            //                          first = cur;
            //                      }
            //                      else
            //                      {
            //                          break;
            //                      }
            //                  }
            //                  // find last element with same display
            //                  cur = this_element;
            //                  while (true)
            //                  {
            //                      cur++;
            //                      if (cur == parent.m_children.end()) break;
            //                      if ((*cur).is_white_space() || (*cur).get_display() == el_disp)
            //                      {
            //                          last = cur;
            //                      }
            //                      else
            //                      {
            //                          break;
            //                      }
            //                  }
            //                  // extract elements with the same display and wrap them with anonymous object
            //                  element::ptr annon_tag = std::make_shared<html_tag>(shared_from_this());
            //                  style st;
            //                  st.add_property(_t("display"), disp_str, 0, false);
            //                  annon_tag.add_style(st);
            //                  annon_tag.parent(parent);
            //                  annon_tag.parse_styles();
            //                  std::for_each(first, last + 1,
            //                      [&annon_tag](element::ptr & el)
            //              {
            //                      annon_tag.appendChild(el);
            //                  }
            //	);
            //                  first = parent.m_children.erase(first, last + 1);
            //                  parent.m_children.insert(first, annon_tag);
            //              }
            //          }
        }
    }
}