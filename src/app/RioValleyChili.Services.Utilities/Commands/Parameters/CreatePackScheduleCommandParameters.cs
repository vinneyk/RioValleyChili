using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreatePackScheduleCommandParameters
    {
        public ICreatePackScheduleParameters Parameters { get; set; }

        public WorkTypeKey WorkTypeKey { get; set; }
        public ChileProductKey ChileProductKey { get; set; }
        public PackagingProductKey PackagingProductKey { get; set; }
        public LocationKey ProductionLocationKey { get; set; }
        public CustomerKey CustomerKey { get; set; }
    }
}