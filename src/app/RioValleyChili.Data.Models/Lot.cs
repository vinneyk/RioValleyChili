using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class Lot : LotKeyEmployeeIdentifiableBase, IPackagingProductKey
    {
        [Index]
        public virtual LotQualityStatus QualityStatus { get; set; }
        [Index]
        public virtual LotProductionStatus ProductionStatus { get; set; }
        public virtual bool ProductSpecComplete { get; set; }
        public virtual bool ProductSpecOutOfRange { get; set; }
        public virtual int ReceivedPackagingProductId { get; set; }
        public virtual int? VendorId { get; set; }

        public virtual LotHoldType? Hold { get; set; }
        [StringLength(Constants.StringLengths.LotHoldDescription)]
        public virtual string HoldDescription { get; set; }
        [StringLength(Constants.StringLengths.LotNotes)]
        public virtual string Notes { get; set; }
        [StringLength(Constants.StringLengths.PurchaseOrderNumber)]
        public virtual string PurchaseOrderNumber { get; set; }
        [StringLength(Constants.StringLengths.ShipperNumber)]
        public virtual string ShipperNumber { get; set; }

        [ForeignKey("ReceivedPackagingProductId")]
        public virtual PackagingProduct ReceivedPackaging { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileLot ChileLot { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual AdditiveLot AdditiveLot { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual PackagingLot PackagingLot { get; set; }
        [ForeignKey("VendorId")]
        public virtual Company Vendor { get; set; }

        public virtual ICollection<LotAttribute> Attributes { get; set; }
        public virtual ICollection<Inventory> Inventory { get; set; }
        public virtual ICollection<PickedInventoryItem> PickedInventory { get; set; }
        public virtual ICollection<LotDefect> LotDefects { get; set; }
        public virtual ICollection<LotAttributeDefect> AttributeDefects { get; set; }
        [InverseProperty("SourceLot")]
        public virtual ICollection<InventoryTransaction> OutputTransactions { get; set; }
        [InverseProperty("DestinationLot")]
        public virtual ICollection<InventoryTransaction> InputTransactions { get; set; }
        public virtual ICollection<LotContractAllowance> ContractAllowances { get; set; }
        public virtual ICollection<LotSalesOrderAllowance> SalesOrderAllowances { get; set; }
        public virtual ICollection<LotCustomerAllowance> CustomerAllowances { get; set; }
        public virtual ICollection<LotHistory> History { get; set; }

        public int PackagingProductKey_ProductId { get { return ReceivedPackagingProductId; } }
    }
}