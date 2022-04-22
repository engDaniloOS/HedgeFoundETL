using System.Threading.Tasks;

namespace BrazilianHedgeFunds.ETL.Business.Services
{
    public interface ITransformHedgeFundsDataService
    {
        Task TransformDataFromCsvToDB();
    }
}
