using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Inventory
{
    internal class CreateInventoryPickOrderCommand
    {
        private readonly IInventoryPickOrderUnitOfWork _inventoryPickOrderUnitOfWork;

        public CreateInventoryPickOrderCommand(IInventoryPickOrderUnitOfWork inventoryPickOrderUnitOfWork)
        {
            if(inventoryPickOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryPickOrderUnitOfWork"); }
            _inventoryPickOrderUnitOfWork = inventoryPickOrderUnitOfWork;
        }

        public IResult<InventoryPickOrder> Execute(IPickedInventoryKey pickedInventoryKey)
        {
            if(pickedInventoryKey == null) { throw new ArgumentNullException("pickedInventoryKey"); }

            var pickOrder = new InventoryPickOrder
                {
                    DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                    Sequence = pickedInventoryKey.PickedInventoryKey_Sequence
                };

            _inventoryPickOrderUnitOfWork.InventoryPickOrderRepository.Add(pickOrder);

            return new SuccessResult().ConvertTo(pickOrder);
        }
    }
}