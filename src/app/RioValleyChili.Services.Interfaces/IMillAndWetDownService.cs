using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Interfaces.Returns.MillAndWetdownService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IMillAndWetDownService
    {
        IResult<string> CreateMillAndWetdown(ICreateMillAndWetdownParameters parameters);
        IResult UpdateMillAndWetdown(IUpdateMillAndWetdownParameters parameters);
        IResult DeleteMillAndWetdown(string lotKey);
        IResult<IMillAndWetdownDetailReturn> GetMillAndWetdownDetail(string lotKey);
        IResult<IQueryable<IMillAndWetdownSummaryReturn>> GetMillAndWetdownSummaries();
    }
}