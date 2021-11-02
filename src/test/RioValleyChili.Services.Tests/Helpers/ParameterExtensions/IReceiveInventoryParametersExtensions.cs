using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class IReceiveInventoryParametersExtensions
    {
        internal static void AssertAsExpected(this IReceiveInventoryParameters parameters, Lot lot)
        {
            Assert.AreEqual(parameters.UserToken, lot.Employee.UserName);
            Assert.AreEqual(parameters.LotType, lot.LotTypeEnum);
            Assert.AreEqual(parameters.PurchaseOrderNumber, lot.PurchaseOrderNumber);
            Assert.AreEqual(parameters.ShipperNumber, lot.ShipperNumber);

            if(!string.IsNullOrWhiteSpace(parameters.VendorKey))
            {
                Assert.AreEqual(parameters.VendorKey, lot.Vendor.ToCompanyKey().KeyValue);
            }

            switch(lot.LotTypeEnum.ToProductType())
            {
                case ProductTypeEnum.Additive:
                    Assert.AreEqual(parameters.ProductKey, new AdditiveProductKey(lot.AdditiveLot).KeyValue);
                    Assert.IsNull(lot.ChileLot);
                    Assert.IsNull(lot.PackagingLot);
                    break;

                case ProductTypeEnum.Chile:
                    Assert.AreEqual(parameters.ProductKey, new ChileProductKey(lot.ChileLot).KeyValue);
                    Assert.IsNull(lot.AdditiveLot);
                    Assert.IsNull(lot.PackagingLot);
                    break;

                case ProductTypeEnum.Packaging:
                    Assert.AreEqual(parameters.ProductKey, new PackagingProductKey(lot.PackagingLot).KeyValue);
                    Assert.IsNull(lot.AdditiveLot);
                    Assert.IsNull(lot.ChileLot);
                    break;
            }

            if(string.IsNullOrWhiteSpace(parameters.PackagingReceivedKey))
            {
                Assert.IsTrue(lot.ReceivedPackaging.Weight <= 0.0);
            }
            else
            {
                Assert.AreEqual(parameters.PackagingReceivedKey, new PackagingProductKey(lot).KeyValue);
            }

            var itemParameters = parameters.Items.ToList();
            var inventory = lot.Inventory.ToList();
            Assert.AreEqual(itemParameters.Count, inventory.Count);
            Assert.IsTrue(parameters.Items.All(p => inventory.Count(p.IsAsExpected) == 1));
        }
    }
}