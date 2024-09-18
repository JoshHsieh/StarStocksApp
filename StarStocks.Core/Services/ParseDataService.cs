// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using Dapper;
using StarStocks.Core.Extensions;
using StarStocks.Core.Interfaces;
using StarStocks.Core.Helpers;
using StarStocks.Core.DbWrapper;
using StarStocks.Core.Repositories;
using StarStocks.Core.UnitOfWork;
using StarStocks.Core.Models;

namespace StarStocks.Core.Services
{
    public class ParseDataService : IFileService
    {
        private readonly ILogger<ParseDataService> _logger;

        private readonly DbConnection _dbConn;

        private IResult _r;

        #region Transaction Date
        private DateTime? fileTransDt;

        #endregion

        public ParseDataService(ILogger<ParseDataService> logger, DbConnection dbConn, IResult r)
        {
            if (dbConn == null)
            {
                throw new ArgumentNullException(nameof(dbConn));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (r == null)
            {
                throw new ArgumentNullException(nameof(r));
            }

            _logger = logger;

            _r = r;

            _dbConn = dbConn;

        }

        #region IService

        public IResult ReturnServiceResult()
        {
            if (_r == null)
            {
                _r = new ResultContainer(true);
            }

            return _r;
        }
        #endregion

        #region IFileService
        /// <summary>
        /// https://stackoverflow.com/questions/29907829/c-sharp-csv-parsing-escaping-double-quotes
        /// </summary>
        /// <param name="fDict"></param>
        /// <returns></returns>
        public async Task<IResult> DoService(Dictionary<string, string> fDict)
        {
            // reset result container
            _r.Reset();

            if (fDict != null && fDict.Count > 0)
            {
                Dictionary<string, string>.KeyCollection keyL = fDict.Keys;

                var callList = new List<string>();

                var putList = new List<string>();

                var darkPoolList = new List<string>();

                var otmStrikeList = new List<string>();

                var dpDashboardList = new List<string>();

                var strategyList = new List<string>();

                var twStockQuoteList = new List<string>();

                string tickerTW = string.Empty;

                // reading content from file
                try
                {
                    foreach (var fk in keyL)
                    {
                        string fPath = string.Empty;

                        if (fDict.TryGetValue(FileModelMapper.CallsDashboard, out fPath))
                        {
                            // retrive transaction date
                            string extractFile = Path.GetFileNameWithoutExtension(fPath);

                            string extractDate = extractFile.GetLast(8);

                            fileTransDt = DateTime.UtcNow.Date.ToEstNy().AddDays(-1);

                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    if (rowNo != 0)
                                    {
                                        callList.Add(line);
                                    }

                                    rowNo++;
                                }
                            }
                        }

                        if (fDict.TryGetValue(FileModelMapper.PutsDashboard, out fPath))
                        {
                            // retrive transaction date
                            string extractFile = Path.GetFileNameWithoutExtension(fPath);

                            string extractDate = extractFile.GetLast(8);

                            //fileTransDt = extractDate.ToDateTime("yyyymmdd", CultureInfo.InvariantCulture);
                            fileTransDt = DateTime.UtcNow.Date.ToEstNy().AddDays(-1);

                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    // skip first line( title line)
                                    if (rowNo != 0)
                                    {
                                        putList.Add(line);
                                    }

                                    rowNo++;
                                }
                            }
                        }

                        if (fDict.TryGetValue(FileModelMapper.DarkpoolTrades, out fPath))
                        {
                            // retrive transaction date
                            string extractFile = Path.GetFileNameWithoutExtension(fPath);

                            string extractDate = extractFile.GetLast(8);

                            //fileTransDt = DateTime.UtcNow.Date.ToEstNy().AddDays(-1);
                            fileTransDt = extractDate.ToDateTime("yyyyMMdd", CultureInfo.InvariantCulture).Date;

                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    // skip first line( title line)
                                    if (rowNo != 0)
                                    {
                                        darkPoolList.Add(line);
                                    }
                                    rowNo++;
                                }
                            }
                        }

