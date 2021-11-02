using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using RioValleyChili.Core;
using RioValleyChili.Data;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using Solutionhead.EntityParser;
using Solutionhead.TestFoundations;
using Solutionhead.TestFoundations.Utilities;

namespace RioValleyChili.Services.Tests.Helpers
{
    public sealed class RVCIntegrationTestHelper : DbContextIntegrationTestHelper<RioValleyChiliDataContext>
    {
        public Solutionhead.EntityParser.EntityParser EntityParser { get; protected set; }
        public bool OmmitEnumerable { get; set; }

        public RVCIntegrationTestHelper(bool dropAndRecreateDatabase = true)
        {
            ResetContext();
            if(dropAndRecreateDatabase)
            {
                DropAndRecreateContext();
            }

            EntityParser = new Solutionhead.EntityParser.EntityParser(Context);
            ForeignKeyConstrainer = new EntityObjectGraphForeignKeyConstrainer(EntityParser.Entities);
            OmmitEnumerable = true;
            ResetObjectInstantiator();
        }

        public void Reset()
        {
            Context.Database.ExecuteSqlCommand(@"
                EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
                EXEC sp_MSForEachTable 'DELETE FROM ?'
                EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'
                EXEC sp_MSForEachTable @command1 =
                'IF EXISTS (SELECT * from syscolumns where id = Object_ID(''?'') and colstat & 1 = 1)
                BEGIN
                    DBCC CHECKIDENT(''?'', RESEED, 0)
                END'");
            ResetContext();
            ResetObjectInstantiator();
        }

        public override RioValleyChiliDataContext ResetContext()
        {
            DisposeOfContext();
            Context = new RioValleyChiliDataContext();
            //Context.Configuration.ValidateOnSaveEnabled = false;
            //Context.Configuration.AutoDetectChangesEnabled = false;
            //Context.Configuration.ProxyCreationEnabled = false;

            return Context;
        }

        public void ResetObjectInstantiator()
        {
            ObjectInstantiator = new ObjectInstantiator();
            if(OmmitEnumerable)
            {
                ObjectInstantiator.Fixture.Customizations.Add(new OmitEnumerableSpecimenBuilder());
            }
            ObjectInstantiator.Fixture.Customizations.Add(new WeightPropertySpecimenBuilder(1000));
            ObjectInstantiator.Fixture.Customizations.Add(new QuantityPropertySpecimenBuilder(10000));
            ObjectInstantiator.Fixture.Customizations.Add(new StringPropertySpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new ToteKeyPropetySpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new AttributeKeyPropetySpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new DateTimePropertySpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new CompanySpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new PackScheduleSpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new ChileLotProductionSpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new InventoryShipmentOrderSpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new DehydratedMaterialsReceivedSpecimenBuilder());
            ObjectInstantiator.Fixture.Customizations.Add(new LocationSpecimenBuilder());

            ObjectInstantiator.Fixture.Customize<AdditiveLot>(c => c.Do(a =>
                {
                    if(a.Lot != null)
                    {
                        a.Lot.ChileLot = null;
                        a.Lot.PackagingLot = null;
                    }
                }));
            ObjectInstantiator.Fixture.Customize<ChileLot>(c => c.Do(a =>
                {
                    if(a.Lot != null)
                    {
                        a.Lot.AdditiveLot = null;
                        a.Lot.PackagingLot = null;
                    }
                }));
            ObjectInstantiator.Fixture.Customize<PackagingLot>(c => c.Do(a =>
                {
                    if(a.Lot != null)
                    {
                        a.Lot.ChileLot = null;
                        a.Lot.AdditiveLot = null;
                    }
                }));
        }

        public List<T> List<T>(uint count, Action<List<T>> init = null) where T : class
        {
            var list = new List<T>();
            for(var i = 0; i < count; ++i)
            {
                list.Add(CreateObjectGraph<T>());
            }

            if(init != null)
            {
                init(list);
            }

            return list;
        }

        public List<T> List<T>(uint count, params Action<T, int>[] init) where T : class
        {
            var list = new List<T>();
            for(var i = 0; i < count; ++i)
            {
                var item = CreateObjectGraph<T>();
                if(init != null)
                {
                    foreach(var initialization in init)
                    {
                        initialization(item, i);
                    }
                }
                list.Add(item);
            }

            return list;
        }

        protected override void DropAndRecreateContext()
        {
            if(Context != null && Context.Database.Exists())
            {
                Context.Database.Delete();
            }

            ResetContext();
        }

        private class OmitEnumerableSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null
                    && CachedType<IEnumerable>.Type.IsAssignableFrom(propertyInfo.PropertyType)
                    && propertyInfo.PropertyType != CachedType<string>.Type)
                {
                    return new OmitSpecimen();
                }

