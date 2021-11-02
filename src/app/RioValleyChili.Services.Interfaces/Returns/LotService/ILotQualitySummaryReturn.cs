using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotQualitySummaryReturn : ILotSummaryReturn
    {
        bool ProductSpecComplete { get; }
        bool ProductSpecOutOfRange { get; }
        IEnumerable<LotQualityStatus> ValidLotQualityStatuses { get; }
        IEnumerable<ILotCustomerAllowanceReturn> CustomerAllowances { get; }
        IEnumerable<ILotCustomerOrderAllowanceReturn> CustomerOrderAllowances { get; }
        IEnumerable<ILotContractAllowanceReturn> ContractAllowances { get; }
    }
}