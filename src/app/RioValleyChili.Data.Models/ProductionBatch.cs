// ReSharper disable RedundantExtendsListEntry

using System;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class ProductionBatch : LotKeyEmployeeIdentifiableBase, ILotKey, IPackScheduleKey, IPickedInventoryOrder
    {
        public virtual bool ProductionHasBeenCompleted { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime PackScheduleDateCreated { get; set; }
        public virtual int PackScheduleSequence { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime InstructionNotebookDateCreated { get; set; }
        public virtual int InstructionNotebookSequence { get; set; }

        public virtual ProductionBatchTargetParameters TargetParameters { get; set; }

        #region Navigational Properties.

        [ForeignKey("PackScheduleDateCreated, PackScheduleSequence")]
        public virtual PackSchedule PackSchedule { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileLotProduction Production { get; set; }
        [ForeignKey("InstructionNotebookDateCreated, InstructionNotebookSequence")]
        public virtual Notebook InstructionNotebook { get; set; }
        PickedInventory IPickedInventoryOrder.PickedInventory { get { return Production.PickedInventory; } }

        #endregion

        #region Implementation of IPackScheduleKey.

        public DateTime PackScheduleKey_DateCreated { get { return PackScheduleDateCreated; } }
        public int PackScheduleKey_DateSequence { get { return PackScheduleSequence; } }

        #endregion
    }
}

// ReSharper restore RedundantExtendsListEntry