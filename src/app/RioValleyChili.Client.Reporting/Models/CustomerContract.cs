using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Reporting.Models
{
    public class CustomerContractDrawSummary
    {
        public string CompanyName { get; set; }
        public string ContractKey { get; set; }
        public string OldContractNumber { get; set; }
        public DateTime ContractTermBegin { get; set; }
        public DateTime ContractTermEnd { get; set; }
        public string ContractType { get; set; }
        public IEnumerable<ContractDrawSummaryItem> LineItems { get; set; }

        public class ContractDrawSummaryItem
        {
            public string ProductName { get; set; }
            public string CustomerProductCode { get; set; }
            public double TotalPoundsContracted { get; set; }
            public double TotalPoundsShipped { get; set; }
            public double TotalPoundsPending { get; set; }
            public double TotalPoundsRemaining { get; set; }
        }
    }

    public class CustomerContractItemDrawSummary
    {
        public string CompanyName { get; set; }
        public string ContractKey { get; set; }
        public string OldContractNumber { get; set; }
        public DateTime? ContractTermBegin { get; set; }
        public DateTime? ContractTermEnd { get; set; }
        public string ContractType { get; set; }
        public string ProductName { get; set; }
        public string CustomerProductCode { get; set; }
        public double TotalPoundsContracted { get; set; }
        public double TotalPoundsShipped { get; set; }
        public double TotalPoundsPending { get; set; }
        public double TotalPoundsRemaining { get; set; }
    }

    public class CustomerContract
    {
        public string CustomerContractKey { get; set; }
        public DateTime ContractDate { get; set; }
        public string PaymentTerms { get; set; }
        public string NotesToPrint { get; set; }
        public string CustomerPurchaseOrder { get; set; }
        public int? ContractNumber { get; set; }
        public DateTime? TermBegin { get; set; }
        public DateTime? TermEnd { get; set; }
        public string FOB { get; set; }
        public ContractType ContractType { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
        public string BrokerCompanyKey { get; set; }
        public string BrokerCompanyName { get; set; }
        public string DistributionWarehouseKey { get; set; }
        public string DistributionWarehouseName { get; set; }

        public AddressLabel AddressLabel { get; set; }

        public IEnumerable<ContractItem> ContractItems { get; set; }

        public int TotalQuantityOnContract { get { return ContractItems.Sum(i => i.Quantity); } }
        public int TotalPoundsOnContract { get { return (int)ContractItems.Sum(i => i.TotalWeight); } }
        public bool HasSpecialInstructions { get { return !string.IsNullOrWhiteSpace(NotesToPrint); } }

        public class ContractItem
        {
            public string ContractItemKey { get; set; }
            public string ChileProductKey { get; set; }
            public string ChileProductName { get; set; }
            public string PackagingProductKey { get; set; }
            public string PackagingProductName { get; set; }
            public string Treatment { get; set; }
            public bool UseCustomerSpec { get; set; }
            public string CustomerProductCode { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public double TotalWeight { get; set; }
        }
    }
}