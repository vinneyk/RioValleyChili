using System;
using System.Linq;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands
{
    internal class DeleteInventoryPickOrderItems
    {
        private readonly IInventoryPickOrderUnitOfWork _pickOrderUnitOfWork;

        public DeleteInventoryPickOrderItems(IInventoryPickOrderUnitOfWork pickOrderUnitOfWork)
        {
            if(pickOrderUnitOfWork == null) { throw new ArgumentNullException("pickOrderUnitOfWork"); }
            _pickOrderUnitOfWork = pickOrderUnitOfWork;
        }

        public IResult Execute(InventoryPickOrder pickOrder)
        {
            foreach(var item in pickOrder.Items.ToList())
            {
                _pickOrderUnitOfWork.InventoryPickOrderItemRepository.Remove(item);
            }

            return new SuccessResult();
        }
    }
}