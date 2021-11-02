using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Security;

namespace RioValleyChili.Client.Mvc.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.Sales)]
    public partial class CustomersController : Controller
    {
        // GET: Customers
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult SalesOrders()
        {
            return View("SalesOrders");
        }

        public virtual ActionResult Contracts(string contractKey = null)
        {
            return View();
        }
        public virtual ActionResult CompanyMaintenance()
        {
            return View();
        }
        public virtual ActionResult Quotes()
        {
            return View();
        }
    }
}