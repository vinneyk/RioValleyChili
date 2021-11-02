using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class BatchPickedInventoryItemReturn : PickedInventoryItemReturn, IBatchPickedInventoryItemReturn
    {
        public string NewLotKey { get { return NewLotKeyReturn.LotKey; } }

        internal LotKeyReturn NewLotKeyReturn { get; set; }
    }
}