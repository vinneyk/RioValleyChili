using PostSharp.Aspects;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;

namespace RioValleyChili.Services.OldContextSynchronization.Interfaces
{
    public interface ISynchronizationCommandFactory
    {
        ISynchronizationCommand<TInput> GetCommand<TInput>(NewContextMethod method, MethodExecutionArgs args);
    }
}