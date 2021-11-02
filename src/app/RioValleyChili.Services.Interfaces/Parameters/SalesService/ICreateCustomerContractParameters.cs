using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ICreateCustomerContractParameters : IUserIdentifiable
    {
        DateTime ContractDate { get; set; }

        DateTime? TermBegin { get; }

        DateTime? TermEnd { get; }

        string CustomerKey { get; }

        ContractType ContractType { get; }

        ContractStatus ContractStatus { get; }

        string ContactName { get; }

        Address ContactAddress { get; }

        string FOB { get; }

        string DefaultPickFromFacilityKey { get; }

        string PaymentTerms { get; }

        string CustomerPurchaseOrder { get; }

        string NotesToPrint { get; }

        IEnumerable<string> Comments { get; }
            
        IEnumerable<IContractItem> ContractItems { get; }
    }
}