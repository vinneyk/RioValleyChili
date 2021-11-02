using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Utilities.Conductors.Parameters
{
    internal class CreateTreatmentOrderConductorParameters<TParams> : SetInventoryShipmentOrderConductorParameters<TParams>
        where TParams : ICreateTreatmentOrderParameters
    {
        public InventoryTreatmentKey TreatmentKey { get; set; }

        public CreateTreatmentOrderConductorParameters(TParams parameters) : base(parameters)
        {
            if(Result.Success)
            {
                if(parameters.TreatmentKey != null)
                {
                    var treatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.TreatmentKey);
                    if(!treatmentKeyResult.Success)
                    {
                        Result = treatmentKeyResult;
                        return;
                    }
                    TreatmentKey = treatmentKeyResult.ResultingObject.ToInventoryTreatmentKey();
                }
            }
        }
    }
}