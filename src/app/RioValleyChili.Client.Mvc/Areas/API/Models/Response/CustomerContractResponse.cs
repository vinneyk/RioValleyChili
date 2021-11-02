using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class CustomerContractResponse : ILinkedResource<CustomerContractResponse>
    {
        public string CustomerContractKey { get; set; }
        public DateTime ContractDate { get; set; }
        public string PaymentTerms { get; set; }
        public string NotesToPrint { get; set; }
        public string CustomerPurchaseOrder { get; set; }
        public int? ContractNumber { get; set; }
        public DateTime? TermBegin { get; set; }
        public DateTime? TermEnd { get; set; }
        public string ContactName { get; set; }
        public Address ContactAddress { get; set; }
        public string FOB { get; set; }
        public ContractType ContractType { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
        public string BrokerCompanyKey { get; set; }
        public string BrokerCompanyName { get; set; }
        public string DistributionWarehouseKey { get; set; }
        public string DistributionWarehouseName { get; set; }
        public string CommentsNotebookKey { get; set; }

        public IEnumerable<ContractItem> ContractItems { get; set; }

        public class ContractItem
        {
            public string ContractItemKey { get; set; }
            public string ChileProductKey { get; set; }
            public string ChileProductCode { get; set; }
            public string ChileProductName { get; set; }
            public string ChileProductClassificationKey { get; set; }
            public string PackagingProductKey { get; set; }
            public string PackagingProductName { get; set; }
            public string TreatmentKey { get; set; }
            public bool UseCustomerSpec { get; set; }
            public string CustomerProductCode { get; set; }
            public int Quantity { get; set; }
            public double PriceBase { get; set; }
            public double PriceFreight { get; set; }
            public double PriceTreatment { get; set; }
            public double PriceWarehouse { get; set; }
            public double PriceRebate { get; set; }
        }

        public ResourceLinkCollection Links { get; set; }

        public string HRef { get { return Links.SelfHRef; } }

        public CustomerContractResponse Data { get { return this; } }
    }
}