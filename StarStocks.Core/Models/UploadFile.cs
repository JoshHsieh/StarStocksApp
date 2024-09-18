// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace StarStocks.Core.Models
{
    [Table("upload_file", Schema = "public")]
    public class UploadFile : BaseModel
    {
        #region Implement Property

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        #endregion
    }
}
