﻿using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateProductionBatchResultsParameters : ICreateProductionBatchResultsParameters
    {
        public string UserToken { get; set; }
        public string ProductionBatchKey { get; set; }
        public string ProductionShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionStartTimestamp { get; set; }
        public DateTime ProductionEndTimestamp { get; set; }

        public IEnumerable<BatchProductionResultInventoryItemSummary> InventoryItems { get; set; }
        public IEnumerable<SetPickedInventoryItemParameters> PickedInventoryItemChanges { get; set; }

        #region explicit interface implementations

        IEnumerable<IBatchResultItemParameters> IProductionBatchResultsParameters.InventoryItems { get { return InventoryItems; } }
        IEnumerable<IPickedInventoryItemParameters> IProductionBatchResultsParameters.PickedInventoryItemChanges { get { return PickedInventoryItemChanges; } }
        string IUserIdentifiable.UserToken { get; set; }

        #endregion
    }
}