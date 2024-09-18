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
    public sealed class OptionDashboardRepository : AbstractRepository<OptionDashboard>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public OptionDashboardRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(OptionDashboard),
                new ColumnAttributeTypeMapper<OptionDashboard>());
        }
    }

    public sealed class HighVolOptionRepository : AbstractRepository<HighVolOption>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public HighVolOptionRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(HighVolOption),
                new ColumnAttributeTypeMapper<HighVolOption>());
        }
    }

    public sealed class OptionChainDailyRepository : AbstractRepository<OptionChainDaily>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public OptionChainDailyRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(OptionChainDaily),
                new ColumnAttributeTypeMapper<OptionChainDaily>());
        }

        public double QuerySinglePriceValue(string sql)
        {
            return _conn.QuerySingleOrDefault<double>(sql);
        }
    }
}
