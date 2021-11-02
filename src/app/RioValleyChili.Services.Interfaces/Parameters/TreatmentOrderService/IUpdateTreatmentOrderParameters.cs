using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService
{
    public interface IUpdateTreatmentOrderParameters : ICreateTreatmentOrderParameters
    {
        string TreatmentOrderKey { get; }
        IEnumerable<ISetPickedInventoryItemCodesParameters> PickedInventoryItemCodes { get; }
    }
}