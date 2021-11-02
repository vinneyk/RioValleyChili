using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class PackagingProductExtensions
    {
        internal static void AssertEqual(this PackagingProduct packagingProduct, IPackagingProductReturn packagingProductReturn)
        {
            if(packagingProduct == null) { throw new ArgumentNullException("packagingProduct"); }
            if(packagingProductReturn == null) { throw new ArgumentNullException("packagingProductReturn"); }

            Assert.AreEqual(new PackagingProductKey(packagingProduct).KeyValue, packagingProductReturn.ProductKey);
            Assert.AreEqual(packagingProduct.Product.Name, packagingProductReturn.ProductName);
            Assert.AreEqual(packagingProduct.Weight, packagingProductReturn.Weight);
        }

        internal static void AssertEqual(this PackagingProduct packagingProduct, IInventoryProductReturn productReturn)
        {
            if(packagingProduct == null) { throw new ArgumentNullException("packagingProduct"); }
            if(productReturn == null) { throw new ArgumentNullException("productReturn"); }

            Assert.AreEqual(new ProductKey((IProductKey)packagingProduct).KeyValue, productReturn.ProductKey);
            Assert.AreEqual(packagingProduct.Product.Name, productReturn.ProductName);
            Assert.AreEqual(packagingProduct.Product.ProductType, productReturn.ProductType);
        }
    }
}