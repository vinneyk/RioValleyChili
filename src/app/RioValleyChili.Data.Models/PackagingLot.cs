using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class PackagingLot : LotKeyEntityBase, IDerivedLot, IPackagingProductKey
    {
        public virtual int PackagingProductId { get; set; }

        [ForeignKey("PackagingProductId")]
        public PackagingProduct PackagingProduct { get; set; }

        IProduct IDerivedLot.LotProduct { get { return PackagingProduct.Product; } }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public Lot Lot { get; set; }

        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
    }
}