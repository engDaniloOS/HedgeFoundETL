using BrazilianHedgeFunds.API.Repository.Interfaces;
using BrazilianHedgeFunds.API.Services.Dtos;
using BrazilianHedgeFunds.API.Services.Interfaces;
using BrazilianHedgeFunds.API.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.PagedList;

namespace BrazilianHedgeFunds.API.Services
{
    public class HedgeFundService : IHedgeFundService
    {
        private readonly ILogger<HedgeFundService> _logger;
        private readonly IHedgeFundRecordRepository _repository;
        private readonly IConfiguration _configuration;

        private const string INVALID_CNPJ = "|Cnpj isn't valid| ";
        private const string INVALID_END_DATE = "|End date isn't valid| ";
        private const string INVALID_PAGE_SIZE = "|Page size should be smaller then 100| ";
        private const string INVALID_START_DATE = "|Start date isn't valid| ";

        private readonly int OldestYearToFetchData;
        private readonly int MaxItensByPage;

        public HedgeFundService(ILogger<HedgeFundService> logger,
                                IConfiguration configuration,
                                IHedgeFundRecordRepository repository)
        {
            _logger = logger;
            _repository = repository;
            _configuration = configuration;

            OldestYearToFetchData = int.Parse(_configuration["Params:OldesYearToProcess"]);
            MaxItensByPage = int.Parse(_configuration["Params:DefaulPageSize"]);
        }

        public async Task<HedgeFundOutDto> GetFundsBy(HedgeFundInDto inDto)
        {
            try
            {
                IsDataValid(inDto);

                if (inDto.StartDate == DateTime.MinValue)
                    inDto.StartDate = new DateTime(OldestYearToFetchData, 1, 1);

                if (inDto.EndDate == DateTime.MinValue)
                    inDto.EndDate = DateTime.Now;

                _logger.LogInformation($"Processing {inDto.CNPJ} from {inDto.StartDate} to {inDto.EndDate}");
                var funds = await _repository.GetFundsBy(inDto);
                _logger.LogInformation($"It was found {funds.Count} records on {inDto.CNPJ} from {inDto.StartDate} to {inDto.EndDate}");

                if (funds.Count == 0)
                    return new HedgeFundOutDto { HedgeFundRecords = new List<HedgeFundRecord>(), IsInvalid = false };

                var pageSize = (inDto.PageSize == 0) ? MaxItensByPage : inDto.PageSize;

                var pageIndex = (inDto.PageIndex == 0) ? 1 : inDto.PageIndex;

                var pagedFunds = await funds.OrderBy(f => f.RecordDate).ToPagedListAsync(pageIndex, pageSize);

                var fundsToReturn = await pagedFunds.OrderBy(f => f.RecordDate).ToListAsync();

                return new HedgeFundOutDto
                {
                    IsInvalid = false,
                    PageInfo = pagedFunds.GetMetaData(),
                    HedgeFundRecords = fundsToReturn
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"It wasn't possible to process this request. Error {ex.Message}");
                return new HedgeFundOutDto { IsInvalid = true, ErrorMessage = ex.Message };
            }
        }

        private void IsDataValid(HedgeFundInDto inDto)
        {
            var builder = new StringBuilder();

            var isAnInvalidCnpj = !Regex.IsMatch(inDto.CNPJ, @"(^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$)");
            if (isAnInvalidCnpj)
                builder.Append(INVALID_CNPJ);

            if (inDto?.StartDate != DateTime.MinValue)
            {
                var isAnInvalidOldDate = (inDto.StartDate.Year < OldestYearToFetchData || inDto.StartDate > DateTime.Now);
                if (isAnInvalidOldDate)
                    builder.Append(INVALID_START_DATE);
            }

            if (inDto?.EndDate != DateTime.MinValue)
            {
                var isAnInvalidNewDate = (inDto.EndDate.Year < OldestYearToFetchData || inDto.EndDate > DateTime.Now);
                if (isAnInvalidNewDate)
                    builder.Append(INVALID_END_DATE);
            }

            var isAmountItensByPageHuge = inDto.PageSize > MaxItensByPage;
            if (isAmountItensByPageHuge) 
                builder.Append(INVALID_PAGE_SIZE);

            var errorMessage = builder.ToString();

            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            throw new ApplicationException(errorMessage);
        }
    }
}
