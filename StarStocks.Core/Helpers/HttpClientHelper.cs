// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Web;
using StarStocks.Core.Models;

namespace StarStocks.Core.Helpers
{
    public class HttpClientHelper
    {
        public async Task<string> FinmindSignIn()
        {
            var path = "https://api.finmindtrade.com/api/v4/login";

            var d = new Dictionary<string, string>
            {
                {"user_id", "mxhsieh"},
                {"password", "mxopqo1221"},
            };

            var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(d) };

            string token = string.Empty;

            using (var client = new HttpClient())
            {
                var res = await client.SendAsync(req);

                switch (res.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var json = await res.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<FinmindLoginResponse>(json);
                        token = result.Token;
                        break;
                    default:
                        throw (new Exception($"{res.StatusCode} {res.ReasonPhrase}"));
                }
            }

            return token;
        }

        public async Task<FinmindStockPriceResponse> FetchStockPriceTickFromFinmind(string token, string tikcer, string sDate)
        {
            var path = "https://api.finmindtrade.com/api/v4/data";

            var d = new Dictionary<string, string>
            {
                {"dataset", "TaiwanStockPriceTick"},
                {"data_id", tikcer},
                {"start_date", sDate},
                {"token", token}
            };

            string respJson = string.Empty;

            var requestUri = QueryHelpers.AddQueryString(path, d);

            var req = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var stockPrice = new FinmindStockPriceResponse();

            using (var client = new HttpClient())
            {
                var res = await client.SendAsync(req);

                switch (res.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var json = await res.Content.ReadAsStringAsync();
                        stockPrice = JsonSerializer.Deserialize<FinmindStockPriceResponse>(json);
                        //respJson = result.Token;
                        break;
                    default:
                        throw (new Exception($"{res.StatusCode} {res.ReasonPhrase}"));
                }
            }

            return stockPrice;
        }
    }
}
