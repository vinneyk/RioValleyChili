using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class InventoryAdjustmentItem : EmployeeIdentifiableBase, IInventoryAdjustmentItemKey, IInventoryKey
    {
        [Key]
        [Column(Order = 0, TypeName = "Date")]
        public virtual DateTime AdjustmentDate { get; set; }

        [Key]
        [Column(Order = 1)]
        public virtual int Sequence { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        public virtual int QuantityAdjustment { get; set; }

        public virtual DateTime LotDateCreated { get; set; }

        public virtual int LotDateSequence { get; set; }

        public virtual int LotTypeId { get; set; }

        public virtual int PackagingProductId { get; set; }

        public virtual int LocationId { get; set; }

        public virtual int TreatmentId { get; set; }

        [StringLength(Constants.StringLengths.ToteKeyLength)]
        public virtual string ToteKey { get; set; }

        #region Navigation Properties.

        [ForeignKey("AdjustmentDate, Sequence")]
        public virtual InventoryAdjustment InventoryAdjustment { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }

        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }

        #endregion

        #region Key Interface Implementations

        #region IInventoryAdjustItemKey

        public DateTime InventoryAdjustmentKey_AdjustmentDate { get { return AdjustmentDate; } }

        public int InventoryAdjustmentKey_Sequence { get { return Sequence; } }

        public int InventoryAdjustmetItemKey_Sequence { get { return ItemSequence; } }

        #endregion

        #region IInventoryKey

        public DateTime LotKey_DateCreated { get { return LotDateCreated; } }

        public int LotKey_DateSequence { get { return LotDateSequence; } }

        public int LotKey_LotTypeId { get { return LotTypeId; } }

        public int LocationKey_Id { get { return LocationId; } }

        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }

        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }

        public string InventoryKey_ToteKey { get { return ToteKey; } }

        #endregion

        #endregion
    }
}