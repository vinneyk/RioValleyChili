using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.SalesService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractDetailReturn : ContractBaseReturn, ICustomerContractDetailReturn
    {
        public string NotesToPrint { get; internal set; }
        public Address ContactAddress { get; internal set; }
        public INotebookReturn Comments { get; internal set; }
        public IEnumerable<IContractItemReturn> ContractItems { get; internal set; }
    }
}