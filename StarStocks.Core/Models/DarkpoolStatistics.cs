// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Dapper;

namespace StarStocks.Core.Models
{
    [Table("darkpool_cross_average", Schema = "public")]
    public class DarkpoolValueCrossAvg : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("date_value")]
        public double DateValue { get; set; }

        [Column("net_value")]
        public double NetValue { get; set; }

        [Column("avg_unit")]
        public int AvgUnit { get; set; }

        [Column("avg_value")]
        public double AvgValue { get; set; }

        [Column("avg_net_value")]
        public double AvgNetValue { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion
    }
}
