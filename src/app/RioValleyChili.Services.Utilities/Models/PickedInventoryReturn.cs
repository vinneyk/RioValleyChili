using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PickedInventoryReturn : AttributesByTypeReturn, IPickedInventoryDetailReturn, IPickedInventorySummaryReturn
    {
        public string PickedInventoryKey { get { return PickedInventoryKeyReturn.PickedInventoryKey; } }
        public int TotalQuantityPicked { get; internal set; }
        public double TotalWeightPicked { get; internal set; }
        public IEnumerable<IPickedInventoryItemReturn> PickedInventoryItems { get; internal set; }

        internal PickedInventoryKeyReturn PickedInventoryKeyReturn { get; set; }
    }
}