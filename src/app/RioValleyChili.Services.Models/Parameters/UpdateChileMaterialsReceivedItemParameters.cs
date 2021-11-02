using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateChileMaterialsReceivedItemParameters : IUpdateChileMaterialsReceivedItemParameters
    {
        public string ItemKey { get; set; }
        public string GrowerCode { get; set; }
        public string ToteKey { get; set; }
        public int Quantity { get; set; }
        public string PackagingProductKey { get; set; }
        public string Variety { get; set; }
        public string LocationKey { get; set; }
    }
}