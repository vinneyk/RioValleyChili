using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetSampleMatchParameters : ISetSampleMatchParameters
    {
        public string SampleOrderItemKey { get; set; }
        public string Notes { get; set; }
        public string Gran { get; set; }
        public string AvgAsta { get; set; }
        public string AoverB { get; set; }
        public string AvgScov { get; set; }
        public string H2O { get; set; }
        public string Scan { get; set; }
        public string Yeast { get; set; }
        public string Mold { get; set; }
        public string Coli { get; set; }
        public string TPC { get; set; }
        public string EColi { get; set; }
        public string Sal { get; set; }
        public string InsPrts { get; set; }
        public string RodHrs { get; set; }
    }
}