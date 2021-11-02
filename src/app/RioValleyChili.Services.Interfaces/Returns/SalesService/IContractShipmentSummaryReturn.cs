using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface IContractShipmentSummaryReturn
    {
        string ContractKey { get; }
        int? ContractNumber { get; }
        string CustomerName { get; }
        ContractStatus ContractStatus { get; }
        ContractType ContractType { get; }

        DateTime? TermBegin { get; }
        DateTime? TermEnd { get; }

        IEnumerable<IContractItemShipmentSummaryReturn> Items { get; }
    }
}