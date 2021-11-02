using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class ProductionResultParameters
    {
        public IProductionBatchResultsParameters Parameters { get; set; }
        public LotKey LotKey { get; set; }
        public LocationKey ProductionLineLocationKey { get; set; }
        public PackScheduleKey PackScheduleKey { get; set; }

        public List<CreateProductionResultItemCommandParameters> InventoryItems { get; set; }
        public List<PickedInventoryParameters> PickedInventoryItemChanges { get; set; }

        public string TransactionSourceReference
        {
            get
            {
                if(PackScheduleKey != null)
                {
                    return PackScheduleKey;
                }
                return LotKey;
            }
        }

    }
}