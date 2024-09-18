// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace StarStocks.Core.Models
{
    /// <summary>
    /// https://help.streetsmart.schwab.com/edge/1.10/Content/Options%20Chain%20Column%20Descriptions.htm
    /// </summary>
    [Table("option_chain_daily", Schema = "public")]
    public class OptionChainDaily : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("strike")]
        public double Strike { get; set; }

        [Column("underlying_price")]
        public double UnderlyingPrice { get; set; }

        [Column("underlying_iv")]
        public double UnderlyingIv { get; set; }

        [Column("option_symbol")]
        public string OptionSymbol { get; set; }

        [Column("strategy")]
        public string Strategy { get; set; }

        [Column("oi")]
        public int OpenInterest { get; set; }

        [Column("expire_date")]
        public System.DateTime? ExpireDate { get; set; }

        /// <summary>
        /// Days To Expiration
        /// </summary>
        [Column("dte")]
        public int Dte { get; set; }

        [Column("last_price")]
        public double LastPrice { get; set; }

        [Column("last_size")]
        public int LastSize { get; set; }

        /// <summary>
        /// bid  與 ask 的平均，也稱市場價
        /// </summary>
        [Column("mark_price")]
        public double MarkPrice { get; set; }

        /// <summary>
        /// ask price
        /// </summary>
        [Column("ask_price")]
        public double AskPrice { get; set; }

        [Column("ask_size")]
        public int AskSize { get; set; }

        /// <summary>
        /// bid price
        /// </summary>
        [Column("bid_price")]
        public double BidPrice { get; set; }

        [Column("bid_size")]
        public int BidSize { get; set; }

        [Column("total_vol")]
        public long TotalVolume { get; set; }

        [Column("net_change")]
        public double NetChange { get; set; }

        [Column("iv")]
        public double Iv { get; set; }

        [Column("delta")]
        public double Delta { get; set; }

        [Column("gamma")]
        public double Gamma { get; set; }

        [Column("vega")]
        public double Vega { get; set; }

        [Column("theta")]
        public double Theta { get; set; }

        [Column("rho")]
        public double Rho { get; set; }

        [Column("time_value")]
        public double TimeValue { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion
    }

    public class SpotGamma
    {
        public double SpotPrice { get; set; }

        public double CallGamma { get; set; }

        public double PutGamma { get; set; }

        public double TotalGamma { get; set; }

        public List<StrikeGamma> StrikeGammaList { get; set; }
    }

    public class StrikeGamma
    {
        public double Strike { get; set; }

        public double CallGamma { get; set; }

        public double PutGamma { get; set; }

        public double TotalGamma { get; set; }
    }
}
