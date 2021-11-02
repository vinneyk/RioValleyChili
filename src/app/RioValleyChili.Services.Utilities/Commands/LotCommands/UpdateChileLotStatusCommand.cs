using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class UpdateChileLotStatusCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal UpdateChileLotStatusCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Execute(ILotKey lotKey)
        {
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            var chileLot = _lotUnitOfWork.ChileLotRepository.FindByKey(new LotKey(lotKey), LotStatusHelper.ChileLotIncludePaths);
            if(chileLot != null)
            {
                var attributeNames = _lotUnitOfWork.AttributeNameRepository.Filter(a => true, a => a.ValidTreatments).ToList();
                LotStatusHelper.UpdateChileLotStatus(chileLot, attributeNames);
            }

            return new SuccessResult();
        }
    }
}