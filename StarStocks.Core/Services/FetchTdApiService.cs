// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarStocks.Core.Extensions;
using StarStocks.Core.Interfaces;
using StarStocks.Core.Helpers;
using StarStocks.Core.DbWrapper;
using StarStocks.Core.Managers;
using StarStocks.Core.Repositories;
using StarStocks.Core.UnitOfWork;
using StarStocks.Core.Models;
using TDAmeritrade;

namespace StarStocks.Core.Services
{
    public class FetchTdApiService : IService<OptionChainDaily>
    {
        private readonly ILogger<FetchTdApiService> _logger;

        private readonly DbConnection _dbConn;

        private IResult _r;

        private IndexOptionChainCaculateManager _idxOpManager;

        public FetchTdApiService(ILogger<FetchTdApiService> logger, DbConnection dbConn, IResult r, IndexOptionChainCaculateManager opManager)
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

            _idxOpManager = opManager;
        }

        #region IService
        public async Task<IResult> DoService(Dictionary<string, string> pDict)
        {
            // reset result container
            _r.Reset();

            string projectRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //_logger.LogDebug($"Dll path : {projectRootPath}");
            // get today's date
            //var estDateStr = DateTime.UtcNow.Date.ToEST().AddDays(1).ToString("yyyy-MM-dd");

            //_logger.LogDebug($"EST time : {estDateStr} ");

            var cache = new TDUnprotectedCache(projectRootPath);
            var client = new TDAmeritradeClient(cache);

            if (pDict != null && pDict.Count > 0)
            {
                await client.SignIn();

                if (client.IsSignedIn)
                {
                    var opReq = new TDOptionChainRequest();

                    string symbol = string.Empty;

                    if (pDict.TryGetValue("symbol", out symbol))
                    {
                        opReq.symbol = symbol.ToUpper();
                    }
                    else
                    {
                        opReq.symbol = "SPY";
                    }

                    string strikeCountStr = string.Empty;
                    int? strikeCount = 0;

                    if (pDict.TryGetValue("strikeCount", out strikeCountStr))
                    {
                        strikeCount = Convert.ToInt32(strikeCountStr);

                        opReq.strikeCount = strikeCount;
                    }
                    else
                    {
                        opReq.strikeCount = 8;
                    }

                    string toDate = string.Empty;

                    DateTime? dtToDate = null;

                    if (pDict.TryGetValue("toDate", out toDate))
                    {
                        dtToDate = DateTime.Parse(toDate).Date.ToUniversalTime().ToEST().AddDays(1);

                        opReq.toDate = dtToDate;
                    }
                    else
                    {
                        opReq.toDate = DateTime.UtcNow.Date.ToEST().AddDays(1);
                    }

                    //get option chain stream
                    //var opChain = await client.GetOptionsChain(opReq);

                    //if (opChain != null && opChain.status.ToUpper() == "SUCCESS")
                    //{
                    //    TODO : 先計算 gex
                    //    InjectToDb(opChain);
                    //}

                    // get option chain json
                    var jPath = $"{projectRootPath}/Json/{symbol}.{DateTime.UtcNow.ToEST().ToString("yyyy-MM-dd_HH.mm.ss")}.json";

                    if (!File.Exists(jPath)) { using (var s = File.Create(jPath)) { } }

                    var opChainJson = await client.GetOptionsChainJson(opReq);

                    if (opChainJson != null && string.IsNullOrEmpty(opChainJson) != true)
                    {
                        lock (jPath)
                        {
                            using (var s = File.AppendText(jPath))
                            {
                                s.WriteLine(opChainJson);
                            }
                        }
                    }

                    // consume indexoptionmanager
                    // _idxOpManager.CalculateSpxSpotGamma(1);
                }
                else
                {
                    _r.IsValid = false;
                    _r.Message = "TdAmeritrade Api Signed In Failed!";
                }
            }
            else
            {
                _r.IsValid = false;
                _r.Message = "FileService parameter is null!";
            }

            return _r;
        }

        public IResult ReturnServiceResult()
        {
            if (_r == null)
            {
                _r = new ResultContainer(true);
            }

            return _r;
        }
        #endregion

