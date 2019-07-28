using System;
using System.Linq;
using System.Collections.Generic;

namespace H3ml.Layout
{
    public class table_row
    {
        public int height;
        public int border_top;
        public int border_bottom;
        public element el_row;
        public int top;
        public int bottom;
        public css_length css_height;
        public int min_height;

        public table_row()
        {
            min_height = 0;
            top = 0;
            bottom = 0;
            border_bottom = 0;
            border_top = 0;
            height = 0;
            el_row = null;
            css_height.predef = 0;
        }

        public table_row(int h, element row)
        {
            min_height = 0;
            height = h;
            el_row = row;
            border_bottom = 0;
            border_top = 0;
            top = 0;
            bottom = 0;
            if (row != null)
                css_height = row.get_css_height();
        }

        public table_row(table_row val)
        {
            min_height = val.min_height;
            top = val.top;
            bottom = val.bottom;
            border_bottom = val.border_bottom;
            border_top = val.border_top;
            height = val.height;
            css_height = val.css_height;
            el_row = val.el_row;
        }
    }

    public class table_column
    {
        public int min_width;
        public int max_width;
        public int width;
        public css_length css_width;
        public int border_left;
        public int border_right;
        public int left;
        public int right;

        public table_column()
        {
            left = 0;
            right = 0;
            border_left = 0;
            border_right = 0;
            min_width = 0;
            max_width = 0;
            width = 0;
            css_width.predef = 0;
        }

        public table_column(int min_w, int max_w)
        {
            left = 0;
            right = 0;
            border_left = 0;
            border_right = 0;
            max_width = max_w;
            min_width = min_w;
            width = 0;
            css_width.predef = 0;
        }

        public table_column(table_column val)
        {
            left = val.left;
            right = val.right;
            border_left = val.border_left;
            border_right = val.border_right;
            max_width = val.max_width;
            min_width = val.min_width;
            width = val.width;
            css_width = val.css_width;
        }
    }

    public interface table_column_accessor
    {
        int get(table_column col);
        void set(table_column col, int value);
    }

    public class table_column_accessor_max_width : table_column_accessor
    {
        table_column_accessor_max_width() { }
        public static table_column_accessor Default = new table_column_accessor_max_width();
        public int get(table_column col) => col.max_width;
        public void set(table_column col, int value) => col.max_width = value;
    }

    public class table_column_accessor_min_width : table_column_accessor
    {
        table_column_accessor_min_width() { }
        public static table_column_accessor Default = new table_column_accessor_min_width();
        public int get(table_column col) => col.min_width;
        public void set(table_column col, int value) => col.min_width = value;
    }

    public class table_column_accessor_width : table_column_accessor
    {
        table_column_accessor_width() { }
        public static table_column_accessor Default = new table_column_accessor_width();
        public int get(table_column col) => col.width;
        public void set(table_column col, int value) => col.width = value;
    }

    public class table_cell
    {
        public element el;
        public int colspan;
        public int rowspan;
        public int min_width;
        public int min_height;
        public int max_width;
        public int max_height;
        public int width;
        public int height;
        public margins borders;

        public table_cell()
        {
            min_width = 0;
            min_height = 0;
            max_width = 0;
            max_height = 0;
            width = 0;
            height = 0;
            colspan = 1;
            rowspan = 1;
            el = null;
        }

        public table_cell(table_cell val)
        {
            el = val.el;
            colspan = val.colspan;
            rowspan = val.rowspan;
            width = val.width;
            height = val.height;
            min_width = val.min_width;
            min_height = val.min_height;
            max_width = val.max_width;
            max_height = val.max_height;
            borders = val.borders;
        }
    }

    public class table_grid
    {
        int _rows_count;
        int _cols_count;
        List<List<table_cell>> _cells = new List<List<table_cell>>();
        List<table_column> _columns = new List<table_column>();
        List<table_row> _rows = new List<table_row>();

        public table_grid()
        {
            _rows_count = 0;
            _cols_count = 0;
        }

        public void clear()
        {
            _rows_count = 0;
            _cols_count = 0;
            _cells.Clear();
            _columns.Clear();
            _rows.Clear();
        }

