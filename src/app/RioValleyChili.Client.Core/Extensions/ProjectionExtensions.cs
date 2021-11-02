using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Core.Extensions
{
    [ExtractIntoSolutionheadLibrary]
    public static class ProjectionExtensions
    {
        public static IProjectionExpression Map<TSource>(this TSource source)
        {
            return new AutoMapperMappingExpression<TSource>(source);
        }

        public static IEnumerableProjectionExpression Project<TSource>(this IEnumerable<TSource> source)
        {
            return new AutoMappingEnumerableProjectionExpression<TSource>(source);
        }

        public static IEnumerableProjectionExpression Project<TSource>(this IQueryable<TSource> source, Action<TSource> preProcess)
        {
            return new AutoMappingEnumeratedQueryableProjectionExpression<TSource>(source, preProcess);
        }
    }

    public class AutoMappingEnumerableProjectionExpression<TSource> : IEnumerableProjectionExpression
    {
        private readonly IEnumerable<TSource> _source;

        public AutoMappingEnumerableProjectionExpression(IEnumerable<TSource> source)
        {
            if(source == null) throw new ArgumentNullException("source");
            _source = source;
        }

        public IEnumerable<TDest> To<TDest>()
        {
            return Mapper.Map<IEnumerable<TSource>, IEnumerable<TDest>>(_source);
        }
    }

    public class AutoMapperMappingExpression<TSource> : IProjectionExpression
    {
        private readonly TSource _source;

        public AutoMapperMappingExpression(TSource source)
        {
            _source = source;
        }

        public TDest To<TDest>()
        {
            return Mapper.Map<TSource, TDest>(_source);
        }
    }

    public class AutoMappingEnumeratedQueryableProjectionExpression<TSource> : IEnumerableProjectionExpression
    {
        private readonly IQueryable<TSource> _source;
        private readonly Action<TSource> _preProcess;

        public AutoMappingEnumeratedQueryableProjectionExpression(IQueryable<TSource> source, Action<TSource> preProcess = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            _source = source;
            _preProcess = preProcess;
        }

        public IEnumerable<TDest> To<TDest>()
        {
            foreach(var result in _source)
            {
                if(_preProcess != null)
                {
                    _preProcess(result);
                }

                yield return Mapper.Map<TSource, TDest>(result);
            }
        }
    }
    
    public interface IProjectionExpression
    {
        TResult To<TResult>();
    }

    public interface IEnumerableProjectionExpression
    {
        IEnumerable<TResult> To<TResult>();
    }
}