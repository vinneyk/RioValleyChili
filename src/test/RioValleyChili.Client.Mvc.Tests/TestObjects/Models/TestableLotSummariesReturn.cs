using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Tests.TestObjects.Models
{
    internal class TestableLotSummariesReturn : ILotQualitySummariesReturn
    {
        public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> AttributeNamesByProductType { get; set; }

        public IQueryable<ILotQualitySummaryReturn> LotSummaries { get; set; }
    }
}