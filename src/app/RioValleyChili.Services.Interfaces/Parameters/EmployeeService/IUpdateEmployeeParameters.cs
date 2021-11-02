using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Parameters.EmployeeService
{
    public interface IUpdateEmployeeParameters
    {
        string EmployeeKey { get; }
        string DisplayName { get; }
        string UserName { get; }
        string EmailAddress { get; }
        IDictionary<string, string> Claims { get; }
        bool IsActive { get; }
    }
}