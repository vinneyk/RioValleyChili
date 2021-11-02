using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class CreateInHouseContaminationDefectCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal CreateInHouseContaminationDefectCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult<LotDefect> Execute(CreateLotDefectParameters parameters, DateTime timestamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_lotUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<LotDefect>();
            }

            var lotKey = parameters.LotKey;
            var lot = _lotUnitOfWork.LotRepository.FindByKey(lotKey, l => l.ChileLot, l => l.Attributes);
            if(lot == null)
            {
                return new InvalidResult<LotDefect>(null, string.Format(UserMessages.LotNotFound, lotKey.KeyValue));
            }

            var historyResult = new RecordLotHistoryCommand(_lotUnitOfWork).Execute(lot, timestamp);
            if(!historyResult.Success)
            {
                return historyResult.ConvertTo<LotDefect>();
            }

            var newDefectId = new EFUnitOfWorkHelper(_lotUnitOfWork).GetNextSequence<LotDefect>(d => d.LotDateCreated == lot.LotDateCreated && d.LotDateSequence == lot.LotDateSequence && d.LotTypeId == lot.LotTypeId, d => d.DefectId);
            var newDefect = _lotUnitOfWork.LotDefectRepository.Add(new LotDefect
                {
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    LotTypeId = lot.LotTypeId,
                    DefectId = newDefectId,

                    DefectType = DefectTypeEnum.InHouseContamination,
                    Description = parameters.Parameters.Description
                });

            lot.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            lot.TimeStamp = timestamp;

            return new SuccessResult<LotDefect>(newDefect);
        }
    }
}