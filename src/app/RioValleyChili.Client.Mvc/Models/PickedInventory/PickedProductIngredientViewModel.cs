using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Client.Mvc.Models.PickedInventory
{
    public class PickedProductIngredientViewModel : IProductIngredientReturn
    {
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public double Percent { get; set; }

        public string AdditiveTypeKey { get; set; }

        public string AdditiveTypeName { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0} lbs")]
        public double PoundsNeeded { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0} lbs")]
        public double PoundsPicked { get; set; }

        public ProductTypeEnum ProductType { get; set; }
        
        public string IngredientDisplay
        {
            get { return string.Format("{0:P0} {1}", Percent, AdditiveTypeName); }
        }

        public string PercentDisplay
        {
            get { return string.Format("{0:P0}", Percent); }
        }

        public string PoundsPickedDisplay
        {
            get { return string.Format("{0:N0}", PoundsPicked); }
        }

        public string PoundsNeededDisplay
        {
            get { return string.Format("{0:N0}", PoundsNeeded); }
        }
    }
}