//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using EF_Projectors;
//using EF_Projectors.Extensions;
//using EF_Split_Projector;
//using LinqKit;
//using NUnit.Framework;
//using RioValleyChili.Business.Core.Helpers;
//using RioValleyChili.Business.Core.Keys;
//using RioValleyChili.Data;
//using RioValleyChili.Data.Models;
//using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
//using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
//using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
//using RioValleyChili.Services.Utilities.Helpers;
//using RioValleyChili.Services.Utilities.LinqProjectors;
//using RioValleyChili.Services.Utilities.Models;
//using RioValleyChili.Services.Utilities.Models.KeyReturns;

//namespace RioValleyChili.Services.Tests.UnitTests
//{
//    [TestFixture]
//    public class SplitQueryTests : ServiceIntegrationTestBase
//    {
//        private const int NumberOfRecords = 5;
//        private const int Skip = 10;
//        private const int Take = 10;
//        private static int ExpectedRecords { get { return Math.Min(Take, Math.Max(0, (NumberOfRecords - Skip))); } }

//        [Test]
//        public void TestCurrent()
//        {
//            Arrange();

//            StartStopwatch();
//            var result = RegularQuery(RVCUnitOfWork)
//                .Where(l => l.PackagingProduct.Weight > 10.0f)
//                .OrderBy(l => l.LotDateCreated).ThenBy(l => l.Quantity)
//                .Skip(Skip).Take(Take).ToList();
//            StopWatchAndWriteTime("Act");

//            Assert.AreEqual(ExpectedRecords, result.Count);
//            if(ExpectedRecords > 0)
//            {
//                Assert.IsNotNull(result[0]);
//            }
//            else
//            {
//                Assert.IsEmpty(result);
//            }
//        }

//        [Test]
//        public void TestSplit()
//        {
//            Arrange();

//            StartStopwatch();

//            var result = SplitQuery(RVCUnitOfWork).Where(l => l.PackagingProduct.Weight > 10.0f)
//                .OrderBy(l => l.LotDateCreated)
//                .ThenBy(l => l.Quantity)
//                .Skip(Skip)
//                .Take(Take)
//                .ToList();

//            StopWatchAndWriteTime("Act");

//            Assert.AreEqual(ExpectedRecords, result.Count);
//            if(ExpectedRecords > 0)
//            {
//                Assert.IsNotNull(result[0]);
//            }
//            else
//            {
//                Assert.IsEmpty(result);
//            }
//        }

//        [Test]
//        public void BasicCurrentTest()
//        {
//            Arrange();

//            StartStopwatch();
//            var results = RegularQuery(RVCUnitOfWork).ToList();
//            StopWatchAndWriteTime("Act");

//            Assert.AreEqual(NumberOfRecords, results.Count);
//        }

//        [Test]
//        public void BasicSplitTest()
//        {
//            Arrange();

//            StartStopwatch();
//            var results = SplitQuery(RVCUnitOfWork).ToList();
//            StopWatchAndWriteTime("Act");

//            Assert.AreEqual(NumberOfRecords, results.Count);
//        }
        
//        [Test]
//        public void TestSplitFirstOrDefault()
//        {
//            StartStopwatch();
//            for(var x = 0; x < NumberOfRecords; ++x)
//            {
//                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(
//                    i => i.Lot.Attributes = new List<LotAttribute>
//                        {
//                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null),
//                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null)
//                        },
//                    i => i.Lot.LotDefects = new List<LotDefect>
//                        {
//                            TestHelper.CreateObjectGraph<LotDefect>(a => a.Lot = null),
//                            TestHelper.CreateObjectGraph<LotDefect>(a => a.Lot = null)
//                        });
//            }
//            StopWatchAndWriteTime("Arrange");

//            StartStopwatch();
//            var result = SplitQuery(RVCUnitOfWork).OrderBy(l => l.PackagingProduct.ProductCode).FirstOrDefault();
//            StopWatchAndWriteTime("Act");

