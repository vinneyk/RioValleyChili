using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.UtilityModels
{
    internal static class FilterInterWarehouseOrderParametersExtensions
    {
        internal static IResult<Expression<Func<InventoryShipmentOrder, bool>>> ParseToPredicate(this FilterInterWarehouseOrderParameters parameters)
        {
            var predicate = InventoryShipmentOrderPredicates.ByOrderType(InventoryShipmentOrderTypeEnum.InterWarehouseOrder);
            
            if(parameters != null)
            {
                if(parameters.OriginFacilityKey != null)
                {
                    var facilityKey = KeyParserHelper.ParseResult<IFacilityKey>(parameters.OriginFacilityKey);
                    if(!facilityKey.Success)
                    {
                        return facilityKey.ConvertTo<Expression<Func<InventoryShipmentOrder, bool>>>();
                    }

                    predicate = predicate.And(InventoryShipmentOrderPredicates.ByOriginFacility(new FacilityKey(facilityKey.ResultingObject)).ExpandAll());
                }

                if(parameters.DestinationFacilityKey != null)
                {
                    var facilityKey = KeyParserHelper.ParseResult<IFacilityKey>(parameters.DestinationFacilityKey);
                    if(!facilityKey.Success)
                    {
                        return facilityKey.ConvertTo<Expression<Func<InventoryShipmentOrder, bool>>>();
                    }

                    predicate = predicate.And(InventoryShipmentOrderPredicates.ByDestinationFacility(new FacilityKey(facilityKey.ResultingObject)).ExpandAll());
                }
            }

            return new SuccessResult<Expression<Func<InventoryShipmentOrder, bool>>>(predicate);
        }
    }
}