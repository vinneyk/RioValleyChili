using System;
using System.Data.Entity.Infrastructure;
using RioValleyChili.Core;
using RioValleyChili.Data;

namespace RioValleyChili.Services.Utilities.Helpers
{
    public static class NewContextConnectionStatus
    {
        public static IDBConnectionStatus NewContextConnectionStatusImplementation
        {
            get { return new NewContextConnectionStatusImplementation(); }
        }
    }

    internal class NewContextConnectionStatusImplementation : IDBConnectionStatus
    {
        public bool IsValid
        {
            get
            {
                bool isValid;
                using(var newContext = new RioValleyChiliDataContext())
                {
                    try
                    {
                        var objectContext = ((IObjectContextAdapter) newContext).ObjectContext;
                        objectContext.CommandTimeout = 1;
                        objectContext.Connection.Open();
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