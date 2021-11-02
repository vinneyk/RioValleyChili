using System;
using System.Data.Entity;
using System.Linq;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class ShipmentInformationKeys : SequenceHelper<DateTime>
    {
        public ShipmentInformationKeys(DbContext context)
        {
            Sequences = context.Set<ShipmentInformation>().AsNoTracking()
                               .Select(p => new { p.DateCreated, p.Sequence })
                               .GroupBy(k => k.DateCreated)
                               .ToDictionary(g => g.Key, g => g.Max(k => k.Sequence));
        }
    }
}