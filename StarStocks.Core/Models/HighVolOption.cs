// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace StarStocks.Core.Models
{
    [Table("option_highvol", Schema = "public")]
    public class HighVolOption : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("strike")]
        public double Strike { get; set; }

        [Column("expire_date")]
        public System.DateTime? ExpireDate { get; set; }

        [Column("volume")]
        public double Volume { get; set; }

        [Column("bid_ask")]
        public double BidAsk { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion
    }
}
