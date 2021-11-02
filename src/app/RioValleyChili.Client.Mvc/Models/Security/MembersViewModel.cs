using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;

namespace RioValleyChili.Client.Mvc.Models.Security
{
    public class MembersViewModel
    {
        public IEnumerable<IEmployeeSummaryReturn> Members { get; set; }
        public IEnumerable<string> PermissionOptions { get; set; }
    }
}