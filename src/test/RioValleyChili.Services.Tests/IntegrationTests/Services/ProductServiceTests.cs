using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Tests.Helpers;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.Helpers.ParameterExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;
using RioValleyChili.Services.Utilities.Helpers;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class ProductServiceTests : ServiceIntegrationTestBase<ProductService>
    {
        protected override bool SetupStaticRecords { get { return false; } }

        [TestFixture]
        public class GetAdditiveProductSummaries : ProductServiceTests
        {
            [Test]
            public void Returns_AdditiveProduct_summaries_as_expected()
            {
                //Arrange
                const string code0 = "Code 0";
                const string code1 = "Code 1";
                const string code2 = "Code 2";
                const bool active0 = false;
                const bool active1 = true;
                const bool active2 = true;

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>(a => a.Product.ProductCode = code0, a => a.Product.IsActive = active0);
                var additiveProductKey1 = new AdditiveProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>(a => a.Product.ProductCode = code1, a => a.Product.IsActive = active1));
                var additiveProductKey2 = new AdditiveProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>(a => a.Product.ProductCode = code2, a => a.Product.IsActive = active2));

                //Act
                var result = Service.GetAdditiveProducts(true);

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.IsNotNull(results.Single(r => r.ProductKey == additiveProductKey1.KeyValue && r.ProductCode == code1 && r.IsActive == active1));
                Assert.IsNotNull(results.Single(r => r.ProductKey == additiveProductKey2.KeyValue && r.ProductCode == code2 && r.IsActive == active2));
            }

            [Test]
            public void Returns_only_AdditiveProducts_that_have_Inventory()
            {
                //Arrange
                var additiveProductKey0 = new AdditiveProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(l => l.Lot.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>()
                    }).AdditiveProduct);
                var additiveProductKey1 = new AdditiveProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveLot>(l => l.Lot.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>()
                    }).AdditiveProduct);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>();

                //Act
                var result = Service.GetAdditiveProducts(true, true);

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(2, results.Count);
                Assert.IsNotNull(results.Single(r => r.ProductKey == additiveProductKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.ProductKey == additiveProductKey1.KeyValue));
            }
        }

        [TestFixture]
        public class GetChileProductDetail : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ChileProduct_could_not_be_found()
            {
                //Act
                var result = Service.GetChileProductDetail(new ChileProductKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.ChileProductNotFound);
            }

            [Test]
            public void Returns_ChileProduct_with_ProductKey_and_ProductCode_as_expected()
            {
                //Arrange
                const string code = "code";
                const bool active = true;
                var chileProductKey = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.ProductCode = code, c => c.Product.IsActive = active));

                //Act
                var result = Service.GetChileProductDetail(chileProductKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(chileProductKey.KeyValue, result.ResultingObject.ProductKey);
                Assert.AreEqual(code, result.ResultingObject.ProductCode);
                Assert.AreEqual(active, result.ResultingObject.IsActive);
            }

            [Test]
            public void Returns_ChileProduct_with_AttributeRanges_as_expected()
            {
                //Arrange
                const int expectedRanges = 4;
                const string expectedName0 = "ATTRIBUTE";
                const string expectedName1 = "Hotnesssss";
                const string expectedName2 = "Bad attribute";
                const string expectedName3 = "No-no-no-no-no";

                const double min0 = 0.0, max0 = 1.0;
                const double min1 = -10.0, max1 = 33.33;
                const double min2 = 20.0, max2 = 40.0;
                const double min3 = -50.0, max3 = 100.0;

                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ProductAttributeRanges = null);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct), r => r.AttributeName.Name = expectedName0, r => r.RangeMin = min0, r => r.RangeMax = max0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct), r => r.AttributeName.Name = expectedName1, r => r.RangeMin = min1, r => r.RangeMax = max1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct), r => r.AttributeName.Name = expectedName2, r => r.RangeMin = min2, r => r.RangeMax = max2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(r => r.ConstrainByKeys(chileProduct), r => r.AttributeName.Name = expectedName3, r => r.RangeMin = min3, r => r.RangeMax = max3);

                //Act
                var result = Service.GetChileProductDetail(new ChileProductKey(chileProduct).KeyValue);

                //Assert
                result.AssertSuccess();
                var ranges = result.ResultingObject.AttributeRanges.ToList();
                Assert.AreEqual(expectedRanges, ranges.Count);

                var index = ranges.IndexOf(ranges.Single(r => r.AttributeName == expectedName0));
                Assert.AreEqual(min0, ranges[index].MinValue);
                Assert.AreEqual(max0, ranges[index].MaxValue);

                index = ranges.IndexOf(ranges.Single(r => r.AttributeName == expectedName1));
                Assert.AreEqual(min1, ranges[index].MinValue);
                Assert.AreEqual(max1, ranges[index].MaxValue);

                index = ranges.IndexOf(ranges.Single(r => r.AttributeName == expectedName2));
                Assert.AreEqual(min2, ranges[index].MinValue);
                Assert.AreEqual(max2, ranges[index].MaxValue);

                index = ranges.IndexOf(ranges.Single(r => r.AttributeName == expectedName3));
                Assert.AreEqual(min3, ranges[index].MinValue);
                Assert.AreEqual(max3, ranges[index].MaxValue);
            }
        }

        [TestFixture]
        public class GetChileProducts : ProductServiceTests
        {
            [Test]
            public void Returns_ChileProducts_as_expected()
            {
                //Arrange
                const string code0 = "Code 0";
                const string code1 = "Code 1";
                const string code2 = "Code 2";
                const bool active0 = false;
                const bool active1 = true;
                const bool active2 = true;
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.ProductCode = code0, c => c.Product.IsActive = active0);
                var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.ProductCode = code1, c => c.Product.IsActive = active1));
                var chileProductKey2 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.ProductCode = code2, c => c.Product.IsActive = active2));

                //Act
                StartStopwatch();
                var result = Service.GetChileProducts(null, true);
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.IsNotNull(results.Single(r => r.ProductKey == chileProductKey1.KeyValue && r.ProductCode == code1 && r.IsActive == active1));
                Assert.IsNotNull(results.Single(r => r.ProductKey == chileProductKey2.KeyValue && r.ProductCode == code2 && r.IsActive == active2));
            }

            [Test]
            public void Returns_ChileProducts_of_ChileState_specified()
            {
                //Arrange
                const ChileStateEnum expectedState = ChileStateEnum.WIP;
                var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.IsActive = true, c => c.ChileState = expectedState));
                var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.IsActive = true, c => c.ChileState = expectedState));
                var chileProductKey2 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Product.IsActive = true, c => c.ChileState = ChileStateEnum.OtherRaw));

                //Act
                StartStopwatch();
                var result = TimedExecution(() => Service.GetChileProducts(expectedState, true));
                var results = result.ResultingObject == null ? null : result.ResultingObject.ToList();
                StopWatchAndWriteTime("Act");

                //Assert
                result.AssertSuccess();
                Assert.AreEqual(expectedState, results.Single(r => r.ProductKey == chileProductKey0.KeyValue).ChileState);
                Assert.AreEqual(expectedState, results.Single(r => r.ProductKey == chileProductKey1.KeyValue).ChileState);
                Assert.IsNull(results.SingleOrDefault(r => r.ProductKey == chileProductKey2.KeyValue));
            }

            [Test]
            public void Returns_only_ChileProducts_that_have_Inventory()
            {
                //Arrange
                var chileProductKey0 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.Lot.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>()
                    }).ChileProduct);
                var chileProductKey1 = new ChileProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileLot>(l => l.Lot.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>()
                    }).ChileProduct);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                //Act
                var result = Service.GetChileProducts(null, true, true);

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(2, results.Count);
                Assert.IsNotNull(results.Single(r => r.ProductKey == chileProductKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.ProductKey == chileProductKey1.KeyValue));
            }
        }

        [TestFixture]
        public class GetChileTypeSummaries : ProductServiceTests
        {
            [Test]
            public void Returns_ChileTypes_as_expected()
            {
                //Arrange
                var chileTypeKey0 = new ChileTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>());
                var chileTypeKey1 = new ChileTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>());
                var chileTypeKey2 = new ChileTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>());

                //Act
                var result = Service.GetChileTypeSummaries();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.IsNotNull(results.Single(r => r.ChileTypeKey == chileTypeKey0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.ChileTypeKey == chileTypeKey1.KeyValue));
                Assert.IsNotNull(results.Single(r => r.ChileTypeKey == chileTypeKey2.KeyValue));
            }
        }

        [TestFixture]
        public class GetChileAttributeNames : ProductServiceTests
        {
            [Test]
            public void Returns_ChileAttributeNames_as_expected()
            {
                //Arrange
                var expectedNames = new List<string>();
                for(int i = 0; i < 5; i++)
                {
                    expectedNames.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.Active = n.ValidForChileInventory = true).Name);
                }

                //Act
                var result = Service.GetChileAttributeNames();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                expectedNames.ForEach(n => results.Single(r => r == n));
            }
        }

        [TestFixture]
        public class SetChileProductAttributeRanges : ProductServiceTests
        {
            [Test]
            public void Returns_Invalid_result_if_Min_value_is_greater_than_Max()
            {
                //Act
                var result = Service.SetChileProductAttributeRanges(new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey(),
                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                new SetAttributeRangeParameters
                                    {
                                        AttributeNameKey = new AttributeNameKey(),
                                        RangeMin = 100.0,
                                        RangeMax = 99.99
                                    }
                            }
                    });

                //Assert
                result.AssertInvalid(UserMessages.ChileProductAttributeRangeMinGreaterThanMax);
            }

            [Test]
            public void Returns_Invalid_result_if_ChileProduct_cannot_be_found()
            {
                //Act
                var result = Service.SetChileProductAttributeRanges(new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey(),
                    });

                //Assert
                result.AssertInvalid(UserMessages.ChileProductNotFound);
            }

            [Test]
            public void Returns_Invalid_result_if_AttributeName_could_not_be_found()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                //Act
                var result = Service.SetChileProductAttributeRanges(new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                new SetAttributeRangeParameters
                                    {
                                        AttributeNameKey = new AttributeNameKey(),
                                        RangeMin = 100.0,
                                        RangeMax = 100.0
                                    }
                            }
                    });

                //Assert
                result.AssertInvalid(UserMessages.AttributeNameNotFound);
            }

            [Test]
            public void Returns_Invalid_result_if_AttributeName_is_not_flagged_as_being_valid_for_Chile()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var attributeName = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.ValidForChileInventory = false);

                //Act
                var result = Service.SetChileProductAttributeRanges(new SetChileProductAttributeRangesParameters
                {
                    UserToken = TestUser.UserName,
                    ChileProductKey = chileProduct.ToChileProductKey(),
                    AttributeRanges = new List<ISetAttributeRangeParameters>
                        {
                            new SetAttributeRangeParameters
                                {
                                    AttributeNameKey = attributeName.ToAttributeNameKey(),
                                    RangeMin = 100.0,
                                    RangeMax = 100.0
                                }
                        }
                });

                //Assert
                result.AssertInvalid(UserMessages.AttributeNameNotValidForChile);
            }

            [Test]
            public void Will_create_nonexistent_ChileProductAttributeRange_as_expected_on_success()
            {
                //Arrange
                var expectedTimestampDate = DateTime.UtcNow.Date;
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.EmptyProduct());
                var attributeName = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(n => n.ValidForChileInventory = true);

                var parameters = new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                new SetAttributeRangeParameters
                                    {
                                        AttributeNameKey = attributeName.ToAttributeNameKey(),
                                        RangeMin = 0.0,
                                        RangeMax = 100.0
                                    }
                            }
                    };

                //Act
                var result = Service.SetChileProductAttributeRanges(parameters);

                //Assert
                result.AssertSuccess();
                var chileProductAttributeRange = RVCUnitOfWork.ChileProductAttributeRangeRepository.All().Single();
                parameters.AttributeRanges.Single().AssertEquivalent(chileProductAttributeRange);
                Assert.AreEqual(expectedTimestampDate, chileProductAttributeRange.TimeStamp.Date);
            }

            [Test]
            public void Will_modify_existing_ChileProductAttributeRange_as_expected_on_success()
            {
                //Arrange
                var expectedTimestampDate = DateTime.UtcNow.Date;
                var attributeRange = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductAttributeRange>(a => a.AttributeName.ValidForChileInventory = true);
                var attributeRangeKey = attributeRange.ToChileProductAttributeRangeKey();

                var parameters = new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = attributeRange.ToChileProductKey(),
                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                new SetAttributeRangeParameters
                                    {
                                        AttributeNameKey = attributeRange.ToAttributeNameKey(),
                                        RangeMin = 0.0,
                                        RangeMax = 100.0
                                    }
                            }
                    };

                //Act
                var result = Service.SetChileProductAttributeRanges(parameters);

                //Assert
                result.AssertSuccess();
                var chileProductAttributeRange = RVCUnitOfWork.ChileProductAttributeRangeRepository.FindByKey(attributeRangeKey);
                parameters.AttributeRanges.Single().AssertEquivalent(chileProductAttributeRange);
                Assert.AreEqual(expectedTimestampDate, chileProductAttributeRange.TimeStamp.Date);
            }

            [Test]
            public void Sets_attribute_ranges_as_expected()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.ProductAttributeRanges = TestHelper.List<ChileProductAttributeRange>(5));
                var expectedRanges = new List<SetAttributeRangeParameters>();
                for(var i = 0; i < 3; ++i)
                {
                    expectedRanges.Add(new SetAttributeRangeParameters
                        {
                            AttributeNameKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.ValidForChileInventory = true).ToAttributeNameKey(),
                            RangeMin = i + 1,
                            RangeMax = 1 + 2
                        });
                }

                //Act
                var result = Service.SetChileProductAttributeRanges(new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        AttributeRanges = expectedRanges
                    });

                //Assert
                result.AssertSuccess();
                var actual = RVCUnitOfWork.ChileProductRepository.FindByKey(chileProduct.ToChileProductKey(), c => c.ProductAttributeRanges).ProductAttributeRanges;
                expectedRanges.AssertEquivalent(actual, e => e.AttributeNameKey, a => a.ToAttributeNameKey().KeyValue,
                    (e, a) => e.AssertEquivalent(a));
            }
        }

        [TestFixture]
        public class SetChileProductIngredient : ProductServiceTests
        {
            [Test]
            public void Returns_Invalid_result_if_Percentage_is_less_than_0_or_greater_than_100()
            {
                //Act
                var lessThanZero = Service.SetChileProductIngredients(new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey().KeyValue,
                        Ingredients = new List<SetChileProductIngredientParameters>
                            {
                                new SetChileProductIngredientParameters
                                    {
                                        AdditiveTypeKey = new AdditiveTypeKey().KeyValue,
                                        Percentage = -0.001
                                    }
                            }
                    });

                var greaterThanHundred = Service.SetChileProductIngredients(new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey().KeyValue,
                        Ingredients = new List<SetChileProductIngredientParameters>
                                {
                                    new SetChileProductIngredientParameters
                                        {
                                            AdditiveTypeKey = new AdditiveTypeKey().KeyValue,
                                            Percentage = 100.001
                                        }
                                }
                    });

                //Assert
                lessThanZero.AssertInvalid(UserMessages.IngredientPercentageCanIngredientPercentageOutOfRange);
                greaterThanHundred.AssertInvalid(UserMessages.IngredientPercentageCanIngredientPercentageOutOfRange);
            }

            [Test]
            public void Returns_Invalid_result_if_ChileProduct_cannot_be_found()
            {
                //Act
                var result = Service.SetChileProductIngredients(new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = new ChileProductKey().KeyValue
                    });

                //Assert
                result.AssertInvalid(UserMessages.ChileProductNotFound);
            }

            [Test]
            public void Returns_Invalid_result_if_AttributeType_could_not_be_found()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                //Act
                var result = Service.SetChileProductIngredients(new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        Ingredients = new List<SetChileProductIngredientParameters>
                            {
                                new SetChileProductIngredientParameters
                                    {
                                        AdditiveTypeKey = new AdditiveTypeKey()
                                    }
                            }
                    });

                //Assert
                result.AssertInvalid(UserMessages.AdditiveTypeNotFound);
            }

            [Test]
            public void Will_create_nonexistent_ChileProductIngredient_as_expected_on_success()
            {
                //Arrange
                var expectedTimestampDate = DateTime.UtcNow.Date;
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();
                var additiveType = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>();

                var parameters = new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        Ingredients = new List<SetChileProductIngredientParameters>
                            {
                                new SetChileProductIngredientParameters
                                    {
                                        AdditiveTypeKey = additiveType.ToAdditiveTypeKey(),
                                        Percentage = 50.0
                                    }
                            }
                    };

                //Act
                var result = Service.SetChileProductIngredients(parameters);

                //Assert
                result.AssertSuccess();
                var ingredient = RVCUnitOfWork.ChileProductIngredientRepository.All().Single();
                ingredient.AssertEqual(parameters.Ingredients.Single());
                Assert.AreEqual(expectedTimestampDate, ingredient.TimeStamp.Date);
            }

            [Test]
            public void Will_modify_existing_ChileProductIngredient_as_expected_on_success()
            {
                //Arrange
                var expectedTimestampDate = DateTime.UtcNow.Date;
                var chileProductIngredient = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProductIngredient>();
                var chileProductIngredientKey = new ChileProductIngredientKey(chileProductIngredient);

                var parameters = new SetChileProductIngredientsParameters
                {
                    UserToken = TestUser.UserName,
                    ChileProductKey = chileProductIngredient.ToChileProductKey(),
                    Ingredients = new List<SetChileProductIngredientParameters>
                        {
                            new SetChileProductIngredientParameters
                                {
                                    AdditiveTypeKey = chileProductIngredient.ToAdditiveTypeKey(),
                                    Percentage = 50.0
                                }
                        }
                };

                //Act
                var result = Service.SetChileProductIngredients(parameters);

                //Assert
                result.AssertSuccess();
                var ingredient = RVCUnitOfWork.ChileProductIngredientRepository.FindByKey(chileProductIngredientKey);
                ingredient.AssertEqual(parameters.Ingredients.Single());
                Assert.AreEqual(expectedTimestampDate, ingredient.TimeStamp.Date);
            }

            [Test]
            public void Sets_ingredients_as_expected()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>(c => c.Ingredients = TestHelper.List<ChileProductIngredient>(5));
                var expectedIngredients = new List<SetChileProductIngredientParameters>();
                for(var i = 0; i < 3; ++i)
                {
                    expectedIngredients.Add(new SetChileProductIngredientParameters
                        {
                            AdditiveTypeKey = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>().ToAdditiveTypeKey(),
                            Percentage = i + 1
                        });
                }

                //Act
                var result = Service.SetChileProductIngredients(new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        Ingredients = expectedIngredients
                    });

                //Assert
                result.AssertSuccess();
                chileProduct = RVCUnitOfWork.ChileProductRepository.FindByKey(chileProduct.ToChileProductKey(), c => c.Ingredients);
                expectedIngredients.AssertEquivalent(chileProduct.Ingredients, e => e.AdditiveTypeKey, r => r.ToAdditiveTypeKey(),
                    (e, r) => Assert.AreEqual(e.Percentage, r.Percentage));
            }
        }

        [TestFixture]
        public class GetPackagingProducts : ProductServiceTests
        {
            [Test]
            public void Returns_PackagingProducts_as_expected()
            {
                //Arrange
                const string code0 = "Code 0";
                const string code1 = "Code 1";
                const string code2 = "Code 2";
                const bool active0 = false;
                const bool active1 = true;
                const bool active2 = true;
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Product.ProductCode = code0, p => p.Product.IsActive = active0);
                var packagingProductKey1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Product.ProductCode = code1, p => p.Product.IsActive = active1));
                var packagingProductKey2 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>(p => p.Product.ProductCode = code2, p => p.Product.IsActive = active2));

                //Act
                var result = Service.GetPackagingProducts(true);

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.IsNotNull(results.Single(r => r.ProductKey == packagingProductKey1.KeyValue && r.ProductCode == code1 && r.IsActive == active1));
                Assert.IsNotNull(results.Single(r => r.ProductKey == packagingProductKey2.KeyValue && r.ProductCode == code2 && r.IsActive == active2));
            }

            [Test]
            public void Returns_only_PackagingProducts_that_have_Inventory()
            {
                //Arrange
                var packagingProduct0 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingLot>(l => l.Lot.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>()
                    }).PackagingProduct);
                var packagingProduct1 = new PackagingProductKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingLot>(l => l.Lot.Inventory = new List<Inventory>
                    {
                        TestHelper.CreateObjectGraph<Inventory>()
                    }).PackagingProduct);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>();

                //Act
                var result = Service.GetPackagingProducts(true, true);

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();
                Assert.AreEqual(2, results.Count);
                Assert.IsNotNull(results.Single(r => r.ProductKey == packagingProduct0.KeyValue));
                Assert.IsNotNull(results.Single(r => r.ProductKey == packagingProduct1.KeyValue));
            }
        }

        [TestFixture]
        public class GetProductSubTypes : ProductServiceTests
        {
            [Test]
            public void Returns_empty_results_by_ProductType_if_no_sub_types_exist_in_database()
            {
                //Act
                var result = Service.GetProductSubTypes();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject.Single(r => r.Key == ProductTypeEnum.Chile).Value);
                Assert.IsEmpty(result.ResultingObject.Single(r => r.Key == ProductTypeEnum.Additive).Value);
                Assert.IsEmpty(result.ResultingObject.Single(r => r.Key == ProductTypeEnum.Packaging).Value);
            }

            [Test]
            public void Returns_expected_results_by_ProductType_on_success()
            {
                //Arrange
                const int expectedChile = 3;
                const int expectedAdditive = 4;
                const int expectedPackaging = 0;
                const string chile0 = "Chile0";
                const string chile1 = "Chile1";
                const string chile2 = "Chile2";
                const string additive0 = "Additive0";
                const string additive1 = "Additive1";
                const string additive2 = "Additive2";
                const string additive3 = "Additive3";

                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>(c => c.Description = chile0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>(c => c.Description = chile1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>(c => c.Description = chile2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>(a => a.Description = additive0);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>(a => a.Description = additive1);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>(a => a.Description = additive2);
                TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>(a => a.Description = additive3);

                //Act
                var result = Service.GetProductSubTypes();

                //Assert
                result.AssertSuccess();

                var chile = result.ResultingObject.Single(r => r.Key == ProductTypeEnum.Chile).Value.ToList();
                Assert.AreEqual(expectedChile, chile.Count);
                Assert.AreEqual(1, chile.Count(c => c == chile0));
                Assert.AreEqual(1, chile.Count(c => c == chile1));
                Assert.AreEqual(1, chile.Count(c => c == chile2));

                var additive = result.ResultingObject.Single(r => r.Key == ProductTypeEnum.Additive).Value.ToList();
                Assert.AreEqual(expectedAdditive, additive.Count);
                Assert.AreEqual(1, additive.Count(a => a == additive0));
                Assert.AreEqual(1, additive.Count(a => a == additive1));
                Assert.AreEqual(1, additive.Count(a => a == additive2));
                Assert.AreEqual(1, additive.Count(a => a == additive3));

                var packaging = result.ResultingObject.Single(r => r.Key == ProductTypeEnum.Packaging).Value.ToList();
                Assert.AreEqual(expectedPackaging, packaging.Count);
            }
        }

        [TestFixture]
        public class CreateAdditiveProduct : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_AdditiveType_could_not_be_found()
            {
                //Arrange
                var parameters = new CreateAdditiveProductParameters
                    {
                        UserToken = "User",
                        ProductCode = "ProductCode",
                        ProductName = "ProductName",
                        AdditiveTypeKey = new AdditiveTypeKey().KeyValue
                    };

                //Act
                var result = Service.CreateAdditiveProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.AdditiveTypeNotFound);
            }

            [Test]
            public void Creates_AdditiveProduct_and_Product_records_as_expected()
            {
                //Arrange
                var additiveType = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>();
                var parameters = new CreateAdditiveProductParameters
                    {
                        UserToken = "User",
                        ProductCode = "ProductCode",
                        ProductName = "ProductName",
                        AdditiveTypeKey = new AdditiveTypeKey(additiveType).KeyValue
                    };

                

                //Act
                var result = Service.CreateAdditiveProduct(parameters);

                //Assert
                result.AssertSuccess();
                var additiveProductResult = RVCUnitOfWork.AdditiveProductRepository.All().Select(a => new { additiveProduct = a, a.Product, a.AdditiveType }).SingleOrDefault();
                Assert.NotNull(additiveProductResult);

                var additiveProduct = additiveProductResult.additiveProduct;
                Assert.AreEqual(parameters.ProductName, additiveProduct.Product.Name);
                Assert.AreEqual(parameters.ProductCode, additiveProduct.Product.ProductCode);
                Assert.IsTrue(additiveProduct.Product.IsActive);
                Assert.AreEqual(parameters.AdditiveTypeKey, new AdditiveTypeKey(additiveProduct.AdditiveType).KeyValue);
            }
        }

        [TestFixture]
        public class UpdateAdditiveProduct : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_AdditiveProduct_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdateAdditiveProductParameters
                    {
                        ProductKey = new ProductKey().KeyValue,
                        AdditiveTypeKey = new AdditiveTypeKey().KeyValue
                    };

                //Act
                var result = Service.UpdateAdditiveProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.AdditiveProductNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_AdditiveType_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdateAdditiveProductParameters
                {
                    ProductKey = new ProductKey((IProductKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>()).KeyValue,
                    AdditiveTypeKey = new AdditiveTypeKey().KeyValue
                };

                //Act
                var result = Service.UpdateAdditiveProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.AdditiveTypeNotFound);
            }

            [Test]
            public void Updates_AdditiveProduct_and_Product_records_as_expected()
            {
                //Arrange
                var additiveProductKey = new ProductKey((IProductKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveProduct>());
                var expectedAdditiveTypeKey = new AdditiveTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>());

                

                var parameters = new UpdateAdditiveProductParameters
                    {
                        UserToken = "User",
                        ProductKey = additiveProductKey.KeyValue,
                        ProductCode = "Product Code",
                        ProductName = "Product Name",
                        IsActive = false,
                        AdditiveTypeKey = expectedAdditiveTypeKey.KeyValue
                    };

                //Act
                var result = Service.UpdateAdditiveProduct(parameters);

                //Assert
                result.AssertSuccess();
                var additiveProduct = RVCUnitOfWork.AdditiveProductRepository.FindByKey(additiveProductKey, a => a.Product);

                Assert.NotNull(additiveProduct);
                Assert.AreEqual(parameters.ProductCode, additiveProduct.Product.ProductCode);
                Assert.AreEqual(parameters.ProductName, additiveProduct.Product.Name);
                Assert.AreEqual(parameters.IsActive, additiveProduct.Product.IsActive);
                Assert.AreEqual(expectedAdditiveTypeKey.KeyValue, new AdditiveTypeKey(additiveProduct).KeyValue);
            }
        }

        [TestFixture]
        public class CreateChileProduct : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ChileType_could_not_be_found()
            {
                //Arrange
                var parameters = new CreateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductCode = "Product Code",
                        ProductName = "Product Name",
                        ChileTypeKey = new ChileTypeKey().KeyValue
                    };

                //Act
                var result = Service.CreateChileProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.ChileTypeNotFound);
            }

            [Test]
            public void Creates_ChileProduct_and_Product_records_as_expected()
            {
                //Arrange
                var chileTypeKey = new ChileTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>());
                var parameters = new CreateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductCode = "Product Code",
                        ProductName = "Product Name",
                        ChileTypeKey = chileTypeKey.KeyValue,
                        ChileState = ChileStateEnum.FinishedGoods,
                        Mesh = 321.0,
                        IngredientsDescription = "100% Rodent Hair"
                    };

                //Act
                var result = Service.CreateChileProduct(parameters);

                //Assert
                result.AssertSuccess();
                var chileProductResult = RVCUnitOfWork.ChileProductRepository.All().Select(c => new { chileProduct = c, c.Product }).SingleOrDefault();
                Assert.NotNull(chileProductResult);

                var chileProduct = chileProductResult.chileProduct;
                Assert.AreEqual(parameters.ProductCode, chileProduct.Product.ProductCode);
                Assert.AreEqual(parameters.ProductName, chileProduct.Product.Name);
                Assert.IsTrue(chileProduct.Product.IsActive);
                Assert.AreEqual(parameters.ChileState, chileProduct.ChileState);
                Assert.AreEqual(chileTypeKey.KeyValue, chileProduct.ToChileTypeKey().KeyValue);
                Assert.AreEqual(parameters.Mesh, chileProduct.Mesh);
                Assert.AreEqual(parameters.IngredientsDescription, chileProduct.IngredientsDescription);
            }

            [Test]
            public void Sets_attribuge_range_and_ingredient_records_as_expected()
            {
                //Arrange
                var chileType = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>();

                var additiveTypes = new List<AdditiveType>();
                var attributes = new List<AttributeName>();
                for(var i = 0; i < 3; ++i)
                {
                    additiveTypes.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>());
                    attributes.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.ValidForChileInventory = true));
                }

                var parameters = new CreateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductCode = "123",
                        ProductName = "kore wa watashi no namae",
                        ChileTypeKey = chileType.ToChileTypeKey(),
                        ChileState = ChileStateEnum.FinishedGoods,

                        Ingredients = additiveTypes.Select((a, n) => new SetChileProductIngredientParameters
                            {
                                AdditiveTypeKey = a.ToAdditiveTypeKey(),
                                Percentage = n + 1
                            }),
                        AttributeRanges = attributes.Select((a, n) => new SetAttributeRangeParameters
                            {
                                AttributeNameKey = a.ToAttributeNameKey(),
                                RangeMin = n, 
                                RangeMax = n + 1
                            })
                    };

                //Act
                var result = Service.CreateChileProduct(parameters);

                //Assert
                result.AssertSuccess();
                var chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => true, c => c.ProductAttributeRanges, c => c.Ingredients).SingleOrDefault();
                parameters.Ingredients.AssertEquivalent(chileProduct.Ingredients, e => e.AdditiveTypeKey, r => r.ToAdditiveTypeKey().KeyValue,
                    (e, n) => Assert.AreEqual(e.Percentage, n.Percentage));
                parameters.AttributeRanges.AssertEquivalent(chileProduct.ProductAttributeRanges, e => e.AttributeNameKey, r => r.ToAttributeNameKey().KeyValue,
                    (e, n) =>
                        {
                            Assert.AreEqual(e.RangeMin, n.RangeMin);
                            Assert.AreEqual(e.RangeMax, n.RangeMax);
                        });
            }
        }

        [TestFixture]
        public class UpdateChileProduct : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_ChileProduct_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = new ProductKey(),
                        ChileTypeKey = new ChileTypeKey()
                    };

                //Act
                var result = Service.UpdateChileProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.ChileProductNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_ChileType_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdateChileProductParameters
                {
                    ProductKey = new ProductKey((IProductKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>()).KeyValue,
                    ChileTypeKey = new ChileTypeKey().KeyValue
                };

                //Act
                var result = Service.UpdateChileProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.ChileTypeNotFound);
            }

            [Test]
            public void Updates_ChileProduct_and_Product_records_as_expected()
            {
                //Arrage
                var chileProductKey = new ProductKey((IProductKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>());
                var expectedChileTypeKey = new ChileTypeKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileType>());

                var parameters = new UpdateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = chileProductKey.KeyValue,
                        ProductCode = "Product Code",
                        ProductName = "Product Name",
                        IsActive = false,
                        ChileTypeKey = expectedChileTypeKey.KeyValue,
                        Mesh = 123.0,
                        IngredientsDescription = "Insect Parts. Lots."
                    };
                
                //Act
                var result = Service.UpdateChileProduct(parameters);

                //Assert
                result.AssertSuccess();
                var chileProductResult = RVCUnitOfWork.ChileProductRepository.All().Select(c => new { chileProduct = c, c.Product }).SingleOrDefault();
                Assert.NotNull(chileProductResult);

                var chileProduct = chileProductResult.chileProduct;
                Assert.AreEqual(parameters.ProductName, chileProduct.Product.Name);
                Assert.AreEqual(parameters.ProductCode, chileProduct.Product.ProductCode);
                Assert.AreEqual(parameters.IsActive, chileProduct.Product.IsActive);
                Assert.AreEqual(parameters.ChileTypeKey, chileProduct.ToChileTypeKey().KeyValue);
                Assert.AreEqual(parameters.Mesh, chileProduct.Mesh);
                Assert.AreEqual(parameters.IngredientsDescription, chileProduct.IngredientsDescription);
            }

            [Test]
            public void Sets_attribuge_range_and_ingredient_records_as_expected()
            {
                //Arrange
                var chileProduct = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ChileProduct>();

                var additiveTypes = new List<AdditiveType>();
                var attributes = new List<AttributeName>();
                for(var i = 0; i < 3; ++i)
                {
                    additiveTypes.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>());
                    attributes.Add(TestHelper.CreateObjectGraphAndInsertIntoDatabase<AttributeName>(a => a.ValidForChileInventory = true));
                }

                var parameters = new UpdateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = chileProduct.ToChileProductKey(),
                        ProductCode = "123",
                        ProductName = "kore wa watashi no namae",
                        ChileTypeKey = chileProduct.ToChileTypeKey(),

                        Ingredients = additiveTypes.Select((a, n) => new SetChileProductIngredientParameters
                            {
                                AdditiveTypeKey = a.ToAdditiveTypeKey(),
                                Percentage = n + 1
                            }),
                        AttributeRanges = attributes.Select((a, n) => new SetAttributeRangeParameters
                            {
                                AttributeNameKey = a.ToAttributeNameKey(),
                                RangeMin = n, 
                                RangeMax = n + 1
                            })
                    };

                //Act
                var result = Service.UpdateChileProduct(parameters);

                //Assert
                result.AssertSuccess();
                chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => true, c => c.ProductAttributeRanges, c => c.Ingredients).SingleOrDefault();
                parameters.Ingredients.AssertEquivalent(chileProduct.Ingredients, e => e.AdditiveTypeKey, r => r.ToAdditiveTypeKey().KeyValue,
                    (e, n) => Assert.AreEqual(e.Percentage, n.Percentage));
                parameters.AttributeRanges.AssertEquivalent(chileProduct.ProductAttributeRanges, e => e.AttributeNameKey, r => r.ToAttributeNameKey().KeyValue,
                    (e, n) =>
                        {
                            Assert.AreEqual(e.RangeMin, n.RangeMin);
                            Assert.AreEqual(e.RangeMax, n.RangeMax);
                        });
            }
        }

        [TestFixture]
        public class CreatePackagingProduct : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Weight_is_less_than_0()
            {
                //Arrange
                var parameters = new CreatePackagingProductParameters
                    {
                        UserToken = "User",
                        ProductCode = "Code",
                        ProductName = "Name",
                        Weight = -0.1
                    };

                //Act
                var result = Service.CreatePackagingProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.NegativeWeight);
            }

            [Test]
            public void Creates_PackagingProduct_and_Product_records_as_expected()
            {
                //Arrange
                
                
                var parameters = new CreatePackagingProductParameters
                {
                    UserToken = "User",
                    ProductCode = "Code",
                    ProductName = "Name",
                    Weight = 0.0
                };

                //Act
                var result = Service.CreatePackagingProduct(parameters);

                //Assert
                result.AssertSuccess();
                var packagingProductResult = RVCUnitOfWork.PackagingProductRepository.All().Select(p => new { packagingProduct = p, p.Product }).SingleOrDefault();
                Assert.NotNull(packagingProductResult);

                var packagingProduct = packagingProductResult.packagingProduct;
                Assert.AreEqual(parameters.ProductCode, packagingProduct.Product.ProductCode);
                Assert.AreEqual(parameters.ProductName, packagingProduct.Product.Name);
                Assert.IsTrue(packagingProduct.Product.IsActive);
                Assert.AreEqual(parameters.Weight, packagingProduct.Weight);
            }
        }

        [TestFixture]
        public class UpdatePackagingProduct : ProductServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_PackagingProduct_could_not_be_found()
            {
                //Arrange
                var parameters = new UpdatePackagingProductParameters
                {
                    UserToken = "User",
                    ProductKey = new ProductKey().KeyValue,
                    ProductCode = "Product Code",
                    ProductName = "Product Name",
                    IsActive = false,
                    Weight = 0.0
                };

                //Act
                var result = Service.UpdatePackagingProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.PackagingProductNotFound);
            }

            [Test]
            public void Returns_non_successful_result_if_Weight_is_less_than_0()
            {
                //Arrange
                var parameters = new UpdatePackagingProductParameters
                {
                    UserToken = "User",
                    ProductKey = new ProductKey((IProductKey) TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>()).KeyValue,
                    ProductCode = "Product Code",
                    ProductName = "Product Name",
                    IsActive = false,
                    Weight = -0.001
                };

                //Act
                var result = Service.UpdatePackagingProduct(parameters);

                //Assert
                result.AssertNotSuccess(UserMessages.NegativeWeight);
            }

            [Test]
            public void Updates_PackagingProduct_and_Product_records_as_expected()
            {
                //Arrage
                var packagingProductKey = new ProductKey((IProductKey)TestHelper.CreateObjectGraphAndInsertIntoDatabase<PackagingProduct>());

                

                var parameters = new UpdatePackagingProductParameters
                {
                    UserToken = "User",
                    ProductKey = packagingProductKey.KeyValue,
                    ProductCode = "Product Code",
                    ProductName = "Product Name",
                    IsActive = false,
                    Weight = 101
                };

                //Act
                var result = Service.UpdatePackagingProduct(parameters);

                //Assert
                result.AssertSuccess();
                var packagingProductResult = RVCUnitOfWork.PackagingProductRepository.All().Select(p => new { packagingProduct = p, p.Product }).SingleOrDefault();
                Assert.NotNull(packagingProductResult);

                var packagingProduct = packagingProductResult.packagingProduct;
                Assert.AreEqual(parameters.ProductName, packagingProduct.Product.Name);
                Assert.AreEqual(parameters.ProductCode, packagingProduct.Product.ProductCode);
                Assert.AreEqual(parameters.IsActive, packagingProduct.Product.IsActive);
                Assert.AreEqual(parameters.Weight, packagingProduct.Weight);
            }
        }

        [TestFixture]
        public class CreateNonInventoryProduct : ProductServiceTests
        {
            [Test]
            public void Creates_Product_record_as_expected()
            {
                //Act
                var result = Service.CreateNonInventoryProduct(new CreateProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductCode = "123",
                        ProductName = "test"
                    });

                //Assert
                result.AssertSuccess();

                var product = RVCUnitOfWork.ProductRepository.FindByKey(KeyParserHelper.ParseResult<IProductKey>(result.ResultingObject).ResultingObject.ToProductKey());
                Assert.AreEqual(ProductTypeEnum.NonInventory, product.ProductType);
            }
        }

        [TestFixture]
        public class UpdateNonInventoryProduct : ProductServiceTests
        {
            [Test]
            public void Updates_Product_record_as_expected()
            {
                //Arrange
                var product = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Product>(p => p.ProductType = ProductTypeEnum.NonInventory);
                var productKey = product.ToProductKey();
                var parameters = new UpdateProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = productKey,
                        ProductCode = "123",
                        ProductName = "test"
                    };

                //Act
                var result = Service.UpdateNonInventoryProduct(parameters);

                //Assert
                result.AssertSuccess();

                product = RVCUnitOfWork.ProductRepository.FindByKey(productKey);
                Assert.AreEqual(ProductTypeEnum.NonInventory, product.ProductType);
                Assert.AreEqual(parameters.ProductName, product.Name);
            }
        }

        [TestFixture]
        public class GetAdditiveTypes : ProductServiceTests
        {
            [Test]
            public void Returns_empty_results_if_no_AdditiveType_records_exist()
            {
                //Act
                var result = Service.GetAdditiveTypes();

                //Assert
                result.AssertSuccess();
                Assert.IsEmpty(result.ResultingObject);
            }

            [Test]
            public void Returns_AdditiveType_results_as_expected()
            {
                //Arrange
                var additiveType0 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>();
                var additiveType1 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>();
                var additiveType2 = TestHelper.CreateObjectGraphAndInsertIntoDatabase<AdditiveType>();

                //Act
                var result = Service.GetAdditiveTypes();

                //Assert
                result.AssertSuccess();
                var results = result.ResultingObject.ToList();

                Assert.AreEqual(additiveType0.Description, results.Single(r => new AdditiveTypeKey(additiveType0) == r.AdditiveTypeKey).AdditiveTypeDescription);
                Assert.AreEqual(additiveType1.Description, results.Single(r => new AdditiveTypeKey(additiveType1) == r.AdditiveTypeKey).AdditiveTypeDescription);
                Assert.AreEqual(additiveType2.Description, results.Single(r => new AdditiveTypeKey(additiveType2) == r.AdditiveTypeKey).AdditiveTypeDescription);
            }
        }
    }
}