using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotDefectResolutionReturn : ILotDefectResolutionReturn
    {
        public ResolutionTypeEnum? ResolutionType { get; internal set; }

        public string Description { get; internal set; }
    }
}