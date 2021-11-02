using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class ChileMaterialsReceivedItemParameters : ICreateChileMaterialsReceivedItemParameters, IUpdateChileMaterialsReceivedItemParameters
    {
        public string ItemKey { get; set; }
        public string ToteKey { get; set; }
        public int Quantity { get; set; }
        public string Variety { get; set; }
        public string GrowerCode { get; set; }
        public string PackagingProductKey { get; set; }
        public string LocationKey { get; set; }

        public ChileMaterialsReceivedItemParameters() { }

        public ChileMaterialsReceivedItemParameters(ChileMaterialsReceivedItem item)
        {
            ItemKey = item.ToChileMaterialsReceivedItemKey();
            ToteKey = item.ToteKey;
            Quantity = item.Quantity;
            PackagingProductKey = item.ToPackagingProductKey();
            Variety = item.ChileVariety;
            GrowerCode = item.GrowerCode;
            LocationKey = item.ToLocationKey();
        }
    }
}