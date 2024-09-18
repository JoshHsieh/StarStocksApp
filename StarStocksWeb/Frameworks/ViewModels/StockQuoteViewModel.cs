// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarStocks.Core.Models;
using StarStocks.Core.Managers;
using StarStocks.Core.Helpers;
using StarStocksWeb.Models;
using StarStocksWeb.Frameworks.Helpers;

namespace StarStocksWeb.Frameworks.ViewModels
{
    public class StockQuoteTwViewModel : BaseViewModel
    {
        public StockQuoteTwViewModel()
        {

        }

        public List<DailyQuoteTwDashboard> TwQuoteList { get; set; }

        public Dictionary<string, List<string>> TickerTransDateGroup { get; set; }

        public List<DailyQuoteTwDashboard> TickerTransDateDashboard { get; set; }
    }

    public class TickerAnalysisViewModel : BaseViewModel
    {
        public TickerAnalysisViewModel()
        {

        }

        public QuoteTwReport QuoteReport { get; set; }

        public string Ticker { get; set; }

        public string TransDate { get; set; }

        /// <summary>
        /// analysis period, ex. 5, 10, 15
        /// </summary>
        public int Timeframe { get; set; }

        public string TickerName { get; set; }

        public int BigQuoteSize { get; set; }

        /// <summary>
        /// 交易起始時間
        /// </summary>
        public List<DateTime?> TransPeriod { get; set; }

        #region Size & Volume
        public List<int> TopSizeByTimeframe { get; set; }

        public int TotalVolume { get; set; }

        /// <summary>
        /// 第一盤試搓
        /// </summary>
        public int FirstVolume { get; set; }

        /// <summary>
        /// 最後5分鐘試搓
        /// </summary>
        public int LastVolume { get; set; }

        /// <summary>
        /// 成交量排名(裡面可以依前5, 前 10 ...) transDatetime_size
        /// </summary>
        public List<int> AllTopSize { get; set; }

        /// <summary>
        /// 內外盤比
        /// </summary>
        public double BidPercent { get; set; }

        public int BidSize { get; set; }

        public double AskPercent { get; set; }

        public int AskSize { get; set; }

        /// <summary>
        /// 平均每筆成交量 (成交量 / 成交筆數 (扣除頭尾試搓))
        /// </summary>
        public double AvgSize { get; set; }

        /// <summary>
        /// 大單比例 (多少張算是大單可以設定)
        /// </summary>
        public double BigSizeRatio { get; set; }

        public double SellBigSizeRatio { get; set; }

        public double BuyBigSizeRatio { get; set; }

        /// <summary>
        /// 外盤成交筆數
        /// </summary>
        public int AskQuote { get; set; }

        public double AskQuotePercent { get; set; }

        public List<StockQuoteTw> AskList { get; set; }

        /// <summary>
        /// 內盤成交筆數
        /// </summary>
        public int BidQuote { get; set; }

        public double BidQuotePercent { get; set; }

        public List<StockQuoteTw> BidList { get; set; }

        /// <summary>
        /// 所有成交筆數
        /// </summary>
        public int TotalQuote { get; set; }

        public Dictionary<string, List<StockQuoteTw>> TimeSeriesDic { get; set; }

        public Dictionary<string, GroupQuoteAnalysisViewModel> TimeSeriesDisplayDic { get; set; }
        #endregion

        #region Price
        /// <summary>
        /// 震幅((最高 - 最低) / 昨收)
        /// </summary>
        public double Vibration { get; set; }

        public double NetChange { get; set; }

        public Dictionary<double, int> VolumeProfile { get; set; }

        /// <summary>
        /// key: price, value: [ask size:bid size:totalVolume]
        /// </summary>
        public Dictionary<double, string> VolumeProfileAskBid { get; set; }
        #endregion

