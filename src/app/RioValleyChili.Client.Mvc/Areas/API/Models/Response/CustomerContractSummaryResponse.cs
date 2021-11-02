using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class CustomerContractSummaryResponse 
    {
        public string CustomerContractKey { get; set; }
        public string ContractDate { get; set; }
        public string TermBegin { get; set; }
        public string TermEnd { get; set; }
        public string PaymentTerms { get; set; }
        public string CustomerPurchaseOrder { get; set; }
        /// <summary>
        /// Reference to contract ID from Access system.
        /// </summary>
        public int? ContractNumber { get; set; } 
        public ContractType ContractType { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string BrokerCompanyKey { get; set; }
        public string BrokerCompanyName { get; set; }
        public string DistributionWarehouseKey { get; set; }
        public string DistributionWarehouseName { get; set; }
        public double AverageBasePrice { get; set; }
        public double AverageTotalPrice { get; set; }
        public double SumQuantity { get; set; }
        public double SumWeight { get; set; }
    }
}