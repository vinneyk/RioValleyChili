using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class RemoveLotDefectResolutionConductor
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal RemoveLotDefectResolutionConductor(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        public IResult RemoveLotDefectResolution(RemoveLotDefectResolutionParameters parameters, DateTime timestamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_lotUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            var lotDefect = _lotUnitOfWork.LotDefectRepository.FindByKey(parameters.LotDefectKey,
                d => d.Resolution,
                d => d.Lot.ChileLot,
                d => d.Lot.Attributes);
            if(lotDefect == null)
            {
                return new InvalidResult(string.Format(UserMessages.LotDefectNotFound, parameters.LotDefectKey));
            }

            var historyResult = new RecordLotHistoryCommand(_lotUnitOfWork).Execute(lotDefect.Lot, timestamp);
            if(!historyResult.Success)
            {
                return historyResult;
            }

            if(lotDefect.Resolution == null)
            {
                return new NoWorkRequiredResult(string.Format(UserMessages.LotDefectHasNoResolution, parameters.LotDefectKey));
            }

            lotDefect.Lot.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            lotDefect.Lot.TimeStamp = timestamp;

            _lotUnitOfWork.LotDefectResolutionRepository.Remove(lotDefect.Resolution);

            return new UpdateChileLotStatusCommand(_lotUnitOfWork).Execute(lotDefect);
        }
    }
}