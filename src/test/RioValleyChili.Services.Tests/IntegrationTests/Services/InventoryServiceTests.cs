using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.Helpers.ParameterExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class InventoryServiceTests : ServiceIntegrationTestBase<InventoryService>
    {
        public const double EPSILON = 0.000001;
        
        [TestFixture]
        public class GetInventory : InventoryServiceTests
        {
            protected override bool SetupStaticRecords { get { return false; } }

            [Test]
            public void Returns_expected_Active_AttributeNames_grouped_by_type()
            {
                //Arrange
                StartStopwatch();
                var unexpectedAttributeName = new KeyValuePair<string, string>("00", "Bad Attribute");
                var expectedChileAttributeName0 = new KeyValuePair<string, string>("c0", "Chile Attribute Name 0");
                var expectedChileAttributeName1 = new KeyValuePair<string, string>("c1", "Chile Attribute Name 1");
                var expectedAdditiveAttributeName0 = new KeyValuePair<string, string>("a0", "Additive AttributeName 0");
                var expectedAdditiveAttributeName1 = new KeyValuePair<string, string>("a1", "AdditiveAttributeName 1");
                var expectedPackagingAttributeName0 = new KeyValuePair<string, string>("p0", "PackagingAttributeName 0");
                var expectedPackagingAttributeName1 = new KeyValuePair<string, string>("p1", "PackagingAttributeName 1");
                var expectedChileAdditiveAttributeName = new KeyValuePair<string, string>("ad0", "Chile Additive Attribute");

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(unexpectedAttributeName, false));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedChileAttributeName0, true, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedChileAttributeName1, true, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedAdditiveAttributeName0, true, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedAdditiveAttributeName1, true, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedPackagingAttributeName0, true, false, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedPackagingAttributeName1, true, false, false, true));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.SetValues(expectedChileAdditiveAttributeName, true, true, true));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var results = result.ResultingObject == null ? null : result.ResultingObject.AttributeNamesByProductType.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.IsTrue(results.All(a => a.Value.Count(n => n.Key == unexpectedAttributeName.Key) == 0));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Chile).Value.Count(n => n.Equals(expectedChileAttributeName0)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Chile).Value.Count(n => n.Equals(expectedChileAttributeName1)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Additive).Value.Count(n => n.Equals(expectedAdditiveAttributeName0)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Additive).Value.Count(n => n.Equals(expectedAdditiveAttributeName1)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Packaging).Value.Count(n => n.Equals(expectedPackagingAttributeName0)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Packaging).Value.Count(n => n.Equals(expectedPackagingAttributeName1)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Chile).Value.Count(n => n.Equals(expectedChileAdditiveAttributeName)));
                Assert.AreEqual(1, results.Single(a => a.Key == ProductTypeEnum.Additive).Value.Count(n => n.Equals(expectedChileAdditiveAttributeName)));
            } 

            [Test]
            public void Returns_expected_Inventory_Keys()
            {
                //Arrange
                StartStopwatch();
                var inventoryKeys = new List<string>();
                for(var i = 0; i < 3; ++i)
                {
                    inventoryKeys.Add(new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>()));
                }
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var inventory = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                inventoryKeys.ForEach(k => Assert.AreEqual(1, inventory.Count(i => i.InventoryKey == k)));
            }

            [Test]
            public void Returns_Inventory_with_expected_Attribute_values()
            {
                //Arrange
                StartStopwatch();
                const string chileAttributeName0 = "Chile 0";
                const string chileAttributeName1 = "Chile 1";
                const string additiveAttributeName0 = "Additive 0";
                const string additiveAttributeName1 = "Additive 1";
                var chileAdditiveAttributeName0 = new KeyValuePair<string, string>("cad0", "Chile Additive");

                var chileAttribute0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, chileAttributeName0, true, true));
                var chileAttribute1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, chileAttributeName1, true, true));
                var additiveAttribute0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, additiveAttributeName0, true, false, true));
                var additiveAttribute1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(null, additiveAttributeName1, true, false, true));
                var chileAdditiveAttribute0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetValues(chileAdditiveAttributeName0, true, true, true));

                const double chile0Attribute0 = 0.5;
                const double chile0Attribute2 = 99.09;
                var chileInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.Attributes = null,
                    i => i.Lot.SetChileLot().ChileLot.Production = null, i => i.Lot.ChileLot.ChileProduct.Product.ProductType = ProductTypeEnum.Chile, i => i.Lot.ChileLot.ChileProduct.ProductAttributeRanges = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileInventoryKey0, chileAttribute0, chile0Attribute0));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileInventoryKey0, chileAdditiveAttribute0, chile0Attribute2));

                const double chile1Attribute0 = 0.0001;
                const double chile1Attribute1 = 1234.56;
                var chileInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.Attributes = null, i => i.Lot.ChileLot.Production = null,
                    i => i.Lot.SetChileLot().ChileLot.ChileProduct.Product.ProductType = ProductTypeEnum.Chile, i => i.Lot.ChileLot.ChileProduct.ProductAttributeRanges = null));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileInventoryKey1, chileAttribute0, chile1Attribute0));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileInventoryKey1, chileAttribute1, chile1Attribute1));

                const double additive0Attribute0 = 1;
                const double additive0Attribute1 = 2;
                const double additive0Attribute2 = 3;
                var additiveInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.Attributes = null, i => i.Lot.SetAdditiveLot().AdditiveLot.AdditiveProduct.Product.ProductType = ProductTypeEnum.Additive));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(additiveInventoryKey0, additiveAttribute0, additive0Attribute0));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(additiveInventoryKey0, additiveAttribute1, additive0Attribute1));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(additiveInventoryKey0, chileAdditiveAttribute0, additive0Attribute2));

                var packagingInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetPackagingLot().Attributes = null));

                TestHelper.ResetContext();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var inventory = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                var inventoryItem = inventory.Single(i => i.InventoryKey == chileInventoryKey0.KeyValue);
                Assert.AreEqual(chile0Attribute0, inventoryItem.Attributes.Single(a => a.Name == chileAttributeName0).Value);
                Assert.AreEqual(null, inventoryItem.Attributes.SingleOrDefault(a => a.Name == chileAttributeName1));
                Assert.AreEqual(chile0Attribute2, inventoryItem.Attributes.Single(a => a.Key.Equals(chileAdditiveAttributeName0.Key)).Value);

                inventoryItem = inventory.Single(i => i.InventoryKey == chileInventoryKey1.KeyValue);
                Assert.AreEqual(chile1Attribute0, inventoryItem.Attributes.Single(a => a.Name == chileAttributeName0).Value);
                Assert.AreEqual(chile1Attribute1, inventoryItem.Attributes.Single(a => a.Name == chileAttributeName1).Value);
                Assert.AreEqual(null, inventoryItem.Attributes.SingleOrDefault(a => a.Key == chileAdditiveAttributeName0.Key));

                inventoryItem = inventory.Single(i => i.InventoryKey == additiveInventoryKey0.KeyValue);
                Assert.AreEqual(additive0Attribute0, inventoryItem.Attributes.Single(a => a.Name == additiveAttributeName0).Value);
                Assert.AreEqual(additive0Attribute1, inventoryItem.Attributes.Single(a => a.Name == additiveAttributeName1).Value);
                Assert.AreEqual(additive0Attribute2, inventoryItem.Attributes.Single(a => a.Key == chileAdditiveAttributeName0.Key).Value);

                inventoryItem = inventory.Single(i => i.InventoryKey == packagingInventoryKey0.KeyValue);
                Assert.IsEmpty(inventoryItem.Attributes);
            }

            [Test]
            public void Returns_Inventory_of_specific_Lot_if_LotKey_is_provided()
            {
                //Arrange
                StartStopwatch();
                const int expectedNumberOfInventoryKeys = 3;
                TestHelper.ObjectInstantiator.Fixture.RepeatCount = expectedNumberOfInventoryKeys;

                var expectedLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null),
                        TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null),
                        TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null)
                    });
                var expectedLotKey = new LotKey(expectedLot);
                var expectedInventoryKeys = expectedLot.Inventory.Select(i => new InventoryKey(i)).ToList();
                Assert.AreEqual(expectedNumberOfInventoryKeys, expectedInventoryKeys.Count);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        LotKey = expectedLotKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Asset
                result.AssertSuccess();
                Assert.AreEqual(expectedNumberOfInventoryKeys, results.Count);
                expectedInventoryKeys.ForEach(k => Assert.AreEqual(1, results.Count(r => r.InventoryKey == k.KeyValue)));
                results.ForEach(i => Assert.AreEqual(expectedLotKey.KeyValue, i.LotKey));
            }

            [Test]
            public void Returns_Invalid_result_if_filtering_by_a_nonexistent_Lot()
            {
                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        LotKey = new LotKey().KeyValue
                    });
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertInvalid(UserMessages.LotNotFound);
            }

            [Test]
            public void Returns_Inventory_of_a_specific_Product_if_ProductKey_is_provided()
            {
                //Arrange
                StartStopwatch();
                const int expectedResults = 2;
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var expectedProductKey = new ChileProductKey(chileProduct);

                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetChileLot().ChileLot.Production = null, i => i.Lot.ChileLot.SetProduct(expectedProductKey)));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetChileLot().ChileLot.Production = null, i => i.Lot.ChileLot.SetProduct(expectedProductKey)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production = null, c => c.Lot.SetChileLot().Inventory = new List<Inventory> { TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null) });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(a => a.Lot.SetChileLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingLot>(p => p.Lot.SetPackagingLot());
                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedResults);
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        ProductKey = expectedProductKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(expectedProductKey.KeyValue, results.Single(r => r.InventoryKey == inventoryKey0.KeyValue).LotProduct.ProductKey);
                Assert.AreEqual(expectedProductKey.KeyValue, results.Single(r => r.InventoryKey == inventoryKey1.KeyValue).LotProduct.ProductKey);
            }

            [Test]
            public void Returns_Inventory_as_expected_given_an_InventoryTreatmentKey()
            {
                //Arrange
                StartStopwatch();
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        TreatmentKey = new InventoryTreatmentKey(expected)
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new InventoryKey(expected).KeyValue, results.Single().InventoryKey);
            }

            [Test]
            public void Returns_Inventory_as_expected_given_a_PackagingKey()
            {
                //Arrange
                StartStopwatch();
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        PackagingKey = new PackagingProductKey(expected.PackagingProduct)
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new InventoryKey(expected).KeyValue, results.Single().InventoryKey);
            }

            [Test]
            public void Returns_Inventory_as_expected_given_a_PackagingReceivedKey()
            {
                //Arrange
                StartStopwatch();
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        PackagingReceivedKey = new PackagingProductKey(expected.Lot.ReceivedPackaging)
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new InventoryKey(expected).KeyValue, results.Single().InventoryKey);
            }

            [Test]
            public void Returns_Inventory_as_expected_given_a_LocationKey()
            {
                //Arrange
                StartStopwatch();
                var expected = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        LocationKey = new LocationKey(expected.Location)
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(new InventoryKey(expected).KeyValue, results.Single().InventoryKey);
            }

            [Test]
            public void Returns_Inventory_in_a_specific_Warehouse_if_WarehouseKey_is_provided()
            {
                //Arrange
                StartStopwatch();
                const int expectedResults = 3;

                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(w => w.Locations = null));
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(warehouseKey)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(warehouseKey)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(warehouseKey)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Production = null, c => c.Lot.Inventory = new List<Inventory> { TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null) });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(a => a.Lot.Inventory = new List<Inventory> { TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null) });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingLot>(p => p.Lot.Inventory = new List<Inventory> { TestHelper.CreateObjectGraph<Inventory>(i => i.Lot = null) });
                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedResults);
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        FacilityKey = warehouseKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(warehouseKey.KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey0.KeyValue).Location.FacilityKey);
                Assert.AreEqual(warehouseKey.KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey1.KeyValue).Location.FacilityKey);
                Assert.AreEqual(warehouseKey.KeyValue, results.Single(r => r.InventoryKey == expectedInventoryKey2.KeyValue).Location.FacilityKey);
            }

            [Test]
            public void Returns_Inventory_of_a_specific_ProductType_if_ProductType_is_provided()
            {
                //Arrange
                StartStopwatch();
                const int expectedResults = 3;
                const ProductTypeEnum productType = ProductTypeEnum.Chile;
                
                var inventoryKey0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetChileLot().ChileLot.Production = null, i => i.Lot.ChileLot.ChileProduct.Product.ProductType = productType).ToInventoryKey();
                var inventoryKey1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetChileLot().ChileLot.Production = null, i => i.Lot.ChileLot.ChileProduct.Product.ProductType = productType).ToInventoryKey();
                var inventoryKey2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetChileLot().ChileLot.Production = null, i => i.Lot.ChileLot.ChileProduct.Product.ProductType = productType).ToInventoryKey();

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetAdditiveLot(), i => i.Lot.AdditiveLot.AdditiveProduct.Product.ProductType = ProductTypeEnum.Additive);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetPackagingLot(), i => i.Lot.PackagingLot.PackagingProduct.Product.ProductType = ProductTypeEnum.Packaging);
                
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        ProductType = productType
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(productType, results.Single(r => r.InventoryKey == inventoryKey0.KeyValue).LotProduct.ProductType);
                Assert.AreEqual(productType, results.Single(r => r.InventoryKey == inventoryKey1.KeyValue).LotProduct.ProductType);
                Assert.AreEqual(productType, results.Single(r => r.InventoryKey == inventoryKey2.KeyValue).LotProduct.ProductType);
            }

            [Test]
            public void Returns_Inventory_of_a_specific_LotType_if_provided()
            {
                //Arrange
                StartStopwatch();
                const LotTypeEnum expectedLotType = LotTypeEnum.Other;
                const int expectedResults = 3;

                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType));

                var unexpectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = LotTypeEnum.GRP));
                var unexpectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = LotTypeEnum.Raw));
                var unexpectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = LotTypeEnum.Additive));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        LotType = expectedLotType
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();

                Assert.AreEqual(expectedResults, results.Count());
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey0.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey1.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey2.KeyValue));

                Assert.AreEqual(0, results.Count(r => r.InventoryKey == unexpectedInventoryKey0.KeyValue));
                Assert.AreEqual(0, results.Count(r => r.InventoryKey == unexpectedInventoryKey1.KeyValue));
                Assert.AreEqual(0, results.Count(r => r.InventoryKey == unexpectedInventoryKey2.KeyValue));
            }

            [Test]
            public void Returns_Inventory_as_expected_given_Lot_and_Warehouse_Keys()
            {
                //Arrange
                StartStopwatch();
                const int expectedResults = 3;
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.Inventory = null));
                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(w => w.Locations = null));

                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lotKey, null, null, null, warehouseKey)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lotKey, null, null, null, warehouseKey)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lotKey, null, null, null, warehouseKey)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lotKey));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, warehouseKey));
                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedResults);
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        LotKey = lotKey.KeyValue,
                        FacilityKey = warehouseKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                var inventory = results.Single(i => i.InventoryKey == expectedInventoryKey0.KeyValue);
                Assert.AreEqual(lotKey.KeyValue, inventory.LotKey);
                Assert.AreEqual(warehouseKey.KeyValue, inventory.Location.FacilityKey);

                inventory = results.Single(i => i.InventoryKey == expectedInventoryKey1.KeyValue);
                Assert.AreEqual(lotKey.KeyValue, inventory.LotKey);
                Assert.AreEqual(warehouseKey.KeyValue, inventory.Location.FacilityKey);

                inventory = results.Single(i => i.InventoryKey == expectedInventoryKey2.KeyValue);
                Assert.AreEqual(lotKey.KeyValue, inventory.LotKey);
                Assert.AreEqual(warehouseKey.KeyValue, inventory.Location.FacilityKey);
            }

            [Test]
            public void Returns_Inventory_as_expected_given_Product_and_Warehouse_Keys()
            {
                //Arrange
                StartStopwatch();
                const int expectedResults = 3;
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(w => w.Locations = null));

                var chileLotKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetDerivedLot().Lot.Inventory = null, c => c.SetProduct(chileProductKey), c => c.Production = null));
                var chileLotKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.SetDerivedLot().Lot.Inventory = null, c => c.SetProduct(chileProductKey), c => c.Production = null));

                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLotKey0, null, null, null, warehouseKey)));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLotKey1, null, null, null, warehouseKey)));
                var inventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLotKey1, null, null, null, warehouseKey)));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLotKey0));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, warehouseKey));
                Assert.Greater(TestHelper.Context.Set<Inventory>().Count(), expectedResults);
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        ProductKey = chileProductKey.KeyValue,
                        FacilityKey = warehouseKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                var inventory = results.Single(r => r.InventoryKey == inventoryKey0.KeyValue);
                Assert.AreEqual(chileProductKey.KeyValue, inventory.LotProduct.ProductKey);
                Assert.AreEqual(warehouseKey.KeyValue, inventory.Location.FacilityKey);

                inventory = results.Single(r => r.InventoryKey == inventoryKey1.KeyValue);
                Assert.AreEqual(chileProductKey.KeyValue, inventory.LotProduct.ProductKey);
                Assert.AreEqual(warehouseKey.KeyValue, inventory.Location.FacilityKey);

                inventory = results.Single(r => r.InventoryKey == inventoryKey2.KeyValue);
                Assert.AreEqual(chileProductKey.KeyValue, inventory.LotProduct.ProductKey);
                Assert.AreEqual(warehouseKey.KeyValue, inventory.Location.FacilityKey);
            }

            [Test]
            public void Returns_Inventory_as_expected_given_LotType_and_Warehouse_key()
            {
                //Arrange
                StartStopwatch();
                const LotTypeEnum expectedLotType = LotTypeEnum.Other;
                const int expectedResults = 3;

                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(w => w.Locations = null));
                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType, i => i.Location.ConstrainByKeys(warehouseKey)));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType, i => i.Location.ConstrainByKeys(warehouseKey)));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType, i => i.Location.ConstrainByKeys(warehouseKey)));
                var unexpectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.LotTypeEnum = expectedLotType));
                var unexpectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.ConstrainByKeys(warehouseKey)));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters
                    {
                        LotType = expectedLotType,
                        FacilityKey = warehouseKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey0.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey1.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey2.KeyValue));

                Assert.AreEqual(0, results.Count(r => r.InventoryKey == unexpectedInventoryKey0.KeyValue));
                Assert.AreEqual(0, results.Count(r => r.InventoryKey == unexpectedInventoryKey1.KeyValue));
            }

            [Test]
            public void Returns_Inventory_as_expected_given_a_ToteKey()
            {
                //Arrange
                StartStopwatch();
                const int expectedInventory = 3;
                var toteKey = "Tonari no Totetotero".ToToteKey();

                var expectedInventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ToteKey = toteKey));
                var expectedInventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ToteKey = toteKey));
                var expectedInventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ToteKey = toteKey));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory(new FilterInventoryParameters { ToteKey = toteKey });
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedInventory, results.Count);
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey0.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey1.KeyValue));
                Assert.AreEqual(1, results.Count(r => r.InventoryKey == expectedInventoryKey2.KeyValue));
            }

            [Test]
            public void Returns_Inventory_as_expected_when_combining_ToteKey_with_other_filtering_options()
            {
                //Arrange
                StartStopwatch();
                var toteKey = "Tonari no Totetotero".ToToteKey();

                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>());
                var inventoryByWarehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, warehouseKey).ToteKey = toteKey);

                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());
                var inventoryByLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lot).ToteKey = toteKey);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ToteKey = toteKey);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(lot));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, warehouseKey));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var withWarehouseResult = Service.GetInventory(new FilterInventoryParameters { ToteKey = toteKey, FacilityKey = warehouseKey.KeyValue });
                var withWarehouseSingle = withWarehouseResult.ResultingObject == null ? null : withWarehouseResult.ResultingObject.Inventory.Single();

                var withLotResult = Service.GetInventory(new FilterInventoryParameters { ToteKey = toteKey, LotKey = new LotKey(lot).KeyValue });
                var withLotSingle = withLotResult.ResultingObject == null ? null : withLotResult.ResultingObject.Inventory.Single();
                StopWatchAndWriteTime("Act");

                //Assert
                withWarehouseResult.AssertSuccess();
                Assert.AreEqual(new InventoryKey(inventoryByWarehouse).KeyValue, withWarehouseSingle.InventoryKey);

                withLotResult.AssertSuccess();
                Assert.AreEqual(new InventoryKey(inventoryByLot).KeyValue, withLotSingle.InventoryKey);
            }

            [Test]
            public void Throws_exception_if_attempting_to_filter_resulting_query_by_a_key()
            {
                //Arrange
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>());

                //Act
                var result = Service.GetInventory();

                //Assert
                result.AssertSuccess();
                try
                {
                    var inventory = result.ResultingObject.Inventory.FirstOrDefault(i => i.InventoryKey == inventoryKey.KeyValue);
                }
                catch(Exception ex)
                {
                    Assert.Pass(ex.Message);
                }
                Assert.Fail("Did not catch exception.");
            }

            [Test]
            public void Does_not_throw_exception_if_attempting_to_filter_resulting_query_by_WarehouseName()
            {
                //Arrange
                StartStopwatch();
                const string warehouseName = "Roman's Warehouse of Magical Wonders";
                var warehouseKey = new FacilityKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(w => w.Locations = null, w => w.Name = warehouseName));
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(null, null, null, null, warehouseKey)));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var inventorySummaryReturn = result.ResultingObject == null ? null : result.ResultingObject.Inventory.Single(i => i.Location.FacilityName == warehouseName);
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(inventoryKey.KeyValue, inventorySummaryReturn.InventoryKey);
            }

            [Test]
            public void Returns_AstaCalc_property_of_null_if_Inventory_has_no_associated_ChileLot_record()
            {
                //Arrange
                StartStopwatch();
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>());
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var inventory = result.ResultingObject == null ? null : result.ResultingObject.Inventory.Single();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(inventoryKey.KeyValue, inventory.InventoryKey);
                Assert.IsNull(inventory.AstaCalc);
            }

            [Test]
            public void Returns_AstaCalc_property_of_null_if_Inventory_is_of_ChileLot_with_no_Asta_attribute()
            {
                //Arrange
                StartStopwatch();
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production = null);
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot)));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var inventory = result.ResultingObject == null ? null : result.ResultingObject.Inventory.Single();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(inventoryKey.KeyValue, inventory.InventoryKey);
                Assert.IsNull(inventory.AstaCalc);
            }

            [Test]
            public void Returns_AstaCalc_property_as_expected_if_Inventory_is_of_ChileLot_with_an_Asta_attribute()
            {
                //Arrange
                StartStopwatch();
                const double originalAsta = 120.0;
                var now = DateTime.UtcNow;
                var productionEnd = now.AddDays(-200);
                var expectedAstaCalc = AstaCalculator.CalculateAsta(originalAsta, productionEnd, productionEnd, now);

                var asta = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.SetKey(GlobalKeyHelpers.AstaAttributeNameKey).SetValues(null, "Asta", true));
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.EmptyLot(), c => c.Production.PickedInventory.EmptyItems(), c => c.Production.Results.EmptyItems().ProductionEnd = productionEnd);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<LotAttribute>(a => a.SetValues(chileLot, asta, originalAsta).AttributeDate = productionEnd);

                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(chileLot)));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var inventory = result.ResultingObject == null ? null : result.ResultingObject.Inventory.Single();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(inventoryKey.KeyValue, inventory.InventoryKey);
                Assert.AreEqual(expectedAstaCalc, inventory.AstaCalc);
            }

            [Test]
            public void Returns_LoBac_property_as_expected()
            {
                //Arrange
                StartStopwatch();
                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.SetAdditiveLot()));

                var loBacChileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.SetChileLot().EmptyLot(), c => c.Production = null, c => c.AllAttributesAreLoBac = true);
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(loBacChileLot)));

                var notLoBacChileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.SetChileLot().EmptyLot(), c => c.Production = null, c => c.AllAttributesAreLoBac = false);
                var inventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.ConstrainByKeys(notLoBacChileLot)));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.IsNull(results.Single(i => i.InventoryKey == inventoryKey0.KeyValue).LoBac);
                Assert.IsTrue((bool) results.Single(i => i.InventoryKey == inventoryKey1.KeyValue).LoBac);
                Assert.IsFalse((bool) results.Single(i => i.InventoryKey == inventoryKey2.KeyValue).LoBac);
            }

            [Test]
            public void Returns_Lot_status_properties_as_expected()
            {
                //Arrange
                StartStopwatch();
                const LotQualityStatus lotStatus0 = LotQualityStatus.Contaminated;
                const bool onHold0 = false;
                const LotProductionStatus status0 = LotProductionStatus.Batched;

                const LotQualityStatus lotStatus1 = LotQualityStatus.Released;
                const bool onHold1 = true;
                const LotProductionStatus status1 = LotProductionStatus.Produced;
                
                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.QualityStatus = lotStatus0, i => i.Lot.ProductionStatus = status0, i => i.Lot.Hold = null));
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.QualityStatus = lotStatus1, i => i.Lot.ProductionStatus = status1));
                StopWatchAndWriteTime("Arrange");

                //Act
                StartStopwatch();
                var result = Service.GetInventory();
                var results = result.ResultingObject == null ? null : result.ResultingObject.Inventory.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                var inventory = results.Single(r => r.InventoryKey == inventoryKey0.KeyValue);
                Assert.AreEqual(inventoryKey0.KeyValue, inventory.InventoryKey);
                Assert.AreEqual(lotStatus0, inventory.QualityStatus);
                Assert.AreEqual(onHold0, inventory.HoldType != null);
                Assert.AreEqual(status0, inventory.ProductionStatus);

                inventory = results.Single(r => r.InventoryKey == inventoryKey1.KeyValue);
                Assert.AreEqual(inventoryKey1.KeyValue, inventory.InventoryKey);
                Assert.AreEqual(lotStatus1, inventory.QualityStatus);
                Assert.AreEqual(onHold1, inventory.HoldType != null);
                Assert.AreEqual(status1, inventory.ProductionStatus);
            }

            [Test, Issue("Since LocationGroupNames are a virtual concept the flitering is implemented by matching on equality or beginning with the group name appended with the location description separator character." +
                         "Note that filtering is case-insensitive, which I believe is as desired anyways. -RI 2016-12-27",
                         References = new[] { "RVCADMIN-1438" })]
            public void Returns_Inventory_filtered_by_LocationGroupName()
            {
                //Arrange
                var expected = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.Description = "Test~1"),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.Description = "test~2"),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.Description = "test~3"),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.Description = "test"),
                    };
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.Description = "no...");
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Location.Description = "testing");

                //Act
                var result = Service.GetInventory(new FilterInventoryParameters { LocationGroupName = "Test" });

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.Inventory.ToList();

                expected.AssertEquivalent(results, e => e.ToInventoryKey().KeyValue, r => r.InventoryKey);
            }
        }

        [TestFixture]
        public class CalculateAttributeWeightedAverages : InventoryServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_inventoryKeysAndQuantities_is_empty()
            {
                //Act
                var result =  Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>());

                //Assert
                result.AssertNotSuccess(UserMessages.EmptySet);
            }

            [Test]
            public void Returns_non_successful_result_if_any_quantity_supplied_is_not_greater_than_0()
            {
                //Arrange
                var inventoryKey0 = new InventoryKey(TestHelper.CreateObjectGraph<Inventory>());
                var inventoryKey1 = new InventoryKey(TestHelper.CreateObjectGraph<Inventory>());
                var inventoryKey2 = new InventoryKey(TestHelper.CreateObjectGraph<Inventory>());
                var inventoryKey3 = new InventoryKey(TestHelper.CreateObjectGraph<Inventory>());
                
                //Act
                var result = Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>
                    {
                        { inventoryKey0.KeyValue, 1234 },
                        { inventoryKey1.KeyValue, 44321 },
                        { inventoryKey2.KeyValue, 0 },
                        { inventoryKey3.KeyValue, 777 },
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
            }

            [Test]
            public void Returns_non_successful_result_if_the_Lot_of_a_given_InventoryKey_could_not_be_found()
            {
                //Arrange
                var inventoryKey = new InventoryKey(TestHelper.CreateObjectGraph<Inventory>());

                //Act
                var result = Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>
                    {
                        { inventoryKey.KeyValue, 123 }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.LotNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_the_PackagingProduct_of_a_given_InventoryKey_could_not_be_found()
            {
                //Arrange
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                var inventoryKey = new InventoryKey(inventory, TestHelper.CreateObjectGraph<PackagingProduct>(), inventory, inventory, inventory.ToteKey);

                //Act
                var result = Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>
                    {
                        { inventoryKey.KeyValue, 123 }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.PackagingProductNotFound);
            }

            [Test]
            public void Returns_expected_results_for_a_single_Inventory_record()
            {
                //Arrange
                var valueSeed = 10d;
                var expectedAttributes = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => valueSeed += 0.27d);
                
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Lot.Attributes = expectedAttributes.Select(e =>
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))
                    ).ToList());

                //Act
                var result = Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>
                    {
                        { new InventoryKey(inventory).KeyValue, 12 }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedAttributes.Count, result.ResultingObject.Count());
                foreach(var expected in expectedAttributes)
                {
                    var resultAttribute = result.ResultingObject.Single(r => r.Key == expected.Key.Name);
                    Assert.AreEqual(expected.Value, resultAttribute.Value, EPSILON);
                }
            }

            [Test]
            public void Returns_expected_results_for_a_single_PickedInventoryItem_record_without_existing_Inventory()
            {
                //Arrange
                var valueSeed = 10d;
                var expectedAttributes = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => valueSeed += 0.27d);

                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.Lot.Attributes = expectedAttributes.Select(e =>
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))).ToList());

                //Act
                var result = Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>
                    {
                        { new InventoryKey(inventory).KeyValue, 12 }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedAttributes.Count, result.ResultingObject.Count());
                foreach(var expected in expectedAttributes)
                {
                    var resultAttribute = result.ResultingObject.Single(r => r.Key == expected.Key.Name);
                    Assert.AreEqual(expected.Value, resultAttribute.Value, EPSILON);
                }
            }

            [Test]
            public void Returns_expected_results_given_multiple_items_to_average()
            {
                //Arrange
                    //Setup constants
                const double packagingWeight0 = 25;
                const double packagingWeight1 = 100;
                const double packagingWeight2 = 55;

                const int itemQuantity0 = 2;
                const int itemQuantity1 = 10;
                const int itemQuantity2 = 5;

                var attributeSeed = 10d;
                var item0Attributes = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => attributeSeed += 23d);
                attributeSeed = 12d;
                var item1Attributes = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => attributeSeed += 21d);
                attributeSeed = 14d;
                var item2Attributes = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a => attributeSeed += 20d);

                    //Expected results
                const double item0weight = packagingWeight0 * itemQuantity0;
                const double item1weight = packagingWeight1 * itemQuantity1;
                const double item2weight = packagingWeight2 * itemQuantity2;
                const double totalWeight = item0weight + item1weight + item2weight;

                var expectedResults = StaticAttributeNames.AttributeNames.ToDictionary(a => a, a =>
                        ((item0Attributes[a] * item0weight) +
                        (item1Attributes[a] * item1weight) +
                        (item2Attributes[a] * item2weight)) / totalWeight);

                    //Setup data
                var packaging0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = packagingWeight0);
                var packaging1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = packagingWeight1);
                var packaging2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Weight = packagingWeight2);

                var item0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.Lot.Attributes = item0Attributes.Select(e =>
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))).ToList());
                var item1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.Lot.Attributes = item1Attributes.Select(e =>
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))).ToList());
                var item2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PickedInventoryItem>(i => i.Lot.Attributes = item2Attributes.Select(e =>
                        TestHelper.CreateObjectGraph<LotAttribute>(a => a.SetValues(null, e.Key, e.Value))).ToList());
                
                //Act
                var result = Service.CalculateAttributeWeightedAverages(new Dictionary<string, int>
                    {
                        { new InventoryKey(item0, packaging0, item0, item0, item0.ToteKey).KeyValue, itemQuantity0 },
                        { new InventoryKey(item1, packaging1, item1, item1, item1.ToteKey).KeyValue, itemQuantity1 },
                        { new InventoryKey(item2, packaging2, item2, item2, item2.ToteKey).KeyValue, itemQuantity2 }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults.Count, result.ResultingObject.Count());
                foreach(var expected in expectedResults)
                {
                    var resultAttribute = result.ResultingObject.Single(r => r.Key == expected.Key.Name);
                    Assert.AreEqual(expected.Value, resultAttribute.Value, EPSILON);
                }
            }
        }

        [TestFixture]
        public class ReceiveInventoryTests : InventoryServiceTests
        {
            private static readonly Expression<Func<Lot, object>>[] AssertIncludePaths = new Expression<Func<Lot, object>>[]
                {
                    l => l.AdditiveLot, l => l.PackagingLot, l => l.ChileLot,
                    l => l.ReceivedPackaging, l => l.Employee, l => l.Inventory,
                    l => l.Vendor
                };

            [Test]
            public void Creates_Lot_and_Inventory_records_as_expected_on_success()
            {
                //Arrange
                var vendor = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>();

                var parameters = new ReceiveInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        LotType = LotTypeEnum.Additive,
                        ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>().ToProductKey(),
                        PackagingReceivedKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey(),
                        VendorKey = vendor.ToCompanyKey(),
                        PurchaseOrderNumber = "Purchordie",
                        ShipperNumber = "shipping4life",
                        Items = new List<ReceiveInventoryItemParameters>
                            {
                                new ReceiveInventoryItemParameters
                                    {
                                        Quantity = 1,
                                        PackagingProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey(),
                                        WarehouseLocationKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>().ToLocationKey(),
                                        TreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>().ToInventoryTreatmentKey(),
                                        ToteKey = "TOTE",
                                    }
                            }
                    };

                //Act
                var result = Service.ReceiveInventory(parameters);

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(KeyParserHelper.ParseResult<ILotKey>(result.ResultingObject).ResultingObject.ToLotKey(), AssertIncludePaths);
                parameters.AssertAsExpected(lot);
            }

            [Test]
            public void Creates_Lot_and_Inventory_records_as_expected_on_success_given_null_PackagingReceived_key()
            {
                //Arrange
                var parameters = new ReceiveInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        LotType = LotTypeEnum.Packaging,
                        ProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToProductKey(),
                        Items = new List<ReceiveInventoryItemParameters>
                            {
                                new ReceiveInventoryItemParameters
                                    {
                                        Quantity = 1,
                                        PackagingProductKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>().ToPackagingProductKey(),
                                        WarehouseLocationKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>().ToLocationKey(),
                                        TreatmentKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>().ToInventoryTreatmentKey(),
                                        ToteKey = "TOTE"
                                    }
                            }
                    };

                //Act
                var result = Service.ReceiveInventory(parameters);

                //Assert
                result.AssertSuccess();
                var lot = RVCUnitOfWork.LotRepository.FindByKey(KeyParserHelper.ParseResult<ILotKey>(result.ResultingObject).ResultingObject.ToLotKey(), AssertIncludePaths);
                parameters.AssertAsExpected(lot);
            }
        }

        [TestFixture]
        public class GetInventoryReceivedTests : InventoryServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_records_are_found()
            {
                //Act
                var result = Service.GetInventoryReceived(new GetInventoryReceivedParameters());

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.IsEmpty(results);
            }

            [Test]
            public void Returns_only_transactions_of_type_InventoryReceived()
            {
                //Arrange
                var expected = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory)
                    };
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.CreatedMillAndWetdown);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.InternalMovement);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.InventoryAdjustment);

                //Act
                var result = Service.GetInventoryReceived(new GetInventoryReceivedParameters());

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                expected.AssertEquivalent(results, e => e.SourceLot.ToLotKey().KeyValue, r => r.SourceLotKey, (e, r) =>
                    {
                        Assert.AreEqual(InventoryTransactionType.ReceiveInventory, r.TransactionType);
                        Assert.AreEqual(e.Quantity, r.Quantity);
                    });
            }

            [Test]
            public void Returns_transactions_of_specified_lot_types()
            {
                //Arrange
                var expected = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.GRP),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.GRP),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.GRP)
                    };
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLot.SetAdditiveLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLot.SetPackagingLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.WIP);

                //Act
                var result = Service.GetInventoryReceived(new GetInventoryReceivedParameters
                    {
                        LotType = LotTypeEnum.GRP
                    });

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                expected.AssertEquivalent(results, e => e.SourceLot.ToLotKey().KeyValue, r => r.SourceLotKey, (e, r) => e.AssertEqual(r));
            }

            [Test]
            public void Returns_transaction_for_specific_lot()
            {
                //Arrange
                var expected = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.GRP)
                    };
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.GRP);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.SourceLotTypeEnum = LotTypeEnum.GRP);

                //Act
                var result = Service.GetInventoryReceived(new GetInventoryReceivedParameters
                    {
                        LotKey = expected.Single().SourceLot.ToLotKey()
                    });

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                expected.AssertEquivalent(results, e => e.SourceLot.ToLotKey().KeyValue, r => r.SourceLotKey, (e, r) => e.AssertEqual(r));
            }

            [Test]
            public void Returns_transactions_created_in_specified_date_range()
            {
                //Arrange
                var expected = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.DateCreated = new DateTime(2016, 1, 10)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.DateCreated = new DateTime(2016, 1, 11)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.DateCreated = new DateTime(2016, 1, 12))
                    };
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.DateCreated = new DateTime(2016, 1, 9));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.TransactionType = InventoryTransactionType.ReceiveInventory, t => t.DateCreated = new DateTime(2016, 1, 13));

                //Act
                var result = Service.GetInventoryReceived(new GetInventoryReceivedParameters
                    {
                        DateReceivedStart = new DateTime(2016, 1, 10),
                        DateReceivedEnd = new DateTime(2016, 1, 12)
                    });

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                expected.AssertEquivalent(results, e => e.SourceLot.ToLotKey().KeyValue, r => r.SourceLotKey, (e, r) => e.AssertEqual(r));
            }
        }

        [TestFixture]
        public class GetInventoryTransactionsTests : InventoryServiceTests
        {
            [Test]
            public void Returns_InventoryTransactions_for_Lot_with_expected_LotKeys()
            {
                //Arrange
                var sourceLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var destinationLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var transactions = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetSourceLot(sourceLot).SetDestinationLot(null)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetSourceLot(sourceLot).SetDestinationLot(null)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetSourceLot(sourceLot).SetDestinationLot(destinationLot))
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryTransactions(sourceLot.ToLotKey());
                var results = result.Success ? result.ResultingObject.ToList() : null;
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(transactions.Count, results.Count);
                foreach(var transaction in transactions)
                {
                    var match = results.Single(r => r.TimeStamp == transaction.TimeStamp);
                    Assert.AreEqual(transaction.SourceLot.ToLotKey().KeyValue, match.SourceLotKey);
                    if(transaction.DestinationLot == null)
                    {
                        Assert.IsNull(match.DestinationLotKey);
                    }
                    else
                    {
                        Assert.AreEqual(transaction.DestinationLot.ToLotKey().KeyValue, match.DestinationLotKey);
                    }
                }
            }

            [Test]
            public void Returns_InventoryTransactions_for_Lot_picked_for_ProductionBatch_without_results()
            {
                //Arrange
                var sourceLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var destinationLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>();
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.Results = null,
                    b => b.Production.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(1, (i, n) => i.ConstrainByKeys(null, sourceLot)));
                var transactions = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetSourceLot(sourceLot).SetDestinationLot(null)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetSourceLot(sourceLot).SetDestinationLot(null)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetSourceLot(sourceLot).SetDestinationLot(destinationLot))
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryTransactions(sourceLot.ToLotKey());
                var results = result.Success ? result.ResultingObject.ToList() : null;
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(transactions.Count + 1, results.Count);
                foreach(var transaction in transactions)
                {
                    var match = results.Single(r => r.TimeStamp == transaction.TimeStamp);
                    Assert.AreEqual(transaction.SourceLot.ToLotKey().KeyValue, match.SourceLotKey);
                    if(transaction.DestinationLot == null)
                    {
                        Assert.IsNull(match.DestinationLotKey);
                    }
                    else
                    {
                        Assert.AreEqual(transaction.DestinationLot.ToLotKey().KeyValue, match.DestinationLotKey);
                    }
                }

                Assert.IsNotNull(results.SingleOrDefault(r => r.DestinationLotKey == productionBatch.ToLotKey().KeyValue));
            }
        }

        [TestFixture]
        public class GetLotInputTransactionsTests : InventoryServiceTests
        {
            [Test]
            public void Returns_empty_transactions_list_if_no_input_transactions_exist()
            {
                //Arrange
                var lot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>();

                //Act
                var result = Service.GetInventoryTransactionsByDestinationLot(lot.ToLotKey());

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject.InputItems);
            }

            [Test]
            public void Returns_InventoryTransactions_for_Lot_with_expected_LotKeys()
            {
                //Arrange
                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.Lot.SetChileLot());
                var inputs = new List<InventoryTransaction>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetDestinationLot(chileLot)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetDestinationLot(chileLot)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTransaction>(t => t.SetDestinationLot(chileLot))
                    };

                //Act
                StartStopwatch();
                var result = Service.GetInventoryTransactionsByDestinationLot(chileLot.ToLotKey());
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(inputs.Count, result.ResultingObject.InputItems.Count());
                Assert.AreEqual(chileLot.ChileProduct.Product.ToProductKey().KeyValue, result.ResultingObject.Product.ProductKey);
                foreach(var transaction in inputs)
                {
                    var match = result.ResultingObject.InputItems.Single(r => r.TimeStamp == transaction.TimeStamp);
                    Assert.AreEqual(transaction.SourceLot.ToLotKey().KeyValue, match.SourceLotKey);
                    if(transaction.DestinationLot == null)
                    {
                        Assert.IsNull(match.DestinationLotKey);
                    }
                    else
                    {
                        Assert.AreEqual(chileLot.ToLotKey().KeyValue, match.DestinationLotKey);
                    }
                }
            }

            [Test]
            public void Returns_InventoryTransaction_records_for_ProductionBatch_without_results()
            {
                //Arrange
                PickedInventoryItem pickedItem = null;
                var productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.Production.Results = null,
                    b => b.Production.PickedInventory.Items = TestHelper.List<PickedInventoryItem>(1, (i, n) => pickedItem = i));

                //Act
                var result = Service.GetInventoryTransactionsByDestinationLot(productionBatch.ToLotKey());

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(pickedItem.ToLotKey().KeyValue, result.ResultingObject.InputItems.Single().SourceLotKey);
            }
        }
    }
}