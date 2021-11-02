using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class InventoryTransaction : EmployeeIdentifiableBase, IInventoryTransactionKey, IInventoryKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime SourceLotDateCreated { get; set; }
        public virtual int SourceLotDateSequence { get; set; }
        [Obsolete("Use SourceLotTypeEnum until http://stackoverflow.com/q/12220956 is resolved.")]
        public virtual int SourceLotTypeId
        {
            get { return (int)((LotTypeEnum)_sourceLotType); }
            set { _sourceLotType = (int)((LotTypeEnum)value); }
        }
        [NotMapped]
        public LotTypeEnum SourceLotTypeEnum
        {
            get { return (LotTypeEnum)_sourceLotType; }
            set { _sourceLotType = (int)value; }
        }

        public virtual InventoryTransactionType TransactionType { get; set; }
        public virtual string Description { get; set; }
        public virtual string SourceReference { get; set; }
        public virtual int PackagingProductId { get; set; }
        public virtual int LocationId { get; set; }
        public virtual int TreatmentId { get; set; }
        [StringLength(Constants.StringLengths.ToteKeyLength)]
        public virtual string ToteKey { get; set; }
        public virtual int Quantity { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime? DestinationLotDateCreated { get; set; }
        public virtual int? DestinationLotDateSequence { get; set; }
        [Obsolete("Use DestinationLotTypeEnum until http://stackoverflow.com/q/12220956 is resolved.")]
        public virtual int? DestinationLotTypeId
        {
            get { return _destinationLotType != null ? (int?)((LotTypeEnum?)_destinationLotType) : null; }
            set { _destinationLotType = value != null ? (int?)((LotTypeEnum?)value) : null; }
        }
        [NotMapped]
        public LotTypeEnum? DestinationLotTypeEnum
        {
            get { return _destinationLotType != null ? (LotTypeEnum?)_destinationLotType : null; }
            set { _destinationLotType = value != null ? (int)value.Value : (int?) null; }
        }

        #region Navigation Properties

        [ForeignKey("SourceLotDateCreated, SourceLotDateSequence, SourceLotTypeId")]
        public virtual Lot SourceLot { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }
        [ForeignKey("DestinationLotDateCreated, DestinationLotDateSequence, DestinationLotTypeId")]
        public virtual Lot DestinationLot { get; set; }

        #endregion

        #region Key Interface Implementations

        #region IInventoryTransactionKey
        public DateTime InventoryTransactionKey_Date { get { return DateCreated; } }
        public int InventoryTransactionKey_Sequence { get { return Sequence; } }
        #endregion

        #region IInventoryKey
        public DateTime LotKey_DateCreated { get { return SourceLotDateCreated; } }
        public int LotKey_DateSequence { get { return SourceLotDateSequence; } }
        public int LotKey_LotTypeId { get { return SourceLotTypeId; } }
        public int LocationKey_Id { get { return LocationId; } }
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }
        public string InventoryKey_ToteKey { get { return ToteKey; } }
        #endregion

        #endregion

        #region Private Parts

        private int _sourceLotType;
        private int? _destinationLotType;

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}