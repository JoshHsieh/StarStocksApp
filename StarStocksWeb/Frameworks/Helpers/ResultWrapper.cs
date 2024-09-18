// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using StarStocks.Core.Models;
using StarStocks.Core.Interfaces;

namespace StarStocksWeb.Frameworks.Helpers
{
    public class ResultWrapper
    {
        private readonly ModelStateDictionary _modelState;

        public bool Success { get; set; }

        public bool IsServiceSuccess { get; set; }

        /// <summary>
        /// 進行資料驗證用
        /// </summary>
        public bool IsValid { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public List<string> InnerMessages { get; set; }

        public Dictionary<string, string> RefSet;

        #region Constructor
        public ResultWrapper()
            : this(false)
        {
        }

        public ResultWrapper(bool success, ModelStateDictionary modelState)
        {
            Success = success;
            InnerMessages = new List<string>();

            if (_modelState != null)
            {
                _modelState.Clear();
            }
            else
            {
                _modelState = modelState;
            }
        }

        public ResultWrapper(bool success)
        {
            Success = success;

            InnerMessages = new List<string>();
        }
        #endregion

        /// <summary>
        /// 將 error message 加入 view model
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        public void AddError(string key, string errorMessage)
        {
            if (_modelState != null)
            {
                _modelState.AddModelError(key, errorMessage);
            }
            else
            {
                Message = errorMessage;
            }
        }

        /// <summary>
        /// 將 error 加進 InnerResults 之中
        /// </summary>
        public void GatherErrorList(string errMsg)
        {
            if (string.IsNullOrEmpty(errMsg) != true)
            {
                InnerMessages.Add(errMsg);
            }
        }

        public void SetRefContainer(Dictionary<string, string> refDict)
        {
            if (RefSet == null)
            {
                RefSet = new Dictionary<string, string>();
            }

            RefSet.Clear();

            RefSet = refDict;
        }

        public Dictionary<string, string> ReturnRefContainer()
        {
            if (RefSet == null)
            {
                RefSet = new Dictionary<string, string>();
            }

            return RefSet;
        }

        public void GatherServiceResult(IResult serviceResult)
        {
            if (serviceResult != null)
            {
                if (serviceResult.InnerMessages.Count > 0)
                {
                    InnerMessages.AddRange(serviceResult.InnerMessages);
                }

                IsServiceSuccess = serviceResult.Success;
            }
        }

        public void Reset()
        {
            if (_modelState != null)
            {
                _modelState.Clear();
            }

            if (RefSet != null)
            {
                RefSet.Clear();
            }

            if (InnerMessages != null)
            {
                InnerMessages.Clear();
            }

            Success = true;

            IsValid = true;

            Message = string.Empty;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
