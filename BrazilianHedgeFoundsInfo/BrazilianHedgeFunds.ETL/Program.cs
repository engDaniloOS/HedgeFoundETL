using BrazilianHedgeFunds.ETL.Business.Interfaces;
using BrazilianHedgeFunds.ETL.Business.Services;
using BrazilianHedgeFunds.ETL.Infrastructure.Files;
using BrazilianHedgeFunds.ETL.Infrastructure.Repository;
using BrazilianHedgeFunds.ETL.Infrastructure.Rest;
using BrazilianHedgeFunds.ETL.Services;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Files;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Repository;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Rest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BrazilianHedgeFunds.ETL
{
    public class Program
    {
        public static DateTime CommandOldestDateToProcess;
        public static void Main(string[] args)
        {
            try
            {
                var year = int.Parse(args[0]);
                var month = int.Parse(args[1]);

                CommandOldestDateToProcess = new DateTime(year, month, 1);
            }
            catch (Exception) { }


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IPrepareEdgeFundsFileDataService, PrepareEdgeFundsFilesService>();
                    services.AddSingleton<ITransformHedgeFundsDataService, TransformHedgeFundsDataService>();
                    services.AddSingleton<IHedgeFundsCsvFileInfraService, HedgeFundsCsvFileInfraService>();
                    services.AddSingleton<IFetchHedgeFundsFilesInfraService, FetchHedgeFundsFilesInfraService>();
                    services.AddSingleton<IHedgeFundsFileManagerInfraService, HedgeFundsFileManagerInfraService>();
                    services.AddSingleton<IHedgeFundRecordRepository, HedgeFundRecordRepository>();
                    services.AddDbContextFactory<Context>(option => option.UseSqlServer(connectionString));
                    services.AddHostedService<Worker>();
                });

        private static readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\HedgeFund.mdf;Integrated Security=True;Connect Timeout=30";
    }
}
