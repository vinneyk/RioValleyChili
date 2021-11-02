using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.OldContextSynchronization.Parameters
{
    public class SynchronizeCustomerProductSpecs
    {
        public ChileProductKey ChileProductKey;
        public CustomerKey CustomerKey;

        public SerializedSpecKey Delete;
    }

    public class SerializedSpecKey
    {
        public int ProdID;
        public string Company_IA;
    }
}