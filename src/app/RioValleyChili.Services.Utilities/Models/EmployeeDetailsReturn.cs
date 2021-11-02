using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class EmployeeDetailsReturn : IEmployeeDetailsReturn
    {
        public string EmployeeKey { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public IDictionary<string, string> Claims { get; set; }
    }
}