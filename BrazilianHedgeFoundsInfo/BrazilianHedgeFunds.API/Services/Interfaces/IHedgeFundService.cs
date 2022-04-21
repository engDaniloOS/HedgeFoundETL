using BrazilianHedgeFunds.API.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.API.Services.Interfaces
{
    public interface IHedgeFundService
    {
        Task<HedgeFundOutDto> GetFundsBy(HedgeFundInDto inDto);
    }
}
