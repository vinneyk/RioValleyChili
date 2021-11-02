using Solutionhead.Services;

namespace Command
{
    public interface IParser<in TInput, out TOutput>
    {
        IResult<TOutput> Parse(TInput input);
    }
}