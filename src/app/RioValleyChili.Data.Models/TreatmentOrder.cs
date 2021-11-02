// ReSharper disable RedundantExtendsListEntry
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{

    public class TreatmentOrder : ITreatmentOrderKey, IInventoryShipmentOrderKey, IInventoryTreatmentKey,
        IPickedInventoryOrder, IInventoryPickOrder
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual DateTime? Returned { get; set; }

        public virtual int InventoryTreatmentId { get; set; }

        [ForeignKey("DateCreated, Sequence")]
        public virtual InventoryShipmentOrder InventoryShipmentOrder { get; set; }
        [ForeignKey("InventoryTreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }

        #region Key Implementations.
        
        public int InventoryTreatmentKey_Id { get { return InventoryTreatmentId; } }

        public DateTime InventoryShipmentOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryShipmentOrderKey_Sequence { get { return Sequence; } }

        #endregion

        PickedInventory IPickedInventoryOrder.PickedInventory { get { return InventoryShipmentOrder.PickedInventory; } }
        InventoryPickOrder IInventoryPickOrder.InventoryPickOrder { get { return InventoryShipmentOrder.InventoryPickOrder; } }
    }
}

// ReSharper restore RedundantExtendsListEntry