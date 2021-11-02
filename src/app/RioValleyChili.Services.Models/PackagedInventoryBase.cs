using System;

namespace RioValleyChili.Services.Models
{
    [Obsolete("Use IPackagedLotInventory instead.")]
    public abstract class PackagedInventoryBase : InventoryBase
    {
        public int TotalWeightAvailable { get; set; }

        public int TotalWeightOnHand { get; set; }

        public string PackagingDescription { get; set; }

        public int PackagingWeight { get; set; }
    }
}