using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class ChileLot : LotKeyEntityBase, IDerivedLot, IChileProductKey
    {
        public virtual bool AllAttributesAreLoBac { get; set; }
        public virtual int ChileProductId { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileLotProduction Production { get; set; }
        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }

        IProduct IDerivedLot.LotProduct { get { return ChileProduct.Product; } }

        public int ChileProductKey_ProductId { get { return ChileProductId; } }
    }
}