//            Assert.IsNotNull(result);
//        }

//        [Test]
//        public void MergesResultsAsExpected()
//        {
//            var inventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
//            var inventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
//            TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(inventory1));
//            TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(inventory1));
//            TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(inventory1));
//            var inventory2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();

//            var results = RVCUnitOfWork.InventoryRepository.All().SplitSelect(i => new InventoryTestReturn
//                {
//                    LotDateCreated = i.Lot.LotDateCreated
//                },
//                i => new InventoryTestReturn
//                {
//                    LotAttributes = i.Lot.Attributes.Select(a => a.AttributeShortName)
//                }).ToList();

//            var result = results.Single(i => i.LotDateCreated == inventory0.LotDateCreated);
//            Assert.IsEmpty(result.LotAttributes);

//            result = results.Single(i => i.LotDateCreated == inventory1.LotDateCreated);
//            Assert.IsNotEmpty(result.LotAttributes);

//            result = results.Single(i => i.LotDateCreated == inventory2.LotDateCreated);
//            Assert.IsEmpty(result.LotAttributes);
//        }

//        [Test]
//        public void ExpressionsWithMultipleParametersAreTranslated()
//        {
//            CreateInventoryWithAsta(90.0);
//            var inventory0 = CreateInventoryWithAsta(120.0);
//            var inventory1 = CreateInventoryWithAsta(150.0);
//            var inventory2 = CreateInventoryWithAsta(110.0);
//            CreateInventoryWithAsta(80.0);

//            var split = SplitQuery(RVCUnitOfWork)
//                .Where(i => i.AstaCalc > 100)
//                .OrderByDescending(i => i.AstaCalc);
//            var results = split.ToList();
//            Assert.AreEqual(3, results.Count);
//            Assert.AreEqual(new InventoryKey(inventory1).KeyValue, results[0].InventoryKey);
//            Assert.AreEqual(new InventoryKey(inventory0).KeyValue, results[1].InventoryKey);
//            Assert.AreEqual(new InventoryKey(inventory2).KeyValue, results[2].InventoryKey);
//        }

//        private static void Arrange()
//        {
//            StartStopwatch();
//            for(var x = 0; x < NumberOfRecords; ++x)
//            {
//                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(
//                    i => i.Lot.Attributes = new List<LotAttribute>
//                        {
//                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null),
//                            TestHelper.CreateObjectGraph<LotAttribute>(a => a.Lot = null)
//                        },
//                    i => i.Lot.LotDefects = new List<LotDefect>
//                        {
//                            TestHelper.CreateObjectGraph<LotDefect>(a => a.Lot = null),
//                            TestHelper.CreateObjectGraph<LotDefect>(a => a.Lot = null)
//                        });
//            }
//            StopWatchAndWriteTime("Arrange");
//        }

//        private Inventory CreateInventoryWithAsta(double value)
//        {
//            var astaDate = DateTime.UtcNow;
//            return TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetChileLot().Attributes = new List<LotAttribute>
//                    {
//                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, GlobalKeyHelpers.AstaAttributeNameKey, value).AttributeDate = astaDate)
//                    },
//                i => i.Lot.ChileLot.Production.Results.ProductionEnd = astaDate);
//        }

//        public IQueryable<IInventorySummaryReturn> RegularQuery(EFRVCUnitOfWork unitOfWork)
//        {
//            var select = InventoryProjectors.SplitSelectInventorySummary(unitOfWork, DateTime.UtcNow)
//                .Aggregate(Projector<Inventory>.To(i => new InventoryItemReturn { }), (a, n) => a.Merge(n).ExpandAll());
//            return unitOfWork.InventoryRepository.All().Select(select);
//        }

//        public IQueryable<IInventorySummaryReturn> SplitQuery(EFRVCUnitOfWork unitOfWork)
//        {
//            return unitOfWork.InventoryRepository.All().SplitSelect(InventoryProjectors.SplitSelectInventorySummary(unitOfWork, DateTime.UtcNow));
//        }

