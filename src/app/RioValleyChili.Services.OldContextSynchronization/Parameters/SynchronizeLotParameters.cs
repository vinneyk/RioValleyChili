using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.OldContextSynchronization.Parameters
{
    public class SynchronizeLotParameters
    {
        public ILotKey LotKey
        {
            get { return LotKeys == null ? null : LotKeys.FirstOrDefault(); }
            set { LotKeys = new List<LotKey> { value.ToLotKey() }; }
        }

        public List<LotKey> LotKeys { get; set; }

        public bool UpdateSerializationOnly = false;
        public bool OverrideOldContextLotAsCompleted;

        public int? LotStat;
        public string Notes;

    }
}
