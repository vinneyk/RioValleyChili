using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Attributes;

namespace RioValleyChili.Client.Mvc.Models
{
    [BindSelectLists]
    public class ShutDownModel
    {
        public ShutDownModel()
        {
            Features = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select feature...",
                },
                new SelectListItem
                {
                    Text = "Dehydrated Materials Receiving", 
                    Value = "DehydratedMaterialRecieving",
                }
            };
        }

        [SelectListSource("Features", "Key", "Value"),
        Display(Name = "Problematic Feature", Description = "Which feature were you having trouble with?"),
        Required]
        public string ProblematicFeature { get; set; }

        [UIHint("TextArea"), Required]
        public string Comment { get; set; }

        public IEnumerable<SelectListItem> Features { get; set; }
    }
}