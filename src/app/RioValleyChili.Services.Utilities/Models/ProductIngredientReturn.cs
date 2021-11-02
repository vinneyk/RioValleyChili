using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    public class ProductIngredientReturn : IProductIngredientReturn
    {
        internal AdditiveTypeKeyReturn AdditiveTypeKeyReturn { get; set; }

        public string AdditiveTypeName { get; set; }

        public double Percent { get; set; }

        public string AdditiveTypeKey
        {
            get { return AdditiveTypeKeyReturn.AdditiveTypeKey; }
        }
    }
}