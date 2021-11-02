using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    [Table("Inventory")]
    public class Inventory : IInventoryKey, ILotContainer
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime LotDateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int LotDateSequence { get; set; }
        [Key, Column(Order = 2)]
        [Obsolete("Use ProductionLineLocationTypeEnum until http://stackoverflow.com/q/12220956 is resolved.")]
        public virtual int LotTypeId
        {
            get { return (int)((LotTypeEnum)_lotType); }
            set { _lotType = (int)((LotTypeEnum)value); }
        }
        [Key, Column(Order = 3)]
        public virtual int PackagingProductId { get; set; }
        [Key, Column(Order = 4)]
        public virtual int LocationId { get; set; }
        [Key, Column(Order = 5)]
        public virtual int TreatmentId { get; set; }
        [Key, Column(Order = 6)]
        [StringLength(Constants.StringLengths.ToteKeyLength)]
        public virtual string ToteKey { get; set; }

        [NotMapped]
        public LotTypeEnum LotTypeEnum
        {
            get { return (LotTypeEnum)_lotType; }
            set { _lotType = (int)value; }
        }

        public virtual int Quantity { get; set; }

        //todo: Unused properties meant to go on InventoryTransactions? Remove? -RI 2015/08/31
        public virtual InventoryTransactionType TransactionType { get; set; }
        public virtual string Description { get; set; }

        #region Navigation Properties

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

        #region Implementation of ILotKey

        public DateTime LotKey_DateCreated { get { return LotDateCreated; } }
        public int LotKey_DateSequence { get { return LotDateSequence; } }
        public int LotKey_LotTypeId { get { return LotTypeId; } }

        #endregion

        public int LocationKey_Id { get { return LocationId; } }
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }
        public string InventoryKey_ToteKey { get { return ToteKey; } }

        #endregion

        #region Private Parts

        private int _lotType;

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}