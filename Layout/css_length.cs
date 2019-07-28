using System;
using System.Globalization;

namespace H3ml.Layout
{
    public class css_length
    {
        float _value;
        int _predef;
        css_units _units;
        bool _is_predefined;

        public css_length(Exception o = null)
        {
            _value = 0;
            _predef = 0;
            _units = css_units.none;
            _is_predefined = false;
        }
        public css_length(css_length val)
        {
            if (val.is_predefined) _predef = val._predef;
            else _value = val._value;
            _units = val._units;
            _is_predefined = val._is_predefined;
        }
        //public css_length assignTo(float val)
        //{
        //    _value = val;
        //    _units = css_units_px;
        //    _is_predefined = false;
        //    return this;
        //}

        public bool is_predefined => _is_predefined;

        public int predef
        {
            get => _is_predefined ? _predef : 0;
            set
            {
                _predef = value;
                _is_predefined = true;
            }
        }

        public void set_value(float val, css_units units)
        {
            _value = val;
            _is_predefined = false;
            _units = units;
        }
        public float val => !_is_predefined ? _value : 0;
        public css_units units => _units;
        public int calc_percent(int width) => !is_predefined ? units == css_units.percentage ? (int)(width * (double)_value / 100.0) : (int)val : 0;

        public void fromString(string str, string predefs = "", int defValue = 0)
        {
            // TODO: Make support for calc
            if (str.StartsWith("calc"))
            {
                _is_predefined = true;
                _predef = 0;
                return;
            }

            var predef = html.value_index(str, predefs, -1);
            if (predef >= 0)
            {
                _is_predefined = true;
                _predef = predef;
            }
            else
            {
                _is_predefined = false;
                var i = 0; for (var chr = '\0'; i < str.Length && (chr = str[i]) != 0 && (char.IsDigit(chr) || chr == '.' || chr == '+' || chr == '-'); i++) { }
                var num = str.Substring(0, i);
                var un = str.Substring(i);
                if (num.Length != 0)
                {
                    _value = float.Parse(num, CultureInfo.InvariantCulture);
                    _units = (css_units)html.value_index(un, types.css_units_strings, (int)css_units.none);
                }
                else
                {
                    // not a number so it is predefined
                    _is_predefined = true;
                    _predef = defValue;
                }
            }
        }

    }
}
