using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    [Table("PickedInventory")]
    public class PickedInventory : EmployeeIdentifiableBase, IPickedInventoryKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual PickedReason PickedReason { get; set; }

        public virtual ICollection<PickedInventoryItem> Items { get; set; }

        /// <summary>
        /// Defines whether or not the picked inventory item records are being kept as historical references.
        /// If not, then attempting to resolve associated lot defects via a treatment will fail if the treatment
        /// on the picked item is not already considered valid for the defect. -RI 2014/12/29
        /// </summary>
        public virtual bool Archived { get; set; }

        #region Implementation of IPickedInventoryKey.

        public DateTime PickedInventoryKey_DateCreated { get { return DateCreated; } }
        public int PickedInventoryKey_Sequence { get { return Sequence; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}