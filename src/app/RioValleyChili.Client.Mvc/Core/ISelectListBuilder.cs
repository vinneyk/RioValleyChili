using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core
{
    [ExtractIntoSolutionheadLibrary]
    public interface ISelectListBuilder
    {
        IEnumerable<SelectListItem> BuildSelectListItemCollection(IEnumerable source);
    }

    [ExtractIntoSolutionheadLibrary]
    public interface ISelectListBuilder<in TSource>
    {
        IEnumerable<SelectListItem> BuildSelectListItemCollection(IEnumerable<TSource> source);
    }
}