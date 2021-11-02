using System;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;

namespace RioValleyChili.Data.DataSeeders
{ 
    public partial class tblLot : ILotKey, ILotAttributes, IProductionBatchTargetParameters
    {
        public DateTime LotKey_DateCreated { get { return ProductionDate.Value; } }
        public int LotKey_DateSequence { get { return BatchNum.Value; } }
        public int LotKey_LotTypeId { get { return PTypeID.Value; } }

        public double BatchTargetWeight { get { return (double) TargetWgt.Value; } }
        public double BatchTargetAsta { get { return (double) TgtAsta.Value; } }
        public double BatchTargetScan { get { return (double) TgtScan.Value; } }
        public double BatchTargetScoville { get { return (double) TgtScov.Value; } }
    }

    public partial class tblLotAttributeHistory : ILotAttributes
    {
        decimal? ILotAttributes.BI { get; set; }
    }
}