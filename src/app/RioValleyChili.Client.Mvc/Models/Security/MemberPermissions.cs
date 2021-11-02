using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Models.Security
{
    public class MemberPermissions
    {
        public string EmployeeKey { get; set; }

        public string EmployeeName { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Claims { get; set; }
    }
}