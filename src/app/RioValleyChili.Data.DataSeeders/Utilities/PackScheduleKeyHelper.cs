using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class PackScheduleKeyHelper
    {
        private Dictionary<DateTime, PackScheduleKey> _keys;
        public PackScheduleKeyHelper(RioValleyChiliDataContext newContext)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _keys = newContext.Set<PackSchedule>().Select(p => new PackScheduleKey
                {
                    PackScheduleKey_DateCreated = p.DateCreated,
                    PackScheduleKey_DateSequence = p.SequentialNumber,
                    PackSchID = p.PackSchID
                }).ToDictionary(p => p.PackSchID, p => p);
        }

        public IPackScheduleKey GetPackScheduleKey(DateTime packSchID)
        {
            PackScheduleKey key;
            _keys.TryGetValue(packSchID, out key);
            return key;
        }

        private class PackScheduleKey : IPackScheduleKey
        {
            public DateTime PackScheduleKey_DateCreated { get; set; }
            public int PackScheduleKey_DateSequence { get; set; }
            public DateTime PackSchID { get; set; }
        }
    }
}