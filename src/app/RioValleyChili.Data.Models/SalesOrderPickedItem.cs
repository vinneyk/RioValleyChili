// ReSharper disable RedundantExtendsListEntry

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    /// <summary>
    /// A reference to an inventory item picked for a <see cref="SalesOrder"/>.
    /// May optionally refer to a <see cref="SalesOrderItem"/> indicating it 
    /// was picked for the fulfillment of that item. 
    /// </summary>
    public class SalesOrderPickedItem : ISalesOrderPickedItemKey, IPickedInventoryItemKey, ISalesOrderKey, ISalesOrderItemKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }
        
        public virtual int OrderItemSequence { get; set; }

        [ForeignKey("DateCreated, Sequence")]
        public virtual SalesOrder SalesOrder { get; set; }
        [ForeignKey("DateCreated, Sequence, ItemSequence")]
        public virtual PickedInventoryItem PickedInventoryItem { get; set; }
        [ForeignKey("DateCreated, Sequence, OrderItemSequence")]
        public virtual SalesOrderItem SalesOrderItem { get; set; }

        #region Key Interface Implementations

        public DateTime SalesOrderKey_DateCreated { get { return DateCreated; } }
        public int SalesOrderKey_Sequence { get { return Sequence; } }
        public int SalesOrderPickedItemKey_ItemSequence { get { return ItemSequence; } }

        public DateTime PickedInventoryKey_DateCreated { get { return DateCreated; } }
        public int PickedInventoryKey_Sequence { get { return Sequence; } }
        public int PickedInventoryItemKey_Sequence { get { return ItemSequence; } }

        public int SalesOrderItemKey_ItemSequence { get { return OrderItemSequence; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}

// ReSharper restore RedundantExtendsListEntry