using RioValleyChili.Core;

namespace RioValleyChili.Services.Utilities.OldContextSynchronization
{
    public static class OldContextConnectionStatus
    {
        public static IDBConnectionStatus OldContextConnectionStatusImplementation
        {
            get { return Services.OldContextSynchronization.Utilities.OldContextConnectionStatus.OldContextConnectionStatusImplementation; }
        }
    }
}