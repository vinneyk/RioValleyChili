using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Areas.API.Models.Requests;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Returns.SalesService;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [RoutePrefix("api/contracts"),
    ClaimsAuthorize(ClaimActions.View, ClaimTypes.SalesClaimTypes.CustomerContracts)]
    public class CustomerContractsController : ApiController
    {
        private readonly ISalesService _salesService;
        private readonly IUserIdentityProvider _identityProvider;

        public CustomerContractsController(ISalesService salesService, IUserIdentityProvider identityProvider)
        {
            if (salesService == null) { throw new ArgumentNullException("salesService"); }
            _salesService = salesService;

            if(identityProvider == null) throw new ArgumentNullException("identityProvider");
            _identityProvider = identityProvider;
        }

        [Route("")]
        public PagedDataResponse<CustomerContractSummaryResponse> Get(string customerKey = null, ContractStatus? status = null, int skipCount = 0, int pageSize = 50)
        {
            return PagedDataResponse<CustomerContractSummaryResponse>.BuildPagedDataResponse(
                GetCustomerContractQuery(customerKey, null), pageSize: pageSize, skipCount: skipCount);
        }

        [Route("~/api/customers/{customerKey}/contracts"), HttpGet]
        public IEnumerable<CustomerContractSummaryResponse> GetCurrentContractsForCustomer(string customerKey, int? take = null)
        {
            var custContractsQuery = _salesService.GetCustomerContracts(new FilterCustomerContractsParameters
            {
                CustomerKey = customerKey
            });

            custContractsQuery.EnsureSuccessWithHttpResponseException();
            var results = custContractsQuery.ResultingObject
                .Where(c => c.ContractStatus == ContractStatus.Confirmed || c.ContractStatus == ContractStatus.Pending)
                .OrderByDescending(c => c.ContractDate);

            //NOTE: QC > LabResults > Allowances and SalesOrders > Contract Selection features are dependent on the returned data being pending or confirmed.
            var query = take.HasValue 
                ? results.Take(take.Value)
                : results;

            return query.Project().To<CustomerContractSummaryResponse>();
        }

        [Route("{contractKey}", Name = "ContractDetailsRoute")]
        public async Task<CustomerContractResponse> Get(string contractKey)
        {
            var getContractResult = _salesService.GetCustomerContract(contractKey);
            if (getContractResult.State == ResultState.Invalid)
            {
                int contractNumber;
                if (int.TryParse(contractKey, out contractNumber))
                {
                    var getContractsResult = _salesService.GetCustomerContracts();
                    getContractsResult.EnsureSuccessWithHttpResponseException();
                    var contractByNumber = await getContractsResult.ResultingObject.FirstOrDefaultAsync(c => c.ContractNumber == contractNumber);
                    if(contractByNumber != null) return await Get(contractByNumber.CustomerContractKey);
                }
            }
            
            getContractResult.EnsureSuccessWithHttpResponseException();

            var contract = getContractResult.ResultingObject.Map().To<CustomerContractResponse>();
            contract.ContractItems = contract.ContractItems.OrderBy(c => c.ChileProductCode);

            contract.Links = new ResourceLinkCollection
            {
                BuildContractDetailsLink(contractKey, "self"),
                BuildContractReportReport(contractKey),
                BuildContractDrawSummaryReportLink(contractKey),
            };

            return contract;
        }

        [Route("{contractKey}"),
        ClaimsAuthorize(ClaimActions.Modify, ClaimTypes.SalesClaimTypes.CustomerContracts)]
        public HttpResponseMessage Put(string contractKey, [FromBody]UpdateCustomerContractRequest values)
        {
            if (!ModelState.IsValid)
            {
                var message = new HttpResponseMessage(HttpStatusCode.BadRequest);
                message.Content = new StringContent("The values contain some invalid data. Please check the data and try again.");
                throw new HttpResponseException(message);
            }

            var dto = values.Map().To<Services.Models.Parameters.UpdateCustomerContractParameters>();
            dto.ContractKey = contractKey;
            _identityProvider.SetUserIdentity(dto);
            return _salesService.UpdateCustomerContract(dto).ToHttpResponseMessage(System.Web.Mvc.HttpVerbs.Put);
        }

        [Route("{contractKey}"),
        ClaimsAuthorize(ClaimActions.Delete, ClaimTypes.SalesClaimTypes.CustomerContracts)]
        public HttpResponseMessage Delete(string contractKey)
        {
            return _salesService.RemoveCustomerContract(contractKey).ToHttpResponseMessage(System.Web.Mvc.HttpVerbs.Delete);
        }

        [ClaimsAuthorize(ClaimActions.Create, ClaimTypes.SalesClaimTypes.CustomerContracts)]
        [Route("")] // though it seems redundant, without this RouteAttribute, the POST operation return 405 Method Not Allowed :( VK 20151215
        public string Post([FromBody]CreateCustomerContractRequest values)
        {
            if (!ModelState.IsValid)
            {
                var message = new HttpResponseMessage(HttpStatusCode.BadRequest);
                message.Content = new StringContent("The values contain some invalid data. Please check the data and try again.");
                throw new HttpResponseException(message);
            }

            var dto = values.Map().To<Services.Models.Parameters.CreateCustomerContractParameters>();
            _identityProvider.SetUserIdentity(dto);
            var result = _salesService.CreateCustomerContract(dto);
            result.EnsureSuccessWithHttpResponseException(System.Web.Mvc.HttpVerbs.Post);

            return result.ResultingObject;
        }

        [Route("{contractKey}/shipments")]
        public ContractShipmentSummaries GetShipmentSummary(string contractKey)
        {
            var result = _salesService.GetContractShipmentSummary(contractKey);
            result.EnsureSuccessWithHttpResponseException();
            var dataItems = result.ResultingObject.Map().To<IEnumerable<ContractShipmentSummaryItem>>().ToList();

            dataItems.ForEach(item => item.Links = new ResourceLinkCollection
            {
                BuildContractDrawSummaryReportLink(contractKey, "self")
            });

            return new ContractShipmentSummaries(dataItems)
            {
                Links = new ResourceLinkCollection { }
            };
        }

        [ActionName("Post"), Route("setStatus")]
        public HttpResponseMessage SetContractsStatus([FromBody]SetContractsStatusRequest parameters)
        {
            var serviceParameters = parameters.Map().To<Services.Models.Parameters.SetContractsStatusParameters>();
            var result = _salesService.SetCustomerContractsStatus(serviceParameters);
            return result.ToHttpResponseMessage(System.Web.Mvc.HttpVerbs.Post);
        }

        private IQueryable<ICustomerContractSummaryReturn> GetCustomerContractQuery(string customerKey, ContractStatus? contractStatus)
        {
            var param = new FilterCustomerContractsParameters
            {
                CustomerKey = customerKey,
                ContractStatus = contractStatus,               
            };

            var results = _salesService.GetCustomerContracts(param);
            results.EnsureSuccessWithHttpResponseException();
            return results.ResultingObject.OrderByDescending(r => r.ContractDate);
        }

        private KeyValuePair<string, Link> BuildContractDetailsLink(string contractKey, string rel = "contract-details") { 
            return new KeyValuePair<string, Link>(rel, new Link
            {
                HRef = Url.Route("ContractDetailsRoute", new { contractKey })
            });
        }

        private KeyValuePair<string, Link> BuildContractReportReport(string contractKey, string rel = "contract-report")
        {
            return new KeyValuePair<string, Link>(rel,
                new Link {HRef = Url.Route("CustomerContractReport", new {contractKey})});
        }

        private KeyValuePair<string, Link> BuildContractDrawSummaryReportLink(string contractKey, string rel = "contract-draw-summary-report")
        {
            return new KeyValuePair<string, Link>(rel,
                new Link {HRef = Url.Route("CustomerContractDrawSummaryReport", new {contractKey})});
        }
    }
}