                return new NoSpecimen(request);
            }
        }

        private class ToteKeyPropetySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(propertyInfo.Name == "ToteKey" && propertyInfo.PropertyType == CachedType<string>.Type)
                    {
                        var @string = context.CreateAnonymous<string>();
                        return @string.Replace(';', '-');
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class AttributeKeyPropetySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(propertyInfo.Name == "ShortName" && propertyInfo.PropertyType == CachedType<string>.Type)
                    {
                        var @string = context.CreateAnonymous("attr");
                        return @string.Replace(';', '-').Substring(0, 10);
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class StringPropertySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    var stringLengthAttribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                    if(stringLengthAttribute != null)
                    {
                        var @string = context.CreateAnonymous<string>().Replace('-', '_');
                        if(@string.Length > stringLengthAttribute.MaximumLength)
                        {
                            @string = @string.Substring(0, stringLengthAttribute.MaximumLength);
                        }
                        return @string;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class DateTimePropertySpecimenBuilder : ISpecimenBuilder
        {
            private static readonly DateTime SqlMinDate = new DateTime(1753, 1, 1);
            private static readonly Random Random = new Random();

            private static DateTime? _lastProductionBegin, _lastProductionEnd;

            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null && propertyInfo.PropertyType == CachedType<DateTime>.Type)
                {
                    var date = context.CreateAnonymous<DateTime>();
                    date = new DateTime(Random.Next(2000, 2099), date.Month, Random.Next(1, 28), date.Hour, date.Minute, date.Second, date.Millisecond);

                    var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                    if(columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.TypeName) && columnAttribute.TypeName.ToUpper() == "DATETIME2")
                    {
                        return date;
                    }

                    date = date.Date;
                    if(date < SqlMinDate)
                    {
                        date = SqlMinDate.AddDays((date - DateTime.MinValue).Days);
                    }

                    if(propertyInfo.DeclaringType == CachedType<LotProductionResults>.Type)
                    {
                        if(propertyInfo.Name == "ProductionBegin")
                        {
                            if(_lastProductionEnd == null)
                            {
                                _lastProductionEnd = date.AddMinutes(120);
                            }
                            return _lastProductionBegin = _lastProductionEnd.Value.AddMinutes(-60);
                            
                        }

                        if(propertyInfo.Name == "ProductionEnd")
                        {
                            if(_lastProductionBegin == null)
                            {
                                _lastProductionBegin = date;
                            }
                            return _lastProductionEnd = _lastProductionBegin.Value.AddMinutes(120);
                        }
                    }

                    return date;
                }

                return new NoSpecimen(request);
            }
        }

        private class QuantityPropertySpecimenBuilder : ISpecimenBuilder
        {
            private readonly int _max ;

            public QuantityPropertySpecimenBuilder(int max)
            {
                _max = max;
            }

            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null && propertyInfo.PropertyType == CachedType<int>.Type && propertyInfo.Name.Contains("Quantity"))
                {
                    var value = context.CreateAnonymous<int>();
                    if(value > _max)
                    {
                        value -= (value / _max) * _max;
                    }

                    return value;
                }

                return new NoSpecimen(request);
            }
        }

        private class WeightPropertySpecimenBuilder : ISpecimenBuilder
        {
            private readonly double _max;

            public WeightPropertySpecimenBuilder(double max)
            {
                _max = max;
            }

            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null && propertyInfo.PropertyType == CachedType<double>.Type && propertyInfo.Name.Contains("Weight"))
                {
                    var value = context.CreateAnonymous<double>();
                    if(value > _max)
                    {
                        value -= ((int) (value / _max)) * _max;
                    }

                    return value;
                }

                return new NoSpecimen(request);
            }
        }

        private class CompanySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(Check<Data.Models.Company>.IsMember(propertyInfo, c => c.Customer))
                    {
                        return null;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class PackScheduleSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    var packScheduleType = CachedType<PackSchedule>.Type;
                    if(propertyInfo.DeclaringType == packScheduleType && propertyInfo.Name == "Customer")
                    {
                        return new OmitSpecimen();
                    }
                    if(propertyInfo.DeclaringType == packScheduleType && propertyInfo.Name == "CustomerId")
                    {
                        return new OmitSpecimen();
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class ChileLotProductionSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(propertyInfo.PropertyType == CachedType<ChileLotProduction>.Type)
                    {
                        var production = context.CreateAnonymous<ChileLotProduction>();
                        if(propertyInfo.DeclaringType == CachedType<ProductionBatch>.Type)
                        {
                            production.ProductionType = ProductionType.ProductionBatch;
                        }
                        return production;
                    }

                    if(propertyInfo.PropertyType == CachedType<ProductionType>.Type && propertyInfo.DeclaringType == CachedType<ChileLotProduction>.Type)
                    {
                        return ProductionType.MillAndWetdown;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class InventoryShipmentOrderSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(Check<InventoryShipmentOrder>.IsMember(propertyInfo, i => i.OrderType))
                    {
                        return InventoryShipmentOrderTypeEnum.InterWarehouseOrder;
                    }

                    if(Check<TreatmentOrder>.IsMember(propertyInfo, i => i.InventoryShipmentOrder))
                    {
                        var order = context.CreateAnonymous<InventoryShipmentOrder>();
                        order.OrderType = InventoryShipmentOrderTypeEnum.TreatmentOrder;
                        return order;
                    }

                    if(Check<SalesOrder>.IsMember(propertyInfo, i => i.InventoryShipmentOrder))
                    {
                        var order = context.CreateAnonymous<InventoryShipmentOrder>();
                        order.OrderType = InventoryShipmentOrderTypeEnum.SalesOrder;
                        return order;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class DehydratedMaterialsReceivedSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(Check<ChileMaterialsReceived>.IsMember(propertyInfo, i => i.ChileProduct))
                    {
                        var product = context.CreateAnonymous<ChileProduct>();
                        product.ChileState = ChileStateEnum.Dehydrated;
                        return product;
                    }

                    if(Check<ChileMaterialsReceived>.IsMember(propertyInfo, i => i.Supplier))
                    {
                        var company = context.CreateAnonymous<Data.Models.Company>();
                        if(company.CompanyTypes == null)
                        {
                            company.CompanyTypes = new List<CompanyTypeRecord>();
                        }
                        if(company.CompanyTypes.All(t => t.CompanyTypeEnum != CompanyType.Dehydrator))
                        {
                            company.CompanyTypes.Add(new CompanyTypeRecord
                                {
                                    CompanyTypeEnum = CompanyType.Dehydrator
                                });
                        }
                        return company;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private class LocationSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if(propertyInfo != null)
                {
                    if(Check<Location>.IsMember(propertyInfo, l => l.Active))
                    {
                        return true;
                    }

                    if(Check<Location>.IsMember(propertyInfo, l => l.Locked))
                    {
                        return false;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private static class Check<TSource>
        {
            public static bool IsMember<TMember>(MemberInfo memberInfo, Expression<Func<TSource, TMember>> selector)
            {
                var memberExpression = selector.Body as MemberExpression;
                return memberExpression != null && memberExpression.Member == memberInfo;
            }
        }

        private static class CachedType<T>
        {
            public static readonly Type Type = typeof(T);
        }
    }

    public sealed class RioAccessSQLIntegrationTestHelper : ObjectContextIntegrationTestHelper<RioAccessSQLEntities>
    {
        public Solutionhead.EntityParser.EntityParser EntityParser { get; protected set; }

        public RioAccessSQLIntegrationTestHelper(bool dropAndRecreateDatabase = true)
        {
            ResetContext();
            if(dropAndRecreateDatabase)
            {
                DropAndRecreateDatabase();
            }

            EntityParser = new Solutionhead.EntityParser.EntityParser(Context);
            ForeignKeyConstrainer = new EntityObjectGraphForeignKeyConstrainer(EntityParser.Entities);
            ObjectInstantiator = new ObjectInstantiator();
        }

        public override RioAccessSQLEntities ResetContext()
        {
            DisposeOfContext();
            Context = new RioAccessSQLEntities();
            return Context;
        }

        protected override void DropAndRecreateContext()
        {
            if(Context != null)
            {
                if(Context.DatabaseExists())
                {
                    Context.DeleteDatabase();
                }
                Context.CreateDatabase();
            }
        }
    }
}
