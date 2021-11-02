using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;

namespace RioValleyChili.Client.Mvc.Models.Employees
{
    public class ActivateEmployeeParameters : IActivateEmployeeParameters
    {
        public string EmployeeKey { get; internal set; }

        public string EmailAddress { get; set; }
    }
}