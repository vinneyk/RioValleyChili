using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class SetLotPackagingReceivedCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal SetLotPackagingReceivedCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Execute(SetLotPackagingReceivedParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lot = _lotUnitOfWork.LotRepository.FindByKey(parameters.LotKey);
            if(lot == null)
            {
                return new InvalidResult(string.Format(UserMessages.LotNotFound, parameters.LotKey));
            }

            var packaging = _lotUnitOfWork.PackagingProductRepository.FindByKey(parameters.PackagingProductKey);
            if(packaging == null)
            {
                return new InvalidResult(string.Format(UserMessages.PackagingProductNotFound, parameters.PackagingProductKey));
            }

            lot.ReceivedPackagingProductId = packaging.Id;

            return new SuccessResult();
        }
    }
}