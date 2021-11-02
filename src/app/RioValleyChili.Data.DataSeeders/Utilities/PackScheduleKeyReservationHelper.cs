using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class PackScheduleKeyReservationHelper : KeyReservationHelperBase<PackScheduleKey, IPackScheduleKey>
    {
        private readonly Dictionary<DateTime, int> _sequences = new Dictionary<DateTime, int>();

        protected override IPackScheduleKey GetNextKey(IPackScheduleKey keyInterface)
        {
            int sequence;
            if(_sequences.TryGetValue(keyInterface.PackScheduleKey_DateCreated.Date, out sequence))
            {
                _sequences[keyInterface.PackScheduleKey_DateCreated.Date] = ++sequence;
            }
            else
            {
                _sequences.Add(keyInterface.PackScheduleKey_DateCreated.Date, sequence = keyInterface.PackScheduleKey_DateSequence + 1);
            }

            return new Key
                {
                    PackScheduleKey_DateCreated = keyInterface.PackScheduleKey_DateCreated.Date,
                    PackScheduleKey_DateSequence = sequence
                };
        }

        public class Key : IPackScheduleKey
        {
            public DateTime PackScheduleKey_DateCreated { get; set; }
            public int PackScheduleKey_DateSequence { get; set; }
        }
    }
}