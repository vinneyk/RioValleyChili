using System;

namespace RioValleyChili.Services.OldContextSynchronization.Interfaces
{
    public interface ISynchronizationCommand<in TInput>
    {
        void Synchronize(Func<TInput> getInput);
    }
}