using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService
{
    public interface IInventoryShipmentOrderItemAnalysisReturn
    {
        string LotKey { get; }
        DateTime? ProductionDate { get; }
        bool? LoBac { get; }
        string Notes { get; }
        IInventoryProductReturn LotProduct { get; }
        IInventoryTreatmentReturn TreatmentReturn { get; }
        IEnumerable<ILotAttributeReturn> Attributes { get; }
    }
}