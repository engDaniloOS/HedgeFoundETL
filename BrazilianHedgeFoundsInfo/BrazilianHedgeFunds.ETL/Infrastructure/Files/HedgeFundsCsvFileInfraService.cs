using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Files;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Infrastructure.Files
{
    public class HedgeFundsCsvFileInfraService : IHedgeFundsCsvFileInfraService
    {
        private readonly ILogger<HedgeFundsFileManagerInfraService> _logger;

        public HedgeFundsCsvFileInfraService(ILogger<HedgeFundsFileManagerInfraService> logger) => _logger = logger;

        public async Task<List<HedgeFundRecord>> ExtractHedgeFundFrom(string fileName)
        {
            try
            {
                _logger.LogInformation($"Extracting information from {fileName}...");

                var hedgeFunds = new List<HedgeFundRecord>();

                var csvLines = await File.ReadAllLinesAsync(fileName);

                var hasType = csvLines[0].Split(";").Length == 9;

                foreach (var csvLine in csvLines.Skip(1))
                {
                    var fund = BuildHedgeFundFrom(csvLine, hasType);

                    if (!string.IsNullOrWhiteSpace(fund.CNPJ))
                        hedgeFunds.Add(fund);
                }

                _logger.LogInformation($"Informations extracted from {fileName}.");

                return hedgeFunds;
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to extract informations from {fileName}; Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);

                throw;
            }
        }

        private HedgeFundRecord BuildHedgeFundFrom(string csvLine, bool hasType)
        {
            try
            {
                var itens = csvLine.Split(';');
                var index = hasType ? 1 : 0;

                var fund = new HedgeFundRecord
                {
                    Type = hasType ? itens[0] : string.Empty,
                    CNPJ = itens[index],
                    RecordDate = DateTime.ParseExact(itens[++index], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    PortfolioTotalValue = double.Parse(itens[++index], CultureInfo.InvariantCulture),
                    QuotaValue = double.Parse(itens[++index], CultureInfo.InvariantCulture),
                    WorthValue = double.Parse(itens[++index], CultureInfo.InvariantCulture),
                    DayInvestmentsTotalValue = double.Parse(itens[++index], CultureInfo.InvariantCulture),
                    DayWithdrawalsTotalValue = double.Parse(itens[++index], CultureInfo.InvariantCulture),
                    InvestorsTotal = int.Parse(itens[++index]),
                };

                fund.Id = $"{fund.CNPJ}_{fund.RecordDate.ToString("yyyy-MM-dd")}_{Guid.NewGuid()}";

                return fund;
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to extract informations from {csvLine}; Exception: {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);

                return new HedgeFundRecord();
            }
        }
    }
}
