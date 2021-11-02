using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class GetPackagingInventoryCommandParameter
    {
        public IPackagingProductKey PackagingProductKey { get; set; }

        public IFacilityKey FacilityKey { get; set; }

        public bool IncludeUnavailable { get; set; }
    }
}