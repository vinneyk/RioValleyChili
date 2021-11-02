using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface ICustomerNotesReturn : ICompanyHeaderReturn
    {
        IEnumerable<ICustomerNoteReturn> CustomerNotes { get; }
    }
}