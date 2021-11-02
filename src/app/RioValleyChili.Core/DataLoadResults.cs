using System;

namespace RioValleyChili.Core
{
    public static class DataLoadResults
    {
        public static IDataLoadResultObtainer Instance { private get; set; }

        public static IDataLoadResult GetLastDataLoadResult()
        {
            return Instance.GetDataLoadResult();
        }
    }

    public interface IDataLoadResultObtainer
    {
        IDataLoadResult GetDataLoadResult();
        IDataLoadResult SetDataLoadResult(IDataLoadResult value);
    }

    public interface IDataLoadResult
    {
        bool Success { get; }
        bool RanToCompletion { get; }
        DateTime TimeStamp { get; }
    }
}