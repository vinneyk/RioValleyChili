using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractSummaryReturn : ContractBaseReturn, ICustomerContractSummaryReturn
    {
        public double AverageBasePrice { get; set; }

        public double AverageTotalPrice { get; set; }

        public double SumQuantity { get; set; }

        public double SumWeight { get; set; }

        public double SumValue { get; set; }
    }
}