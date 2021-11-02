using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryExtensions
    {
        internal static Inventory ConstrainByKeys(this Inventory inventory, ILotKey lotKey = null, IPackagingProductKey packagingProductKey = null, ILocationKey locationKey = null, IInventoryTreatmentKey treatment = null, IFacilityKey facility = null, string toteKey = null)
        {
            if(inventory == null) { throw new ArgumentNullException("inventory"); }

            if(lotKey != null)
            {
                inventory.Lot = null;
                inventory.LotDateCreated = lotKey.LotKey_DateCreated;
                inventory.LotDateSequence = lotKey.LotKey_DateSequence;
                inventory.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(packagingProductKey != null)
            {
                inventory.PackagingProduct = null;
                inventory.PackagingProductId = packagingProductKey.PackagingProductKey_ProductId;
            }

            if(locationKey != null)
            {
                inventory.Location = null;
                inventory.LocationId = locationKey.LocationKey_Id;
            }

            if(treatment != null)
            {
                inventory.Treatment = null;
                inventory.TreatmentId = treatment.InventoryTreatmentKey_Id;
            }

            if(facility != null)
            {
                if(inventory.Location == null)
                {
                    throw new ArgumentNullException("warehouseLocation is null. Maybe you tried constraining by both the LocationKey and FacilityKey. Can't do that. Pick one or the other. -RI");
                }
                inventory.Location.Facility = null;
                inventory.Location.FacilityId = facility.FacilityKey_Id;
            }

            if(toteKey != null)
            {
                inventory.ToteKey = toteKey;
            }

            return inventory;
        }

        internal static Inventory SetNoTreatment(this Inventory inventory)
        {
            if(inventory != null)
            {
                inventory.Treatment = null;
                inventory.TreatmentId = StaticInventoryTreatments.NoTreatment.Id;
            }

            return inventory;
        }

        internal static Inventory SetToPickedCurrentLocation(this Inventory inventory, PickedInventoryItem pickedInventory)
        {
            inventory.Quantity = pickedInventory.Quantity;
            return inventory.ConstrainByKeys(pickedInventory, pickedInventory, pickedInventory.CurrentLocation, pickedInventory, null, pickedInventory.ToteKey);
        }

        internal static Inventory SetValidToPick(this Inventory inventory, IFacilityKey sourceFacility = null)
        {
            if(inventory == null) { throw new ArgumentNullException("inventory"); }
            
            inventory.Lot.SetValidToPick();
            inventory.Location.ConstrainByKeys(sourceFacility).Locked = false;
            inventory.Location.Active = true;

            return inventory;
        }

        internal static Inventory SetValidToPick(this Inventory inventory, SalesOrder salesOrder)
        {
            return inventory.SetValidToPick(salesOrder.InventoryShipmentOrder.SourceFacility);
        }

        internal static Inventory CloneWithDifferentLocation(this Inventory a, Inventory b)
        {
            return a.ConstrainByKeys(b, b, null, b, b.Location, b.ToteKey);
        }
    }
}
