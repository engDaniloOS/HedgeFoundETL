using BrazilianHedgeFunds.API.Repository.Interfaces;
using BrazilianHedgeFunds.API.Services.Dtos;
using BrazilianHedgeFunds.API.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.API.Repository
{
    public class HedgeFundRecordRepository : IHedgeFundRecordRepository
    {
        private readonly ILogger<HedgeFundRecordRepository> _logger;
        private readonly IDbContextFactory<Context> _contextFactory;

        public HedgeFundRecordRepository(IDbContextFactory<Context> contextFactory, ILogger<HedgeFundRecordRepository> logger)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<List<HedgeFundRecord>> GetFundsBy(HedgeFundInDto inDto)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                _logger.LogInformation($"Get information on DB to {inDto.CNPJ} from {inDto.StartDate} to {inDto.EndDate}");

                return await context.HedgeFundRecord
                    .Where(f => f.CNPJ == inDto.CNPJ && f.RecordDate >= inDto.StartDate && f.RecordDate <= inDto.EndDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"It wasn't possible to fetch informations from {inDto.CNPJ} on DB. Error: {ex.Message}");
                throw;
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
