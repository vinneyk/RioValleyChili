using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.TreatmentOrderService;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Utilities.Conductors.Parameters
{
    internal class UpdateTreatmentOrderConductorParameters<TParams> : CreateTreatmentOrderConductorParameters<TParams>
        where TParams : IUpdateTreatmentOrderParameters
    {
        public TreatmentOrderKey TreatmentOrderKey { get; set; }
        public List<SetPickedInventoryItemCodesParameters> SetPickedInventoryItemCodes { get; set; }

        public UpdateTreatmentOrderConductorParameters(TParams parameters)
            : base(parameters)
        {
            if(Result.Success)
            {
                var keyResult = KeyParserHelper.ParseResult<ITreatmentOrderKey>(parameters.TreatmentOrderKey);
                if(!keyResult.Success)
                {
                    Result = keyResult;
                    return;
                }
                TreatmentOrderKey = new TreatmentOrderKey(keyResult.ResultingObject);

                var setItemCodesResult = ParseParameters(parameters.PickedInventoryItemCodes);
                if(!setItemCodesResult.Success)
                {
                    Result = setItemCodesResult;
                    return;
                }

                SetPickedInventoryItemCodes = setItemCodesResult.ResultingObject;
            }
        }
    }
}