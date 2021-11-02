using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using Solutionhead.Services;
using TechTalk.SpecFlow;

namespace RioValleyChili.Services.Tests
{
    [Binding]
    public class CalculateAttributeWeightedAveragesSteps : ServiceAcceptanceTestsBase<InventoryService>
    {
        public CalculateAttributeWeightedAveragesSteps(SharedContext sharedContext) : base(sharedContext) { }
        
        [When(@"I calculate attribute weighted averages")]
        public void WhenICalculateAttributeWeightedAverages()
        {
            _result = Service.CalculateAttributeWeightedAverages(SharedContext.CreatedInventory.ToDictionary(i => new InventoryKey(i).KeyValue, i => i.Quantity));
        }
        
        [Then(@"the attribute weighted average results are as expected")]
        public void ThenTheAttributeWeightedAverageResultsAreAsExpected(Dictionary<string, double> expectedResults)
        {
            _result.AssertSuccess();
            var results = _result.ResultingObject.ToList();
            foreach(var expectedResult in expectedResults)
            {
                var result = results.Single(r => r.Key == expectedResult.Key);
                Assert.Less(Math.Abs(result.Value - expectedResult.Value), 0.01);
            }
        }

        private IResult<IEnumerable<KeyValuePair<string, double>>> _result;
    }
}
