using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.InventoryTransactions;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.InventoryService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class InventoryController : ApiController
    {
        #region fields and constructors
        
        private readonly IInventoryService _inventoryService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public InventoryController(IInventoryService inventoryService, IUserIdentityProvider userIdentityProvider)
        {
            if (inventoryService == null) { throw new ArgumentNullException("inventoryService"); }
            _inventoryService = inventoryService;

            if (userIdentityProvider == null) {  throw new ArgumentNullException("userIdentityProvider"); }
            _userIdentityProvider = userIdentityProvider;
        }

        #endregion
        
        #region GET /api/inventory

        public IEnumerable<InventoryItem> Get(ProductTypeEnum? productType = null, ProductTypeEnum? inventoryType = null, LotTypeEnum? lotType = null, string productSubType = null, string productKey = null, string packagingProductKey = null, string warehouseKey = null, string warehouseLocationKey = null, string treatmentKey = null, string ingredientType = null, string locationGroupName = null,
            int pageSize = 50, int skipCount = 0)
        {
            var getInventoryResult = _inventoryService.GetInventory(new FilterInventoryParameters
                {
                    ProductType = inventoryType ?? productType ?? ProductTypeEnum.Chile,
                    LotType = lotType,
                    ProductKey = productKey,
                    FacilityKey = warehouseKey,
                    LocationKey = warehouseLocationKey,
                    PackagingKey = packagingProductKey,
                    TreatmentKey = treatmentKey,
                    LocationGroupName = locationGroupName
                });
            getInventoryResult.EnsureSuccessWithHttpResponseException();

            productSubType = productSubType ?? ingredientType;
            var filterBySubType = !string.IsNullOrWhiteSpace(productSubType);
            var inventoryResults = getInventoryResult.ResultingObject
                .Inventory.Where(i => filterBySubType == false || i.LotProduct.ProductSubType == productSubType)
                .OrderBy(i => i.Location.Description);

            return inventoryResults.PageResults(pageSize, skipCount).Project().To<InventoryItem>();
        }

        #endregion
        
        #region GET /api/inventorytotals

        [System.Web.Http.Route("~/api/inventorytotals")]
        public async Task<GetInventoryWithTotalsResponse> GetInventoryWithTotals(ProductTypeEnum? productType = null, ProductTypeEnum? inventoryType = null, LotTypeEnum? lotType = null, string productSubType = null, string productKey = null, string packagingProductKey = null, string warehouseKey = null, string warehouseLocationKey = null, string treatmentKey = null, string ingredientType = null, string locationGroupName = null,
            int pageSize = 50, int skipCount = 0)
        {
            var getInventoryResult = _inventoryService.GetInventory(new FilterInventoryParameters
            {
                ProductType = inventoryType ?? productType ?? ProductTypeEnum.Chile,
                LotType = lotType,
                ProductKey = productKey,
                FacilityKey = warehouseKey,
                LocationKey = warehouseLocationKey,
                PackagingKey = packagingProductKey,
                TreatmentKey = treatmentKey,
                LocationGroupName = locationGroupName   
            });
            getInventoryResult.EnsureSuccessWithHttpResponseException();

            productSubType = productSubType ?? ingredientType;
            var filterBySubType = !string.IsNullOrWhiteSpace(productSubType);
            var inventoryQuery = getInventoryResult.ResultingObject
                .Inventory.Where(i => filterBySubType == false || i.LotProduct.ProductSubType == productSubType)
                .OrderBy(i => i.Location.Description);

            return new GetInventoryWithTotalsResponse
            {
                TotalPounds = (await inventoryQuery.SumAsync(i => i.Quantity * i.PackagingProduct.Weight)),
                Items = (await inventoryQuery.PageResultsAsync(pageSize, skipCount)).Project().To<InventoryItem>()
            };
        }

        #endregion

        #region GET /api/inventory/030913407

        public LotInventory Get(string lotKey)
        {
            var getInventoryResult = _inventoryService.GetInventory(new FilterInventoryParameters
            {
                LotKey = lotKey,
            });
            getInventoryResult.EnsureSuccessWithHttpResponseException(HttpVerbs.Get);

            var result = new LotInventory
                             {
                                 InventoryItems = getInventoryResult.ResultingObject.Inventory.Project().To<InventoryItem>(),
                             };
            return result;
        }

        #endregion

        #region POST /api/inventory

        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.InventoryClaimTypes.Inventory),
        ValidateAntiForgeryTokenFromCookie]
        public HttpResponseMessage Post(ReceiveInventoryDto values)
        {
            var parameters = values.Map().To<Services.Models.Parameters.ReceiveInventoryParameters>();
            _userIdentityProvider.SetUserIdentity(parameters);
            var result = _inventoryService.ReceiveInventory(parameters);

            return result.ToHttpResponseMessage(HttpVerbs.Post);
        }

        #endregion

        [System.Web.Http.Route("~/api/inventory-received")]
        public ReceivedInventoryDetails Get(LotTypeEnum? lotType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return Get(null, lotType, startDate, endDate);
        }

        [System.Web.Http.Route("~/api/inventory-received/{lotKey}")]
        public ReceivedInventoryDetails Get(string lotKey, LotTypeEnum? lotType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = _inventoryService.GetInventoryReceived(new GetInventoryReceivedParameters
                {
                    LotKey = lotKey,
                    LotType = lotType,
                    DateReceivedStart = startDate,
                    DateReceivedEnd = endDate
                });
            result.EnsureSuccessWithHttpResponseException();

            var data = result.ResultingObject.ToList();
            var firstResult = data.First();

            return new ReceivedInventoryDetails
                {
                    DateEntered = firstResult.TimeStamp,
                    EnteredByUser = firstResult.EmployeeName,

                    LotKey = firstResult.SourceLotKey,
                    PackagingReceived = firstResult.SourceLotPackagingReceived.ProductName,
                    ProductName = firstResult.Product.ProductName,

                    PurchaseOrderNumber = firstResult.SourceLotPurchaseOrderNumber,
                    ShipperNumber = firstResult.SourceLotShipperNumber,
                    VendorName = firstResult.SourceLotVendorName,

                    InventoryItems = data.Select(t => new ReceivedInventoryItem
                        {
                            Weight = t.Weight,
                            FacilityName = t.Location.FacilityName,
                            Location = t.FacilityLocationDescription,
                            InventoryUnits = t.Packaging.ProductName,
                            Treatment = t.Treatment.TreatmentNameShort,
                            Quantity = t.Quantity
                        })
            };
        }
    }
}
