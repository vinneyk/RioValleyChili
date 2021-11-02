using System.Collections.Generic;

namespace RioValleyChili.Client.Reporting.Models
{
    public interface ICustomerNotesContainer
    {
        IEnumerable<CustomerNotesReturn> CustomerNotes { get; }
    }
}