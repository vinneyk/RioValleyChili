using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateContractParameters : ICreateCustomerContractParameters
    {
        public string UserToken { get; set; }
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
        public IEnumerable<IContractItem> ContractItems { get; set; }
    }
}