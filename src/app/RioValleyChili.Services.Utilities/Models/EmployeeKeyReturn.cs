using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class EmployeeKeyReturn : IEmployeeKey
    {
        public int EmployeeKey_Id { get; internal set; }

        internal string EmployeeKey { get { return new EmployeeKey(this); } }
    }
}