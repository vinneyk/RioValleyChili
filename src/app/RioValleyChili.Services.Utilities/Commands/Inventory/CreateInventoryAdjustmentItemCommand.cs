using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class CreateInventoryAdjustmentItemCommand
    {
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;

        internal CreateInventoryAdjustmentItemCommand(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;
        }

        internal IResult<InventoryAdjustmentItem> Execute(InventoryAdjustment inventoryAdjustment, CreateInventoryAdjustmentItemCommandParameters parameters)
        {
            if(inventoryAdjustment == null) { throw new ArgumentNullException("inventoryAdjustment"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.InventoryAdjustmentParameters.Adjustment == 0)
            {
                return new InvalidResult<InventoryAdjustmentItem>(null, UserMessages.AdjustmentQuantityCannotBeZero);
            }

            var nextSequence = new EFUnitOfWorkHelper(_inventoryUnitOfWork).GetNextSequence<InventoryAdjustmentItem>(i =>
                i.AdjustmentDate == inventoryAdjustment.InventoryAdjustmentKey_AdjustmentDate && i.Sequence == inventoryAdjustment.InventoryAdjustmentKey_Sequence,
                i => i.ItemSequence);

            var adjustment = _inventoryUnitOfWork.InventoryAdjustmentItemRepository.Add(new InventoryAdjustmentItem
                {
                    AdjustmentDate = inventoryAdjustment.AdjustmentDate,
                    Sequence = inventoryAdjustment.Sequence,
                    ItemSequence = nextSequence,
                    EmployeeId = inventoryAdjustment.EmployeeId,
                    TimeStamp = inventoryAdjustment.TimeStamp,

                    QuantityAdjustment = parameters.InventoryAdjustmentParameters.Adjustment,
                    LotDateCreated = parameters.LotKey.LotKey_DateCreated,
                    LotDateSequence = parameters.LotKey.LotKey_DateSequence,
                    LotTypeId = parameters.LotKey.LotKey_LotTypeId,
                    PackagingProductId = parameters.PackagingProductKey.PackagingProductKey_ProductId,
                    LocationId = parameters.LocationKey.LocationKey_Id,
                    TreatmentId = parameters.InventoryTreatmentKey.InventoryTreatmentKey_Id,
                    ToteKey = parameters.ToteKey
                });

            return new SuccessResult<InventoryAdjustmentItem>(adjustment);
        }
    }
}