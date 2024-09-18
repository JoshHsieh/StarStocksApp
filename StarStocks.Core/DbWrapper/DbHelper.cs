// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StarStocks.Core.Interfaces;

namespace StarStocks.Core.DbWrapper
{
    public class DbHelper : IDataBase
    {
        private readonly string _connString;

        private readonly IDbConnection _dbConn;

        public ConnectionState State => this._dbConn.State;

        public DbHelper()
        { }

        public DbHelper(string connStr)
        {
            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException(nameof(connStr));
            }
            _connString = connStr;

            _dbConn = new NpgsqlConnection(_connString);
        }

        public IDbConnection GetDbConnection()
        {
            _dbConn.Open();

            return _dbConn;
        }

        public IDbConnection GetDbConnection(string connStr)
        {
            NpgsqlConnection conn;

            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException(nameof(connStr));
            }

            conn = new NpgsqlConnection(connStr);

            conn.Open();

            return conn;
        }

    }

    public class DbConnection
    {
        public string DefaultConnection { get; set; }

        public string AlgoDataDbConnection { get; set; }

    }
}
