using BrazilianHedgeFunds.API.Services.Models;
using System.Collections.Generic;
using X.PagedList;

namespace BrazilianHedgeFunds.API.Services.Dtos
{
    public class HedgeFundOutDto
    {
        public List<HedgeFundRecord> HedgeFundRecords { get; set; }
        public bool IsInvalid { get; set; }
        public string ErrorMessage { get; set; }
        public PagedListMetaData PageInfo { get; set; }
    }
}
