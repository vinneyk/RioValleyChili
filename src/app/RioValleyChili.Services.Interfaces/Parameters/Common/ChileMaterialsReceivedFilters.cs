using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public class ChileMaterialsReceivedFilters
    {
        public ChileMaterialsReceivedType? ChileMaterialsType { get; set; }
        public string SupplierKey;
        public string ChileProductKey;
    }
}