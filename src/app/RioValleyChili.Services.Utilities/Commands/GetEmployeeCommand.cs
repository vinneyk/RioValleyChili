using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands
{
    internal class GetEmployeeCommand
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;

        internal GetEmployeeCommand(ICoreUnitOfWork coreUnitOfWork)
        {
            if (coreUnitOfWork == null) throw new ArgumentNullException("coreUnitOfWork");
            _coreUnitOfWork = coreUnitOfWork;
        }

        internal IResult<Employee> GetEmployee(IUserIdentifiable userIdentifiable)
        {
            //todo: Introduce 'Token' and 'TokenExpiration' properties to Employee class.
            var employee = _coreUnitOfWork.EmployeesRepository.FindBy(e => e.UserName.Equals(userIdentifiable.UserToken));
            if(employee == null)
            { 
                return new InvalidResult<Employee>(null, UserMessages.EmployeeByTokenNotFound);
            }

            return new SuccessResult<Employee>(employee);
        }
    }
}