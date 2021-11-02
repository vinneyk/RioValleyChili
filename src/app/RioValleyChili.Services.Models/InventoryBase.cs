using System;

namespace RioValleyChili.Services.Models
{
    [Obsolete("Use ILotInventory instead.")]
    public abstract class InventoryBase
    {
        // Note: this class will provide a flattened view of the InventoryBase and InventoryQuantityByLocation tables.

        /// <summary>
        /// The InventoryQuantityByLocation key
        /// </summary>
        public string InventoryKey { get; set; } //todo: rename?

        public string LotNumber { get; set; } 
        
        public string ProductName { get; set; }

        public string ProductKey { get; set; }

        public string CurrentWarehouseLocation { get; set; }

        public string CurrentWarehouse { get; set; }

        public int QuantityAvailable { get; set; }

        public int QuantityOnHand { get; set; }
    }
}