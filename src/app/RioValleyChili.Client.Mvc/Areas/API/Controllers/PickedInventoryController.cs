using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Utilities;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.Route("api/{pickedInventoryContext}/{contextKey}/pick")]
    [Obsolete("Use PickableInventoryController instead.")]
    public class PickedInventoryController : ApiController
    {
        #region fields and constructors

        private readonly IUserIdentityProvider _identityProvider;

        public PickedInventoryController(IUserIdentityProvider identityProvider)
        {
            if(identityProvider == null) throw new ArgumentNullException("identityProvider");
            _identityProvider = identityProvider;
        }

        #endregion

        #region API Methods
        
        // POST api/{pickedInventoryContext}/{contextKey}/pickedInventory
        // POST api/movements/2013-01-01/pickedInventory
        public async Task Post(InventoryOrderEnum pickedInventoryContext, string contextKey, [FromBody] PickedInventoryDto value)
        {
            var service = InventoryPickingServiceFactory.ResolveComponent(pickedInventoryContext);
            var setPickedInventory = value.Map().To<SetPickedInventoryParameters>();
            _identityProvider.SetUserIdentity(setPickedInventory);

            var result = await Task.Run(() => service.SetPickedInventory(contextKey, setPickedInventory));
            result.EnsureSuccessWithHttpResponseException(HttpVerbs.Put);
        }

        // DELETE api/{pickedInventoryContext}/{contextKey}/pickedInventory
        // DELETE api/movements/2013-01-01/pickedInventory
        public async Task Delete(InventoryOrderEnum pickedInventoryContext, string contextKey)
        {
            var pickedInventory = new PickedInventoryDto
                                      {
                                          PickedInventoryItems = new PickedInventoryItemDto[0]
                                      };

            await Post(pickedInventoryContext, contextKey, pickedInventory);
        }

        #endregion
    }
}