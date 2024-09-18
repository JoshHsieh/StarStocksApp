// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace StarStocks.Core.Models
{
    public class FinmindLoginResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }

    public class FinmindStockPriceResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("data")]
        public List<FinmindStockPriceEntity> StockPriceList { get; set; }
    }

    public class FinmindStockPriceEntity
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("stock_id")]
        public string StockId { get; set; }

        [JsonPropertyName("deal_price")]
        public double DealPrice { get; set; }

        [JsonPropertyName("volume")]
        public int Volume { get; set; }

        [JsonPropertyName("Time")]
        public string Time { get; set; }

        [JsonPropertyName("TickType")]
        public int TickType { get; set; }
    }
}