        public void begin_row(element row)
        {
            _cells.Add(new List<table_cell>());
            _rows.Add(new table_row(0, row));

        }
        public void add_cell(element el)
        {
            var cell = new table_cell
            {
                el = el,
                colspan = int.Parse(el.get_attr("colspan", "1")),
                rowspan = int.Parse(el.get_attr("rowspan", "1")),
                borders = el.get_borders,
            };
            while (is_rowspanned(_cells.Count - 1, _cells.Last().Count))
                _cells.Last().Add(new table_cell());
            _cells.Last().Add(cell);
            for (var i = 1; i < cell.colspan; i++)
                _cells.Last().Add(new table_cell());
        }

        public bool is_rowspanned(int r, int c)
        {
            for (var row = r - 1; row >= 0; row--)
                if (c < _cells[row].Count)
                    if (_cells[row][c].rowspan > 1)
                        if (_cells[row][c].rowspan >= r - row + 1)
                            return true;
            return false;
        }

        public void finish()
        {
            _rows_count = _cells.Count;
            _cols_count = 0;
            for (var i = 0; i < _cells.Count; i++)
                _cols_count = Math.Max(_cols_count, _cells[i].Count);
            for (var i = 0; i < _cells.Count; i++)
                for (var j = _cells[i].Count; j < _cols_count; j++)
                    _cells[i].Add(new table_cell());

            _columns.Clear();
            for (var i = 0; i < _cols_count; i++)
                _columns.Add(new table_column(0, 0));

            for (var col = 0; col < _cols_count; col++)
                for (var row = 0; row < _rows_count; row++)
                {
                    if (cell(col, row).el != null)
                    {
                        // find minimum left border width
                        _columns[col].border_left = _columns[col].border_left != 0
                            ? Math.Min(_columns[col].border_left, cell(col, row).borders.left)
                            : cell(col, row).borders.left;
                        // find minimum right border width
                        _columns[col].border_right = _columns[col].border_right != 0
                            ? Math.Min(_columns[col].border_right, cell(col, row).borders.right)
                            : cell(col, row).borders.right;
                        // find minimum top border width
                        _rows[row].border_top = _rows[row].border_top != 0
                            ? Math.Min(_rows[row].border_top, cell(col, row).borders.top)
                            : cell(col, row).borders.top;
                        // find minimum bottom border width
                        _rows[row].border_bottom = _rows[row].border_bottom != 0
                            ? Math.Min(_rows[row].border_bottom, cell(col, row).borders.bottom)
                            : cell(col, row).borders.bottom;
                    }

                    if (cell(col, row).el != null && cell(col, row).colspan <= 1)
                        if (!cell(col, row).el.get_css_width().is_predefined && _columns[col].css_width.is_predefined)
                            _columns[col].css_width = cell(col, row).el.get_css_width();
                }

            for (var col = 0; col < _cols_count; col++)
                for (var row = 0; row < _rows_count; row++)
                    if (cell(col, row).el != null)
                        cell(col, row).el.set_css_width(_columns[col].css_width);
        }

        public table_cell cell(int t_col, int t_row) => t_col >= 0 && t_col < _cols_count && t_row >= 0 && t_row < _rows_count ? _cells[t_row][t_col] : null;

        public table_column column(int c) => _columns[c];
        public table_row row(int r) => _rows[r];

        public int rows_count() => _rows_count;
        public int cols_count() => _cols_count;

