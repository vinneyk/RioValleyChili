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
namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class ProductionReportingController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProductionReportingController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ProductionReportingController(Dummy d) { }

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

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ProductionBatchPacket()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionBatchPacket);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult PackSchedulePickSheet()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PackSchedulePickSheet);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ProductionRecap()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionRecap);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ProductionAdditives()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionAdditives);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult ProductionSchedule()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionSchedule);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ProductionReportingController Actions { get { return MVC.Reporting.ProductionReporting; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Reporting";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "ProductionReporting";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "ProductionReporting";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string ProductionBatchPacket = "ProductionBatchPacket";
            public readonly string PackSchedulePickSheet = "PackSchedulePickSheet";
            public readonly string ProductionRecap = "ProductionRecap";
            public readonly string ProductionAdditives = "ProductionAdditives";
            public readonly string ProductionSchedule = "ProductionSchedule";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string ProductionBatchPacket = "ProductionBatchPacket";
            public const string PackSchedulePickSheet = "PackSchedulePickSheet";
            public const string ProductionRecap = "ProductionRecap";
            public const string ProductionAdditives = "ProductionAdditives";
            public const string ProductionSchedule = "ProductionSchedule";
        }


        static readonly ActionParamsClass_ProductionBatchPacket s_params_ProductionBatchPacket = new ActionParamsClass_ProductionBatchPacket();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ProductionBatchPacket ProductionBatchPacketParams { get { return s_params_ProductionBatchPacket; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ProductionBatchPacket
        {
            public readonly string packScheduleKey = "packScheduleKey";
            public readonly string batchKey = "batchKey";
        }
        static readonly ActionParamsClass_PackSchedulePickSheet s_params_PackSchedulePickSheet = new ActionParamsClass_PackSchedulePickSheet();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_PackSchedulePickSheet PackSchedulePickSheetParams { get { return s_params_PackSchedulePickSheet; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_PackSchedulePickSheet
        {
            public readonly string packScheduleKey = "packScheduleKey";
        }
        static readonly ActionParamsClass_ProductionRecap s_params_ProductionRecap = new ActionParamsClass_ProductionRecap();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ProductionRecap ProductionRecapParams { get { return s_params_ProductionRecap; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ProductionRecap
        {
            public readonly string startDate = "startDate";
            public readonly string endDate = "endDate";
        }
        static readonly ActionParamsClass_ProductionAdditives s_params_ProductionAdditives = new ActionParamsClass_ProductionAdditives();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ProductionAdditives ProductionAdditivesParams { get { return s_params_ProductionAdditives; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ProductionAdditives
        {
            public readonly string startDate = "startDate";
            public readonly string endDate = "endDate";
        }
        static readonly ActionParamsClass_ProductionSchedule s_params_ProductionSchedule = new ActionParamsClass_ProductionSchedule();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ProductionSchedule ProductionScheduleParams { get { return s_params_ProductionSchedule; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ProductionSchedule
        {
            public readonly string productionDate = "productionDate";
            public readonly string productionLocationKey = "productionLocationKey";
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
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_ProductionReportingController : RioValleyChili.Client.Mvc.Areas.Reporting.Controllers.ProductionReportingController
    {
        public T4MVC_ProductionReportingController() : base(Dummy.Instance) { }

        [NonAction]
        partial void ProductionBatchPacketOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string packScheduleKey, string batchKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult ProductionBatchPacket(string packScheduleKey, string batchKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionBatchPacket);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "packScheduleKey", packScheduleKey);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "batchKey", batchKey);
            ProductionBatchPacketOverride(callInfo, packScheduleKey, batchKey);
            return callInfo;
        }

        [NonAction]
        partial void PackSchedulePickSheetOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string packScheduleKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult PackSchedulePickSheet(string packScheduleKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PackSchedulePickSheet);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "packScheduleKey", packScheduleKey);
            PackSchedulePickSheetOverride(callInfo, packScheduleKey);
            return callInfo;
        }

        [NonAction]
        partial void ProductionRecapOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, System.DateTime? startDate, System.DateTime? endDate);

        [NonAction]
        public override System.Web.Mvc.ActionResult ProductionRecap(System.DateTime? startDate, System.DateTime? endDate)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionRecap);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "startDate", startDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "endDate", endDate);
            ProductionRecapOverride(callInfo, startDate, endDate);
            return callInfo;
        }

        [NonAction]
        partial void ProductionAdditivesOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, System.DateTime? startDate, System.DateTime? endDate);

        [NonAction]
        public override System.Web.Mvc.ActionResult ProductionAdditives(System.DateTime? startDate, System.DateTime? endDate)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionAdditives);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "startDate", startDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "endDate", endDate);
            ProductionAdditivesOverride(callInfo, startDate, endDate);
            return callInfo;
        }

        [NonAction]
        partial void ProductionScheduleOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, System.DateTime productionDate, string productionLocationKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult ProductionSchedule(System.DateTime productionDate, string productionLocationKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ProductionSchedule);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "productionDate", productionDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "productionLocationKey", productionLocationKey);
            ProductionScheduleOverride(callInfo, productionDate, productionLocationKey);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114