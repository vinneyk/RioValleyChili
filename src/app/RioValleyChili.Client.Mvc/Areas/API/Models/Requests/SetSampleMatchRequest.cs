using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetSampleMatchRequest
    {
        [StringLength(Constants.StringLengths.SampleOrderNotes)]
        public string Notes { get; set; }

        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string Gran { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string AvgAsta { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string AoverB { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string AvgScov { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string H2O { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string Scan { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string Yeast { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string Mold { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string Coli { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string TPC { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string EColi { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string Sal { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string InsPrts { get; set; }
        [StringLength(Constants.StringLengths.SampleMatchAttribute)]
        public string RodHrs { get; set; }
    }
}