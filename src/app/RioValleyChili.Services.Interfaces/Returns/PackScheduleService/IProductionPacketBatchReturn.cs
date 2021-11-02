using System.Collections.Generic;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IProductionPacketBatchReturn
    {
        string LotKey { get; }

        string Notes { get; }

        IProductionBatchTargetParameters TargetParameters { get; }

        IProductionBatchTargetParameters CalculatedParameters { get; }

        IEnumerable<IPickedInventoryItemReturn> PickedItems { get; }

        INotebookReturn Instructions { get; } 
    }
}