        public void distribute_max_width(int width, int start, int end) => distribute_width(width, start, end, table_column_accessor_max_width.Default);
        public void distribute_min_width(int width, int start, int end) => distribute_width(width, start, end, table_column_accessor_min_width.Default);
        public void distribute_width(int width, int start, int end)
        {
            if (!(start >= 0 && start < _cols_count && end >= 0 && end < _cols_count))
                return;

            var distribute_columns = new List<table_column>();

            for (var step = 0; step < 3; step++)
            {
                distribute_columns.Clear();

                switch (step)
                {
                    case 0:
                        {
                            // distribute between the columns with width == auto
                            for (var col = start; col <= end; col++)
                                if (_columns[col].css_width.is_predefined)
                                    distribute_columns.Add(_columns[col]);
                        }
                        break;
                    case 1:
                        {
                            // distribute between the columns with percents
                            for (var col = start; col <= end; col++)
                                if (!_columns[col].css_width.is_predefined && _columns[col].css_width.units == css_units_percentage)
                                    distribute_columns.Add(_columns[col]);
                        }
                        break;
                    case 2:
                        {
                            // well distribute between all columns
                            for (var col = start; col <= end; col++)
                                distribute_columns.Add(_columns[col]);
                        }
                        break;
                }

                var added_width = 0;

                if (distribute_columns.Count != 0 || step == 2)
                {
                    var cols_width = 0;
                    foreach (var col in distribute_columns)
                        cols_width += col.max_width - col.min_width;

                    if (cols_width != 0)
                    {
                        var add = width / distribute_columns.Count;
                        foreach (var col in distribute_columns)
                        {
                            add = (int)Math.Round(width * ((col.max_width - col.min_width) / (float)cols_width));
                            if (col.width + add >= col.min_width)
                            {
                                col.width += add;
                                added_width += add;
                            }
                            else
                            {
                                added_width += (col.width - col.min_width) * (add / Math.Abs(add));
                                col.width = col.min_width;
                            }
                        }
                        if (added_width < width && step != 0)
                        {
                            distribute_columns.First().width += width - added_width;
                            added_width = width;
                        }
                    }
                    else
                    {
                        distribute_columns.Last().width += width;
                        added_width = width;
                    }
                }

                if (added_width == width)
                    break;
                else
                    width -= added_width;
            }
        }

        public void distribute_width(int width, int start, int end, table_column_accessor acc)
        {
            if (!(start >= 0 && start < _cols_count && end >= 0 && end < _cols_count))
                return;

            var cols_width = 0;
            for (var col = start; col <= end; col++)
                cols_width += _columns[col].max_width;

            var add = width / (end - start + 1);
            var added_width = 0;
            for (var col = start; col <= end; col++)
            {
                if (cols_width != 0)
                    add = (int)Math.Round(width * (_columns[col].max_width / (float)cols_width));
                added_width += add;
                acc.set(_columns[col], acc.get(_columns[col]) + add);
            }
            if (added_width < width)
                acc.set(_columns[start], acc.get(_columns[start]) + width - added_width);
        }

        public int calc_table_width(int block_width, bool is_auto, int min_table_width, int max_table_width)
        {
            min_table_width = 0; // MIN
            max_table_width = 0; // MAX

            var cur_width = 0;
            var max_w = 0;
            var min_w = 0;

            for (var col = 0; col < _cols_count; col++)
            {
                min_table_width += _columns[col].min_width;
                max_table_width += _columns[col].max_width;

                if (!_columns[col].css_width.is_predefined)
                {
                    _columns[col].width = _columns[col].css_width.calc_percent(block_width);
                    _columns[col].width = Math.Max(_columns[col].width, _columns[col].min_width);
                }
                else
                {
                    _columns[col].width = _columns[col].min_width;
                    max_w += _columns[col].max_width;
                    min_w += _columns[col].min_width;
                }

                cur_width += _columns[col].width;
            }

            if (cur_width == block_width)
                return cur_width;

            if (cur_width < block_width)
            {
                if (cur_width - min_w + max_w <= block_width)
                {
                    cur_width = 0;
                    for (var col = 0; col < _cols_count; col++)
                    {
                        if (_columns[col].css_width.is_predefined)
                            _columns[col].width = _columns[col].max_width;
                        cur_width += _columns[col].width;
                    }
                    if (cur_width == block_width || is_auto)
                        return cur_width;
                }
                distribute_width(block_width - cur_width, 0, _cols_count - 1);
                cur_width = 0;
                for (var col = 0; col < _cols_count; col++)
                    cur_width += _columns[col].width;
            }
            else
            {
                var fixed_width = 0;
                var percent = 0F;
                for (int col = 0; col < _cols_count; col++)
                {
                    if (!_columns[col].css_width.is_predefined && _columns[col].css_width.units == css_units.percentage)
                        percent += _columns[col].css_width.val;
                    else
                        fixed_width += _columns[col].width;
                }
                var scale = (float)(100.0 / percent);
                cur_width = 0;
                for (var col = 0; col < _cols_count; col++)
                {
                    if (!_columns[col].css_width.is_predefined && _columns[col].css_width.units == css_units.percentage)
                    {
                        var w = new css_length();
                        w.set_value(_columns[col].css_width.val * scale, css_units.percentage);
                        _columns[col].width = w.calc_percent(block_width - fixed_width);
                        if (_columns[col].width < _columns[col].min_width)
                            _columns[col].width = _columns[col].min_width;
                    }
                    cur_width += _columns[col].width;
                }
            }
            return cur_width;
        }

