using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Filters;

namespace RioValleyChili.Client.Mvc.Controllers
{
    [BypassKillSwitchFilter, AllowAnonymous]
    public partial class UITestsController : Controller
    {
        //
        // GET: /UITests/
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult LotsTests()
        {
            return View();
        }

        public virtual ActionResult LotInventoryTests()
        {
            return View();
        }

        //
        // GET: /UITests/LabResultsTests/
        public virtual ActionResult LabResultsTests()
        {
            return View();
        }

        public virtual ActionResult LotHoldTests()
        {
            return View();
        }

        public virtual ActionResult PackSheduleTests()
        {
            return View();
        }

        public virtual ActionResult ProductionBatchTests()
        {
            return View();
        }
	}
}