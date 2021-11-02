using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Extensions
{
    public static class ProductionLineExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<KeyValuePair<int, string>> options)
        {
            return options.Select(l => new SelectListItem
                                           {
                                               Text = l.Value,
                                               Value = l.Key.ToString()
                                           });
        }
    }
}