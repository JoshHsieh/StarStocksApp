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
    public sealed class AssetUnitRepository : AbstractRepository<AssetUnit>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public AssetUnitRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(AssetUnit),
                new ColumnAttributeTypeMapper<AssetUnit>());
        }
    }

    public sealed class TickerSettingRepository : AbstractRepository<TickerSettings>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public TickerSettingRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(TickerSettings),
                new ColumnAttributeTypeMapper<TickerSettings>());
        }
    }

    public sealed class TickerEventRepository : AbstractRepository<TickerEvent>
    {
        private string _tableName => GetTableNameMapper();

        public IUnitOfWork Unit => _unit;

        private readonly IDbConnection _dbConn;

        private readonly IUnitOfWork _unit;

        public TickerEventRepository(IUnitOfWork unit) : base(unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));

            _unit = unit;

            _dbConn = unit.Connection;

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);

            // create mapping
            Dapper.SqlMapper.SetTypeMap(
                typeof(TickerEvent),
                new ColumnAttributeTypeMapper<TickerEvent>());
        }
    }
}
