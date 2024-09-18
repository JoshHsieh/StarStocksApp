// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarStocks.Core.Models;

namespace StarStocksWeb.Frameworks.ViewModels
{
    public class OptionChartViewModel : BaseViewModel
    {
        public OptionChartViewModel()
        {

        }

        public List<OptionChainDaily> Spx0DteList { get; set; }

        public List<OptionChainDaily> Spy0DteList { get; set; }

        public List<OptionChainDaily> Qqq0DteList { get; set; }
    }

    public class FinmindViewModel
    {
        public FinmindViewModel()
        {

        }

        public string DisplayToken { get; set; }

        public string StockPriceJson { get; set; }

        public List<FinmindStockPriceEntity> PriceList { get; set; }
    }

    public class DarkpoolChartViewModel : BaseViewModel
    {
        public List<DarkpoolValueCrossAvg> CrossAvgList { get; set; }

        public Dictionary<string, List<DarkpoolValueCrossAvg>> CrossAvgDictByDateGroup { get; set; }
    }

}
