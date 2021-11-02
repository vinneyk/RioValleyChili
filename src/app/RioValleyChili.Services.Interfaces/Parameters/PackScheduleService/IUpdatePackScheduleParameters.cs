namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface IUpdatePackScheduleParameters : ICreatePackScheduleParameters
    {
        string PackScheduleKey { get; }
    }
}