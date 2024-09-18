// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;

namespace StarStocks.Core.Models
{
    [Dapper.Table("tw_stock_quote", Schema = "public")]
    public class StockQuoteTw : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        /// <summary>
        /// bid price
        /// </summary>
        [Column("bid")]
        public double BidPrice { get; set; }

        /// <summary>
        /// ask price
        /// </summary>
        [Column("ask")]
        public double AskPrice { get; set; }

        [Column("last")]
        public double LastPrice { get; set; }

        [Column("net_change")]
        public double NetChange { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("cumulative_volume")]
        public int CumulativeVol { get; set; }

        [Column("cumulative_ask")]
        public int CumulativeAsk { get; set; }

        [Column("cumulative_bid")]
        public int CumulativeBid { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion

        //[IgnoreInsert]
        [Editable(false)]
        public string Span { get; set; }
    }

    public class DailyQuoteTwDashboard : BaseModel
    {
        public string Ticker { get; set; }

        public string TransDate { get; set; }

        /// <summary>
        /// 震幅((最高 - 最低) / 昨收)
        /// </summary>
        public double Vibration { get; set; }

        /// <summary>
        /// 漲跌
        /// </summary>
        public double NetChange { get; set; }

        public int TotalVolume { get; set; }

        /// <summary>
        /// 第一盤成交量
        /// </summary>
        public int FirstVolume { get; set; }

        /// <summary>
        /// 平均每筆成交量 (成交量 / 成交筆數 (扣除頭尾試搓))
        /// </summary>
        public double AvgSize { get; set; }

        /// <summary>
        /// 大單比例 (多少張算是大單可以設定)
        /// </summary>
        public double BigSizeRatio { get; set; }

        public string EventMessage { get; set; }
    }

    public class QuoteTwReport : BaseModel
    {
        public string Ticker { get; set; }

        public List<StockQuoteTw> AllQuotes { get; set; }

        public List<StockQuoteTw> Last5DaysAllQuotes { get; set; }

        public List<StockQuoteTw> ExcludeFirstLastQuotes { get; set; }

        public List<TickerSettings> TickerSettings { get; set; }
    }

}
