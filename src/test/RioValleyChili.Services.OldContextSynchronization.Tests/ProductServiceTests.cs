using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;
using SetChileProductAttributeRangesParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetChileProductAttributeRangesParameters;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        [TestFixture]
        public class SynchronizeProductUnitTests : SynchronizeOldContextUnitTestsBase<IResult<ProductKey>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.Product; } }
        }

        [TestFixture]
        public class SynchronizeSetChileProductIngredientsUnitTests : SynchronizeOldContextUnitTestsBase<IResult<SyncProductParameters>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.ChileProductIngredients; } }
        }

        [TestFixture]
        public class CreateChileProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Creates_new_tblProduct_record_as_expected()
            {
                //Arrange
                var chileType = RVCUnitOfWork.ChileTypeRepository.Filter(t => true).FirstOrDefault();
                if(chileType == null)
                {
                    Assert.Inconclusive("No suitable ChileType to test.");
                }

                var additiveTypes = RVCUnitOfWork.AdditiveTypeRepository.Filter(a => true).Take(3).ToList()
                    .Select((a, n) => new
                        {
                            AdditiveType = a,
                            Percentage = n + 1
                        })
                    .ToList();

                var parameters = new CreateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileState = ChileStateEnum.FinishedGoods,
                        ChileTypeKey = chileType.ToChileTypeKey(),
                        ProductName = "Test Chile Product",
                        ProductCode = "1234",
                        IngredientsDescription = "lotsa stuff - and things as well",
                        Ingredients = additiveTypes.Select(a => new SetChileProductIngredientParameters
                            {
                                AdditiveTypeKey = a.AdditiveType.ToAdditiveTypeKey(),
                                Percentage = a.Percentage
                            }),

                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                new SetAttributeRangeParameters
                                    {
                                        AttributeNameKey = StaticAttributeNames.Asta.ToAttributeNameKey(),
                                        RangeMin = 1,
                                        RangeMax = 2
                                    }
                            }
                    };

                //Act
                var result = Service.CreateChileProduct(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Product);
                    Assert.AreEqual(int.Parse(parameters.ProductCode), tblProduct.ProdID);

                    Assert.AreEqual(1, (double)tblProduct.AstaStart, 0.01);
                    Assert.AreEqual(2, (double)tblProduct.AstaEnd, 0.01);
                    Assert.AreEqual(parameters.IngredientsDescription, tblProduct.IngrDesc);

                    var tblIngrs = oldContext.tblProductIngrs.Where(i => i.ProdID == prodId).ToList();
                    foreach(var additive in additiveTypes)
                    {
                        var tblIngr = tblIngrs.FirstOrDefault(i => i.IngrID == additive.AdditiveType.Id);
                        if(tblIngr != null)
                        {
                            Assert.AreEqual(additive.Percentage, (double)tblIngr.Percentage, 0.01);
                        }
                    }
                }
            }
        }

        [TestFixture]
        public class UpdateChileProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Updates_tblProduct_record_as_expected()
            {
                //Arrange
                var chileProduct = RVCUnitOfWork
                    .ChileProductRepository
                    .FindBy(c => c.ProductAttributeRanges.Any(r => r.AttributeShortName == StaticAttributeNames.Asta.ShortName),
                    c => c.Product,
                    c => c.ProductAttributeRanges);
                if(chileProduct == null)
                {
                    Assert.Inconclusive("No suitable ChileProduct to test.");
                }

                var additiveTypes = RVCUnitOfWork.AdditiveTypeRepository.Filter(a => true).Take(3).ToList()
                    .Select((a, n) => new
                        {
                            AdditiveType = a,
                            Percentage = n + 1
                        })
                    .ToList();
                
                var parameters = new UpdateChileProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = chileProduct.ToChileProductKey(),
                        ChileTypeKey = chileProduct.ToChileTypeKey(),
                        ProductName = chileProduct.Product.Name,
                        ProductCode = chileProduct.Product.ProductCode,
                        IngredientsDescription = ". . . .",

                        Ingredients = additiveTypes.Select(a => new SetChileProductIngredientParameters
                            {
                                AdditiveTypeKey = a.AdditiveType.ToAdditiveTypeKey(),
                                Percentage = a.Percentage
                            }),

                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                new SetAttributeRangeParameters
                                    {
                                        AttributeNameKey = StaticAttributeNames.Asta.ToAttributeNameKey(),
                                        RangeMin = 1,
                                        RangeMax = 2
                                    }
                            }
                    };

                //Act
                var result = Service.UpdateChileProduct(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Product);
                    Assert.AreEqual(int.Parse(parameters.ProductCode), tblProduct.ProdID);

                    Assert.AreEqual(1, (double)tblProduct.AstaStart, 0.01);
                    Assert.AreEqual(2, (double)tblProduct.AstaEnd, 0.01);
                    Assert.AreEqual(parameters.IngredientsDescription, tblProduct.IngrDesc);

                    var tblIngrs = oldContext.tblProductIngrs.Where(i => i.ProdID == prodId).ToList();
                    foreach(var additive in additiveTypes)
                    {
                        var tblIngr = tblIngrs.FirstOrDefault(i => i.IngrID == additive.AdditiveType.Id);
                        if(tblIngr != null)
                        {
                            Assert.AreEqual(additive.Percentage, (double)tblIngr.Percentage, 0.01);
                        }
                    }
                }
            }
        }

        [TestFixture]
        public class CreateAdditiveProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Creates_new_tblProduct_record_as_expected()
            {
                //Arrange
                var additiveType = RVCUnitOfWork.AdditiveTypeRepository.FindBy(t => true);
                if(additiveType == null)
                {
                    Assert.Inconclusive("No suitable AdditiveType to test.");
                }

                var parameters = new CreateAdditiveProductParameters
                    {
                        UserToken = TestUser.UserName,
                        AdditiveTypeKey = additiveType.ToAdditiveTypeKey(),
                        ProductName = "Test Additive Product",
                        ProductCode = "12345"
                    };

                //Act
                var result = Service.CreateAdditiveProduct(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Product);
                    Assert.AreEqual(int.Parse(parameters.ProductCode), tblProduct.ProdID);
                    Assert.AreEqual(additiveType.Id, tblProduct.IngrID);
                }
            }
        }

        [TestFixture]
        public class UpdateAdditiveProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Updates_tblProduct_record_as_expected()
            {
                //Arrange
                var additiveProduct = RVCUnitOfWork
                    .AdditiveProductRepository
                    .FindBy(a => true, c => c.Product);
                if(additiveProduct == null)
                {
                    Assert.Inconclusive("No suitable AdditiveProduct to test.");
                }
                
                var parameters = new UpdateAdditiveProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = additiveProduct.ToAdditiveProductKey(),
                        AdditiveTypeKey = additiveProduct.ToAdditiveTypeKey(),
                        ProductName = additiveProduct.Product.Name,
                        ProductCode = additiveProduct.Product.ProductCode
                    };

                //Act
                var result = Service.UpdateAdditiveProduct(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Product);
                    Assert.AreEqual(int.Parse(parameters.ProductCode), tblProduct.ProdID);
                    Assert.AreEqual(additiveProduct.AdditiveTypeId, tblProduct.IngrID);
                }
            }
        }

        [TestFixture]
        public class CreatePackagingProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Creates_new_tblPackaging_record_as_expected()
            {
                //Arrange
                var parameters = new CreatePackagingProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductName = "Test Packaging Product",
                        ProductCode = "123456",
                        Weight = 1, 
                        PackagingWeight = 2, 
                        PalletWeight = 3
                    };

                //Act
                var result = Service.CreatePackagingProduct(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblPackaging);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var pkgId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblPackagings.FirstOrDefault(p => p.PkgID == pkgId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Packaging);
                    Assert.AreEqual(int.Parse(parameters.ProductCode), tblProduct.PkgID);
                    Assert.AreEqual(parameters.Weight, tblProduct.NetWgt);
                    Assert.AreEqual(parameters.PackagingWeight, tblProduct.PkgWgt);
                    Assert.AreEqual(parameters.PalletWeight, tblProduct.PalletWgt);
                }
            }
        }

        [TestFixture]
        public class UpdatePackagingProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Updates_tblPackaging_record_as_expected()
            {
                //Arrange
                var packaging = RVCUnitOfWork.PackagingProductRepository.FindBy(p => true, p => p.Product);
                if(packaging == null)
                {
                    Assert.Inconclusive("No suitable PackagingProduct to test.");
                }

                var parameters = new UpdatePackagingProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductKey = packaging.ToPackagingProductKey(),
                        ProductName = "Test Packaging Product",
                        ProductCode = packaging.Product.ProductCode,
                        Weight = packaging.Weight,
                        PackagingWeight = packaging.PackagingWeight,
                        PalletWeight = packaging.PalletWeight
                    };

                //Act
                var result = Service.UpdatePackagingProduct(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblPackaging);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var pkgId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblPackagings.FirstOrDefault(p => p.PkgID == pkgId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Packaging);
                    Assert.AreEqual(int.Parse(parameters.ProductCode), tblProduct.PkgID);
                    Assert.AreEqual(parameters.Weight, tblProduct.NetWgt);
                    Assert.AreEqual(parameters.PackagingWeight, tblProduct.PkgWgt);
                    Assert.AreEqual(parameters.PalletWeight, tblProduct.PalletWgt);
                }
            }
        }

        [TestFixture]
        public class CreateNonInventoryProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Creates_tblProduct_record_as_expected()
            {
                //Arrange
                int prodId;
                using(var context = new RioAccessSQLEntities())
                {
                    prodId = context.tblProducts.Select(p => p.ProdID).DefaultIfEmpty(0).Max() + 1;
                }

                //Act
                var parameters = new CreateProductParameters
                    {
                        UserToken = TestUser.UserName,
                        ProductCode = prodId.ToString(),
                        ProductName = "ProductNameTest"
                    };
                var result = Service.CreateNonInventoryProduct(parameters);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Product);
                    Assert.AreEqual(parameters.ProductCode, tblProduct.ProdID.ToString());
                    Assert.AreEqual(7, tblProduct.ProdGrpID);
                    Assert.AreEqual(3, tblProduct.PTypeID);
                    Assert.AreEqual(false, tblProduct.InActive);
                }
            }
        }

        [TestFixture]
        public class UpdataeNonInventoryProduct : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Updates_tblProduct_record_as_expected()
            {
                //Arrange
                var product = TestHelper.Context.Products.FirstOrDefault(p => p.ProductType == ProductTypeEnum.NonInventory);
                if(product == null)
                {
                    Assert.Inconclusive("No suitable non-inventory product found for testing.");
                }

                //Act
                var parameters = new UpdateProductParameters
                    {
                        ProductKey = product.ToProductKey(),
                        UserToken = TestUser.UserName,
                        ProductCode = product.ProductCode,
                        ProductName = product.Name,
                        IsActive = !product.IsActive
                    };
                var result = Service.UpdateNonInventoryProduct(parameters);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(parameters.ProductName, tblProduct.Product);
                    Assert.AreEqual(parameters.ProductCode, tblProduct.ProdID.ToString());
                    Assert.AreEqual(parameters.IsActive, !tblProduct.InActive);
                }
            }
        }

        [TestFixture]
        public class SetChileProductAttributeRanges : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Updates_tblProduct_record_as_expected()
            {
                //Arrange
                var range = RVCUnitOfWork.ChileProductAttributeRangeRepository.FindBy(r => r.AttributeShortName == StaticAttributeNames.Asta.ShortName);
                if(range == null)
                {
                    Assert.Inconclusive("No suitable ChileProductAttributeRange to test.");
                }

                var asta = new SetAttributeRangeParameters
                    {
                        AttributeNameKey = range.ToAttributeNameKey(),
                        RangeMin = range.RangeMin == 1 ? 2 : 1,
                        RangeMax = range.RangeMax == 333 ? 444 : 333
                    };
                var gran = new SetAttributeRangeParameters
                    {
                        AttributeNameKey = StaticAttributeNames.Granularity.ToAttributeNameKey(),
                        RangeMin = 101,
                        RangeMax = 102
                    };

                var parameters = new SetChileProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = range.ToChileProductKey(),
                        AttributeRanges = new List<ISetAttributeRangeParameters>
                            {
                                asta,
                                gran,
                            }
                    };

                //Act
                var result = Service.SetChileProductAttributeRanges(parameters);
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProduct = oldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
                    Assert.AreEqual(asta.RangeMin, tblProduct.AstaStart);
                    Assert.AreEqual(asta.RangeMax, tblProduct.AstaEnd);
                    Assert.AreEqual(gran.RangeMin, tblProduct.GranStart);
                    Assert.AreEqual(gran.RangeMax, tblProduct.GranEnd);
                }
            }
        }

        [TestFixture]
        public class SetChileProductIngredients : SynchronizeOldContextIntegrationTestsBase<ProductService>
        {
            [Test]
            public void Sets_tblProductIngrs_records_as_expected()
            {
                //Arrange
                var chileProduct = RVCUnitOfWork.ChileProductRepository
                    .FindBy(c => c.Product.ProductCode != null && c.Ingredients.Count > 1, c => c.Ingredients);
                if(chileProduct == null)
                {
                    Assert.Inconclusive("No suitable ChileProduct found to test.");
                }

                var except = chileProduct.Ingredients.Where((i, n) => n > 0).Select(i => i.AdditiveTypeId).ToList();
                var additiveTypes = chileProduct.Ingredients.Where((i, n) => n == 0)
                    .Select(n => n.ToAdditiveTypeKey())
                    .Concat(RVCUnitOfWork.AdditiveTypeRepository
                        .All().ToList()
                        .Where(a => !except.Contains(a.Id))
                        .Take(2)
                        .ToList()
                        .Select(a => a.ToAdditiveTypeKey()));

                //Act
                var ingredients = additiveTypes.Select((a, n) => new
                    {
                        additiveType = a,
                        parameters = new SetChileProductIngredientParameters
                            {
                                AdditiveTypeKey = a,
                                Percentage = (n + 1) / 10.0f
                            }
                    }).ToList();
                var result = Service.SetChileProductIngredients(new SetChileProductIngredientsParameters
                    {
                        UserToken = TestUser.UserName,
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        Ingredients = ingredients.Select(i => i.parameters).ToList()
                    });
                result.AssertSuccess();
                var resultString = GetKeyFromConsoleString(ConsoleOutput.SynchedTblProduct);

                //Assert
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var prodId = int.Parse(resultString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblProductIngrs = oldContext.tblProductIngrs.Where(i => i.ProdID == prodId).ToList();
                    foreach(var ingredient in ingredients)
                    {
                        Assert.AreEqual(ingredient.parameters.Percentage, (double)tblProductIngrs.Single(i => i.IngrID == ingredient.additiveType.AdditiveTypeKey_AdditiveTypeId).Percentage, 0.01);
                    }
                }
            }
        }
    }
}