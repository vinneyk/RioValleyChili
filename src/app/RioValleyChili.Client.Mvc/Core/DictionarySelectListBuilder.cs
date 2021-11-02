using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core
{
    [ExtractIntoSolutionheadLibrary]
    public class DictionarySelectListBuilder : ISelectListBuilder
    {
        public IEnumerable<SelectListItem> BuildSelectListItemCollection(IEnumerable source)
        {
            var dictionary = source as IDictionary;
            if (dictionary == null) { throw new ArgumentException(string.Format("The source object could not be cast as a IDictionary. The object was of type '{0}'.", source.GetType().FullName)); }

            return (from dynamic d in dictionary
                    select new SelectListItem
                               {
                                   Text = d.Value,
                                   Value = d.Key,
                               });
        }
    }
}