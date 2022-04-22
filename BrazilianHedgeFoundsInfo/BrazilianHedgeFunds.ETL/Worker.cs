using BrazilianHedgeFunds.ETL.Business.Interfaces;
using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Business.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IPrepareEdgeFundsFileDataService _prepareFileservice;
        private readonly ITransformHedgeFundsDataService _transformDataservice;

        public Worker(ILogger<Worker> logger,
                      IHostApplicationLifetime hostApplicationLifetime,
                      IPrepareEdgeFundsFileDataService prepareFileservice,
                      ITransformHedgeFundsDataService transformDataservice)
        {
            _logger = logger;
            _prepareFileservice = prepareFileservice;
            _transformDataservice = transformDataservice;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hedge Funds' ETL running..." );
            var initTime = DateTimeOffset.Now;

            var wereDataPreparedToTransform = await _prepareFileservice.PrepareData(stoppingToken);

            if (wereDataPreparedToTransform)
                await _transformDataservice.TransformDataFromCsvToDB();

            _prepareFileservice.CleanTempFiles();

            if(ProcessError.Errors.Count > 0)
            {
                _logger.LogInformation("We found the followings errors:");

                foreach (var error in ProcessError.Errors)
                    _logger.LogInformation(error);
            }

            var endTime = DateTime.Now;
            var timeElapsed = endTime - initTime;

            _logger.LogInformation($"Hedge Funds' ETL ending at {endTime}, and spent {timeElapsed}.");
            _hostApplicationLifetime.StopApplication();
        }
    }
}
