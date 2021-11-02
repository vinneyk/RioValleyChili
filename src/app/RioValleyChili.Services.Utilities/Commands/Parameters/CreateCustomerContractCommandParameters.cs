using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateCustomerContractCommandParameters : IUpdateCustomerContractCommandParameters
    {
        internal ICreateCustomerContractParameters CreateCustomerContractParameters { get; set; }

        public CustomerKey CustomerKey { get; set; }

        public string ContactName { get { return CreateCustomerContractParameters.ContactName; } }

        public string FOB { get { return CreateCustomerContractParameters.FOB; } }

        public FacilityKey DefaultPickFromFacilityKey { get; set; }

        public string CustomerPurchaseOrder { get { return CreateCustomerContractParameters.CustomerPurchaseOrder; } }

        public DateTime ContractDate { get { return CreateCustomerContractParameters.ContractDate; } }

        public DateTime? TermBegin { get { return CreateCustomerContractParameters.TermBegin; } }

        public DateTime? TermEnd { get { return CreateCustomerContractParameters.TermEnd; } }

        public ContractType ContractType { get { return CreateCustomerContractParameters.ContractType; } }

        public ContractStatus ContractStatus { get { return CreateCustomerContractParameters.ContractStatus; } }

        public Address ContactAddress { get { return CreateCustomerContractParameters.ContactAddress; } } 

        public string PaymentTerms { get { return CreateCustomerContractParameters.PaymentTerms; } }

        public string NotesToPrint { get { return CreateCustomerContractParameters.NotesToPrint; } }

        public List<SetContractItemParameters> ContractItems { get; set; }
    }
}