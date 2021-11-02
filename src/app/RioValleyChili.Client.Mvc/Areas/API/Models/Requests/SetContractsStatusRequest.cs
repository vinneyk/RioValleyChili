using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class SetContractsStatusRequest
    {
        public ContractStatus ContractStatus { get; set; }
        public IEnumerable<string> ContractKeys { get; set; }
    }
}