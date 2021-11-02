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
    public partial class SalesReportingController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SalesReportingController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected SalesReportingController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult Contract()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Contract);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult CustomerOrderConfirmation()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CustomerOrderConfirmation);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult InHouseConfirmation()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.InHouseConfirmation);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult CustomerInvoice()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CustomerInvoice);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult InHouseInvoice()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.InHouseInvoice);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult MiscInvoice()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MiscInvoice);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult MiscOrderCustomerConfirmation()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MiscOrderCustomerConfirmation);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult MiscOrderInternalConfirmation()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MiscOrderInternalConfirmation);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult SalesQuoteReport()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SalesQuoteReport);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public SalesReportingController Actions { get { return MVC.Reporting.SalesReporting; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "Reporting";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "SalesReporting";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "SalesReporting";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Contract = "Contract";
            public readonly string ContractDrawSummary = "ContractDrawSummary";
            public readonly string PendingOrderDetails = "PendingOrderDetails";
            public readonly string CustomerOrderConfirmation = "CustomerOrderConfirmation";
            public readonly string InHouseConfirmation = "InHouseConfirmation";
            public readonly string CustomerInvoice = "CustomerInvoice";
            public readonly string InHouseInvoice = "InHouseInvoice";
            public readonly string MiscInvoice = "MiscInvoice";
            public readonly string MiscOrderCustomerConfirmation = "MiscOrderCustomerConfirmation";
            public readonly string MiscOrderInternalConfirmation = "MiscOrderInternalConfirmation";
            public readonly string SalesQuoteReport = "SalesQuoteReport";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Contract = "Contract";
            public const string ContractDrawSummary = "ContractDrawSummary";
            public const string PendingOrderDetails = "PendingOrderDetails";
            public const string CustomerOrderConfirmation = "CustomerOrderConfirmation";
            public const string InHouseConfirmation = "InHouseConfirmation";
            public const string CustomerInvoice = "CustomerInvoice";
            public const string InHouseInvoice = "InHouseInvoice";
            public const string MiscInvoice = "MiscInvoice";
            public const string MiscOrderCustomerConfirmation = "MiscOrderCustomerConfirmation";
            public const string MiscOrderInternalConfirmation = "MiscOrderInternalConfirmation";
            public const string SalesQuoteReport = "SalesQuoteReport";
        }


        static readonly ActionParamsClass_Contract s_params_Contract = new ActionParamsClass_Contract();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Contract ContractParams { get { return s_params_Contract; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Contract
        {
            public readonly string contractKey = "contractKey";
        }
        static readonly ActionParamsClass_ContractDrawSummary s_params_ContractDrawSummary = new ActionParamsClass_ContractDrawSummary();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ContractDrawSummary ContractDrawSummaryParams { get { return s_params_ContractDrawSummary; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ContractDrawSummary
        {
            public readonly string contractKey = "contractKey";
        }
        static readonly ActionParamsClass_PendingOrderDetails s_params_PendingOrderDetails = new ActionParamsClass_PendingOrderDetails();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_PendingOrderDetails PendingOrderDetailsParams { get { return s_params_PendingOrderDetails; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_PendingOrderDetails
        {
            public readonly string startDate = "startDate";
            public readonly string endDate = "endDate";
        }
        static readonly ActionParamsClass_CustomerOrderConfirmation s_params_CustomerOrderConfirmation = new ActionParamsClass_CustomerOrderConfirmation();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_CustomerOrderConfirmation CustomerOrderConfirmationParams { get { return s_params_CustomerOrderConfirmation; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_CustomerOrderConfirmation
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_InHouseConfirmation s_params_InHouseConfirmation = new ActionParamsClass_InHouseConfirmation();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_InHouseConfirmation InHouseConfirmationParams { get { return s_params_InHouseConfirmation; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_InHouseConfirmation
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_CustomerInvoice s_params_CustomerInvoice = new ActionParamsClass_CustomerInvoice();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_CustomerInvoice CustomerInvoiceParams { get { return s_params_CustomerInvoice; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_CustomerInvoice
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_InHouseInvoice s_params_InHouseInvoice = new ActionParamsClass_InHouseInvoice();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_InHouseInvoice InHouseInvoiceParams { get { return s_params_InHouseInvoice; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_InHouseInvoice
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_MiscInvoice s_params_MiscInvoice = new ActionParamsClass_MiscInvoice();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MiscInvoice MiscInvoiceParams { get { return s_params_MiscInvoice; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MiscInvoice
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_MiscOrderCustomerConfirmation s_params_MiscOrderCustomerConfirmation = new ActionParamsClass_MiscOrderCustomerConfirmation();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MiscOrderCustomerConfirmation MiscOrderCustomerConfirmationParams { get { return s_params_MiscOrderCustomerConfirmation; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MiscOrderCustomerConfirmation
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_MiscOrderInternalConfirmation s_params_MiscOrderInternalConfirmation = new ActionParamsClass_MiscOrderInternalConfirmation();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MiscOrderInternalConfirmation MiscOrderInternalConfirmationParams { get { return s_params_MiscOrderInternalConfirmation; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MiscOrderInternalConfirmation
        {
            public readonly string orderKey = "orderKey";
        }
        static readonly ActionParamsClass_SalesQuoteReport s_params_SalesQuoteReport = new ActionParamsClass_SalesQuoteReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SalesQuoteReport SalesQuoteReportParams { get { return s_params_SalesQuoteReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SalesQuoteReport
        {
            public readonly string quoteNumber = "quoteNumber";
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
    public partial class T4MVC_SalesReportingController : RioValleyChili.Client.Mvc.Areas.Reporting.Controllers.SalesReportingController
    {
        public T4MVC_SalesReportingController() : base(Dummy.Instance) { }

        [NonAction]
        partial void ContractOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string contractKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult Contract(string contractKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Contract);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "contractKey", contractKey);
            ContractOverride(callInfo, contractKey);
            return callInfo;
        }

        [NonAction]
        partial void ContractDrawSummaryOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string contractKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult ContractDrawSummary(string contractKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ContractDrawSummary);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "contractKey", contractKey);
            ContractDrawSummaryOverride(callInfo, contractKey);
            return callInfo;
        }

        [NonAction]
        partial void PendingOrderDetailsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, System.DateTime? startDate, System.DateTime? endDate);

        [NonAction]
        public override System.Web.Mvc.ActionResult PendingOrderDetails(System.DateTime? startDate, System.DateTime? endDate)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PendingOrderDetails);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "startDate", startDate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "endDate", endDate);
            PendingOrderDetailsOverride(callInfo, startDate, endDate);
            return callInfo;
        }

        [NonAction]
        partial void CustomerOrderConfirmationOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult CustomerOrderConfirmation(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CustomerOrderConfirmation);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            CustomerOrderConfirmationOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void InHouseConfirmationOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult InHouseConfirmation(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.InHouseConfirmation);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            InHouseConfirmationOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void CustomerInvoiceOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult CustomerInvoice(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CustomerInvoice);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            CustomerInvoiceOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void InHouseInvoiceOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult InHouseInvoice(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.InHouseInvoice);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            InHouseInvoiceOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void MiscInvoiceOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult MiscInvoice(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MiscInvoice);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            MiscInvoiceOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void MiscOrderCustomerConfirmationOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult MiscOrderCustomerConfirmation(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MiscOrderCustomerConfirmation);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            MiscOrderCustomerConfirmationOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void MiscOrderInternalConfirmationOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string orderKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult MiscOrderInternalConfirmation(string orderKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MiscOrderInternalConfirmation);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "orderKey", orderKey);
            MiscOrderInternalConfirmationOverride(callInfo, orderKey);
            return callInfo;
        }

        [NonAction]
        partial void SalesQuoteReportOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int quoteNumber);

        [NonAction]
        public override System.Web.Mvc.ActionResult SalesQuoteReport(int quoteNumber)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SalesQuoteReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "quoteNumber", quoteNumber);
            SalesQuoteReportOverride(callInfo, quoteNumber);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
