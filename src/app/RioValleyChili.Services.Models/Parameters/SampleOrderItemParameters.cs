using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SampleOrderItemParameters : ISampleOrderItemParameters
    {
        public string SampleOrderItemKey { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string CustomerProductName { get; set; }
        public string ProductKey { get; set; }
        public string LotKey { get; set; }
    }
}