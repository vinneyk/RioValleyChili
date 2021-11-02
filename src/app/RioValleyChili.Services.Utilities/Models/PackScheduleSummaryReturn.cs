using System;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackScheduleSummaryReturn : PackScheduleBaseParametersReturn, IPackScheduleSummaryReturn
    {
        public string WorkTypeKey { get { return WorkTypeKeyReturn.WorkTypeKey; } }

        public string ChileProductKey { get { return ChileProductKeyReturn.ProductKey; } }

        public string ProductionLineKey { get { return ProductionLocationKeyReturn.LocationKey; } }

        public DateTime DateCreated { get; internal set; }

        public DateTime ScheduledProductionDate { get; internal set; }

        public DateTime? ProductionDeadline { get; internal set; }

        public string ChileProductName { get; internal set; }

        public string OrderNumber { get; internal set; }

        public ICompanyHeaderReturn Customer { get; internal set; }

        #region Internal Parts

        internal LocationKeyReturn ProductionLocationKeyReturn { get; set; }

        internal ProductKeyReturn ChileProductKeyReturn { get; set; }

        internal WorkTypeKeyReturn WorkTypeKeyReturn { get; set; }

        #endregion
    }
}