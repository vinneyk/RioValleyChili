using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IMaterialsReceivedService
    {
        IResult<string> CreateChileMaterialsReceived(ICreateChileMaterialsReceivedParameters parameters);
        IResult<string> UpdateChileMaterialsReceived(IUpdateChileMaterialsReceivedParameters parameters);
        IResult<IQueryable<IChileMaterialsReceivedSummaryReturn>> GetChileMaterialsReceivedSummaries(ChileMaterialsReceivedFilters filters = null);
        IResult<IChileMaterialsReceivedDetailReturn> GetChileMaterialsReceivedDetail(string lotKey);
        IResult<IEnumerable<string>> GetChileVarieties();

        IResult<IChileMaterialsReceivedRecapReturn> GetChileRecapReport(string lotKey);
    }
}