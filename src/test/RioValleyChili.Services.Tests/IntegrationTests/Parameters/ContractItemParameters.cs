using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class ContractItemParameters : IContractItem
    {
        public string ChileProductKey { get; set; }
        public string PackagingProductKey { get; set; }
        public string TreatmentKey { get; set; }
        public bool UseCustomerSpec { get; set; }
        public string CustomerCodeOverride { get; set; }
        public int Quantity { get; set; }
        public double PriceBase { get; set; }
        public double PriceFreight { get; set; }
        public double PriceTreatment { get; set; }
        public double PriceWarehouse { get; set; }
        public double PriceRebate { get; set; }
    }
}