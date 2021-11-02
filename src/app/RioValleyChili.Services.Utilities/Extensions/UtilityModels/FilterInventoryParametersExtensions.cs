using System;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.UtilityModels
{
    internal static class FilterInventoryParametersExtensions
    {
        internal static IResult<InventoryPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterInventoryParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var baseResult = (parameters as FilterInventoryParametersBase).ParseToPredicateBuilderFilters();
            if(!baseResult.Success)
            {
                return baseResult;
            }
            var filters = baseResult.ResultingObject;

            if(!string.IsNullOrEmpty(parameters.FacilityKey))
            {
                var warehouseKeyResult = KeyParserHelper.ParseResult<IFacilityKey>(parameters.FacilityKey);
                if(!warehouseKeyResult.Success)
                {
                    return warehouseKeyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                filters.FacilityKey = warehouseKeyResult.ResultingObject;
            }

            if(!string.IsNullOrEmpty(parameters.ToteKey))
            {
                filters.ToteKey = parameters.ToteKey.ToToteKey();
            }

            if(!string.IsNullOrWhiteSpace(parameters.LocationGroupName))
            {
                filters.LocationGroupName = parameters.LocationGroupName;
            }

            return new SuccessResult<InventoryPredicateBuilder.PredicateBuilderFilters>(filters);
        }

        internal static IResult<InventoryPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterInventoryForShipmentOrderParameters parameters,
            out InventoryShipmentOrderKey orderKey, out InventoryPickOrderItemKey orderItemKey)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            orderKey = null;
            orderItemKey = null;

            if(!string.IsNullOrWhiteSpace(parameters.OrderKey))
            {
                var orderKeyResult = KeyParserHelper.ParseResult<IInventoryShipmentOrderKey>(parameters.OrderKey);
                if(!orderKeyResult.Success)
                {
                    return orderKeyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                orderKey = orderKeyResult.ResultingObject.ToInventoryShipmentOrderKey();
            }

            if(!string.IsNullOrWhiteSpace(parameters.OrderItemKey))
            {
                var orderItemKeyResult = KeyParserHelper.ParseResult<IInventoryPickOrderItemKey>(parameters.OrderItemKey);
                if(!orderItemKeyResult.Success)
                {
                    return orderItemKeyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                orderItemKey = orderItemKeyResult.ResultingObject.ToInventoryPickOrderItemKey();
            }

            var baseResult = parameters.ParseToPredicateBuilderFilters();
            if(baseResult.Success)
            {
                var filters = baseResult.ResultingObject;
                return new SuccessResult<InventoryPredicateBuilder.PredicateBuilderFilters>(filters);
            }
            return baseResult;
        }

        internal static IResult<InventoryPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterInventoryForBatchParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var baseResult = (parameters as FilterInventoryParametersBase).ParseToPredicateBuilderFilters();
            if(!baseResult.Success)
            {
                return baseResult;
            }
            var filters = baseResult.ResultingObject;

            if(!string.IsNullOrWhiteSpace(parameters.IngredientKey))
            {
                var additiveTypeKeyResult = KeyParserHelper.ParseResult<IAdditiveTypeKey>(parameters.IngredientKey);
                if(!additiveTypeKeyResult.Success)
                {
                    return additiveTypeKeyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                filters.AdditiveTypeKey = additiveTypeKeyResult.ResultingObject;
            }

            filters.FacilityKey = GlobalKeyHelpers.RinconFacilityKey;

            return new SuccessResult<InventoryPredicateBuilder.PredicateBuilderFilters>(filters);
        }

        private static IResult<InventoryPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterInventoryParametersBase parameters)
        {
            var result = new InventoryPredicateBuilder.PredicateBuilderFilters();

            if(!string.IsNullOrWhiteSpace(parameters.LotKey))
            {
                var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
                if(!lotKeyResult.Success)
                {
                    return lotKeyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                result.LotKey = lotKeyResult.ResultingObject;
            }

            if(!string.IsNullOrWhiteSpace(parameters.ProductKey))
            {
                var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(parameters.ProductKey);
                if(!productKeyResult.Success)
                {
                    return productKeyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                result.ProductKey = productKeyResult.ResultingObject;
            }

            if(!string.IsNullOrWhiteSpace(parameters.TreatmentKey))
            {
                var keyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.TreatmentKey);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                result.TreatmentKey = keyResult.ResultingObject;
            }

            if(!string.IsNullOrWhiteSpace(parameters.PackagingKey))
            {
                var keyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingKey);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                result.PackagingKey = keyResult.ResultingObject;
            }

            if(!string.IsNullOrWhiteSpace(parameters.PackagingReceivedKey))
            {
                var keyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingReceivedKey);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                result.PackagingReceivedKey = keyResult.ResultingObject;
            }

            if(!string.IsNullOrWhiteSpace(parameters.LocationKey))
            {
                var keyResult = KeyParserHelper.ParseResult<ILocationKey>(parameters.LocationKey);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<InventoryPredicateBuilder.PredicateBuilderFilters>();
                }
                result.LocationKey = keyResult.ResultingObject;
            }

            result.ProductType = parameters.ProductType;
            result.LotType = parameters.LotType;
            result.HoldType = parameters.HoldType;
            result.LocationGroupName = parameters.LocationGroupName;

            return new SuccessResult<InventoryPredicateBuilder.PredicateBuilderFilters>(result);
        }
    }
}