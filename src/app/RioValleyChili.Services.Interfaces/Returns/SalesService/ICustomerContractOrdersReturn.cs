using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.SalesService
{
    public interface ICustomerContractOrdersReturn
    {
        string CustomerContractKey { get; }

        IEnumerable<ICustomerContractOrderReturn> Orders { get; }
    }
}