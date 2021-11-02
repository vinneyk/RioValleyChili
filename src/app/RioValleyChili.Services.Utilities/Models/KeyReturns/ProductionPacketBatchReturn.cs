using System.Collections.Generic;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ProductionPacketBatchReturn : IProductionPacketBatchReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public string Notes { get; internal set; }
        public IProductionBatchTargetParameters TargetParameters { get; internal set; }
        public IProductionBatchTargetParameters CalculatedParameters { get; internal set; }
        public IEnumerable<IPickedInventoryItemReturn> PickedItems { get; internal set; }
        public INotebookReturn Instructions { get; internal set; }

        internal LotKeyReturn LotKeyReturn { get; set; }
    }
}