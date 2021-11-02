using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService
{
    public interface IProductionBatchResultsParameters : IUserIdentifiable
    {
        string ProductionShiftKey { get; }
        string ProductionLineKey { get; }
        DateTime ProductionStartTimestamp { get; }
        DateTime ProductionEndTimestamp { get; }

        IEnumerable<IBatchResultItemParameters> InventoryItems { get; }
        IEnumerable<IPickedInventoryItemParameters> PickedInventoryItemChanges { get; }
    }
}