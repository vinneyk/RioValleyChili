using System;

namespace RioValleyChili.Core
{
    public interface IExceptionLogger
    {
        void LogException(Exception exception);
    }
}
