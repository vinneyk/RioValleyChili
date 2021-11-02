using RioValleyChili.Core;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.OldContextSynchronization;

namespace RioValleyChili.Services
{
    public static class DBConnectionStatusImplementation
    {
        public static void RegisterImplementations()
        {
            DBConnectionStatus.RegisterOldContextStatus(OldContextConnectionStatus.OldContextConnectionStatusImplementation);
            DBConnectionStatus.RegisterNewContextStatus(NewContextConnectionStatus.NewContextConnectionStatusImplementation);
        }
    }
}