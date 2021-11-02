using System;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core
{
    public interface IResolver
    {
        TReturn Get<TReturn>();
        TReturn TryGet<TReturn>();
        void Register<TContract>();
        void Register<TContract>(TContract implementation) where TContract : class;
        void Register(Type implementationType);
    }
}