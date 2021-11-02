using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class InventoryShipmentOrder : EmployeeIdentifiableBase, IInventoryShipmentOrderKey, IPickedInventoryKey, IInventoryPickOrderKey, IShipmentInformationKey,
        IInventoryPickOrder, IPickedInventoryOrder
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime ShipmentInfoDateCreated { get; set; }
        public virtual int ShipmentInfoSequence { get; set; }

        public virtual InventoryShipmentOrderTypeEnum OrderType { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }

        public virtual int? DestinationFacilityId { get; set; }
        public virtual int SourceFacilityId { get; set; }

        [StringLength(Constants.StringLengths.PurchaseOrderNumber)]
        public virtual string PurchaseOrderNumber { get; set; }
        
        [Column(TypeName = "date")]
        public virtual DateTime? DateReceived { get; set; }
        [StringLength(Constants.StringLengths.ContactName)]
        public virtual string RequestedBy { get; set; }
        [StringLength(Constants.StringLengths.ContactName)]
        public virtual string TakenBy { get; set; }
        [Index]
        public virtual int? MoveNum { get; set; }

        [ForeignKey("DestinationFacilityId")]
        public virtual Facility DestinationFacility { get; set; }
        [ForeignKey("SourceFacilityId")]
        public virtual Facility SourceFacility { get; set; }
        [ForeignKey("DateCreated, Sequence")]
        public virtual PickedInventory PickedInventory { get; set; }
        [ForeignKey("DateCreated, Sequence")]
        public virtual InventoryPickOrder InventoryPickOrder { get; set; }
        [ForeignKey("ShipmentInfoDateCreated, ShipmentInfoSequence")]
        public virtual ShipmentInformation ShipmentInformation { get; set; }

        #region Key Interface Implementations.
        
        public DateTime InventoryShipmentOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryShipmentOrderKey_Sequence { get { return Sequence; } }
        public DateTime PickedInventoryKey_DateCreated { get { return DateCreated; } }
        public int PickedInventoryKey_Sequence { get { return Sequence; } }
        public DateTime InventoryPickOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryPickOrderKey_Sequence { get { return Sequence; } }
        public DateTime ShipmentInfoKey_DateCreated { get { return ShipmentInfoDateCreated; } }
        public int ShipmentInfoKey_Sequence { get { return ShipmentInfoSequence; } }

        #endregion
    }
}