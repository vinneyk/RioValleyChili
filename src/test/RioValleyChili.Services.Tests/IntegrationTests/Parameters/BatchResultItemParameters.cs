using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductionResultsService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class BatchResultItemParameters : IBatchResultItemParameters
    {
        public string PackagingKey { get; set; }
        public string LocationKey { get; set; }
        public string InventoryTreatmentKey { get; set; }
        public int Quantity { get; set; }

        public PackagingProduct PackagingProduct
        {
            set { PackagingKey = new PackagingProductKey(value); }
        }

        public Location Location
        {
            get { return _location; }
            set { LocationKey = new LocationKey(_location = value); }
        }
        private Location _location;

        public InventoryTreatment InventoryTreatment { set { InventoryTreatmentKey = new InventoryTreatmentKey(value); } }
    }
}