using System;
using AutoMapper;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Core.Helpers
{
    public static class AutoMapperHelpers
    {
        public static void ResolveWithWarehouseLocationFormatting<T>(this IMemberConfigurationExpression<T> scope, Func<T, string> resolver)
        {
            scope.ResolveUsing(m => LocationDescriptionHelper.FormatLocationDescription(resolver.Invoke(m)));
        }
    }
}