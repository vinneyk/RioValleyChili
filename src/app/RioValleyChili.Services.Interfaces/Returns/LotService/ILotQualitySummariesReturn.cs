using System.Linq;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotQualitySummariesReturn : IAttributesByProductType
    {
        IQueryable<ILotQualitySummaryReturn> LotSummaries { get; }
    }
}