using System;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.LinqProjectors;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CustomerOrderInvoiceConductor
    {
        private readonly IInventoryShipmentOrderUnitOfWork _inventoryShipmentOrderUnitOfWork;

        internal CustomerOrderInvoiceConductor(IInventoryShipmentOrderUnitOfWork inventoryShipmentOrderUnitOfWork)
        {
            if(inventoryShipmentOrderUnitOfWork == null) { throw new ArgumentNullException("inventoryShipmentOrderUnitOfWork"); }
            _inventoryShipmentOrderUnitOfWork = inventoryShipmentOrderUnitOfWork;
        }

        internal IResult<ISalesOrderInvoice> Get(ISalesOrderKey orderKey)
        {
            var customerOrderKey = orderKey.ToSalesOrderKey();
            var predicate = customerOrderKey.FindByPredicate;
            var select = SalesOrderProjectors.SelectCustomerOrderInvoice();

            var invoice = _inventoryShipmentOrderUnitOfWork.SalesOrderRepository
                .Filter(predicate)
                .AsExpandable()
                .Select(select)
                .FirstOrDefault();
            if(invoice == null)
            {
                return new InvalidResult<ISalesOrderInvoice>(null, string.Format(UserMessages.SalesOrderNotFound, customerOrderKey));
            }

            invoice.Initialize();

            return new SuccessResult<ISalesOrderInvoice>(invoice);
        }
    }
}