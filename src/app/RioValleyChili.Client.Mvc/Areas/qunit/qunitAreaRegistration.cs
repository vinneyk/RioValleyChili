using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Areas.qunit
{
    public class qunitAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "qunit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "qunit_default",
                "qunit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
