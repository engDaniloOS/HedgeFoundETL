using BrazilianHedgeFunds.ETL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Services.Infrastructure.Files
{
    public interface IHedgeFundsCsvFileInfraService
    {
        Task<List<HedgeFundRecord>> ExtractHedgeFundFrom(string fileName);
    }
}
