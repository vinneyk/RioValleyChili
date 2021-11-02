using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerContractDetailReturn : ICustomerContractBaseReturn
    {
        string NotesToPrint { get; }
        Address ContactAddress { get; }
        INotebookReturn Comments { get; }
        IEnumerable<IContractItemReturn> ContractItems { get; }
    }
}