        public void calc_horizontal_positions(margins table_borders, border_collapse bc, int bdr_space_x)
        {
            if (bc == border_collapse.separate)
            {
                var left = bdr_space_x;
                for (var i = 0; i < _cols_count; i++)
                {
                    _columns[i].left = left;
                    _columns[i].right = _columns[i].left + _columns[i].width;
                    left = _columns[i].right + bdr_space_x;
                }
            }
            else
            {
                var left = 0;
                if (_cols_count != 0)
                    left -= Math.Min(table_borders.left, _columns[0].border_left);
                for (var i = 0; i < _cols_count; i++)
                {
                    if (i > 0)
                        left -= Math.Min(_columns[i - 1].border_right, _columns[i].border_left);

                    _columns[i].left = left;
                    _columns[i].right = _columns[i].left + _columns[i].width;
                    left = _columns[i].right;
                }
            }
        }

        public void calc_vertical_positions(margins table_borders, border_collapse bc, int bdr_space_y)
        {
            if (bc == border_collapse.separate)
            {
                var top = bdr_space_y;
                for (var i = 0; i < _rows_count; i++)
                {
                    _rows[i].top = top;
                    _rows[i].bottom = _rows[i].top + _rows[i].height;
                    top = _rows[i].bottom + bdr_space_y;
                }
            }
            else
            {
                var top = 0;
                if (_rows_count != 0)
                    top -= Math.Min(table_borders.top, _rows[0].border_top);
                for (var i = 0; i < _rows_count; i++)
                {
                    if (i > 0)
                        top -= Math.Min(_rows[i - 1].border_bottom, _rows[i].border_top);

                    _rows[i].top = top;
                    _rows[i].bottom = _rows[i].top + _rows[i].height;
                    top = _rows[i].bottom;
                }
            }
        }

        public void calc_rows_height(int blockHeight, int borderSpacingY)
        {
            var min_table_height = 0;

            // compute vertical size inferred by cells
            foreach (var row in _rows)
            {
                if (!row.css_height.is_predefined)
                    if (row.css_height.units != css_units.percentage)
                        if (row.height < (int)row.css_height.val)
                            row.height = (int)row.css_height.val;
                row.min_height = row.height;
                min_table_height += row.height;
            }

            //min_table_height += borderSpacingY * ((int)_rows.Count + 1);

            if (blockHeight > min_table_height)
            {
                var extra_height = blockHeight - min_table_height;
                var auto_count = 0; // number of rows with height=auto
                foreach (var row in _rows)
                {
                    if (!row.css_height.is_predefined && row.css_height.units == css_units.percentage)
                    {
                        row.height = row.css_height.calc_percent(blockHeight);
                        if (row.height < row.min_height)
                            row.height = row.min_height;

                        extra_height -= row.height - row.min_height;

                        if (extra_height <= 0) break;
                    }
                    else if (row.css_height.is_predefined)
                        auto_count++;
                }
                if (extra_height > 0)
                {
                    if (auto_count != 0)
                    {
                        // distribute height to the rows with height=auto
                        var extra_row_height = extra_height / auto_count;
                        foreach (var row in _rows)
                            if (row.css_height.is_predefined)
                                row.height += extra_row_height;
                    }
                    else
                    {
                        // We don't have rows with height=auto, so distribute height to all rows
                        if (_rows.Count != 0)
                        {
                            var extra_row_height = extra_height / _rows.Count;
                            foreach (var row in _rows)
                                row.height += extra_row_height;
                        }
                    }
                }
                else if (extra_height < 0)
                {
                    extra_height = -extra_height;
                    foreach (var row in _rows)
                    {
                        if (extra_height <= 0) break;
                        if (row.height > row.min_height)
                        {
                            if (row.height - extra_height >= row.min_height)
                            {
                                row.height -= extra_height;
                                extra_height = 0;
                            }
                            else
                            {
                                extra_height -= row.height - row.min_height;
                                row.height = row.min_height;
                            }
                        }
                    }
                }
            }
        }
    }
}
