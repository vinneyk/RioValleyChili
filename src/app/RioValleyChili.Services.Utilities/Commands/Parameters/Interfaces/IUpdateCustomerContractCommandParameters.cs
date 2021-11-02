using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    internal interface IUpdateCustomerContractCommandParameters
    {
        CustomerKey CustomerKey { get; }

        string ContactName { get; }

        string FOB { get; }

        FacilityKey DefaultPickFromFacilityKey { get; }

        Address ContactAddress { get; }

        ContractType ContractType { get; }

        ContractStatus ContractStatus { get; }

        string PaymentTerms { get; }

        string CustomerPurchaseOrder { get; }

        DateTime ContractDate { get; }

        DateTime? TermBegin { get; }

        DateTime? TermEnd { get; }

        string NotesToPrint { get; }

        List<SetContractItemParameters> ContractItems { get; }
    }
}