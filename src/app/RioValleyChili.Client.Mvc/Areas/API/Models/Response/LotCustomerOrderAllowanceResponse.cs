namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotCustomerOrderAllowanceResponse
    {
        public string OrderKey { get; set; }
        public int? OrderNumber { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
    }
}