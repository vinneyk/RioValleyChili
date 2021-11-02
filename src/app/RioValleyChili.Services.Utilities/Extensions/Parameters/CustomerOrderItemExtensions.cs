using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CustomerOrderItemExtensions
    {
        internal static IResult<List<SetSalesOrderItemParameters>> ToParsedParametersList(this IEnumerable<ISalesOrderItem> orderItems)
        {
            if(orderItems == null) { throw new ArgumentNullException("orderItems"); }

            var parsedItems = new List<SetSalesOrderItemParameters>();
            foreach(var item in orderItems)
            {
                var parseResult = item.ToParsedParameters();
                if(!parseResult.Success)
                {
                    return parseResult.ConvertTo<List<SetSalesOrderItemParameters>>();
                }
                parsedItems.Add(parseResult.ResultingObject);
            }

            return new SuccessResult<List<SetSalesOrderItemParameters>>(parsedItems);
        }

        private static IResult<SetSalesOrderItemParameters> ToParsedParameters(this ISalesOrderItem orderItem)
        {
            if(orderItem == null) { throw new ArgumentNullException("orderItem"); }

            IContractItemKey contractItemKey = null;
            if(!string.IsNullOrWhiteSpace(orderItem.ContractItemKey))
            {
                var contractItemKeyResult = KeyParserHelper.ParseResult<IContractItemKey>(orderItem.ContractItemKey);
                if(!contractItemKeyResult.Success)
                {
                    return contractItemKeyResult.ConvertTo<SetSalesOrderItemParameters>();
                }

                contractItemKey = contractItemKeyResult.ResultingObject;
            }

            var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(orderItem.ProductKey);
            if(!productKeyResult.Success)
            {
                return productKeyResult.ConvertTo<SetSalesOrderItemParameters>();
            }
            
            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(orderItem.PackagingKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo<SetSalesOrderItemParameters>();
            }

            var treatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(orderItem.TreatmentKey);
            if(!treatmentKeyResult.Success)
            {
                return treatmentKeyResult.ConvertTo<SetSalesOrderItemParameters>();
            }

            return new SuccessResult<SetSalesOrderItemParameters>(new SetSalesOrderItemParameters
                {
                    SalesOrderItem = orderItem,
                    ContractItemKey = contractItemKey == null ? null : contractItemKey.ToContractItemKey(),
                    ProductKey = productKeyResult.ResultingObject.ToProductKey(),
                    PackagingProductKey = packagingProductKeyResult.ResultingObject.ToPackagingProductKey(),
                    InventoryTreatmentKey = treatmentKeyResult.ResultingObject.ToInventoryTreatmentKey()
                });
        }
    }
}