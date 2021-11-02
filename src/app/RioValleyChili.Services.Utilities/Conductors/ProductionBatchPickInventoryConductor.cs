using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class ProductionBatchPickInventoryConductor : PickedInventoryConductorBase<IProductionUnitOfWork>
    {
        public ProductionBatchPickInventoryConductor(IProductionUnitOfWork productionUnitOfWork) : base(productionUnitOfWork) { }

        public IResult<LotKey> Execute(DateTime timeStamp, ISetPickedInventoryParameters pickedInventory, LotKey batchKey)
        {
            var productionBatch = UnitOfWork.ProductionBatchRepository.FindByKey(batchKey,
                b => b.Production.ResultingChileLot.Lot.Attributes.Select(a => a.AttributeName),
                b => b.Production.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(productionBatch == null)
            {
                return new InvalidResult<LotKey>(null, string.Format(UserMessages.ProductionBatchNotFound, batchKey.KeyValue));
            }

            if(productionBatch.ProductionHasBeenCompleted)
            {
                return new InvalidResult<LotKey>(null, string.Format(UserMessages.ProductionBatchAlreadyComplete, batchKey.KeyValue));
            }

            var employeeResult = new GetEmployeeCommand(UnitOfWork).GetEmployee(pickedInventory);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<LotKey>();
            }

            var parsedPicked = pickedInventory.PickedInventoryItems.ToParsedParameters();
            if(!parsedPicked.Success)
            {
                return parsedPicked.ConvertTo<LotKey>();
            }

            var pickResult = UpdatePickedInventory(PickedInventoryValidator.ForProductionBatch, employeeResult.ResultingObject, timeStamp, productionBatch.Production.PickedInventory, parsedPicked.ResultingObject);
            if(!pickResult.Success)
            {
                return pickResult.ConvertTo<LotKey>();
            }

            var setWeightedAttributes = new SetLotWeightedAttributesConductor(UnitOfWork).Execute(productionBatch.Production.ResultingChileLot, productionBatch.Production.PickedInventory.Items.ToList(), timeStamp);
            if(!setWeightedAttributes.Success)
            {
                return setWeightedAttributes.ConvertTo<LotKey>();
            }

            return new SuccessResult<LotKey>(batchKey);
        }

        public IResult UpdatePickedWithoutModifyingInventory(Employee employee, DateTime timeStamp, ProductionBatch productionBatch, List<PickedInventoryParameters> setPickedInventoryItems, ref List<ModifyInventoryParameters> pendingInventoryModifications)
        {
            var updateResult = UpdatePickedInventory(PickedInventoryValidator.ForProductionBatch, employee, timeStamp, productionBatch.Production.PickedInventory, setPickedInventoryItems, ref pendingInventoryModifications);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            return new SetLotWeightedAttributesConductor(UnitOfWork).Execute(productionBatch.Production.ResultingChileLot, productionBatch.Production.PickedInventory.Items.ToList(), timeStamp);
        }
    }
}