using BrazilianHedgeFunds.ETL.Business.Interfaces;
using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Files;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Services
{
    public class PrepareEdgeFundsFilesService : IPrepareEdgeFundsFileDataService
    {
        private readonly ILogger<PrepareEdgeFundsFilesService> _logger;
        private readonly IFetchHedgeFundsFilesInfraService _fetchService;
        private readonly IHedgeFundsFileManagerInfraService _fileService;
        private readonly IConfiguration _configuration;

        private readonly string _mainPathToGetData;
        private readonly string _filePath;
        private readonly int _yearsWithoutHist;

        public PrepareEdgeFundsFilesService(IConfiguration configuration,
                                           IFetchHedgeFundsFilesInfraService fetchService,
                                           IHedgeFundsFileManagerInfraService fileService,
                                           ILogger<PrepareEdgeFundsFilesService> logger)
        {
            _logger = logger;
            _fetchService = fetchService;
            _configuration = configuration;
            _fileService = fileService;

            _filePath = _configuration["Defaults:filePath"];
            _mainPathToGetData = _configuration["Rest:OnlineDataSourcePath"];
            _yearsWithoutHist = int.Parse(_configuration["Defaults:YearsWithoutHist"]);
        }

        public async Task<bool> PrepareData(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Preparing files from data transform...");
                
                Directory.CreateDirectory(_filePath);

                var urlsToGetHistData = BuildUrlsToGetHist();

                var urls = new List<string>();
                urls.AddRange(urlsToGetHistData);
                urls.AddRange(BuildUrlsToGetFiles());

                await FetchHistData(urls, stoppingToken);
                await ExtractFiles(urlsToGetHistData, stoppingToken);

                _logger.LogInformation("Files ready to be transformed");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Errors when trying to prepare files to data transform.");
                return false;
            }
        }

        public void CleanTempFiles()
        {
            try
            {
                Directory.Delete(_filePath, true);
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to delete temporary files. Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);
            }
        }

        private Task ExtractFiles(List<string> histUrls, CancellationToken stoppingToken)
        {
            var tasksToUnzipFiles = new List<Task>();

            foreach (var url in histUrls)
            {
                var fileName = url.Split('/').Last();
                var task = Task.Run(() => _fileService.UnzipFile(fileName), stoppingToken);
                tasksToUnzipFiles.Add(task);
            }

            return Task.WhenAll(tasksToUnzipFiles);
        }

        private Task FetchHistData(List<string> urlsToGetHistData, CancellationToken stoppingToken)
        {
            var tasksToGetHistData = new List<Task>();

            foreach (var url in urlsToGetHistData)
            {
                var task = Task.Run(() => _fetchService.GetFileData(url), stoppingToken);
                tasksToGetHistData.Add(task);
            }

            return Task.WhenAll(tasksToGetHistData);
        }

        private List<string> BuildUrlsToGetHist()
        {
            _logger.LogInformation("Building history urls to download.");

            var histFileName = _configuration["Rest:DefaultHistFileName"];
            var histPathToGetData = _configuration["Rest:OnlineDataSourceHistPath"];

            var oldestYearToGetData = 
                (Program.CommandOldestDateToProcess == DateTime.MinValue) ?
                    int.Parse(_configuration["Defaults:OldestYear"]) :
                    Program.CommandOldestDateToProcess.Year;

            var newestYearToGetHistData = DateTime.Now.Year - _yearsWithoutHist;
            var urlsToGetHist = new List<string>();

            for (int year = oldestYearToGetData; year <= newestYearToGetHistData; year++)
            {
                var url = $"{_mainPathToGetData}{histPathToGetData}{histFileName}".Replace("{YYYY}", year.ToString());
                urlsToGetHist.Add(url);
            }

            _logger.LogInformation("History urls to download were built.");

            return urlsToGetHist;
        }

        private List<string> BuildUrlsToGetFiles()
        {
            _logger.LogInformation("Building urls' file to download.");

            var fileName = _configuration["Rest:DefaultFileName"];
            var firtsYearToGetFileData = DateTime.Now.Year - _yearsWithoutHist + 1;

            var firtsDateToGetFileData =
                    (Program.CommandOldestDateToProcess == DateTime.MinValue || Program.CommandOldestDateToProcess.Year < firtsYearToGetFileData) ?
                        new DateTime(firtsYearToGetFileData, 1, 1) :
                        Program.CommandOldestDateToProcess;

            var urlsToGetFiles = new List<string>();

            for (var data = firtsDateToGetFileData; data <= DateTime.Now; data = data.AddMonths(1))
            {
                var url = $"{_mainPathToGetData}{fileName}"
                        .Replace("{YYYY}", data.ToString("yyyy"))
                        .Replace("{MM}", data.ToString("MM"));

                urlsToGetFiles.Add(url);
            }

            _logger.LogInformation("Urls to download were built.");

            return urlsToGetFiles;
        }
    }
}
