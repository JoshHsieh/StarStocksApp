// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using StarStocks.Core.Helpers;
using StarStocks.Core.DbWrapper;

namespace StarStocks.Core.UnitOfWork
{
    public class DataTransSession : IDisposable
    {
        private UnitOfWork _unit { get; set; }

        private bool _disposed = false;

        private readonly DbConnection _dbConnInfo;

        private readonly DbHelper _dbHelper;

        private readonly IDbConnection _dbConn;

        public UnitOfWork Unit
        {
            get { return _unit; }
        }

        public DataTransSession(DbConnection dbConn)
        {
            _dbConnInfo = dbConn;

            _dbHelper = new DbHelper(dbConn.AlgoDataDbConnection);

            _dbConn = _dbHelper.GetDbConnection();

            _unit = new UnitOfWork(_dbConn);
        }

        public DataTransSession()
        {
            var fixedDbConn = new DbConnection();

            _dbHelper = new DbHelper(fixedDbConn.AlgoDataDbConnection);

            _dbConn = _dbHelper.GetDbConnection();

            _unit = new UnitOfWork(_dbConn);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbConn.Dispose();

                    _unit.Dispose();

                    if (_unit != null)
                    {
                        _unit.Dispose();
                        _unit = null;
                    }
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
