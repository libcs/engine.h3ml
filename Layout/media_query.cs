using System;
using System.Collections.Generic;

namespace H3ml.Layout
{
    public class media_query_expression
    {
        public media_feature feature;
        public int val;
        public int val2;
        public bool check_as_bool;

        public media_query_expression()
        {
            check_as_bool = false;
            feature = media_feature.none;
            val = 0;
            val2 = 0;
        }

        public bool check(media_features features)
        {
            switch (feature)
            {
                case media_feature.width:
                    if (check_as_bool) return features.width != 0;
                    else if (features.width == val) return true;
                    break;
                case media_feature.min_width:
                    if (features.width >= val) return true;
                    break;
                case media_feature.max_width:
                    if (features.width <= val) return true;
                    break;
                case media_feature.height:
                    if (check_as_bool) return (features.height != 0);
                    else if (features.height == val) return true;
                    break;
                case media_feature.min_height:
                    if (features.height >= val) return true;
                    break;
                case media_feature.max_height:
                    if (features.height <= val) return true;
                    break;

                case media_feature.device_width:
                    if (check_as_bool) return (features.device_width != 0);
                    else if (features.device_width == val) return true;
                    break;
                case media_feature.min_device_width:
                    if (features.device_width >= val) return true;
                    break;
                case media_feature.max_device_width:
                    if (features.device_width <= val) return true;
                    break;
                case media_feature.device_height:
                    if (check_as_bool) return (features.device_height != 0);
                    else if (features.device_height == val) return true;
                    break;
                case media_feature.min_device_height:
                    if (features.device_height <= val) return true;
                    break;
                case media_feature.max_device_height:
                    if (features.device_height <= val) return true;
                    break;

                case media_feature.orientation:
                    if (features.height >= features.width)
                    {
                        if ((media_orientation)val == media_orientation.portrait) return true;
                    }
                    else
                    {
                        if ((media_orientation)val == media_orientation.landscape) return true;
                    }
                    break;
                case media_feature.aspect_ratio:
                    if (features.height != 0 && val2 != 0)
                    {
                        var ratio_this = (int)Math.Round(val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round(features.width / (double)features.height * 100.0);
                        if (ratio_this == ratio_feat) return true;
                    }
                    break;
                case media_feature.min_aspect_ratio:
                    if (features.height != 0 && val2 != 0)
                    {
                        var ratio_this = (int)Math.Round(val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round(features.width / (double)features.height * 100.0);
                        if (ratio_feat >= ratio_this) return true;
                    }
                    break;
                case media_feature.max_aspect_ratio:
                    if (features.height != 0 && val2 != 0)
                    {
                        var ratio_this = (int)Math.Round(val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round(features.width / (double)features.height * 100.0);
                        if (ratio_feat <= ratio_this) return true;
                    }
                    break;

                case media_feature.device_aspect_ratio:
                    if (features.device_height != 0 && val2 != 0)
                    {
                        var ratio_this = (int)Math.Round(val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round(features.device_width / (double)features.device_height * 100.0);
                        if (ratio_feat == ratio_this) return true;
                    }
                    break;
                case media_feature.min_device_aspect_ratio:
                    if (features.device_height != 0 && val2 != 0)
                    {
                        var ratio_this = (int)Math.Round(val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round(features.device_width / (double)features.device_height * 100.0);
                        if (ratio_feat >= ratio_this) return true;
                    }
                    break;
                case media_feature.max_device_aspect_ratio:
                    if (features.device_height != 0 && val2 != 0)
                    {
                        var ratio_this = (int)Math.Round(val / (double)val2 * 100);
                        var ratio_feat = (int)Math.Round(features.device_width / (double)features.device_height * 100.0);
                        if (ratio_feat <= ratio_this) return true;
                    }
                    break;

                case media_feature.color:
                    if (check_as_bool) return features.color != 0;
                    else if (features.color == val) return true;
                    break;
                case media_feature.min_color:
                    if (features.color >= val) return true;
                    break;
                case media_feature.max_color:
                    if (features.color <= val) return true;
                    break;

                case media_feature.color_index:
                    if (check_as_bool) return features.color_index != 0;
                    else if (features.color_index == val) return true;
                    break;
                case media_feature.min_color_index:
                    if (features.color_index >= val) return true;
                    break;
                case media_feature.max_color_index:
                    if (features.color_index <= val) return true;
                    break;

                case media_feature.monochrome:
                    if (check_as_bool) return features.monochrome != 0;
                    else if (features.monochrome == val) return true;
                    break;
                case media_feature.min_monochrome:
                    if (features.monochrome >= val) return true;
                    break;
                case media_feature.max_monochrome:
                    if (features.monochrome <= val) return true;
                    break;

                case media_feature.resolution:
                    if (features.resolution == val) return true;
                    break;
                case media_feature.min_resolution:
                    if (features.resolution >= val) return true;
                    break;
                case media_feature.max_resolution:
                    if (features.resolution <= val) return true;
                    break;
                default: return false;
            }
            return false;
        }
    }

    public class media_query
    {
        List<media_query_expression> _expressions;
        bool _not;
        media_type _media_type;

        public media_query()
        {
            _expressions = new List<media_query_expression>();
            _media_type = media_type.all;
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
            var tokens = new List<string>();
            html.split_string(str, tokens, " \t\r\n", "", "(");
            for (var i = 0; i < tokens.Count; i++)
            {
                var tok = tokens[i];
                if (tok == "not")
                    query._not = true;
                else if (tok[0] == '(')
                {
                    tok = tok.Substring(1);
                    if (tok[tok.Length - 1] == ')')
                        tok = tok.Remove(tok.Length - 1, 1);
                    var expr = new media_query_expression();
                    var expr_tokens = new List<string>();
                    html.split_string(tok, expr_tokens, ":");
                    if (expr_tokens.Count != 0)
                    {
                        expr_tokens[0] = expr_tokens[0].Trim();
                        expr.feature = (media_feature)html.value_index(expr_tokens[0], types.media_feature_strings, (int)media_feature.none);
                        if (expr.feature != media_feature.none)
                        {
                            if (expr_tokens.Count == 1)
                                expr.check_as_bool = true;
                            else
                            {
                                expr_tokens[1] = expr_tokens[1].Trim();
                                expr.check_as_bool = false;
                                if (expr.feature == media_feature.orientation)
                                    expr.val = html.value_index(expr_tokens[1], types.media_orientation_strings, (int)media_orientation.landscape);
                                else
                                {
                                    var slash_pos = expr_tokens[1].IndexOf('/');
                                    if (slash_pos != -1)
                                    {
                                        var val1 = expr_tokens[1].Substring(0, slash_pos).Trim();
                                        var val2 = expr_tokens[1].Substring(slash_pos + 1).Trim();
                                        expr.val = int.Parse(val1);
                                        expr.val2 = int.Parse(val2);
                                    }
                                    else
                                    {
                                        var length = new css_length();
                                        length.fromString(expr_tokens[1]);
                                        if (length.units == css_units.dpcm) expr.val = (int)(length.val * 2.54);
                                        else if (length.units == css_units.dpi) expr.val = (int)(length.val * 2.54);
                                        else
                                        {
                                            if (doc != null)
                                                doc.cvt_units(length, doc.container.get_default_font_size());
                                            expr.val = (int)length.val;
                                        }
                                    }
                                }
                            }
                            query._expressions.Add(expr);
                        }
                    }
                }
                else query._media_type = (media_type)html.value_index(tok, types.media_type_strings, (int)media_type.all);
            }
            return query;
        }

        public bool check(media_features features)
        {
            var res = false;
            if (_media_type == media_type.all || _media_type == features.type)
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
            html.split_string(str, tokens, ",");
            for (var i = 0; i < tokens.Count; i++)
            {
                var tok = tokens[i].Trim().ToLowerInvariant();
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
