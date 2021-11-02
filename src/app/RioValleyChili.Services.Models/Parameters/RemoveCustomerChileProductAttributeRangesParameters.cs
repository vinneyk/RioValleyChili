using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class RemoveCustomerChileProductAttributeRangesParameters : IRemoveCustomerChileProductAttributeRangesParameters
    {
        public string CustomerKey { get; set; }
        public string ChileProductKey { get; set; }
    }
}