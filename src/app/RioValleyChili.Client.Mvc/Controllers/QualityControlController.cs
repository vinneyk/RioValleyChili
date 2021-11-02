using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Security;

namespace RioValleyChili.Client.Mvc.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.QualityControl)]
    public partial class QualityControlController : Controller
    {
        // GET: QualityControl
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult LabResults()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.Full, ClaimTypes.QualityControlClaimTypes.CustomerProductSpec)]
        public virtual ActionResult CustomerSpecs(string customerKey, string productKey)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.QualityControl)]
        public virtual ActionResult ProductSpecs(string productKey = null)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.QualityControl)]
        public virtual ActionResult ProductMaintenance()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.QualityControl)]
        public virtual ActionResult SampleMatching()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.QualityControl)]
        public virtual ActionResult LotTrace()
        {
            return View();
        }
    }
}