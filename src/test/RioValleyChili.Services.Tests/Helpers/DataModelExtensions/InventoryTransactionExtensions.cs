using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryTransactionExtensions
    {
        internal static InventoryTransaction SetSourceLot(this InventoryTransaction t, ILotKey lot)
        {
            t.SourceLot = null;
            t.SourceLotDateCreated = lot.LotKey_DateCreated;
            t.SourceLotDateSequence = lot.LotKey_DateSequence;
            t.SourceLotTypeId = lot.LotKey_LotTypeId;
            return t;
        }

        internal static InventoryTransaction SetDestinationLot(this InventoryTransaction t, ILotKey lot)
        {
            t.DestinationLot = null;

            if(lot != null)
            {
                t.DestinationLotDateCreated = lot.LotKey_DateCreated;
                t.DestinationLotDateSequence = lot.LotKey_DateSequence;
                t.DestinationLotTypeId = lot.LotKey_LotTypeId;
            }
            else
            {
                t.DestinationLotDateCreated = null;
                t.DestinationLotDateSequence = null;
                t.DestinationLotTypeId = null;
            }

            return t;
        }

        internal static void AssertEqual(this InventoryTransaction t, IInventoryTransactionReturn result)
        {
            Assert.AreEqual(t.TransactionType, result.TransactionType);
            Assert.AreEqual(t.SourceLot.ToLotKey().KeyValue, result.SourceLotKey);
            if(t.DestinationLot == null)
            {
                Assert.IsNull(result.DestinationLotKey);
            }
            else
            {
                Assert.AreEqual(t.DestinationLot.ToLotKey().KeyValue, result.DestinationLotKey);
            }
            Assert.AreEqual(t.SourceReference, result.SourceReference);
            Assert.AreEqual(t.Quantity, result.Quantity);
            Assert.AreEqual(t.ToteKey, result.ToteKey);

            if(t.SourceLot.Vendor == null)
            {
                Assert.IsNull(result.SourceLotVendorName);
            }
            else
            {
                Assert.AreEqual(t.SourceLot.Vendor.Name, result.SourceLotVendorName);
            }
            Assert.AreEqual(t.SourceLot.PurchaseOrderNumber, result.SourceLotPurchaseOrderNumber);
            Assert.AreEqual(t.SourceLot.ShipperNumber, result.SourceLotShipperNumber);

            t.PackagingProduct.AssertEqual(result.Packaging);
            t.Location.AssertEqual(result.Location);
            t.Treatment.AssertEqual(result.Treatment);
        }
    }
}