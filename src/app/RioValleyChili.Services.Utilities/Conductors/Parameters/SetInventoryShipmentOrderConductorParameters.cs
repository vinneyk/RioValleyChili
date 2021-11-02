using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors.Parameters
{
    internal class SetInventoryShipmentOrderConductorParameters<TParams>
        where TParams : ISetOrderParameters
    {
        internal IResult Result
        {
            get { return _result ?? new SuccessResult(); }
            set
            {
                if(_result == null && !value.Success)
                {
                    _result = value;
                }
            }
        }
        private IResult _result;

        internal TParams Params { get; set; }
        internal FacilityKey DestinationFacilityKey { get; set; }
        internal FacilityKey SourceFacilityKey { get; set; }
        internal List<ISchedulePickOrderItemParameter> PickOrderItems { get; set; }

        public SetInventoryShipmentOrderConductorParameters(TParams parameters)
        {
            Params = parameters;

            if(parameters.DestinationFacilityKey != null)
            {
                var facilityKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.DestinationFacilityKey);
                if(!facilityKeyResult.Success)
                {
                    _result = facilityKeyResult;
                    return;
                }

                DestinationFacilityKey = new FacilityKey(facilityKeyResult.ResultingObject);
            }

            if(parameters.SourceFacilityKey != null)
            {
                var sourceKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.SourceFacilityKey);
                if(!sourceKeyResult.Success)
                {
                    _result = sourceKeyResult;
                    return;
                }

                SourceFacilityKey = new FacilityKey(sourceKeyResult.ResultingObject);
            }
                
            if(parameters.InventoryPickOrderItems != null)
            {
                var pickOrderItemsResult = parameters.InventoryPickOrderItems.ToParsedItems();
                if(!pickOrderItemsResult.Success)
                {
                    _result = pickOrderItemsResult;
                    return;
                }
                PickOrderItems = pickOrderItemsResult.ResultingObject.Cast<ISchedulePickOrderItemParameter>().ToList();
            }
        }

        internal IResult<List<SetPickedInventoryItemCodesParameters>> ParseParameters(IEnumerable<ISetPickedInventoryItemCodesParameters> parameters)
        {
            List<SetPickedInventoryItemCodesParameters> setPickedInventoryItemCodes = null;
            if(parameters != null)
            {
                setPickedInventoryItemCodes = new List<SetPickedInventoryItemCodesParameters>();
                foreach(var item in parameters)
                {
                    var pickedInventoryItemKey = KeyParserHelper.ParseResult<IPickedInventoryItemKey>(item.PickedInventoryItemKey);
                    if(!pickedInventoryItemKey.Success)
                    {
                        return pickedInventoryItemKey.ConvertTo<List<SetPickedInventoryItemCodesParameters>>();
                    }

                    setPickedInventoryItemCodes.Add(new SetPickedInventoryItemCodesParameters
                        {
                            Parameters = item,
                            PickedInventoryItemKey = new PickedInventoryItemKey(pickedInventoryItemKey.ResultingObject)
                        });
                }
            }

            return new SuccessResult<List<SetPickedInventoryItemCodesParameters>>(setPickedInventoryItemCodes);
        }
    }
}