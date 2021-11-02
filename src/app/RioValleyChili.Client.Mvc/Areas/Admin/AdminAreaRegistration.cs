using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        private string[] NameSpaces
        {
            get { return new[] { "RioValleyChili.Client.Mvc.Areas.Admin.Controllers" }; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new
                {
                    area = AreaName,
                    controller = MVC.Admin.Home.Name,
                    action = MVC.Admin.Home.ActionNames.Index,
                    id = UrlParameter.Optional,
                },
                NameSpaces
            );
        }
    }
}
