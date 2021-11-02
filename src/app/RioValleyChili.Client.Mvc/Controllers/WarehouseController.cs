using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Security;

namespace RioValleyChili.Client.Mvc.Controllers
{
    public partial class WarehouseController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.Shipments)]
        public virtual ActionResult Shipments()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.InterwarehouseMovements)]
        public virtual ActionResult InterWarehouseMovements()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.TreatmentOrders)]
        public virtual ActionResult TreatmentOrders()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.WarehouseLocations)]
        public virtual ActionResult WarehouseLocations()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.WarehouseLocations),
        Route("~/Warehouse/FacilityMaintenance/{id?}/{mode?}")
        ]
        public virtual ActionResult FacilityMaintenance(string id = null, string mode = null)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.IntrawarehouseMovements)]
        public virtual ActionResult RinconMovements(string id = null)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.WarehouseClaimTypes.IntrawarehouseMovements)]
        public virtual ActionResult Inventory(string id = null)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.Full, ClaimTypes.InventoryClaimTypes.ReceiveInventory)]
        public virtual ActionResult Receiving(string id = null)
        {
            return View();
        }
        
        // GET: /Inventory/Receiving/DehydratedMaterial
        // GET: /Inventory/Receiving/DehydratedMaterial/13 01 123 01
        // GET: /Inventory/Receiving/DehydratedMaterial/new
        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.InventoryClaimTypes.DehydratedMaterials)]
        public virtual ActionResult DehydratedMaterialReceiving(string id = null)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.Full, ClaimTypes.InventoryClaimTypes.LotHistory)]
        public virtual ActionResult LotHistory()
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.Full, ClaimTypes.WarehouseClaimTypes.IntrawarehouseMovements)]
        public virtual ActionResult InventoryAdjustments(string id = null)
        {
            return View();
        }

        [ClaimsAuthorize(ClaimActions.Full, ClaimTypes.InventoryClaimTypes.DehydratedMaterials)]
        public virtual ActionResult ReceiveChileProduct()
        {
            return View();
        }
    }
}
