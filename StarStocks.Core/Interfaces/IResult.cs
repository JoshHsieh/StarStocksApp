// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace StarStocks.Core.Interfaces
{
    public interface IResult
    {
        /// <summary>
        /// 進行操作後回傳的結果
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// 進行驗證的判斷
        /// </summary>
        bool IsValid { get; set; }

        string Message { get; set; }

        Exception Exception { get; set; }

        List<string> InnerMessages { get; set; }

        /// <summary>
        /// 將訊息加入 ViewModel 或者其他 container
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        void AddError(string key, string errorMessage);

        /// <summary>
        /// 將錯誤訊息加入自身的 error list
        /// </summary>
        /// <param name="errorMessage"></param>
        void GatherErrorList(string errorMessage);

        void SetRefContainer(Dictionary<string, string> d);

        /// <summary>
        /// 傳回結果集
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> ReturnRefContainer();

        void Reset();
    }
}
