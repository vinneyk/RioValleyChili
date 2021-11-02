using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IEmployeesService
    {
        IResult<IEmployeeDetailsReturn> GetEmployeeDetailsByUserName(string userName);
        IResult ActivateEmployee(IActivateEmployeeParameters values);
        IResult<string> CreateEmployee(ICreateEmployeeParameters values);
        IResult<IQueryable<IEmployeeSummaryReturn>> GetEmployees();
        IResult<IEmployeeDetailsReturn> GetEmployeeDetailsByKey(string memberKey);
        IResult UpdateEmployee(IUpdateEmployeeParameters values);
    }
}