using System.Collections.Generic;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.InventoryTransactions
{
    public class LotInputResponse
    {
        public InventoryProductResponse Product { get; set; }
        public IEnumerable<LotInventoryTransactionResponse> InputItems { get; set; }
    }
}