using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class ModifyInventoryParameters
    {
        public InventoryKey InventoryKey { get; private set; }
        public LotKey LotKey { get; private set; }
        public PackagingProductKey PackagingProductKey { get; private set; }
        public LocationKey LocationKey { get; private set; }
        public InventoryTreatmentKey InventoryTreatmentKey { get; private set; }
        public int ModifyQuantity { get; private set; }

        public ModifyInventoryParameters(ILotKey lotKey, IPackagingProductKey packagingProductKey, ILocationKey locationKey, IInventoryTreatmentKey inventoryTreatmentKey, string toteKey, int adjustQuantity)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }
            if(packagingProductKey == null) { throw new ArgumentNullException("packagingProductKey"); }
            if(locationKey == null) { throw new ArgumentNullException("locationKey"); }
            if(inventoryTreatmentKey == null) { throw new ArgumentNullException("inventoryTreatmentKey"); }

            InventoryKey = new InventoryKey(lotKey, packagingProductKey, locationKey, inventoryTreatmentKey, toteKey);
            LotKey = lotKey.ToLotKey();
            PackagingProductKey = packagingProductKey.ToPackagingProductKey();
            LocationKey = locationKey.ToLocationKey();
            InventoryTreatmentKey = inventoryTreatmentKey.ToInventoryTreatmentKey();

            ModifyQuantity = adjustQuantity;
        }

        public ModifyInventoryParameters(IInventoryKey inventoryKey, int adjustQuantity)
            : this(inventoryKey, inventoryKey, inventoryKey, inventoryKey, inventoryKey.InventoryKey_ToteKey, adjustQuantity) { }
    }
}