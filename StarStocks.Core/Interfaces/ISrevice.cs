// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StarStocks.Core.Models;

namespace StarStocks.Core.Interfaces
{
    public interface IService<T> where T : BaseModel
    {
        public Task<IResult> DoService(Dictionary<string, string> dd);

        IResult ReturnServiceResult();
    }
}
