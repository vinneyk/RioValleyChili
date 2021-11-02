using System;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class RioAccessSQLEntitiesExtensions
    {
        public static tblLot GetLot(this RioAccessSQLEntities oldContext, ILotKey lotKey, bool throwIfNotFound = true)
        {
            var lotNumber = LotNumberBuilder.BuildLotNumber(lotKey);
            var oldLot = oldContext.tblLots.Where(l => l.Lot == lotNumber).Select(l => new
                {
                    lot = l,
                    l.tblLotAttributeHistory
                }).FirstOrDefault();
            if(oldLot == null && throwIfNotFound)
            {
                throw new Exception(string.Format("Lot[{0}] not found in old context.", lotNumber));
            }

            return oldLot == null ? null : oldLot.lot;
        }
    }
}