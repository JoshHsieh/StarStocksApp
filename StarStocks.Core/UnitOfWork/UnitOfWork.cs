// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using StarStocks.Core.Interfaces;
using StarStocks.Core.DbWrapper;

namespace StarStocks.Core.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        IDbConnection _conn = null;

        IDbTransaction _transaction = null;

        Guid _id = Guid.Empty;

        internal UnitOfWork(IDbConnection conn)
        {
            _id = Guid.NewGuid();

            _conn = conn;

        }

        IDbConnection IUnitOfWork.Connection
        {
            get
            {
                return _conn;
            }
        }

        IDbTransaction IUnitOfWork.Transaction
        {
            get
            {
                return _transaction;
            }
        }

        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }

        public void Begin()
        {
            _transaction = _conn.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            Dispose();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_conn != null)
            {
                _conn.Dispose();
                _conn = null;
            }
        }
    }
}
