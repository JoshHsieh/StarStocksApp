// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using StarStocks.Core.Interfaces;

namespace StarStocks.Core.Models
{
    [Table("option_dashboard", Schema = "public")]
    public class OptionDashboard : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("proportion")]
        public double Proportion { get; set; }

        [Column("orders")]
        public int Orders { get; set; }

        [Column("buys")]
        public double Buys { get; set; }

        [Column("at_ask")]
        public double AtAsk { get; set; }

        [Column("at_bid")]
        public double AtBid { get; set; }

        [Column("net_prems")]
        public double NetPrems { get; set; }

        /// <summary>
        /// raw data 單位是 %
        /// </summary>
        [Column("prems_change")]
        public double PremiumsChange { get; set; }

        [Column("avg_expire_date")]
        public double AvgExpiredDate { get; set; }

        [Column("otm")]
        public double Otm { get; set; }

        [Column("otm_score")]
        public double OtmScore { get; set; }

        [Column("unusual")]
        public double Unusual { get; set; }

        [Column("spot_change")]
        public double SpotChange { get; set; }

        [Column("iv")]
        public double Iv { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion
    }
}
