using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Utilities.Extensions.Keys
{
    public static class ChileProductIngredientKeyExtensions
    {
        #region Extensions.

        public static string BuildKey(this ChileProductIngredient chileProductIngredient)
        {
            if(chileProductIngredient == null) { throw new ArgumentNullException("chileProductIngredient"); }

            return new ChileProductIngredientKey(chileProductIngredient).ToString();
        }

        #endregion
    }
}
