// ReSharper disable RedundantExtendsListEntry

using System;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class CustomerCompanyNoteReturn : ICustomerCompanyNoteReturn
    {
        public string NoteKey { get { return NoteKeyReturn.CustomerNoteKey; } }
        public bool DisplayBold { get; internal set; }
        public string NoteType { get; internal set; }
        public string Text { get; internal set; }
        public DateTime TimeStamp { get; internal set; }
        public IUserSummaryReturn CreatedByUser { get; internal set; }

        internal CustomerNoteKeyReturn NoteKeyReturn { get; set; }
    }
}

// ReSharper restore RedundantExtendsListEntry