namespace H3ml.Layout
{
	public class el_text : element
	{
		protected string _text;
		protected string _transformed_text;
		protected size _size;
		protected text_transform _text_transform;
		protected bool _use_transformed;
		protected bool _draw_spaces;

		public el_text(string text, document doc)
		{
			element = doc;
			if (text)
			{
				m_text = text;
			}
			m_text_transform = text_transform_none;
			m_use_transformed = false;
			m_draw_spaces = true;
		}

		public override void get_text(ref string text) => text += _text;
		public override string get_style_property(string name, bool inherited, string def = null)
		{
			if (inherited)
			{
				element::ptr el_parent = parent();
				if (el_parent)
				{
					return el_parent->get_style_property(name, inherited, def);
				}
			}
			return def;
		}
		public override void parse_styles(bool is_reparse)
		{
			m_text_transform = (text_transform)value_index(get_style_property(_t("text-transform"), true, _t("none")), text_transform_strings, text_transform_none);
			if (m_text_transform != text_transform_none)
			{
				m_transformed_text = m_text;
				m_use_transformed = true;
				get_document()->container()->transform_text(m_transformed_text, m_text_transform);
			}

			if (is_white_space())
			{
				m_transformed_text = _t(" ");
				m_use_transformed = true;
			}
			else
			{
				if (m_text == _t("\t"))
				{
					m_transformed_text = _t("    ");
					m_use_transformed = true;
				}
				if (m_text == _t("\n") || m_text == _t("\r"))
				{
					m_transformed_text = _t("");
					m_use_transformed = true;
				}
			}

			font_metrics fm;
			uint_ptr font = 0;
			element::ptr el_parent = parent();
			if (el_parent)
			{
				font = el_parent->get_font(&fm);
			}
			if (is_break())
			{
				m_size.height = 0;
				m_size.width = 0;
			}
			else
			{
				m_size.height = fm.height;
				m_size.width = get_document()->container()->text_width(m_use_transformed ? m_transformed_text.c_str() : m_text.c_str(), font);
			}
			m_draw_spaces = fm.draw_spaces;
		}

		public override int get_base_line()
		{
			element::ptr el_parent = parent();
			if (el_parent)
			{
				return el_parent->get_base_line();
			}
			return 0;
		}

		public override void draw(uint_ptr hdc, int x, int y, position clip)
		{
			if (is_white_space() && !m_draw_spaces)
			{
				return;
			}

			position pos = m_pos;
			pos.x += x;
			pos.y += y;

			if (pos.does_intersect(clip))
			{
				element::ptr el_parent = parent();
				if (el_parent)
				{
					document::ptr doc = get_document();

					uint_ptr font = el_parent->get_font();
					litehtml::web_color color = el_parent->get_color(_t("color"), true, doc->get_def_color());
					doc->container()->draw_text(hdc, m_use_transformed ? m_transformed_text.c_str() : m_text.c_str(), font, color, pos);
				}
			}
		}

		public override int line_height()
		{
			var el_parent = parent();
			return el_parent != 0 ? el_parent.line_height() : 0;
		}

		public override uint_ptr get_font(font_metrics fm = null)
		{
			var el_parent = parent();
				return el_parent != null ? el_parent.get_font(fm) : 0;
		}

		public override style_display get_display() => display_inline_text;

		public override white_space get_white_space()
		{
			var el_parent = parent();
			return el_parent != null ? el_parent.get_white_space() : white_space_normal;
		}

		public override element_position get_element_position(css_offsets offsets = null)
		{
			element::ptr p = parent();
			while (p && p->get_display() == display_inline)
			{
				if (p->get_element_position() == element_position_relative)
				{
					if (offsets)
					{
						*offsets = p->get_css_offsets();
					}
					return element_position_relative;
				}
				p = p->parent();
			}
			return element_position_static;
		}

		public override css_offsets get_css_offsets()
		{
			element::ptr p = parent();
			while (p && p->get_display() == display_inline)
			{
				if (p->get_element_position() == element_position_relative)
				{
					return p->get_css_offsets();
				}
				p = p->parent();
			}
			return css_offsets();
		}

		protected override void get_content_size(out size sz, int max_width) => sz = _size;
	}
}
