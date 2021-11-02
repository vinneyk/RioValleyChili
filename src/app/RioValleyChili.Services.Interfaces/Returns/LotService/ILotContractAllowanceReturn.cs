using System;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotContractAllowanceReturn
    {
        string ContractKey { get; }
        DateTime? TermBegin { get; }
        DateTime? TermEnd { get; }
        string CustomerKey { get; }
        string CustomerName { get; }
    }
}