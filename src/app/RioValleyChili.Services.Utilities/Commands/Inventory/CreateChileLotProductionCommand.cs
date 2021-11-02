using System;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class CreateChileLotProductionCommand
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        public CreateChileLotProductionCommand(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult<ChileLotProduction> Execute(ICreateChileLotProduction parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var pickedInventoryResult = new CreatePickedInventoryCommand(_productionUnitOfWork).Execute(parameters);
            if(!pickedInventoryResult.Success)
            {
                return pickedInventoryResult.ConvertTo<ChileLotProduction>();
            }
            var pickedInventory = pickedInventoryResult.ResultingObject;

            var chileLotProduction = _productionUnitOfWork.ChileLotProductionRepository.Add(new ChileLotProduction
                {
                    EmployeeId = pickedInventory.EmployeeId,
                    TimeStamp = pickedInventory.TimeStamp,

                    LotDateCreated = parameters.LotKey.LotKey_DateCreated,
                    LotDateSequence = parameters.LotKey.LotKey_DateSequence,
                    LotTypeId = parameters.LotKey.LotKey_LotTypeId,

                    ProductionType = parameters.ProductionType,
                    PickedInventory = pickedInventory
                });

            return new SuccessResult<ChileLotProduction>(chileLotProduction);
        }
    }
}