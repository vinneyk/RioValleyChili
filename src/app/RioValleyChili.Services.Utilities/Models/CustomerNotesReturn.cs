using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerNotesReturn : CompanyHeaderReturn, ICustomerNotesReturn
    {
        public IEnumerable<ICustomerNoteReturn> CustomerNotes { get; internal set; }
    }
}