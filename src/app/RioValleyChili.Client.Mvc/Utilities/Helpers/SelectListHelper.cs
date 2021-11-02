using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class SelectListHelper
    {
        public static IEnumerable<SelectListItem> ToSelectList<TObject>(this IEnumerable<TObject> enumerable,
                                                                        Expression<Func<TObject, string>> textSelector, 
                                                                        Expression<Func<TObject, string>> valueSelector, 
                                                                        Expression<Func<TObject, bool>> selectedPredicate,
                                                                        string optionalLabel = null)
        {
            var valueSelectorFunc = valueSelector.Compile();
            var textSelectorFunc = textSelector.Compile();
            var selected = selectedPredicate.Compile();

            var selecteListItems = enumerable.ToList().Select(w => new SelectListItem
                                                       {
                                                           Text = textSelectorFunc.Invoke(w),
                                                           Value = valueSelectorFunc.Invoke(w),
                                                           Selected = selected.Invoke(w)
                                                       }).ToList();

            InsertLabel(selecteListItems, optionalLabel);

            return selecteListItems;
        }

        public static IEnumerable<SelectListItem> ToSelectList<TObject>(this IEnumerable<TObject> enumerable,
                                                                        Expression<Func<TObject, string>> textSelector, 
                                                                        Expression<Func<TObject, string>> valueSelector, 
                                                                        string optionalLabel = null,
                                                                        string selectedValue = null)
        {
            var valueSelectorFunc = valueSelector.Compile();
            var textSelectorFunc = textSelector.Compile();

            var selectListItems = enumerable.Select(w => new SelectListItem
                                                       {
                                                           Text = textSelectorFunc.Invoke(w),
                                                           Value = valueSelectorFunc.Invoke(w),
                                                           Selected = !string.IsNullOrWhiteSpace(selectedValue) && selectedValue.Equals(valueSelectorFunc.Invoke(w))
                                                       }).ToList();

            InsertLabel(selectListItems, optionalLabel);

            return selectListItems;
        }

        private static void InsertLabel(IList<SelectListItem> selectListItems, string labelText)
        {
            if (labelText != null)
            {
                selectListItems.Insert(0, new SelectListItem
                {
                    Text = labelText,
                    Value = "",
                });
            }
        }
    }
}