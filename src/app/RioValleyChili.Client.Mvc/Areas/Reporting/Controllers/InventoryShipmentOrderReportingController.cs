using System;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class InventoryShipmentOrderReportingController : RvcReportingMvcControllerBase
    {
        public virtual ActionResult WarehouseOrderAcknowledgement(string orderKey)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.WarehouseOrderAcknowledgementReport,
                new { OrderKey = orderKey });
        }

        public virtual ActionResult BillOfLading(string orderKey)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.BillOfLadingReport,
                new { OrderKey = orderKey });
        }

        public virtual ActionResult PackingList(string orderKey)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.PackingListReport,
                new { OrderKey = orderKey });
        }

        public virtual ActionResult PackingListBarcode(string orderKey)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.PackingListBarcodeReport,
                new { OrderKey = orderKey });
        }

        public virtual ActionResult PickSheet(string orderKey)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.PickSheetReport,
                new { OrderKey = orderKey });
        }

        public virtual ActionResult CertificateOfAnalysis(string orderKey)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.CertificateOfAnalysisReport,
                new { OrderKey = orderKey });
        }

        #region GET: /Reporting/InventoryShipmentOrderReporting/CustomerOrderAcknowledgement/20150721-31

        public virtual ActionResult CustomerOrderAcknowledgement(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.CustomerOrderAcknowledgementReport,
                new { OrderKey = orderKey });
        }

        #endregion

        #region GET: /Reporting/InventoryShipmentOrderReporting/PendingOrders?startDate=2015-01-24&endDate=2015-02-23

        public virtual ActionResult PendingOrders(DateTime startDate, DateTime endDate)
        {
            return Pdf(
                ReportTypes.ShipmentOrderReports.PendingOrderDetailsReport,
                new { StartDate = startDate, EndDate = endDate });
        }

        #endregion
    }
}