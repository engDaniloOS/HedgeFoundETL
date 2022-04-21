using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BrazilianHedgeFunds.ETL.Infrastructure.Files
{
    public class HedgeFundsFileManagerInfraService : IHedgeFundsFileManagerInfraService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HedgeFundsFileManagerInfraService> _logger;

        private readonly string _filePath;

        public HedgeFundsFileManagerInfraService(IConfiguration configuration, ILogger<HedgeFundsFileManagerInfraService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _filePath = _configuration["Defaults:filePath"];
        }

        public void UnzipFile(string fileName)
        {
            try
            {
                var zipFileName = $"{_filePath}{fileName}";

                _logger.LogInformation($"Unziping {fileName}...");
                ZipFile.ExtractToDirectory(zipFileName, _filePath, Encoding.UTF8, true);
                _logger.LogInformation($"{fileName} unziped successfully");

                _logger.LogInformation($"Deleting {fileName}...");
                File.Delete(zipFileName);
                _logger.LogInformation($"{fileName} was deleted successfully.");
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible unzip files from {fileName}. Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);

                throw;
            }
        }
    }
}
