using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotSummaryReturn : ILotBaseReturn
    {
        bool? LoBac { get; }
        int? AstaCalc { get; }
        DateTime AstaCalcDate { get; }
        DateTime LotDateCreated { get; }
        ICompanyHeaderReturn Customer { get; }
        IInventoryProductReturn LotProduct { get; }

        IEnumerable<ILotAttributeReturn> Attributes { get; }
        IEnumerable<ILotDefectReturn> Defects { get; }
    }
}