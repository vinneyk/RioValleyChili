using System;
using System.Collections.Generic;
using System.ComponentModel;
using AutoMapper;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class LotReportingService : ReportingService
    {
        #region initialization
        
        private readonly ILotService _lotService;

        public LotReportingService() : this(ResolveService<ILotService>()) { }

        public LotReportingService(ILotService lotService)
        {
            if (lotService == null) throw new ArgumentNullException("lotService");
            _lotService = lotService;
        }

        #endregion

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<LabResultsReportModel> GetLabResultsReportData(DateTime? startDate, DateTime? endDate)
        {
            var defaultStartDate = DateTime.Now.AddDays(-1).Date;
            var defaultEndDate = DateTime.Now;

            var result = _lotService.GetLabReport(
                startDate ?? defaultStartDate,
                endDate ?? defaultEndDate);

            return result.Success
                ? Mapper.Map<IEnumerable<LabResultsReportModel>>(result.ResultingObject)
                : new List<LabResultsReportModel>();
        }
    }
}