//        public class InventoryTestReturn
//        {
//            public DateTime LotDateCreated { get; set; }
//            public IEnumerable<string> LotAttributes { get; set; }
//        }
//    }

//    [TestFixture]
//    public class SplitIncludeTests : ServiceIntegrationTestBase
//    {
//        private const int NumberOfRecords = 50;
//        private List<Inventory> _testData;

//        private void Arrange()
//        {
//            _testData = new List<Inventory>();
//            for(var i = 0; i < NumberOfRecords; ++i)
//            {
//                _testData.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(n => n.Lot.SetChileLot()));
//            }
//            TestHelper.ResetContext();
//        }

//        [Test]
//        public void BaseLine()
//        {
//            Arrange();
//            var selectInventory = SelectInventory().ExpandAll();

//            StartStopwatch();
//            var inventory = RVCUnitOfWork.InventoryRepository.All().Select(selectInventory).ToList();
//            StopWatchAndWriteTime("BaseLine Act");

//            Assert.AreEqual(NumberOfRecords, inventory.Count);
//        }

//        [Test]
//        public void SplitIncludeBASE()
//        {
//            Arrange();
//            var selectInventory = SelectInventory().ExpandAll().Compile();

//            StartStopwatch();
//            var inventorySource0 = RVCUnitOfWork.InventoryRepository.Filter(i => true,
//                i => i.PackagingProduct.Product,
//                i => i.Location.Facility, i => i.Location,
//                i => i.Treatment,
//                i => i.Lot.Attributes.Select(a => a.AttributeName), i => i.Lot.LotDefects.Select(d => d.Resolution)).ToList();
//            StopWatchAndWriteTime("ToList");
            
//            StartStopwatch();
//            var results = inventorySource0.Select(datamodelInventory => selectInventory(datamodelInventory)).ToList();
//            StopWatchAndWriteTime("Select");

//            Assert.AreEqual(NumberOfRecords, results.Count);
//        }

//        [Test]
//        public void Test0()
//        {
//            Arrange();

//            StartStopwatch();
//            var result = RVCUnitOfWork.InventoryRepository.Filter(i => true, i => i.PackagingProduct.Product).ToList();
//            StopWatchAndWriteTime("Act");

//            Assert.IsNotNull(result);
//        }

//        [Test]
//        public void Test1()
//        {
//            Arrange();

//            StartStopwatch();
//            var result = RVCUnitOfWork.InventoryRepository.All().Select(i => new
//                {
//                    i.PackagingProduct.Product.ProductCode
//                }).ToList();
//            StopWatchAndWriteTime("Act");

//            Assert.IsNotNull(result);
//        }

//        [Test]
//        public void A()
//        {
//            TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();

//            StartStopwatch();
//            var result = RVCUnitOfWork.InventoryRepository.All().Select(i => new
//            {
//                i.PackagingProduct.Product.ProductCode
//            }).ToList();
//            StopWatchAndWriteTime("Act");

//            Assert.IsNotNull(result);
//        }

//        internal class InventorySelect
//        {
//            public LotKeyReturn LotKey { get; set; }
//            public PackagingProductReturn PackagingProduct { get; set; }
//            public LocationReturn Location { get; set; }
//            public InventoryTreatmentReturn Treatment { get; set; }
//            public IEnumerable<LotAttributeReturn> LotAttributes { get; set; }
//            public IEnumerable<LotDefectReturn> LotDefects { get; set; }
//        }

//        internal static Expression<Func<Inventory, InventorySelect>> SelectInventory()
//        {
//            var lotKey = LotProjectors.SelectLotKey<Lot>();
//            var packaging = ProductProjectors.SelectPackagingProduct();
//            var location = LocationProjectors.SelectLocation();
//            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

//            var attribute = LotAttributeProjectors.Select();
//            var defect = SelectLotDefect();

