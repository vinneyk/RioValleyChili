using System;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.SalesReports)]
    public partial class SalesReportingController : RvcReportingMvcControllerBase
    {
        #region GET: /Reporting/Sales/Contract/2014-123

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.CustomerContracts, ClaimTypes.SalesClaimTypes.SalesReports)]
        public virtual ActionResult Contract(string contractKey)
        {
            return Pdf(
                ReportTypes.SalesReports.CustomerContractReport,
                new
                {
                    ContractKey = contractKey
                });
        }

        #endregion
        
        #region GET: /Reporting/Sales/Contract/2014-123/ContractDrawSummary

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.CustomerContracts, ClaimTypes.SalesClaimTypes.SalesReports)]
        public virtual ActionResult ContractDrawSummary(string contractKey = null)
        {
            ViewBag.ContractKey = contractKey;
            return Pdf(
                ReportTypes.SalesReports.CustomerContractDrawSummaryReport,
                new 
                {
                    ContractKey = contractKey
                });
        }

        #endregion
        
        public virtual ActionResult PendingOrderDetails(DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.Now;
            var start = startDate ?? new DateTime(now.Year, now.Month, 1);
            var end = endDate ?? start.AddMonths(1).AddDays(-1);

            return ReportView(
                "Pending Orders",
                ReportTypes.SalesReports.PendingOrderDetailsReport,
                new
                {
                    StartDate = start,
                    EndDate = end
                });
        }

        public virtual ActionResult CustomerOrderConfirmation(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.CustomerOrderAcknowledgementReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.SalesReports)]
        public virtual ActionResult InHouseConfirmation(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.InHouseOrderAcknowledgementReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.SalesReports)]
        public virtual ActionResult CustomerInvoice(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.CustomerOrderInvoiceReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.SalesReports)]
        public virtual ActionResult InHouseInvoice(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.InHouseInvoiceCopyReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.SalesReports)]
        public virtual ActionResult MiscInvoice(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.MiscOrderInvoiceReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        public virtual ActionResult MiscOrderCustomerConfirmation(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.MiscOrderCustomerAcknowledgementReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        public virtual ActionResult MiscOrderInternalConfirmation(string orderKey)
        {
            return Pdf(
                ReportTypes.SalesReports.MiscOrderInternalAcknowledgementReport,
                new
                {
                    OrderKey = orderKey
                });
        }

        public virtual ActionResult SalesQuoteReport(int quoteNumber)
        {
            return Pdf(
                ReportTypes.SalesReports.SalesQuoteReport,
                new
                {
                    QuoteNumber = quoteNumber
                });
        }
    }
}