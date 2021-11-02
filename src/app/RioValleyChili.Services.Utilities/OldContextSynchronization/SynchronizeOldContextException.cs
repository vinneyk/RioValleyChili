using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PostSharp.Aspects;

namespace RioValleyChili.Services.Utilities.OldContextSynchronization
{
    [Serializable]
    public class SynchronizeOldContextException : ApplicationException
    {
        public override string Message { get { return _message; } }
        private readonly string _message;

        public SynchronizeOldContextException(MethodExecutionArgs methodExecutionArgs, Exception innerException) : base("SynchronizeOldContext exception.", innerException)
        {
            var methodArgsString = JsonConvert.SerializeObject(methodExecutionArgs, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var innerExceptionString = JsonConvert.SerializeObject(innerException, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            _message = string.Format("SynchronizeOldContext failed - contexts are now out of synchronization.\nBaseMessage = \"{0}\"\n\nMethodExecutionArgs\n{1}\n\nInnerException\n{2}", base.Message, methodArgsString, innerExceptionString);
        }

        public SynchronizeOldContextException() { }

        public SynchronizeOldContextException(string message) : base(message) { }

        public SynchronizeOldContextException(string message, Exception innerException) : base(message, innerException) { } 

        public SynchronizeOldContextException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}