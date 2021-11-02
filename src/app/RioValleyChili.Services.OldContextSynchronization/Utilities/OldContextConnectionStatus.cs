using System;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class OldContextConnectionStatus
    {
        public static IDBConnectionStatus OldContextConnectionStatusImplementation
        {
            get { return new OldContextConnectionStatusImplementation(); }
        }
    }

    internal class OldContextConnectionStatusImplementation : IDBConnectionStatus
    {
        public bool IsValid
        {
            get
            {
                bool isValid;
                using(var oldContext = new RioAccessSQLEntities())
                {
                    try
                    {
                        oldContext.CommandTimeout = 1;
                        oldContext.Connection.Open();
                        isValid = true;
                    }
                    catch(Exception ex)
                    {
                        isValid = false;
                        Exception = ex;
                    }
                }
                return isValid;
            }
        }

        public Exception Exception { get; private set; }
    }
}