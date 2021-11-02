using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractBaseReturn
    {
        public string CustomerContractKey { get { return ContractKeyReturn.ContractKey; } }
        public DateTime ContractDate { get; internal set; }
        public string CustomerPurchaseOrder { get; internal set; }
        public int? ContractNumber { get; internal set; }
        public DateTime? TermBegin { get; internal set; }
        public DateTime? TermEnd { get; internal set; }
        public string PaymentTerms { get; internal set; }
        public string ContactName { get; internal set; }
        public string FOB { get; internal set; }
        public ContractType ContractType { get; internal set; }
        public ContractStatus ContractStatus { get; internal set; }

        public ICompanySummaryReturn Customer { get; internal set; }
        public ICompanySummaryReturn Broker { get; internal set; }
        public IFacilitySummaryReturn DefaultPickFromFacility { get; internal set; }

        internal ContractKeyReturn ContractKeyReturn { get; set; }
    }
}