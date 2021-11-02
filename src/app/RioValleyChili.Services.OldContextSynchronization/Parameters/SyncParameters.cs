using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Parameters
{
    public interface ISynchronizeParameters<out TSyncParameters> : IResult
    {
        TSyncParameters Parameters { get; }
    }

    public class SyncParameters<TResult, TSyncParameters> : SyncParameters<TSyncParameters>, IResult<TResult>
    {
        public SyncParameters(IResult<TResult> result, TSyncParameters parameters)
            : base(result, parameters)
        {
            ResultingObject = result.ResultingObject;
        }

        public SyncParameters(IResult result, TSyncParameters parameters) : base(result, parameters) { }

        public TResult ResultingObject { get; private set; }
    }

    public class SyncParameters<TSyncParameters> : ISynchronizeParameters<TSyncParameters>
    {
        public string Message { get { return _result.Message; } }
        public ResultState State { get { return _result.State; } }
        public bool Success { get { return _result.Success; } }
        public TSyncParameters Parameters { get; private set; }

        private readonly IResult _result;

        public SyncParameters(IResult result, TSyncParameters parameters)
        {
            _result = result;
            Parameters = parameters;
        }
    }

    public static class SyncParameters
    {
        public static SyncParameters<TResult, TSynchParameters> Using<TSynchParameters, TResult>(IResult<TResult> result, TSynchParameters parameters)
        {
            return new SyncParameters<TResult, TSynchParameters>(result, parameters);
        }

        public static SyncParameters<TSynchParameters> Using<TSynchParameters>(IResult result, TSynchParameters parameters)
        {
            return new SyncParameters<TSynchParameters>(result, parameters);
        }
    }
}