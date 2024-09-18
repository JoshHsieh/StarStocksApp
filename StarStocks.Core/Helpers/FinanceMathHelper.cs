// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace StarStocks.Core.Helpers
{
    public class FinanceMathHelper
    {
        /// <summary>
        /// 標準常態分布的密度(gsussian)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double n(double x)
        {
            double aa = 1.0 / Math.Sqrt(2.0 * 3.1415);

            return aa * Math.Exp(-x * x * 0.5);
        }

        /// <summary>
        /// 標準正態分布的累計分布
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double N(double x)
        {
            double a1 = 0.4361836;
            double a2 = -0.1201676;
            double a3 = 0.9372980;

            double k = 1.0 / (1.0 + (0.33267 * x));

            if (x >= 0.0)
            {
                return 1.0 - n(x) * (a1 * k + (a2 * k * k) + (a3 * k * k * k));
            }
            else
            {
                return 1.0 - N(-x);
            }
        }
    }
}
