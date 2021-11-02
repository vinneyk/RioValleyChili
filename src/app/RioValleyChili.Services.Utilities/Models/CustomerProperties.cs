using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerProperties : ICustomerCompanyReturn
    {
        public ICompanyHeaderReturn Broker { get; internal set; }
        public IEnumerable<ICustomerCompanyNoteReturn> CustomerNotes { get; internal set; }
    }
}