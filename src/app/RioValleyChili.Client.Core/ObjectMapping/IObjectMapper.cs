using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace RioValleyChili.Client.Core.ObjectMapping
{
    public interface IObjectMapper
    {
        TDestination MapObject<TSource, TDestination>(TSource sourceObject)
            where TSource : class
            where TDestination : class;

        void AssertIsConfigurationValid(string profileName = null);
    }
}
