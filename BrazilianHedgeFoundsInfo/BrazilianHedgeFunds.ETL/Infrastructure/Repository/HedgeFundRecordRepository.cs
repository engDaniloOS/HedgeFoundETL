using BrazilianHedgeFunds.ETL.Business.Models;
using BrazilianHedgeFunds.ETL.Services.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Infrastructure.Repository
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

        public async Task SaveFunds(List<HedgeFundRecord> funds)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                _logger.LogInformation("Saving data on DB;");

                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                await context.HedgeFundRecord.AddRangeAsync(funds);
                await context.SaveChangesAsync();

                _logger.LogInformation("The informations were saved successfully.");
            }
            catch (Exception ex)
            {
                var errorMessage = $"It wasn't possible to save/update informations on DB. Exception {ex.Message}";

                _logger.LogError(ex, errorMessage);
                ProcessError.Errors.Add(errorMessage);

                throw;
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
