using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    public static class ChileProductExtensions
    {
        public static ChileProduct EmptyProduct(this ChileProduct chileProduct)
        {
            if(chileProduct == null) { throw new ArgumentNullException("chileProduct"); }

            chileProduct.ProductAttributeRanges = null;
            chileProduct.Ingredients = null;

            return chileProduct;
        }

        public static void AssertEqual(this ChileProduct chileProduct, IInventoryProductReturn productReturn)
        {
            if(chileProduct == null) { throw new ArgumentNullException("chileProduct"); }
            if(productReturn == null) { throw new ArgumentNullException("productReturn"); }

            Assert.AreEqual(chileProduct.ToProductKey().KeyValue, productReturn.ProductKey);
            Assert.AreEqual(chileProduct.Product.Name, productReturn.ProductName);
            Assert.AreEqual(chileProduct.Product.ProductType, productReturn.ProductType);
            Assert.AreEqual(chileProduct.ChileType.Description, productReturn.ProductSubType);
        }

        public static void AssertEqual(this ChileProduct chileProduct, IChileProductReturn productReturn)
        {
            if(chileProduct == null) { throw new ArgumentNullException("chileProduct"); }
            if(productReturn == null) { throw new ArgumentNullException("productReturn"); }

            Assert.AreEqual(chileProduct.ToProductKey().KeyValue, productReturn.ProductKey);
            Assert.AreEqual(chileProduct.Product.Name, productReturn.ProductName);
            Assert.AreEqual(chileProduct.ChileState, productReturn.ChileState);
        }
    }
}