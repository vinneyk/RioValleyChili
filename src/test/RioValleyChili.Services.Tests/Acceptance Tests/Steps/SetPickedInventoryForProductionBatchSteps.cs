using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.Services;
using TechTalk.SpecFlow;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;

namespace RioValleyChili.Services.Tests.Acceptance_Tests
{
    [Binding]
    public class SetPickedInventoryForProductionBatchSteps : ServiceAcceptanceTestsBase<ProductionService>
    {
        public SetPickedInventoryForProductionBatchSteps(SharedContext sharedContext) : base(sharedContext) { }

        [Given(@"a production batch that has been produced")]
        public void GivenAProductionBatchThatHasBeenProduced()
        {
            _productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.ProductionHasBeenCompleted = true);
        }
        
        [Given(@"a production batch that has not been produced")]
        public void GivenAProductionBatchThatHasNotBeenProduced()
        {
            _productionBatch = TestHelper.CreateObjectGraphAndInsertIntoDatabase<ProductionBatch>(b => b.SetToNotCompleted().EmptyItems());
        }
        
        [When(@"I set picked inventory for the production batch")]
        public void WhenISetPickedInventoryForTheProductionBatch()
        {
            var result = Service.SetPickedInventoryForProductionBatch(new LotKey(_productionBatch), new Utilities.Models.SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = new List<IPickedInventoryItemParameters>()
                });

            _expectedResults.Add(new ExpectedInvalidResults
                {
                    ActualResult = result,
                    ExpectedMessage = UserMessages.ProductionBatchAlreadyComplete
                });
        }
        
        [When(@"I attempt to pick any of the above inventory for the batch")]
        public void WhenIAttemptToPickAnyOfTheAboveInventoryForTheBatch()
        {
            SharedContext.CreatedInventory.ForEach(CreateExpectedResult);
            _expectedResults.ForEach(r => r.ActualResult = Service.SetPickedInventoryForProductionBatch(new LotKey(_productionBatch), r.Parameters));
        }

        [When(@"I pick the above inventory for the batch")]
        public void WhenIPickTheAboveInventoryForTheBatch()
        {
            _result = Service.SetPickedInventoryForProductionBatch(new LotKey(_productionBatch), new Utilities.Models.SetPickedInventoryParameters
                {
                    UserToken = TestUser.UserName,
                    PickedInventoryItems = SharedContext.CreatedInventory.Select(i => new SetPickedInventoryItemParameters
                        {
                            InventoryKey = new InventoryKey(i).KeyValue,
                            Quantity = i.Quantity
                        })
                });
        }
        
        [Then(@"the service method returns an invalid result")]
        public void ThenTheServiceMethodReturnsAnInvalidResult()
        {
            _expectedResults.ForEach(r => r.Assert());
        }

        [Then(@"the batch's resulting lot will have its attribute values set to")]
        public void ThenTheBatchSResultingLotWillHaveItsAttributeValuesSetTo(Dictionary<string, double> expectedAttributes)
        {
            _result.AssertSuccess();
            var attributes = SharedContext.RVCUnitOfWork.LotRepository.FindByKey(_productionBatch.ToLotKey(), b => b.Attributes).Attributes;
            foreach(var expected in expectedAttributes)
            {
                var result = attributes.Single(r => r.AttributeShortName == expected.Key);
                Assert.Less(Math.Abs(expected.Value - result.AttributeValue), 0.01);
            }
        }

        protected void CreateExpectedResult(Inventory inventory)
        {
            string expectedMessage = null;
            if(inventory.Lot.QualityStatus != LotQualityStatus.Released)
            {
                expectedMessage = string.Format(UserMessages.CannotPickLotQualityState, new LotKey(inventory).KeyValue, inventory.Lot.QualityStatus);
            }
            else if(inventory.Lot.Hold != null)
            {
                expectedMessage = string.Format(UserMessages.CannotPickLotOnHold, new LotKey(inventory).KeyValue, inventory.Lot.Hold.ToString());
            }
            else if(inventory.Location.Locked)
            {
                expectedMessage = string.Format(UserMessages.InventoryLocationLocked, new InventoryKey(inventory).KeyValue);
            }

            _expectedResults.Add(new ExpectedInvalidResults
                {
                    Parameters = new Utilities.Models.SetPickedInventoryParameters
                        {
                            UserToken = TestUser.UserName,
                            PickedInventoryItems = new[]
                                {
                                    new SetPickedInventoryItemParameters
                                        {
                                            InventoryKey = new InventoryKey(inventory),
                                            Quantity = 1
                                        }
                                }
                        },
                    ExpectedMessage = expectedMessage
                });
        }

        private ProductionBatch _productionBatch;
        private IResult _result; 
        private readonly List<ExpectedInvalidResults> _expectedResults = new List<ExpectedInvalidResults>();
        
        private class ExpectedInvalidResults
        {
            public Utilities.Models.SetPickedInventoryParameters Parameters;
            public IResult ActualResult;
            public string ExpectedMessage;

            public void Assert()
            {
                ActualResult.AssertInvalid(ExpectedMessage);
            }
        }
    }
}
