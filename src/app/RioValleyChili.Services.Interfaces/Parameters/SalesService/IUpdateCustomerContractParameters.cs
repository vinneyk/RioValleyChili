using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface IUpdateCustomerContractParameters : IUserIdentifiable
    {
        string ContractKey { get; }

        string CustomerKey { get; }

        string ContactName { get; }

        string FOB { get; }

        string DefaultPickFromWarehouseKey { get; }

        string BrokerKey { get; }

        Address ContactAddress { get; }

        ContractType ContractType { get; }

        ContractStatus ContractStatus { get; }

        string PaymentTerms { get; }

        string CustomerPurchaseOrder { get; }

        DateTime ContractDate { get; }

        DateTime? TermBegin { get; }

        DateTime? TermEnd { get; }

        string NotesToPrint { get; }

        IEnumerable<IContractItem> ContractItems { get; }
    }
}