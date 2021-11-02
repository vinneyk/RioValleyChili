using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class CustomerKeyReturn : ICustomerKey
    {
        internal string CustomerKey { get { return new CustomerKey(this); } }
        public int CustomerKey_Id { get; internal set; }
    }
}