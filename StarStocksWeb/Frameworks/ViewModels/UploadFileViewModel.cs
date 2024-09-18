// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using X.PagedList;
using StarStocks.Core.Models;

namespace StarStocksWeb.Frameworks.ViewModels
{
    public class UploadFileViewModel : BaseViewModel
    {
        #region Constructor
        public UploadFileViewModel()
        {

        }
        #endregion

        /// <summary>
        /// 分頁使用
        /// </summary>
        public int Index { get; set; }

        [Display(Name = "HIGH VOLUME CHEAPLIES")]
        public IFormFile HighVolCheaplies { get; set; }

        [Display(Name = "HIGH VOLUME LEAPS")]
        public IFormFile HighVolLeaps { get; set; }

        [Display(Name = "MOST OTM STRIKES")]
        public IFormFile MostOtmStrikes { get; set; }

        [Display(Name = "LARGE OTM OI")]
        public IFormFile LargeOtmOi { get; set; }

        [Display(Name = "CALLS MARKET DASHBOARD")]
        public IFormFile CallsDashboard { get; set; }

        [Display(Name = "PUTS MARKET DASHBOARD")]
        public IFormFile PutsDashboard { get; set; }

        [Display(Name = "DARKPOOL TRADES TODAY")]
        public IFormFile DarkpoolTrades { get; set; }

        [Display(Name = "BLOCK TRADES TODAY")]
        public IFormFile BlockTrades { get; set; }

        [Display(Name = "DARKPOOL TICKER DASHBOARD")]
        public IFormFile DarkpoolTickerDashboard { get; set; }

        [Display(Name = "Blue's Strategy")]
        public IFormFile StrategyResult { get; set; }

        [Display(Name = "Tw Stock Quote Daily")]
        public IFormFile StockQuoteDailyTw { get; set; }

        public List<UploadFile> StrategyFileList { get; set; }

        public List<UploadFile> UploadFileList { get; set; }

        public IPagedList<UploadFile> PagedUploadFileList { get; set; }
    }
}
