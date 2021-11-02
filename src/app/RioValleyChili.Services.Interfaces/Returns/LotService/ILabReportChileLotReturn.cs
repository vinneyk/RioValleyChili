using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILabReportChileLotReturn : ILotBaseReturn, IPackScheduleBaseParametersReturn, IProductionResultBaseReturn
    {
        bool LoBac { get; }
        string ChileProductKey { get; }
        string CustomerKey { get; }
        string CustomerName { get; }
        bool ValidToPick { get; }

        IDictionary<string, IWeightedLotAttributeReturn> Attributes { get; }
        IEnumerable<string> UnresolvedDefects { get; }
        IEnumerable<ILotCustomerAllowanceReturn> CustomerAllowances { get; }
        IEnumerable<IDehydratedInputReturn> DehydratedInputs { get; }
    }
}