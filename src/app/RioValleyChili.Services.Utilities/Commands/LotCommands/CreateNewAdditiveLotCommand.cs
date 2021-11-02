using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class CreateNewAdditiveLotCommand : CreateNewLotCommandBase
    {
        public CreateNewAdditiveLotCommand(ILotUnitOfWork lotUnitOfWork) : base(lotUnitOfWork) { }

        internal IResult<AdditiveLot> Execute(CreateNewAdditiveLotCommandParameters parameters)
        {
            var createNewLotResult = CreateLot(parameters);
            if(!createNewLotResult.Success)
            {
                return createNewLotResult.ConvertTo((AdditiveLot)null);
            }

            var newLot = createNewLotResult.ResultingObject;
            var newAdditiveLot = LotUnitOfWork.AdditiveLotRepository.Add(new AdditiveLot
                {
                    Lot = newLot,
                    LotDateCreated = newLot.LotDateCreated,
                    LotDateSequence = newLot.LotDateSequence,
                    LotTypeId = newLot.LotTypeId,
                    AdditiveProductId = parameters.AdditiveProductKey.AdditiveProductKey_Id,
                });

            return new SuccessResult<AdditiveLot>(newAdditiveLot);
        }
    }
}