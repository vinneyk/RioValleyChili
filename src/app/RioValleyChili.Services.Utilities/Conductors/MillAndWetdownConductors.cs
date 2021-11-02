using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;
using RioValleyChili.Services.Utilities.Commands.Production;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;
using IProductionUnitOfWork = RioValleyChili.Data.Interfaces.UnitsOfWork.IProductionUnitOfWork;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class MillAndWetdownConductorBase
    {
        protected readonly IProductionUnitOfWork ProductionUnitOfWork;

        internal MillAndWetdownConductorBase(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            ProductionUnitOfWork = productionUnitOfWork;
        }

        protected IResult<ChileLotProduction> SetMillAndWetdown<TParams>(ChileLotProduction millAndWetdown, SetMillAndWetdownParameters<TParams> parameters, DateTime timestamp, IEmployeeKey employee)
            where TParams : ISetMillAndWetdownParameters
        {
            millAndWetdown.ProductionType = ProductionType.MillAndWetdown;
            millAndWetdown.EmployeeId = employee.EmployeeKey_Id;
            millAndWetdown.TimeStamp = timestamp;

            millAndWetdown.Results.EmployeeId = employee.EmployeeKey_Id;
            millAndWetdown.Results.TimeStamp = timestamp;
            millAndWetdown.Results.ProductionLineLocationId = parameters.ProductionLineKey.LocationKey_Id;
            millAndWetdown.Results.ShiftKey = parameters.Params.ShiftKey;
            millAndWetdown.Results.ProductionBegin = parameters.Params.ProductionBegin;
            millAndWetdown.Results.ProductionEnd = parameters.Params.ProductionEnd;

            var lotKey = new LotKey(millAndWetdown);
            if(!parameters.ChileProductKey.Equals(millAndWetdown.ResultingChileLot))
            {
                var chileProduct = ProductionUnitOfWork.ChileProductRepository.FindByKey(parameters.ChileProductKey);
                if(chileProduct == null)
                {
                    return new InvalidResult<ChileLotProduction>(null, string.Format(UserMessages.ChileProductNotFound, parameters.ChileProductKey));
                }

                if(chileProduct.ChileState.ToLotType() != millAndWetdown.ResultingChileLot.ChileProduct.ChileState.ToLotType())
                {
                    return new InvalidResult<ChileLotProduction>(null, UserMessages.ChileProductDifferentLotType);
                }

                if(ProductionUnitOfWork.PickedInventoryItemRepository.Filter(PickedInventoryItemPredicates.FilterByLotKey(lotKey)).Any())
                {
                    return new InvalidResult<ChileLotProduction>(null, string.Format(UserMessages.LotHasExistingPickedInventory, lotKey));
                }

                millAndWetdown.ResultingChileLot.ChileProductId = parameters.ChileProductKey.ChileProductKey_ProductId;
            }

            var pickedInventoryItemModifications = PickedInventoryHelper.CreateModifyPickedInventoryItemParameters(millAndWetdown.PickedInventory, parameters.PickedItems);
            var validationResults = PickedInventoryValidator.ForMillAndWetdown.ValidateItems(ProductionUnitOfWork.InventoryRepository, pickedInventoryItemModifications.Where(i => i.DeltaQuantity > 0).Select(i => i.InventoryKey));
            if(!validationResults.Success)
            {
                return validationResults.ConvertTo<ChileLotProduction>();
            }

            var modifyPickedResult = new ModifyPickedInventoryItemsCommand(ProductionUnitOfWork).Execute(millAndWetdown, pickedInventoryItemModifications);
            if(!modifyPickedResult.Success)
            {
                return modifyPickedResult.ConvertTo<ChileLotProduction>();
            }

            millAndWetdown.PickedInventory.Archived = true;
            var inventoryModifications = pickedInventoryItemModifications.Select(i => i.ToModifySourceInventoryParameters()).ToList();
            var setResults = new SetLotProductionResultItemsCommand(ProductionUnitOfWork).Execute(millAndWetdown.Results, parameters.ResultItems, ref inventoryModifications);
            if(!setResults.Success)
            {
                return setResults.ConvertTo<ChileLotProduction>();
            }

            var transaction = new InventoryTransactionParameters(employee, timestamp, InventoryTransactionType.CreatedMillAndWetdown, lotKey, millAndWetdown);
            var modifyInventoryResult = new ModifyInventoryCommand(ProductionUnitOfWork).Execute(inventoryModifications, transaction);
            if(!modifyInventoryResult.Success)
            {
                return modifyInventoryResult.ConvertTo<ChileLotProduction>();
            }

            return new SuccessResult<ChileLotProduction>(millAndWetdown);
        }
    }

    internal class UpdateMillAndWetdownConductor : MillAndWetdownConductorBase
    {
        internal UpdateMillAndWetdownConductor(IProductionUnitOfWork productionUnitOfWork) : base(productionUnitOfWork) { }

        internal IResult<ChileLotProduction> Execute(DateTime timeStamp, UpdateMillAndWetdownParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(ProductionUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ChileLotProduction>();
            }

            var chileLotResult = ProductionUnitOfWork.ChileLotProductionRepository.FindByKey(parameters.LotKey,
                c => c.ResultingChileLot.ChileProduct,
                c => c.PickedInventory.Items.Select(i => i.CurrentLocation),
                c => c.Results.ResultItems);
            if(chileLotResult == null || chileLotResult.ProductionType != ProductionType.MillAndWetdown)
            {
                return new InvalidResult<ChileLotProduction>(null, string.Format(UserMessages.MillAndWetdownEntryNotFound, parameters.LotKey));
            }

            return SetMillAndWetdown(chileLotResult, parameters, timeStamp, employeeResult.ResultingObject);
        }
    }

    internal class CreateMillAndWetdownConductor : MillAndWetdownConductorBase
    {
        internal CreateMillAndWetdownConductor(IProductionUnitOfWork productionUnitOfWork) : base(productionUnitOfWork) { }

        internal IResult<ChileLotProduction> Execute(DateTime timeStamp, CreateMillAndWetdownParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(ProductionUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ChileLotProduction>();
            }
            var employee = employeeResult.ResultingObject;

            var chileLotResult = new CreateNewChileLotCommand(ProductionUnitOfWork).Execute(new CreateNewChileLotCommandParameters
                {
                    EmployeeKey = employeeResult.ResultingObject,
                    TimeStamp = timeStamp,
                    LotDate = parameters.Params.ProductionDate,
                    LotSequence = parameters.Params.LotSequence,
                    LotType = LotTypeEnum.WIP,
                    ChileProductKey = parameters.ChileProductKey,
                    SetLotProductionStatus = LotProductionStatus.Produced,
                    SetLotQualityStatus = LotQualityStatus.Pending
                });
            if(!chileLotResult.Success)
            {
                return chileLotResult.ConvertTo<ChileLotProduction>();
            }
            var chileLot = chileLotResult.ResultingObject;

            var pickedInventoryResult = new CreatePickedInventoryCommand(ProductionUnitOfWork).Execute(new CreatePickedInventoryCommandParameters
                {
                    PickedReason = PickedReason.Production,
                    EmployeeKey = employee,
                    TimeStamp = timeStamp
                });
            if(!pickedInventoryResult.Success)
            {
                return pickedInventoryResult.ConvertTo<ChileLotProduction>();
            }

            var millAndWetdown = ProductionUnitOfWork.ChileLotProductionRepository.Add(new ChileLotProduction
                {
                    LotDateCreated = chileLot.LotDateCreated,
                    LotDateSequence = chileLot.LotDateSequence,
                    LotTypeId = chileLot.LotTypeId,
                    ResultingChileLot = chileLot,

                    PickedInventoryDateCreated = pickedInventoryResult.ResultingObject.DateCreated,
                    PickedInventorySequence = pickedInventoryResult.ResultingObject.Sequence,
                    PickedInventory = pickedInventoryResult.ResultingObject,

                    Results = new LotProductionResults
                        {
                            LotDateCreated = chileLot.LotDateCreated,
                            LotDateSequence = chileLot.LotDateSequence,
                            LotTypeId = chileLot.LotTypeId,
                            
                            DateTimeEntered = timeStamp,
                            ResultItems = new List<LotProductionResultItem>()
                        }
                });

            return SetMillAndWetdown(millAndWetdown, parameters, timeStamp, employee);
        }
    }

    internal class DeleteMillAndWetdownConductor
    {
        private readonly IProductionUnitOfWork _productionUnitOfWork;

        internal DeleteMillAndWetdownConductor(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            _productionUnitOfWork = productionUnitOfWork;
        }

        internal IResult Execute(LotKey lotKey)
        {
            var includePaths = DeleteLotConductor.ConstructIncludePaths<ChileLotProduction>(p => p.ResultingChileLot.Lot,
                p => p.PickedInventory.Items.Select(i => i.CurrentLocation),
                p => p.Results.ResultItems).ToArray();
            var lotProduction = _productionUnitOfWork.ChileLotProductionRepository.FindByKey(lotKey, includePaths);
            if(lotProduction == null)
            {
                return new NoWorkRequiredResult();
            }

            if(lotProduction.ProductionType != ProductionType.MillAndWetdown)
            {
                return new InvalidResult(string.Format(UserMessages.MillAndWetdownEntryNotFound, lotKey));
            }

            var pickedInventoryItemModifications = PickedInventoryHelper.CreateModifyPickedInventoryItemParameters(lotProduction.PickedInventory, new List<PickedInventoryParameters>());
            var modifyPickedResult = new ModifyPickedInventoryItemsCommand(_productionUnitOfWork).Execute(lotProduction, pickedInventoryItemModifications);
            if(!modifyPickedResult.Success)
            {
                return modifyPickedResult.ConvertTo<ChileLotProduction>();
            }

            var inventoryModifications = pickedInventoryItemModifications.Select(i => i.ToModifySourceInventoryParameters()).ToList();
            var setResults = new SetLotProductionResultItemsCommand(_productionUnitOfWork).Execute(lotProduction.Results, new List<IProductionResultItemParameters>(), ref inventoryModifications);
            if(!setResults.Success)
            {
                return setResults.ConvertTo<ChileLotProduction>();
            }
            
            var modifyInventoryResult = new ModifyInventoryCommand(_productionUnitOfWork).Execute(inventoryModifications, null);
            if(!modifyInventoryResult.Success)
            {
                return modifyInventoryResult.ConvertTo<ChileLotProduction>();
            }

            _productionUnitOfWork.LotProductionResultsRepository.Remove(lotProduction.Results);
            _productionUnitOfWork.PickedInventoryRepository.Remove(lotProduction.PickedInventory);

            var deleteLotResult = new DeleteLotConductor(_productionUnitOfWork).Delete(lotProduction.ResultingChileLot.Lot);
            if(!deleteLotResult.Success)
            {
                return deleteLotResult;
            }

            return new SuccessResult();
        }
    }
}
