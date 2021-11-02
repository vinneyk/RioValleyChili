using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateProductionBatchResultsParameters : ICreateProductionBatchResultsParameters
    {
        public string UserToken { get; set; }
        public string ProductionShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionStartTimestamp { get; set; }
        public DateTime ProductionEndTimestamp { get; set; }
        public string ProductionBatchKey { get; set; }
        public IEnumerable<IBatchResultItemParameters> InventoryItems { get; set; }
        public IEnumerable<IPickedInventoryItemParameters> PickedInventoryItemChanges { get; set; }
    }

    public class UpdateProductionBatchResultsParameters : IUpdateProductionBatchResultsParameters
    {
        public string UserToken { get; set; }
        public string ProductionResultKey { get; set; }
        public string ProductionShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionStartTimestamp { get; set; }
        public DateTime ProductionEndTimestamp { get; set; }
        public IEnumerable<IBatchResultItemParameters> InventoryItems { get; set; }
        public IEnumerable<IPickedInventoryItemParameters> PickedInventoryItemChanges { get; set; }
    }
}