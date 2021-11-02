using System;
using System.Collections.Generic;

namespace RioValleyChili.Client.Reporting.Models
{
    public class SampleOrderMatchingSummaryReportModel
    {
        public string SampleOrderKey { get; set; }
        public string Company { get; set; }
        public string Contact { get; set; }
        public IEnumerable<SampleOrderMatchingSummaryItem> Items { get; set; }
    }

    public class SampleOrderMatchingSummaryItem
    {
        public string SampleMatch { get; set; }

        public string AstaColorSpec { get; set; }
        public string MoistureSpec { get; set; }
        public string ParticleSpec { get; set; }
        public string SurfaceColorSpec { get; set; }
        public string ABSpec { get; set; }
        public string SHUSpec { get; set; }
        public string TPCSpec { get; set; }
        public string YeastSpec { get; set; }
        public string MoldSpec { get; set; }
        public string ColiformsSpec { get; set; }
        public string EColiSpec { get; set; }
        public string SalmonellaSpec { get; set; }

        public string AstaColorSample { get; set; }
        public string MoistureSample { get; set; }
        public string ParticleSample { get; set; }
        public string SurfaceColorSample { get; set; }
        public string ABSample { get; set; }
        public string SHUSample { get; set; }
        public string TPCSample { get; set; }
        public string YeastSample { get; set; }
        public string MoldSample { get; set; }
        public string ColiformsSample { get; set; }
        public string EColiSample { get; set; }
        public string SalmonellaSample { get; set; }

        public string AstaColorMatch { get; set; }
        public string MoistureMatch { get; set; }
        public string ParticleMatch { get; set; }
        public string SurfaceColorMatch { get; set; }
        public string ABMatch { get; set; }
        public string SHUMatch { get; set; }
        public string TPCMatch { get; set; }
        public string YeastMatch { get; set; }
        public string MoldMatch { get; set; }
        public string ColiformsMatch { get; set; }
        public string EColiMatch { get; set; }
        public string SalmonellaMatch { get; set; }

        public string Status { get; set; }
        public string LotNumber { get; set; }
        public string QuantitySent { get; set; }
        public string ProductName { get; set; }
        public string Employee { get; set; }
        public DateTime Received { get; set; }
        public DateTime SampleDate { get; set; }
        public DateTime? Completed { get; set; }

        public IEnumerable<SampleOrderMatchingSummaryItem> This { get { return new[] { this }; } }
    }
}