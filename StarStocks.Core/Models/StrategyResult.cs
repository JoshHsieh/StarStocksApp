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
    [Table("strategy_result", Schema = "public")]
    public class StrategyResult : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("buy_price")]
        public double BuyPrice { get; set; }

        [Column("buy_time")]
        public System.DateTime? BuyTime { get; set; }

        [Column("sell_price")]
        public double SellPrice { get; set; }

        [Column("sell_time")]
        public System.DateTime? SellTime { get; set; }

        [Column("net_balance")]
        public double NetBalance { get; set; }

        #endregion
    }
}
