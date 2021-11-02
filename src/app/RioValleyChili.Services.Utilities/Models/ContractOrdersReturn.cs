using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ContractOrdersReturn : ICustomerContractOrdersReturn
    {
        public string CustomerContractKey { get { return ContracKeyReturn.ContractKey; } }

        public IEnumerable<ICustomerContractOrderReturn> Orders { get; internal set; }

        internal ContractKeyReturn ContracKeyReturn { get; set; }
    }
}