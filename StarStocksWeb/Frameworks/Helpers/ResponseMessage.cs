// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarStocksWeb.Frameworks.Helpers
{
    /// <summary>
    /// this POCO is used to store response message, not a database table
    /// http://jameschambers.com/2014/06/day-14-bootstrap-alerts-and-mvc-framework-tempdata/
    /// </summary>
    public class ResponseMessage
    {
        public const string TempDataKey = "AlertResponseMessage";

        public string Style { get; set; }

        public string Status { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public List<string> DetailMessageList { get; set; }

        public bool Dismissable { get; set; }
    }
}
