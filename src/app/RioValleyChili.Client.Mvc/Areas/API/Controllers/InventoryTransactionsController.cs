using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.InventoryTransactions;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class InventoryTransactionsController : ApiController
    {
        public const string GetInventoryTransactionsByLotRouteName = "GetInventoryTransactionsByLot";
        public const string GetLotInputsByLotKeyRouteName = "GetLotInputsByLotKey";

        private readonly IInventoryService _inventoryService;

        public InventoryTransactionsController(IInventoryService inventoryService)
        {
            if(inventoryService == null) { throw new ArgumentNullException("inventoryService"); }
            _inventoryService = inventoryService;
        }

        [Route("~/api/lots/{lotKey}/inventory/transactions", Name = GetInventoryTransactionsByLotRouteName)]
        public IEnumerable<LotInventoryTransactionResponse> GetInventoryTransactions(string lotKey)
        {
            var result = _inventoryService.GetInventoryTransactions(lotKey);
            result.EnsureSuccessWithHttpResponseException();

            var mapped =  result.ResultingObject
                .OrderBy(t => t.TimeStamp)
                .ThenBy(t => t.Quantity)
                .Project().To<LotInventoryTransactionResponse>();
            
            return mapped;
        }

        [Route("~/api/lots/{lotKey}/input", Name = GetLotInputsByLotKeyRouteName)]
        public LotInputResponse GetLotInputs(string lotKey)
        {
            var result = _inventoryService.GetInventoryTransactionsByDestinationLot(lotKey);
            result.EnsureSuccessWithHttpResponseException();

            var mapped = result.ResultingObject.Map().To<LotInputResponse>();
            mapped.InputItems = mapped.InputItems.OrderBy(m => m.TransactionDate).ThenBy(m => m.Quantity);
            return mapped;
        }
    }
}
