using BrazilianHedgeFunds.API.Services.Dtos;
using BrazilianHedgeFunds.API.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.API.Repository.Interfaces
{
    public interface IHedgeFundRecordRepository
    {
        Task<List<HedgeFundRecord>> GetFundsBy(HedgeFundInDto inDto);
    }
}
