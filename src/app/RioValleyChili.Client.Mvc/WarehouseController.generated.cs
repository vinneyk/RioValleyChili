// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace RioValleyChili.Client.Mvc.Controllers
{
    public partial class WarehouseController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public WarehouseController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected WarehouseController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }


        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public WarehouseController Actions { get { return MVC.Warehouse; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Warehouse";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Warehouse";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string Shipments = "Shipments";
            public readonly string InterWarehouseMovements = "InterWarehouseMovements";
            public readonly string TreatmentOrders = "TreatmentOrders";
            public readonly string WarehouseLocations = "WarehouseLocations";
            public readonly string FacilityMaintenance = "FacilityMaintenance";
            public readonly string RinconMovements = "RinconMovements";
            public readonly string Inventory = "Inventory";
            public readonly string Receiving = "Receiving";
            public readonly string DehydratedMaterialReceiving = "DehydratedMaterialReceiving";
            public readonly string LotHistory = "LotHistory";
            public readonly string InventoryAdjustments = "InventoryAdjustments";
            public readonly string ReceiveChileProduct = "ReceiveChileProduct";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string Shipments = "Shipments";
            public const string InterWarehouseMovements = "InterWarehouseMovements";
            public const string TreatmentOrders = "TreatmentOrders";
            public const string WarehouseLocations = "WarehouseLocations";
            public const string FacilityMaintenance = "FacilityMaintenance";
            public const string RinconMovements = "RinconMovements";
            public const string Inventory = "Inventory";
            public const string Receiving = "Receiving";
            public const string DehydratedMaterialReceiving = "DehydratedMaterialReceiving";
            public const string LotHistory = "LotHistory";
            public const string InventoryAdjustments = "InventoryAdjustments";
            public const string ReceiveChileProduct = "ReceiveChileProduct";
        }


        static readonly ActionParamsClass_FacilityMaintenance s_params_FacilityMaintenance = new ActionParamsClass_FacilityMaintenance();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_FacilityMaintenance FacilityMaintenanceParams { get { return s_params_FacilityMaintenance; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_FacilityMaintenance
        {
            public readonly string id = "id";
            public readonly string mode = "mode";
        }
        static readonly ActionParamsClass_RinconMovements s_params_RinconMovements = new ActionParamsClass_RinconMovements();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_RinconMovements RinconMovementsParams { get { return s_params_RinconMovements; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_RinconMovements
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Inventory s_params_Inventory = new ActionParamsClass_Inventory();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Inventory InventoryParams { get { return s_params_Inventory; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Inventory
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Receiving s_params_Receiving = new ActionParamsClass_Receiving();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Receiving ReceivingParams { get { return s_params_Receiving; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Receiving
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_DehydratedMaterialReceiving s_params_DehydratedMaterialReceiving = new ActionParamsClass_DehydratedMaterialReceiving();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_DehydratedMaterialReceiving DehydratedMaterialReceivingParams { get { return s_params_DehydratedMaterialReceiving; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_DehydratedMaterialReceiving
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_InventoryAdjustments s_params_InventoryAdjustments = new ActionParamsClass_InventoryAdjustments();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_InventoryAdjustments InventoryAdjustmentsParams { get { return s_params_InventoryAdjustments; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_InventoryAdjustments
        {
            public readonly string id = "id";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string DehydratedMaterialReceiving = "DehydratedMaterialReceiving";
                public readonly string FacilityMaintenance = "FacilityMaintenance";
                public readonly string Index = "Index";
                public readonly string InterWarehouseMovements = "InterWarehouseMovements";
                public readonly string Inventory = "Inventory";
                public readonly string InventoryAdjustments = "InventoryAdjustments";
                public readonly string LotHistory = "LotHistory";
                public readonly string ReceiveChileProduct = "ReceiveChileProduct";
                public readonly string Receiving = "Receiving";
                public readonly string RinconMovements = "RinconMovements";
                public readonly string Shipments = "Shipments";
                public readonly string TreatmentOrders = "TreatmentOrders";
                public readonly string WarehouseLocations = "WarehouseLocations";
            }
            public readonly string DehydratedMaterialReceiving = "~/Views/Warehouse/DehydratedMaterialReceiving.cshtml";
            public readonly string FacilityMaintenance = "~/Views/Warehouse/FacilityMaintenance.cshtml";
            public readonly string Index = "~/Views/Warehouse/Index.cshtml";
            public readonly string InterWarehouseMovements = "~/Views/Warehouse/InterWarehouseMovements.cshtml";
            public readonly string Inventory = "~/Views/Warehouse/Inventory.cshtml";
            public readonly string InventoryAdjustments = "~/Views/Warehouse/InventoryAdjustments.cshtml";
            public readonly string LotHistory = "~/Views/Warehouse/LotHistory.cshtml";
            public readonly string ReceiveChileProduct = "~/Views/Warehouse/ReceiveChileProduct.cshtml";
            public readonly string Receiving = "~/Views/Warehouse/Receiving.cshtml";
            public readonly string RinconMovements = "~/Views/Warehouse/RinconMovements.cshtml";
            public readonly string Shipments = "~/Views/Warehouse/Shipments.cshtml";
            public readonly string TreatmentOrders = "~/Views/Warehouse/TreatmentOrders.cshtml";
            public readonly string WarehouseLocations = "~/Views/Warehouse/WarehouseLocations.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_WarehouseController : RioValleyChili.Client.Mvc.Controllers.WarehouseController
    {
        public T4MVC_WarehouseController() : base(Dummy.Instance) { }

        [NonAction]
        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void ShipmentsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Shipments()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Shipments);
            ShipmentsOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void InterWarehouseMovementsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult InterWarehouseMovements()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.InterWarehouseMovements);
            InterWarehouseMovementsOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void TreatmentOrdersOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult TreatmentOrders()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TreatmentOrders);
            TreatmentOrdersOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void WarehouseLocationsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult WarehouseLocations()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.WarehouseLocations);
            WarehouseLocationsOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void FacilityMaintenanceOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string id, string mode);

        [NonAction]
        public override System.Web.Mvc.ActionResult FacilityMaintenance(string id, string mode)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.FacilityMaintenance);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "mode", mode);
            FacilityMaintenanceOverride(callInfo, id, mode);
            return callInfo;
        }

        [NonAction]
        partial void RinconMovementsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string id);

        [NonAction]
        public override System.Web.Mvc.ActionResult RinconMovements(string id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.RinconMovements);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            RinconMovementsOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void InventoryOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string id);

        [NonAction]
        public override System.Web.Mvc.ActionResult Inventory(string id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Inventory);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            InventoryOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void ReceivingOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string id);

        [NonAction]
        public override System.Web.Mvc.ActionResult Receiving(string id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Receiving);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            ReceivingOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void DehydratedMaterialReceivingOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string id);

        [NonAction]
        public override System.Web.Mvc.ActionResult DehydratedMaterialReceiving(string id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.DehydratedMaterialReceiving);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            DehydratedMaterialReceivingOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void LotHistoryOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult LotHistory()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LotHistory);
            LotHistoryOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void InventoryAdjustmentsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string id);

        [NonAction]
        public override System.Web.Mvc.ActionResult InventoryAdjustments(string id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.InventoryAdjustments);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            InventoryAdjustmentsOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void ReceiveChileProductOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult ReceiveChileProduct()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ReceiveChileProduct);
            ReceiveChileProductOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
