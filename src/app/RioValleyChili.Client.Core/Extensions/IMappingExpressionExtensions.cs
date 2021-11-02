using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;

namespace RioValleyChili.Client.Core.Extensions
{
    public static class IMappingExpressionExtensions
    {
        public static IMappingExpression<TSource, TDestination> Ignoring<TSource, TDestination>(this IMappingExpression<TSource, TDestination> m, params Expression<Func<TDestination, object>>[] ignore)
        {
            return ignore.Aggregate(m, (c, s) => c.ForMember(s, opt => opt.Ignore()));
        }

        public static IMappingExpression<TSource, TDestination> Map<TSource, TDestination, TSourceMember>(this IMappingExpression<TSource, TDestination> m, Expression<Func<TDestination, object>> destinationMember, Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            return m.ForMember(destinationMember, opt => opt.MapFrom(sourceMember));
        }
    }
}