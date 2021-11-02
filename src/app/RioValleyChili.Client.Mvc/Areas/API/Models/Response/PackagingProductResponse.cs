namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class PackagingProductResponse : ProductResponse
    {
        public double Weight { get; set; }
        public double PackagingWeight { get; set; }
        public double PalletWeight { get; set; }
    }
}