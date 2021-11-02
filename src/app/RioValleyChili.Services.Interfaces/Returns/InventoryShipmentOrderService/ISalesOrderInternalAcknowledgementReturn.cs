using System.Collections.Generic;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface ISalesOrderInternalAcknowledgementReturn
    {
        string PaymentTerms { get; }
        string Broker { get; }
        ShippingLabel SoldToShippingLabel { get; }
        IEnumerable<ICustomerNoteReturn> CustomerNotes { get; }
        IEnumerable<ISalesOrderItemInternalAcknowledgement> OrderItems { get; }
    }
}