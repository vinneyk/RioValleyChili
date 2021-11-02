using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Interfaces.Returns.CompanyService
{
    public interface ICompanyDetailReturn : ICompanySummaryReturn
    {
        ICustomerCompanyReturn Customer { get;  }
    }

    public interface ICustomerCompanyReturn
    {
        ICompanyHeaderReturn Broker { get; }
        IEnumerable<ICustomerCompanyNoteReturn> CustomerNotes { get; }
    }

    public interface ICustomerCompanyNoteReturn
    {
        string NoteKey { get; }
        bool DisplayBold { get; }
        string NoteType { get; }
        string Text { get; }
        DateTime TimeStamp { get; }

        IUserSummaryReturn CreatedByUser { get; }
    }
}