using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotQualitySingleSummaryReturn : ILotQualitySingleSummaryReturn
    {
        public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> AttributeNamesByProductType { get { return AttributesByTypeReturn.AttributeNamesByProductType; } }
        public ILotQualitySummaryReturn LotSummary { get; internal set; }

        internal AttributesByTypeReturn AttributesByTypeReturn;
    }
}