using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi
{
    [ExtractIntoSolutionheadLibrary]
    public struct Link 
    {
        public string HRef { get; set; }
        public bool Templated { get; set; }
    }
}