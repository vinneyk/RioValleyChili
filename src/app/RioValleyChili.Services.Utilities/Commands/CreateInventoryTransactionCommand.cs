using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands
{
    internal class CreateInventoryTransactionCommand
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;

        internal CreateInventoryTransactionCommand(ICoreUnitOfWork coreUnitOfWork)
        {
            if(coreUnitOfWork == null) throw new ArgumentNullException("coreUnitOfWork");
            _coreUnitOfWork = coreUnitOfWork;
        }

        internal IResult<InventoryTransaction> Create(InventoryTransactionParameters parameters, IInventoryKey inventoryKey, int quantity)
        {
            var date = parameters.TimeStamp.Date;
            var sequence = new EFUnitOfWorkHelper(_coreUnitOfWork).GetNextSequence<InventoryTransaction>(t => t.DateCreated == date, t => t.Sequence);
            var setDestinationLot = quantity < 0 && parameters.DestinationLotKey != null;

            var transaction = _coreUnitOfWork.InventoryTransactionsRepository.Add(new InventoryTransaction
                {
                    DateCreated = date,
                    Sequence = sequence,
                    EmployeeId = parameters.EmployeeKey.EmployeeKey_Id,
                    TimeStamp = parameters.TimeStamp,
                    TransactionType = parameters.TransactionType,
                    Description = parameters.Description,
                    SourceReference = parameters.SourceReference,

                    SourceLotDateCreated = inventoryKey.LotKey_DateCreated,
                    SourceLotDateSequence = inventoryKey.LotKey_DateSequence,
                    SourceLotTypeId = inventoryKey.LotKey_LotTypeId,
                    PackagingProductId = inventoryKey.PackagingProductKey_ProductId,
                    LocationId = inventoryKey.LocationKey_Id,
                    TreatmentId = inventoryKey.InventoryTreatmentKey_Id,
                    ToteKey = inventoryKey.InventoryKey_ToteKey,

                    DestinationLotDateCreated = setDestinationLot ? parameters.DestinationLotKey.LotKey_DateCreated : (DateTime?)null,
                    DestinationLotDateSequence = setDestinationLot ? parameters.DestinationLotKey.LotKey_DateSequence : (int?)null,
                    DestinationLotTypeId = setDestinationLot ? parameters.DestinationLotKey.LotKey_LotTypeId : (int?)null,

                    Quantity = quantity
                });

            return new SuccessResult<InventoryTransaction>(transaction);
        }

        internal IResult LogPickedInventory(InventoryTransactionParameters parameters, IEnumerable<PickedInventoryItem> items)
        {
            foreach(var item in items)
            {
                var createResult = Create(parameters, new InventoryKey(item, item, new LocationKey(item.FromLocationId), item, item.ToteKey), -item.Quantity);
                if(!createResult.Success)
                {
                    return createResult;
                }
            }

            return new SuccessResult();
        }
    }
}