using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Production;
using Solutionhead.Data;
using Solutionhead.Services;
using IProductionUnitOfWork = RioValleyChili.Data.Interfaces.UnitsOfWork.IProductionUnitOfWork;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class ProductionBatchResultsConductor
    {
        private readonly Data.Interfaces.UnitsOfWork.IProductionUnitOfWork _productionUnitOfWork;

        public ProductionBatchResultsConductor(Data.Interfaces.UnitsOfWork.IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        public IResult<LotProductionResults> Create(DateTime timestamp, ProductionResultParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var batchResult = GetBatch(parameters.LotKey);
            if(!batchResult.Success)
            {
                return batchResult.ConvertTo<LotProductionResults>();
            }

            if(batchResult.ResultingObject.ProductionHasBeenCompleted)
            {
                return new InvalidResult<LotProductionResults>(null, string.Format(UserMessages.ProductionBatchAlreadyComplete, parameters.LotKey.KeyValue));
            }

            if(batchResult.ResultingObject.Production.Results != null)
            {
                return new InvalidResult<LotProductionResults>(null, string.Format(UserMessages.ProductionBatchHasResult, parameters.LotKey.KeyValue));
            }
            
            parameters.PackScheduleKey = new PackScheduleKey(batchResult.ResultingObject);

            batchResult.ResultingObject.Production.Results = _productionUnitOfWork.LotProductionResultsRepository.Add(new LotProductionResults
                {
                    LotDateCreated = batchResult.ResultingObject.LotDateCreated,
                    LotDateSequence = batchResult.ResultingObject.LotDateSequence,
                    LotTypeId = batchResult.ResultingObject.LotTypeId,
                    DateTimeEntered = timestamp,
                    ResultItems = new List<LotProductionResultItem>(),
                    Production = batchResult.ResultingObject.Production
                });

            return SetProductionResults(batchResult.ResultingObject, timestamp, parameters, true);
        }

        public IResult<LotProductionResults> Update(DateTime timestamp, ProductionResultParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var batchResult = GetBatch(parameters.LotKey);
            if(!batchResult.Success)
            {
                return batchResult.ConvertTo<LotProductionResults>();
            }

            if(batchResult.ResultingObject == null)
            {
                return new InvalidResult<LotProductionResults>(null, string.Format(UserMessages.ProductionBatchNotFound, parameters.LotKey));
            }
            
            if(batchResult.ResultingObject.Production.Results == null)
            {
                return new InvalidResult<LotProductionResults>(null, string.Format(UserMessages.ProductionResultNotFound, parameters.LotKey));
            }
            
            parameters.PackScheduleKey = new PackScheduleKey(batchResult.ResultingObject);

            return SetProductionResults(batchResult.ResultingObject, timestamp, parameters, false);
        }

        private IResult<ProductionBatch> GetBatch(IKey<ProductionBatch> lotKey)
        {
            var batch = _productionUnitOfWork.ProductionBatchRepository.FindByKey(lotKey,
                b => b.Production.Results.ResultItems,
                b => b.Production.ResultingChileLot.Lot.Attributes.Select(a => a.AttributeName),
                b => b.Production.PickedInventory.Items.Select(i => i.CurrentLocation));
            if(batch == null)
            {
                return new InvalidResult<ProductionBatch>(null, string.Format(UserMessages.ProductionBatchNotFound, lotKey));
            }

            return new SuccessResult<ProductionBatch>(batch);
        }

        private IResult<LotProductionResults> SetProductionResults(ProductionBatch productionBatch, DateTime timestamp, ProductionResultParameters parameters, bool logOriginalPicked)
        {
            if(productionBatch == null) { throw new ArgumentNullException("productionBatch"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employee = new GetEmployeeCommand(_productionUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employee.Success)
            {
                return employee.ConvertTo<LotProductionResults>();
            }

            var productionLine = _productionUnitOfWork.LocationRepository.FindByKey(parameters.ProductionLineLocationKey);
            if(productionLine == null)
            {
                return new InvalidResult<LotProductionResults>(null, string.Format(UserMessages.ProductionLocationNotFound, parameters.ProductionLineLocationKey));
            }

            productionBatch.ProductionHasBeenCompleted = true;
            productionBatch.Production.PickedInventory.Archived = true;
            productionBatch.Production.ResultingChileLot.Lot.ProductionStatus = LotProductionStatus.Produced;

            var productionResults = productionBatch.Production.Results;
            productionResults.EmployeeId = employee.ResultingObject.EmployeeId;
            productionResults.TimeStamp = timestamp;
            productionResults.ProductionLineLocation = productionLine;
            productionResults.ProductionLineLocationId = productionLine.Id;
            productionResults.ProductionBegin = parameters.Parameters.ProductionStartTimestamp;
            productionResults.ProductionEnd = parameters.Parameters.ProductionEndTimestamp;
            productionResults.ShiftKey = parameters.Parameters.ProductionShiftKey;

            List<ModifyInventoryParameters> inventoryModifications = null;
            var updateResultItems = new SetLotProductionResultItemsCommand(_productionUnitOfWork).Execute(productionResults, parameters.InventoryItems, ref inventoryModifications);
            if(!updateResultItems.Success)
            {
                return updateResultItems.ConvertTo<LotProductionResults>();
            }

            var transactionParameters = new InventoryTransactionParameters(employee.ResultingObject, timestamp, InventoryTransactionType.ProductionResults, parameters.TransactionSourceReference, productionResults);
            if(logOriginalPicked)
            {
                var logPicked = new CreateInventoryTransactionCommand(_productionUnitOfWork).LogPickedInventory(transactionParameters, productionResults.Production.PickedInventory.Items);
                if(!logPicked.Success)
                {
                    return logPicked.ConvertTo<LotProductionResults>();
                }
            }

            var updatePickedItems = UpdatePickedItems(employee.ResultingObject, timestamp, productionBatch, parameters.PickedInventoryItemChanges, ref inventoryModifications);
            if(!updatePickedItems.Success)
            {
                return updatePickedItems.ConvertTo<LotProductionResults>();
            }

            var modifyInventory = new ModifyInventoryCommand(_productionUnitOfWork).Execute(inventoryModifications, transactionParameters);
            if(!modifyInventory.Success)
            {
                return modifyInventory.ConvertTo<LotProductionResults>();
            }

            return new SuccessResult<LotProductionResults>(productionResults);
        }

        private IResult UpdatePickedItems(Employee employee, DateTime timeStamp, ProductionBatch productionBatch, List<PickedInventoryParameters> pickedInventoryChanges, ref List<ModifyInventoryParameters> inventoryModifications)
        {
            if(!pickedInventoryChanges.Any())
            {
                return new SuccessResult();
            }

            var setItems = productionBatch.Production.PickedInventory.Items.Select(i => new PickedInventoryParameters(i)).ToList();
            foreach(var change in pickedInventoryChanges)
            {
                var existing = setItems.FirstOrDefault(i => i.Match(change));
                if(existing != null)
                {
                    existing.Quantity += change.Quantity;
                }
                else
                {
                    setItems.Add(change);
                }
            }

            return new ProductionBatchPickInventoryConductor(_productionUnitOfWork).UpdatePickedWithoutModifyingInventory(employee, timeStamp, productionBatch, setItems, ref inventoryModifications);
        }
    }
}