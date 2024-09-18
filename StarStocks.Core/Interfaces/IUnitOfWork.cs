// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StarStocks.Core.Interfaces
{

    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }

        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }

        void Begin();

        void Commit();

        void Rollback();
    }
}
