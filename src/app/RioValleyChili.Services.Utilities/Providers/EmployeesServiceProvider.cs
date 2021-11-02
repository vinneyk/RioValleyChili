using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class EmployeesServiceProvider
    {
        readonly ICoreUnitOfWork _coreUnitOfWork;

        public EmployeesServiceProvider(ICoreUnitOfWork coreUnitOfWork)
        {
            if(coreUnitOfWork == null) throw new ArgumentNullException("coreUnitOfWork");
            _coreUnitOfWork = coreUnitOfWork;
        }

        public IResult<IEmployeeDetailsReturn> GetEmployeeDetailsByUserName(string userName)
        {
            var employee = _coreUnitOfWork.EmployeesRepository.FindBy(e => e.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if(employee == null)
            {
                return new InvalidResult<IEmployeeDetailsReturn>(null, string.Format(UserMessages.EmployeeByUserNameNotFound, userName));
            }

            return new SuccessResult<IEmployeeDetailsReturn>(new EmployeeDetailsReturn
                {
                    EmployeeKey = new EmployeeKey(employee).KeyValue,
                    DisplayName = employee.DisplayName,
                    UserName = employee.UserName,
                    EmailAddress = employee.EmailAddress,
                    IsActive = employee.IsActive,
                    Claims = ClaimsSerializationHelper.Deserialize(employee.Claims)
                });
        }

        public IResult<IEmployeeDetailsReturn> GetEmployeeDetailsByEmployeeKey(string employeeKeyValue)
        {
            var employeeKeyResult = KeyParserHelper.ParseResult<IEmployeeKey>(employeeKeyValue);
            if (!employeeKeyResult.Success) return employeeKeyResult.ConvertTo<IEmployeeDetailsReturn>();

            var employeeKey = new EmployeeKey(employeeKeyResult.ResultingObject);
            var employee = _coreUnitOfWork.EmployeesRepository.FindByKey(employeeKey);
            if(employee == null)
            {
                return new InvalidResult<IEmployeeDetailsReturn>(null, string.Format(UserMessages.EmployeeByKeyNotFound, employeeKeyValue));
            }

            return new SuccessResult<IEmployeeDetailsReturn>(new EmployeeDetailsReturn
                {
                    EmployeeKey = new EmployeeKey(employee).KeyValue,
                    DisplayName = employee.DisplayName,
                    UserName = employee.UserName,
                    EmailAddress = employee.EmailAddress,
                    IsActive = employee.IsActive,
                    Claims = ClaimsSerializationHelper.Deserialize(employee.Claims)
                });
        }

        public IResult ActivateEmployee(IActivateEmployeeParameters values)
        {
            var employeeKey = KeyParserHelper.ParseResult<IEmployeeKey>(values.EmployeeKey);
            var employee = _coreUnitOfWork.EmployeesRepository.FindBy(e => e.EmployeeId.Equals(employeeKey.ResultingObject.EmployeeKey_Id));
            if(employee == null)
            {
                return new InvalidResult<IEmployeeDetailsReturn>(null, string.Format(UserMessages.EmployeeByKeyNotFound, values.EmployeeKey));
            }

            employee.EmailAddress = values.EmailAddress;
            employee.IsActive = true;
            _coreUnitOfWork.Commit();
            return new SuccessResult("Employee activated successfully.");
        }

        public IResult<string> CreateEmployee(ICreateEmployeeParameters values)
        {
            throw new NotImplementedException();
        }

        public IResult<IQueryable<IEmployeeSummaryReturn>> GetEmployeeSummaries()
        {
            var employees = _coreUnitOfWork.EmployeesRepository.All()
                .Select(e => new EmployeeSummaryReturn
                    {
                        EmployeeId = e.EmployeeId,
                        DisplayName = e.DisplayName,
                        EmailAddress = e.EmailAddress,
                        IsActive = e.IsActive,
                        UserName = e.UserName,
                    });

            return new SuccessResult<IQueryable<IEmployeeSummaryReturn>>(employees);
        }

        public IResult UpdateEmployee(IUpdateEmployeeParameters values)
        {
            var parseKeyResult = KeyParserHelper.ParseResult<IEmployeeKey>(values.EmployeeKey);
            if (!parseKeyResult.Success) return parseKeyResult;

            var employeeKey = new EmployeeKey(parseKeyResult.ResultingObject);
            var employee = _coreUnitOfWork.EmployeesRepository.FindByKey(employeeKey);
            if (employee == null)
            {
                return new InvalidResult(string.Format(UserMessages.EmployeeByKeyNotFound, employeeKey));
            }

            employee.DisplayName = values.DisplayName;
            employee.EmailAddress = values.EmailAddress;
            employee.IsActive = values.IsActive;
            employee.UserName = values.UserName;
            employee.Claims = ClaimsSerializationHelper.Serialize(values.Claims);

            _coreUnitOfWork.Commit();
            return new SuccessResult("Employee updated successfully.");
        }
    }
}