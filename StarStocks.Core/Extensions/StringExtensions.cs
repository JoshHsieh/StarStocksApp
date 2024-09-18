// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace StarStocks.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GetLast(this string source, int tailLength)
        {
            if (tailLength >= source.Length)
                return source;

            return source.Substring(source.Length - tailLength);
        }
    }
}
