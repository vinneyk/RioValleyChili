using System;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SyncAttribute : Attribute
    {
        public readonly NewContextMethod NewContextMethod;

        public SyncAttribute(NewContextMethod method)
        {
            NewContextMethod = method;
        }
    }
}