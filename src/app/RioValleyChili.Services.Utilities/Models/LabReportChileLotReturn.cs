using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LabReportChileLotReturn : LotBaseReturn, ILabReportChileLotReturn
    {
        public string PackScheduleKey { get { return PackScheduleBaseReturn != null ? PackScheduleBaseReturn.PackScheduleKey : null; } }
        public int? PSNum { get { return PackScheduleBaseReturn != null ? PackScheduleBaseReturn.PSNum : null; } }
        public string WorkType { get { return PackScheduleBaseReturn != null ? PackScheduleBaseReturn.WorkType : null; } }
        public string ProductionLineDescription { get { return ProductionResultBaseReturn != null ? ProductionResultBaseReturn.ProductionLocation.Description : null; } }

        public IProductionBatchTargetParameters TargetParameters
        {
            get
            {
                if(_targetParameters != null)
                {
                    return _targetParameters;
                }
                if(PackScheduleBaseReturn != null)
                {
                    return _targetParameters = PackScheduleBaseReturn.TargetParameters;
                }
                return _targetParameters = new ProductionBatchTargetParameters();
            }
        }
        private IProductionBatchTargetParameters _targetParameters;

        public DateTime ProductionEndDate { get { return ProductionResultBaseReturn != null ? ProductionResultBaseReturn.ProductionEndDate : LotKeyReturn.LotKey_DateCreated; } }
        public string ProductionShiftKey { get { return ProductionResultBaseReturn != null ? ProductionResultBaseReturn.ProductionShiftKey : null; } }
        public ILocationReturn ProductionLocation { get { return ProductionResultBaseReturn != null ? ProductionResultBaseReturn.ProductionLocation : null; } }
        public bool LoBac { get; internal set; }
        public string ChileProductKey { get { return ChileProductKeyReturn.ProductKey; } }

        public string CustomerKey
        {
            get
            {
                if(PackScheduleBaseReturn != null && PackScheduleBaseReturn.Customer != null)
                {
                    return PackScheduleBaseReturn.Customer.CustomerKey;
                }
                return null;
            }
        }

        public string CustomerName
        {
            get
            {
                if(PackScheduleBaseReturn != null && PackScheduleBaseReturn.Customer != null)
                {
                    return PackScheduleBaseReturn.Customer.CustomerName;
                }
                return null;
            }
        }
        public bool ValidToPick { get; internal set; }
        public IEnumerable<string> UnresolvedDefects { get; internal set; }
        public IEnumerable<ILotCustomerAllowanceReturn> CustomerAllowances { get; internal set; }

        public IDictionary<string, IWeightedLotAttributeReturn> Attributes
        {
            get { return _attributes ?? (_attributes = WeightedAttributes.ToDictionary(a => a.Key, a => (IWeightedLotAttributeReturn)a)); }
        }
        private IDictionary<string, IWeightedLotAttributeReturn> _attributes;

        public IEnumerable<INoteReturn> Notes { get; internal set; }
        public IEnumerable<IDehydratedInputReturn> DehydratedInputs { get; internal set; }

        #region Internal Parts

        internal ProductKeyReturn ChileProductKeyReturn { get; set; }
        internal PackScheduleBaseWithCustomerReturn PackScheduleBaseReturn { get; set; }
        internal ProductionResultBaseReturn ProductionResultBaseReturn { get; set; }
        internal IEnumerable<WeightedLotAttributeReturn> WeightedAttributes { get; set; }
        internal IEnumerable<PickedLotReturn> PickedLots { get; set; }

        #endregion
    }
}