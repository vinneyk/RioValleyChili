using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateProductionResultsParameters : IUpdateProductionBatchResultsParameters
    {
        public string UserToken { get; set; }
        public string ProductionResultKey { get; set; }
        public string ProductionShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionStartTimestamp { get; set; }
        public DateTime ProductionEndTimestamp { get; set; }

        public Location ProductionLine
        {
            get { return _productionLine; }
            set { ProductionLineKey = new LocationKey(_productionLine = value); }
        }
        private Location _productionLine;

        public IEnumerable<IBatchResultItemParameters> InventoryItems { get; set; }
        public IEnumerable<IPickedInventoryItemParameters> PickedInventoryItemChanges { get; set; }
    }
}