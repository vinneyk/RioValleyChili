using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Helpers
{
    public static class EnumHelper
    {
        [ExtractIntoSolutionheadLibrary]
        public static IEnumerable<SelectListItem> BuildSelectListItems<TEnum>(TEnum selectedValue = default(TEnum), bool numericValue = true)
        {
            return Enum.GetValues(typeof (TEnum)).Cast<object>()
                .Select(e => new SelectListItem()
                                 {
                                     Selected = selectedValue.Equals(e),
                                     Text = e.ToString(),
                                     Value = numericValue ? Convert.ToInt32(e).ToString(CultureInfo.InvariantCulture) : e.ToString(),
                                 }).ToList();
        }
    }
}