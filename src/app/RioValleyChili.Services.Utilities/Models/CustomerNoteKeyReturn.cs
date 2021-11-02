using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerNoteKeyReturn : ICustomerNoteKey
    {
        public string CustomerNoteKey { get { return new CustomerNoteKey(this); } }
        public int CustomerKey_Id { get; internal set; }
        public int CustomerNoteKey_Id { get; internal set; }
    }
}