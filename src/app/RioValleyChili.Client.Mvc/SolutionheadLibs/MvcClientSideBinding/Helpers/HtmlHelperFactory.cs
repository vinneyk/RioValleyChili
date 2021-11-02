using System.Linq;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers
{
    public static class HtmlHelperFactory
    {
        public static HtmlHelper<TModel> BuildHtmlHelper<TModel>(ViewContext viewContext, TModel model)
        {
            var viewDataDictionary = new ViewDataDictionary<TModel>(model);

            viewContext.ViewData.Where(v => !viewDataDictionary.ContainsKey(v.Key)).ToList()
                       .ForEach(viewDataDictionary.Add);

            var viewDataContainer = new ViewDataContainer(viewDataDictionary);
            return new HtmlHelper<TModel>(viewContext, viewDataContainer);
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }

            public ViewDataDictionary ViewData { get; set; }
        }
    }
}