        #region Private & Custom method
        private void InjectToDb(TDOptionChain opChain)
        {
            string ticker = opChain.symbol;
            string strategy = opChain.strategy;
            double underlyingPrice = opChain.underlyingPrice;
            double underlyingIv = opChain.volatility;

            try
            {
                using (var dataSession = new DataTransSession(_dbConn))
                {
                    // store data into DB
                    var opChainRepo = new OptionChainDailyRepository(dataSession.Unit);

                    // seperate call & put, detect and notify
                    foreach (var callExpItem in opChain.callExpDateMap)
                    {
                        foreach (var item in callExpItem.options)
                        {
                            var opChainDaily = new OptionChainDaily();

                            opChainDaily.Ticker = ticker;
                            opChainDaily.Strategy = strategy;
                            opChainDaily.OptionSymbol = item.symbol;
                            opChainDaily.UnderlyingPrice = underlyingPrice;
                            opChainDaily.UnderlyingIv = underlyingIv;

                            if (item.putCall.ToUpper() == "CALL")
                            {
                                opChainDaily.Type = "C";
                            }
                            else
                            {
                                opChainDaily.Type = "P";
                            }

                            opChainDaily.Strike = item.strikePrice;
                            opChainDaily.OpenInterest = item.openInterest;
                            opChainDaily.ExpireDate = item.expirationDate.UnixTimeMillisecondsToUtcDatetime().ToEST();
                            opChainDaily.Dte = item.daysToExpiration;
                            opChainDaily.AskPrice = item.askPrice;
                            opChainDaily.AskSize = item.askSize;
                            opChainDaily.BidPrice = item.bidPrice;
                            opChainDaily.BidSize = item.bidSize;
                            opChainDaily.LastPrice = item.lastPrice;
                            opChainDaily.LastSize = item.lastSize;
                            opChainDaily.MarkPrice = item.markPrice;
                            opChainDaily.NetChange = item.netChange;
                            opChainDaily.TimeValue = item.timeValue;
                            opChainDaily.Iv = item.volatility;
                            opChainDaily.Delta = item.delta;
                            opChainDaily.Gamma = item.gamma;
                            opChainDaily.Vega = item.vega;
                            opChainDaily.Rho = item.rho;
                            opChainDaily.Theta = item.theta;
                            opChainDaily.TotalVolume = item.totalVolume;
                            opChainDaily.TransDate = item.tradeTimeInLong.UnixTimeMillisecondsToUtcDatetime().ToEST();
                            //_logger.LogDebug($"Trade Time: {item.tradeTimeInLong.UnixTimeMillisecondsToUtcDatetime().ToEST()}");
                            opChainDaily.CreatedDate = DateTime.Now.ToLocalTime();

                            int? rr = opChainRepo.Add(opChainDaily);

                            if (rr.GetValueOrDefault() < 1) _r.IsValid = false;
                        }
                    }

                    foreach (var putExpItem in opChain.putExpDateMap)
                    {
                        foreach (var item in putExpItem.options)
                        {
                            var opChainDaily = new OptionChainDaily();

                            opChainDaily.Ticker = ticker;
                            opChainDaily.Strategy = strategy;
                            opChainDaily.OptionSymbol = item.symbol;
                            opChainDaily.UnderlyingPrice = underlyingPrice;
                            opChainDaily.UnderlyingIv = underlyingIv;

                            if (item.putCall.ToUpper() == "PUT")
                            {
                                opChainDaily.Type = "P";
                            }
                            else
                            {
                                opChainDaily.Type = "C";
                            }

                            opChainDaily.Strike = item.strikePrice;
                            opChainDaily.OpenInterest = item.openInterest;
                            opChainDaily.ExpireDate = item.expirationDate.UnixTimeMillisecondsToUtcDatetime().ToEST();
                            opChainDaily.Dte = item.daysToExpiration;
                            opChainDaily.AskPrice = item.askPrice;
                            opChainDaily.AskSize = item.askSize;
                            opChainDaily.BidPrice = item.bidPrice;
                            opChainDaily.BidSize = item.bidSize;
                            opChainDaily.LastPrice = item.lastPrice;
                            opChainDaily.LastSize = item.lastSize;
                            opChainDaily.MarkPrice = item.markPrice;
                            opChainDaily.NetChange = item.netChange;
                            opChainDaily.TimeValue = item.timeValue;
                            opChainDaily.Iv = item.volatility;
                            opChainDaily.Delta = item.delta;
                            opChainDaily.Gamma = item.gamma;
                            opChainDaily.Vega = item.vega;
                            opChainDaily.Rho = item.rho;
                            opChainDaily.Theta = item.theta;
                            opChainDaily.TotalVolume = item.totalVolume;
                            opChainDaily.TransDate = item.tradeTimeInLong.UnixTimeMillisecondsToUtcDatetime().ToEST();
                            opChainDaily.CreatedDate = DateTime.Now.ToLocalTime();

                            int? rr = opChainRepo.Add(opChainDaily);

                            if (rr.GetValueOrDefault() < 1) _r.IsValid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"OptionChain Inject To DB Error : {ex}");

                _r.IsValid = false;

                _r.GatherErrorList("OptionChain Inject To DB Error.");

                throw ex;
            }
        }
        #endregion
    }
}
