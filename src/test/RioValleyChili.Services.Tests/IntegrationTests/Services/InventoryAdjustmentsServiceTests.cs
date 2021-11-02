using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class InventoryAdjustmentsServiceTests : ServiceIntegrationTestBase<InventoryAdjustmentsService>
    {
        [TestFixture]
        public class CreateInventoryAdjustmentTests : InventoryAdjustmentsServiceTests
        {
            internal class Params : ICreateInventoryAdjustmentParameters
            {
                public string UserToken { get; set; }

                public string Comment { get; set; }
                
                public IEnumerable<IInventoryAdjustmentParameters> InventoryAdjustments { get; set; }

                internal class ParamsItem : IInventoryAdjustmentParameters
                {
                    public string LotKey { get; set; }
                    public string WarehouseLocationKey { get; set; }
                    public string PackagingProductKey { get; set; }
                    public string TreatmentKey { get; set; }
                    public string ToteKey { get; set; }
                    public int Adjustment { get; set; }

                    internal ParamsItem() { }

                    internal ParamsItem(IInventoryKey inventoryKey)
                    {
                        LotKey = inventoryKey.ToLotKey();
                        WarehouseLocationKey = inventoryKey.ToLocationKey();
                        PackagingProductKey = inventoryKey.ToPackagingProductKey();
                        TreatmentKey = inventoryKey.ToInventoryTreatmentKey();
                        ToteKey = inventoryKey.InventoryKey_ToteKey;
                    }
                }
            }

            [Test]
            public void Returns_non_successful_result_if_InventoryAdjustments_is_null_or_empty()
            {
                //Arrange
                var userToken = TestUser.UserName;

                //Act
                var resultNull = Service.CreateInventoryAdjustment(new Params
                    {
                        UserToken = userToken,
                        InventoryAdjustments = null
                    });

                var resultEmpty = Service.CreateInventoryAdjustment(new Params
                {
                    UserToken = userToken,
                    InventoryAdjustments = new List<IInventoryAdjustmentParameters>()
                });

                //Assert
                resultNull.AssertNotSuccess(UserMessages.InventoryAdjustmentItemsRequired);
                resultEmpty.AssertNotSuccess(UserMessages.InventoryAdjustmentItemsRequired);
            }

            [Test]
            public void Creates_InventoryAdjustment_record_as_expected_on_success()
            {
                //Arrange
                var expectedDate = DateTime.UtcNow.Date;
                const int expectedSequence = 1;
                const string comment = "no";

                var user = TestUser.UserName;
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();

                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                    {
                        UserToken = user,
                        Comment = comment,
                        InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventory) { Adjustment = 1 }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var adjustmentResult = RVCUnitOfWork.InventoryAdjustmentRepository.All().Select(a => new { adjustment = a, notebook = a.Notebook, notes = a.Notebook.Notes, employee = a.Employee }).Single();
                var inventoryAdjustment = adjustmentResult.adjustment;
                Assert.AreEqual(expectedDate, inventoryAdjustment.AdjustmentDate.Date);
                Assert.AreEqual(expectedSequence, inventoryAdjustment.Sequence);
                Assert.AreEqual(user, inventoryAdjustment.Employee.UserName);
                Assert.AreEqual(comment, inventoryAdjustment.Notebook.Notes.Single().Text);
            }

            [Test]
            public void Creates_InventoryAdjustmentItem_records_as_expected_on_success()
            {
                //Arrange
                const int expectedResults = 3;
                const int adjustmentQuantity0 = 10;
                const int adjustmentQuantity1 = 11;
                const int adjustmentQuantity2 = -12;

                var userToken = TestUser.UserName;
                var inventory0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                var inventory1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = 20);
                
                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                    {
                        UserToken = userToken,
                        InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventory0) { Adjustment = adjustmentQuantity0 },
                                new Params.ParamsItem(inventory0) { Adjustment = adjustmentQuantity1 },
                                new Params.ParamsItem(inventory1) { Adjustment = adjustmentQuantity2 }
                            }
                    });

                //Assert
                result.AssertSuccess();
                var adjustments = RVCUnitOfWork.InventoryAdjustmentItemRepository.All().ToList();
                Assert.AreEqual(expectedResults, adjustments.Count);
                var inventoryKey0 = new InventoryKey(inventory0);
                var inventoryKey1 = new InventoryKey(inventory1);
                Assert.IsNotNull(adjustments.Single(a => inventoryKey0.KeyValue == new InventoryKey(a).KeyValue && adjustmentQuantity0 == a.QuantityAdjustment));
                Assert.IsNotNull(adjustments.Single(a => inventoryKey0.KeyValue == new InventoryKey(a).KeyValue && adjustmentQuantity1 == a.QuantityAdjustment));
                Assert.IsNotNull(adjustments.Single(a => inventoryKey1.KeyValue == new InventoryKey(a).KeyValue && adjustmentQuantity2 == a.QuantityAdjustment));
            }

            [Test]
            public void Returns_non_successful_result_if_any_Adjustment_quantity_is_0()
            {
                //Arrange
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>();
                var userToken = TestUser.UserName;

                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                {
                    UserToken = userToken,
                    InventoryAdjustments = new List<Params.ParamsItem>
                            {
                                new Params.ParamsItem(inventory) { Adjustment = 0 }
                            }
                });

                //Assert
                result.AssertNotSuccess(UserMessages.AdjustmentQuantityCannotBeZero);
            }

            [Test]
            public void Returns_non_successful_result_if_Adjustment_would_result_in_Inventory_with_a_negative_quantity()
            {
                //Arrange
                const int inventoryQuantity = 10;
                const int adjustQuantity = -11;
                var userToken = TestUser.UserName;
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = inventoryQuantity);

                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                    {
                        UserToken = userToken,
                        InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventory) { Adjustment = adjustQuantity }
                            }
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.NegativeInventoryLots);
            }

            [Test]
            public void Removes_Inventory_record_if_Adjustment_quantity_would_result_in_Inventory_with_quantity_of_zero()
            {
                //Arrange
                const int inventoryQuantity = 10;
                const int adjustQuantity = -10;
                var userToken = TestUser.UserName;
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = inventoryQuantity);

                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                {
                    UserToken = userToken,
                    InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventory) { Adjustment = adjustQuantity }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory)));
            }

            [Test]
            public void Modifies_existing_Inventory_record_as_expected()
            {
                //Arrange
                const int inventoryQuantity = 10;
                const int adjustQuantity = 12;
                const int expectedQuantity = inventoryQuantity + adjustQuantity;
                var userToken = TestUser.UserName;
                var inventory = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = inventoryQuantity);

                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                {
                    UserToken = userToken,
                    InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventory) { Adjustment = adjustQuantity }
                            }
                });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventory)).Quantity);
            }

            [Test]
            public void Creates_new_Inventory_record_as_expected_if_Adjustment_quantity_is_positive_and_Inventory_record_does_not_previously_exist()
            {
                //Arrange
                const int adjustmentQuantity = 12;
                const string toteKey = "";
                var userToken = TestUser.UserName;
                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot()));
                var packagingProductKey = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());
                var warehouseLocationKey = new LocationKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>());
                var treatmentKey = new InventoryTreatmentKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>());
                var inventoryKey = new InventoryKey(lotKey, packagingProductKey, warehouseLocationKey, treatmentKey, toteKey);
                
                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                    {
                        UserToken = userToken,
                        InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventoryKey) { Adjustment = adjustmentQuantity }
                            }
                    });

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(adjustmentQuantity, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryKey).Quantity);
            }

            [Test]
            public void Modifies_Inventory_records_in_aggregrate_as_expected_on_success()
            {
                //Arrange
                const int modifyAdjust0 = 75;
                const int modifyAdjust1 = -25;
                const int inventoryToModifyQuantity = 100;
                const int expectedInventoryToModifyResult = inventoryToModifyQuantity + modifyAdjust0 + modifyAdjust1;

                const int modifyRemove0 = -245;
                const int modifyRemove1 = 45;
                const int inventoryToRemoveQuantity = 200;

                const int createAdjust0 = -10;
                const int createAdjust1 = 50;
                const int expectedCreateResult = createAdjust0 + createAdjust1;

                var userToken = TestUser.UserName;
                var inventoryToModify = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = inventoryToModifyQuantity);
                var inventoryToRemove = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Inventory>(i => i.Quantity = inventoryToRemoveQuantity);

                var lotToCreate = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot());
                var packagingToCreate = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                var warehouseLocationToCreate = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Location>();
                var inventoryTreatmentToCreate = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryTreatment>();
                const string toteKeyToCreate = "";
                Func<InventoryKey> inventoryToCreate = () => new InventoryKey(lotToCreate, packagingToCreate, warehouseLocationToCreate, inventoryTreatmentToCreate, toteKeyToCreate);

                //Act
                var result = Service.CreateInventoryAdjustment(new Params
                    {
                        UserToken = userToken,
                        InventoryAdjustments = new List<IInventoryAdjustmentParameters>
                            {
                                new Params.ParamsItem(inventoryToModify) { Adjustment = modifyAdjust0 },
                                new Params.ParamsItem(inventoryToModify) { Adjustment = modifyAdjust1 },

                                new Params.ParamsItem(inventoryToRemove) { Adjustment = modifyRemove0 },
                                new Params.ParamsItem(inventoryToRemove) { Adjustment = modifyRemove1 },

                                new Params.ParamsItem(inventoryToCreate()) { Adjustment = createAdjust0 },
                                new Params.ParamsItem(inventoryToCreate()) { Adjustment = createAdjust1 },
                            }
                    });

                //Assert
                result.AssertSuccess();
                
                Assert.AreEqual(expectedInventoryToModifyResult, RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToModify)).Quantity);
                Assert.IsNull(RVCUnitOfWork.InventoryRepository.FindByKey(new InventoryKey(inventoryToRemove)));
                Assert.AreEqual(expectedCreateResult, RVCUnitOfWork.InventoryRepository.FindByKey(inventoryToCreate()).Quantity);
            }
        }

        [TestFixture]
        public class GetInventoryAdjustmentsTests : InventoryAdjustmentsServiceTests
        {
            [Test]
            public void Returns_empty_result_if_no_InventoryAdjustment_records_exist_in_the_database()
            {
                //Act
                StartStopwatch();
                var result = Service.GetInventoryAdjustments();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_all_InventoryAdjustments_if_no_filtering_parameters_are_specified()
            {
                //Arrange
                const int expectedResults = 3;

                var inventoryAdjustment0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>();
                var inventoryAdjustment1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>();
                var inventoryAdjustment2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>();

                //Act
                StartStopwatch();
                var result = Service.GetInventoryAdjustments();
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                inventoryAdjustment0.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment0).KeyValue));
                inventoryAdjustment1.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment1).KeyValue));
                inventoryAdjustment2.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment2).KeyValue));
            }

            [Test]
            public void Returns_InventoryAdjustments_that_modify_Inventory_of_the_specified_LotKey()
            {
                //Arrange
                const int expectedResults = 2;

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetChileLot()));

                var inventoryAdjustment0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.Items = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment0).Lot.SetChileLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment0, lotKey));

                var inventoryAdjustment1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.Items = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment1).Lot.SetChileLot());
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment1, lotKey));

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>();

                //Act
                StartStopwatch();
                var result = Service.GetInventoryAdjustments(new FilterInventoryAdjustmentParameters { LotKey = lotKey.KeyValue });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime();

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                inventoryAdjustment0.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment0).KeyValue));
                inventoryAdjustment1.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment1).KeyValue));
            }

            [Test]
            public void Returns_InventoryAdjustments_that_have_their_TimeStamp_within_the_specified_AdjustmentDateRange()
            {
                //Arrange
                const int expectedResults = 3;
                var rangeStart = new DateTime(2012, 3, 29);
                var rangeEnd = new DateTime(2012, 4, 20);

                var inventoryAdjustment0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart);
                var inventoryAdjustment1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart.AddDays(10));
                var inventoryAdjustment2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeEnd);

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart.AddDays(-10));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeEnd.AddDays(10));

                //Act
                StartStopwatch();
                var result = Service.GetInventoryAdjustments(new FilterInventoryAdjustmentParameters { AdjustmentDateRangeStart = rangeStart, AdjustmentDateRangeEnd = rangeEnd });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);

                inventoryAdjustment0.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment0).KeyValue));
                inventoryAdjustment1.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment1).KeyValue));
                inventoryAdjustment2.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment2).KeyValue));
            }

            [Test]
            public void Returns_InventoryAdjustments_as_expected_with_combined_filtering_options()
            {
                //Arrange
                const int expectedResults = 2;
                var rangeStart = new DateTime(2012, 3, 29);
                var rangeEnd = new DateTime(2012, 4, 20);

                var lotKey = new LotKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Lot>(l => l.EmptyLot().SetChileLot()));

                var inventoryAdjustment0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart.AddDays(10),
                    a => a.Items = new List<InventoryAdjustmentItem> { TestHelper.CreateObjectGraph<InventoryAdjustmentItem>(i => i.ConstrainByKeys(null, lotKey)) });
                var inventoryAdjustment1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart,
                    a => a.Items = new List<InventoryAdjustmentItem> { TestHelper.CreateObjectGraph<InventoryAdjustmentItem>(i => i.ConstrainByKeys(null, lotKey)) });

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart.AddDays(10));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeStart.AddDays(-10),
                    a => a.Items = new List<InventoryAdjustmentItem> { TestHelper.CreateObjectGraph<InventoryAdjustmentItem>(i => i.ConstrainByKeys(null, lotKey)) });
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.AdjustmentDate = rangeEnd.AddDays(10),
                    a => a.Items = new List<InventoryAdjustmentItem> { TestHelper.CreateObjectGraph<InventoryAdjustmentItem>(i => i.ConstrainByKeys(null, lotKey)) });

                //Act
                StartStopwatch();
                var result = Service.GetInventoryAdjustments(new FilterInventoryAdjustmentParameters
                    {
                        AdjustmentDateRangeStart = rangeStart,
                        AdjustmentDateRangeEnd = rangeEnd,
                        LotKey = lotKey.KeyValue
                    });
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedResults, results.Count);
                inventoryAdjustment0.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment0).KeyValue));
                inventoryAdjustment1.AssertEqual(results.Single(r => r.InventoryAdjustmentKey == new InventoryAdjustmentKey(inventoryAdjustment1).KeyValue));
            }
        }

        [TestFixture]
        public class GetInventoryAdjustmentTests : InventoryAdjustmentsServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_InventoryAdjustment_record_could_not_be_found()
            {
                //Act
                var result = TimedExecution(() => Service.GetInventoryAdjustment(new InventoryAdjustmentKey(TestHelper.CreateObjectGraph<InventoryAdjustment>()).KeyValue));

                //Assert
                result.AssertNotSuccess(UserMessages.InventoryAdjustmentNotFound);
            }

            [Test]
            public void Returns_InventoryAdjustment_as_expected_on_success()
            {
                //Arrange
                var inventoryAdjustment = TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustment>(a => a.Items = null);
                var adjustmentKey = new InventoryAdjustmentKey(inventoryAdjustment);

                var chileLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(c => c.NullProduction().Lot.EmptyLot().SetChileLot());
                var additiveLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(a => a.Lot.EmptyLot().SetAdditiveLot());
                var packagingLot = TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingLot>(p => p.Lot.EmptyLot().SetPackagingLot());

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment, chileLot));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment, additiveLot));
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<InventoryAdjustmentItem>(i => i.ConstrainByKeys(inventoryAdjustment, packagingLot));

                //Act
                var result = TimedExecution(() => Service.GetInventoryAdjustment(adjustmentKey.KeyValue));

                //Assert
                result.AssertSuccess();
                inventoryAdjustment.AssertEqual(result.ResultingObject, new List<dynamic> { chileLot, additiveLot, packagingLot });
            }
        }
    }
}