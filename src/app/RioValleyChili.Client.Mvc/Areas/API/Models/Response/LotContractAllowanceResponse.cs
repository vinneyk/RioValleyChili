using System;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response
{
    public class LotContractAllowanceResponse
    {
        public string ContractKey { get; set; }
        public DateTime? TermBegin { get; set; }
        public DateTime? TermEnd { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
    }
}