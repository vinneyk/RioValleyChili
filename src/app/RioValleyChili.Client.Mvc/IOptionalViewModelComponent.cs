using System;

namespace RioValleyChili.Client.Mvc
{
    [Obsolete]
    public interface IOptionalViewModelComponent
    {
        bool InitializedAsEmpty { get; }
    }
}