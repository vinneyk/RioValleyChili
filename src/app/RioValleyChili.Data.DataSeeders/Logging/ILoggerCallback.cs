using System;

namespace RioValleyChili.Data.DataSeeders.Logging
{
    public interface ILoggerCallback<in TCallbackParameters>
    {
        Action<TCallbackParameters> Callback { get; }
    }
}