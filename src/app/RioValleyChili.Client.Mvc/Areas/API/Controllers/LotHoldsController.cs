using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Models.Inventory;
using RioValleyChili.Client.Mvc.Utilities.Helpers;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.QualityControlClaimTypes.QAHolds)]
    public class LotHoldsController : ApiController
    {
        private readonly ILotService _lotService;
        private readonly IUserIdentityProvider _userIdentityProvider;

        public LotHoldsController(ILotService lotService, IUserIdentityProvider userIdentityProvider)
        {
            if(lotService == null) throw new ArgumentNullException("lotService");
            _lotService = lotService;

            if(userIdentityProvider == null) throw new ArgumentNullException("userIdentityProvider");
            _userIdentityProvider = userIdentityProvider;
        }

        [ValidateAntiForgeryTokenFromCookie,
        ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.QualityControlClaimTypes.QAHolds)]
        public HttpResponseMessage Put(string lotKey, LotHoldDto values)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = ModelState.GetErrorSummary()
                };
            }

            var dto = new SetLotAttributeDto(lotKey, values);
            _userIdentityProvider.SetUserIdentity(dto);
            return _lotService.SetLotHoldStatus(dto).ToMapped().Response<LotStatInfoResponse>(HttpVerbs.Put);
        }

        private class SetLotAttributeDto : ISetLotHoldStatusParameters
        {
            public SetLotAttributeDto(string lotKey, ILotHold hold)
            {
                LotKey = lotKey;
                Hold = hold;
            }

            public ILotHold Hold { get; private set; }
            public string LotKey { get; private set; }
        
            string IUserIdentifiable.UserToken { get; set; }
        }
    }
}