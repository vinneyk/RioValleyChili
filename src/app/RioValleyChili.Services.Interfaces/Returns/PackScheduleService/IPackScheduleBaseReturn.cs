namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IPackScheduleBaseReturn
    {
        string PackScheduleKey { get; }
        int? PSNum { get; }
        string WorkType { get; }
        string ProductionLineDescription { get; }
    }
}