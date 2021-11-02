using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetSampleSpecsParameters : ISetSampleSpecsParameters
    {
        public string SampleOrderItemKey { get; set; }
        public string Notes { get; set; }

        public double? AstaMin { get; set; }
        public double? AstaMax { get; set; }
        public double? MoistureMin { get; set; }
        public double? MoistureMax { get; set; }
        public double? WaterActivityMin { get; set; }
        public double? WaterActivityMax { get; set; }
        public double? Mesh { get; set; }
        public double? AoverB { get; set; }
        public double? ScovMin { get; set; }
        public double? ScovMax { get; set; }
        public double? ScanMin { get; set; }
        public double? ScanMax { get; set; }
        public double? TPCMin { get; set; }
        public double? TPCMax { get; set; }
        public double? YeastMin { get; set; }
        public double? YeastMax { get; set; }
        public double? MoldMin { get; set; }
        public double? MoldMax { get; set; }
        public double? ColiformsMin { get; set; }
        public double? ColiformsMax { get; set; }
        public double? EColiMin { get; set; }
        public double? EColiMax { get; set; }
        public double? SalMin { get; set; }
        public double? SalMax { get; set; }
    }
}