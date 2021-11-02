using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Attributes
{
    public interface IClientBoundProperty
    {
        //IEnumerable<KeyValuePair<string, object>> GetDatabindingAttributes();
        DataBindingAttributeDictionary GetDatabindingAttributes();
    }
}