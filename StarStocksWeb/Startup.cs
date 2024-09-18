using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using StarStocks.Core.DbWrapper;
using StarStocks.Core.Interfaces;
using StarStocks.Core.Models;
using StarStocks.Core.Managers;
using StarStocks.Core.Helpers;
using StarStocks.Core.Services;
using StarStocksWeb.Frameworks.Helpers;
using StarStocksWeb.Frameworks.Jobs;

namespace StarStocksWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IndexOptionChainCaculateManager>((ctx) =>
            {
                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new IndexOptionChainCaculateManager(ctx.GetRequiredService<ILogger<IndexOptionChainCaculateManager>>(), dbConn);
            }
            );

            services.AddSingleton<AssetManager>((ctx) =>
            {
                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new AssetManager(ctx.GetRequiredService<ILogger<AssetManager>>(), dbConn);
            }
            );

            services.AddScoped<IFileService, ParseDataService>((ctx) =>
            {
                IResult r = new ResultContainer(true);

                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new ParseDataService(ctx.GetRequiredService<ILogger<ParseDataService>>(), dbConn, r);
            }
            );

            services.AddScoped<OptionDataManager>((ctx) =>
            {
                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new OptionDataManager(ctx.GetRequiredService<ILogger<OptionDataManager>>(), dbConn);
            }
            );

            services.AddScoped<DarkpoolManager>((ctx) =>
            {
                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new DarkpoolManager(ctx.GetRequiredService<ILogger<DarkpoolManager>>(), dbConn);
            }
            );

            services.AddScoped<StockQuoteManager>((ctx) =>
            {
                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new StockQuoteManager(ctx.GetRequiredService<ILogger<StockQuoteManager>>(), dbConn, ctx.GetRequiredService<AssetManager>());
            }
            );

            services.AddScoped<IService<OptionChainDaily>, FetchTdApiService>((ctx) =>
            {
                IResult r = new ResultContainer(true);

                var dbConn = new DbConnection();

                Configuration.GetSection("ConnectionStrings").Bind(dbConn);

                return new FetchTdApiService(ctx.GetRequiredService<ILogger<FetchTdApiService>>(), dbConn, r, ctx.GetRequiredService<IndexOptionChainCaculateManager>());
            }
            );

            #region Schedule Job
            ////register scheduler
            //services.AddSingleton<IJobFactory, JobFactory>();
            //services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            //// register schedule job class
            //services.AddSingleton<OptionChainDailyJob>();
            //// register job instance
            //services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(OptionChainDailyJob), "OptionChainDaily Job", "0/30 * * * * ?"));

            ////register Host service
            //services.AddSingleton<CronJobHostedService>();
            //services.AddHostedService(provider => provider.GetService<CronJobHostedService>());
            #endregion

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                    .AddScoped<IUrlHelper>(x => x
                    .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseGlobalErrorHandleMiddleware();


                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
                // https, The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // https
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
