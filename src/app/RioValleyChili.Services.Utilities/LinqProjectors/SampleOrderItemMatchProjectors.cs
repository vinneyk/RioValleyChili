using System;
using System.Linq.Expressions;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SampleOrderItemMatchProjectors
    {
        internal static Expression<Func<SampleOrderItemMatch, SampleOrderItemMatchReturn>> Select()
        {
            return m => new SampleOrderItemMatchReturn
                {
                    Gran = m.Gran,
                    AvgAsta = m.AvgAsta,
                    AoverB = m.AoverB,
                    AvgScov = m.AvgScov,
                    H2O = m.H2O,
                    Scan = m.Scan,
                    Yeast = m.Yeast,
                    Mold = m.Mold,
                    Coli = m.Coli,
                    TPC = m.TPC,
                    EColi = m.EColi,
                    Sal = m.Sal,
                    InsPrts = m.InsPrts,
                    RodHrs = m.RodHrs,

                    Notes = m.Notes
                };
        }
    }
}