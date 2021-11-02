using System;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class CreateInventoryPickOrderItemCommand
    {
        private readonly IInventoryPickOrderUnitOfWork _inventoryPickOrderUnitOfWork;

        public CreateInventoryPickOrderItemCommand(IInventoryPickOrderUnitOfWork inventoryPickOrderUnitOfWork)
        {
            if(inventoryPickOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryPickOrderUnitOfWork"); }
            _inventoryPickOrderUnitOfWork = inventoryPickOrderUnitOfWork;
        }

        public IResult Execute(ISchedulePickOrderItemParameter item)
        {
            var newSequence = new EFUnitOfWorkHelper(_inventoryPickOrderUnitOfWork).GetNextSequence(InventoryPickOrderItemPredicates.FilterByInventoryPickOrderKey(item.PickOrderKey), i => i.ItemSequence);

            var newItem = new InventoryPickOrderItem
                {
                    DateCreated = item.PickOrderKey.InventoryPickOrderKey_DateCreated,
                    OrderSequence = item.PickOrderKey.InventoryPickOrderKey_Sequence,
                    ItemSequence = newSequence,
                    ProductId = item.ProductId,
                    PackagingProductId = item.PackagingProductId,
                    TreatmentId = item.TreatmentId ?? StaticInventoryTreatments.NoTreatment.Id,
                    Quantity = item.Quantity,
                    CustomerLotCode = item.CustomerLotCode,
                    CustomerProductCode = item.CustomerProductCode,
                    CustomerId = item.CustomerKey != null ? item.CustomerKey.CustomerKey_Id : (int?)null
                };

            _inventoryPickOrderUnitOfWork.InventoryPickOrderItemRepository.Add(newItem);

            return new SuccessResult();
        }
    }
}