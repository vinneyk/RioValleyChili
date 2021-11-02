using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Filters;

namespace RioValleyChili.Client.Mvc.Areas.qunit.Controllers
{
    [AllowAnonymous, BypassKillSwitchFilter]
    public partial class CustomObservableTestsController : Controller
    {
        //
        // GET: /qunit/CustomObservableTests/ObservableDateTime
        public virtual ActionResult ObservableDateTime()
        {
            return View();
        }

        public virtual ActionResult ObservableDate()
        {
            return View();
        }

        public virtual ActionResult ObservableTime()
        {
            return View();
        }
	}
}