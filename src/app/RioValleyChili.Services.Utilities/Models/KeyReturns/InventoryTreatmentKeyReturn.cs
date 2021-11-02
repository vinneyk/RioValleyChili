using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class InventoryTreatmentKeyReturn : IInventoryTreatmentKey
    {
        internal string InventoryTreatmentKey { get { return new InventoryTreatmentKey(this).KeyValue; } }

        public int InventoryTreatmentKey_Id { get; internal set; }
    }
}