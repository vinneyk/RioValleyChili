using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    /// <summary>
    /// Represents an item to be picked for the fulfillment of the <see cref="SalesOrder"/>.
    /// </summary>
    public class SalesOrderItem : ISalesOrderItemKey, IInventoryPickOrderItemKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        public virtual int? ContractYear { get; set; }
        public virtual int? ContractSequence { get; set; }
        public virtual int? ContractItemSequence { get; set; }

        public virtual double PriceBase { get; set; }
        public virtual double PriceFreight { get; set; }
        public virtual double PriceTreatment { get; set; }
        public virtual double PriceWarehouse { get; set; }
        public virtual double PriceRebate { get; set; }

        [Obsolete("For data load/sync purposes. -RI 2014/12/2")]
        public virtual DateTime? ODetail { get; set; }

        [ForeignKey("DateCreated, Sequence")]
        public virtual SalesOrder Order { get; set; }
        [ForeignKey("DateCreated, Sequence, ItemSequence")]
        public virtual InventoryPickOrderItem InventoryPickOrderItem { get; set; }
        [ForeignKey("ContractYear, ContractSequence, ContractItemSequence")]
        public virtual ContractItem ContractItem { get; set; }
        public virtual ICollection<SalesOrderPickedItem> PickedItems { get; set; }

        #region Key Interface Implementations

        #region ISalesOrderItemKey

        public DateTime SalesOrderKey_DateCreated { get { return DateCreated; } }
        public int SalesOrderKey_Sequence { get { return Sequence; } }
        public int SalesOrderItemKey_ItemSequence { get { return ItemSequence; } }

        #endregion

        #region IInventoryPickOrderItemKey

        public DateTime InventoryPickOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryPickOrderKey_Sequence { get { return Sequence; } }
        public int InventoryPickOrderItemKey_Sequence { get { return ItemSequence; } }

        #endregion

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}