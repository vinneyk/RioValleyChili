using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderItemMatchReturn : ISampleOrderItemMatchReturn
    {
        public string Gran { get; internal set; }
        public string AvgAsta { get; internal set; }
        public string AoverB { get; internal set; }
        public string AvgScov { get; internal set; }
        public string H2O { get; internal set; }
        public string Scan { get; internal set; }
        public string Yeast { get; internal set; }
        public string Mold { get; internal set; }
        public string Coli { get; internal set; }
        public string TPC { get; internal set; }
        public string EColi { get; internal set; }
        public string Sal { get; internal set; }
        public string InsPrts { get; internal set; }
        public string RodHrs { get; internal set; }

        public string Notes { get; internal set; }
    }
}