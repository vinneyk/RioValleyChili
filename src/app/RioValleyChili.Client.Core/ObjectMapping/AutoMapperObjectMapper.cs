using AutoMapper;

namespace RioValleyChili.Client.Core.ObjectMapping
{
    public class AutoMapperObjectMapper : IObjectMapper
    {
        public TDestination MapObject<TSource, TDestination>(TSource sourceObject)
            where TSource : class
            where TDestination : class
        {
            return Mapper.Map<TSource, TDestination>(sourceObject);
        }

        public void AssertIsConfigurationValid(string profileName = null)
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
