using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class PickedInventoryItemExtensions
    {
        internal static PickedInventoryItem SetFromLocation(this PickedInventoryItem item, ILocationKey fromLocationKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            item.FromLocation = null;
            item.FromLocationId = fromLocationKey.LocationKey_Id;

            return item;
        }

        internal static PickedInventoryItem SetCurrentLocation(this PickedInventoryItem item, ILocationKey currentLocationKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            item.CurrentLocation = null;
            item.CurrentLocationId = currentLocationKey.LocationKey_Id;

            return item;
        }

        internal static PickedInventoryItem SetCurrentLocationToSource(this PickedInventoryItem item)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            item.CurrentLocation = null;
            item.CurrentLocationId = item.FromLocationId;

            return item;
        }

        internal static PickedInventoryItem ConstrainByKeys(this PickedInventoryItem item, IPickedInventoryKey pickedInventoryKey = null, ILotKey lotKey = null, IPackagingProductKey packagingProductKey = null, IInventoryTreatmentKey treatmentKey = null, ILocationKey sourceLocationKey = null, ILocationKey currentLocationKey = null, string toteKey = null)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(pickedInventoryKey != null)
            {
                item.PickedInventory = null;
                item.DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated;
                item.Sequence = pickedInventoryKey.PickedInventoryKey_Sequence;
            }

            if(lotKey != null)
            {
                item.Lot = null;
                item.LotDateCreated = lotKey.LotKey_DateCreated;
                item.LotDateSequence = lotKey.LotKey_DateSequence;
                item.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(packagingProductKey != null)
            {
                item.PackagingProduct = null;
                item.PackagingProductId = packagingProductKey.PackagingProductKey_ProductId;
            }

            if(treatmentKey != null)
            {
                item.Treatment = null;
                item.TreatmentId = treatmentKey.InventoryTreatmentKey_Id;
            }

            if(sourceLocationKey != null)
            {
                item.FromLocation = null;
                item.FromLocationId = sourceLocationKey.LocationKey_Id;
            }

            if(currentLocationKey != null)
            {
                item.CurrentLocation = null;
                item.CurrentLocationId = currentLocationKey.LocationKey_Id;
            }

            if(toteKey != null)
            {
                item.ToteKey = toteKey;
            }

            return item;
        }

        internal static PickedInventoryItem SetSourceWarehouse(this PickedInventoryItem item, IFacilityKey facilityKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }
            if(facilityKey == null) { throw new ArgumentNullException("facilityKey"); }

            if(item.FromLocation != null)
            {
                item.FromLocation.ConstrainByKeys(facilityKey);
            }

            return item;
        }

        internal static PickedInventoryItem SetToInventory(this PickedInventoryItem item, IInventoryKey inventoryKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }
            if(inventoryKey == null) { throw new ArgumentNullException("inventoryKey"); }

            return item.ConstrainByKeys(null, inventoryKey, inventoryKey, inventoryKey, inventoryKey, inventoryKey, inventoryKey.InventoryKey_ToteKey);
        }

        internal static PickedInventoryItem SetPicked(this PickedInventoryItem item, int quantity, double packagingWeight)
        {
            if(item == null) { throw new ArgumentNullException("item"); }
            
            item.Quantity = quantity;
            item.PackagingProduct.Weight = packagingWeight;

            return item;
        }

        internal static PickedInventoryItem NullCustomerCodes(this PickedInventoryItem item)
        {
            if(item == null) { throw new ArgumentNullException("item"); }
            item.CustomerProductCode = item.CustomerLotCode = null;
            return item;
        }
    }
}