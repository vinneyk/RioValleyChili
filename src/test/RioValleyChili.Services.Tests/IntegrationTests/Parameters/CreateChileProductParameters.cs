using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateChileProductParameters : ICreateChileProductParameters
    {
        public string UserToken { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ChileTypeKey { get; set; }
        public ChileStateEnum ChileState { get; set; }
        public double? Mesh { get; set; }
        public string IngredientsDescription { get; set; }

        public IEnumerable<ISetAttributeRangeParameters> AttributeRanges { get; set; }
        public IEnumerable<ISetChileProductIngredientParameters> Ingredients { get; set; }
    }
}