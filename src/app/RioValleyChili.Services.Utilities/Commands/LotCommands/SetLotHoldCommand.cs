using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class SetLotHoldCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal SetLotHoldCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Execute(SetLotHoldStatusParameters parameters, DateTime timestamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_lotUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            var lot = _lotUnitOfWork.LotRepository.FindByKey(parameters.LotKey, l => l.ChileLot, l => l.Attributes);
            if(lot == null)
            {
                return new InvalidResult(string.Format(UserMessages.LotNotFound, parameters.LotKey));
            }

            var historyResult = new RecordLotHistoryCommand(_lotUnitOfWork).Execute(lot, timestamp);
            if(!historyResult.Success)
            {
                return historyResult;
            }

            if(parameters.Parameters.Hold == null)
            {
                lot.Hold = null;
                lot.HoldDescription = null;
            }
            else
            {
                lot.Hold = parameters.Parameters.Hold.HoldType;
                var description = parameters.Parameters.Hold.Description;
                if(description != null && description.Length > Constants.StringLengths.LotHoldDescription)
                {
                    description = description.Substring(0, Constants.StringLengths.LotHoldDescription);
                }
                lot.HoldDescription = description;
            }

            lot.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            lot.TimeStamp = timestamp;

            return new SuccessResult();
        }
    }
}