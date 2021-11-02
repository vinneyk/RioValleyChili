// ReSharper disable RedundantExtendsListEntry

using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{

    public interface INotebookUnitOfWork : IUnitOfWork,
        ICoreUnitOfWork
    {
        IRepository<Notebook> NotebookRepository { get; }

        IRepository<Note> NoteRepository { get; }
    }
}

// ReSharper restore RedundantExtendsListEntry