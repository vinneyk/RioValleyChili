using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ChileLotProductionExtensions
    {
        internal static ChileLotProduction Set(this ChileLotProduction production, ILotKey lotKey, ProductionType? productionType = null)
        {
            if(production == null) { throw new ArgumentNullException("production"); }

            if(lotKey != null)
            {
                production.ResultingChileLot = null;
                production.LotDateCreated = lotKey.LotKey_DateCreated;
                production.LotDateSequence = lotKey.LotKey_DateSequence;
                production.LotTypeId = lotKey.LotKey_LotTypeId;
            }

            if(productionType != null)
            {
                production.ProductionType = productionType.Value;
            }

            return production;
        }
    }
}