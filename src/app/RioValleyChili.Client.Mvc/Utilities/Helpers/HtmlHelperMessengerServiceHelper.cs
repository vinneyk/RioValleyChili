using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Messaging;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public sealed class HtmlHelperMessengerServiceHelper : TempDataMessengerService
    {
        public HtmlHelperMessengerServiceHelper(HtmlHelper htmlHelper)
            : base(htmlHelper.ViewContext.TempData) { }
    }
}