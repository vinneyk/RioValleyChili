using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class ChileVaritiesController : ApiController
    {
        #region fields and constructors

        private readonly IMaterialsReceivedService _materialsReceivedService;

        public ChileVaritiesController(IMaterialsReceivedService materialsReceivedService)
        {
            if (materialsReceivedService == null) { throw new ArgumentNullException("materialsReceivedService"); }
            _materialsReceivedService = materialsReceivedService;
        }

        #endregion

        // GET api/chilevarities
        public IEnumerable<string> Get()
        {
            var results = _materialsReceivedService.GetChileVarieties();
            results.EnsureSuccessWithHttpResponseException();
            return results.ResultingObject.OrderBy(c => c);
        }

        // GET api/chilevarities/5
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/chilevarities
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT api/chilevarities/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/chilevarities/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
