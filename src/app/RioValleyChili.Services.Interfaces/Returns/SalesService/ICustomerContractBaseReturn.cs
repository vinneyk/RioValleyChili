using System;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerContractBaseReturn
    {
        string CustomerContractKey { get; }
        string CustomerPurchaseOrder { get; }
        int? ContractNumber { get; }
        DateTime ContractDate { get; }
        DateTime? TermBegin { get; }
        DateTime? TermEnd { get; }
        string PaymentTerms { get; }
        string ContactName { get; }
        string FOB { get; }
        ContractType ContractType { get; }
        ContractStatus ContractStatus { get; }
        ICompanySummaryReturn Customer { get; }
        ICompanySummaryReturn Broker { get; }
        IFacilitySummaryReturn DefaultPickFromFacility { get; }
    }
}