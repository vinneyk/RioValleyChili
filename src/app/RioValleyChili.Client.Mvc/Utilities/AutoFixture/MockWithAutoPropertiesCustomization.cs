using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Utilities.AutoFixture
{
    // websites referencing this issue:
    // https://autofixture.codeplex.com/workitem/4245
    // http://pol84.tumblr.com/post/23032897078/autofixture-and-interfaces
    // http://stackoverflow.com/questions/12949417/how-to-let-autofixture-create-an-instance-of-a-type-that-contains-properties-wit

    [ExtractIntoSolutionheadLibrary]
    public class MockWithAutoPropertiesCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new MockWithAutoPropertiesSpecimenBuilder());
        }

        private class MockWithAutoPropertiesSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var type = request as Type;
                if (type == null || !type.IsInterface || type.IsGenericType)
                {
                    return new NoSpecimen(request);
                }

                object specimen = GetType()
                    .GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(new[] { type })
                    .Invoke(this, new object[] { context });

                return specimen;
            }

            private static object Create<TRequest>(ISpecimenContext context)
                where TRequest : class
            {
                var mock = new Mock<TRequest>();

                typeof(TRequest).GetProperties().ToList()
                    .ForEach(p => SetPropertyValue(p, mock, context));

                // NOTE: When creating a specimen from an interface, if the interface extends another interface,
                // only the properties declared by the specified interface (TRequest) will appear here. 
                //todo: recurs over GetInterfaces() results?
                typeof(TRequest).GetInterfaces().ToList()
                    .ForEach(i => i.GetProperties().ToList()
                        .ForEach(p => SetPropertyValue(p, mock, context)));

                return mock.Object;
            }

            private static void SetPropertyValue<TRequest>(PropertyInfo property, Mock<TRequest> mock, ISpecimenContext context)
                where TRequest : class
            {
                var value = context.Resolve(property);
                if (property.CanWrite) {
                    property.SetValue(mock.Object, value);
                } else {
                    SetupPropertyWithMock(mock, property, value);
                }   
            }

            private static void SetupPropertyWithMock<TRequest>(Mock<TRequest> mock, PropertyInfo property, object value) 
                where TRequest : class
            {
                var expressionParamater = Expression.Parameter(typeof(TRequest));
                var propertyAccessor = Expression.MakeMemberAccess(expressionParamater, property);
                var expression =  Expression.Lambda(propertyAccessor, expressionParamater);

                typeof (MockWithAutoPropertiesSpecimenBuilder)
                    .GetMethod("SetupGet", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(typeof(TRequest), property.PropertyType)
                    .Invoke(null, new object[] { mock, expression, value });
            }

            private static void SetupGet<TRequest, TProperty>(Mock<TRequest> mock, Expression<Func<TRequest, TProperty>> memberExpression, TProperty value) 
                where TRequest : class
            {
                mock.SetupGet(memberExpression).Returns(value);
            }
        }
    }
}