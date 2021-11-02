using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;
using SetPickedInventoryParameters = RioValleyChili.Services.Tests.IntegrationTests.Parameters.SetPickedInventoryParameters;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class SalesServiceTests
    {
        [TestFixture]
        public class SynchronizeCustomerSpecsUnitTests : SynchronizeOldContextUnitTestsBase<IResult, SynchronizeCustomerProductSpecs>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncCustomerSpecs; } }
        }

        [TestFixture]
        public class SynchronizeContractUnitTests : SynchronizeOldContextUnitTestsBase<IResult<string>, SyncCustomerContractParameters>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncContract; } }
        }

        [TestFixture]
        public class SynchronizeContractsStatusUnitTests : SynchronizeOldContextUnitTestsBase<IResult, List<ContractKey>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncContractsStatus; } }
        }

        [TestFixture]
        public class DeleteContractUnitTests : SynchronizeOldContextUnitTestsBase<IResult<int?>, int?>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.DeleteContract; } }
        }

        [TestFixture]
        public class CompleteExpiredContractsUnitTests : SynchronizeOldContextUnitTestsBase<IResult<List<Contract>>, List<Contract>>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.CompleteExpiredContracts; } }
        }

        [TestFixture]
        public class PostInvoiceUnitTests : SynchronizeOldContextUnitTestsBase<IResult, SalesOrderKey>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SyncPostInvoice; } }
        }

        [TestFixture]
        public class SalesQuoteUnitTests : SynchronizeOldContextUnitTestsBase<IResult, SyncSalesQuoteParameters>
        {
            protected override NewContextMethod NewContextMethod { get { return NewContextMethod.SalesQuote; } }
        }

        [TestFixture]
        public class SetCustomerChileProductAttributeRange : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Creates_new_SerializedCustomerProdSpecs_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.Filter(c => !c.ProductSpecs.Any(),
                    c => c.Company).FirstOrDefault();
                if(customer == null)
                {
                    Assert.Inconclusive("No Customer without ProductSpecs found.");
                }

                var chileProduct = RVCUnitOfWork.ChileProductRepository.Filter(c => true, c => c.Product).FirstOrDefault();
                if(chileProduct == null)
                {
                    Assert.Inconclusive("No ChileProduct found.");
                }

                var attribute = RVCUnitOfWork.AttributeNameRepository.All().FirstOrDefault();
                if(attribute == null)
                {
                    Assert.Inconclusive("No AttributeName found.");
                }

                //Act
                var result = Service.SetCustomerChileProductAttributeRanges(new SetCustomerProductAttributeRangesParameters
                    {
                        UserToken = TestUser.UserName,
                        CustomerKey = customer.ToCustomerKey(),
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        AttributeRanges = new List<ISetCustomerProductAttributeRangeParameters>
                            {
                                new SetCustomerProductAttributeRangeParameters
                                    {
                                        AttributeNameKey = attribute.ToAttributeNameKey(),
                                        RangeMin = 1.0f,
                                        RangeMax = 2.0f
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var prodId = int.Parse(chileProduct.Product.ProductCode);
                    var specs = oldContext.SerializedCustomerProdSpecs.First(s => s.ProdID == prodId && s.Company_IA == customer.Company.Name);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(specs.Serialized));
                }
            }
        }

        [TestFixture]
        public class RemoveCustomerChileProductAttributeRange : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Removes_SerializedCustomerProdSpecs_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.Filter(c => c.ProductSpecs.Any(),
                    c => c.Company,
                    c => c.ProductSpecs.Select(s => s.ChileProduct.Product)).FirstOrDefault();
                if(customer == null)
                {
                    Assert.Inconclusive("No Customer without ProductSpecs found.");
                }

                var chileProduct = customer.ProductSpecs.Select(s => s.ChileProduct).FirstOrDefault();
                if(chileProduct == null)
                {
                    Assert.Inconclusive("No ChileProduct found.");
                }

                var attribute = RVCUnitOfWork.AttributeNameRepository.All().FirstOrDefault();
                if(attribute == null)
                {
                    Assert.Inconclusive("No AttributeName found.");
                }

                //Act
                var result = Service.RemoveCustomerChileProductAttributeRanges(new RemoveCustomerChileProductAttributeRangesParameters
                    {
                        ChileProductKey = chileProduct.ToChileProductKey(),
                        CustomerKey = customer.ToCustomerKey()
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var prodId = int.Parse(chileProduct.Product.ProductCode);
                    var specs = oldContext.SerializedCustomerProdSpecs.FirstOrDefault(s => s.ProdID == prodId && s.Company_IA == customer.Company.Name);
                    Assert.IsNull(specs);
                }
            }
        }

        [TestFixture]
        public class CreateContract : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Creates_new_Contract_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.All().FirstOrDefault();
                var warehouse = RVCUnitOfWork.FacilityRepository.All().FirstOrDefault();
                var chileProduct = RVCUnitOfWork.ChileProductRepository.All().FirstOrDefault();
                var packagingProduct = RVCUnitOfWork.PackagingProductRepository.All().FirstOrDefault(p => p.Weight > 0);
                var treatment = RVCUnitOfWork.TreatmentOrderRepository.All().FirstOrDefault();

                //Act
                var result = Service.CreateCustomerContract(new CreateContractParameters
                {
                    UserToken = TestUser.UserName,
                    CustomerKey = new CustomerKey((ICustomerKey)customer),
                    DefaultPickFromFacilityKey = new FacilityKey(warehouse),
                    ContractDate = DateTime.Today,
                    ContractItems = new List<ContractItemParameters>
                            {
                                new ContractItemParameters
                                    {
                                        Quantity = 1,
                                        ChileProductKey = new ChileProductKey(chileProduct),
                                        PackagingProductKey = new PackagingProductKey(packagingProduct),
                                        TreatmentKey = new InventoryTreatmentKey(treatment)
                                    }
                            },
                    Comments = new[]
                            {
                                "First comment.",
                                "Second comment."
                            }
                });
                var contractKeyString = GetKeyFromConsoleString(ConsoleOutput.SynchronizedContract);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var contractId = int.Parse(contractKeyString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var contract = oldContext.tblContracts.Include("tblContractDetails").FirstOrDefault(c => c.ContractID == contractId);
                    Assert.NotNull(contract);
                }
            }
        }

        [TestFixture]
        public class SetCustomerContractsStatus : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Updates_tblContract_records_status_as_expected()
            {
                //Arrange
                var contracts = RVCUnitOfWork.ContractRepository.All()
                    .Where(c => c.ContractStatus != ContractStatus.Rejected)
                    .OrderByDescending(c => c.ContractDate).Take(3).ToList();

                //Act
                var result = Service.SetCustomerContractsStatus(new SetContractsStatusParameters
                    {
                        ContractStatus = ContractStatus.Rejected,
                        ContractKeys = contracts.Select(c => c.ToContractKey().KeyValue)
                    });
                var contractKeyString = GetKeyFromConsoleString(ConsoleOutput.SyncContractsStatus);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                
                var ids = contractKeyString.Split(',').Select(c => int.Parse(c.Trim(' '))).ToArray();
                using(var oldContext = new RioAccessSQLEntities())
                {
                    foreach(var id in ids)
                    {
                        Assert.AreEqual("Rejected", oldContext.tblContracts.FirstOrDefault(n => n.ContractID == id).KStatus);
                    }
                }
            }
        }

        [TestFixture]
        public class DeleteContract : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Deletes_tblContract_and_tblContractDetail_items_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                var contract = RVCUnitOfWork.ContractRepository.All()
                    .FirstOrDefault(c => c.ContractId != null && c.ContractItems.Any() && c.ContractItems.All(i => !i.OrderItems.Any()));
                if(contract == null)
                {
                    Assert.Inconclusive("Could not find Contract record suitable for testing.");
                }

                //Act
                var result = Service.RemoveCustomerContract(new ContractKey(contract));
                var contractKeyString = GetKeyFromConsoleString(ConsoleOutput.RemovedContract);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var contractId = int.Parse(contractKeyString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNull(oldContext.tblContracts.FirstOrDefault(c => c.ContractID == contractId));
                }
            }
        }

        [TestFixture]
        public class CompleteExpiredContract : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Sets_expired_contract_statuses_to_complete()
            {
                //Arrange
                var now = DateTime.UtcNow;
                var contracts = RVCUnitOfWork.ContractRepository.Filter(c => now >= c.TermEnd && c.ContractStatus == ContractStatus.Confirmed).ToList();

                //Act
                var result = Service.CompleteExpiredContracts();

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());
                using(var oldContext = new RioAccessSQLEntities())
                {
                    foreach(var contract in contracts)
                    {
                        Assert.AreEqual(ContractStatus.Completed.ToString(), oldContext.tblContracts.FirstOrDefault(c => c.ContractID == contract.ContractId).KStatus);
                    }
                }
            }
        }

        [TestFixture]
        public class CreateSalesOrder : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Creates_new_tblOrder_record()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.All().FirstOrDefault();
                var contractItem = RVCUnitOfWork.ContractItemRepository.All().FirstOrDefault(i => i.Contract.CustomerId == customer.Id);
                var sourceFacility = RVCUnitOfWork.FacilityRepository.All().FirstOrDefault();
                RVCUnitOfWork.FacilityRepository.Filter(f => f.Id != sourceFacility.Id).FirstOrDefault();

                var parameters = TestHelper.CreateObjectGraph<CreateSalesOrderParameters>();
                parameters.UserToken = TestUser.UserName;
                parameters.CustomerKey = new CustomerKey((ICustomerKey)customer);
                parameters.FacilitySourceKey = new FacilityKey(sourceFacility);
                parameters.OrderItems = new List<SalesOrderItemParameters>
                    {
                        new SalesOrderItemParameters
                            {
                                ContractItemKey = contractItem.ToContractItemKey(),
                                ProductKey = contractItem.ToChileProductKey(),
                                PackagingKey = contractItem.ToPackagingProductKey(),
                                TreatmentKey = contractItem.ToInventoryTreatmentKey(),
                                Quantity = 123,
                                CustomerLotCode = "LotCode",
                                PriceBase = 1,
                                PriceFreight = 2,
                                PriceTreatment = 3,
                                PriceWarehouse = 4,
                                PriceRebate = 5
                            }
                    };

                //Act
                var result = Service.CreateSalesOrder(parameters);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.SyncTblOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders
                        .Include("tblOrderDetails")
                        .FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.IsNotNull(tblOrder);
                    Assert.IsNotNull(tblOrder.tblOrderDetails.First());
                }
            }

            [Test]
            public void Removes_tblOrderDetail_record_as_expected()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.All().FirstOrDefault();
                var contractItem = RVCUnitOfWork.ContractItemRepository.All().FirstOrDefault(i => i.Contract.CustomerId == customer.Id);
                var inventory = RVCUnitOfWork.InventoryRepository.Filter(i =>
                        i.LotTypeId == (decimal)LotTypeEnum.FinishedGood &&
                        i.Lot.ProductionStatus == LotProductionStatus.Produced &&
                        i.Lot.QualityStatus == LotQualityStatus.Released,
                        i => i.Location.Facility)
                    .FirstOrDefault();
                if(inventory == null)
                {
                    Assert.Inconclusive("No valid test inventory found.");
                }
                var sourceFacility = inventory.Location.Facility;

                var parameters = TestHelper.CreateObjectGraph<CreateSalesOrderParameters>();
                parameters.UserToken = TestUser.UserName;
                parameters.CustomerKey = customer.ToCustomerKey();
                parameters.FacilitySourceKey = sourceFacility.ToFacilityKey();
                parameters.OrderItems = new List<SalesOrderItemParameters>
                    {
                        new SalesOrderItemParameters
                            {
                                ContractItemKey = contractItem.ToContractItemKey(),
                                ProductKey = contractItem.ToChileProductKey(),
                                PackagingKey = contractItem.ToPackagingProductKey(),
                                TreatmentKey = contractItem.ToInventoryTreatmentKey(),
                                Quantity = 123,
                                CustomerLotCode = "LotCode",
                                PriceBase = 1,
                                PriceFreight = 2,
                                PriceTreatment = 3,
                                PriceWarehouse = 4,
                                PriceRebate = 5
                            }
                    };

                //Act / Assert
                var createResult = Service.CreateSalesOrder(parameters);
                createResult.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.SyncTblOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders
                        .Include("tblOrderDetails")
                        .FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.AreEqual(1, tblOrder.tblOrderDetails.Count());
                }
            }

            [Test]
            public void Modifies_tblOrderDetail_and_tblStagedFG_records_as_expected()
            {
                //Arrange
                var customer = RVCUnitOfWork.CustomerRepository.Filter(c => true, c => c.Broker).FirstOrDefault();
                var contractItem = RVCUnitOfWork.ContractItemRepository.All().FirstOrDefault(i => i.Contract.CustomerId == customer.Id);
                var validInventory = RVCUnitOfWork.InventoryRepository.Filter(i =>
                                                                              i.LotTypeId == (decimal) LotTypeEnum.FinishedGood &&
                                                                              i.Lot.ProductionStatus == LotProductionStatus.Produced &&
                                                                              i.Lot.QualityStatus == LotQualityStatus.Released,
                                                                              i => i.Location)
                                                  .ToList();
                if(!validInventory.Any())
                {
                    Assert.Inconclusive("No valid test inventory found.");
                }

                var inventory = validInventory
                    .GroupBy(i => i.Location.FacilityId)
                    .First(g => g.Count() >= 2)
                    .OrderBy(i => i.LotDateCreated)
                    .Take(2)
                    .ToList();

                var parameters = TestHelper.CreateObjectGraph<CreateSalesOrderParameters>();
                parameters.UserToken = TestUser.UserName;
                parameters.CustomerKey = customer.ToCustomerKey();
                parameters.BrokerKey = customer.Broker.ToCompanyKey();
                parameters.FacilitySourceKey = inventory[0].Location.ToFacilityKey();
                parameters.OrderItems = new List<SalesOrderItemParameters>
                    {
                        new SalesOrderItemParameters
                            {
                                ContractItemKey = contractItem.ToContractItemKey(),
                                ProductKey = contractItem.ToChileProductKey(),
                                PackagingKey = contractItem.ToPackagingProductKey(),
                                TreatmentKey = contractItem.ToInventoryTreatmentKey(),
                                Quantity = 123,
                                CustomerLotCode = "OrderItem1",
                                PriceBase = 1,
                                PriceFreight = 2,
                                PriceTreatment = 3,
                                PriceWarehouse = 4,
                                PriceRebate = 5
                            },
                        new SalesOrderItemParameters
                            {
                                ContractItemKey = contractItem.ToContractItemKey(),
                                ProductKey = contractItem.ToChileProductKey(),
                                PackagingKey = contractItem.ToPackagingProductKey(),
                                TreatmentKey = contractItem.ToInventoryTreatmentKey(),
                                Quantity = 123,
                                CustomerLotCode = "OrderItem2",
                                PriceBase = 1,
                                PriceFreight = 2,
                                PriceTreatment = 3,
                                PriceWarehouse = 4,
                                PriceRebate = 5
                            },
                    };

                //Act / Assert
                //Create
                var createResult = Service.CreateSalesOrder(parameters);
                createResult.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                //Pick
                var orderKey = new SalesOrderKey().Parse(createResult.ResultingObject).ToSalesOrderKey();
                var order = RVCUnitOfWork.SalesOrderRepository.FindByKey(orderKey, o => o.SalesOrderItems);
                var lotsToAllow = new List<LotKey>();
                var itemsToPick = order.SalesOrderItems.Select(i =>
                    {
                        var result = Service.GetPickableInventoryForContext(new FilterInventoryForShipmentOrderParameters
                            {
                                OrderKey = i.ToSalesOrderKey(),
                                OrderItemKey = i.ToSalesOrderItemKey()
                            }).ResultingObject;
                        var pickable = result.Items.Where(n => n.Quantity > 1).Take(1).ToList();
                        lotsToAllow.AddRange(pickable.Cast<PickableInventoryItemReturn>().Select(p => p.LotKeyReturn.ToLotKey()));
                        return new SetPickedInventoryItemParameters
                            {
                                InventoryKey = pickable.Select(r => r.InventoryKey).FirstOrDefault(),
                                Quantity = 1,
                                OrderItemKey = i.ToSalesOrderItemKey()
                            };
                    }
                    ).ToList();
                if(itemsToPick.Any(i => i.InventoryKey == null))
                {
                    Assert.Inconclusive("Could not find valid Inventory to pick for CustomerOrderItem.");
                }

                lotsToAllow.Distinct().ForEach((l, n) =>
                    {
                        RVCUnitOfWork.LotSalesOrderAllowanceRepository.Add(new LotSalesOrderAllowance
                        {
                            SalesOrderDateCreated = orderKey.SalesOrderKey_DateCreated,
                            SalesOrderSequence = orderKey.SalesOrderKey_Sequence,
                            LotDateCreated = l.LotKey_DateCreated,
                            LotDateSequence = l.LotKey_DateSequence,
                            LotTypeId = l.LotKey_LotTypeId
                        });
                    });
                RVCUnitOfWork.Commit();

                var pickResult = Service.SetPickedInventory(orderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = itemsToPick
                    });
                pickResult.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.SyncTblOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders
                        .Include("tblOrderDetails.tblStagedFGs")
                        .FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.AreEqual(2, tblOrder.tblOrderDetails.Count);
                    foreach(var detail in tblOrder.tblOrderDetails)
                    {
                        Assert.AreEqual(1, detail.tblStagedFGs.Count);
                    }
                }

                //Unpick
                pickResult = Service.SetPickedInventory(orderKey, new SetPickedInventoryParameters
                    {
                        UserToken = TestUser.UserName,
                        PickedInventoryItems = new List<IPickedInventoryItemParameters> { itemsToPick[0] }
                    });
                pickResult.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders
                        .Include("tblOrderDetails.tblStagedFGs")
                        .FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.AreEqual(2, tblOrder.tblOrderDetails.Count);
                    Assert.AreEqual(1, tblOrder.tblOrderDetails.Single(d => d.CustLot == "OrderItem1").tblStagedFGs.Count);
                    Assert.AreEqual(0, tblOrder.tblOrderDetails.Single(d => d.CustLot == "OrderItem2").tblStagedFGs.Count);
                }
            }

            [Test]
            public void Creates_MiscInvoice_as_expected()
            {
                //Arrange
                var packaging = TestHelper.Context.PackagingProducts.FirstOrDefault();
                var product = TestHelper.Context.Products.FirstOrDefault(p => p.ProductType == ProductTypeEnum.NonInventory);
                var treatment = TestHelper.Context.Set<InventoryTreatment>().FirstOrDefault();
                var facility = TestHelper.Context.Set<Facility>().FirstOrDefault();

                //Act
                var result = Service.CreateSalesOrder(new CreateSalesOrderParameters
                    {
                        UserToken = TestUser.UserName,
                        IsMiscellaneous = true,
                        FacilitySourceKey = facility.ToFacilityKey(),

                        HeaderParameters = new SetOrderHeaderParameters(),

                        OrderItems = new List<SalesOrderItemParameters>
                            {
                                new SalesOrderItemParameters
                                    {
                                        ProductKey = product.ToProductKey(),
                                        PackagingKey = packaging.ToPackagingProductKey(),
                                        TreatmentKey = treatment.ToInventoryTreatmentKey(),
                                        Quantity = 1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.SyncTblOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders.AsQueryable()
                        .Include(o => o.tblOrderDetails)
                        .FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.AreEqual((int)TransType.MiscInvoice, tblOrder.TTypeID);
                    Assert.IsNull(tblOrder.Company_IA);

                    var tblOrderDetail = tblOrder.tblOrderDetails.Single();
                    Assert.AreEqual(int.Parse(product.ProductCode), tblOrderDetail.ProdID);
                    Assert.IsNull(tblOrderDetail.KDetailID);
                }
            }
        }

        public class UpDateSalesOrder : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Updates_MiscInvoice_as_expected()
            {
                //Arrange
                var order = TestHelper.Context.Set<SalesOrder>()
                    .Include
                        (
                            o => o.Customer.Company,
                            o => o.InventoryShipmentOrder.SourceFacility
                        )
                    .FirstOrDefault(o => o.InventoryShipmentOrder.OrderType == InventoryShipmentOrderTypeEnum.MiscellaneousOrder && o.Customer == null);
                var packaging = TestHelper.Context.PackagingProducts.FirstOrDefault();
                var product = TestHelper.Context.Products.FirstOrDefault(p => p.ProductType == ProductTypeEnum.NonInventory);
                var treatment = TestHelper.Context.Set<InventoryTreatment>().FirstOrDefault();

                //Act
                var result = Service.UpdateSalesOrder(new UpdateSalesOrderParameters
                    {
                        SalesOrderKey = order.ToSalesOrderKey(),
                        UserToken = TestUser.UserName,
                        FacilitySourceKey = order.InventoryShipmentOrder.SourceFacility.ToFacilityKey(),
                        
                        HeaderParameters = new SetOrderHeaderParameters(),
                    
                        OrderItems = new List<SalesOrderItemParameters>
                                {
                                    new SalesOrderItemParameters
                                        {
                                            ProductKey = product.ToProductKey(),
                                            PackagingKey = packaging.ToPackagingProductKey(),
                                            TreatmentKey = treatment.ToInventoryTreatmentKey(),
                                            Quantity = 1
                                        }
                                }
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.SyncTblOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders.AsQueryable()
                        .Include(o => o.tblOrderDetails)
                        .FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.AreEqual((int)TransType.MiscInvoice, tblOrder.TTypeID);
                    Assert.IsNull(tblOrder.Company_IA);

                    var tblOrderDetail = tblOrder.tblOrderDetails.Single();
                    Assert.AreEqual(int.Parse(product.ProductCode), tblOrderDetail.ProdID);
                    Assert.IsNull(tblOrderDetail.KDetailID);
                }
            } 
        }

        [TestFixture]
        public class DeleteSalesOrder : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Removes_tblMove_record_as_expected()
            {
                //Arrange
                var orders = RVCUnitOfWork.SalesOrderRepository.Filter(o =>
                        o.InventoryShipmentOrder.ShipmentInformation.Status != ShipmentStatus.Shipped &&
                        o.SalesOrderPickedItems.All(i => i.PickedInventoryItem.CurrentLocationId == i.PickedInventoryItem.FromLocationId))
                    .ToList();
                var order = orders.FirstOrDefault(o => o.SalesOrderItems.Any() && o.SalesOrderPickedItems.Any());
                if(order == null)
                {
                    order = orders.FirstOrDefault();
                    if(order == null)
                    {
                        Assert.Inconclusive("Could not find CustomerOrder suitable for testing.");
                    }
                }

                //Act
                var result = Service.DeleteSalesOrder(order.ToSalesOrderKey());

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.RemovedTblOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders.FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.IsNull(tblOrder);
                }
            }
        }

        [TestFixture]
        public class PostInvoice : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Updates_tblOrder_record_as_expected()
            {
                //Arrange
                var order = RVCUnitOfWork.SalesOrderRepository.Filter(o => o.OrderStatus != SalesOrderStatus.Invoiced && o.InventoryShipmentOrder.ShipmentInformation.Status == ShipmentStatus.Shipped).FirstOrDefault();
                if(order == null)
                {
                    Assert.Inconclusive("No suitable order for testing.");
                }

                //Act
                var result = Service.PostInvoice(order.ToSalesOrderKey());

                //Arrange
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var orderNumString = GetKeyFromConsoleString(ConsoleOutput.InvoicedOrder);
                var orderNum = int.Parse(orderNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblOrder = oldContext.tblOrders.FirstOrDefault(o => o.OrderNum == orderNum);
                    Assert.AreEqual(tblOrderStatus.Invoiced, (tblOrderStatus)tblOrder.Status);
                }
            }
        }

        [TestFixture]
        public class SetSalesQuote : SynchronizeOldContextIntegrationTestsBase<SalesService>
        {
            [Test]
            public void Creates_new_tblQuote_record()
            {
                //Act
                var result = Service.SetSalesQuote(new SalesQuoteParameters
                    {
                        UserToken = TestUser.UserName,
                        QuoteDate = new DateTime(2016, 1, 1),
                        Items = new List<ISalesQuoteItemParameters>
                            {
                                new SalesQuoteItemParameters
                                    {
                                        CustomerProductCode = "code",
                                        ProductKey = RVCUnitOfWork.ProductRepository.SourceQuery.First().ToProductKey(),
                                        PackagingKey = RVCUnitOfWork.PackagingProductRepository.SourceQuery.First().ToPackagingProductKey(),
                                        TreatmentKey = RVCUnitOfWork.InventoryTreatmentRepository.SourceQuery.First().ToInventoryTreatmentKey(),
                                        Quantity = 1
                                    }
                            }
                    });

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var quoteNumString = GetKeyFromConsoleString(ConsoleOutput.SyncTblQuote);
                var quoteNum = int.Parse(quoteNumString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    Assert.IsNotNull(oldContext.tblQuotes.FirstOrDefault(q => q.QuoteNum == quoteNum));
                }
            }

            [Test]
            public void Updates_existing_tblQuote_record()
            {
                //Arrange
                var salesQuote = RVCUnitOfWork.SalesQuoteRepository.SourceQuery.FirstOrDefault(q => q.QuoteNum != null);
                if(salesQuote == null)
                {
                    Assert.Inconclusive("Could not find valid SalesQuote to test.");
                }

                //Act
                var quoteDate = salesQuote.QuoteDate.AddDays(1);
                var result = Service.SetSalesQuote(new SalesQuoteParameters
                    {
                        UserToken = TestUser.UserName,
                        SalesQuoteNumber = salesQuote.QuoteNum.Value,
                        QuoteDate = quoteDate,
                        Items = new List<ISalesQuoteItemParameters>
                            {
                                new SalesQuoteItemParameters
                                    {
                                        CustomerProductCode = "code",
                                        ProductKey = RVCUnitOfWork.ProductRepository.SourceQuery.First().ToProductKey(),
                                        PackagingKey = RVCUnitOfWork.PackagingProductRepository.SourceQuery.First().ToPackagingProductKey(),
                                        TreatmentKey = RVCUnitOfWork.InventoryTreatmentRepository.SourceQuery.First().ToInventoryTreatmentKey(),
                                        Quantity = 1
                                    }
                            }
                    }, true);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                using(var oldContext = new RioAccessSQLEntities())
                {
                    var tblQuote = oldContext.tblQuotes.FirstOrDefault(q => q.QuoteNum == salesQuote.QuoteNum);
                    Assert.AreEqual(quoteDate, tblQuote.Date);
                }
            }
        }
    }
}