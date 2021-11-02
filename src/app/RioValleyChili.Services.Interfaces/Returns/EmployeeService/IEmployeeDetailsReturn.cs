using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.EmployeeService
{
    public interface IEmployeeDetailsReturn
    {
        string EmployeeKey { get; }

        string DisplayName { get; }

        string UserName { get; }

        string EmailAddress { get; }

        bool IsActive { get; }

        IDictionary<string, string> Claims { get; }
    }
}