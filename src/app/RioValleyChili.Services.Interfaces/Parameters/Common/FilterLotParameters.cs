using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class FilterLotParameters
    {
        public ProductTypeEnum? ProductType = null;

        public LotTypeEnum? LotType = null;

        public LotProductionStatus? ProductionStatus = null;

        public LotQualityStatus? QualityStatus = null;

        public bool? ProductSpecComplete = null;

        public bool? ProductSpecOutOfRange = null;

        /// <summary>
        /// If not null, then this is the date that the Lot's ProductionStart must be greater than or equal to in order to be included in the results.
        /// </summary>
        public DateTime? ProductionStartRangeStart = null;

        /// <summary>
        /// If not null, then this is the date that the Lot's ProductionStart must be less than or equal to in order to be included in the results.
        /// </summary>
        public DateTime? ProductionStartRangeEnd = null;

        public string StartingLotKey = null;

        public string ProductKey = null;
    }
}