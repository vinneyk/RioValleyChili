using System;
using System.Web;
using System.Web.Http.Controllers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class InventoryPickingFilterParametersHelper
    {
        public static FilterInventoryForPickingContextParameters ParseFilterParameters(InventoryOrderEnum pickingContext, HttpActionContext actionContext)
        {
            var routeValues = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
            var contextKey = actionContext.ActionArguments["contextKey"] as string;
            switch (pickingContext)
            {
                case InventoryOrderEnum.ProductionBatch:
                    return new FilterInventoryForBatchParameters
                    {
                        BatchKey = contextKey,
                        IngredientKey = routeValues["ingredientType"],
                    }.ParseCommon(actionContext);

                case InventoryOrderEnum.CustomerOrder:
                    return new FilterInventoryForShipmentOrderParameters
                        {
                            OrderKey = contextKey,
                            OrderItemKey = routeValues["orderItemKey"]
                        }.ParseCommon(actionContext);

                case InventoryOrderEnum.WarehouseMovements:
                    return new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = actionContext.ActionArguments["contextKey"] as string,
                        OrderItemKey = actionContext.ActionArguments["orderItemKey"] as string
                    }.ParseCommon(actionContext);

                case InventoryOrderEnum.Treatments:
                case InventoryOrderEnum.TransWarehouseMovements:
                    return new FilterInventoryForShipmentOrderParameters
                    {
                        OrderKey = contextKey,
                    }.ParseCommon(actionContext);

                default: throw new ArgumentOutOfRangeException("pickingContext");
            }
        }

        public static FilterInventoryForPickingContextParameters ParseCommon(this FilterInventoryForPickingContextParameters parameters, HttpActionContext actionContext)
        {
            //note: Using the values parsed from the querystring in the `ParseFilterParameters` method, these options can be removed from the ActionMethod's signature. 
            //      However, it needs to be noted that any argument values which are provided by the route and are not querystring members need to remain referenced by the
            //      `ActionArguments` collection.
            parameters.LotKey = actionContext.ActionArguments["lotKey"] as string;
            parameters.ProductKey = actionContext.ActionArguments["productKey"] as string;
            parameters.LotType = actionContext.ActionArguments["lotType"] as LotTypeEnum?;
            parameters.HoldType = actionContext.ActionArguments["holdType"] as LotHoldType?;
            parameters.ProductType = (actionContext.ActionArguments["productType"] ?? actionContext.ActionArguments["inventoryType"]) as ProductTypeEnum?;
            parameters.LocationKey = actionContext.ActionArguments["warehouseLocationKey"] as string;
            parameters.PackagingKey = actionContext.ActionArguments["packagingProductKey"] as string;
            parameters.TreatmentKey = actionContext.ActionArguments["treatmentKey"] as string;
            parameters.LocationGroupName = actionContext.ActionArguments["locationGroupName"] as string;

            return parameters;
        }
    }
}