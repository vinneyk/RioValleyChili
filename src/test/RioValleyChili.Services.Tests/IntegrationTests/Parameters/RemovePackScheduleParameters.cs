using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class RemovePackScheduleParameters : IRemovePackScheduleParameters
    {
        public string UserToken { get; set; }
        public string PackScheduleKey { get; set; }
    }
}