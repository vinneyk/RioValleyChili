using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderMatchingSummaryReportReturn
    {
        string SampleOrderKey { get; }
        string Company { get; }
        string Contact { get; }
        IEnumerable<ISampleOrderMatchingItemReturn> Items { get; }
    }

    public interface ISampleOrderMatchingItemReturn
    {
        string SampleMatch { get; }

        string AstaColorSpec { get; }
        string MoistureSpec { get; }
        string ParticleSpec { get; }
        string SurfaceColorSpec { get; }
        string ABSpec { get; }
        string SHUSpec { get; }
        string TPCSpec { get; }
        string YeastSpec { get; }
        string MoldSpec { get; }
        string ColiformsSpec { get; }
        string EColiSpec { get; }
        string SalmonellaSpec { get; }

        string AstaColorSample { get; }
        string MoistureSample { get; }
        string ParticleSample { get; }
        string SurfaceColorSample { get; }
        string ABSample { get; }
        string SHUSample { get; }
        string TPCSample { get; }
        string YeastSample { get; }
        string MoldSample { get; }
        string ColiformsSample { get; }
        string EColiSample { get; }
        string SalmonellaSample { get; }

        string AstaColorMatch { get; }
        string MoistureMatch { get; }
        string ParticleMatch { get; }
        string SurfaceColorMatch { get; }
        string ABMatch { get; }
        string SHUMatch { get; }
        string TPCMatch { get; }
        string YeastMatch { get; }
        string MoldMatch { get; }
        string ColiformsMatch { get; }
        string EColiMatch { get; }
        string SalmonellaMatch { get; }

        string Status { get; }
        string LotNumber { get; }
        string QuantitySent { get; }
        string ProductName { get; }
        string Employee { get; }
        DateTime Received { get; }
        DateTime SampleDate { get; }
        DateTime? Completed { get; }
    }
}