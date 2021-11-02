using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ChileProductIngredientExtensions
    {
        internal static void AssertEqual(this ChileProductIngredient ingredient, ISetChileProductIngredientParameters parameters)
        {
            if(ingredient == null) { throw new ArgumentNullException("ingredient"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            
            Assert.AreEqual(ingredient.ToAdditiveTypeKey().KeyValue, parameters.AdditiveTypeKey);
            Assert.AreEqual(ingredient.Percentage, parameters.Percentage);
        }
    }
}