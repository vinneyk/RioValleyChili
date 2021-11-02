using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ISetContractsStatusParameters
    {
        ContractStatus ContractStatus { get; }
        IEnumerable<string> ContractKeys { get; }
    }
}