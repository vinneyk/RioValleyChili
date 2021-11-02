using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdatePackScheduleParameters : IUpdatePackScheduleParameters
    {
        public string UserToken { get; set; }
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }
        public string WorkTypeKey { get; set; }
        public string ChileProductKey { get; set; }
        public string PackagingProductKey { get; set; }
        public DateTime ScheduledProductionDate { get; set; }
        public DateTime? ProductionDeadline { get; set; }
        public string ProductionLineKey { get; set; }
        public string SummaryOfWork { get; set; }
        public string CustomerKey { get; set; }
        public string OrderNumber { get; set; }
        public string PackScheduleKey { get; set; }

        DateTime? ICreatePackScheduleParameters.DateCreated { get { return null; } }
        int? ICreatePackScheduleParameters.Sequence { get { return null; } }
        int? ICreatePackScheduleParameters.PSNum { get { return null; } }

        public UpdatePackScheduleParameters() { }

        public UpdatePackScheduleParameters(PackSchedule packSchedule)
        {
            PackScheduleKey = new PackScheduleKey(packSchedule);
            BatchTargetWeight = packSchedule.DefaultBatchTargetParameters.BatchTargetWeight;
            BatchTargetAsta = packSchedule.DefaultBatchTargetParameters.BatchTargetAsta;
            BatchTargetScan = packSchedule.DefaultBatchTargetParameters.BatchTargetScan;
            BatchTargetScoville = packSchedule.DefaultBatchTargetParameters.BatchTargetScoville;
            WorkTypeKey = new WorkTypeKey(packSchedule);
            ChileProductKey = new WorkTypeKey(packSchedule);
            PackagingProductKey = new WorkTypeKey(packSchedule);
            ScheduledProductionDate = packSchedule.ScheduledProductionDate;
            ProductionDeadline = packSchedule.ProductionDeadline;
            ProductionLineKey = new LocationKey(packSchedule);
            SummaryOfWork = packSchedule.SummaryOfWork;
            CustomerKey = packSchedule.CustomerId == null ? null : new CustomerKey(packSchedule);
            OrderNumber = packSchedule.OrderNumber;
        }

        public void SetBatchTargetParameters(IProductionBatchTargetParameters targetParameters)
        {
            BatchTargetWeight = targetParameters.BatchTargetWeight;
            BatchTargetAsta = targetParameters.BatchTargetAsta;
            BatchTargetScan = targetParameters.BatchTargetScan;
            BatchTargetScoville = targetParameters.BatchTargetScoville;
        }
    }
}