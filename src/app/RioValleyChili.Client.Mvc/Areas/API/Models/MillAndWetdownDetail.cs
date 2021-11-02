using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class MillAndWetdownDetail :MillAndWetdownSummary
    {
        public IEnumerable<MillAndWetdownResultItem> ResultItems { get; set; }
        public IEnumerable<MillAndWetdownPickedItem> PickedItems { get; set; }
    }
}