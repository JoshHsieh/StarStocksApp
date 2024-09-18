// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace StarStocks.Core.Models
{
    [Table("asset_list", Schema = "public")]
    public class AssetUnit : BaseModel
    {
        #region Implement Property

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("name")]
        public string Name { get; set; }

        #endregion
    }

    /// <summary>
    /// TODO : 要有版本，因為這些 setting 是動態的，不同股價時期會有不一樣的設定
    /// </summary>
    [Table("ticker_settings", Schema = "public")]
    public class TickerSettings : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("setting_category")]
        public string SettingCategory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("setting_name")]
        public string SettingName { get; set; }

        [Column("setting_display_name")]
        public string SettingDisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("setting_value")]
        public string SettingValue { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column("updated_date")]
        public System.DateTime? UpdatedAt { get; set; }

    }

    public class TickerEvent : BaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("event")]
        public string Event { get; set; }

        [Column("event_date")]
        public DateTime? EventDate { get; set; }
    }
}
