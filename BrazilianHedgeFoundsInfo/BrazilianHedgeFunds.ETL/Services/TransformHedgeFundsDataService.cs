using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Business.Services;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Files;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Repository;
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
    public class TransformHedgeFundsDataService : ITransformHedgeFundsDataService
    {
        private readonly IConfiguration _configuration;
        private readonly IHedgeFundRecordRepository _repository;
        private readonly IHedgeFundsCsvFileInfraService _csvService;
        private readonly ILogger<TransformHedgeFundsDataService> _logger;

        public TransformHedgeFundsDataService(IConfiguration configuration,
                                       IHedgeFundRecordRepository repository,
                                       IHedgeFundsCsvFileInfraService csvService,
                                       ILogger<TransformHedgeFundsDataService> logger)
        {
            _logger = logger;
            _repository = repository;
            _csvService = csvService;
            _configuration = configuration;
        }

        public async Task TransformDataFromCsvToDB(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting process to transform data...");

                var filesPath = _configuration["Defaults:filePath"];
                var filesNames = Directory.GetFiles(filesPath, "*.csv").ToList();
                var filesProcessed = 0;

                foreach (var fileName in filesNames)
                {
                    _logger.LogInformation($"Processing file {filesProcessed + 1}/{filesNames.Count + 1}...");
                    List<HedgeFundRecord> funds = await ExtractDataFromFile(fileName);
                    
                    _logger.LogInformation($"Saving {funds.Count} records from {fileName} on database...");
                    await SaveDataOnDB(funds);

                    funds.Clear();
                    filesProcessed++;
                }

                _logger.LogInformation("Data transformed successfully");

            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to extract and save data on DB. Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);
            }
        }

        private async Task SaveDataOnDB(List<HedgeFundRecord> funds)
        {
            try
            {
                var limitSizeToInsert = 20_000;

                if (funds.Count == 0) return;

                if (funds.Count <= limitSizeToInsert)
                {
                    await _repository.SaveFunds(funds);
                    return;
                }

                var lastItensAmount = funds.Count % limitSizeToInsert;

                for (int i = 0; i < (funds.Count - lastItensAmount); i+= limitSizeToInsert)
                {
                    _logger.LogInformation($"Saving {i + limitSizeToInsert} records from {funds.Count} on database...");

                    var fundsToSave = funds.Skip(i).Take(limitSizeToInsert);
                    await _repository.SaveFunds(fundsToSave.ToList());
                }

                _logger.LogInformation($"Saving {funds.Count} records from {funds.Count} on database...");
                var lastItens = funds.TakeLast(lastItensAmount);
                await _repository.SaveFunds(lastItens.ToList());

                funds.Clear();
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible save data on DB. Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);
            }
        }

        private async Task<List<HedgeFundRecord>> ExtractDataFromFile(string fileName)
        {
            try
            {
                return await _csvService.ExtractHedgeFundFrom(fileName);
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to extract data from {fileName}. Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);

                return new List<HedgeFundRecord>();
            }
        }
    }
}
