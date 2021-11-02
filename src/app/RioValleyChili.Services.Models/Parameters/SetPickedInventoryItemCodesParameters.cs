using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetPickedInventoryItemCodesParameters : ISetPickedInventoryItemCodesParameters
    {
        public string PickedInventoryItemKey { get; set; }
        public string CustomerProductCode { get; set; }
        public string CustomerLotCode { get; set; }
    }
}