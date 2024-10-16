// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StarStocks.Core.Extensions
{
    public static class ConvertExtensions
    {
        public static double ConvertStrAbbrToNumeric(this string strAbbr)
        {
            //get last char of string array
            string lastAbbr = strAbbr.Substring(strAbbr.Length - 1);

            double result = 0;

            string formatStr = strAbbr.Remove(strAbbr.Length - 1);

            if (lastAbbr.ToUpper() == "M")
            {
                result = Convert.ToDouble(formatStr) * 1000000;
            }
            else if (lastAbbr.ToUpper() == "K")
            {
                result = Convert.ToDouble(formatStr) * 1000;
            }
            else if (lastAbbr.ToUpper() == "B")
            {
                result = Convert.ToDouble(formatStr) * 1000000000;
            }
            else if (lastAbbr.ToUpper() == "0")
            {
                result = Convert.ToDouble(strAbbr);
            }
            else
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// bool to int value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int BoolToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// JObject to nested dictionaries
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this JObject jobject)
        {
            var dico = new Dictionary<string, object>();

            foreach (var o in jobject)
            {
                var oValue = o.Value;

                var jArray = o.Value as JArray;
                if (jArray != null)
                {
                    var first = jArray[0]; // use first element to guess if we are dealing with a list of dico or an array
                    var isValueArray = first is JValue;

                    if (isValueArray)
                    {
                        var array = jArray.Values().Select(x => ((JValue)x).Value).ToArray();
                        dico[o.Key] = array;
                    }
                    else
                    {
                        var list = new List<IDictionary<string, object>>();
                        foreach (var token in jArray)
                        {
                            var elt = ((JObject)token).ToDictionary();
                            list.Add(elt);
                        }

                        dico[o.Key] = list;
                    }
                }
                else
                {
                    dico[o.Key] = ((JValue)oValue).Value;
                }
            }

            return dico;
        }

        /// <summary>
        /// convert list to string with quote & comma
        /// usage : string xxx = string.Format("select * from xxx where id in ({0});", list.ToSplitString(null));
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="cast"></param>
        /// <returns></returns>
        public static string ToSplitString<TSource>(this IEnumerable<TSource> source, Func<TSource, string> cast)
        {
            const string Empty = "''";

            if (source == null)
                return Empty;

            var sb = new StringBuilder();

            foreach (var entity in source)
            {
                if (cast == null)
                    sb.Append(",'").Append(entity.ToSqlQuoteString()).Append("'");
                else
                    sb.Append(",'").Append(cast(entity).ToSqlQuoteString()).Append("'");
            }

            if (sb.Length == 0)
                return Empty;

            return sb.ToString().Substring(1);
        }

        /// <summary>
        /// convert object to string with quote mark
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToSqlQuoteString(this object obj)
        {
            string str = obj.ToStringNotNull();
            return str.Replace("'", "''");
        }

        /// <summary>
        /// convert object to string, if object is null return empty string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringNotNull(this object obj)
        {
            if (obj == null) return string.Empty;
            if (obj == DBNull.Value) return string.Empty;
            return obj.ToString().Trim();
        }
    }
}
