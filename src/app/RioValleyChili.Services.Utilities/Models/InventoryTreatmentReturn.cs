using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryTreatmentReturn : IInventoryTreatmentReturn
    {
        public string TreatmentKey { get { return InventoryTreatmentKeyReturn.InventoryTreatmentKey; } }

        public string TreatmentName { get; internal set; }

        public string TreatmentNameShort { get; internal set; }

        internal InventoryTreatmentKeyReturn InventoryTreatmentKeyReturn { get; set; }
    }
}