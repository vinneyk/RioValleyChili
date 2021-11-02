using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotProductionResultItem : LotKeyEntityBase, ILotProductionResultItemKey, IInventoryKey
    {
        [Key]
        [Column(Order = 3)]
        public virtual int ResultItemSequence { get; set; }

        public virtual int PackagingProductId { get; set; }
        public virtual int LocationId { get; set; }
        public virtual int TreatmentId { get; set; }
        public virtual int Quantity { get; set; }

        #region Navigation Properties.

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual LotProductionResults ProductionResults { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }

        #endregion

        #region Key Interface Implementations.

        public int ProductionResultItemKey_Sequence { get { return ResultItemSequence; } }
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int LocationKey_Id { get { return LocationId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }
        public string InventoryKey_ToteKey { get { return ""; } }

        #endregion
    }
}