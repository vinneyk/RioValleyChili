using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class UpdateInventoryPickOrderItemCommand
    {
        private readonly IInventoryPickOrderUnitOfWork _inventoryPickOrderUnitOfWork;

        public UpdateInventoryPickOrderItemCommand(IInventoryPickOrderUnitOfWork inventoryPickOrderUnitOfWork)
        {
            if(inventoryPickOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryPickOrderUnitOfWork"); }
            _inventoryPickOrderUnitOfWork = inventoryPickOrderUnitOfWork;
        }

        public IResult Execute(ISchedulePickOrderItemParameter item)
        {
            var key = new InventoryPickOrderItemKey(item);
            var orderItem = _inventoryPickOrderUnitOfWork.InventoryPickOrderItemRepository.FindByKey(key);

            if(orderItem == null)
            {
                return new InvalidResult(string.Format("Could not find Inventory Pick Order Item with key '{0}'.", key.KeyValue));
            }

            orderItem.ProductId = item.ProductId;
            orderItem.PackagingProductId = item.PackagingProductId;
            orderItem.TreatmentId = item.TreatmentId ?? StaticInventoryTreatments.NoTreatment.Id;
            orderItem.Quantity = item.Quantity;
            orderItem.CustomerLotCode = item.CustomerLotCode;
            orderItem.CustomerProductCode = item.CustomerProductCode;
            orderItem.CustomerId = item.CustomerKey != null ? item.CustomerKey.CustomerKey_Id : (int?)null;

            return new SuccessResult();
        }
    }
}