        public void PopulateAndAnalysis(AssetManager assMgr, StockQuoteManager qMgr)
        {
            // Ticker name
            TickerName = assMgr.AssetUnits.Where(x => x.Ticker == QuoteReport.Ticker).FirstOrDefault().Name ?? "";

            // big size criteria
            string strBigSize = string.Empty;

            if (QuoteReport.TickerSettings != null && QuoteReport.TickerSettings.Count > 0 && QuoteReport.TickerSettings.Where(x => x.SettingName == TermHelper.BigSize).FirstOrDefault() != null)
            {
                strBigSize = QuoteReport.TickerSettings.Where(x => x.SettingName == TermHelper.BigSize).FirstOrDefault().SettingValue;
            }
            else
            {
                strBigSize = "10";

                // default value
                if (QuoteReport.Ticker == "3443")
                {
                    strBigSize = "5";
                }
            }

            int bigSizeFlag = 5;

            Int32.TryParse(strBigSize, out bigSizeFlag);

            BigQuoteSize = bigSizeFlag;

            // 計算內外盤
            // 內盤價成交 quote list
            var sellList = QuoteReport.ExcludeFirstLastQuotes.Where(x => x.LastPrice <= x.BidPrice).ToList();

            // 外盤價成交 quote list
            var buyList = QuoteReport.ExcludeFirstLastQuotes.Where(x => x.LastPrice >= x.AskPrice).ToList();

            // 第一次filter 無法判斷內外盤，有spread 的狀況
            var unknownList = QuoteReport.ExcludeFirstLastQuotes.Except(sellList).Except(buyList);

            // update : 先不用這段邏輯
            // 由後一筆的成交價來判斷是 ask or bid，注意不能加入最後一盤的資料
            //unknownList.ToList().ForEach(x =>
            //{
            //    var nxId = x.Id + 1;
            //    var nxQuote = qMgr.ReturnSpecificQuote(nxId);
            //    var lastQuoteId = QuoteReport.AllQuotes.LastOrDefault().Id;
            //    if (nxQuote != null && nxQuote.LastPrice >= nxQuote.AskPrice && nxId != lastQuoteId)
            //    {
            //        buyList.Add(nxQuote);
            //    }

            //    if (nxQuote != null && nxQuote.LastPrice <= nxQuote.BidPrice && nxId != lastQuoteId)
            //    {
            //        sellList.Add(nxQuote);
            //    }
            //});

            int totalQuote = QuoteReport.ExcludeFirstLastQuotes.Count;

            this.Ticker = QuoteReport.Ticker;
            this.NetChange = QuoteReport.AllQuotes.LastOrDefault().NetChange;
            this.FirstVolume = QuoteReport.AllQuotes.OrderBy(x => x.Id).FirstOrDefault().CumulativeVol;
            this.LastVolume = QuoteReport.AllQuotes.OrderBy(x => x.Id).LastOrDefault().Size;
            this.TotalVolume = QuoteReport.AllQuotes.LastOrDefault().CumulativeVol;
            this.AskQuote = buyList.Count;
            this.BidQuote = sellList.Count;
            this.TotalQuote = totalQuote;
            this.AskSize = buyList.Sum(x => x.Size);
            this.BidSize = sellList.Sum(x => x.Size);

            // 計算內外盤成交量比例
            int excludeSize = QuoteReport.ExcludeFirstLastQuotes.LastOrDefault().CumulativeVol;
            double askRaw = Convert.ToDouble(AskSize) / excludeSize;
            this.AskPercent = Math.Round(askRaw * 100, 0, MidpointRounding.AwayFromZero);

            double bidRaw = Convert.ToDouble(BidSize) / excludeSize;
            this.BidPercent = Math.Round(bidRaw * 100, 0, MidpointRounding.AwayFromZero);

            // 計算內外盤成交筆數比例
            double askQuoteRaw = Convert.ToDouble(buyList.Count) / totalQuote;
            this.AskQuotePercent = Math.Round(askQuoteRaw * 100, 0, MidpointRounding.AwayFromZero);

            double bidQuoteRaw = Convert.ToDouble(sellList.Count) / totalQuote;
            this.BidQuotePercent = Math.Round(bidQuoteRaw * 100, 0, MidpointRounding.AwayFromZero);

            int bigSize = QuoteReport.ExcludeFirstLastQuotes.Where(x => x.Size >= bigSizeFlag).ToList().Count;
            double ratio = Convert.ToDouble(bigSize) / QuoteReport.ExcludeFirstLastQuotes.Count;
            this.BigSizeRatio = Math.Round(ratio * 100, 1, MidpointRounding.AwayFromZero);
            //_logger.LogDebug($"bigSize : {bigSize}, tCount : {excludeil.Count}, cDouble:{ratio}");

            BidList = sellList.Where(x => x.Size >= bigSizeFlag).OrderBy(i => i.TransDate).ToList();
            int sellSize = BidList.Count;
            double sellRatio = Convert.ToDouble(sellSize) / sellList.Count;
            this.SellBigSizeRatio = Math.Round(sellRatio * 100, 1, MidpointRounding.AwayFromZero);

            AskList = buyList.Where(x => x.Size >= bigSizeFlag).OrderBy(i => i.TransDate).ToList();
            int buySize = AskList.Count;
            double buyRatio = Convert.ToDouble(buySize) / buyList.Count;
            this.BuyBigSizeRatio = Math.Round(buyRatio * 100, 1, MidpointRounding.AwayFromZero);

            this.AvgSize = Math.Round(Convert.ToDouble(QuoteReport.ExcludeFirstLastQuotes.OrderBy(x => x.Id).LastOrDefault().CumulativeVol) / QuoteReport.ExcludeFirstLastQuotes.Count, 2, MidpointRounding.AwayFromZero);

            // 震幅
            double preClose = QuoteReport.AllQuotes.OrderBy(x => x.Id).LastOrDefault().LastPrice - QuoteReport.AllQuotes.OrderBy(x => x.Id).LastOrDefault().NetChange;
            this.Vibration = Math.Round(((QuoteReport.AllQuotes.Max(x => x.LastPrice) - QuoteReport.AllQuotes.Min(x => x.LastPrice)) / preClose) * 100, 1, MidpointRounding.AwayFromZero);

            // volume profile
            var vf = QuoteReport.AllQuotes.GroupBy(o => o.LastPrice).ToDictionary(o => o.Key, o => o.ToList().Sum(x => x.Size));
            this.VolumeProfile = vf;

            GroupTickByInterval(Timeframe);
        }
        public void GroupTickByInterval(int minutes)
        {
            var period = TimeSpan.FromMinutes(minutes);

            var quotes = QuoteReport.AllQuotes.Select(d => new StockQuoteTw
            {
                Id = d.Id,
                Ticker = d.Ticker,
                TransDate = d.TransDate,
                AskPrice = d.AskPrice,
                BidPrice = d.BidPrice,
                LastPrice = d.LastPrice,
                CumulativeVol = d.CumulativeVol,
                Size = d.Size,
                NetChange = d.NetChange,
                Span = new TimeSpan((d.TransDate.Value.TimeOfDay.Ticks / period.Ticks) * period.Ticks).ToString(@"hh\:mm")
                //Span = d.TransDate.Value.TimeOfDay.Ticks / period.Ticks
            })
                .GroupBy(d => d.Span)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dr = new DateRange();
            dr.Start = DateTime.ParseExact($"{QuoteReport.AllQuotes.FirstOrDefault().TransDate.Value.ToString("yyyy-MM-dd")} 09:00:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            dr.End = DateTime.ParseExact($"{QuoteReport.AllQuotes.FirstOrDefault().TransDate.Value.ToString("yyyy-MM-dd")} 13:30:00", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var dateL = SiteHelper.SplitInToDatetimes(dr, minutes).Select(x => x.Start.ToString("HH:mm")).ToList();

            var d = new Dictionary<string, List<StockQuoteTw>>();

            dateL.ForEach(x =>
            {
                var ql = new List<StockQuoteTw>();
                quotes.TryGetValue(x, out ql);
                d.Add(x, ql);
            });

            this.TimeSeriesDic = d;
        }

        public void BriefFromTickGroup()
        {
            if (TimeSeriesDic != null && TimeSeriesDic.Count > 0)
            {
                int i = 1;

                double openPrice = 0.0;

                // 計算時間段內成交量
                int preVol = 0;

                if (TimeSeriesDisplayDic != null)
                {
                    TimeSeriesDisplayDic.Clear();
                }
                else
                {
                    TimeSeriesDisplayDic = new Dictionary<string, GroupQuoteAnalysisViewModel>();
                }

                foreach (KeyValuePair<string, List<StockQuoteTw>> kvp in TimeSeriesDic)
                {
                    var l = kvp.Value;

                    var gq = new GroupQuoteAnalysisViewModel();

                    if (i == 1)
                    {
                        openPrice = l.FirstOrDefault().LastPrice;

                        // 第一筆試搓不計算
                        l.RemoveAt(0);

                        var cumulativeVol = l.LastOrDefault().CumulativeVol;

                        gq = InjectToGroupQuote(l, cumulativeVol, openPrice);
                    }
                    else if (i == TimeSeriesDic.Count)
                    {
                        // 先要保存最後一筆的 netchange, last price
                        double netChange = l.LastOrDefault().NetChange;
                        double lastPrice = l.LastOrDefault().LastPrice;

                        // 最後一筆試搓不計算，但是只有一筆也不需要移除
                        if (l.Count > 1)
                        {
                            l.RemoveAt(l.Count - 1);
                        }

                        var cumulativeVol = l.LastOrDefault().CumulativeVol - preVol;

                        gq = InjectToGroupQuote(l, cumulativeVol, openPrice);

                        gq.NetChange = netChange;
                        gq.NetChangeSinceOpen = lastPrice - openPrice;
                    }
                    else
                    {
                        if (l != null && l.Any())
                        {
                            var cumulativeVol = l.LastOrDefault().CumulativeVol - preVol;

                            gq = InjectToGroupQuote(l, cumulativeVol, openPrice);
                        }
                    }

                    if (l != null && l.Any())
                    {
                        preVol = l.LastOrDefault().CumulativeVol;
                    }

                    TimeSeriesDisplayDic.Add(kvp.Key, gq);
                    i++;
                }
            }
        }

        private GroupQuoteAnalysisViewModel InjectToGroupQuote(List<StockQuoteTw> l, int cumulativeVol, double openPrice)
        {
            var gq = new GroupQuoteAnalysisViewModel();

            gq.TotalVolume = cumulativeVol;

            var sellL = l.Where(x => x.LastPrice <= x.BidPrice).ToList();
            var sellSize = sellL.Sum(o => o.Size);
            int sellCount = sellL.Count;
            double sellRaw = Convert.ToDouble(sellSize) / cumulativeVol;
            int sellBigSizeCount = l.Where(x => x.LastPrice <= x.BidPrice && x.Size >= BigQuoteSize).ToList().Count;
            double sellBigSizeRatioRaw = sellCount < 1 ? 0 : Convert.ToDouble(sellBigSizeCount) / sellCount;

            gq.SellSideSize = sellSize;
            gq.SellSideRatio = Math.Round(sellRaw * 100, 1, MidpointRounding.AwayFromZero);
            gq.SellSideBigSizeRatio = Math.Round(sellBigSizeRatioRaw * 100, 1, MidpointRounding.AwayFromZero);
            gq.SellSideBigSizeCount = sellBigSizeCount;

            var buyL = l.Where(x => x.LastPrice >= x.AskPrice).ToList();
            var buySize = buyL.Sum(o => o.Size);
            int buyCount = buyL.Count;
            double buyRaw = Convert.ToDouble(buySize) / cumulativeVol;
            int buyBigSizeCount = l.Where(x => x.LastPrice >= x.AskPrice && x.Size >= BigQuoteSize).ToList().Count;
            double buyBigSizeRatioRaw = buyCount < 1 ? 0 : Convert.ToDouble(buyBigSizeCount) / buyCount;

            gq.BuySideSize = buySize;
            gq.BuySideRatio = Math.Round(buyRaw * 100, 1, MidpointRounding.AwayFromZero);
            gq.BuySideBigSizeRatio = Math.Round(buyBigSizeRatioRaw * 100, 1, MidpointRounding.AwayFromZero);
            gq.BuySideBigSizeCount = buyBigSizeCount;

            double avgSizeRaw = Convert.ToDouble(cumulativeVol) / l.Count;
            gq.AvgQuoteSize = Math.Round(avgSizeRaw, 1, MidpointRounding.AwayFromZero);

            double netChangeRateRaw = Convert.ToDouble(l.LastOrDefault().NetChange) / (l.LastOrDefault().LastPrice - l.LastOrDefault().NetChange);
            gq.NetChange = l.LastOrDefault().NetChange;
            gq.NetChangeRatio = Math.Round(netChangeRateRaw * 100, 1, MidpointRounding.AwayFromZero);

            double openChaneRateRaw = Convert.ToDouble(l.LastOrDefault().LastPrice - openPrice) / openPrice;
            gq.NetChangeSinceOpen = l.LastOrDefault().LastPrice - openPrice;
            gq.NetChangeRatioSinceOpen = Math.Round(openChaneRateRaw * 100, 1, MidpointRounding.AwayFromZero);

            return gq;
        }

        private void CalculateAskBidInVolProfile()
        {
            if (VolumeProfile != null && VolumeProfile.Count > 1)
            {
                var vpAskBid = new Dictionary<double, string>();

                foreach (KeyValuePair<double, int> item in VolumeProfile)
                {
                    if (QuoteReport.AllQuotes != null && QuoteReport.AllQuotes.Any())
                    {
                        var buyList = QuoteReport.AllQuotes.Where(x => x.LastPrice == item.Key && x.LastPrice >= x.AskPrice).ToList();

                        var sellList = QuoteReport.AllQuotes.Where(x => x.LastPrice == item.Key && x.LastPrice <= x.BidPrice).ToList();

                        vpAskBid.Add(item.Key, $"{buyList.Sum(x => x.Size)}:{sellList.Sum(x => x.Size)}:{item.Value}");
                    }
                }

                this.VolumeProfileAskBid = vpAskBid;
            }
        }
    }

    public class GroupQuoteAnalysisViewModel
    {
        /// <summary>
        /// 震幅
        /// </summary>
        public double NetChangeSinceOpen { get; set; }

        public double NetChangeRatioSinceOpen { get; set; }

        /// <summary>
        /// 漲跌對比昨天收盤
        /// </summary>
        public double NetChange { get; set; }

        public double NetChangeRatio { get; set; }

        public int TotalVolume { get; set; }

        /// <summary>
        /// 平均每筆成交量 (成交量 / 成交筆數)
        /// </summary>
        public double AvgQuoteSize { get; set; }

        /// <summary>
        /// 大單數量
        /// </summary>
        public int SellSideBigSizeCount { get; set; }

        /// <summary>
        /// 大單比例 (多少張算是大單可以設定)
        /// </summary>
        public double SellSideBigSizeRatio { get; set; }

        public int SellSideSize { get; set; }

        public double SellSideRatio { get; set; }

        public int BuySideBigSizeCount { get; set; }

        public double BuySideBigSizeRatio { get; set; }

        public int BuySideSize { get; set; }

        public double BuySideRatio { get; set; }
    }
}
