using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateCustomerContractParameters : ICreateCustomerContractParameters
    {
        public DateTime ContractDate { get; set; }
        public DateTime? TermBegin { get; set; }
        public DateTime? TermEnd { get; set; }
        public string CustomerKey { get; set; }
        public ContractType ContractType { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public string ContactName { get; set; }
        public Address ContactAddress { get; set; }
        public string FOB { get; set; }
        public string DefaultPickFromFacilityKey { get; set; }
        public string PaymentTerms { get; set; }
        public string CustomerPurchaseOrder { get; set; }
        public string NotesToPrint { get; set; }
        public IEnumerable<string> Comments { get; set; }
        public IEnumerable<ContractItem> ContractItems { get; set; }

        IEnumerable<IContractItem> ICreateCustomerContractParameters.ContractItems { get { return ContractItems; }}

        string IUserIdentifiable.UserToken { get; set; }

        public class ContractItem : IContractItem
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
}