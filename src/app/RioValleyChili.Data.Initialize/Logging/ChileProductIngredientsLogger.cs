using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class ChileProductIngredientsLogger : EntityLoggerBase<ChileProductIngredientsMother.CallbackParameters, ChileProductIngredientsMother.CallbackReason>
    {
        public ChileProductIngredientsLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(ChileProductIngredientsMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case ChileProductIngredientsMother.CallbackReason.NullIngredient:
                    return string.Format("{0} has null IngrID", GetProductIngredient(parameters.Ingredient));

                case ChileProductIngredientsMother.CallbackReason.NullProduct:
                    return string.Format("{0} has null ProdID", GetProductIngredient(parameters.Ingredient));

                case ChileProductIngredientsMother.CallbackReason.ChileProductNotLoaded:
                    return string.Format("ChileProduct[{0}] not loaded in new context for {1}", parameters.Ingredient.Product.ProductName, GetProductIngredient(parameters.Ingredient));

                case ChileProductIngredientsMother.CallbackReason.AdditiveTypeNotLoaded:
                    return string.Format("AdditiveType[{0}] not loaded in new context for {1}", parameters.Ingredient.Ingredient.IngrDesc, GetProductIngredient(parameters.Ingredient));
            }

            return null;
        }

        private static string GetProductIngredient(ChileProductIngredientsMother.tblProductIngrDTO ingredient)
        {
            return string.Format("ProductIngredient[EntryDate[{0}], Product[{1}], Ingredient[{2}]]", ingredient.EntryDate, ingredient.Product != null ? ingredient.Product.ProductName : "null", ingredient.Ingredient != null ? ingredient.Ingredient.IngrDesc : "null");
        }
    }
}