// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;


namespace StarStocks.Core.Models
{
    [Table("ticker_bar_series", Schema = "public")]
    public class TickerBarSeries : BaseModel
    {
        #region Implement Property
        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("open")]
        public double Open { get; set; }

        [Column("high")]
        public double High { get; set; }

        [Column("low")]
        public double Low { get; set; }

        [Column("close")]
        public double Close { get; set; }

        [Column("volume")]
        public int Volume { get; set; }

        [Column("k_timestamp")]
        public DateTime? KTime { get; set; }
        #endregion
    }
}
