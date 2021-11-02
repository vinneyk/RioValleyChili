using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class InventoryAdjustment : EmployeeIdentifiableBase, IInventoryAdjustmentKey
    {
        [Key]
        [Column(Order = 0, TypeName = "Date")]
        public virtual DateTime AdjustmentDate { get; set; }

        [Key]
        [Column(Order = 1)]
        public virtual int Sequence { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime NotebookDate { get; set; }

        public virtual int NotebookSequence { get; set; }

        [ForeignKey("NotebookDate, NotebookSequence")]
        public virtual Notebook Notebook { get; set; }

        public virtual ICollection<InventoryAdjustmentItem> Items { get; set; }

        #region Implementation of IInventoryAdjustmentKey

        public DateTime InventoryAdjustmentKey_AdjustmentDate { get { return AdjustmentDate; } }

        public int InventoryAdjustmentKey_Sequence { get { return Sequence; } }

        #endregion
    }
}