using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryAdjustmentsService
{
    public interface IInventoryAdjustmentReturn 
    {
        string InventoryAdjustmentKey { get; }

        DateTime AdjustmentDate { get; }

        string User { get; }

        DateTime TimeStamp { get; }

        INotebookReturn Notebook { get; }
        
        IEnumerable<IInventoryAdjustmentItemReturn> Items { get; }
    }
}
