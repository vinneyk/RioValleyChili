using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotDefectReturn : ILotDefectReturn
    {
        public string LotDefectKey { get { return LotDefectKeyReturn.LotDefectKey; } }

        public DefectTypeEnum DefectType { get; internal set; }

        public string Description { get; internal set; }

        public ILotAttributeDefectReturn AttributeDefect { get; internal set; }

        public ILotDefectResolutionReturn Resolution { get; internal set; }

        internal LotDefectKeyReturn LotDefectKeyReturn { get; set; }
    }
}