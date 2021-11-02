using System;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdatePackScheduleParameters : IUpdatePackScheduleParameters
    {
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
        public double BatchTargetWeight { get; set; }
        public double BatchTargetAsta { get; set; }
        public double BatchTargetScan { get; set; }
        public double BatchTargetScoville { get; set; }


        public string UserToken
        {
            get { return ((IUserIdentifiable)this).UserToken; }
            internal set { ((IUserIdentifiable)this).UserToken = value; }
        }

        #region explicit interface implementations

        string IUserIdentifiable.UserToken { get; set; }

        DateTime? ICreatePackScheduleParameters.DateCreated { get { return null; } }
        int? ICreatePackScheduleParameters.Sequence { get { return null; } }
        int? ICreatePackScheduleParameters.PSNum { get { return null; } }

        #endregion

        public void SetBatchTargetParameters(IProductionBatchTargetParameters targetParameters)
        {
            BatchTargetWeight = targetParameters.BatchTargetWeight;
            BatchTargetAsta = targetParameters.BatchTargetAsta;
            BatchTargetScan = targetParameters.BatchTargetScan;
            BatchTargetScoville = targetParameters.BatchTargetScoville;
        }
    }
}