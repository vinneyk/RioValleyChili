using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Parameters.Common
{
    public abstract class FilterInventoryParametersBase
    {
        public string LotKey = null;
        public string ProductKey = null;
        public ProductTypeEnum? ProductType = null;
        public LotTypeEnum? LotType = null;
        public LotHoldType? HoldType = null;
        public string TreatmentKey = null;
        public string PackagingKey = null;
        public string PackagingReceivedKey = null;
        public string LocationKey = null;
        public string LocationGroupName = null;
    }
}