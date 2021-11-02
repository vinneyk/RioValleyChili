using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi
{
    [ExtractIntoSolutionheadLibrary]
    public interface ILinkedResource<out TData> : ILinkedResource
    {
        TData Data { get; }
    }

    [ExtractIntoSolutionheadLibrary]
    public interface ILinkedResource
    {
        ResourceLinkCollection Links { get; }
        string HRef { get; }
    }
}