// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StarStocks.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// return the last n character fron a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tailLength"></param>
        /// <returns></returns>
        public static string GetLast(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;

            return source.Substring(source.Length - tailLength);
        }

        /// <summary>
        /// return first maxLength characters to display 
        /// from https://stackoverflow.com/questions/3566830/what-method-in-the-string-class-returns-only-the-first-n-characters
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string TruncateLongString(this string str, int maxLength)
        {
            return str?[0..Math.Min(str.Length, maxLength)];
        }

        /// <summary>
        /// sql "Like" comparison in linq to objects
        /// From http://stackoverflow.com/questions/5663655/like-operator-in-linq-to-objects
        /// usage : string pattern = ".*ine.*e"; var res = from i in list where i.Like(pattern)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool Like(this string s, string pattern)
        {
            //Find the pattern anywhere in the string
            pattern = ".*" + pattern + ".*";

            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }
    }
}
