using System;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Services.OldContextSynchronization.Interfaces;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using Solutionhead.Data;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    public abstract class SyncCommandBase<TUnitOfWork, TInput> : ISynchronizationCommand<TInput>
        where TUnitOfWork : IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        protected RioAccessSQLEntities OldContext { get { return OldContextHelper.OldContext; } }
        protected readonly OldContextHelper OldContextHelper;

        protected SyncCommandBase(TUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            OldContextHelper = new OldContextHelper(new RioAccessSQLEntities());
        }

        public abstract void Synchronize(Func<TInput> getInput);
    }
}