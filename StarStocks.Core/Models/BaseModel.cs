// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace StarStocks.Core.Models
{
    public abstract class BaseModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("created_date")]
        public System.DateTime? CreatedDate { get; set; }
    }
}
