using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotAttributeDefectReturn : ILotAttributeDefectReturn
    {
        public string AttributeShortName { get; internal set; }

        public double OriginalValue { get; internal set; }

        public double OriginalMinLimit { get; internal set; }

        public double OriginalMaxLimit { get; internal set; }
    }
}