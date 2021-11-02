using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotQualitySummaryReturn : LotSummaryReturn, ILotQualitySummaryReturn
    {
        public bool ProductSpecComplete { get; internal set; }
        public bool ProductSpecOutOfRange { get; internal set; }

        public IEnumerable<ILotCustomerAllowanceReturn> CustomerAllowances { get; internal set; }
        public IEnumerable<ILotCustomerOrderAllowanceReturn> CustomerOrderAllowances { get; internal set; }
        public IEnumerable<ILotContractAllowanceReturn> ContractAllowances { get; internal set; }

        public IEnumerable<LotQualityStatus> ValidLotQualityStatuses
        {
            get
            {
                if(_validLotQualityStatuses == null)
                {
                    _validLotQualityStatuses = LotStatusHelper.GetValidLotQualityStatuses(QualityStatus, ProductSpecComplete,
                        Defects.Any(d => d.DefectType == DefectTypeEnum.ActionableDefect && d.Resolution == null),
                        Defects.Any(d => d.DefectType == DefectTypeEnum.BacterialContamination && d.Resolution == null));
                }
                return _validLotQualityStatuses;
            }
        }
        private IEnumerable<LotQualityStatus> _validLotQualityStatuses;
    }
}