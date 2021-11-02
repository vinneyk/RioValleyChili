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
    public partial class CustomersController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CustomersController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected CustomersController(Dummy d) { }

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
        public CustomersController Actions { get { return MVC.Customers; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Customers";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Customers";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string SalesOrders = "SalesOrders";
            public readonly string Contracts = "Contracts";
            public readonly string CompanyMaintenance = "CompanyMaintenance";
            public readonly string Quotes = "Quotes";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string SalesOrders = "SalesOrders";
            public const string Contracts = "Contracts";
            public const string CompanyMaintenance = "CompanyMaintenance";
            public const string Quotes = "Quotes";
        }


        static readonly ActionParamsClass_Contracts s_params_Contracts = new ActionParamsClass_Contracts();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Contracts ContractsParams { get { return s_params_Contracts; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Contracts
        {
            public readonly string contractKey = "contractKey";
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
                public readonly string CompanyMaintenance = "CompanyMaintenance";
                public readonly string Contracts = "Contracts";
                public readonly string Index = "Index";
                public readonly string Quotes = "Quotes";
                public readonly string SalesOrders = "SalesOrders";
            }
            public readonly string CompanyMaintenance = "~/Views/Customers/CompanyMaintenance.cshtml";
            public readonly string Contracts = "~/Views/Customers/Contracts.cshtml";
            public readonly string Index = "~/Views/Customers/Index.cshtml";
            public readonly string Quotes = "~/Views/Customers/Quotes.cshtml";
            public readonly string SalesOrders = "~/Views/Customers/SalesOrders.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_CustomersController : RioValleyChili.Client.Mvc.Controllers.CustomersController
    {
        public T4MVC_CustomersController() : base(Dummy.Instance) { }

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
        partial void SalesOrdersOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult SalesOrders()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SalesOrders);
            SalesOrdersOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void ContractsOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string contractKey);

        [NonAction]
        public override System.Web.Mvc.ActionResult Contracts(string contractKey)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Contracts);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "contractKey", contractKey);
            ContractsOverride(callInfo, contractKey);
            return callInfo;
        }

        [NonAction]
        partial void CompanyMaintenanceOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult CompanyMaintenance()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CompanyMaintenance);
            CompanyMaintenanceOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void QuotesOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Quotes()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Quotes);
            QuotesOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114