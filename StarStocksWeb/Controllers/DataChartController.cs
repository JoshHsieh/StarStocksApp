// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StarStocks.Core.Helpers;
using StarStocks.Core.Managers;
using StarStocks.Core.Models;
using StarStocks.Core.Extensions;
using StarStocksWeb.Frameworks;
using StarStocksWeb.Frameworks.ViewModels;
using StarStocksWeb.Frameworks.Helpers;

namespace StarStocksWeb.Controllers
{
    public class DataChartController : BaseController
    {
        private readonly ILogger<DataChartController> _logger;

        private readonly OptionDataManager _opManger;

        private readonly DarkpoolManager _dpManger;

        private readonly StockQuoteManager _qManger;

        private readonly AssetManager _assetManger;

        public DataChartController(IWebHostEnvironment env, ILogger<DataChartController> logger, OptionDataManager opManager, DarkpoolManager dpManager, StockQuoteManager qManager, AssetManager assetManager)
        {
            if (opManager == null)
            {
                throw new ArgumentNullException(nameof(opManager));
            }

            if (dpManager == null)
            {
                throw new ArgumentNullException(nameof(dpManager));
            }

            if (qManager == null)
            {
                throw new ArgumentNullException(nameof(qManager));
            }

            if (assetManager == null)
            {
                throw new ArgumentNullException(nameof(assetManager));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _opManger = opManager;
            _dpManger = dpManager;
            _qManger = qManager;
            _assetManger = assetManager;

            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DarkpoolValueCrossAvg()
        {
            var vm = new DarkpoolChartViewModel();

            var crossDict = (from valueCrossAvg in _dpManger.DpWithValueCrossAvgLast10Days
                             group valueCrossAvg by valueCrossAvg.TransDate.ToStringOrDefault("yyyy-MM-dd")
                          into groupedValueCrossAvg
                             select groupedValueCrossAvg).ToDictionary(gdc => gdc.Key, gdc => gdc.ToList());

            vm.CrossAvgDictByDateGroup = crossDict.OrderByDescending(x => x.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            return View(vm);
        }

        public async Task<IActionResult> Option0DteView()
        {
            var vm = new OptionChartViewModel();

            _opManger.ResetSpx0dte();

            vm.Spx0DteList = _opManger.Spx0dte;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StockQuoteTwIndex()
        {
            var vm = new StockQuoteTwViewModel();

            var d = new Dictionary<string, List<string>>();

            d = _qManger.ReturnGroupTickerAndDate();

            vm.TickerTransDateGroup = d;

            vm.TickerTransDateDashboard = await _qManger.PopulateTwQuoteDahboard(d);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> TwTickerAnalysis(string ticker, string transDate, string timeFrame = "5")
        {
            var vm = new TickerAnalysisViewModel();

            if (string.IsNullOrEmpty(ticker) != true && string.IsNullOrEmpty(transDate) != true)
            {
                vm.Ticker = ticker;
                vm.TransDate = transDate;

                int timeSpan = 5;
                int.TryParse(timeFrame, out timeSpan);

                vm.Timeframe = timeSpan;

                var report = await _qManger.PopulateQuoteTwAnalysisReport(ticker, transDate);

                vm.QuoteReport = report;

                vm.PopulateAndAnalysis(_assetManger, _qManger);
                vm.BriefFromTickGroup();
            }
            else
            {
                ModelState.AddModelError("err", Constants.InvalidOperation);
            }

            return View(vm);
        }

        public async Task<IActionResult> TestFinmind()
        {
            var vm = new FinmindViewModel();

            var finHelper = new HttpClientHelper();

            string finToken = await finHelper.FinmindSignIn();

            var stockResponse = new FinmindStockPriceResponse();

            stockResponse = await finHelper.FetchStockPriceTickFromFinmind(finToken, "4967", "2023-02-24");

            var priceResult = from price in stockResponse.StockPriceList
                              where DateTime.Parse($"{price.Date} {price.Time}").ToString("HH:mm:ss") == DateTime.Parse("2023/2/24  09:23:38").ToString("HH:mm:ss")
                              select price;

            //_logger.LogDebug($"FinMind Json : {respJson}");

            vm.PriceList = priceResult.ToList();

            return View(vm);
        }

    }
}
