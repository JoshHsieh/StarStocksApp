// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
