using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Controllers
{
    [AllowAnonymous]
    public partial class BladeController : Controller
    {
        // GET: Blade
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}