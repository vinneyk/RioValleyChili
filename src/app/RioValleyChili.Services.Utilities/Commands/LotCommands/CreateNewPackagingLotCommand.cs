using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class CreateNewPackagingLotCommand : CreateNewLotCommandBase
    {
        internal CreateNewPackagingLotCommand(ILotUnitOfWork lotUnitOfWork) : base(lotUnitOfWork) { }

        internal IResult<PackagingLot> Execute(CreateNewPackagingLotCommandParameters parameters)
        {
            var createNewLotResult = CreateLot(parameters);
            if(!createNewLotResult.Success)
            {
                createNewLotResult.ConvertTo((PackagingLot) null);
            }

            var newLot = createNewLotResult.ResultingObject;
            var newPackagingLot = LotUnitOfWork.PackagingLotRepository.Add(new PackagingLot
                {
                    Lot = newLot,
                    LotDateCreated = newLot.LotDateCreated,
                    LotDateSequence = newLot.LotDateSequence,
                    LotTypeId = newLot.LotTypeId,
                    PackagingProductId = parameters.PackagingProductKey.PackagingProductKey_ProductId
                });

            return new SuccessResult<PackagingLot>(newPackagingLot);
        }
    }
}