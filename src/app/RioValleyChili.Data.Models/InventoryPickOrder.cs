using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class InventoryPickOrder: IInventoryPickOrderKey, IPickedInventoryKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual ICollection<InventoryPickOrderItem> Items { get; set; }

        #region Navigational Properties.

        [ForeignKey("DateCreated, Sequence")]
        public virtual PickedInventory PickedInventory { get; set; }

        #endregion

        #region Key Interface Implementations.

        #region Implementation of IInventoryPickOrderKey.

        public DateTime InventoryPickOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryPickOrderKey_Sequence { get { return Sequence; } }

        #endregion

        #region Implementation of IPickedInventoryKey.

        public DateTime PickedInventoryKey_DateCreated { get { return DateCreated; } }
        public int PickedInventoryKey_Sequence { get { return Sequence; } }

        #endregion

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}