using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SyncPostInvoice)]
    public class SyncPostInvoice : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, SalesOrderKey>
    {
        public SyncPostInvoice(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SalesOrderKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var orderKey = getInput();
            var order = UnitOfWork.SalesOrderRepository.FindByKey(orderKey, o => o.InventoryShipmentOrder);
            var tblOrder = OldContext.tblOrders
                .FirstOrDefault(o => o.OrderNum == order.InventoryShipmentOrder.MoveNum);
            if(tblOrder == null)
            {
                throw new Exception(string.Format("Could not find tblOrder[{0}]", order.InventoryShipmentOrder.MoveNum));
            }

            tblOrder.Status = (int?) tblOrderStatus.Invoiced;
            tblOrder.InvoiceDate = order.InvoiceDate;
            tblOrder.InvoiceNotes = order.InvoiceNotes;

            OldContext.SaveChanges();

            Console.WriteLine(ConsoleOutput.InvoicedOrder, tblOrder.OrderNum);
        }
    }
}