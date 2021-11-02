using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ReceiveInventoryParametersExtensions
    {
        internal static IResult<ReceiveInventoryParameters> ToParsedParameters(this IReceiveInventoryParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(parameters.ProductKey);
            if(!productKeyResult.Success)
            {
                return productKeyResult.ConvertTo<ReceiveInventoryParameters>();
            }
            var productKey = productKeyResult.ResultingObject.ToProductKey();

            PackagingProductKey packagingKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.PackagingReceivedKey))
            {
                var packagingKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(parameters.PackagingReceivedKey);
                if(!packagingKeyResult.Success)
                {
                    return packagingKeyResult.ConvertTo<ReceiveInventoryParameters>();
                }
                packagingKey = packagingKeyResult.ResultingObject.ToPackagingProductKey();
            }

            CompanyKey vendorKey = null;
            if(!string.IsNullOrWhiteSpace(parameters.VendorKey))
            {
                var vendorKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.VendorKey);
                if(!vendorKeyResult.Success)
                {
                    return vendorKeyResult.ConvertTo<ReceiveInventoryParameters>();
                }
                vendorKey = vendorKeyResult.ResultingObject.ToCompanyKey();
            }

            var items = new List<ReceiveInventoryItemParameters>();
            foreach(var item in parameters.Items)
            {
                var parsedItem = item.ToParsedParameters();
                if(!parsedItem.Success)
                {
                    return parsedItem.ConvertTo<ReceiveInventoryParameters>();
                }
                items.Add(parsedItem.ResultingObject);
            }

            return new SuccessResult<ReceiveInventoryParameters>(new ReceiveInventoryParameters
                {
                    Parameters = parameters,
                    ProductKey = productKey,
                    PackagingReceivedKey = packagingKey,
                    VendorKey = vendorKey,
                    Items = items
                });
        }
    }
}