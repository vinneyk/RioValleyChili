using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;

namespace RioValleyChili.Tests.IntegrationTests.AutoMapper
{
    [TestFixture]
    public class ObjectMapperConfigurationTests
    {
        private static readonly Type NullableDefinition = typeof(Nullable<>);

        [Test]
        public void Is_Configuration_Valid()
        {
            Client.Mvc.AutoMapperConfiguration.Configure();
            Client.Reporting.AutoMapperConfiguration.Configure();
            
            var abstractFound = false;
            foreach(var mapping in Mapper.GetAllTypeMaps())
            {
                foreach(var propertyMap in (mapping.GetPropertyMaps() ?? new PropertyMap[0]).ToList())
                {
                    CheckPropertyMap(propertyMap);
                }

                if(mapping.DestinationType.IsAbstract && mapping.CustomMapper == null && mapping.DestinationCtor == null)
                {
                    abstractFound = true;
                    Console.WriteLine("- Abstract DestinationType '{0}' without customer mapper or destination constructor.", mapping.DestinationType);
                }
                else
                {
                    if(mapping.SourceType.IsValueType && mapping.DestinationType.IsValueType
                        && mapping.SourceType.IsGenericType && mapping.SourceType.GetGenericTypeDefinition() == NullableDefinition
                        && mapping.SourceType == NullableDefinition.MakeGenericType(mapping.DestinationType))
                    {
                        AssertNullableMapping(mapping.DestinationType);
                    }
                    else if(!mapping.SourceType.IsGenericTypeDefinition)
                    {
                        if(mapping.SourceType.IsInterface)
                        {
                            AssertInterfaceMapping(mapping.SourceType, mapping.DestinationType);
                        }
                        else if(!mapping.SourceType.IsAbstract)
                        {
                            AssertMapping(mapping.SourceType, mapping.DestinationType, AutoFixtureHelper.BuildFixture());
                        }
                    }
                }
            }

            if(abstractFound)
            {
                Assert.Fail("Abstract destination type(s) with no custom mapping found.");
            }
        }

        private static void CheckPropertyMap(PropertyMap propertyMap)
        {
            var sourceType = GetMemberType(propertyMap.SourceMember);
            if(sourceType == null)
            {
                return;
            }
            
            var destinationType = propertyMap.DestinationPropertyType;
            if(destinationType == null)
            {
                return;
            }

            if(sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == NullableDefinition)
            {
                if(sourceType.GetGenericArguments().FirstOrDefault() == destinationType)
                {
                    if(propertyMap.CustomExpression == null)
                    {
                        Assert.Fail("Nullable source mapped to non-nullable destination without a custom expression. Results may not be as expected. Add a custom expression if this was intended.\n {0}.{1} -> {2}.{3}",
                            propertyMap.SourceMember.DeclaringType.Name, propertyMap.SourceMember.Name,
                            propertyMap.DestinationProperty.MemberInfo.DeclaringType.Name, propertyMap.DestinationProperty.MemberInfo.Name);
                    }
                }
            }
        }

        private static void AssertMapping(Type source, Type destination, IFixture fixture)
        {
            ExerciseMappingMethodInfo.MakeGenericMethod(source, destination).Invoke(null, new object[] { fixture });
        }

        private static void AssertInterfaceMapping(Type sourceInterface, Type destination)
        {
            ExerciseMappedInterfaceMethodInfo.MakeGenericMethod(sourceInterface, destination).Invoke(null, null);
        }

        private static void AssertNullableMapping(Type destination)
        {
            ExerciseNullableMappingMethodInfo.MakeGenericMethod(destination).Invoke(null, null);
        }

        private static Type GetMemberType(MemberInfo memberInfo)
        {
            if(memberInfo == null)
            {
                return null;
            }

            switch(memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default: return null;
            }
        }

// ReSharper disable UnusedMember.Local
        private static readonly MethodInfo ExerciseMappingMethodInfo = typeof(ObjectMapperConfigurationTests).GetMethod("ExerciseMapping", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static object ExerciseMapping<TSource, TDestination>(IFixture fixture)
        {
            var source = fixture.Create<TSource>();
            var mapResult = Mapper.Map<TSource, TDestination>(source);
            return mapResult;
        }

        private static readonly MethodInfo ExerciseMappedInterfaceMethodInfo = typeof(ObjectMapperConfigurationTests).GetMethod("ExerciseMappedInterface", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static object ExerciseMappedInterface<TInterface, TDestination>()
            where TInterface : class
        {
            var mock = new Mock<TInterface>
                {
                    DefaultValue = DefaultValue.Mock
                };
            mock.SetupAllProperties();
            var mapResult = Mapper.Map<TInterface, TDestination>(mock.Object);
            return mapResult;
        }

        private static readonly MethodInfo ExerciseNullableMappingMethodInfo = typeof(ObjectMapperConfigurationTests).GetMethod("ExerciseNullableMapping", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static object ExerciseNullableMapping<TValueType>()
            where TValueType : struct
        {
            TValueType? source;

            var valueType = typeof(TValueType);
            if(valueType.IsEnum)
            {
                var enumValues = Enum.GetValues(valueType).Cast<object>().ToList();
                if(!enumValues.Any())
                {
                    return null;
                }

                source = enumValues.Contains(0) ? default(TValueType) : enumValues.Cast<TValueType>().First();
            }
            else
            {
                source = default(TValueType);
            }

            var mapResult = Mapper.Map<TValueType?, TValueType>(source);
            return mapResult;
        }
// ReSharper restore UnusedMember.Local
    }
}
;