// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarStocksWeb.Frameworks.Helpers;
using StarStocksWeb.Frameworks.Extensions;
using Constants = StarStocksWeb.Frameworks.Constants;

namespace StarStocksWeb.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }

        public void Success(string message, bool dismissable = false)
        {
            AddNotification(Constants.Success, message, dismissable);
        }

        public void Information(string message, bool dismissable = false)
        {
            AddNotification(Constants.Information, message, dismissable);
        }

        public void Warning(string message, bool dismissable = false, List<string> details = null)
        {
            AddNotification(Constants.Warning, message, dismissable, details);
        }

        public void Danger(string message, bool dismissable = false)
        {
            AddNotification(Constants.Danger, message, dismissable);
        }

        /// <summary>
        /// 將 result 裡面封裝，判斷 validation 訊息 display 出來
        /// </summary>
        /// <param name="result"></param>
        public void DisplayServiceResult(ResultWrapper result)
        {
            if (result != null)
            {
                if (result.IsServiceSuccess)
                {
                    Success(result.Message);
                }
                else
                {
                    if (result.InnerMessages != null && result.InnerMessages.Count > 0)
                    {
                        var dl = result.InnerMessages.ToList();

                        Warning(result.Message, true, dl);
                    }
                    else
                    {
                        Warning(result.Message);
                    }
                }
            }
        }

        public void DisplayOperationResult(ResultWrapper result)
        {
            // check ModelState Error
            //var modelErr = ModelState.Where(x => x.Value.Errors.Count > 0)
            //    .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray());

            string modelErrStr = ModelState.StringifyModelErrors();

            if (result != null)
            {
                if (result.Success)
                {
                    Success(result.Message);
                }
                else
                {
                    var dl = new List<string>();

                    dl.Insert(0, modelErrStr);

                    if (result.InnerMessages != null && result.InnerMessages.Count > 0)
                    {

                        dl.InsertRange(1, result.InnerMessages.ToList());
                    }

                    Warning(result.Message, true, dl);
                }
            }
        }

        public void DisplayExceptionResult(ResultWrapper result)
        {
            if (result != null)
            {
                Danger(result.Exception.Message);
            }
        }

        /// <summary>
        /// 針對 ResultWrapper 包裝成 notification
        /// </summary>
        /// <param name="alertStyle"></param>
        /// <param name="message"></param>
        /// <param name="dismissable"></param>
        /// <param name="details"></param>
        private void AddNotification(string alertStyle, string message, bool dismissable, List<string> details = null)
        {
            var alert = TempData.ContainsKey(ResponseMessage.TempDataKey)
                ? (ResponseMessage)TempData[ResponseMessage.TempDataKey]
                : new ResponseMessage();

            alert.Style = alertStyle;

            alert.Message = message;

            alert.Dismissable = dismissable;

            if (details != null && details.Count > 0)
            {
                alert.DetailMessageList = details;
            }

            TempData[ResponseMessage.TempDataKey] = alert;
        }

        /// <summary>
        /// 增加多個 notification 目前沒有使用
        /// </summary>
        /// <param name="alertStyle"></param>
        /// <param name="message"></param>
        /// <param name="dismissable"></param>
        /// <param name="details"></param>
        private void AddNotifications(string alertStyle, string message, bool dismissable, List<string> details = null)
        {
            var alerts = TempData.ContainsKey(ResponseMessage.TempDataKey)
                ? (List<ResponseMessage>)TempData[ResponseMessage.TempDataKey]
                : new List<ResponseMessage>();

            if (details != null && details.Count > 0)
            {
                alerts.Add(new ResponseMessage
                {
                    Style = alertStyle,
                    Message = message,
                    Dismissable = dismissable,
                    DetailMessageList = details
                });
            }

            alerts.Add(new ResponseMessage
            {
                Style = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            TempData[ResponseMessage.TempDataKey] = alerts;
        }
    }
}
