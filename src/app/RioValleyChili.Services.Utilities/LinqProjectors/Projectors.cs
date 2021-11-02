// ReSharper disable RedundantEmptyObjectOrCollectionInitializer
using System;
using System.Linq.Expressions;
using EF_Projectors.Extensions;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    public static class ProjectorExtensions
    {
        public static TranslateBuilder<TBaseSource, TBaseReturn> Translate<TBaseSource, TBaseReturn>(this Expression<Func<TBaseSource, TBaseReturn>> projector)
            where TBaseReturn : new()
        {
            return new TranslateBuilder<TBaseSource, TBaseReturn>(projector);
        }

        public class TranslateBuilder<TBaseSource, TBaseReturn> where TBaseReturn : new()
        {
            private readonly Expression<Func<TBaseSource, TBaseReturn>> _projector;

            public TranslateBuilder(Expression<Func<TBaseSource, TBaseReturn>> projector)
            {
                _projector = projector;
            }

            public Expression<Func<TDerivedSource, TDerivedResult>> To<TDerivedSource, TDerivedResult>(Expression<Func<TDerivedSource, TBaseSource>> selectBaseSource)
                    where TDerivedResult : TBaseReturn, new()
            {
                return _projector.Merge(i => new TDerivedResult { } , selectBaseSource);
            }

            public Expression<Func<TBaseSource, TDerivedResult>> To<TDerivedResult>()
                    where TDerivedResult : TBaseReturn, new()
            {
                return _projector.Merge(i => new TDerivedResult { });
            }
        }
    }
}

// ReSharper restore RedundantEmptyObjectOrCollectionInitializer