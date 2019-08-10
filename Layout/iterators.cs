using System.Collections.Generic;
using System.Linq;

namespace H3ml.Layout
{
    public interface iterator_selector
    {
        bool select(element el);
    }

    public class elements_iterator
    {
        struct stack_item
        {
            public int idx;
            public element el;
            //stack_item() { }
            stack_item(stack_item val)
            {
                idx = val.idx;
                el = val.el;
            }
        }

        Stack<stack_item> _stack = new Stack<stack_item>();
        element _el;
        int _idx;
        iterator_selector _go_inside;
        iterator_selector _select;

        public elements_iterator(element el, iterator_selector go_inside, iterator_selector select)
        {
            _el = el;
            _idx = -1;
            _go_inside = go_inside;
            _select = select;
        }

        public element next(bool ret_parent = true)
        {
            next_idx();
            while (_idx < _el.get_children_count)
            {
                var el = _el.get_child(_idx);
                if (el.get_children_count != 0 && _go_inside != null && _go_inside.select(el))
                {
                    stack_item si;
                    si.idx = _idx;
                    si.el = _el;
                    _stack.Push(si);
                    _el = el;
                    _idx = -1;
                    if (ret_parent)
                        return el;
                    next_idx();
                }
                else
                {
                    if (_select == null || (_select != null && _select.select(_el.get_child(_idx))))
                        return _el.get_child(_idx);
                    else next_idx();
                }
            }
            return null;
        }

        void next_idx()
        {
            _idx++;
            while (_idx >= _el.get_children_count && _stack.Count != 0)
            {
                var si = _stack.Last();
                _stack.Pop();
                _idx = si.idx;
                _el = si.el;
                _idx++;
                continue;
            }
        }
    }

    public class go_inside_inline : iterator_selector
    {
        public bool select(element el) => el.get_display == style_display.inline || el.get_display == style_display.inline_text;
    }

    public class go_inside_table : iterator_selector
    {
        public bool select(element el) => el.get_display == style_display.table_row_group || el.get_display == style_display.table_header_group || el.get_display == style_display.table_footer_group;
    }

    public class table_rows_selector : iterator_selector
    {
        public bool select(element el) => el.get_display == style_display.table_row;
    }

    public class table_cells_selector : iterator_selector
    {
        public bool select(element el) => el.get_display == style_display.table_cell;
    }
}
