// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace StarStocks.Core.Models
{
    [Table("darkpool_dashboard", Schema = "public")]
    public class DarkpoolDashboard : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("sentiment")]
        public string Sentiment { get; set; }

        [Column("total_amt")]
        public double TotalAmt { get; set; }

        [Column("daily_amt_change")]
        public double DailyAmtChange { get; set; }

        [Column("at_ask")]
        public double AtAsk { get; set; }

        [Column("at_bid")]
        public double AtBid { get; set; }

        [Column("net_value")]
        public double NetValue { get; set; }

        [Column("dp_trades")]
        public int DpTrades { get; set; }

        [Column("block_trades")]
        public int BlockTrades { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion
    }
}
