using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class AttributeNamesController : ApiController
    {
        private readonly ILotService _lotService;

        public AttributeNamesController(ILotService lotService)
        {
            if(lotService == null) throw new ArgumentNullException("lotService");
            _lotService = lotService;
        }

        [OutputCache(Duration = 600)]
        public IDictionary<int, IEnumerable<KeyValuePair<string, string>>> Get()
        {
            var result = _lotService.GetLotSummaries();
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject.AttributeNamesByProductType.ToDictionary(k => (int)k.Key, v => v.Value);
        }
    }
}
