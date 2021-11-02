using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class CustomerOrderItemExtensions
    {
        internal static SalesOrderItem ConstrainByKeys(this SalesOrderItem item, ISalesOrderKey order = null, IContractItemKey contractItem = null, IChileProductKey chileProduct = null, IPackagingProductKey packagingProduct = null, IInventoryTreatmentKey treatment = null)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(order != null)
            {
                item.Order = null;
                item.DateCreated = order.SalesOrderKey_DateCreated;
                item.Sequence = order.SalesOrderKey_Sequence;

                if(item.InventoryPickOrderItem != null)
                {
                    item.InventoryPickOrderItem.InventoryPickOrder = null;
                }
            }

            if(contractItem != null)
            {
                item.ContractItem = null;
                item.ContractYear = contractItem.ContractKey_Year;
                item.ContractSequence = contractItem.ContractKey_Sequence;
                item.ContractItemSequence = contractItem.ContractItemKey_Sequence;
            }

            if(chileProduct != null)
            {
                item.InventoryPickOrderItem.Product = null;
                item.InventoryPickOrderItem.ProductId = chileProduct.ChileProductKey_ProductId;
            }

            if(packagingProduct != null)
            {
                item.InventoryPickOrderItem.PackagingProduct = null;
                item.InventoryPickOrderItem.PackagingProductId = packagingProduct.PackagingProductKey_ProductId;
            }

            if(treatment != null)
            {
                item.InventoryPickOrderItem.InventoryTreatment = null;
                item.InventoryPickOrderItem.TreatmentId = treatment.InventoryTreatmentKey_Id;
            }

            return item;
        }

        internal static SalesOrderItem SetToContractItem(this SalesOrderItem item)
        {
            return item.ConstrainByKeys(null, null, item.ContractItem, item.ContractItem, item.ContractItem);
        }

        internal static SalesOrderItem SetContractKey(this SalesOrderItem salesOrderItem, IContractKey contractKey)
        {
            if(salesOrderItem == null) { throw new ArgumentNullException("salesOrderItem"); }
            if(contractKey == null) { throw new ArgumentNullException("contractKey"); }

            salesOrderItem.ContractYear = contractKey.ContractKey_Year;
            salesOrderItem.ContractSequence = contractKey.ContractKey_Sequence;

            return salesOrderItem;
        }

        internal static SalesOrderItem SetPrices(this SalesOrderItem salesOrderItem, double? @base, double? freight = null, double? treatment = null, double? warehouse = null, double? rebate = null)
        {
            if(salesOrderItem == null) { throw new ArgumentNullException("salesOrderItem"); }

            salesOrderItem.PriceBase = @base ?? salesOrderItem.PriceBase;
            salesOrderItem.PriceFreight = freight ?? salesOrderItem.PriceFreight;
            salesOrderItem.PriceTreatment = treatment ?? salesOrderItem.PriceTreatment;
            salesOrderItem.PriceWarehouse = warehouse ?? salesOrderItem.PriceWarehouse;
            salesOrderItem.PriceRebate = rebate ?? salesOrderItem.PriceRebate;

            return salesOrderItem;
        }

        internal static void AssertEqual(this SalesOrderItem expected, ISalesOrderItem result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(result == null) { throw new ArgumentNullException("result"); }

            if(expected.ContractItem == null)
            {
                Assert.IsNull(result.ContractItemKey);
            }
            else
            {
                Assert.AreEqual(expected.ContractItem.ToContractItemKey().KeyValue, result.ContractItemKey);
            }

            Assert.AreEqual(expected.InventoryPickOrderItem.ToProductKey().KeyValue, result.ProductKey);
            Assert.AreEqual(expected.InventoryPickOrderItem.ToPackagingProductKey().KeyValue, result.PackagingKey);
            Assert.AreEqual(expected.InventoryPickOrderItem.ToInventoryTreatmentKey().KeyValue, result.TreatmentKey);
            Assert.AreEqual(expected.InventoryPickOrderItem.Quantity, result.Quantity);
            Assert.AreEqual(expected.PriceBase, result.PriceBase);
            Assert.AreEqual(expected.PriceFreight, result.PriceFreight);
            Assert.AreEqual(expected.PriceTreatment, result.PriceTreatment);
            Assert.AreEqual(expected.PriceWarehouse, result.PriceWarehouse);
            Assert.AreEqual(expected.PriceRebate, result.PriceRebate);
            Assert.AreEqual(expected.InventoryPickOrderItem.CustomerLotCode, result.CustomerLotCode);
            Assert.AreEqual(expected.InventoryPickOrderItem.CustomerProductCode, result.CustomerProductCode);
        }

        internal static void AssertEqual(this SalesOrderItem expected, ISalesOrderItemReturn itemReturn)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(itemReturn == null) { throw new ArgumentNullException("itemReturn"); }

            Assert.AreEqual(expected.ToSalesOrderItemKey().KeyValue, itemReturn.OrderItemKey);

            if(expected.ContractItem == null)
            {
                Assert.IsNull(itemReturn.ContractItemKey);
            }
            else
            {
                Assert.AreEqual(expected.ContractItem.ToContractItemKey().KeyValue, itemReturn.ContractItemKey);
            }
            
            Assert.AreEqual(expected.InventoryPickOrderItem.ToProductKey().KeyValue, itemReturn.ProductKey);
            Assert.AreEqual(expected.InventoryPickOrderItem.ToPackagingProductKey().KeyValue, itemReturn.PackagingProductKey);
            Assert.AreEqual(expected.InventoryPickOrderItem.ToInventoryTreatmentKey().KeyValue, itemReturn.TreatmentKey);
            Assert.AreEqual(expected.InventoryPickOrderItem.Quantity, itemReturn.Quantity);
            Assert.AreEqual(expected.InventoryPickOrderItem.CustomerLotCode, itemReturn.CustomerLotCode);
            Assert.AreEqual(expected.InventoryPickOrderItem.CustomerProductCode, itemReturn.CustomerProductCode);
            Assert.AreEqual(expected.PriceBase, itemReturn.PriceBase);
            Assert.AreEqual(expected.PriceFreight, itemReturn.PriceFreight);
            Assert.AreEqual(expected.PriceTreatment, itemReturn.PriceTreatment);
            Assert.AreEqual(expected.PriceWarehouse, itemReturn.PriceWarehouse);
            Assert.AreEqual(expected.PriceRebate, itemReturn.PriceRebate);
        }

        internal static void AssertEqual(this SalesOrderItem expected, ICustomerContractOrderItemReturn result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }
            if(result == null) { throw new ArgumentNullException("result"); }

            Assert.AreEqual(new SalesOrderItemKey((ISalesOrderItemKey)expected).KeyValue, result.OrderItemKey);

            if(expected.ContractItem == null)
            {
                Assert.IsNull(result.ContractItemKey);
            }
            else
            {
                Assert.AreEqual(expected.ContractItem.ToContractItemKey().KeyValue, result.ContractItemKey);
            }

            Assert.AreEqual(new ProductKey(expected.InventoryPickOrderItem.Product).KeyValue, result.Product.ProductKey);
            Assert.AreEqual(new PackagingProductKey(expected.InventoryPickOrderItem.PackagingProduct).KeyValue, result.Packaging.ProductKey);
            Assert.AreEqual(new InventoryTreatmentKey(expected.InventoryPickOrderItem.InventoryTreatment).KeyValue, result.Treatment.TreatmentKey);

            var totalPrice = expected.PriceBase + expected.PriceFreight + expected.PriceTreatment + expected.PriceWarehouse - expected.PriceRebate;
            Assert.AreEqual(totalPrice, result.TotalPrice);
        }

        internal static SalesOrderItemParameters ToCustomerOrderItemParameters(this SalesOrderItem item, Action<SalesOrderItemParameters> initialize = null)
        {
            var parameters = new SalesOrderItemParameters
                {
                    ContractItemKey = item.ContractItem == null ? null : item.ContractItem.ToContractItemKey(),
                    ProductKey = item.InventoryPickOrderItem.ToProductKey(),
                    PackagingKey = item.InventoryPickOrderItem.ToPackagingProductKey(),
                    TreatmentKey = item.InventoryPickOrderItem.ToInventoryTreatmentKey(),
                    Quantity = item.InventoryPickOrderItem.Quantity,
                    PriceBase = item.PriceBase,
                    PriceFreight = item.PriceFreight,
                    PriceTreatment = item.PriceTreatment,
                    PriceWarehouse = item.PriceWarehouse,
                    PriceRebate = item.PriceRebate
                };

            if(initialize != null)
            {
                initialize(parameters);
            }

            return parameters;
        }
    }
}