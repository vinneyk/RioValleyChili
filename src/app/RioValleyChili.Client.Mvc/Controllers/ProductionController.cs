using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.Production)]
    public partial class ProductionController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        #region Mill & Wetdown

        //
        // GET: Production
        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.MillAndWetdown)]
        public virtual ActionResult MillAndWetdown()
        {
            return View();
        }

        // 
        // GET:
        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.MillAndWetdown)]
        public virtual ActionResult CreateMillAndWetdown()
        {
            if (KillSwitch.IsEngaged) return MillAndWetdown();
            return Redirect(string.Format("{0}#new", Url.Action("MillAndWetdown")));
        }

        //
        // GET: /Production/MillAndWetdown/02 2013 280 07
        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.MillAndWetdown)]
        public virtual ActionResult MillAndWetdownDetails(string id)
        {
            return this.RedirectWithHash(MillAndWetdown(), id);
        }

        #endregion

        #region Pack Schedules
        
        //
        // GET: /Production/PackSchedules/
        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.PackSchedules)]
        public virtual ActionResult PackSchedules()
        {
            return View();
        }

        //
        // GET: /Production/PackSchedules/2012042-001

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.PackSchedules)]
        public virtual ActionResult PackScheduleDetails(string id)
        {
            return this.RedirectWithHash(PackSchedules(), id);
        }

        #endregion

        #region Production Results
        
        //
        // GET: /Production/Results/

        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.ProductionResults)]
        [Route("Results", Name = "ProductionResults")]
        public virtual ActionResult ProductionResults()
        {
            return View();
        }

        //
        // GET: /Production/Results/03 14 001 01

        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.ProductionClaimTypes.ProductionResults)]
        [Route("Results/{lot}", Name = "ProductionResultDetails")]
        public virtual ActionResult ProducionResultDetails(string lot)
        {
            if (KillSwitch.IsEngaged) return ProductionResults();
            return this.RedirectWithHash(ProductionResults(), lot);
        }

        // GET: /Production/ProductionScheduling/
        public virtual ActionResult ProductionSchedules()
        {
            return View();
        }

        #endregion
    }
}