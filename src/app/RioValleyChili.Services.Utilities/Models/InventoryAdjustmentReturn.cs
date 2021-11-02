using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryAdjustmentReturn : IInventoryAdjustmentReturn
    {
        public string InventoryAdjustmentKey { get { return InventoryAdjustmentKeyReturn.InventoryAdjustmentKey; } }

        public DateTime AdjustmentDate { get; internal set; }

        public string User { get; internal set; }

        public DateTime TimeStamp { get; internal set; }

        public INotebookReturn Notebook { get; internal set; }

        public IEnumerable<IInventoryAdjustmentItemReturn> Items { get; internal set; }

        internal InventoryAdjustmentKeyReturn InventoryAdjustmentKeyReturn { get; set; }
    }
}