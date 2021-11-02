using System.Collections.Generic;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SalesOrderInternalAcknowledgementReturn : ISalesOrderInternalAcknowledgementReturn
    {
        public string PaymentTerms { get; set; }
        public string Broker { get; set; }
        public ShippingLabel SoldToShippingLabel { get; set; }
        public IEnumerable<ICustomerNoteReturn> CustomerNotes { get; set; }
        public IEnumerable<ISalesOrderItemInternalAcknowledgement> OrderItems { get; set; }
    }
}