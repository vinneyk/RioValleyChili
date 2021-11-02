using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SampleOrderItemSpecProjectors
    {
        internal static Expression<Func<SampleOrderItemSpec, SampleOrderItemSpecReturn>> Select()
        {
            return s => new SampleOrderItemSpecReturn
                {
                    AstaMin = s.AstaMin,
                    AstaMax = s.AstaMax,
                    MoistureMin = s.MoistureMin,
                    MoistureMax = s.MoistureMax,
                    WaterActivityMin = s.WaterActivityMin,
                    WaterActivityMax = s.WaterActivityMax,
                    Mesh = s.Mesh,
                    AoverB = s.AoverB,
                    ScovMin = s.ScovMin,
                    ScovMax = s.ScovMax,
                    ScanMin = s.ScanMin,
                    ScanMax = s.ScanMax,
                    TPCMin = s.TPCMin,
                    TPCMax = s.TPCMax,
                    YeastMin = s.YeastMin,
                    YeastMax = s.YeastMax,
                    MoldMin = s.MoldMin,
                    MoldMax = s.MoldMax,
                    ColiformsMin = s.ColiformsMin,
                    ColiformsMax = s.ColiformsMax,
                    EColiMin = s.EColiMin,
                    EColiMax = s.EColiMax,
                    SalMin = s.SalMin,
                    SalMax = s.SalMax,

                    Notes = s.Notes
                };
        }
    }
}