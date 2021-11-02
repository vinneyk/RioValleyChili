using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Moq;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Attributes;

namespace RioValleyChili.Tests.Helpers
{
    public static class BindSelectListAttributeTestHelper
    {
        public static ControllerBase ExecuteOnResultExecutingTest<TViewModel>(this BindSelectListsAttribute attribute, TViewModel viewModel)
        {
            var viewData = new ViewDataDictionary(viewModel);

            var mockController = new Mock<ControllerBase>();
            mockController.Object.ViewData = viewData;

            var mockFilterContext = new Mock<ResultExecutingContext>();
            mockFilterContext.Setup(m => m.Controller)
                             .Returns(mockController.Object);
            
            attribute.OnResultExecuting(mockFilterContext.Object);

            return mockController.Object;
        }

        public static IEnumerable<SelectListSourceAttribute> GetSelectListAttributes<TObject>(this TObject me)
        {
            return me.GetType().GetProperties()
                     .Where(p => p.IsDefined(typeof (SelectListSourceAttribute), false))
                     .SelectMany(p => p.GetCustomAttributes(typeof (SelectListSourceAttribute), false))
                     .Cast<SelectListSourceAttribute>();
        }
    }
}