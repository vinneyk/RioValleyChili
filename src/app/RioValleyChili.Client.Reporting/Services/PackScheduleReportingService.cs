using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Client.Core.Extensions;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class PackScheduleReportingService : ReportingService
    {
        private readonly IProductionService _productionService;

        public PackScheduleReportingService() : this(ResolveService<IProductionService>()) { }

        public PackScheduleReportingService(IProductionService productionService)
        {
            if(productionService == null) throw new ArgumentNullException("productionService");
            _productionService = productionService;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<ProductionBatchPacketReportModel> GetProductionBatchBatchPacket(string packScheduleKey, string batchKey)
        {
            var result = string.IsNullOrEmpty(batchKey)
                ? _productionService.GetProductionPacketForPackSchedule(packScheduleKey)
                : _productionService.GetProductionPacketForBatch(batchKey);

            return result.Success 
                ? result.ResultingObject.Map().To<IEnumerable<ProductionBatchPacketReportModel>>()
                : new List<ProductionBatchPacketReportModel>();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<PackSchedulePickSheetReportModel> GetPackSchedulePickSheet(string packScheduleKey)
        {
            var result = _productionService.GetPackSchedulePickSheet(packScheduleKey);
            var mappingResults = result.Success ? result.ResultingObject.Map().To<IEnumerable<PackSchedulePickSheetReportModel>>() : new List<PackSchedulePickSheetReportModel>();

            try
            {
                var serviceJson = JsonConvert.SerializeObject(result.ResultingObject, Formatting.Indented);
                var mappingJson = JsonConvert.SerializeObject(mappingResults, Formatting.Indented);

                var now = DateTime.Now;
                var logPath = string.Format(@"D:\RVCDataLoad\PickSheetReportLogs\packSchedulePickSheetReport {0}.txt", now.ToString("yyyy-MM-dd_hh-mm-ss-tt"));
                using(var writer = new StreamWriter(logPath, true))
                {
                    writer.WriteLine("Timestamp: {0}", now);
                    writer.WriteLine("ServiceResult:");
                    writer.WriteLine(serviceJson);

                    writer.WriteLine("MappingResult:");
                    writer.WriteLine(mappingJson);

                    writer.WriteLine("----------------------------------");
                }
            }
            catch(Exception) { }

            return mappingResults;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<ProductionScheduleReportModel> GetProductionSchedule(DateTime productionDate, string productionLocationKey)
        {
            var result = _productionService.GetProductionScheduleReport(productionDate, productionLocationKey);
            var reportModel = result.Success
                ? result.ResultingObject.Project().To<ProductionScheduleReportModel>()
                    .Where(m => m.ScheduledItems.Any())
                    .OrderBy(m => m.Line)
                    .ToList()
                : new List<ProductionScheduleReportModel>();
            reportModel.ForEach(m => m.ScheduledItems.ForEach(i => i.Initialize(m)));

            return reportModel;
        }
    }
}