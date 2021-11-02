using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    /// <summary>
    /// Represents an item to be picked for the fulfillment of the <see cref="InventoryPickOrder"/>.
    /// </summary>
    public class InventoryPickOrderItem : IInventoryPickOrderItemKey, IProductKey, IPackagingProductKey, IInventoryTreatmentKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int OrderSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        public virtual int Quantity { get; set; }

        public virtual int ProductId { get; set; }
        public virtual int PackagingProductId { get; set; }
        public virtual int TreatmentId { get; set; }
        public virtual int? CustomerId { get; set; }

        [StringLength(Constants.StringLengths.CustomerProductCode)]
        public virtual string CustomerProductCode { get; set; }
        [StringLength(Constants.StringLengths.CustomerLotCode)]
        public virtual string CustomerLotCode { get; set; }

        #region Navigation Properties.

        [ForeignKey("DateCreated, OrderSequence")]
        public virtual InventoryPickOrder InventoryPickOrder { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment InventoryTreatment { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        #endregion

        #region Key Interface Implementation.

        #region Implementation of IInventoryPickOrderItemKey.

        public DateTime InventoryPickOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryPickOrderKey_Sequence { get { return OrderSequence; } }
        public int InventoryPickOrderItemKey_Sequence { get { return ItemSequence; } }

        #endregion

        public int ProductKey_ProductId { get { return ProductId; } }
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}