using System;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.EmployeeService;
using RioValleyChili.Services.Interfaces.Returns.EmployeeService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class EmployeesService : IEmployeesService
    {
        private readonly EmployeesServiceProvider _employeesServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public EmployeesService(EmployeesServiceProvider employeesServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(employeesServiceProvider == null) throw new ArgumentNullException("employeesServiceProvider");
            _employeesServiceProvider = employeesServiceProvider;

            if (exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }

        public IResult<IEmployeeDetailsReturn> GetEmployeeDetailsByUserName(string userName)
        {
            try
            {
                return _employeesServiceProvider.GetEmployeeDetailsByUserName(userName);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEmployeeDetailsReturn>(null, "Internal failure occurred while attempting to get employee details.");
            }
        }

        public IResult ActivateEmployee(IActivateEmployeeParameters values)
        {
            try
            {
                return _employeesServiceProvider.ActivateEmployee(values);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult("");
            }
        }

        public IResult<string> CreateEmployee(ICreateEmployeeParameters values)
        {
            try
            {
                return _employeesServiceProvider.CreateEmployee(values);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>("");
            }
        }

        public IResult<IQueryable<IEmployeeSummaryReturn>> GetEmployees()
        {
            try
            {
                return _employeesServiceProvider.GetEmployeeSummaries();
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IEmployeeSummaryReturn>>();
            }
        }

        public IResult<IEmployeeDetailsReturn> GetEmployeeDetailsByKey(string memberKey)
        {
            try
            {
                return _employeesServiceProvider.GetEmployeeDetailsByEmployeeKey(memberKey);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEmployeeDetailsReturn>();
            }
        }

        public IResult UpdateEmployee(IUpdateEmployeeParameters values)
        {
            try
            {
                return _employeesServiceProvider.UpdateEmployee(values);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult();
            }
        }
    }
}