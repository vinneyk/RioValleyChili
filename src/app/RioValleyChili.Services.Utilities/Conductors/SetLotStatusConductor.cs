using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetLotStatusConductor
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal SetLotStatusConductor(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Execute(DateTime timestamp, SetLotStatusParameters parameters, bool forceResolveAllDefects = false)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_lotUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult;
            }

            return Execute(timestamp, employeeResult.ResultingObject, parameters.LotKey, parameters.Parameters.QualityStatus, forceResolveAllDefects);
        }

        internal IResult Execute(DateTime timestamp, Employee employee, LotKey lotKey, LotQualityStatus status, bool forceResolveAllDefects = false)
        {
            var lot = _lotUnitOfWork.LotRepository.FindByKey(lotKey,
                l => l.ChileLot,
                l => l.Attributes.Select(a => a.AttributeName),
                l => l.LotDefects.Select(d => d.Resolution));
            if(lot == null)
            {
                return new InvalidResult(string.Format(UserMessages.LotNotFound, lotKey));
            }

            var historyResult = new RecordLotHistoryCommand(_lotUnitOfWork).Execute(lot, timestamp);
            if(!historyResult.Success)
            {
                return historyResult;
            }

            if(forceResolveAllDefects)
            {
                foreach(var defect in lot.LotDefects)
                {
                    if(defect.Resolution == null)
                    {
                        defect.Resolution = new LotDefectResolution
                            {
                                EmployeeId = employee.EmployeeId,
                                TimeStamp = timestamp,
                                ResolutionType = ResolutionTypeEnum.AcceptedByUser,
                                Description = "Forced acceptance by user."
                            };
                    }
                }
            }

            if(lot.ChileLot != null)
            {
                var chileLot = _lotUnitOfWork.ChileLotRepository.FindByKey(lotKey, LotStatusHelper.ChileLotIncludePaths);
                if(chileLot == null)
                {
                    return new InvalidResult(string.Format(UserMessages.ChileLotNotFound, lotKey));
                }

                if(!LotStatusHelper.GetValidLotQualityStatuses(chileLot).Contains(status))
                {
                    return new InvalidResult(string.Format(UserMessages.CannotSetLotStatus, lotKey, chileLot.Lot.QualityStatus, status));
                }
            }

            lot.QualityStatus = status;
            lot.EmployeeId = employee.EmployeeId;
            lot.TimeStamp = timestamp;

            return new SuccessResult();
        }
    }
}