//            return i => new InventorySelect
//                {
//                    LotKey = lotKey.Invoke(i.Lot),
//                    PackagingProduct = packaging.Invoke(i.PackagingProduct),
//                    Location = location.Invoke(i.Location),
//                    Treatment = treatment.Invoke(i.Treatment),

//                    LotAttributes = i.Lot.Attributes
//                        .Where(a => a.AttributeShortName == "Asta")
//                        .OrderBy(a => a.AttributeValue)
//                        .Where(a => a.AttributeValue > 0)
//                        //.ThenBy(a => a.key0) <- added before execution
//                        //.ThenBy(a => a.key1) <- added before execution
//                        .Select(a => attribute.Invoke(a)),

//                    LotDefects = i.Lot.LotDefects.Select(d => defect.Invoke(d))
//                };
//        }

//        internal static Expression<Func<LotDefect, LotDefectReturn>> SelectLotDefect()
//        {
//            var lotDefectKey = LotDefectProjectors.SelectKey();
//            var resolution = LotDefectResolutionProjectors.Select();

//            return d => new LotDefectReturn
//            {
//                LotDefectKeyReturn = lotDefectKey.Invoke(d),

//                DefectType = d.DefectType,
//                Description = d.Description,

//                //AttributeDefect = lotAttributeDefect.Invoke(d),
//                Resolution = new[] { d.Resolution }.Where(r => r != null).Select(r => resolution.Invoke(r)).FirstOrDefault(),
//            };
//        }
//    }

//    [TestFixture]
//    public class TypeBuilderTests
//    {
//        public abstract class TestBase<T>
//        {
//            public string BaseStringField;
//            public T BaseGenericField;
//        }

//        [Test]
//        public void Throws_exception_if_supplied_parent_is_generic_definition()
//        {
//            try
//            {
//                var parentType = typeof(TestBase<>);
//                var newFields = new Dictionary<string, Type>
//                {
//                    { "DerivedStringField", typeof(string) },
//                    { "DerivedIntField", typeof(int) }
//                };

//                TypeBuilder.GetDynamicType(newFields, parentType);
//            }
//            catch(Exception ex)
//            {
//                Assert.Pass(ex.Message);
//            }
//            Assert.Fail("Expected exception to be thrown.");
//        }

//        [Test]
//        public void Can_build_type_derived_from_abstract_generic_base_class_with_new_fields()
//        {
//            var parentType = typeof(TestBase<int>);
//            var newFields = new Dictionary<string, Type>
//                {
//                    { "DerivedStringField", typeof(string) },
//                    { "DerivedIntField", typeof(int) }
//                };

//            var anonymousType = TypeBuilder.GetDynamicType(newFields, parentType);

//            Assert.IsTrue(anonymousType.BaseType == parentType);
//            var resultFields = anonymousType.GetFields(BindingFlags.Instance | BindingFlags.Public);
//            newFields.ForEach(f => Assert.IsTrue(resultFields.Any(r => f.Key == r.Name && f.Value == r.FieldType)));
//        }
//    }

//    [TestFixture]
//    public class AnonymousTypeProjection
//    {
//        public class DataModel
//        {
//            public int Integer { get; set; }
//            public string String { get; set; }
//        }

//        [Test]
//        public void Can_build_anonymous_projector()
//        {
//            var sourceProperties = typeof(DataModel).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
//            var lambda = LambdaExpressionHelper.CreateAnonymousSelector(sourceProperties);
//            Assert.NotNull(lambda);
//        }

//        [Test]
//        public void Returns_same_type_for_same_fields_regardless_of_order()
//        {
//            Assert.AreEqual(
//                TypeBuilder.GetDynamicType(new Dictionary<string, Type>
//                {
//                    { "Test", typeof(int) },
//                    { "Test2", typeof(string) }
//                }),
//                TypeBuilder.GetDynamicType(new Dictionary<string, Type>
//                {
//                    { "Test2", typeof(string) },
//                    { "Test", typeof(int) }
//                }));
//        }
//    }
//}