                        if (fDict.TryGetValue(FileModelMapper.BlockTrades, out fPath))
                        {
                            // retrive transaction date
                            string extractFile = Path.GetFileNameWithoutExtension(fPath);

                            string extractDate = extractFile.GetLast(8);

                            //fileTransDt = DateTime.UtcNow.Date.ToEstNy().AddDays(-1);
                            fileTransDt = extractDate.ToDateTime("yyyyMMdd", CultureInfo.InvariantCulture).Date;

                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    // skip first line( title line)
                                    if (rowNo != 0)
                                    {
                                        darkPoolList.Add(line);
                                    }

                                    rowNo++;
                                }
                            }
                        }

                        if (fDict.TryGetValue(FileModelMapper.MostOtmStrikes, out fPath))
                        {
                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    otmStrikeList.Add(line);

                                    rowNo++;
                                }
                            }
                        }

                        if (fDict.TryGetValue(FileModelMapper.DarkpoolTickerDashboard, out fPath))
                        {
                            // retrive transaction date
                            string extractFile = Path.GetFileNameWithoutExtension(fPath);

                            string extractDate = extractFile.GetLast(8);

                            //fileTransDt = DateTime.UtcNow.Date.ToEstNy().AddDays(-1);
                            fileTransDt = extractDate.ToDateTime("yyyyMMdd", CultureInfo.InvariantCulture).Date;

                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    // skip first line( title line)
                                    if (rowNo != 0)
                                    {
                                        dpDashboardList.Add(line);
                                    }

                                    rowNo++;
                                }
                            }
                        }

                        // parse blue's strategy file
                        #region parse blue's strategy file
                        //if (fDict.TryGetValue(FileModelMapper.StrategyResult, out fPath))
                        //{
                        //    using (var workBook = new XLWorkbook(fPath))
                        //    {
                        //        // read the first sheet
                        //        IXLWorksheet workSheet = workBook.Worksheet(1);

                        //        if (workSheet == null)
                        //        {
                        //            _r.IsValid = false;

                        //            _r.GatherErrorList("excel worksheet is null!!");
                        //        }

                        //        string line = string.Empty;

                        //        int rowNo = 0;

                        //        using (var dataSession = new DataTransSession(_dbConn))
                        //        {
                        //            // store data into DB
                        //            var stratRepo = new StrategyRespository(dataSession.Unit);

                        //            var tmpL = new List<StrategyResult>();

                        //            // skip the first title row
                        //            foreach (var row in workSheet.RangeUsed().RowsUsed().Skip(1))
                        //            {
                        //                var strateR = new StrategyResult();

                        //                // ticker
                        //                string ticker = row.Cell(3).GetString().Trim().ToUpper();
                        //                strateR.Ticker = ticker;

                        //                // buy time
                        //                var buyTime = ParseDateValue(row.Cell(1));
                        //                strateR.BuyTime = buyTime;

                        //                // sell time
                        //                var sellTime = ParseDateValue(row.Cell(2));
                        //                strateR.SellTime = sellTime;

                        //                tmpL.Add(strateR);

                        //                rowNo++;
                        //            }

                        //            var resultL = await FetchPriceResult(tmpL);

                        //            stratRepo.AddRange(resultL);
                        //        }
                        //    }
                        //}
                        #endregion

                        // parse tw stock quote file from yuan da 
                        if (fDict.TryGetValue(FileModelMapper.StockQuoteTw, out fPath))
                        {
                            // retrive transaction date
                            string extractFile = Path.GetFileNameWithoutExtension(fPath);

                            string extractDate = extractFile.GetLast(8);

                            tickerTW = extractFile.Split('-')[1];

                            //fileTransDt = DateTime.UtcNow.Date.ToEstNy().AddDays(-1);
                            fileTransDt = extractDate.ToDateTime("yyyyMMdd", CultureInfo.InvariantCulture).Date;

                            using (var streamReader = new StreamReader(fPath))
                            {
                                string line = string.Empty;

                                int rowNo = 0;

                                while (string.IsNullOrEmpty(line = streamReader.ReadLine()) != true)
                                {
                                    // skip first line( title line)
                                    if (rowNo != 0)
                                    {
                                        twStockQuoteList.Add(line);
                                    }

                                    rowNo++;
                                }
                            }
                        }
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError("Convert File Transaction Date Error :{0}", ex);

                    _r.IsValid = false;

                    _r.GatherErrorList("Convert File Transaction Date Error");

                    throw ex;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Read File Error :{0}", ex);

                    _r.IsValid = false;

                    _r.GatherErrorList("Read File Error");

                    throw ex;
                }

                // call
                if (callList.Count > 0)
                {
                    int i = 1;

                    char[] quotes = { '\"', ' ' };

                    try
                    {
                        using (var dataSession = new DataTransSession(_dbConn))
                        {
                            // store data into DB
                            var optionRepo = new OptionDashboardRepository(dataSession.Unit);

                            foreach (var pc in callList)
                            {
                                var l = pc.Split(',', StringSplitOptions.None).Select(s => s.Trim(quotes).Replace("\\\"", "\"")).ToList();

                                var putCall = new OptionDashboard();

                                putCall = InjectDataToOption("C", l);

                                int? rr = await optionRepo.AddAsync(putCall);

                                if (rr.HasValue != true && rr.GetValueOrDefault() < 1)
                                {
                                    _r.GatherErrorList(string.Format("Ticker: {0} Add call option to DB failure!", putCall.Ticker));

                                    _r.IsValid = false;
                                }

                                i++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Add call File into DB Error :{ex}");

                        _r.IsValid = false;

                        _r.GatherErrorList("Add call File into DB Error.");

                        throw ex;
                    }

                    if (callList.Count != i)
                    {
                        _r.IsValid = false;

                        _r.GatherErrorList("Process call data count not match upload data count.");

                        _logger.LogError("Process call data count not match upload data count.");
                    }
                }

                // put
                if (putList.Count > 0)
                {
                    int i = 1;

                    char[] quotes = { '\"', ' ' };

                    try
                    {
                        using (var dataSession = new DataTransSession(_dbConn))
                        {
                            // store data into DB
                            var optionRepo = new OptionDashboardRepository(dataSession.Unit);

                            foreach (var pc in putList)
                            {
                                var l = pc.Split(',', StringSplitOptions.None).Select(s => s.Trim(quotes).Replace("\\\"", "\"")).ToList();

                                var putCall = new OptionDashboard();

                                putCall = InjectDataToOption("P", l);

                                int? rr = await optionRepo.AddAsync(putCall);

                                if (rr.HasValue != true && rr.GetValueOrDefault() < 1)
                                {
                                    _r.GatherErrorList(string.Format("Ticker: {0} Add put option to DB failure!", putCall.Ticker));

                                    _r.IsValid = false;
                                }

                                i++;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Add put File into DB Error :{ex}");

                        _r.IsValid = false;

                        _r.GatherErrorList("Add put File into DB Error.");

                        throw ex;
                    }

                    if (putList.Count != i)
                    {
                        _r.IsValid = false;

                        _r.GatherErrorList("Process put data count not match upload data count.");

                        _logger.LogError("Process put data count not match upload data count.");

                    }
                }

                // darkPoolList
                if (darkPoolList.Count > 0)
                {
                    int i = 1;

                    char[] quotes = { '\"', ' ' };

                    try
                    {
                        using (var dataSession = new DataTransSession(_dbConn))
                        {
                            var dpTransRepo = new DarkpoolTransRepository(dataSession.Unit);
                            var dpRepo = new DarkpoolDashboardRepository(dataSession.Unit);
                            var dpStaticRepo = new DarkpoolValueCrossAvgRepository(dataSession.Unit);

                            foreach (var dp in darkPoolList)
                            {
                                var l = dp.Split(',', StringSplitOptions.None).Select(s => s.Trim(quotes).Replace("\\\"", "\"")).ToList();

                                var dpTran = new DarkpoolTrans();

                                dpTran = InjectDataToDarkpool(l);

                                int? rr = await dpTransRepo.AddAsync(dpTran);

                                if (rr.HasValue != true && rr.GetValueOrDefault() < 1)
                                {
                                    _r.GatherErrorList(string.Format("Ticker: {0}, Type: {1} Add Darkpool Trans to DB failure!", dpTran.Ticker, dpTran.Type));

                                    _r.IsValid = false;
                                }

                                i++;
                            }

                            // inject data to statistics when parse darkpool is successfully
                            if (_r.IsValid)
                            {
                                // 從 darkpool trans找個股，排除指數
                                var aboveAvgList = await GatherValueCrossAvgReport(dpStaticRepo, "IDX_ETF", 5);

                                if (aboveAvgList.Any())
                                {
                                    // fetch darkpool dasboard and update to statistics
                                    string transDate = aboveAvgList.FirstOrDefault().TransDate.ToStringOrDefault("yyyy-MM-dd");

                                    bool isExist = await CheckDarkpoolDashboardExistByDate(dpRepo, transDate);

                                    var ul = new List<DarkpoolValueCrossAvg>();

                                    bool isSuccess = false;

                                    if (isExist)
                                    {
                                        // select & update list
                                        foreach (var dpItem in aboveAvgList)
                                        {
                                            _logger.LogDebug($"duplicate Ticker :{dpItem.Ticker}");

                                            var dp = FetchDpBoardValueByTicker(dpRepo, dpItem.Ticker, dpItem.TransDate.ToStringOrDefault("yyyy-MM-dd"));

                                            if (dp != null)
                                            {
                                                dpItem.NetValue = dp.NetValue;
                                                dpItem.AvgNetValue = FetchDpAvgNetValueByTicker(dpRepo, dpItem.Ticker, dpItem.TransDate.ToStringOrDefault("yyyy-MM-dd"), 5);
                                            }

                                            ul.Add(dpItem);
                                        }

                                        isSuccess = await dpStaticRepo.AddRangeAsync(ul);
                                    }
                                    else
                                    {
                                        isSuccess = await dpStaticRepo.AddRangeAsync(aboveAvgList);
                                    }

                                    if (isSuccess != true)
                                    {
                                        _r.GatherErrorList(string.Format(" Add Darkpool Cross Avg to DB failure!"));

                                        _r.IsValid = false;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Add darkpool transaction File into DB Error :{ex}");

                        _r.IsValid = false;

                        _r.GatherErrorList("Add darkpool transaction File into DB Error.");

                        throw ex;
                    }

                    if (darkPoolList.Count != i)
                    {
                        _r.IsValid = false;

                        _r.GatherErrorList("Process darkpool transaction data count not match upload data count.");

                        _logger.LogError("Process darkpool transaction data count not match upload data count.");
                    }
                }

                // darkPoolDashboard
                if (dpDashboardList.Count > 0)
                {
                    int i = 1;

                    char[] quotes = { '\"', ' ' };

                    try
                    {
                        using (var dataSession = new DataTransSession(_dbConn))
                        {
                            var dpRepo = new DarkpoolDashboardRepository(dataSession.Unit);
                            var avgRepo = new DarkpoolValueCrossAvgRepository(dataSession.Unit);

                            var dpl = new List<DarkpoolDashboard>();

                            foreach (var dp in dpDashboardList)
                            {
                                var l = dp.Split(',', StringSplitOptions.None).Select(s => s.Trim(quotes).Replace("\\\"", "\"")).ToList();

                                var dpBoard = new DarkpoolDashboard();

                                dpBoard = InjectDataToDarkpoolDashboard(l);

                                int? rr = await dpRepo.AddAsync(dpBoard);

                                if (rr.HasValue != true && rr.GetValueOrDefault() < 1)
                                {
                                    _r.GatherErrorList(string.Format("Ticker: {0}, Add DarkpoolDashboard Trans to DB failure!", dpBoard.Ticker));

                                    _r.IsValid = false;
                                }
                                else
                                {
                                    dpl.Add(dpBoard);
                                }

                                i++;
                            }

                            // final : update statistics table
                            if (_r.IsValid)
                            {
                                bool isExist = await CheckDarkpoolStatisticExistByDate(avgRepo, fileTransDt.ToStringOrDefault("yyyy-MM-dd"));

                                if (isExist)
                                {
                                    // update
                                    var avgl = new List<DarkpoolValueCrossAvg>();

                                    foreach (var dpi in dpl)
                                    {
                                        var avg = FetchStaticsEntityByTicker(avgRepo, dpi.Ticker, dpi.TransDate.ToStringOrDefault("yyyy-MM-dd"));

                                        if (avg != null)
                                        {
                                            avg.DateValue = dpi.TotalAmt;
                                            avg.NetValue = dpi.NetValue;
                                            avg.AvgNetValue = FetchDpAvgNetValueByTicker(dpRepo, dpi.Ticker, dpi.TransDate.ToStringOrDefault("yyyy-MM-dd"), 5);

                                            avgl.Add(avg);
                                        }
                                    }

                                    if (avgl.Any())
                                    {
                                        foreach (var avgi in avgl)
                                        {
                                            await avgRepo.UpdateAsync(avgi);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Add DarkpoolDashboard File into DB Error :{ex}");

                        _r.IsValid = false;

                        _r.GatherErrorList("Add DarkpoolDashboard File into DB Error.");

                        throw ex;
                    }

                    if (dpDashboardList.Count != i)
                    {
                        _r.IsValid = false;

                        _r.GatherErrorList("Process DarkpoolDashboard data count not match upload data count.");

                        _logger.LogError("Process DarkpoolDashboard data count not match upload data count.");
                    }
                }

                // stock quote (by ticker, one file one ticker)
                if (twStockQuoteList.Count > 0)
                {
                    int i = 1;

                    char[] quotes = { '\"', ' ' };

                    string ticker = string.Empty;
                    string transDate = string.Empty;

                    bool isUnDeterminedBidAsk = false;
                    int unDeterminedSize = 0;

                    try
                    {
                        using (var dataSession = new DataTransSession(_dbConn))
                        {
                            var qRepo = new StockQuoteRepository(dataSession.Unit);

                            var dpl = new List<StockQuoteTw>();

                            int cumAskSize = 0;
                            int cumBidSize = 0;

                            int? newlyQuoteId = 0;

                            var timeLine = new TimeSpan(13, 30, 0);

                            foreach (var sq in twStockQuoteList)
                            {
                                var l = sq.Split(',', StringSplitOptions.None).Select(s => s.Trim(quotes).Replace("\\\"", "\"")).ToList();

                                var quote = new StockQuoteTw();

                                quote = InjectDataToStockQuoteTw(tickerTW, l);

                                // 盤後交易不列入
                                if (quote.TransDate.GetValueOrDefault().TimeOfDay <= timeLine)
                                {
                                    // cumulate bid or ask size - 外盤價成交(第一以及最後一筆不計算)
                                    if (i > 1 && i < twStockQuoteList.Count && quote.LastPrice >= quote.AskPrice)
                                    {
                                        // update : 這邏輯先不要
                                        // 先處理上一筆無法判斷ask or bid 的狀況(就只回朔一筆)
                                        //if (isUnDeterminedBidAsk)
                                        //{
                                        //    var preQ = qRepo.FindById(newlyQuoteId.Value);
                                        //    int updateSize = preQ.CumulativeAsk + unDeterminedSize;

                                        //    _logger.LogDebug($"update Quote Id: {newlyQuoteId.Value}, CumulativeAsk : {updateSize} ");

                                        //    preQ.CumulativeAsk = updateSize;
                                        //    int? updateR = await qRepo.UpdateAsync(preQ);

                                        //    // 更新計量
                                        //    cumAskSize = updateSize;
                                        //    isUnDeterminedBidAsk = false;
                                        //}
                                        cumAskSize = cumAskSize + quote.Size;
                                    }

                                    if (i > 1 && i < twStockQuoteList.Count && quote.LastPrice <= quote.BidPrice)
                                    {
                                        // update : 這邏輯先不要
                                        // 先處理上一筆無法判斷ask or bid 的狀況(就只回朔一筆)
                                        //if (isUnDeterminedBidAsk)
                                        //{
                                        //    var preQ = qRepo.FindById(newlyQuoteId.Value);
                                        //    int updateSize = preQ.CumulativeBid + unDeterminedSize;

                                        //    _logger.LogDebug($"update Quote Id: {newlyQuoteId.Value}, CumulativeBid : {updateSize} ");

                                        //    preQ.CumulativeBid = updateSize;
                                        //    int? updateR = await qRepo.UpdateAsync(preQ);

                                        //    // 更新計量
                                        //    cumBidSize = updateSize;
                                        //    isUnDeterminedBidAsk = false;
                                        //}
                                        cumBidSize = cumBidSize + quote.Size;
                                    }
                                    //else
                                    //{
                                    //    // 無法決定 ask or bid 的狀況(第一筆不計算)
                                    //    if (i > 1)
                                    //    {
                                    //        isUnDeterminedBidAsk = true;

                                    //        unDeterminedSize = quote.Size;
                                    //    }
                                    //}

                                    quote.CumulativeAsk = cumAskSize;
                                    quote.CumulativeBid = cumBidSize;

                                    // get the values from last record
                                    if (i == twStockQuoteList.Count - 1)
                                    {
                                        ticker = quote.Ticker;
                                        transDate = quote.TransDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                                    }

                                    newlyQuoteId = await qRepo.AddAsync(quote);

                                    if (newlyQuoteId.HasValue != true && newlyQuoteId.GetValueOrDefault() < 1)
                                    {
                                        _r.GatherErrorList($"Ticker: {tickerTW}, Add StockQuoteTw to DB failure!");

                                        _r.IsValid = false;
                                    }
                                    else
                                    {
                                        dpl.Add(quote);
                                    }

                                    i++;
                                }
                            }

                            // update statistics table
                            if (_r.IsValid)
                            {
                                // populate k bar
                                var kBarRepo = new TickerBarRepository(dataSession.Unit);

                                string kSql = $@"SELECT tsq.ticker as ticker, partgroup.hPrice as high, partgroup.lPrice as low, partgroup.size as volume, tsq.last as open, tsqq.last as close, partgroup.one_min_timestamp as k_timestamp FROM 
                                               (SELECT min(id) as minId, max(id) as maxId,max(last) as hPrice, min(last) as lPrice, sum(size) as size, date_trunc('hour', trans_date) + (((date_part('minute', trans_date)::integer / 1::integer) * 1::integer)|| ' minutes')::interval AS one_min_timestamp
                                                FROM tw_stock_quote WHERE ticker = @Ticker and TO_CHAR(trans_date, 'YYYY-MM-DD') = @TransDate GROUP BY one_min_timestamp) as partgroup 
                                                INNER JOIN tw_stock_quote as tsq ON partgroup.minId = tsq.id
                                                INNER JOIN tw_stock_quote as tsqq ON partgroup.maxId = tsqq.id
                                                WHERE tsq.ticker = @Ticker and TO_CHAR(tsq.trans_date, 'YYYY-MM-DD') = @TransDate and tsqq.ticker = @Ticker and TO_CHAR(tsqq.trans_date, 'YYYY-MM-DD') = @TransDate;
                                              ";

                                var aqParams = new DynamicParameters();
                                aqParams.Add("@Ticker", ticker);
                                aqParams.Add("@TransDate", transDate);

                                var l = (await kBarRepo.QueryBySqlAsync(kSql, aqParams)).ToList();

                                bool isKBar = false;

                                if (l != null && l.Any())
                                {
                                    isKBar = await kBarRepo.AddRangeAsync(l);
                                }

                                if (isKBar != true)
                                {
                                    _r.GatherErrorList("Populate K Bar Failed!");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Add Tw Quote File into DB Error :{ex}");

                        _r.IsValid = false;

                        _r.GatherErrorList("Add Tw Quote File into DB Error.");

                        throw ex;
                    }

                    if (dpDashboardList.Count != i)
                    {
                        _r.IsValid = false;

                        _r.GatherErrorList("Process DarkpoolDashboard data count not match upload data count.");

                        _logger.LogError("Process DarkpoolDashboard data count not match upload data count.");
                    }
                }
            }

            return _r;
        }

        public async Task<IEnumerable<UploadFile>> ReturnFileListAsync()
        {
            //var l = new List<UploadFile>();

            using (var dalSession = new DataTransSession(_dbConn))
            {
                var fileRepo = new UploadFileRepository(dalSession.Unit);

                return await fileRepo.GetAllAsync();
            }
        }

        public IEnumerable<UploadFile> ReturnFileList()
        {
            //var l = new List<UploadFile>();

            using (DataTransSession dalSession = new DataTransSession(_dbConn))
            {
                var fileRepo = new UploadFileRepository(dalSession.Unit);

                var l = fileRepo.GetAll();

                return l;
            }
        }
        #endregion

        #region private & helper method
        private OptionDashboard InjectDataToOption(string type, List<string> l)
        {
            var putCall = new OptionDashboard();

            putCall.Ticker = l[0].Trim();
            putCall.Type = type;

            string strPropor = l[1].Trim().Remove(l[1].Trim().Length - 1);
            putCall.Proportion = Convert.ToDouble(strPropor) / 100;

            putCall.Orders = Convert.ToInt32(l[2].Trim());
            putCall.Buys = Convert.ToDouble(l[3].Trim());
            putCall.AtAsk = l[4].Trim().ConvertStrAbbrToNumeric();
            putCall.AtBid = l[5].Trim().ConvertStrAbbrToNumeric();
            putCall.NetPrems = l[6].Trim().ConvertStrAbbrToNumeric();

            string strPremChg = l[7].Trim().Remove(l[7].Trim().Length - 1);
            putCall.PremiumsChange = Convert.ToDouble(strPremChg);

            putCall.AvgExpiredDate = Convert.ToDouble(l[8].Trim());

            string strOtm = l[9].Trim().Remove(l[9].Trim().Length - 1);
            putCall.Otm = Convert.ToDouble(strOtm);

            putCall.OtmScore = Convert.ToDouble(l[10].Trim());
            putCall.Unusual = Convert.ToDouble(l[11].Trim());
            putCall.SpotChange = Convert.ToDouble(l[12].Trim());
            putCall.Iv = Convert.ToDouble(l[13].Trim());
            putCall.TransDate = fileTransDt;
            putCall.CreatedDate = DateTime.Now.ToLocalTime();

            return putCall;
        }

        private DarkpoolTrans InjectDataToDarkpool(List<string> l)
        {
            var dp = new DarkpoolTrans();

            dp.Ticker = l[1].Trim();
            dp.Type = l[2].Trim();
            dp.Price = Convert.ToDouble(l[3].Trim());
            dp.Size = l[4].Trim().ConvertStrAbbrToNumeric();
            dp.Value = l[5].Trim().ConvertStrAbbrToNumeric();
            dp.Filled = l[6].Trim();
            dp.Sector = l[7].Trim();
            dp.Proportion = Convert.ToDouble(l[8].Trim());

            // transaction date : raw text -> 02-02 17:43:52.287
            // so ... we need to add year -> 2023-02-02 17:43:52.287
            //string transDate = string.Format("{0}-{1}", DateTime.Now.Year, l[0].Trim());
            //dp.TransDate = transDate.ToDateTime("yyyy-mm-dd HH:mm:ss", CultureInfo.InvariantCulture);
            dp.TransDate = fileTransDt;

            dp.CreatedDate = DateTime.Now.ToLocalTime();

            return dp;
        }

        private DarkpoolDashboard InjectDataToDarkpoolDashboard(List<string> l)
        {
            var dp = new DarkpoolDashboard();

            dp.Ticker = l[0].Trim();
            dp.Sentiment = l[1].Trim();
            dp.TotalAmt = l[2].Trim().ConvertStrAbbrToNumeric();

            string amtChange = l[3].Trim().Remove(l[3].Trim().Length - 1);
            dp.DailyAmtChange = Convert.ToDouble(amtChange);

            dp.AtAsk = l[4].Trim().ConvertStrAbbrToNumeric();
            dp.AtBid = l[5].Trim().ConvertStrAbbrToNumeric();
            dp.NetValue = l[6].Trim().ConvertStrAbbrToNumeric();
            dp.DpTrades = Convert.ToInt32(l[7].Trim());
            dp.BlockTrades = Convert.ToInt32(l[8].Trim());

            // transaction date : raw text -> 02-02 17:43:52.287
            // so ... we need to add year -> 2023-02-02 17:43:52.287
            //string transDate = string.Format("{0}-{1}", DateTime.Now.Year, l[0].Trim());
            //dp.TransDate = transDate.ToDateTime("yyyy-mm-dd HH:mm:ss", CultureInfo.InvariantCulture);
            dp.TransDate = fileTransDt;

            dp.CreatedDate = DateTime.Now.ToLocalTime();

            return dp;
        }

        /// <summary>
        /// rowNo 用來判斷是否為第一筆或是最後一筆
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="l"></param>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        private StockQuoteTw InjectDataToStockQuoteTw(string ticker, List<string> l)
        {
            var sq = new StockQuoteTw();

            sq.Ticker = ticker;

            if (string.IsNullOrEmpty(l[1].Trim()))
            {
                sq.BidPrice = 0;
            }
            else
            {
                sq.BidPrice = Convert.ToDouble(l[1].Trim());
            }

            if (string.IsNullOrEmpty(l[2].Trim()))
            {
                sq.AskPrice = 0;
            }
            else
            {
                sq.AskPrice = Convert.ToDouble(l[2].Trim());
            }

            if (string.IsNullOrEmpty(l[3].Trim()))
            {
                sq.LastPrice = 0;
            }
            else
            {
                sq.LastPrice = Convert.ToDouble(l[3].Trim());
            }

            sq.NetChange = Convert.ToDouble(l[4].Trim());
            sq.Size = Convert.ToInt32(l[5].Trim());
            sq.CumulativeVol = Convert.ToInt32(l[6].Trim());

            string strTransDate = $"{fileTransDt.GetValueOrDefault().ToString("yyyy-MM-dd")} {l[0].Trim()}";
            sq.TransDate = strTransDate.ToDateTime("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            return sq;
        }

        private DateTime? ParseDateValue(IXLCell cell)
        {
            // 如果是 closedxml 裡定義的 DateTime 型態，就回傳 value
            if (cell.DataType == XLDataType.DateTime)
            {
                return cell.GetDateTime();
            }

            var dateString = cell.GetString().Trim();

            DateTime d;

            var dateFormats = "yyyy/M/d,yyyyMMdd,yyyy/MM/dd,yyyy.M.d,yyyy/MM/dd HH:mm:ss".Split(',');

            foreach (var fmt in dateFormats)
            {
                if (DateTime.TryParseExact(dateString, fmt, null,
                    DateTimeStyles.None, out d))
                {
                    return d;
                }
            }

            return null;
        }

        /// <summary>
        /// for blue's request
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        //private async Task<List<StrategyResult>> FetchPriceResult(List<StrategyResult> l)
        //{
        //    var finHelper = new HttpClientHelper();

        //    string finToken = await finHelper.FinmindSignIn();

        //    var resultL = new List<StrategyResult>();

        //    foreach (var strate in l)
        //    {
        //        var stockResponse = new FinmindStockPriceResponse();

        //        string transDate = strate.SellTime.GetValueOrDefault().ToString("yyyy-MM-dd");

        //        stockResponse = await finHelper.FetchStockPriceTickFromFinmind(finToken, strate.Ticker, transDate);

        //        var buyResult = new List<FinmindStockPriceEntity>();

        //        buyResult = (from buyPrice in stockResponse.StockPriceList
        //                     where DateTime.Parse($"{buyPrice.Date} {buyPrice.Time}").ToString("HH:mm:ss") == strate.BuyTime.GetValueOrDefault().ToString("HH:mm:ss")
        //                     select buyPrice).ToList();

        //        if (buyResult == null || buyResult.Any() != true)
        //        {
        //            buyResult = (from buyPrice in stockResponse.StockPriceList
        //                         where DateTime.Parse($"{buyPrice.Date} {buyPrice.Time}").ToString("HH:mm") == strate.BuyTime.GetValueOrDefault().ToString("HH:mm")
        //                         select buyPrice).ToList();
        //        }

        //        //_logger.LogDebug($"Buy : {buyResult}");

        //        var sellResult = new List<FinmindStockPriceEntity>();

        //        sellResult = (from sellPrice in stockResponse.StockPriceList
        //                      where DateTime.Parse($"{sellPrice.Date} {sellPrice.Time}").ToString("HH:mm:ss") == strate.SellTime.GetValueOrDefault().ToString("HH:mm:ss")
        //                      select sellPrice).ToList();

        //        if (sellResult == null || sellResult.Any() != true)
        //        {
        //            sellResult = (from sellPrice in stockResponse.StockPriceList
        //                          where DateTime.Parse($"{sellPrice.Date} {sellPrice.Time}").ToString("HH:mm") == strate.SellTime.GetValueOrDefault().ToString("HH:mm")
        //                          select sellPrice).ToList();
        //        }

        //        if (buyResult.FirstOrDefault() != null && sellResult.FirstOrDefault() != null)
        //        {
        //            strate.BuyPrice = buyResult.FirstOrDefault().DealPrice;
        //            strate.SellPrice = sellResult.FirstOrDefault().DealPrice;

        //            strate.NetBalance = sellResult.FirstOrDefault().DealPrice - buyResult.FirstOrDefault().DealPrice;
        //        }

        //        resultL.Add(strate);
        //    }

        //    return resultL;
        //}

        private async Task<List<DarkpoolValueCrossAvg>> GatherValueCrossAvgReport(DarkpoolValueCrossAvgRepository repo, string excludeType, int avgUnit)
        {
            string notExistSql = string.Empty;

            if (string.IsNullOrEmpty(excludeType) != true)
            {
                notExistSql = $" NOT EXISTS ( select 1 from asset_list al where dt.ticker = al.ticker and al.TYPE = '{excludeType}') and";
            }

            string reportSQl = @$"select tdt.ticker as ticker, tdt.value as date_value, round( cast(tdt.price as numeric), 2) as price, tdt.trans_date as trans_date, odt.avgvalue as avg_value, {avgUnit} as avg_unit
                               from 
                               (
                                 select ticker, avg(value) as value, avg(price) as price, trans_date::date from darkpool_trans where trans_date::date = (select max(trans_date)::date from darkpool_trans) group by ticker, trans_date::date
                               ) tdt
                               inner join (
                               select dt.ticker , sum(value)/4 as avgValue from darkpool_trans dt where
                               {notExistSql}
                               dt.trans_date between (( select max(trans_date)::date from darkpool_trans)  - INTERVAL '{avgUnit} DAY')::date and ((select max(trans_date)::date from darkpool_trans) - INTERVAL '1 DAY')::date group by dt.ticker ) odt
                               on tdt.ticker = odt.ticker where 
                                 tdt.trans_date::date = (select max(trans_date)::date from darkpool_trans)
                                 and tdt.value > (odt.avgValue + odt.avgValue*0.1) order by (tdt.value- odt.avgvalue )/100 desc;";

            var l = await repo.QueryBySqlAsync(reportSQl);

            return l.ToList();
        }

        private async Task<List<DarkpoolValueCrossAvg>> GatherValueCrossAvgReportFromDpBoard(DarkpoolValueCrossAvgRepository repo, string excludeType, int avgUnit)
        {
            string notExistSql = string.Empty;

            if (string.IsNullOrEmpty(excludeType) != true)
            {
                notExistSql = $" NOT EXISTS ( select 1 from asset_list al where upper(dt.ticker) = upper(al.ticker) and al.TYPE = '{excludeType}') and";
            }

            string reportSQl = @$"select tdt.ticker as ticker, tdt.value as date_value, round( cast(tdt.price as numeric), 2) as price, tdt.trans_date as trans_date, odt.avgvalue as avg_value, {avgUnit} as avg_unit
                               from 
                               (
                                 select ticker, avg(value) as value, avg(price) as price, trans_date::date from darkpool_trans where trans_date::date = (select max(trans_date)::date from darkpool_trans) group by ticker, trans_date::date
                               ) tdt
                               inner join (
                               select dt.ticker , sum(value)/4 as avgValue from darkpool_trans dt where
                               {notExistSql}
                               dt.trans_date between (( select max(trans_date)::date from darkpool_trans)  - INTERVAL '{avgUnit} DAY')::date and ((select max(trans_date)::date from darkpool_trans) - INTERVAL '1 DAY')::date group by dt.ticker ) odt
                               on tdt.ticker = odt.ticker where 
                                 tdt.trans_date::date = (select max(trans_date)::date from darkpool_trans)
                                 and tdt.value > (odt.avgValue + odt.avgValue*0.1) order by (tdt.value- odt.avgvalue )/100 desc;";

            var l = await repo.QueryBySqlAsync(reportSQl);

            return l.ToList();
        }

        private static async Task<bool> CheckDarkpoolDashboardExistByDate(DarkpoolDashboardRepository dpRepo, string dateStr)
        {
            bool isExist = false;

            string sql = $@"SELECT * FROM darkpool_dashboard WHERE trans_date::date = '{dateStr}' LIMIT 1";

            var l = await dpRepo.QueryBySqlAsync(sql);

            if (l != null && l.Any())
            {
                isExist = true;
            }

            return isExist;
        }

        private static async Task<bool> CheckDarkpoolStatisticExistByDate(DarkpoolValueCrossAvgRepository dpRepo, string dateStr)
        {
            bool isExist = false;

            string sql = $@"SELECT * FROM darkpool_cross_average WHERE trans_date::date = '{dateStr}' LIMIT 1";

            var l = await dpRepo.QueryBySqlAsync(sql);

            if (l != null && l.Any())
            {
                isExist = true;
            }

            return isExist;
        }

        private static DarkpoolDashboard FetchDpBoardValueByTicker(DarkpoolDashboardRepository dpRepo, string ticker, string tranDate)
        {
            string sql = $@"SELECT * FROM darkpool_dashboard WHERE ticker = '{ticker}' AND trans_date::date = '{tranDate}'";

            //var dp = dpRepo.QuerySingle(sql);
            var dp = dpRepo.QueryBySql(sql).FirstOrDefault();

            return dp;
        }

        private static double FetchDpAvgNetValueByTicker(DarkpoolDashboardRepository dpRepo, string ticker, string tranDate, int avgDateCount)
        {
            string sql = $@"SELECT ROUND(AVG(cast(net_value as numeric)),2) as NetValue FROM darkpool_dashboard WHERE ticker = '{ticker}' 
                            AND trans_date::date BETWEEN (TO_DATE('{tranDate}','YYYY-MM-DD') - INTERVAL '{avgDateCount} DAY')::date AND (TO_DATE('{tranDate}','YYYY-MM-DD') - INTERVAL '1 DAY')::date ";

            object dp = dpRepo.QuerySingleValue(sql);

            return Convert.ToDouble(dp);
        }

        private static DarkpoolValueCrossAvg FetchStaticsEntityByTicker(DarkpoolValueCrossAvgRepository dpRepo, string ticker, string tranDate)
        {
            string sql = $@"SELECT * FROM darkpool_cross_average WHERE ticker = '{ticker}' AND trans_date::date = '{tranDate}'";

            var avg = dpRepo.QuerySingle(sql);

            return avg;
        }
        #endregion
    }
}
