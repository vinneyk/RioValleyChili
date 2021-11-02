using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.Helpers.ParameterExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class MaterialsReceivedServiceTests : ServiceIntegrationTestBase<MaterialsReceivedService>
    {
        [TestFixture]
        public class CreateChileMaterialsReceivedTests : MaterialsReceivedServiceTests
        {
            protected InventoryTreatmentKey NoTreatmentKey = new InventoryTreatmentKey(StaticInventoryTreatments.NoTreatment);

            [Test]
            public void Returns_non_successful_result_if_ItemsReceived_is_null_or_empty()
            {
                //Act
                var resultNull = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters());
                var resultEmpty = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        ChileProductKey = new ChileProductKey(),
                        TreatmentKey = new InventoryTreatmentKey(),
                        SupplierKey = new CompanyKey(),
                        Items = new List<ChileMaterialsReceivedItemParameters>()
                    });

                //Assert
                resultNull.AssertNotSuccess(UserMessages.CannotCreateChileReceivedWithNoItems);
                resultEmpty.AssertNotSuccess(UserMessages.CannotCreateChileReceivedWithNoItems);
            }

            [Test]
            public void Returns_non_successful_result_if_Dehydrator_Company_is_not_of_CompanyType_Dehydrator()
            {
                //Arrange
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                
                var supplier = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Supplier));

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                        UserToken = TestUser.UserName,
                        DateReceived = new DateTime(2012, 3, 29),
                        ChileProductKey = chileProductKey,
                        SupplierKey = supplier.ToCompanyKey(),
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        LoadNumber = "123",
                        PurchaseOrder = "MLAH!",
                        ShipperNumber = "1029384756",
                        Items = new List<ChileMaterialsReceivedItemParameters>
                                {
                                    new ChileMaterialsReceivedItemParameters
                                        {
                                            Quantity = 10,
                                            PackagingProductKey = packagingProductKey.KeyValue,
                                            LocationKey = warehouseLocationKey.KeyValue,
                                            Variety = "Variety"
                                        }
                                }
                    });

                //Assert
                result.AssertNotSuccess(string.Format(UserMessages.CompanyNotOfType, "{0}", "{1}", CompanyType.Dehydrator));
            }

            [Test]
            public void Creates_DehydratedMaterialsReceived_record_as_expected_on_success()
            {
                //Arrange
                var expectedLotDate = new DateTime(2012, 3, 29);
                const int expectedLotDateSequence = 1;
                const int expectedLotTypeId = (int) LotTypeEnum.DeHydrated;
                const string expectedLoad = "123";
                const string expectedPurchaseOrder = "MLAH!";
                const string expectedShipperNumber = "1029384756";

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var locationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var supplierKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                        UserToken = TestUser.UserName,
                        DateReceived = expectedLotDate,
                        ChileProductKey = chileProductKey,
                        SupplierKey = supplierKey,
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        LoadNumber = expectedLoad,
                        PurchaseOrder = expectedPurchaseOrder,
                        ShipperNumber = expectedShipperNumber,
                        Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = 10,
                                        PackagingProductKey = packagingProductKey,
                                        LocationKey = locationKey,
                                        Variety = "Variety"
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var received = RVCUnitOfWork.ChileMaterialsReceivedRepository.Filter(d => true, d => d.ChileLot.Lot, d => d.Items).Single();
                Assert.AreEqual(expectedLotDate, received.LotDateCreated);
                Assert.AreEqual(expectedLotDateSequence, received.LotDateSequence);
                Assert.AreEqual(expectedLotTypeId, received.LotTypeId);

                Assert.AreEqual(expectedLoad, received.LoadNumber);
                Assert.AreEqual(expectedPurchaseOrder, received.ChileLot.Lot.PurchaseOrderNumber);
                Assert.AreEqual(expectedShipperNumber, received.ChileLot.Lot.ShipperNumber);

                Assert.IsNotNull(received.ChileLot);
                Assert.IsNotNull(received.Items.SingleOrDefault());
            }

            [Test]
            public void Creates_Lot_record_with_LotProductionStatus_set_to_Produced()
            {
                //Arrange
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        DateReceived = new DateTime(2012, 3, 29),
                        ChileProductKey = chileProductKey,
                        SupplierKey = dehydratorKey,
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        LoadNumber = "123",
                        PurchaseOrder = "MLAH!",
                        ShipperNumber = "1029384756",
                        Items = new List<ChileMaterialsReceivedItemParameters>
                                {
                                    new ChileMaterialsReceivedItemParameters
                                        {
                                            Quantity = 10,
                                            PackagingProductKey = packagingProductKey.KeyValue,
                                            LocationKey = warehouseLocationKey.KeyValue,
                                            Variety = "Variety"
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();

                var dehydratedReceived = RVCUnitOfWork.ChileMaterialsReceivedRepository.Filter(d => true, d => d.ChileLot.Lot, d => d.Items).Single();
                Assert.AreEqual(LotProductionStatus.Produced, dehydratedReceived.ChileLot.Lot.ProductionStatus);
            }

            [Test]
            public void Creates_DehydratedMaterialsReceivedItem_records_as_expected_on_sucess()
            {
                //Arrange
                var expectedLotDate = new DateTime(2012, 3, 29);
                const int expectedLotDateSequence = 1;
                const int expectedLotTypeId = (int)LotTypeEnum.DeHydrated;
                const int expectedItems = 4;
                const int quantity0 = 10;
                const int quantity1 = 12;
                const int quantity2 = 21;
                const int quantity3 = 40001;
                const string tote0 = "89";
                const string tote1 = "0";
                const string tote2 = "54";
                const string growercode0 = "Grower0";
                const string growercode1 = "Grower1";
                const string growercode2 = "Grower2";
                const string growercode3 = "Grower3";
                const string varietyKey0 = "Variety0";
                const string varietyKey1 = "Variety1";

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var warehouseLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        DateReceived = expectedLotDate,
                        ChileProductKey = chileProductKey.KeyValue,
                        SupplierKey = dehydratorKey.KeyValue,
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity0,
                                        GrowerCode = growercode0,
                                        Variety = varietyKey0,
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue,
                                        ToteKey = tote0
                                    },
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity1,
                                        GrowerCode = growercode1,
                                        Variety = varietyKey0,
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue,
                                        ToteKey = tote0
                                    },
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity2,
                                        GrowerCode = growercode2,
                                        Variety = varietyKey0,
                                        PackagingProductKey = packagingProductKey1.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue,
                                        ToteKey = tote1
                                    },
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity3,
                                        GrowerCode = growercode3,
                                        Variety = varietyKey1,
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue,
                                        ToteKey = tote2
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var items = RVCUnitOfWork.ChileMaterialsReceivedItemRepository.Filter(i => i.LotDateCreated == expectedLotDate && i.LotDateSequence == expectedLotDateSequence && i.LotTypeId == expectedLotTypeId).ToList();
                Assert.AreEqual(expectedItems, items.Count);

                var item = items.Single(i => i.Quantity == quantity0);
                Assert.AreEqual(growercode0, item.GrowerCode);
                Assert.AreEqual(varietyKey0, item.ChileVariety);
                Assert.IsTrue(packagingProductKey0.Equals(item));
                Assert.IsTrue(warehouseLocationKey0.Equals(item));
                Assert.AreEqual(tote0, item.ToteKey);

                item = items.Single(i => i.Quantity == quantity1);
                Assert.AreEqual(growercode1, item.GrowerCode);
                Assert.AreEqual(varietyKey0, item.ChileVariety);
                Assert.IsTrue(packagingProductKey0.Equals(item));
                Assert.IsTrue(warehouseLocationKey0.Equals(item));
                Assert.AreEqual(tote0, item.ToteKey);

                item = items.Single(i => i.Quantity == quantity2);
                Assert.AreEqual(growercode2, item.GrowerCode);
                Assert.AreEqual(varietyKey0, item.ChileVariety);
                Assert.IsTrue(packagingProductKey1.Equals(item));
                Assert.IsTrue(warehouseLocationKey0.Equals(item));
                Assert.AreEqual(tote1, item.ToteKey);

                item = items.Single(i => i.Quantity == quantity3);
                Assert.AreEqual(growercode3, item.GrowerCode);
                Assert.AreEqual(varietyKey1, item.ChileVariety);
                Assert.IsTrue(packagingProductKey0.Equals(item));
                Assert.IsTrue(warehouseLocationKey1.Equals(item));
                Assert.AreEqual(tote2, item.ToteKey);
            }

            [Test]
            public void Creates_Inventory_records_as_expected_on_success()
            {
                //Arrange
                var expectedLotDate = new DateTime(2012, 3, 29);
                const int expectedLotDateSequence = 1;
                const int expectedLotTypeId = (int)LotTypeEnum.DeHydrated;
                const int expectedItems = 3;
                const int quantity0 = 10;
                const int quantity1 = 21;
                const int quantity2 = 40001;
                const string toteKey0 = "tote0";
                const string toteKey1 = "tote-1";
                const string toteKey2 = "";
                const string varietyKey0 = "variety0";
                const string varietyKey1 = "variety1";

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var warehouseLocationKey1 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        DateReceived = expectedLotDate,
                        ChileProductKey = chileProductKey.KeyValue,
                        SupplierKey = dehydratorKey.KeyValue,
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity0,
                                        Variety = varietyKey0,
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue,
                                        ToteKey = toteKey0
                                    },
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity1,
                                        Variety = varietyKey0,
                                        PackagingProductKey = packagingProductKey1.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue,
                                        ToteKey = toteKey1
                                    },
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = quantity2,
                                        Variety = varietyKey1,
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey1.KeyValue,
                                        ToteKey = toteKey2
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();

                var inventory = RVCUnitOfWork.InventoryRepository.Filter(i => i.LotDateCreated == expectedLotDate && i.LotDateSequence == expectedLotDateSequence && i.LotTypeId == expectedLotTypeId).ToList();
                Assert.AreEqual(expectedItems, inventory.Count);

                var item = inventory.Single(i => i.Quantity == quantity0);
                Assert.IsTrue(packagingProductKey0.Equals(item));
                Assert.IsTrue(warehouseLocationKey0.Equals(item));
                Assert.IsTrue(NoTreatmentKey.Equals(item));
                Assert.AreEqual(toteKey0, item.ToteKey);

                item = inventory.Single(i => i.Quantity == quantity1);
                Assert.IsTrue(packagingProductKey1.Equals(item));
                Assert.IsTrue(warehouseLocationKey0.Equals(item));
                Assert.IsTrue(NoTreatmentKey.Equals(item));
                Assert.AreEqual(toteKey1, item.ToteKey);

                item = inventory.Single(i => i.Quantity == quantity2);
                Assert.IsTrue(packagingProductKey0.Equals(item));
                Assert.IsTrue(warehouseLocationKey1.Equals(item));
                Assert.IsTrue(NoTreatmentKey.Equals(item));
                Assert.AreEqual(toteKey2, item.ToteKey);
            }

            [Test]
            public void Creates_a_single_Inventory_record_with_aggregate_Quantity_for_Items_received_that_reference_the_same_Packaging_WarehouseLocation()
            {
                //Arrange
                var expectedLotDate = new DateTime(2012, 3, 29);
                const int expectedLotDateSequence = 1;
                const int expectedLotTypeId = (int)LotTypeEnum.DeHydrated;
                const int quantity0 = 10;
                const int quantity1 = 21;
                const int expectedQuantity = quantity0 + quantity1;

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        DateReceived = expectedLotDate,
                        ChileProductKey = chileProductKey.KeyValue,
                        SupplierKey = dehydratorKey.KeyValue,
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        Items = new List<ChileMaterialsReceivedItemParameters>
                                {
                                    new ChileMaterialsReceivedItemParameters
                                        {
                                            Quantity = quantity0,
                                            Variety = "Variety",
                                            PackagingProductKey = packagingProductKey.KeyValue,
                                            LocationKey = warehouseLocationKey.KeyValue
                                        },
                                    new ChileMaterialsReceivedItemParameters
                                        {
                                            Quantity = quantity1,
                                            Variety = "Variety",
                                            PackagingProductKey = packagingProductKey.KeyValue,
                                            LocationKey = warehouseLocationKey.KeyValue
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();

                var inventory = RVCUnitOfWork.InventoryRepository.All().Single(i => i.LotDateCreated == expectedLotDate && i.LotDateSequence == expectedLotDateSequence && i.LotTypeId == expectedLotTypeId);
                Assert.AreEqual(expectedQuantity, inventory.Quantity);
            }

            [Test]
            public void Returns_non_successful_result_if_ChileProduct_is_not_of_Dehydrated_ChileState()
            {
                //Arrange
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.WIP));
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                {
                    UserToken = TestUser.UserName,
                    DateReceived = new DateTime(2012, 3, 29),
                    ChileProductKey = chileProductKey,
                    SupplierKey = dehydratorKey,
                    TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                    Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = 10,
                                        Variety = "variety",
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.ChileProductInvalidState);
            }

            [Test]
            public void Returns_non_succesful_result_if_any_Items_received_have_a_Quantity_not_greater_than_0()
            {
                //Arrange
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                var packagingProductKey0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey0 = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                {
                    UserToken = TestUser.UserName,
                    DateReceived = new DateTime(2012, 3, 29),
                    ChileProductKey = chileProductKey,
                    SupplierKey = dehydratorKey,
                    TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                    Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = 0,
                                        Variety = "variety",
                                        PackagingProductKey = packagingProductKey0.KeyValue,
                                        LocationKey = warehouseLocationKey0.KeyValue
                                    }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.QuantityNotGreaterThanZero);
            }

            [Test]
            public void Returns_successful_result_if_ProductionDate_has_a_time_component_and_a_Lot_exists_with_the_same_Date_Type_and_a_Sequence_of_1()
            {
                //Arrange
                var productionDate = new DateTime(2012, 4, 1, 10, 30, 21);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.LotDateCreated = productionDate, c => c.LotDateSequence = 1, c => c.LotTypeId = (int) LotTypeEnum.DeHydrated);

                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct().ChileState = ChileStateEnum.Dehydrated));
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var dehydratorKey = new CompanyKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>(c => c.SetCompanyTypes(CompanyType.Dehydrator)));

                //Act
                var result = Service.CreateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        DateReceived = productionDate,
                        ChileProductKey = chileProductKey,
                        SupplierKey = dehydratorKey,
                        TreatmentKey = StaticInventoryTreatments.NoTreatment.ToInventoryTreatmentKey(),
                        LoadNumber = "123",
                        PurchaseOrder = "POPO",
                        ShipperNumber = "1029384756",
                        Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        Quantity = 10,
                                        PackagingProductKey = packagingProductKey.KeyValue,
                                        LocationKey = warehouseLocationKey.KeyValue,
                                        Variety = "variety"
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
            }
        }

        [TestFixture]
        public class UpdateChileMaterialsReceivedTests : MaterialsReceivedServiceTests
        {
            protected override bool SetupStaticRecords { get { return true; } }

            [Test]
            public void Returns_non_successful_result_if_ChileMaterialsReceived_record_could_not_be_found()
            {
                //Act
                var result = Service.UpdateChileMaterialsReceived(new ChileMaterialsReceivedParameters
                    {
                        UserToken = TestUser.UserName,
                        LotKey = new LotKey(),
                        SupplierKey = new CompanyKey(),
                        ChileProductKey = new ChileProductKey(),
                        TreatmentKey = new InventoryTreatmentKey(),
                        Items = new List<ChileMaterialsReceivedItemParameters>
                            {
                                new ChileMaterialsReceivedItemParameters
                                    {
                                        PackagingProductKey = new PackagingProductKey(),
                                        LocationKey = new LocationKey(),
                                        Quantity = 1
                                    }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.ChileMaterialsReceivedNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_update_results_in_negative_inventory()
            {
                //Arrange
                var received = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(
                    r => r.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Other,
                    r => r.Items = TestHelper.List<ChileMaterialsReceivedItem>(1, l => l[0].Quantity = 10));
                var parameters = new ChileMaterialsReceivedParameters(received);
                parameters.Items[0].Quantity = 9;

                //Act
                var result = Service.UpdateChileMaterialsReceived(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.NegativeInventoryLots);
            }

            [Test]
            public void Modifies_ChileMaterialsReceived_records_as_expected()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(
                    p => p.ChileState = ChileStateEnum.Dehydrated);
                var received = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(
                    r => r.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                    r => r.Items = TestHelper.List<ChileMaterialsReceivedItem>(3));
                received.Items.ForEach(i => TestHelper.InsertObjectGraphIntoDatabase(i.ToInventory(received)));
                var parameters = new ChileMaterialsReceivedParameters(received)
                    {
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        DateReceived = DateTime.Now.Date,
                        LoadNumber = "123",
                        PurchaseOrder = "PurchaseOrder",
                        ShipperNumber = "ShipperNumber",
                    };
                parameters.Items[0].Quantity -= 1;
                parameters.Items[1].Quantity += 2;
                parameters.Items.RemoveAt(2);

                //Act
                var result = Service.UpdateChileMaterialsReceived(parameters);

                //Assert
                result.AssertSuccess();
                parameters.AssertAsExpected(RVCUnitOfWork.ChileMaterialsReceivedRepository
                    .FindByKey(received.ToLotKey(),
                        r => r.ChileLot.Lot,
                        r => r.Items));
            }

            [Test]
            public void Modifies_Inventory_as_expected()
            {
                //Arrange
                var received = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(
                    r => r.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Other,
                    r => r.Items = TestHelper.List<ChileMaterialsReceivedItem>(3));
                var inventory = received.Items.Select(i => TestHelper.InsertObjectGraphIntoDatabase(i.ToInventory(received))).ToList();
                var parameters = new ChileMaterialsReceivedParameters(received)
                    {
                        LotKey = received.ToLotKey(),
                        DateReceived = DateTime.Now.Date,
                        LoadNumber = "123",
                        PurchaseOrder = "PurchaseOrder",
                        ShipperNumber = "ShipperNumber",
                    };
                parameters.Items[0].Quantity -= 1;
                parameters.Items[1].Quantity += 2;
                parameters.Items.RemoveAt(2);

                //Act
                var result = Service.UpdateChileMaterialsReceived(parameters);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(parameters.Items[0].Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory[0])).Quantity);
                Assert.AreEqual(parameters.Items[1].Quantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory[1])).Quantity);
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory[2])));
            }
        }

        [TestFixture]
        public class GetChileMaterialsReceivedSummariesTests : MaterialsReceivedServiceTests
        {
            [Test]
            public void Returns_empty_collection_if_no_records_exist_on_success()
            {
                //Act
                var result = Service.GetChileMaterialsReceivedSummaries();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_summaries_with_expected_keys_on_success()
            {
                //Arrange
                const int expectedResults = 3;
                var expectedKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.ChileLot.Lot.EmptyLot()));
                var expectedKey1 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.ChileLot.Lot.EmptyLot()));
                var expectedKey2 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.ChileLot.Lot.EmptyLot()));

                //Act
                var result = Service.GetChileMaterialsReceivedSummaries();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.IsNotNull(results.Single(r => r.LotKey == expectedKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.LotKey == expectedKey1.KeyValue));
                Assert.IsNotNull(results.Single(r => r.LotKey == expectedKey2.KeyValue));
            }

            [Test]
            public void Will_return_summary_with_TotalLoad_equal_to_0_if_it_has_no_Items()
            {
                //Arrange
                const int expectedResults = 3;
                var expectedKey0 = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.Items = null, d => d.ChileLot.Lot.EmptyLot()));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.ChileLot.Lot.EmptyLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.ChileLot.Lot.EmptyLot());

                //Act
                var result = Service.GetChileMaterialsReceivedSummaries();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(expectedResults, results.Count);
                Assert.AreEqual(0, results.Single(r => r.LotKey == expectedKey0.KeyValue).TotalLoad);
            }

            [Test]
            public void Filters_results_as_expected()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var company = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Company>();

                var expected = new List<ChileMaterialsReceived>
                    {
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(m => m.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                            m => m.SetSupplier(company).ChileLot.SetProduct(chileProduct)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(m => m.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                            m => m.SetSupplier(company).ChileLot.SetProduct(chileProduct)),
                        TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(m => m.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                            m => m.SetSupplier(company).ChileLot.SetProduct(chileProduct))
                    };

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(m => m.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Other,
                    m => m.SetSupplier(company).ChileLot.SetProduct(chileProduct));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(m => m.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                    m => m.SetSupplier(company));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(m => m.ChileMaterialsReceivedType = ChileMaterialsReceivedType.Dehydrated,
                    m => m.ChileLot.SetProduct(chileProduct));

                //Act
                var result = Service.GetChileMaterialsReceivedSummaries(new ChileMaterialsReceivedFilters
                    {
                        ChileMaterialsType = ChileMaterialsReceivedType.Dehydrated,
                        SupplierKey = company.ToCompanyKey(),
                        ChileProductKey = chileProduct.ToChileProductKey()
                    });

                //Assert
                result.AssertSuccess();
                expected.AssertEquivalent(result.ResultingObject.ToList(), e => e.ToLotKey().KeyValue, r => r.LotKey,
                    (e, r) => { });
            }
        }

        [TestFixture]
        public class GetChileMaterialsReceivedDetailTests : MaterialsReceivedServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_record_could_not_be_found()
            {
                //Act
                var result = Service.GetChileMaterialsReceivedDetail(new LotKey(LotKey.Null).KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ChileMaterialsReceivedNotFound);
            }

            [Test]
            public void Returns_detail_with_expected_key_on_success()
            {
                //Arrange
                var key = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.ChileLot.Lot.EmptyLot()));

                //Act
                var result = Service.GetChileMaterialsReceivedDetail(key.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(key.KeyValue, result.ResultingObject.LotKey);
            }

            [Test]
            public void Returns_detail_with_Item_collection_as_expected_on_success()
            {
                //Arrange
                const int expectedItems = 3;
                var materialsReceivedKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceived>(d => d.Items = null, d => d.ChileLot.Lot.EmptyLot()));
                var itemKey0 = new ChileMaterialsReceivedItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceivedItem>(i => i.ConstrainByKeys(materialsReceivedKey)));
                var itemKey1 = new ChileMaterialsReceivedItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceivedItem>(i => i.ConstrainByKeys(materialsReceivedKey)));
                var itemKey2 = new ChileMaterialsReceivedItemKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceivedItem>(i => i.ConstrainByKeys(materialsReceivedKey)));

                //Act
                var result = Service.GetChileMaterialsReceivedDetail(materialsReceivedKey.KeyValue);

                //Assert
                result.AssertSuccess();
                var items = result.ResultingObject.Items.ToList();
                Assert.AreEqual(expectedItems, items.Count);
                Assert.IsNotNull(items.Single(i => i.ItemKey == itemKey0.KeyValue));
                Assert.IsNotNull(items.Single(i => i.ItemKey == itemKey1.KeyValue));
                Assert.IsNotNull(items.Single(i => i.ItemKey == itemKey2.KeyValue));
            }
        }

        [TestFixture]
        public class GetChileVarieties : MaterialsReceivedServiceTests
        {
            [Test]
            public void Returns_empty_result_if_no_ChileVariety_records_exist_in_database()
            {
                //Act
                var result = Service.GetChileVarieties();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_ChileVarieties_as_expected()
            {
                //Arrange
                var varities = new List<string>
                    {
                        "variety0",
                        "variety0",
                        "variety2",
                        "variety1",
                    };
                varities.ForEach(v => TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileMaterialsReceivedItem>(i => i.ChileVariety = v));
                
                //Act
                var result = Service.GetChileVarieties();

                //Assert
                result.AssertSuccess();
                varities = varities.Distinct().ToList();
                Assert.AreEqual(result.ResultingObject.Count(), varities.Count);
                varities.ForEach(v => Assert.IsTrue(result.ResultingObject.Contains(v)));
            }
        }
    }
}