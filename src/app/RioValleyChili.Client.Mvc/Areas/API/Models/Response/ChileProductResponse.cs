using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class ChileProductResponse : InventoryProductResponse
    {
        public string ChileTypeKey { get; set; }
        public string ChileTypeDescription { get; set; }
        public ChileStateEnum ChileState { get; set; }
        public string ChileStateName { get; set; }
    }
}