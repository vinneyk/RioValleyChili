using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractShipmentSummaryReturn : IContractShipmentSummaryReturn
    {
        public string ContractKey { get { return ContractKeyReturn.ContractKey; } }
        public int? ContractNumber { get; internal set; }
        public string CustomerName { get; internal set; }
        public ContractType ContractType { get; internal set; }
        public ContractStatus ContractStatus { get; internal set; }
        public DateTime? TermBegin { get; internal set; }
        public DateTime? TermEnd { get; internal set; }
        public IEnumerable<IContractItemShipmentSummaryReturn> Items { get; internal set; }

        internal ContractKeyReturn ContractKeyReturn { get; set; }
    }
}