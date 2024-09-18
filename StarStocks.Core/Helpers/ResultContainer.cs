// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using StarStocks.Core.Interfaces;

namespace StarStocks.Core.Helpers
{
    public class ResultContainer : IResult
    {
        private bool _success;

        public bool Success
        {
            get { return IsValid ? true : false; }

            set { _success = value; }
        }

        /// <summary>
        /// 進行資料驗證用
        /// </summary>
        public bool IsValid { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public List<string> InnerMessages { get; set; }

        public Dictionary<string, string> RefSet;

        #region Constructor
        public ResultContainer(bool isValid)
        {
            IsValid = isValid;

            InnerMessages = new List<string>();

            RefSet = new Dictionary<string, string>();
        }
        #endregion

        /// <summary>
        /// 將 error message 加入 view model
        /// </summary>
        /// <param name="key"></param>
        /// <param name="errorMessage"></param>
        public void AddError(string key, string errorMessage)
        {
            RefSet.Add(key, errorMessage);

            Message = errorMessage;
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

        public void Reset()
        {
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
    }
}
