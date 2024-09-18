// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentDateTime;
using Dapper;
using StarStocks.Core.Extensions;
using StarStocks.Core.Interfaces;
using StarStocks.Core.Helpers;
using StarStocks.Core.DbWrapper;
using StarStocks.Core.Repositories;
using StarStocks.Core.UnitOfWork;
using StarStocks.Core.Models;

namespace StarStocks.Core.Managers
{
    public sealed class AssetManager
    {
        private readonly ILogger<AssetManager> _logger;

        private static DbConnection _dbConn;

        public AssetManager(ILogger<AssetManager> logger, DbConnection dbConn)
        {
            if (dbConn == null)
            {
                throw new ArgumentNullException(nameof(dbConn));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;

            _dbConn = dbConn;
        }

        #region Data Member
        #region Asset List
        private List<AssetUnit> _assetUnits;

        public List<AssetUnit> AssetUnits
        {
            get
            {
                if (_assetUnits != null)
                {
                    return _assetUnits;
                }

                ResetAssetUnits();

                return _assetUnits;
            }
        }

        public void ResetAssetUnits()
        {
            if (_assetUnits == null)
            {
                _assetUnits = new List<AssetUnit>();
            }

            _assetUnits.Clear();

            try
            {
                _assetUnits = MakeAssetUnits().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeAssetUnits Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<AssetUnit> MakeAssetUnits()
        {
            var l = new List<AssetUnit>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var aRepo = new AssetUnitRepository(dataSession.Unit);

                l = aRepo.GetAll().ToList();
            }

            return l;
        }
        #endregion

        #region Ticker Seeting
        private List<TickerSettings> _tickerSettings;

        public List<TickerSettings> TickerSettings
        {
            get
            {
                if (_tickerSettings != null)
                {
                    return _tickerSettings;
                }

                ResetTickerSettings();

                return _tickerSettings;
            }
        }

        public void ResetTickerSettings()
        {
            if (_tickerSettings == null)
            {
                _tickerSettings = new List<TickerSettings>();
            }

            _tickerSettings.Clear();

            try
            {
                _tickerSettings = MakeTickerSettings().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeTickerSettings Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<TickerSettings> MakeTickerSettings()
        {
            var l = new List<TickerSettings>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var tRepo = new TickerSettingRepository(dataSession.Unit);

                l = tRepo.GetAll().ToList();
            }

            return l;
        }
        #endregion
        #endregion

        #region public method
        public async Task<List<TickerSettings>> ReturnSettingsByTicker(string ticker)
        {
            var l = new List<TickerSettings>();

            if (string.IsNullOrEmpty(ticker))
            {
                return TickerSettings;
            }

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var tRepo = new TickerSettingRepository(dataSession.Unit);

                string tSql = $"SELECT * FROM ticker_settings WHERE ticker = '{ticker}'";

                l = (await tRepo.QueryBySqlAsync(tSql)).ToList();
            }

            return l;
        }

        public async Task<string> ReturnEventMessageByTicker(string ticker, string eDate)
        {
            string eMesage = string.Empty;

            if (string.IsNullOrEmpty(ticker))
            {
                return eMesage;
            }

            var te = new TickerEvent();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var eRepo = new TickerEventRepository(dataSession.Unit);

                string tSql = $"SELECT * FROM ticker_event WHERE ticker = @Ticker and TO_CHAR(event_date, 'YYYY-MM-DD') = @EventDate";

                var parameters = new DynamicParameters();
                parameters.Add("@Ticker", ticker);
                parameters.Add("@EventDate", eDate);

                te = await eRepo.QuerySingleAsync(tSql, parameters);

                if (te != null)
                {
                    eMesage = te.Event;
                }
            }

            return eMesage;
        }
        #endregion
    }

    public sealed class StockQuoteManager
    {
        private readonly ILogger<StockQuoteManager> _logger;

        private static DbConnection _dbConn;

        private AssetManager _assetManager;

        public StockQuoteManager(ILogger<StockQuoteManager> logger, DbConnection dbConn, AssetManager assManager)
        {
            if (dbConn == null)
            {
                throw new ArgumentNullException(nameof(dbConn));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;

            _dbConn = dbConn;

            _assetManager = assManager;
        }

        #region Data Member
        #endregion

        #region Public Method
        public StockQuoteTw ReturnSpecificQuote(int qId)
        {
            var qq = new StockQuoteTw();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                qq = qRepo.FindById(qId);
            }

            return qq;
        }

        public List<StockQuoteTw> ReturnTwQuoteSingleDayByDateAndTicker(string ticker, string transDate)
        {
            var l = new List<StockQuoteTw>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                string sql = $@"select * from tw_stock_quote WHERE date(trans_date) = '{transDate}' AND ticker = '{ticker}' ORDER BY trans_date";

                l = qRepo.QueryBySql(sql).ToList();
            }

            return l;
        }

