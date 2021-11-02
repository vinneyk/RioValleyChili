using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetChileMaterialsReceivedItemParameters
    {
        internal string GrowerCode { get; set; }
        internal string ToteKey { get; set; }
        internal string ChileVariety { get; set; }
        internal int Quantity { get; set; }

        internal ChileMaterialsReceivedItemKey ItemKey { get; set; }
        internal PackagingProductKey PackagingProductKey { get; set; }
        internal LocationKey LocationKey { get; set; }
    }
}