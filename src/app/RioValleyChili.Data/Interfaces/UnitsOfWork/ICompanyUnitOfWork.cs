// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface ICompanyUnitOfWork : IUnitOfWork,
        INotebookUnitOfWork
    {
        IRepository<Company> CompanyRepository { get; }
        IRepository<CompanyTypeRecord> CompanyTypeRecords { get; }
        IRepository<Contact> ContactRepository { get; }
        IRepository<ContactAddress> ContactAddressRepository { get; }
        IRepository<Customer> CustomerRepository { get; }
        IRepository<CustomerNote> CustomerNoteRepository { get; }
        IRepository<CustomerProductAttributeRange> CustomerProductAttributeRangeRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry