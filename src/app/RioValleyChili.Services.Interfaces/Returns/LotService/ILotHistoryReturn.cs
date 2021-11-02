using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotHistoryReturn
    {
        string LotKey { get; }
        DateTime Timestamp { get; }
        IUserSummaryReturn Employee { get; }

        bool LoBac { get; }
        LotHoldType? HoldType { get; }
        string HoldDescription { get; }
        LotQualityStatus QualityStatus { get; }
        LotProductionStatus ProductionStatus { get; }

        IInventoryProductReturn Product { get; }
        IEnumerable<ILotHistoryAttributeReturn> Attributes { get; }
        IEnumerable<ILotHistoryRecordReturn> History { get; }
    }

    public interface ILotHistoryAttributeReturn
    {
        string AttributeShortName { get; }
        double Value { get; }
        DateTime AttributeDate { get; }
        bool Computed { get; }
    }

    public interface ILotHistoryRecordReturn
    {
        DateTime Timestamp { get; }
        IUserSummaryReturn Employee { get; }
        
        bool LoBac { get; }
        LotHoldType? HoldType { get; }
        string HoldDescription { get; }
        LotQualityStatus QualityStatus { get; }
        LotProductionStatus ProductionStatus { get; }

        IEnumerable<ILotHistoryAttributeReturn> Attributes { get; }
    }
}