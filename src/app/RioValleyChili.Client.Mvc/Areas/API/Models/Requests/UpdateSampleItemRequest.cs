﻿namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateSampleItemRequest
    {
        public string SampleOrderItemKey { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string CustomerProductName { get; set; }
        public string ProductKey { get; set; }
        public string LotKey { get; set; }
    }
}