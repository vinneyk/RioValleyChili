using System.ComponentModel.DataAnnotations;

namespace RioValleyChili.Client.Mvc.Models.Production
{
    public class PackagingMaterialSummaryViewModel
    {
        public string PackagingProductName { get; set; }

        public string PackagingProductKey { get; set; }

        public double QuantityPicked { get; set; }

        [DisplayFormat(NullDisplayText = "-")]
        public double? QuantityOrdered { get; set; }
    }
}