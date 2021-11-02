// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface ISampleOrderUnitOfWork : IUnitOfWork,
        ICompanyUnitOfWork, ILotUnitOfWork, IProductUnitOfWork
    {
        IRepository<SampleOrder> SampleOrderRepository { get; }
        IRepository<SampleOrderItem> SampleOrderItemRepository { get; }
        IRepository<SampleOrderItemMatch> SampleOrderItemMatchRepository { get; }
        IRepository<SampleOrderItemSpec> SampleOrderItemSpecRepository { get; }
        IRepository<SampleOrderJournalEntry> SampleOrderJournalEntryRepository { get; }
    }
}
// ReSharper restore RedundantExtendsListEntry