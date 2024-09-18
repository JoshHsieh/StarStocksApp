// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using X.PagedList;
using StarStocks.Core.Interfaces;
using StarStocksWeb.Frameworks;
using StarStocksWeb.Frameworks.ViewModels;
using StarStocksWeb.Frameworks.Helpers;

namespace StarStocksWeb.Controllers
{
    public class UploadFileController : BaseController
    {
        private readonly ILogger<UploadFileController> _logger;

        private readonly IFileService _fileService;

        private readonly string _uploadRoot;

        public UploadFileController(IWebHostEnvironment env, IFileService fileService)
        {
            _uploadRoot = $@"{env.WebRootPath}\Uploads";

            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> OpHighVolCheapFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            //vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
            vm.Index = pageIndex.HasValue ? pageIndex.Value : 1;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpHighVolLeapFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpMostOtmStrikesFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpLargeOtmOiFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpCallFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpPutFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DarkPoolTradeFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> BlockTradeFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DarkpoolDashboardFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        public IActionResult UploadFiles()
        {
            var vm = new UploadFileViewModel();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(UploadFileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // fail
                var fl = new List<string>();

                // success
                var sl = new Dictionary<string, string>();

                string filePath = string.Empty;

                if (vm.HighVolCheaplies != null && vm.HighVolCheaplies.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.HighVolCheaplies, vm.HighVolCheaplies.FileName);

                    if (SiteHelper.SaveFileByType(vm.HighVolCheaplies, filePath) != true)
                    {
                        fl.Add(Constants.HighVolCheaplies);
                    }
                    else
                    {
                        sl.Add(Constants.HighVolCheaplies, filePath);
                    }
                }

                if (vm.HighVolLeaps != null && vm.HighVolLeaps.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.HighVolLeaps, vm.HighVolLeaps.FileName);

                    if (SiteHelper.SaveFileByType(vm.HighVolLeaps, filePath) != true)
                    {
                        fl.Add(Constants.HighVolLeaps);
                    }
                    else
                    {
                        sl.Add(Constants.HighVolLeaps, filePath);
                    }
                }

                if (vm.MostOtmStrikes != null && vm.MostOtmStrikes.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.MostOtmStrikes, vm.MostOtmStrikes.FileName);

                    if (SiteHelper.SaveFileByType(vm.MostOtmStrikes, filePath) != true)
                    {
                        fl.Add(Constants.MostOtmStrikes);
                    }
                    else
                    {
                        sl.Add(Constants.MostOtmStrikes, filePath);
                    }
                }

                if (vm.LargeOtmOi != null && vm.LargeOtmOi.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.LargeOtmOi, vm.LargeOtmOi.FileName);

                    if (SiteHelper.SaveFileByType(vm.LargeOtmOi, filePath) != true)
                    {
                        fl.Add(Constants.LargeOtmOi);
                    }
                    else
                    {
                        sl.Add(Constants.LargeOtmOi, filePath);
                    }
                }

                if (vm.CallsDashboard != null && vm.CallsDashboard.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.CallsDashboard, vm.CallsDashboard.FileName);

