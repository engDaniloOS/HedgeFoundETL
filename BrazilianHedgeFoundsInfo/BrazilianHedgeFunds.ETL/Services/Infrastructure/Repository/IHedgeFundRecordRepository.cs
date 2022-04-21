using BrazilianHedgeFunds.ETL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Services.Infrastructure.Repository
{
    public interface IHedgeFundRecordRepository
    {
        Task SaveFunds(List<HedgeFundRecord> funds);
    }
}
