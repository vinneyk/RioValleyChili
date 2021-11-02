using RioValleyChili.Core;
using RioValleyChili.Data;

namespace RioValleyChili.Services
{
    public static class DataLoadResultImplementation
    {
        public static IDataLoadResultObtainer GetRVCDataLoadResultInstance()
        {
            return new RVCDataLoadResultObtainer();
        }
    }
}