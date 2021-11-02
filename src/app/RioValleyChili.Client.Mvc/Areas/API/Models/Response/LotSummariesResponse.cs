using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotSummariesResponse
    {
        public int Total { get; set; }
        public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> AttributeNamesByProductType { get; internal set; }
        public IEnumerable<LotQualitySummaryResponse> LotSummaries { get; internal set; }
    }
}