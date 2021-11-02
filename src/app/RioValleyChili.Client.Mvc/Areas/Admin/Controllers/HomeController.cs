using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Areas.Admin.Controllers
{
    public partial class HomeController : Controller
    {
        //
        // GET: /Admin/Home/

        public virtual ActionResult Index()
        {
            return View();
        }

    }
}
