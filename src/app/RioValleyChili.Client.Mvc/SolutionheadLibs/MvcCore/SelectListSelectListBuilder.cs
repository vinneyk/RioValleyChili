using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore
{
    /// <summary>
    /// Casts data source object to an IEnumerable<SelectListItem>. 
    /// </summary>
    [ExtractIntoSolutionheadLibrary]
    public class SelectListSelectListBuilder : ISelectListBuilder
    {
        public IEnumerable<SelectListItem> BuildSelectListItemCollection(IEnumerable source)
        {
            return source.Cast<SelectListItem>();
        }
    }
}