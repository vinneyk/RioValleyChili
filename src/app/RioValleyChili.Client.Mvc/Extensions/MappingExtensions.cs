using AutoMapper;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;

namespace RioValleyChili.Client.Mvc.Extensions
{
    public static class MappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapLinkedResource<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
            where TDestination : ILinkedResource
        {
            mappingExpression.ForMember(m => m.Links, opt => opt.Ignore());
            return mappingExpression;
        }
    }
}