using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class AdditiveLotExtensions
    {
        internal static AdditiveLot SetLotKey(this AdditiveLot additiveLot, ILotKey lotKey)
        {
            if(additiveLot == null) { throw new ArgumentNullException("additiveLot"); }
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            additiveLot.Lot = null;
            additiveLot.LotDateCreated = lotKey.LotKey_DateCreated;
            additiveLot.LotDateSequence = lotKey.LotKey_DateSequence;
            additiveLot.LotTypeId = lotKey.LotKey_LotTypeId;
            return additiveLot;
        }

        internal static AdditiveLot SetLot(this AdditiveLot additiveLot, Lot lot)
        {
            if(additiveLot == null) { throw new ArgumentNullException("additiveLot"); }
            if(lot == null) { throw new ArgumentNullException("lot"); }

            additiveLot.SetLotKey(lot).Lot = lot;

            return additiveLot;
        }
    }
}