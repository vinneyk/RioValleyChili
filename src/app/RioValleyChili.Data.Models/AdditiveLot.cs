using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class AdditiveLot : LotKeyEntityBase, IDerivedLot, IAdditiveProductKey
    {
        public virtual int AdditiveProductId { get; set; }

        [ForeignKey("AdditiveProductId")]
        public virtual AdditiveProduct AdditiveProduct { get; set; }

        IProduct IDerivedLot.LotProduct { get { return AdditiveProduct.Product; } }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public Lot Lot { get; set; }

        public int AdditiveProductKey_Id { get { return AdditiveProductId; } }
    }
}