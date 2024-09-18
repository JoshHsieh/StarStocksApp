// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StarStocks.Core.Interfaces
{

    public interface IDataBase
    {
        /// <summary>
        /// return default database connection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();

        /// <summary>
        /// return specific database connection
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        IDbConnection GetDbConnection(string connStr);

    }
}
