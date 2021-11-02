using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class CreateNewChileLotCommand : CreateNewLotCommandBase
    {
        internal CreateNewChileLotCommand(ILotUnitOfWork lotUnitOfWork) : base(lotUnitOfWork) { }

        internal IResult<ChileLot> Execute(CreateNewChileLotCommandParameters parameters)
        {
            var createNewLotResult = CreateLot(parameters);
            if(!createNewLotResult.Success)
            {
                return createNewLotResult.ConvertTo<ChileLot>();
            }
            var newLot = createNewLotResult.ResultingObject;

            var newChileLot = LotUnitOfWork.ChileLotRepository.Add(new ChileLot
                {
                    Lot = newLot,
                    LotDateCreated = newLot.LotDateCreated,
                    LotDateSequence = newLot.LotDateSequence,
                    LotTypeId = newLot.LotTypeId,
                    ChileProductId = parameters.ChileProductKey.ChileProductKey_ProductId,
                    AllAttributesAreLoBac = false
                });
            
            return new SuccessResult<ChileLot>(newChileLot);
        }
    }
}