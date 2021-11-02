using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ChileLotExtensions
    {
        internal static ChileLot SetLotKey(this ChileLot chileLot, ILotKey lotKey = null)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }

            if(lotKey != null)
            {
                if(chileLot.Lot != null)
                {
                    chileLot.Lot.LotDateCreated = lotKey.LotKey_DateCreated;
                    chileLot.Lot.LotDateSequence = lotKey.LotKey_DateSequence;
                    chileLot.Lot.LotTypeId = lotKey.LotKey_LotTypeId;
                }
                chileLot.LotDateCreated = lotKey.LotKey_DateCreated;
                chileLot.LotDateSequence = lotKey.LotKey_DateSequence;
                chileLot.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            return chileLot;
        }

        internal static ChileLot SetDerivedLot(this ChileLot chileLot)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }
            if(chileLot.Lot != null)
            {
                chileLot.Lot.AdditiveLot = null;
                chileLot.Lot.PackagingLot = null;
            }
            return chileLot;
        }

        internal static ChileLot NullProduction(this ChileLot chileLot)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }

            chileLot.Production = null;

            return chileLot;
        }

        internal static ChileLot SetProduct(this ChileLot chileLot, IChileProductKey chileProductKey)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }

            chileLot.ChileProduct = null;
            chileLot.ChileProductId = chileProductKey.ChileProductKey_ProductId;

            return chileLot;
        }

        internal static ChileLot SetLotType(this ChileLot chileLot, LotTypeEnum? lotType = null)
        {
            if(chileLot == null) { throw new ArgumentNullException("chileLot"); }
            chileLot.LotTypeEnum = lotType ?? LotTypeTestHelper.GetRandomChileLotType();
            return chileLot;
        }
    }
}