using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ContractItemExtensions
    {
        internal static ContractItem ConstrainByKeys(this ContractItem contractItem, IContractKey contractKey)
        {
            if(contractItem == null) { throw new ArgumentNullException("contractItem"); }

            if(contractKey != null)
            {
                contractItem.Contract = null;
                contractItem.ContractYear = contractKey.ContractKey_Year;
                contractItem.ContractSequence = contractKey.ContractKey_Sequence;
            }

            return contractItem;
        }

        internal static void AssertEqual(this ContractItem contractItem, IContractItemReturn contractItemReturn)
        {
            if(contractItem == null) { throw new ArgumentNullException("contractItem"); }
            if(contractItemReturn == null) { throw new ArgumentNullException("contractItemReturn"); }

            Assert.AreEqual(new ContractItemKey(contractItem).KeyValue, contractItemReturn.ContractItemKey);
            Assert.AreEqual(new ChileProductKey(contractItem).KeyValue, contractItemReturn.ChileProduct.ProductKey);
            Assert.AreEqual(new PackagingProductKey(contractItem).KeyValue, contractItemReturn.PackagingProduct.ProductKey);
            Assert.AreEqual(new InventoryTreatmentKey(contractItem).KeyValue, contractItemReturn.Treatment.TreatmentKey);
            Assert.AreEqual(contractItem.UseCustomerSpec, contractItemReturn.UseCustomerSpec);
            Assert.AreEqual(contractItem.CustomerProductCode, contractItemReturn.CustomerProductCode);
            Assert.AreEqual(contractItem.Quantity, contractItemReturn.Quantity);
            Assert.AreEqual(contractItem.PriceBase, contractItemReturn.PriceBase);
            Assert.AreEqual(contractItem.PriceFreight, contractItemReturn.PriceFreight);
            Assert.AreEqual(contractItem.PriceTreatment, contractItemReturn.PriceTreatment);
            Assert.AreEqual(contractItem.PriceWarehouse, contractItemReturn.PriceWarehouse);
            Assert.AreEqual(contractItem.PriceRebate, contractItemReturn.PriceRebate);
        }

        internal static SalesOrderItemParameters ToCustomerOrderItemParameters(this ContractItem item, Action<SalesOrderItemParameters> initialize = null)
        {
            var parameters = new SalesOrderItemParameters
                {
                    ContractItemKey = item.ToContractItemKey(),
                    ProductKey = item.ToChileProductKey(),
                    PackagingKey = item.ToPackagingProductKey(),
                    TreatmentKey = item.ToInventoryTreatmentKey(),
                    Quantity = item.Quantity,
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