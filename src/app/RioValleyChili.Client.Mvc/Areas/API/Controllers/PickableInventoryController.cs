using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Inventory;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Services.Models.Parameters;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.Route("api/inventory/pick-{pickingContext}/{contextKey}")]
    public class PickableInventoryController : ApiController
    {
        #region fields and constructors

        private readonly IUserIdentityProvider _identityProvider;

        public PickableInventoryController(IUserIdentityProvider identityProvider)
        {
            if(identityProvider == null) throw new ArgumentNullException("identityProvider");
            _identityProvider = identityProvider;
        }

        #endregion

        public IEnumerable<PickableInventoryItem> Get(InventoryOrderEnum pickingContext, string contextKey,
            ProductTypeEnum? productType = null, ProductTypeEnum? inventoryType = null, string lotKey = null, LotTypeEnum? lotType = null, LotHoldType? holdType = null, string ingredientType = null, string productKey = null, string packagingProductKey = null, string warehouseLocationKey = null, string treatmentKey = null, string productSubType = null, string locationGroupName = null,
            int pageSize = 50, int skipCount = 0)
        {
            var service = InventoryPickingServiceFactory.ResolveComponent(pickingContext);
            var filterParameters = InventoryPickingFilterParametersHelper.ParseFilterParameters(pickingContext, ActionContext);
            var inventoryOrderResult = service.GetPickableInventoryForContext(filterParameters);
            inventoryOrderResult.EnsureSuccessWithHttpResponseException();

            var filterBySubType = !string.IsNullOrWhiteSpace(productSubType);
            var result = inventoryOrderResult.ResultingObject
                .Items
                .Where(i => filterBySubType == false || i.LotProduct.ProductSubType == productSubType)
                .OrderBy(i => i.Location.Description)
                .PageResults(pageSize, skipCount)
                .Project(i =>
                    {
                        if(inventoryOrderResult.ResultingObject.Initializer != null)
                        {
                            inventoryOrderResult.ResultingObject.Initializer.Initialize(i);
                        }
                    })
                .To<PickableInventoryItem>();
            return result;
        }

        public async Task Post(InventoryOrderEnum pickingContext, string contextKey, [FromBody] IEnumerable<PickedInventoryItemDto> value)
        {
            var service = InventoryPickingServiceFactory.ResolveComponent(pickingContext);
            var pickedItemParameters = new SetPickedInventoryParameters
            {
                PickedInventoryItems = value.Project().To<SetPickedInventoryItemParameters>()
            };
            _identityProvider.SetUserIdentity(pickedItemParameters);

            var result = await Task.Run(() => service.SetPickedInventory(contextKey, pickedItemParameters));
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }

        public async Task Delete(InventoryOrderEnum pickedInventoryContext, string contextKey)
        {
            await Post(pickedInventoryContext, contextKey, new PickedInventoryItemDto[0]);
        }
    }
}