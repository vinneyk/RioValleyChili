using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateChileProductParameters : IUpdateChileProductParameters
    {
        public string UserToken { get; set; }
        public string ProductKey { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
        public string ChileTypeKey { get; set; }
        public double? Mesh { get; set; }
        public string IngredientsDescription { get; set; }

        public IEnumerable<ISetAttributeRangeParameters> AttributeRanges { get; set; }
        public IEnumerable<ISetChileProductIngredientParameters> Ingredients { get; set; }
    }
}