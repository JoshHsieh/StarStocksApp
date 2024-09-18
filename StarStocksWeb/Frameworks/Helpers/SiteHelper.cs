// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StarStocksWeb.Models;

namespace StarStocksWeb.Frameworks.Helpers
{
    public class SiteHelper
    {
        public static bool SaveFileByType(IFormFile f, string filePath, string userId = "")
        {
            bool isFileSave = true;

            try
            {
                string dirPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    f.CopyTo(fileStream);
                }
            }
            catch (Exception)
            {

                isFileSave = false;
            }

            return isFileSave;
        }

        /// <summary>
        /// 指定起始時間，拆開成 5, 10, 15 .... 分鐘間隔
        /// https://stackoverflow.com/questions/42910213/split-the-timerange-to-list-based-on-day
        /// https://stackoverflow.com/questions/11605813/c-sharp-is-there-a-way-to-make-a-list-of-time-range-configurable
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static IEnumerable<DateRange> SplitInToDatetimes(DateRange range, int minutes)
        {
            var ranges = new List<DateRange>();

            var tempRange = new DateRange() { Start = range.Start, End = range.End };

            for (DateTime subRangeStart = tempRange.Start; subRangeStart < tempRange.End; subRangeStart = subRangeStart.AddMinutes(minutes))
            {
                var dateRange = new DateRange()
                {
                    Start = tempRange.Start,
                    End = tempRange.Start.AddMinutes(minutes)
                };

                ranges.Add(dateRange);
                tempRange.Start = dateRange.End;
            }

            //while (tempRange.Start.TimeOfDay != tempRange.End.TimeOfDay)
            //{
            //    var dateRange = new DateRange()
            //    {
            //        Start = tempRange.Start,
            //        End = tempRange.Start.AddMinutes(minutes)
            //    };
            //    ranges.Add(dateRange);
            //    tempRange.Start = dateRange.End;
            //}

            ranges.Add(tempRange);

            return ranges;
        }
    }

}
