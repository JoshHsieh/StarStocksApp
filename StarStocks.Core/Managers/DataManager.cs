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
    /// for summarize
    /// </summary>
    public sealed class OptionDataManager
    {
        private readonly ILogger<OptionDataManager> _logger;

        private static DbConnection _dbConn;

        public OptionDataManager(ILogger<OptionDataManager> logger, DbConnection dbConn)
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

        #region RealTime SPX price
        private double _realTimeSpx;

        public double RealTimeSpx
        {
            get
            {
                ResetRealTimeSpx();

                return _realTimeSpx;
            }
        }

        public void ResetRealTimeSpx()
        {
            _realTimeSpx = 0;

            try
            {
                _realTimeSpx = GetRealtimePriceFromOptionChain("$SPX.X");
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeRealTimeSpx Error :{0}", ex);

                throw ex;
            }
        }
        #endregion

        #region OpWithTickerLast30Days
        private List<OptionDashboard> _opWithTickerLast30Days;

        public List<OptionDashboard> OpWithTickerLast30Days
        {
            get
            {
                if (_opWithTickerLast30Days != null)
                {
                    return _opWithTickerLast30Days;
                }

                ResetOpWithTickerLast30Days();

                return _opWithTickerLast30Days;
            }
        }

        public void ResetOpWithTickerLast30Days()
        {
            if (_opWithTickerLast30Days == null)
            {
                _opWithTickerLast30Days = new List<OptionDashboard>();
            }

            _opWithTickerLast30Days.Clear();

            try
            {
                _opWithTickerLast30Days = MakeOpWithTickerLast30Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeOpWithTickerLast30Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<OptionDashboard> MakeOpWithTickerLast30Days()
        {
            var l = new List<OptionDashboard>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var optionRepo = new OptionDashboardRepository(dataSession.Unit);

                string sql = string.Format("select * from option_dashboard where trans_date >= '{0}'", DateTime.Now.AddBusinessDays(-30).ToString("yyyy-MM-dd"));

                l = optionRepo.QueryBySql(sql, null).ToList();
            }

            return l;
        }
        #endregion

        #region OpWithHighVolLast30Days
        private List<HighVolOption> _opWithHighVolsLast30Days;

        public List<HighVolOption> OpWithHighVolsLast30Days
        {
            get
            {
                if (_opWithHighVolsLast30Days != null)
                {
                    return _opWithHighVolsLast30Days;
                }

                ResetOpWithHighVolsLast30Days();

                return _opWithHighVolsLast30Days;
            }
        }

        public void ResetOpWithHighVolsLast30Days()
        {
            if (_opWithHighVolsLast30Days == null)
            {
                _opWithHighVolsLast30Days = new List<HighVolOption>();
            }

            _opWithHighVolsLast30Days.Clear();

            try
            {
                _opWithHighVolsLast30Days = MakeOpWithHighVolsLast30Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeOpWithHighVolsLast30Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<HighVolOption> MakeOpWithHighVolsLast30Days()
        {
            var l = new List<HighVolOption>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var optionRepo = new HighVolOptionRepository(dataSession.Unit);

                string sql = string.Format("select * from option_highvol where trans_date >= '{0}'", DateTime.Now.AddBusinessDays(-30).ToString("yyyy-MM-dd"));

                l = optionRepo.QueryBySql(sql, null).ToList();
            }

            return l;
        }
        #endregion

        #region SPX 0dte
        private List<OptionChainDaily> _spxOdte;

        public List<OptionChainDaily> Spx0dte
        {
            get
            {
                if (_spxOdte != null)
                {
                    return _spxOdte;
                }

                ResetSpx0dte();

                return _spxOdte;
            }
        }

        public void ResetSpx0dte()
        {
            if (_spxOdte == null)
            {
                _spxOdte = new List<OptionChainDaily>();
            }

            _spxOdte.Clear();

            try
            {
                _spxOdte = MakeOption0dte("$SPX.X").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeSpx0dte Error :{0}", ex);

                throw ex;
            }
        }

        #endregion

        #region SPY 0dte
        private List<OptionChainDaily> _spyOdte;

        public List<OptionChainDaily> Spy0dte
        {
            get
            {
                if (_spyOdte != null)
                {
                    return _spyOdte;
                }

                ResetSpy0dte();

                return _spyOdte;
            }
        }

        public void ResetSpy0dte()
        {
            if (_spyOdte == null)
            {
                _spyOdte = new List<OptionChainDaily>();
            }

            _spyOdte.Clear();

            try
            {
                _spyOdte = MakeOption0dte("SPY").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeSpy0dte Error :{0}", ex);

                throw ex;
            }
        }

        #endregion

        #region QQQ 0dte
        private List<OptionChainDaily> _qqqOdte;

        public List<OptionChainDaily> Qqq0dte
        {
            get
            {
                if (_qqqOdte != null)
                {
                    return _qqqOdte;
                }

                ResetQqq0dte();

                return _qqqOdte;
            }
        }

        public void ResetQqq0dte()
        {
            if (_qqqOdte == null)
            {
                _qqqOdte = new List<OptionChainDaily>();
            }

            _qqqOdte.Clear();

            try
            {
                _qqqOdte = MakeOption0dte("QQQ").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeQqq0dte Error :{0}", ex);

                throw ex;
            }
        }

        #endregion

        #region IWM 0dte
        private List<OptionChainDaily> _iwmOdte;

        public List<OptionChainDaily> Iwm0dte
        {
            get
            {
                if (_iwmOdte != null)
                {
                    return _iwmOdte;
                }

                ResetIwm0dte();

                return _iwmOdte;
            }
        }

        public void ResetIwm0dte()
        {
            if (_iwmOdte == null)
            {
                _iwmOdte = new List<OptionChainDaily>();
            }

            _iwmOdte.Clear();

            try
            {
                _iwmOdte = MakeOption0dte("IWM").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeIwm0dte Error :{0}", ex);

                throw ex;
            }
        }

        #endregion

        #region DIA 0dte
        private List<OptionChainDaily> _diaOdte;

        public List<OptionChainDaily> Dia0dte
        {
            get
            {
                if (_diaOdte != null)
                {
                    return _diaOdte;
                }

                ResetDia0dte();

                return _diaOdte;
            }
        }

        public void ResetDia0dte()
        {
            if (_diaOdte == null)
            {
                _diaOdte = new List<OptionChainDaily>();
            }

            _diaOdte.Clear();

            try
            {
                _diaOdte = MakeOption0dte("DIA").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeDia0dte Error :{0}", ex);

                throw ex;
            }
        }

        #endregion
        #endregion

        #region Calculate & Fetch Data
        /// <summary>
        /// Calculate Zero Gamma value, default is 0dte option
        /// ticker only allow SPX, SPY, QQQ, DIA, IWM
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="daysToExpire"></param>
        /// <returns></returns>
        public Dictionary<string, double> CalculateZeroGamma(string ticker, int daysToExpire)
        {
            var d = new Dictionary<string, double>();

            return d;
        }
        #endregion

        #region private method

        private static IEnumerable<OptionChainDaily> MakeOption0dte(string ticker)
        {
            var l = new List<OptionChainDaily>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var optionRepo = new OptionChainDailyRepository(dataSession.Unit);

                // get today's date(eastern time)
                var estDateStr = DateTime.UtcNow.Date.ToEST().AddDays(1).ToString("yyyy-MM-dd");

                string sql = $"select * from option_chain_daily where ticker = '{ticker}' and expire_date < '{estDateStr}'";

                l = optionRepo.QueryBySql(sql, null).ToList();
            }

            return l;
        }

        /// <summary>
        /// Get Last Price
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        private static double GetRealtimePriceFromOptionChain(string ticker)
        {
            double rPrice = 0;

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var optionRepo = new OptionChainDailyRepository(dataSession.Unit);

                string sql = $"select underlying_price from option_chain_daily where ticker = '{ticker.ToUpper()}' ORDER BY id desc limit 1";

                rPrice = optionRepo.QuerySinglePriceValue(sql);
            }

            return rPrice;
        }
        #endregion
    }

    public sealed class DarkpoolManager
    {
        private readonly ILogger<DarkpoolManager> _logger;

        private static DbConnection _dbConn;

        public DarkpoolManager(ILogger<DarkpoolManager> logger, DbConnection dbConn)
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
        #region BlockTradeWithTickerLast30Days(BlockTrade -> bt)
        private List<DarkpoolTrans> _btWithTickerLast7Days;

        public List<DarkpoolTrans> BtWithTickerLast7Days
        {
            get
            {
                if (_btWithTickerLast7Days != null)
                {
                    return _btWithTickerLast7Days;
                }

                ResetBtWithTickerLast7Days();

                return _btWithTickerLast7Days;
            }
        }

        public void ResetBtWithTickerLast7Days()
        {
            if (_btWithTickerLast7Days == null)
            {
                _btWithTickerLast7Days = new List<DarkpoolTrans>();
            }

            _btWithTickerLast7Days.Clear();

            try
            {
                _btWithTickerLast7Days = MakeBtWithTickerLast30Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeBtWithTickerLast30Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<DarkpoolTrans> MakeBtWithTickerLast30Days()
        {
            var l = new List<DarkpoolTrans>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var dpRepo = new DarkpoolTransRepository(dataSession.Unit);

                string sql = string.Format("select * from option_dashboard where trans_date >= '{0}'", DateTime.Now.AddBusinessDays(-30).ToString("yyyy-MM-dd"));

                l = dpRepo.QueryBySql(sql, null).ToList();
            }

            return l;
        }
        #endregion

        #region DarkpoolWithSpyLast10Days(Darkpool -> dp)
        private List<DarkpoolTrans> _dpWithSpyLast10Days;

        public List<DarkpoolTrans> DpWithSpyLast10Days
        {
            get
            {
                if (_dpWithSpyLast10Days != null)
                {
                    return _dpWithSpyLast10Days;
                }

                ResetDpWithSpyLast10Days();

                return _dpWithSpyLast10Days;
            }
        }

        public void ResetDpWithSpyLast10Days()
        {
            if (_dpWithSpyLast10Days == null)
            {
                _dpWithSpyLast10Days = new List<DarkpoolTrans>();
            }

            _dpWithSpyLast10Days.Clear();

            try
            {
                _dpWithSpyLast10Days = MakeDpWithSpyLast10Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeDpWithSpyLast10Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<DarkpoolTrans> MakeDpWithSpyLast10Days()
        {
            var l = new List<DarkpoolTrans>();

            l = MakeDpWithTickerLastSpecificDays("SPY", 10).ToList();

            return l;
        }
        #endregion

        #region DarkpoolWithQQQLast10Days(Darkpool -> dp)
        private List<DarkpoolTrans> _dpWithQqqLast10Days;

        public List<DarkpoolTrans> DpWithQqqLast10Days
        {
            get
            {
                if (_dpWithQqqLast10Days != null)
                {
                    return _dpWithQqqLast10Days;
                }

                ResetDpWithQqqLast10Days();

                return _dpWithQqqLast10Days;
            }
        }

        public void ResetDpWithQqqLast10Days()
        {
            if (_dpWithQqqLast10Days == null)
            {
                _dpWithQqqLast10Days = new List<DarkpoolTrans>();
            }

            _dpWithQqqLast10Days.Clear();

            try
            {
                _dpWithQqqLast10Days = MakeDpWithQqqLast10Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeDpWithQqqLast10Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<DarkpoolTrans> MakeDpWithQqqLast10Days()
        {
            var l = new List<DarkpoolTrans>();

            l = MakeDpWithTickerLastSpecificDays("QQQ", 10).ToList();

            return l;
        }
        #endregion

        #region DarkpoolWithIWMLast10Days(Darkpool -> dp)
        private List<DarkpoolTrans> _dpWithIwmLast10Days;

        public List<DarkpoolTrans> DpWithIwmLast10Days
        {
            get
            {
                if (_dpWithIwmLast10Days != null)
                {
                    return _dpWithIwmLast10Days;
                }

                ResetDpWithIwmLast10Days();

                return _dpWithIwmLast10Days;
            }
        }

        public void ResetDpWithIwmLast10Days()
        {
            if (_dpWithIwmLast10Days == null)
            {
                _dpWithIwmLast10Days = new List<DarkpoolTrans>();
            }

            _dpWithIwmLast10Days.Clear();

            try
            {
                _dpWithIwmLast10Days = MakeDpWithIwmLast10Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeDpWithIwmLast10Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<DarkpoolTrans> MakeDpWithIwmLast10Days()
        {
            var l = new List<DarkpoolTrans>();

            l = MakeDpWithTickerLastSpecificDays("IWM", 10).ToList();

            return l;
        }
        #endregion

        #region DarkpoolWithDIALast10Days(Darkpool -> dp)
        private List<DarkpoolTrans> _dpWithDiaLast10Days;

        public List<DarkpoolTrans> DpWithDiaLast10Days
        {
            get
            {
                if (_dpWithDiaLast10Days != null)
                {
                    return _dpWithDiaLast10Days;
                }

                ResetDpWithDiaLast10Days();

                return _dpWithDiaLast10Days;
            }
        }

        public void ResetDpWithDiaLast10Days()
        {
            if (_dpWithDiaLast10Days == null)
            {
                _dpWithDiaLast10Days = new List<DarkpoolTrans>();
            }

            _dpWithDiaLast10Days.Clear();

            try
            {
                _dpWithIwmLast10Days = MakeDpWithDiaLast10Days().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeDpWithDiaLast10Days Error :{0}", ex);

                throw ex;
            }
        }

        private static IEnumerable<DarkpoolTrans> MakeDpWithDiaLast10Days()
        {
            var l = new List<DarkpoolTrans>();

            l = MakeDpWithTickerLastSpecificDays("DIA", 10).ToList();

            return l;
        }
        #endregion

        #region Darkpool Value Cross Over Average
        private List<DarkpoolValueCrossAvg> _dpWithValueCrossAvgLast10Days;

        public List<DarkpoolValueCrossAvg> DpWithValueCrossAvgLast10Days
        {
            get
            {
                if (_dpWithValueCrossAvgLast10Days != null)
                {
                    return _dpWithValueCrossAvgLast10Days;
                }

                ResetDpWithValueCrossAvgLast10Days();

                return _dpWithValueCrossAvgLast10Days;
            }
        }

        public void ResetDpWithValueCrossAvgLast10Days()
        {
            if (_dpWithValueCrossAvgLast10Days == null)
            {
                _dpWithValueCrossAvgLast10Days = new List<DarkpoolValueCrossAvg>();
            }

            _dpWithValueCrossAvgLast10Days.Clear();

            try
            {
                _dpWithValueCrossAvgLast10Days = MakeDpWithValueCrossAvgLastSpecificDays(5).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("MakeDpWithValueCrossAvgLastSpecificDays Error :{0}", ex);

                throw ex;
            }
        }
        #endregion
        #endregion

        #region Public method
        /// <summary>
        /// return most avtive ticker in specific days and times 
        /// date range is similar to "ResetDpWithValueCrossAvgLast10Days"
        /// </summary>
        /// <param name="days"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public List<string> ReturnMostActiveTicker(int days, int times)
        {
            var l = new List<string>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var dpRepo = new DarkpoolValueCrossAvgRepository(dataSession.Unit);

                string businessDate = DateTime.Now.Date.AddBusinessDays(-days).ToShortDateString();

                string sql = $@"select ticker from darkpool_cross_average where trans_date 
                                between ('{businessDate}'::date - INTERVAL '1 DAY')::date and ( select max(trans_date)::date from darkpool_cross_average)::date
                                group by ticker HAVING count(*) >= {times} ";

                var tmpl = dpRepo.QueryListValue(sql).ToList() as IList<string>;

                l = tmpl.ToList();
            }

            return l;
        }

        public List<DarkpoolValueCrossAvg> ReturnMostActiveEntity(int days, int times)
        {
            var l = new List<string>();

            l = ReturnMostActiveTicker(days, times);

            var rl = new List<DarkpoolValueCrossAvg>();

            rl = DpWithValueCrossAvgLast10Days.Where(o => l.Contains(o.Ticker)).ToList();

            return rl;
        }
        #endregion

        #region Private Method
        private static IEnumerable<DarkpoolTrans> MakeDpWithTickerLastSpecificDays(string ticker, int days)
        {
            var l = new List<DarkpoolTrans>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var dpRepo = new DarkpoolTransRepository(dataSession.Unit);

                // get today's date(eastern time)
                string estLastDateStr = DateTime.UtcNow.Date.ToEST().AddDays(0 - days).ToString("yyyy-MM-dd");

                string sql = $"select * from darkpool_trans where ticker = '{ticker.ToUpper()}' and type = '{TermHelper.Darkpool}' and trans_date >= '{estLastDateStr}'";

                l = dpRepo.QueryBySql(sql, null).ToList();
            }

            return l;
        }

        private static IEnumerable<DarkpoolValueCrossAvg> MakeDpWithValueCrossAvgLastSpecificDays(int days)
        {
            var l = new List<DarkpoolValueCrossAvg>();

            using (var dataSession = new DataTransSession(_dbConn))
            {
                var dpRepo = new DarkpoolValueCrossAvgRepository(dataSession.Unit);

                string businessDate = DateTime.Now.Date.AddBusinessDays(-days).ToShortDateString();

                string sql = $@"select * from darkpool_cross_average where trans_date 
                                between ('{businessDate}'::date - INTERVAL '1 DAY')::date and ( select max(trans_date)::date from darkpool_trans)::date;";

                l = dpRepo.QueryBySql(sql, null).ToList();
            }

            return l;
        }

        #endregion
    }

}
