using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class CreateLotDefectResolutionCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal CreateLotDefectResolutionCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult<LotDefectResolution> Execute(CreateLotDefectResolutionParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.LotDefect.Resolution != null)
            {
                return new InvalidResult<LotDefectResolution>(null, string.Format(UserMessages.LotDefectHasResolution, new LotDefectKey(parameters.LotDefect)));
            }

            if(!parameters.LotDefect.DefectType.GetValidResolutions().Contains(parameters.ResolutionType))
            {
                return new InvalidResult<LotDefectResolution>(null, string.Format(UserMessages.InvalidDefectResolutionType, parameters.ResolutionType, parameters.LotDefect.DefectType));
            }

            var lotDefectResolution = _lotUnitOfWork.LotDefectResolutionRepository.Add(new LotDefectResolution
                {
                    EmployeeId = parameters.Employee.EmployeeId,
                    TimeStamp = parameters.TimeStamp,

                    LotDateCreated = parameters.LotDefect.LotDateCreated,
                    LotDateSequence = parameters.LotDefect.LotDateSequence,
                    LotTypeId = parameters.LotDefect.LotTypeId,
                    DefectId = parameters.LotDefect.DefectId,

                    ResolutionType = parameters.ResolutionType,
                    Description = parameters.Description ?? ""
                });

            return new SuccessResult<LotDefectResolution>(lotDefectResolution);
        }
    }
}