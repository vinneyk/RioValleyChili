using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Core.Filters;

namespace RioValleyChili.Tests.TestObjects.Models
{
    internal class SelectListAttributeTestObject_SingleSelectListProperty
    {
        public SelectListAttributeTestObject_SingleSelectListProperty()
        {
            Prop1 = "1";
            Prop2 = "Property 2";
            SelectListItems = new List<SelectListItemObject>
                                  {
                                      new SelectListItemObject
                                          {
                                              Id = 1,
                                              Text = "First"
                                          },
                                      new SelectListItemObject
                                          {
                                              Id = 2,
                                              Text = "Second"
                                          }
                                  };
        }

        [SelectListSource("SelectListItems", "Id", "Text")]
        public string Prop1 { get; set; }

        public string Prop2 { get; set; }

        public IEnumerable<SelectListItemObject> SelectListItems { get; set; }
    }
}