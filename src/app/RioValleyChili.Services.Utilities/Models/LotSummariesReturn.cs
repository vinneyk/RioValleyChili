using System.Linq;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotSummariesReturn : AttributesByTypeReturn, ILotQualitySummariesReturn
    {
        public IQueryable<ILotQualitySummaryReturn> LotSummaries { get; internal set; }
    }
}