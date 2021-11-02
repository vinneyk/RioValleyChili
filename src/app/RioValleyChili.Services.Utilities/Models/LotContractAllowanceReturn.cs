using System;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotContractAllowanceReturn : ILotContractAllowanceReturn
    {
        public string ContractKey { get { return ContractKeyReturn.ContractKey; } }
        public DateTime? TermBegin { get; internal set; }
        public DateTime? TermEnd { get; internal set; }
        public string CustomerKey { get { return CustomerKeyReturn.CustomerKey; } }
        public string CustomerName { get; internal set; }

        internal ContractKeyReturn ContractKeyReturn { get; set; }
        internal CustomerKeyReturn CustomerKeyReturn { get; set; }
    }
}