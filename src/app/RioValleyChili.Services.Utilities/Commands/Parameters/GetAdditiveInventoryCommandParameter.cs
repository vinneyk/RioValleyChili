using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class GetAdditiveInventoryCommandParameter
    {
        public IAdditiveProductKey AdditiveProductKey { get; set; }

        public bool IncludeUnavailable { get; set; }
    }
}