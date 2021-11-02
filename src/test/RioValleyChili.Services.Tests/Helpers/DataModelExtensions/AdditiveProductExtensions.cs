using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class AdditiveProductExtensions
    {
        internal static void SetAdditiveType(this AdditiveProduct additiveProduct, IAdditiveTypeKey additiveTypeKey)
        {
            if(additiveProduct == null) { throw new ArgumentNullException("additiveProduct"); }
            if(additiveTypeKey == null) { throw new ArgumentNullException("additiveTypeKey"); }

            additiveProduct.AdditiveType = null;
            additiveProduct.AdditiveTypeId = additiveTypeKey.AdditiveTypeKey_AdditiveTypeId;
        }

        internal static void AssertEqual(this AdditiveProduct additiveProduct, IInventoryProductReturn productReturn)
        {
            if(additiveProduct == null) { throw new ArgumentNullException("additiveProduct"); }
            if(productReturn == null) { throw new ArgumentNullException("productReturn"); }

            Assert.AreEqual(new ProductKey((IProductKey)additiveProduct).KeyValue, productReturn.ProductKey);
            Assert.AreEqual(additiveProduct.Product.Name, productReturn.ProductName);
            Assert.AreEqual(additiveProduct.Product.ProductType, productReturn.ProductType);
            Assert.AreEqual(additiveProduct.AdditiveType.Description, productReturn.ProductSubType);
        }
    }
}