using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class PackagingLotExtensions
    {
        internal static void ConstrainByLotKey(this PackagingLot packagingLot, ILotKey lotKey)
        {
            if(packagingLot == null) { throw new ArgumentNullException("packagingLot"); }
            if(lotKey == null) { throw new ArgumentNullException("lotKey"); }

            packagingLot.Lot = null;
            packagingLot.LotDateCreated = lotKey.LotKey_DateCreated;
            packagingLot.LotDateSequence = lotKey.LotKey_DateSequence;
            packagingLot.LotTypeId = lotKey.LotKey_LotTypeId;
        }
    }
}