using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Areas.Reporting
{
    public class ReportingAreaRegistration : AreaRegistration 
    {
        public override string AreaName { get { return "Reporting"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            RegisterContractReportRoutes(context);
            
            context.MapRoute(
                "Reporting_InventoryShipmentOrder",
                "Reporting/InventoryShipmentOrderReporting/{action}/{orderKey}",
                new
                {
                    controller = MVC.Reporting.InventoryShipmentOrderReporting.Name,
                    action = "Index", orderKey = UrlParameter.Optional
                });

            context.MapRoute(
                "Reporting_default",
                "Reporting/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });

            context.MapRoute(
                "ProductionRecapReport",
                "Reporting/Production/Recap/{startDate}/{endDate}",
                new
                {
                    controller = MVC.Reporting.ProductionReporting.Name,
                    action = MVC.Reporting.ProductionReporting.ActionNames.ProductionRecap
                });

            context.MapRoute(
                "ProductionAdditiveInputsReport",
                "Reporting/Production/Additives/{startDate}/{endDate}",
                new
                {
                    controller = MVC.Reporting.ProductionReporting.Name,
                    action = MVC.Reporting.ProductionReporting.ActionNames.ProductionAdditives
                });
        }

        private static void RegisterContractReportRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CustomerContractDrawSummaryReport",
                "Contract/{contractKey}/Reports/ContractDrawSummary",
                new
                {
                    controller = MVC.Reporting.SalesReporting.Name,
                    action = MVC.Reporting.SalesReporting.ActionNames.ContractDrawSummary,
                });

            context.MapRoute(
                "CustomerContractReport",
                "Contract/{contractKey}/Reports/Contract",
                new
                {
                    controller = MVC.Reporting.SalesReporting.Name,
                    action = MVC.Reporting.SalesReporting.ActionNames.Contract,
                });
        }
    }
}