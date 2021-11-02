using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface IRemovePackScheduleParameters : IUserIdentifiable
    {
        string PackScheduleKey { get; }
    }
}