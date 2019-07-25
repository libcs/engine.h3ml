namespace H3ml.Layout
{
    public class css_length
    {
        float _value;
        int _predef;
        css_units _units;
        bool _is_predefined;

        public css_length()
        {
            _value = 0;
            _predef = 0;
            _units = css_units_none;
            _is_predefined = false;
        }
        public css_length(css_length val)
        {
            if (val.is_predefined) _predef = val._predef;
            else _value = val._value;
            _units = val._units;
            _is_predefined = val._is_predefined;
        }

        //public css_length operator=(css_length val)
        //{
        //    if (val.is_predefined) _predef = val._predef;
        //    else _value = val._value;
        //    _units = val.m_units;
        //    _is_predefined = val._is_predefined;
        //    return this;
        //}

        //public css_length operator=(float val)
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
        public int calc_percent(int width) => !is_predefined ? units == css_units_percentage ? (int)((double)width * (double)_value / 100.0) : (int)val : 0;

        public void fromString(string str, string predefs = "", int defValue = 0)
        {
            // TODO: Make support for calc
            if (str.StartsWith("calc"))
            {
                _is_predefined = true;
                _predef = 0;
                return;
            }

            var predef = value_index(str, predefs, -1);
            if (predef >= 0)
            {
                _is_predefined = true;
                _predef = predef;
            }
            else
            {
                _is_predefined = false;

                var num = new List<char>();
                var un = new List<char>();
                var is_unit = false;
                foreach (var chr in str)
                {
                    if (!is_unit)
                    {
                        if (t_isdigit( chr) || chr == '.' || chr == '+' || chr == '-') num.Add(chr);
                        else is_unit = true;
                    }
                    if (is_unit)
                        un.Add(chr);
                }
                if (!num.empty())
                {
                    _value = (float)t_strtod(num.ToString(), 0);
                    _units = (css_units)value_index(un.ToString(), css_units_strings, css_units_none);
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
