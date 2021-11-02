using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class IUpdatePackScheduleParametersExtensions
    {
        internal static void AssertAsExpected(this IUpdatePackScheduleParameters parameters, PackSchedule packSchedule)
        {
            ((ICreatePackScheduleParameters)parameters).AssertAsExpected(packSchedule);
        }
    }
}