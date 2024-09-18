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
using FluentDateTime;
using TDAmeritrade;
using StarStocks.Core.Extensions;
using StarStocks.Core.Interfaces;
using StarStocks.Core.Helpers;
using StarStocks.Core.DbWrapper;
using StarStocks.Core.Repositories;
using StarStocks.Core.UnitOfWork;
using StarStocks.Core.Models;

namespace StarStocks.Core.Managers
{
    /// <summary>
    /// only for index : SPX, SPY, QQQ, DIA, IWM
    /// </summary>
    public sealed class IndexOptionChainCaculateManager
    {
        private readonly ILogger<IndexOptionChainCaculateManager> _logger;

        private static DbConnection _dbConn;

        public IndexOptionChainCaculateManager(ILogger<IndexOptionChainCaculateManager> logger, DbConnection dbConn)
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

        #region SPX
        public List<SpotGamma> SpxSpotGammaList { get; set; }

        public List<int> SpxUnderlying { get; set; }

        public Dictionary<int, List<OptionChainDaily>> SpxOpChainByStrike { get; set; }

        /// <summary>
        /// strike : oi
        /// </summary>
        public Dictionary<int, int> SpxCallOIByStrikeInCurrent { get; set; }

        public Dictionary<int, int> SpxPutOIByStrikeInCurrent { get; set; }

        public Dictionary<int, int> SpxCallOIByStrikeInPrevious { get; set; }

        public Dictionary<int, int> SpxPutOIByStrikeInPrevious { get; set; }

        public Dictionary<int, int> SpxCallOIMoreThan10Percent { get; private set; }

        public Dictionary<int, int> SpxPutOIMoreThan10Percent { get; private set; }

        public Dictionary<int, int> SpxCallOILessThan10Percent { get; private set; }

        public Dictionary<int, int> SpxPutOILessThan10Percent { get; private set; }

        /// <summary>
        /// TODO : clear all container
        /// </summary>
        public void SpxClearAll()
        {

        }

        /// <summary>
        /// Gamma Exposure = Unit Gamma * Open Interest * Contract Size * Spot Price 
        /// To further convert into 'per 1% move' quantity, multiply by 1% of spotPrice
        /// CallGEX = CallGamma * CallOI * 100 * spotPrice * spotPrice * 0.01
        /// PutGEX = PutGamma * PutOI * 100 * spotPrice * spotPrice * 0.01 * -1
        /// TotalGamma = (CallGEX + PutGEX) / 10**9
        /// </summary>
        /// <param name="i"></param>
        public void CalculateSpxSpotGamma(OptionChainDaily opChain)
        {

        }


        #endregion

        #region SPY
        public Dictionary<int, List<OptionChainDaily>> SpyOpChainByStrike { get; set; }

        public Dictionary<int, int> SpyCurrentOIByStrike { get; set; }

        public Dictionary<int, int> SpyPreviousOIByStrike { get; set; }

        public Dictionary<int, int> SpyOiMoreThan10Percent { get; private set; }

        public Dictionary<int, int> SpyOiLessThan10Percent { get; private set; }
        #endregion

    }

    public sealed class BarSeriesCaculateManager
    {
        private readonly ILogger<BarSeriesCaculateManager> _logger;

        private static DbConnection _dbConn;

        public BarSeriesCaculateManager(ILogger<BarSeriesCaculateManager> logger, DbConnection dbConn)
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
    }
}
