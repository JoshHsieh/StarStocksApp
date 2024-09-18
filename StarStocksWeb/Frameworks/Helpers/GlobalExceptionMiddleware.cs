// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Routing;
using StarStocksWeb.Models;
using StarStocksWeb.Frameworks.Extensions;

namespace StarStocksWeb.Frameworks.Helpers
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;

            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception happened! caught in the middleware: ");

                await HandleExceptionASync(context, ex);
            }
        }

        private async Task HandleExceptionASync(HttpContext httpContext, Exception ex)
        {
            // log 
            _logger.LogError(ex, $"Exception massage: {ex.Message}, StackTrace: {ex.StackTrace}", ex);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (httpContext.Request.Headers["Content-Type"].ToString().ToLower() == "application/json"
                  || httpContext.Request.Headers["X-Requested-With"].ToString().ToLower() == "xmlhttprequest")
            {
                httpContext.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new ApiResponse
                { Status = "NG", ErrorResult = new Error { Code = "500", Message = "Internal Server Error, Pls Check System Log" } });

                await httpContext.Response.WriteAsync(result);
            }
            else
            {
                var errPageUrl = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/pages/Error.html");

                //when request page
                httpContext.Response.Redirect(errPageUrl.ToString(), false);
                //httpContext.Request.Path = "/Home/Error";

                return;
            }

            await _next.Invoke(httpContext);
        }
    }

    public static class ExceptionHandleMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
