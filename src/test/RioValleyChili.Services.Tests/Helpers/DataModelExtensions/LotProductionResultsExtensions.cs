using System;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class LotProductionResultsExtensions
    {
        internal static LotProductionResults EmptyItems(this LotProductionResults productionResult)
        {
            if(productionResult == null) { throw new ArgumentNullException("productionResult"); }

            productionResult.ResultItems = null;

            return productionResult;
        }
    }
}