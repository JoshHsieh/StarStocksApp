// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using StarStocks.Core.Models;
using StarStocks.Core.Helpers;
using StarStocks.Core.Interfaces;
using Dapper;

namespace StarStocks.Core.Repositories
{
    public sealed class StockQuoteRepository : AbstractRepository<StockQuoteTw>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public StockQuoteRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(StockQuoteTw),
                new ColumnAttributeTypeMapper<StockQuoteTw>());

            Dapper.SqlMapper.SetTypeMap(
                typeof(TickerBarSeries),
                new ColumnAttributeTypeMapper<TickerBarSeries>());
        }
    }
}
