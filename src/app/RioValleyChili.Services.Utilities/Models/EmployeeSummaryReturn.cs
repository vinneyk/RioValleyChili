using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class EmployeeSummaryReturn : IEmployeeSummaryReturn, IEmployeeKey
    {
        internal int EmployeeId { private get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }

        public string EmployeeKey
        {
            get { return new EmployeeKey(this); }
        }
        
        int IEmployeeKey.EmployeeKey_Id
        {
            get { return EmployeeId; }
        }
    }
}