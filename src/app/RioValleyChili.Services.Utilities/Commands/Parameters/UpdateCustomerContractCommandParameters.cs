using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateCustomerContractCommandParameters : IUpdateCustomerContractCommandParameters
    {
        internal IUpdateCustomerContractParameters UpdateCustomerContractParameters { get; set; }

        public string ContactName { get { return UpdateCustomerContractParameters.ContactName; } }

        public string FOB { get { return UpdateCustomerContractParameters.FOB; } }

        public CustomerKey CustomerKey { get; set; }

        public ContractKey ContractKey { get; set; }

        public FacilityKey DefaultPickFromFacilityKey { get; set; }

        public CompanyKey BrokerKey { get; set; }

        public List<SetContractItemParameters> ContractItems { get; set; }

        public string CustomerPurchaseOrder { get { return UpdateCustomerContractParameters.CustomerPurchaseOrder; } }

        public DateTime ContractDate { get { return UpdateCustomerContractParameters.ContractDate; } }

        public DateTime? TermBegin { get { return UpdateCustomerContractParameters.TermBegin; } }

        public DateTime? TermEnd { get { return UpdateCustomerContractParameters.TermEnd; } }

        public ContractType ContractType { get { return UpdateCustomerContractParameters.ContractType; } }

        public ContractStatus ContractStatus { get { return UpdateCustomerContractParameters.ContractStatus; } }

        public Address ContactAddress { get { return UpdateCustomerContractParameters.ContactAddress; } } 

        public string PaymentTerms { get { return UpdateCustomerContractParameters.PaymentTerms; } }

        public string NotesToPrint { get { return UpdateCustomerContractParameters.NotesToPrint; } }
    }
}