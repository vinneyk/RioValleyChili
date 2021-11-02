using System.Web.Mvc;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class ControllerHelpers
    {
        public static ActionResult RedirectWithHash(this Controller controller, ActionResult result, string id)
        {
            //when kill switch is engaged, the Redirect will throw an exception "Child actions are not allowed to perform redirect actions."
            return KillSwitch.IsEngaged 
                ? result 
                : new  RedirectResult(string.Format("{0}#{1}", controller.Url.Action(result), id));
        }
    }
}