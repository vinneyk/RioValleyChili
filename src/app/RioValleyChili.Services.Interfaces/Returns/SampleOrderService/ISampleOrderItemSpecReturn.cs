namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderItemSpecReturn
    {
        double? AstaMin { get; }
        double? AstaMax { get; }
        double? MoistureMin { get; }
        double? MoistureMax { get; }
        double? WaterActivityMin { get; }
        double? WaterActivityMax { get; }
        double? Mesh { get; }
        double? AoverB { get; }
        double? ScovMin { get; }
        double? ScovMax { get; }
        double? ScanMin { get; }
        double? ScanMax { get; }
        double? TPCMin { get; }
        double? TPCMax { get; }
        double? YeastMin { get; }
        double? YeastMax { get; }
        double? MoldMin { get; }
        double? MoldMax { get; }
        double? ColiformsMin { get; }
        double? ColiformsMax { get; }
        double? EColiMin { get; }
        double? EColiMax { get; }
        double? SalMin { get; }
        double? SalMax { get; }

        string Notes { get; }
    }
}