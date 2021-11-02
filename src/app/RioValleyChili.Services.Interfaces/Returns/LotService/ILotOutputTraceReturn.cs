using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotOutputTraceReturn
    {
        IEnumerable<string> LotPath { get; }
        IEnumerable<ILotOutputTraceInputReturn> Inputs { get; }
        IEnumerable<ILotOutputTraceOrdersReturn> Orders { get; }
    }

    public interface ILotOutputTraceInputReturn
    {
        string LotKey { get; }
        string Treatment { get; }
    }

    public interface ILotOutputTraceOrdersReturn
    {
        string Treatment { get; }
        int? OrderNumber { get; }
        DateTime? ShipmentDate { get; }
        string CustomerName { get; }
    }
}