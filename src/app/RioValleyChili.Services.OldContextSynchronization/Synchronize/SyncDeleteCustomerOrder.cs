using System;
using System.Linq;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.DeleteSalesOrder)]
    public class SyncDeleteCustomerOrder : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, int?>
    {
        public SyncDeleteCustomerOrder(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<int?> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var orderNum = getInput();
            var order = OldContext.tblOrders
                .Include(o => o.tblOrderDetails.Select(d => d.tblStagedFGs))
                .FirstOrDefault(o => o.OrderNum == orderNum);
            if(order == null)
            {
                throw new Exception(string.Format("Could not find customer order with key[{0}].", orderNum));
            }

            foreach(var detail in order.tblOrderDetails.ToList())
            {
                foreach(var staged in detail.tblStagedFGs.ToList())
                {
                    OldContext.tblStagedFGs.DeleteObject(staged);
                }

                OldContext.tblOrderDetails.DeleteObject(detail);
            }

            OldContext.tblOrders.DeleteObject(order);

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.RemovedTblOrder, orderNum);
        }
    }
}