using System;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class AdditiveTypeProjectors
    {
        internal static Expression<Func<AdditiveType, AdditiveTypeKeyReturn>> SelectKey()
        {
            return a => new AdditiveTypeKeyReturn
                {
                    AdditiveTypeKey_AdditiveTypeId = a.Id
                };
        }

        internal static Expression<Func<AdditiveType, AdditiveTypeReturn>> Select()
        {
            var key = SelectKey();

            return a => new AdditiveTypeReturn
                {
                    AdditiveTypeKeyReturn = key.Invoke(a),
                    AdditiveTypeDescription = a.Description
                };
        }
    }
}