using System.Collections.Generic;

namespace H3ml.Layout
{
    public struct media_query_expression
    {
        public media_feature feature;
        public int val;
        public int val2;
        public bool check_as_bool;

        public media_query_expression()
        {
            check_as_bool = false;
            feature = media_feature_none;
            val = 0;
            val2 = 0;
        }

        public bool check(media_features features)
        {
            switch (feature)
            {
                case media_feature_width:
                    if (check_as_bool) return features.width != 0;
                    else if (features.width == val) return true;
                    break;
                case media_feature_min_width:
                    if (features.width >= val) return true;
                    break;
                case media_feature_max_width:
                    if (features.width <= val) return true;
                    break;
                case media_feature_height:
                    if (check_as_bool) return (features.height != 0);
                    else if (features.height == val) return true;
                    break;
                case media_feature_min_height:
                    if (features.height >= val) return true;
                    break;
                case media_feature_max_height:
                    if (features.height <= val) return true;
                    break;

                case media_feature_device_width:
                    if (check_as_bool) return (features.device_width != 0);
                    else if (features.device_width == val) return true;
                    break;
                case media_feature_min_device_width:
                    if (features.device_width >= val) return true;
                    break;
                case media_feature_max_device_width:
                    if (features.device_width <= val) return true;
                    break;
                case media_feature_device_height:
                    if (check_as_bool) return (features.device_height != 0);
                    else if (features.device_height == val) return true;
                    break;
                case media_feature_min_device_height:
                    if (features.device_height <= val) return true;
                    break;
                case media_feature_max_device_height:
                    if (features.device_height <= val) return true;
                    break;

                case media_feature_orientation:
                    if (features.height >= features.width)
                    {
                        if (val == media_orientation_portrait) return true;
                    }
                    else
                    {
                        if (val == media_orientation_landscape) return true;
                    }
                    break;
                case media_feature_aspect_ratio:
                    if (features.height && val2)
                    {
                        var ratio_this = (int)Math.Round((double)val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round((double)features.width / (double)features.height * 100.0);
                        if (ratio_this == ratio_feat) return true;
                    }
                    break;
                case media_feature_min_aspect_ratio:
                    if (features.height && val2)
                    {
                        var ratio_this = (int)Math.Round((double)val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round((double)features.width / (double)features.height * 100.0);
                        if (ratio_feat >= ratio_this) return true;
                    }
                    break;
                case media_feature_max_aspect_ratio:
                    if (features.height && val2)
                    {
                        var ratio_this = (int)Math.Round((double)val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round((double)features.width / (double)features.height * 100.0);
                        if (ratio_feat <= ratio_this) return true;
                    }
                    break;

                case media_feature_device_aspect_ratio:
                    if (features.device_height && val2)
                    {
                        var ratio_this = (int)Math.Round((double)val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round((double)features.device_width / (double)features.device_height * 100.0);
                        if (ratio_feat == ratio_this) return true;
                    }
                    break;
                case media_feature_min_device_aspect_ratio:
                    if (features.device_height && val2)
                    {
                        var ratio_this = (int)Math.Round((double)val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round((double)features.device_width / (double)features.device_height * 100.0);
                        if (ratio_feat >= ratio_this) return true;
                    }
                    break;
                case media_feature_max_device_aspect_ratio:
                    if (features.device_height && val2)
                    {
                        var ratio_this = (int)Math.Round((double)val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round((double)features.device_width / (double)features.device_height * 100.0);
                        if (ratio_feat <= ratio_this) return true;
                    }
                    break;

                case media_feature_color:
                    if (check_as_bool) return features.color != 0;
                    else if (features.color == val) return true;
                    break;
                case media_feature_min_color:
                    if (features.color >= val) return true;
                    break;
                case media_feature_max_color:
                    if (features.color <= val) return true;
                    break;

                case media_feature_color_index:
                    if (check_as_bool) return features.color_index != 0;
                    else if (features.color_index == val) return true;
                    break;
                case media_feature_min_color_index:
                    if (features.color_index >= val) return true;
                    break;
                case media_feature_max_color_index:
                    if (features.color_index <= val) return true;
                    break;

                case media_feature_monochrome:
                    if (check_as_bool) return features.monochrome != 0;
                    else if (features.monochrome == val) return true;
                    break;
                case media_feature_min_monochrome:
                    if (features.monochrome >= val) return true;
                    break;
                case media_feature_max_monochrome:
                    if (features.monochrome <= val) return true;
                    break;

                case media_feature_resolution:
                    if (features.resolution == val) return true;
                    break;
                case media_feature_min_resolution:
                    if (features.resolution >= val) return true;
                    break;
                case media_feature_max_resolution:
                    if (features.resolution <= val) return true;
                    break;
                default: return false;
            }
            return false;
        }
    }

    public class media_query
    {
        IList<media_query_expression> _expressions;
        bool _not;
        media_type _media_type;

        public media_query()
        {
            _media_type = media_type_all;
            _not = false;
        }

        public media_query(media_query val)
        {
            _not = val._not;
            _expressions = val._expressions;
            _media_type = val._media_type;
        }

        public static media_query create_from_string(string str, document doc)
        {
            var query = new media_query();

            html.split_string(str, out var tokens, " \t\r\n", "", "(");

            foreach (var tok in tokens)
            {
                if (tok == "not")
                    query._not = true;
                else if (tok.at(0) == '(')
                {
                    tok.erase(0, 1);
                    if (tok.at(tok.length - 1) == ')')
                        tok.erase(tok.length - 1, 1);
                    media_query_expression expr;
                    html.split_string(tok, out var expr_tokens, ":");
                    if (!expr_tokens.empty())
                    {
                        trim(expr_tokens[0]);
                        expr.feature = (media_feature)value_index(expr_tokens[0], media_feature_strings, media_feature_none);
                        if (expr.feature != media_feature_none)
                        {
                            if (expr_tokens.size() == 1)
                                expr.check_as_bool = true;
                            else
                            {
                                trim(expr_tokens[1]);
                                expr.check_as_bool = false;
                                if (expr.feature == media_feature_orientation)
                                    expr.val = value_index(expr_tokens[1], media_orientation_strings, media_orientation_landscape);
                                else
                                {
                                    var slash_pos = expr_tokens[1].find('/');
                                    if (slash_pos != -1)
                                    {
                                        var val1 = expr_tokens[1].substr(0, slash_pos);
                                        var val2 = expr_tokens[1].substr(slash_pos + 1);
                                        trim(val1);
                                        trim(val2);
                                        expr.val = int.Parse(val1);
                                        expr.val2 = int.Parse(val2);
                                    }
                                    else
                                    {
                                        css_length length;
                                        length.fromString(expr_tokens[1]);
                                        if (length.units == css_units_dpcm)
                                            expr.val = (int)(length.val * 2.54);
                                        else if (length.units == css_units_dpi)
                                            expr.val = (int)(length.val * 2.54);
                                        else
                                        {
                                            if (doc != null)
                                                doc.cvt_units(length, doc.container().get_default_font_size());
                                            expr.val = (int)length.val;
                                        }
                                    }
                                }
                            }
                            query._expressions.Add(expr);
                        }
                    }
                }
                else
                    query._media_type = (media_type)value_index(tok, media_type_strings, media_type_all);
            }
            return query;
        }

        public bool check(media_features features)
        {
            var res = false;
            if (_media_type == media_type_all || _media_type == features.type)
            {
                res = true;
                foreach (var expr in _expressions)
                    if (!expr.check(features))
                    {
                        res = false;
                        break;
                    }
            }
            if (_not)
                res = !res;
            return res;
        }
    }

    public class media_query_list
    {
        IList<media_query> _queries;
        bool _is_used;

        public media_query_list()
        {
            _is_used = false;
        }
        public media_query_list(media_query_list val)
        {
            _is_used = val._is_used;
            _queries = val._queries;
        }

        public static media_query_list create_from_string(string str, document doc)
        {
            var list = new media_query_list();
            var tokens = new List<string>();
            html.split_string(str, out tokens, ",");
            foreach (var tok in tokens)
            {
                trim(tok);
                lcase(tok);
                var query = media_query.create_from_string(tok, doc);
                if (query != null)
                    list._queries.Add(query);
            }
            return list._queries.Count != 0 ? list : null;
        }
        public bool is_used => _is_used;
        public bool apply_media_features(media_features features)   // returns true if the _is_used changed
        {
            var apply = false;
            foreach (var iter in _queries)
                if (iter.check(features))
                {
                    apply = true;
                    break;
                }
            var ret = apply != _is_used;
            _is_used = apply;
            return ret;
        }
    }
}
