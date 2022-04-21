using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Services.Infrastructure.Rest
{
    public interface IFetchHedgeFundsFilesInfraService
    {
        Task GetFileData(string url);
    }
}
