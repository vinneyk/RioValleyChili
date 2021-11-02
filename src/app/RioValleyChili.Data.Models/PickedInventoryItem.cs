// ReSharper disable RedundantExtendsListEntry

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    /// <summary>
    /// Represents inventory picked. To determine the reason it was picked, see the associated <see cref="PickedInventory"/>.
    /// </summary>
    public class PickedInventoryItem : IPickedInventoryItemKey, ILotKey, IPackagingProductKey, ILocationKey, IInventoryTreatmentKey, IInventoryKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime LotDateCreated { get; set; }
        public virtual int LotDateSequence { get; set; }
        public virtual int LotTypeId { get; set; }
        public virtual int PackagingProductId { get; set; }
        public virtual int FromLocationId { get; set; }
        public virtual int TreatmentId { get; set; }
        public virtual int CurrentLocationId { get; set; }

        public virtual int Quantity { get; set; }

        [StringLength(Constants.StringLengths.ToteKeyLength)]
        public virtual string ToteKey { get; set; }
        [StringLength(Constants.StringLengths.CustomerProductCode)]
        public virtual string CustomerProductCode { get; set; }
        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public virtual string CustomerLotCode { get; set; }

        [Obsolete("For data load/synch purposes. -RI 2015/2/2")]
        public virtual DateTime? DetailID { get; set; }

        #region Navigation Properties.

        [ForeignKey("DateCreated, Sequence")]
        public virtual PickedInventory PickedInventory { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("FromLocationId")]
        public virtual Location FromLocation { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }
        [ForeignKey("CurrentLocationId")]
        public virtual Location CurrentLocation { get; set; }

        #endregion

        #region Key Interface Implementations.

        #region Implementation of IPickedInventoryItemKey.

        public DateTime PickedInventoryKey_DateCreated { get { return DateCreated; } }
        public int PickedInventoryKey_Sequence { get { return Sequence; } }
        public int PickedInventoryItemKey_Sequence { get { return ItemSequence; } }

        #endregion

        #region Implementation of ILotKey

        public DateTime LotKey_DateCreated { get { return LotDateCreated; } }
        public int LotKey_DateSequence { get { return LotDateSequence; } }
        public int LotKey_LotTypeId { get { return LotTypeId; } }

        #endregion

        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int LocationKey_Id { get { return FromLocationId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }
        public string InventoryKey_ToteKey { get { return ToteKey; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}

// ReSharper restore RedundantExtendsListEntry