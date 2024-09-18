// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using StarStocks.Core.Models;

namespace StarStocks.Core.Helpers
{
    public static class ModelFactory
    {
        private static readonly Dictionary<string, BaseModel> _fileModels;

        static ModelFactory()
        {
            _fileModels = new Dictionary<string, BaseModel>();

            _fileModels.Add(FileModelMapper.BlockTrades, new DarkpoolTrans());
            _fileModels.Add(FileModelMapper.DarkpoolTrades, new DarkpoolTrans());

            _fileModels.Add(FileModelMapper.CallsDashboard, new OptionDashboard());
            _fileModels.Add(FileModelMapper.PutsDashboard, new OptionDashboard());
        }

        private static BaseModel ReturnFileModel(string fileType)
        {
            var fModel = _fileModels
                .Where(x => x.Key.Equals(fileType))
                .Select(x => x.Value)
                .FirstOrDefault();

            return fModel ?? throw new Exception("No Match File Model!");
        }
    }
}
