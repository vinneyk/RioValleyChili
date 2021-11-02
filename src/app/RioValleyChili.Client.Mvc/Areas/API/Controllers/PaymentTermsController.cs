using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class PaymentTermsController : ApiController
    {
        private readonly ISalesService _salesService;

        public PaymentTermsController(ISalesService salesService)
        {
            if(salesService == null) throw new ArgumentNullException("salesService");
            _salesService = salesService;
        }

        // GET api/paymentterms
        public IEnumerable<string> Get()
        {
            return _salesService.GetDistinctContractPaymentTerms()
                .ResultingObject.OrderBy(t => t);
        }
    }
}
