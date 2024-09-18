// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using StarStocks.Core.Models;
using StarStocks.Core.Helpers;
using StarStocks.Core.Interfaces;
using Dapper;

namespace StarStocks.Core.Repositories
{
    public class DarkpoolDashboardRepository : AbstractRepository<DarkpoolDashboard>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IDbTransaction _dbTrans;

        private readonly IUnitOfWork _unit;

        public DarkpoolDashboardRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbTrans = unit.Transaction;
            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(DarkpoolDashboard),
                new ColumnAttributeTypeMapper<DarkpoolDashboard>());
        }
    }

    public class DarkpoolValueCrossAvgRepository : AbstractRepository<DarkpoolValueCrossAvg>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IDbTransaction _dbTrans;

        private readonly IUnitOfWork _unit;

        public DarkpoolValueCrossAvgRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbTrans = unit.Transaction;
            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(DarkpoolValueCrossAvg),
                new ColumnAttributeTypeMapper<DarkpoolValueCrossAvg>());
        }
    }
}
