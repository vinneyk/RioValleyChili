using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;

namespace RioValleyChili.Data.Models
{
    public class SerializedLotHistory
    {
        public DateTime TimeStamp { get; set; }
        public LotQualityStatus QualityStatus { get; set; }
        public LotProductionStatus ProductionStatus { get; set; }
        public LotHoldType? Hold { get; set; }
        public string HoldDescription { get; set; }
        public bool LoBac { get; set; }

        public IEnumerable<SerializedLotHistoryAttribute> Attributes { get; set; }

        public SerializedLotHistory() { }

        public SerializedLotHistory(Lot lot)
        {
            TimeStamp = lot.TimeStamp;
            QualityStatus = lot.QualityStatus;
            ProductionStatus = lot.ProductionStatus;
            Hold = lot.Hold;
            HoldDescription = lot.HoldDescription;
            LoBac = lot.ChileLot == null || lot.ChileLot.AllAttributesAreLoBac;

            Attributes = lot.Attributes.Select(a => new SerializedLotHistoryAttribute(a)).ToList();
        }
    }

    public class SerializedLotHistoryAttribute
    {
        public string AttributeShortName { get; set; }
        public double AttributeValue { get; set; }
        public DateTime AttributeDate { get; set; }
        public bool Computed { get; set; }

        public SerializedLotHistoryAttribute() { }

        public SerializedLotHistoryAttribute(LotAttribute attribute)
        {
            AttributeShortName = attribute.AttributeShortName;
            AttributeValue = attribute.AttributeValue;
            AttributeDate = attribute.AttributeDate;
            Computed = attribute.Computed;
        }
    }
}