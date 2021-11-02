using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using Solutionhead.Services;
using TechTalk.SpecFlow;

namespace RioValleyChili.Services.Tests
{
    [Binding]
    public class GetInventoryItemsToPickABatchSteps : ServiceAcceptanceTestsBase<ProductionService>
    {
        public GetInventoryItemsToPickABatchSteps(SharedContext sharedContext) : base(sharedContext) { }

        [When(@"I get inventory to pick for a batch")]
        public void WhenIGetInventoryToPickForABatch()
        {
            _getInventoryItemsToPickBatchResult = Service.GetInventoryItemsToPickBatch();
            if(_getInventoryItemsToPickBatchResult.ResultingObject != null)
            {
                _results = _getInventoryItemsToPickBatchResult.ResultingObject.Items.ToList();
            }
        }

        [Then(@"the inventory items will not be included in the results")]
        public void ThenTheInventoryItemsShouldNotBeIncludedInTheResults()
        {
            _getInventoryItemsToPickBatchResult.AssertSuccess();
            Assert.True(SharedContext.CreatedInventory.Select(i => i.ToInventoryKey().KeyValue).All(k => _results.All(r => r.InventoryKey != k)));
        }

        private IResult<IPickableInventoryReturn> _getInventoryItemsToPickBatchResult;

        private List<IPickableInventorySummaryReturn> _results;
    }
}
