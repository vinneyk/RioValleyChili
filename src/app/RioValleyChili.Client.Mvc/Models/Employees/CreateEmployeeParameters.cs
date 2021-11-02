using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;

namespace RioValleyChili.Client.Mvc.Models.Employees
{
    public class CreateEmployeeParameters : ICreateEmployeeParameters
    {
        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string DisplayName { get; set; }
    }
}