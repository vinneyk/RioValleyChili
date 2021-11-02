using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderItemSpecReturn : ISampleOrderItemSpecReturn
    {
        public double? AstaMin { get; internal set; }
        public double? AstaMax { get; internal set; }
        public double? MoistureMin { get; internal set; }
        public double? MoistureMax { get; internal set; }
        public double? WaterActivityMin { get; internal set; }
        public double? WaterActivityMax { get; internal set; }
        public double? Mesh { get; internal set; }
        public double? AoverB { get; internal set; }
        public double? ScovMin { get; internal set; }
        public double? ScovMax { get; internal set; }
        public double? ScanMin { get; internal set; }
        public double? ScanMax { get; internal set; }
        public double? TPCMin { get; internal set; }
        public double? TPCMax { get; internal set; }
        public double? YeastMin { get; internal set; }
        public double? YeastMax { get; internal set; }
        public double? MoldMin { get; internal set; }
        public double? MoldMax { get; internal set; }
        public double? ColiformsMin { get; internal set; }
        public double? ColiformsMax { get; internal set; }
        public double? EColiMin { get; internal set; }
        public double? EColiMax { get; internal set; }
        public double? SalMin { get; internal set; }
        public double? SalMax { get; internal set; }
        
        public string Notes { get; internal set; }
    }
}