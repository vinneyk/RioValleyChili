using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Data.Models
{
    [Table("ShipmentInformation")]
    public class ShipmentInformation : IShipmentInformationKey
    {
        public ShipmentInformation()
        {
            ShipFrom = new ShippingLabel();
            ShipTo = new ShippingLabel();
            FreightBill = new ShippingLabel();
        }

        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual ShipmentStatus Status { get; set; }
        public virtual double? PalletWeight { get; set; }
        public virtual int PalletQuantity { get; set; }

        [StringLength(Constants.StringLengths.FreightBillType)]
        public virtual string FreightBillType { get; set; }
        [Index, StringLength(Constants.StringLengths.ShipmentMethod)]
        public virtual string ShipmentMethod { get; set; }
        [StringLength(Constants.StringLengths.DriverName)]
        public virtual string DriverName { get; set; }
        [StringLength(Constants.StringLengths.CarrierName)]
        public virtual string CarrierName { get; set; }
        [StringLength(Constants.StringLengths.TrailerLicenseNumber)]
        public virtual string TrailerLicenseNumber { get; set; }
        [StringLength(Constants.StringLengths.ContainerSeal)]
        public virtual string ContainerSeal { get; set; }

        public virtual ShippingLabel ShipFrom { get; set; }
        public virtual ShippingLabel ShipTo { get; set; }
        public virtual ShippingLabel FreightBill { get; set; }

        [Column(TypeName = "datetime")]
        public virtual DateTime? RequiredDeliveryDate { get; set; }
        [Column(TypeName = "datetime")]
        public virtual DateTime? ShipmentDate { get; set; }

        [StringLength(Constants.StringLengths.ShipmentInformationNotes)]
        public virtual string InternalNotes { get; set; }
        [StringLength(Constants.StringLengths.ShipmentInformationNotes)]
        public virtual string ExternalNotes { get; set; }
        [StringLength(Constants.StringLengths.ShipmentInformationNotes)]
        public virtual string SpecialInstructions { get; set; }

        #region Implemenation of IShipmentInformationKey.

        public DateTime ShipmentInfoKey_DateCreated { get { return DateCreated; } }
        public int ShipmentInfoKey_Sequence { get { return Sequence; } }

        #endregion
    }
}