// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;


namespace StarStocks.Core.Models
{
    [Table("darkpool_trans", Schema = "public")]
    public class DarkpoolTrans : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("proportion")]
        public double Proportion { get; set; }

        [Column("size")]
        public double Size { get; set; }

        [Column("value")]
        public double Value { get; set; }

        [Column("filled")]
        public string Filled { get; set; }

        [Column("sector")]
        public string Sector { get; set; }

        [Column("trans_date")]
        public System.DateTime? TransDate { get; set; }

        #endregion
    }
}
