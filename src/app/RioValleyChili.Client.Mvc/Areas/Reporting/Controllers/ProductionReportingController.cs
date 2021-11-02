using System;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Controllers;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Reporting.Reports;

namespace RioValleyChili.Client.Mvc.Areas.Reporting.Controllers
{
    public partial class ProductionReportingController : RvcReportingMvcControllerBase
    {
        #region GET: /Reporting/Production/ProductionBatchPacket

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.PackSchedules)]
        public virtual ActionResult ProductionBatchPacket(string packScheduleKey, string batchKey)
        {
            return Pdf(ReportTypes.ProductionReports.ProductionBatchPacketReport, new
            {
                PackScheduleKey = packScheduleKey,
                BatchKey = batchKey
            });
        }

        #endregion

        [ClaimsAuthorize(ClaimActions.View, ClaimTypes.ProductionClaimTypes.PackSchedules)]
        public virtual ActionResult PackSchedulePickSheet(string packScheduleKey)
        {
            return Pdf(
                ReportTypes.ProductionReports.PackSchedulePickSheetReport,
                new
                {
                    PackScheduleKey = packScheduleKey
                });
        }

        public virtual ActionResult ProductionRecap(DateTime? startDate, DateTime? endDate)
        {
            return ReportView(
                "Production Recap",
                ReportTypes.ProductionReports.ProductionRecapReport,
                new
                {
                    StartDate = startDate ?? DateTime.Today,
                    EndDate = endDate ?? DateTime.Today
                });
        }

        public virtual ActionResult ProductionAdditives(DateTime? startDate, DateTime? endDate)
        {
            return ReportView(
                "Recap of Production Addtive Usage",
                ReportTypes.ProductionReports.ProductionAdditiveInputsReport,
                new
                {
                    StartDate = startDate ?? DateTime.Today,
                    EndDate = endDate ?? DateTime.Today
                });
        }

        public virtual ActionResult ProductionSchedule(DateTime productionDate, string productionLocationKey = "")
        {
            return Pdf(
                ReportTypes.ProductionReports.ProductionScheduleReport,
                new
                {
                    ProductionDate = productionDate,
                    ProductionLocationKey = productionLocationKey
                });
        }
    }
}