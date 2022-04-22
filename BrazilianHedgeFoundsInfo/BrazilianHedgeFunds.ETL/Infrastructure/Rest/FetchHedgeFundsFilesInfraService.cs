using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Infrastructure.Rest
{
    public class FetchHedgeFundsFilesInfraService : IFetchHedgeFundsFilesInfraService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FetchHedgeFundsFilesInfraService> _logger;

        public FetchHedgeFundsFilesInfraService(IConfiguration configuration, ILogger<FetchHedgeFundsFilesInfraService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task GetFileData(string url)
        {
            try
            {
                var filePath = _configuration["Defaults:filePath"];
                var fileName = url.Split('/').Last();

                _logger.LogInformation($"Starting download from {url}");

                using (WebClient webClient = new WebClient())
                    await webClient.DownloadFileTaskAsync(new Uri(url), $"{filePath}{fileName}");

                _logger.LogInformation($"File was downloaded from {url}");
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to download from {url}. Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);

                throw;
            }
        }
    }
}
