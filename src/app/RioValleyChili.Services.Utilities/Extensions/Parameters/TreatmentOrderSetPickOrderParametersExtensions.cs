using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.OrderInventoryServiceComponent;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class TreatmentOrderSetPickOrderParametersExtensions
    {
        internal static IResult<TreatmentOrderSetPickOrderParameters> ToTreatmentOrderParsedParameters(this ISetInventoryPickOrderParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var treatmentOrderKey = KeyParserHelper.ParseResult<ITreatmentOrderKey>(parameters.OrderKey);
            if(!treatmentOrderKey.Success)
            {
                return treatmentOrderKey.ConvertTo<TreatmentOrderSetPickOrderParameters>();
            }

            var parsedItems = parameters.ToParsedItems();
            if(!parsedItems.Success)
            {
                return parsedItems.ConvertTo<TreatmentOrderSetPickOrderParameters>();
            }

            return new SuccessResult<TreatmentOrderSetPickOrderParameters>(new TreatmentOrderSetPickOrderParameters
                {
                    Parameters = parameters,
                    TreatmentOrderKey = new TreatmentOrderKey(treatmentOrderKey.ResultingObject),
                    PickedItems = parsedItems.ResultingObject
                });
        }

        internal static IResult<List<InventoryPickOrderItemParameter>> ToParsedItems(this ISetInventoryPickOrderParameters parameters)
        {
            if(parameters.InventoryPickOrderItems == null)
            {
                return new InvalidResult<List<InventoryPickOrderItemParameter>>(null, "Inventory Pick Order Items required.");
            }

            return parameters.InventoryPickOrderItems.ToParsedItems();
        }

        internal static IResult<List<InventoryPickOrderItemParameter>> ToParsedItems(this IEnumerable<ISetInventoryPickOrderItemParameters> parameters)
        {
            var pickedItems = new List<InventoryPickOrderItemParameter>();
            foreach(var item in parameters)
            {
                if(item.Quantity <= 0)
                {
                    return new InvalidResult<List<InventoryPickOrderItemParameter>>(null, "Quantity cannot be less than or equal to 0.");
                }

                var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(item.ProductKey);
                if(!productKeyResult.Success)
                {
                    return productKeyResult.ConvertTo<List<InventoryPickOrderItemParameter>>();
                }

                var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(item.PackagingKey);
                if(!packagingProductKeyResult.Success)
                {
                    return packagingProductKeyResult.ConvertTo<List<InventoryPickOrderItemParameter>>();
                }

                IInventoryTreatmentKey treatmentKey = StaticInventoryTreatments.NoTreatment;
                if(!string.IsNullOrEmpty(item.TreatmentKey))
                {
                    var treatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(item.TreatmentKey);
                    if(!treatmentKeyResult.Success)
                    {
                        return treatmentKeyResult.ConvertTo<List<InventoryPickOrderItemParameter>>();
                    }
                    treatmentKey = treatmentKeyResult.ResultingObject;
                }

                ICustomerKey customer = null;
                if(!string.IsNullOrWhiteSpace(item.CustomerKey))
                {
                    var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(item.CustomerKey);
                    if(!customerKeyResult.Success)
                    {
                        return customerKeyResult.ConvertTo<List<InventoryPickOrderItemParameter>>();
                    }
                    customer = customerKeyResult.ResultingObject;
                }

                pickedItems.Add(new InventoryPickOrderItemParameter(productKeyResult.ResultingObject, packagingProductKeyResult.ResultingObject, treatmentKey, item.Quantity, customer, item.CustomerProductCode, item.CustomerLotCode));
            }

            return new SuccessResult<List<InventoryPickOrderItemParameter>>(pickedItems);
        }
    }
}