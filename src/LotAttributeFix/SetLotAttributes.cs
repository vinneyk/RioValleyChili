using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace LotAttributeFix
{
    public class SetLotAttributes
    {
        private readonly RioAccessSQLEntities _oldContext;

        public SetLotAttributes(RioAccessSQLEntities oldContext)
        {
            if(oldContext == null) { throw new ArgumentNullException("oldContext"); }
            _oldContext = oldContext;
        }

        public void UpdateOldLotAttributes(ILotKey lotKey, List<LotAttribute> newLotAttributes)
        {
            var lotNumber = LotNumberBuilder.BuildLotNumber(lotKey);
            var oldLot = _oldContext.tblLots
                .Include(l => l.tblLotAttributeHistory)
                .FirstOrDefault(l => l.Lot == lotNumber);
            if(oldLot == null)
            {
                throw new Exception(string.Format("Could not find tblLot[{0}]", lotNumber));
            }

            SyncProductionBatchPickedInventoryHelper.SetLotBatchAttributes(oldLot, newLotAttributes);
            LotSyncHelper.SetLotAttributes(oldLot, newLotAttributes);
        }
    }
}