using System.Threading;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Business.Interfaces
{
    public interface IPrepareEdgeFundsFileDataService
    {
        Task<bool> PrepareData(CancellationToken stoppingToken);
        void CleanTempFiles();
    }
}