        public List<StockQuoteTw> ReturnTwQuoteByDateRangeAndTicker(string ticker, string sDate, string eDate = "")
        {
            var l = new List<StockQuoteTw>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                var sb = new StringBuilder();

                string sql = $@"select * from tw_stock_quote where ticker = '{ticker}'";

                sb.Append(sql);

                if (string.IsNullOrEmpty(eDate) != true)
                {
                    sb.Append($" and date(trans_date)>= '{sDate}' and date(trans_date) <= '{eDate}' ");
                }
                else
                {
                    sb.Append($" and date(trans_date)>= '{sDate}' ");
                }

                sb.Append(" order by trans_date;");

                l = qRepo.QueryBySql(sb.ToString()).ToList();
            }

            return l;
        }

        public Dictionary<string, int> ReturnVolSizeByTimeRange(string ticker, string transDate, int mins)
        {
            var d = new Dictionary<string, int>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                var sb = new StringBuilder();

                string sql = $@"SELECT sum(size) as qSize,
                                date_trunc('hour', trans_date) +
                                    (((date_part('minute', trans_date)::integer / {mins}::integer) * {mins}::integer)
                                     || ' minutes')::interval AS TimeGroup
                                FROM tw_stock_quote WHERE ticker = '{ticker}' AND date(trans_date) = '{transDate}' AND TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00' GROUP BY TimeGroup;";

                var dt = qRepo.ReturnDataTableFromSql(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        d.Add(dr["TimeGroup"].ToString(), Convert.ToInt32(dr["qSize"]));
                    }
                }
            }

            return d;
        }

        /// <summary>
        /// minimal date
        /// </summary>
        /// <param name="mDate"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ReturnGroupTickerAndDate(string mDate = "")
        {
            var d = new Dictionary<string, List<string>>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                var sb = new StringBuilder();

                string sql = string.Empty;

                if (string.IsNullOrEmpty(mDate))
                {
                    sql = "select ticker, date(trans_date) as tDate from tw_stock_quote group by ticker, date(trans_date) order by tDate;";
                }
                else
                {
                    sql = $@"select ticker, date(trans_date) as tDate from tw_stock_quote where trans_date >= '{mDate}' group by ticker, date(trans_date) order by tDate;";
                }

                var dt = qRepo.ReturnDataTableFromSql(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    d = dt.AsEnumerable().GroupBy(x => x.Field<string>("ticker")).ToDictionary
                        (
                        g => g.Key,
                        g => g.Select(x => x.Field<DateTime>("tDate").ToString("yyyy-MM-dd")).ToList()
                        );
                }
            }

            return d;
        }

        /// <summary>
        /// pass ticker with list of trans date and return Dashboard by date
        /// </summary>
        /// <param name="tickerD"></param>
        /// <returns></returns>
        public async Task<List<DailyQuoteTwDashboard>> PopulateTwQuoteDahboard(Dictionary<string, List<string>> tickerD)
        {
            var l = new List<DailyQuoteTwDashboard>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                foreach (var kvp in tickerD)
                {
                    // get ticker's settings
                    var tickerSettings = await _assetManager.ReturnSettingsByTicker(kvp.Key);

                    if (kvp.Value != null && kvp.Value.Any())
                    {
                        foreach (var item in kvp.Value)
                        {
                            var quote = new DailyQuoteTwDashboard();

                            string allQuote = $"SELECT * FROM tw_stock_quote WHERE ticker = @Ticker AND TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate AND TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00';";

                            var allil = await ReturnAllStockQuote(kvp.Key, item);

                            var excludeil = await ReturnStockQuoteWithoutFirstLast(kvp.Key, item);

                            quote.Ticker = kvp.Key;
                            quote.TransDate = item;

                            double netChg = allil.OrderBy(x => x.Id).LastOrDefault().NetChange;

                            // 昨收盤
                            double preClose = allil.OrderBy(x => x.Id).LastOrDefault().LastPrice - netChg;
                            quote.NetChange = netChg;
                            quote.Vibration = Math.Round(((allil.Max(x => x.LastPrice) - allil.Min(x => x.LastPrice)) / preClose) * 100, 1, MidpointRounding.AwayFromZero);

                            quote.TotalVolume = allil.OrderBy(x => x.Id).LastOrDefault().CumulativeVol;
                            quote.FirstVolume = allil.OrderBy(x => x.Id).FirstOrDefault().CumulativeVol;
                            quote.AvgSize = Math.Round(Convert.ToDouble(excludeil.OrderBy(x => x.Id).LastOrDefault().CumulativeVol) / excludeil.Count, 1, MidpointRounding.AwayFromZero);

                            int bigSizeFlag = await FetchBigSizeSetting(kvp.Key);

                            int bigSize = excludeil.Where(x => x.Size >= bigSizeFlag).ToList().Count;
                            double ratio = Convert.ToDouble(bigSize) / excludeil.Count;
                            //_logger.LogDebug($"bigSize : {bigSize}, tCount : {excludeil.Count}, cDouble:{ratio}");

                            quote.BigSizeRatio = Math.Round(ratio * 100, 1, MidpointRounding.AwayFromZero);
                            quote.EventMessage = await _assetManager.ReturnEventMessageByTicker(kvp.Key, item);

                            l.Add(quote);
                        }
                    }
                }
            }

            return l;
        }

        public async Task<QuoteTwReport> PopulateQuoteTwAnalysisReport(string ticker, string transDate)
        {
            var qReport = new QuoteTwReport();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                qReport.Ticker = ticker;

                var allil = await ReturnAllStockQuote(ticker, transDate);
                qReport.AllQuotes = allil;

                var excludeil = await ReturnStockQuoteWithoutFirstLast(ticker, transDate);
                qReport.ExcludeFirstLastQuotes = excludeil;

                var tickerSettings = await _assetManager.ReturnSettingsByTicker(ticker);
                qReport.TickerSettings = tickerSettings;
            }

            return qReport;
        }

        #endregion

        #region Private method
        /// <summary>
        /// TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00' -> 這各條件式為了不要抓到盤後成交的紀錄
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="transDate"></param>
        /// <returns></returns>
        private async Task<List<StockQuoteTw>> ReturnAllStockQuote(string ticker, string transDate)
        {
            var l = new List<StockQuoteTw>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                var allQuote = $"SELECT * FROM tw_stock_quote WHERE ticker = @Ticker AND TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate AND TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00';";

                var aqParams = new DynamicParameters();
                aqParams.Add("@Ticker", ticker);
                aqParams.Add("@QDate", transDate);

                l = (await qRepo.QueryBySqlAsync(allQuote, aqParams)).ToList();
            }

            return l;
        }

        /// <summary>
        /// 排除頭尾試搓
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="transDate"></param>
        /// <returns></returns>
        private async Task<List<StockQuoteTw>> ReturnStockQuoteWithoutFirstLast(string ticker, string transDate)
        {
            var l = new List<StockQuoteTw>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                // 排除第一筆，最後一筆 試搓
                string excludeQuote = $@"SELECT * FROM tw_stock_quote WHERE ticker = @Ticker and TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate 
                                   AND id > (SELECT MIN(Id) FROM tw_stock_quote WHERE ticker = @Ticker AND TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate ) 
                                   AND id < (SELECT MAX(Id) FROM tw_stock_quote WHERE ticker = @Ticker AND TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate )
                                   AND TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00';";

                var eqParams = new DynamicParameters();
                eqParams.Add("@Ticker", ticker);
                eqParams.Add("@QDate", transDate);

                l = (await qRepo.QueryBySqlAsync(excludeQuote, eqParams)).ToList();
            }

            return l;
        }

        private async Task<List<StockQuoteTw>> ReturnStockQuoteWithoutFirstQuote(string ticker, string transDate)
        {
            var l = new List<StockQuoteTw>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                // 排除第一筆試搓
                string excludeQuote = $@"SELECT * FROM tw_stock_quote WHERE ticker = @Ticker AND TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate AND TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00'
                                   AND id > (SELECT MIN(Id) FROM tw_stock_quote WHERE ticker = @Ticker AND TO_CHAR(trans_date, 'YYYY-MM-DD') = @QDate AND TO_CHAR(trans_date, 'HH24:MI:SS') < '13:31:00') ";

                var eqParams = new DynamicParameters();
                eqParams.Add("@Ticker", ticker);
                eqParams.Add("@QDate", transDate);

                l = (await qRepo.QueryBySqlAsync(excludeQuote, eqParams)).ToList();
            }

            return l;
        }

        private async Task<List<StockQuoteTw>> ReturnLastDaysStockQuote(string ticker, string transDate, int days = 5)
        {
            var l = new List<StockQuoteTw>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var qRepo = new StockQuoteRepository(dataSession.Unit);

                string tDateSql = $"SELECT distinct(TO_CHAR(trans_date, 'YYYY-MM-DD')) as transDate FROM tw_stock_quote WHERE ticker = @Ticker GROUP BY TO_CHAR(trans_date, 'YYYY-MM-DD') ORDER BY transDate desc";

                var tParams = new DynamicParameters();
                tParams.Add("@Ticker", ticker);

                var datel = (await qRepo.QueryListValueAsync(tDateSql, tParams)).ToList().OfType<string>();
                var orderDatel = datel.OrderByDescending(i => DateTime.ParseExact(i, "yyyy-MM-dd", null)).ToList();

                // 取要找出過往幾日的日期(當日不計入)
                var lastDatel = orderDatel.GetRange(1, days);
            }

            return l;
        }

        private async Task<int> FetchBigSizeSetting(string ticker)
        {
            var tickerSettings = await _assetManager.ReturnSettingsByTicker(ticker);

            string strBigSize = string.Empty;

            if (tickerSettings != null && tickerSettings.Count > 0 && tickerSettings.Where(x => x.SettingName == TermHelper.BigSize).FirstOrDefault() != null)
            {
                strBigSize = tickerSettings.Where(x => x.SettingName == TermHelper.BigSize).FirstOrDefault().SettingValue;
            }
            else
            {
                strBigSize = "10";

                // default value
                if (ticker == "3443")
                {
                    strBigSize = "5";
                }

                if (ticker == "3661")
                {
                    strBigSize = "4";
                }
            }

            return Convert.ToInt32(strBigSize);
        }
        #endregion
    }
}