                    if (SiteHelper.SaveFileByType(vm.CallsDashboard, filePath) != true)
                    {
                        fl.Add(Constants.CallsDashboard);
                    }
                    else
                    {
                        sl.Add(Constants.CallsDashboard, filePath);
                    }
                }

                if (vm.PutsDashboard != null && vm.PutsDashboard.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.PutsDashboard, vm.PutsDashboard.FileName);

                    if (SiteHelper.SaveFileByType(vm.PutsDashboard, filePath) != true)
                    {
                        fl.Add(Constants.PutsDashboard);
                    }
                    else
                    {
                        sl.Add(Constants.PutsDashboard, filePath);
                    }
                }

                if (vm.DarkpoolTrades != null && vm.DarkpoolTrades.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.DarkpoolTrades, vm.DarkpoolTrades.FileName);

                    if (SiteHelper.SaveFileByType(vm.DarkpoolTrades, filePath) != true)
                    {
                        fl.Add(Constants.DarkpoolTrades);
                    }
                    else
                    {
                        sl.Add(Constants.DarkpoolTrades, filePath);
                    }
                }

                if (vm.BlockTrades != null && vm.BlockTrades.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.BlockTrades, vm.BlockTrades.FileName);

                    if (SiteHelper.SaveFileByType(vm.BlockTrades, filePath) != true)
                    {
                        fl.Add(Constants.BlockTrades);
                    }
                    else
                    {
                        sl.Add(Constants.BlockTrades, filePath);
                    }
                }

                if (vm.DarkpoolTickerDashboard != null && vm.DarkpoolTickerDashboard.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.DarkpoolTickerDashboard, vm.DarkpoolTickerDashboard.FileName);

                    if (SiteHelper.SaveFileByType(vm.DarkpoolTickerDashboard, filePath) != true)
                    {
                        fl.Add(Constants.DarkpoolTickerDashboard);
                    }
                    else
                    {
                        sl.Add(Constants.DarkpoolTickerDashboard, filePath);
                    }
                }

                // start to parse file
                var resWrapper = new ResultWrapper(true, ModelState);

                IResult parserResult = await _fileService.DoService(sl);

                resWrapper.GatherServiceResult(parserResult);

                var refreshVm = new UploadFileViewModel();

                refreshVm.ResultWrapper = resWrapper;

                DisplayServiceResult(resWrapper);

                return View(refreshVm);
            }
            ModelState.AddModelError("err", Constants.InvalidOperation);

            return View();
        }

        public IActionResult UploadStockQuoteFiles()
        {
            var vm = new UploadFileViewModel();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadStockQuoteFiles(UploadFileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // fail
                var fl = new List<string>();

                // success
                var sl = new Dictionary<string, string>();

                string filePath = string.Empty;

                if (vm.StockQuoteDailyTw != null && vm.StockQuoteDailyTw.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.StockQuoteTw, vm.StockQuoteDailyTw.FileName);

                    if (SiteHelper.SaveFileByType(vm.StockQuoteDailyTw, filePath) != true)
                    {
                        fl.Add(Constants.StockQuoteTw);
                    }
                    else
                    {
                        sl.Add(Constants.StockQuoteTw, filePath);
                    }
                }

                // start to parse file
                var resWrapper = new ResultWrapper(true, ModelState);

                IResult parserResult = await _fileService.DoService(sl);

                resWrapper.GatherServiceResult(parserResult);

                var refreshVm = new UploadFileViewModel();

                refreshVm.ResultWrapper = resWrapper;

                DisplayServiceResult(resWrapper);

                return View(refreshVm);
            }

            ModelState.AddModelError("err", Constants.InvalidOperation);

            return View();
        }


        public async Task<IActionResult> UploadStrategyFiles(int? pageIndex = 1)
        {
            var vm = new UploadFileViewModel();

            // 確定分頁的目前頁數
            //vm.Index = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
            vm.Index = pageIndex.HasValue ? pageIndex.Value : 1;

            var uploads = await _fileService.ReturnFileListAsync();

            // 上傳檔案清單
            vm.UploadFileList = uploads.Where(x => x.UserName == Constants.BluesStrategyAccountBlues).OrderByDescending(x => x.CreatedDate).ToList();

            vm.PagedUploadFileList = uploads.ToPagedList(vm.Index, Constants.DefaultPagedCount);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadStrategyFiles(UploadFileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // fail
                var fl = new List<string>();

                // success
                var sl = new Dictionary<string, string>();

                string filePath = string.Empty;

                if (vm.StrategyResult != null && vm.StrategyResult.Length > 0)
                {
                    filePath = Path.Combine(_uploadRoot, Constants.StrategyResult, vm.StrategyResult.FileName);

                    if (SiteHelper.SaveFileByType(vm.StrategyResult, filePath) != true)
                    {
                        fl.Add(Constants.StrategyResult);
                    }
                    else
                    {
                        sl.Add(Constants.StrategyResult, filePath);
                    }
                }

                // start to parse file
                var resWrapper = new ResultWrapper(true, ModelState);

                IResult parserResult = await _fileService.DoService(sl);

                resWrapper.GatherServiceResult(parserResult);

                var refreshVm = new UploadFileViewModel();

                refreshVm.ResultWrapper = resWrapper;

                DisplayServiceResult(resWrapper);

                return View(refreshVm);
            }

            ModelState.AddModelError("err", Constants.InvalidOperation);

            return View();
        }
    }
}
