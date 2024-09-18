// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarStocksWeb.Frameworks
{
    public static class Constants
    {
        #region UI settings
        public const int DefaultPagedCount = 30;
        #endregion

        #region System Settings
        public const string AlgoDataDbConnName = "AlgoDataDbConnection";

        public const int Default60Days = -60;

        public const int Default30Days = -30;

        public const int Default180Days = -180;
        #endregion

        #region Operation Setting
        public const string BluesStrategyAccountBlues = "Blues";
        #endregion

        #region Upload Path
        public const string HighVolCheaplies = "HighVolCheaplies";

        public const string HighVolLeaps = "HighVolLeaps";

        public const string MostOtmStrikes = "MostOtmStrikes";

        public const string LargeOtmOi = "LargeOtmOi";

        public const string CallsDashboard = "CallsDashboard";

        public const string PutsDashboard = "PutsDashboard";

        public const string DarkpoolTrades = "DarkpoolTrades";

        public const string BlockTrades = "BlockTrades";

        public const string DarkpoolTickerDashboard = "DarkpoolTickerDashboard";

        public const string StrategyResult = "StrategyResult";

        public const string StockQuoteTw = "StockQuoteTw";
        #endregion

        #region Alert Term
        // debug
        public const string Debug = "Debug";

        public const string ExceptionRaise = "Exception";

        public const string Error = "Error";

        // do not modify
        public const string Success = "success";

        public const string Information = "info";

        public const string Warning = "warning";

        public const string Danger = "danger";

        #endregion Alert Term

        #region Operation, Validation : Error & Warning Message
        public const string AddRecordError = "新增資料錯誤!";

        public const string ProcessAllowanceRecordError = "處理折讓(銷退)資料錯誤!";

        public const string OperationError = "操作資料發生錯誤! 請聯絡管理員";

        public const string NoSelectedRecord = "請選擇資料再送出!";

        public const string QueryNoResult = "查無資料，請重新以其他條件進行搜尋!";

        public const string InvalidOperation = "缺少資料，請勿進行異常操作!";

        public const string LoginError = "無效的登入操作，請聯絡管理員!";

        public const string LoginFailure = "登入錯誤，請檢查帳號與密碼!";

        public const string ProfileDataShortError = "帳號資料有誤!";

        public const string ProfileDataError = "帳號資料有誤，請聯絡管理員!";

        public const string UploadFileEncodingError = "上傳檔案編碼錯誤!";

        public const string UploadFileExtError = "上傳檔案需為 Excel(xlsx) 檔!";

        public const string UploadFileExtErrorForCth = "上傳檔案需為 txt 檔!";

        public const string UploadSheetCountLessThan2Error = "Excel 中的工作表數量需要 2 張!";

        public const string UploadSheetCountLessThan1Error = "Excel 中的工作表數量需要 1 張!";

        public const string UploadFileValidateError = "上傳檔案驗證有誤!";

        public const string UploadFileAndStoreSuccessfully = "檔案上傳與資料處理成功!";

        public const string UploadFileAndStoreFailure = "檔案上傳與資料處理失敗，需聯絡管理員!";

        public const string UploadFileInvalidFormat = "上傳檔案中資料格式有誤，請參閱詳細說明!";

        public const string InvMasterOccursError = "發票主資料有誤";

        public const string InvDetailOccursError = "發票細項資料有誤";

        public const string InvProcessingOccursError = "發票處理過程有誤! 請聯絡管理員";

        public const string InitParamIsNullOrEmpty = "服務起始參數是空或是無效型別!，請聯絡管理員";

        public const string InputParamIsNullOrEmpty = "輸入參數是空或是無效型別!";

        public const string InvoiceNoIsNullOrEmpty = "發票號碼不能為空!";

        public const string InvoiceIdIsNullOrEmpty = "發票辨識碼不能為空!";

        public const string InvoiceNoLengthIsNotEqual = "完整發票號碼必須為英文字母字軌，發票數字，一共10位!";

        public const string UserAccountIsNullOrWrong = "使用者帳號錯誤，請聯絡管理員!";

        public const string UploadFilePathNameIsNull = "上傳檔案名為空，請聯絡管理員!";

        public const string UploadFilePathIsNotExist = "上傳檔案不存在，請聯絡管理員!";

        public const string UploadFileContentIsNull = "上傳檔案內容為空，請再檢查一次!";

        public const string UploadExcelWorkSheetContentIsNull = "上傳檔案內的工作表內容為空，請再檢查一次!";

        public const string UploadExcelWorkSheetContentIsNullOrInvNumberNotFound = "上傳檔案內的工作表內容為空或是發票號碼欄位不存在，請再檢查一次!";

        public const string UploadFileFormatIsNotCorrect = "上傳檔案內容格式有誤，，請再檢查一次!";

        public const string UploadFileFieldCountIsNotEqual = "上傳檔案每筆發票必須要有16個欄位，數目不符!";

        public const string UploadFileInvoiceIsExist = "此筆發票已經上傳過!";

        public const string InsertInvoiceToDbFailure = "新增發票至 DB 有誤!";

        public const string InsertAllowToDbFailure = "新增銷退折讓至 DB 有誤!";

        public const string InvoiceCodeIsNotEngChar = "發票字軌必須是大寫英文字母!";

        public const string InvoiceDateIsNullOrEmpty = "發票日期不能為空!";

        public const string InvoiceDateMustBeNumeric = "發票日期須為數字";

        public const string InvoiceDateGreaterThanToday = "無法存證，因欲存證發票的發票日期超過今日!";

        public const string RandomNumIsNullOrEmpty = "隨機碼不能為空!";

        public const string RandomNumMustBeNumeric = "隨機碼須為數字!";

        public const string BuyerVatIsNullOrEmpty = "買受人統編不能為空!";

        public const string BuyerVatMustBeNumeric = "買受人統編須為數字!";

        public const string SalesAmountIsNullOrEmpty = "應稅銷售額不能為空或是 0!";

        public const string FreeSalesAmountIsNullOrEmpty = "免稅銷售額不能為空或是 0!";

        public const string TotalAmountIsNullOrEmpty = "總金額不能為空或是 0!";

        public const string TotalAmountMustBeNumeric = "總金額須為大於等於0的數字!";

        public const string TaxAmountIsNullOrEmpty = "稅額不能為空或是 0!";

        public const string GoodnessNameIsNullOrEmpty = "品名不能為空!";

        public const string QuantityIsNullOrEmpty = "數量不能為空!";

        public const string QuantityMustBeNumeric = "數量須為數字!";

        public const string UnitPriceIsNullOrEmpty = "單價不能為空!";

        public const string UnitPriceMustBeNumeric = "單價須為大於等於0的數字!";

        public const string ItemTotalIsNullOrEmpty = "金額不能為空!";

        public const string ItemTotalMustBeNumeric = "金額須為大於等於0的數字!";

        public const string InvoiceCountIsNotMatchInput = "發票數量不符輸入數量! 請手動確認發票號碼是否存在";

        #region 發票列印
        public const string PrinterNotSet = "印表機尚未設定! 請聯絡系統管理員";

        public const string QueryPrintedInvoiceNoResult = "最近兩個月並無列印發票!";

        public const string QueryPrintableInvoiceNoResult = "最近兩個月並無可列印發票!";

        public const string QueryUnPrintInvoiceNoResult = "最近兩個月並無待列印發票!";

        public const string PrintSuccessfully = "發票證明聯列印成功!";

        public const string PrintAllSuccessfully = "所有發票證明聯列印成功!";

        public const string PrintAllFailed = "發票證明聯列印失敗! 請參見錯誤訊息，或是聯絡管理員";
        #endregion

        #region API Response
        public const string CommitAllSuccessfully = "所有資料存證成功!";

        public const string CommitAllFailed = "資料存證失敗，請參考詳細訊息並修正";

        public const string CommitCountError = "資料存證過程發生存證資料數不符錯誤，請聯絡管理員!";

        public const string CommitSuccessfully = "發票資料存證成功!";

        public const string ApiNotResponse = "伺服器未回應，或許是網路問題，請稍後再試或是聯絡管理員!";

        public const string UrlNotFound = "存證路徑錯誤，請聯絡管理員!";

        public const string ErrorTrack = "發票字軌錯誤，請重新修正再重新進行存證!";

        public const string ServerError = "伺服器處理資料發生異常，請聯絡管理員!";

        public const string BadRequest = "存證的發票資料有誤，請修正後再重新進行存證";

        public const string StatusOkButDataNotSaved = "發票存證作業成功，但是資料存檔出現錯誤，請聯絡管理員!";

        public const string StatusUnknown = "發票存證作業發生不明原因錯誤，請聯絡管理員!";

        #endregion
        #endregion
    }
}
