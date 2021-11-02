using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Models;
using RioValleyChili.Client.Mvc.Models.Home;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Controllers
{
    public partial class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        
        [BypassKillSwitchFilter]
        public virtual ActionResult SiteStatus()
        {
            var desiresJson = ControllerContext.RequestContext.HttpContext.Request.AcceptTypes != null && ControllerContext.RequestContext.HttpContext.Request.AcceptTypes.Contains("application/json");
            if (desiresJson)
            {
                return Json(new
                {
                    isRunning = !KillSwitch.IsEngaged,
                    timeOfDeath = KillSwitch.EngagedTimeStamp
                }, JsonRequestBehavior.AllowGet);
            }

            var model = new SiteStatusModel
            {
                KillswitchEngaged = KillSwitch.IsEngaged,
                KillswitchEngagedTimeStamp = KillSwitch.EngagedTimeStamp,
            };

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => {
                var dataLoadResult = DataLoadResults.GetLastDataLoadResult();
                model.LastDataMigrationTimeStamp = dataLoadResult.TimeStamp;
            }));
            tasks.Add(Task.Factory.StartNew(() => {
                model.RvcDataDatabaseConnected = DBConnectionStatus.NewContextIsValid;
            }));
            tasks.Add(Task.Factory.StartNew(() => {
                model.RioAccessSqlDatabaseConnected = DBConnectionStatus.OldContextIsValid;
            }));

            Task.WaitAll(tasks.ToArray());
            
            return View(model);
        }

        [ClaimsAuthorize(ClaimActions.Execute, ClaimTypes.Admin.KillswitchDisengage)]
        public virtual ActionResult Startup()
        {
            KillSwitch.Disengage();
            return RedirectToAction(MVC.Home.SiteStatus());
        }

        public virtual ActionResult ShutDown()
        {
            return View(new ShutDownModel());
        }

        [HttpPost]
        public virtual ActionResult ShutDown(ShutDownModel values)
        {
            KillSwitch.Engage();
            return RedirectToAction(MVC.Home.SiteStatus());
        }

        [AllowAnonymous,
        OutputCache(VaryByCustom = CacheSettings.VaryByUser, Duration = CacheSettings.NavigationMenuOutputCacheDuration)]
        public virtual PartialViewResult MainNavigation()
        {
            return PartialView(MVC.Shared.Views._MainNavigation);
        }
    }
}
