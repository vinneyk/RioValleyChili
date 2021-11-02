using System;
using System.Linq;
using System.Reflection;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Core.Helpers.AutoFixture
{
    [ExtractIntoSolutionheadLibrary]
    public class QueryableCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new QueryableSpecimenBuilder());
        }

        private class QueryableSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var type = request as Type;
                if (type == null || !type.IsInterface || !IsQueryable(type))
                {
                    return new NoSpecimen();
                }

                var args = type.GetGenericArguments();
                return GetType().GetMethod("BuildQueryable", BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(args)
                                .Invoke(null, new object[] {context});
            }

            private static bool IsQueryable(Type type)
            {   
                return type.GetInterfaces().Any(i => i.Name == "IQueryable");
            }

            private static IQueryable<TObject> BuildQueryable<TObject>(ISpecimenContext context)
            {
                return Queryable.AsQueryable<TObject>(context.CreateMany<TObject>());
            }
        }
    }
}