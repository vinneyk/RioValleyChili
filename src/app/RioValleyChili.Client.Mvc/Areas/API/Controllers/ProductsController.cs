using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [System.Web.Http.RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly IProductService _productsService;
        private readonly IUserIdentityProvider _userIdentityProvider;
        
        public ProductsController(IProductService productsService, IUserIdentityProvider userIdentityProvider)
        {
            if(productsService == null) { throw new ArgumentNullException("productsService"); }
            _productsService = productsService;

            if(userIdentityProvider == null) { throw new ArgumentNullException("userIdentityProvider");}
            _userIdentityProvider = userIdentityProvider;
        }

        // GET: api/products/finishedGood
        [OutputCache(Duration = 600, VaryByParam = "*"), System.Web.Http.HttpGet]
        [Issue("There appears to be some inconsistency/misunderstanding between lot type, chile state, and chile variety in" +
               "the new context and possibly ProdGrpID and PTypeID in the old." +
               "This method is modified to fulfill the specific expectation of returning GRP products when supplying the GRP LotTypeEnum," +
               "although previous implementation would *not* have done so because GRP products are actually loaded with ChileStates of" +
               "WIP and FinishedGoods - ChileState being the actual property queried for and translated from LotTypeEnum (in the case of" +
               "LotTypeEnum.GRP it translates to ChileStateEnum.OtherRaw)." +
               "-RI 2016-08-09",
            References = new[]{ "RVCADMIN-1228" })]
        [System.Web.Http.Route("{productType}")]
        public IEnumerable<IProductReturn> Get(ProductTypeEnum productType, LotTypeEnum? lotType = null, bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            IQueryable<IProductReturn> data;
            switch(productType)
            {
                case ProductTypeEnum.Chile:
                    var chileLotType = lotType ?? LotTypeEnum.FinishedGood;
                    var chileState = chileLotType.ToChileState();
                    IResult<IQueryable<IChileProductReturn>> result;
                    if(chileState == ChileStateEnum.OtherRaw && chileLotType == LotTypeEnum.GRP)
                    {
                        result = _productsService.GetChileProducts(null, includeInactive, filterProductsWithInventory);
                        result.EnsureSuccessWithHttpResponseException();
                        data = result.ResultingObject.Where(c => c.ChileTypeDescription == "GRP");
                    }
                    else
                    {
                        result = _productsService.GetChileProducts(chileState, includeInactive, filterProductsWithInventory);
                        result.EnsureSuccessWithHttpResponseException();
                        data = result.ResultingObject;
                    }
                    break;

                case ProductTypeEnum.Packaging:
                    var packagingResult = _productsService.GetPackagingProducts(includeInactive, filterProductsWithInventory);
                    packagingResult.EnsureSuccessWithHttpResponseException();
                    data = packagingResult.ResultingObject;
                    break;

                case ProductTypeEnum.Additive:
                    var additiveResult = _productsService.GetAdditiveProducts(includeInactive, filterProductsWithInventory);
                    additiveResult.EnsureSuccessWithHttpResponseException();
                    data = additiveResult.ResultingObject;
                    break;
                    
                default:
                    var productsResult = _productsService.GetProducts(productType, includeInactive);
                    productsResult.EnsureSuccessWithHttpResponseException();
                    data = productsResult.ResultingObject;
                    break;
            }

            return data
                .Where(p => includeInactive || p.IsActive)
                .OrderBy(p => p.ProductName);
        }


        [System.Web.Http.Route("{productType}/{id}"), System.Web.Http.HttpGet]
        public IProductReturn Get(ProductTypeEnum productType, string id)
        {
            switch(productType)
            {
                case ProductTypeEnum.Chile:
                    var chileProductResult = _productsService.GetChileProductDetail(id);
                    chileProductResult.EnsureSuccessWithHttpResponseException();
                    return chileProductResult.ResultingObject;

                case ProductTypeEnum.Additive:
                    var additiveProductResult = _productsService.GetAdditiveProducts(includeInactive: true);
                    additiveProductResult.EnsureSuccessWithHttpResponseException();
                    var additiveProduct = additiveProductResult.ResultingObject.ToList().FirstOrDefault(p => p.ProductKey == id);
                    if(additiveProduct == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    return additiveProduct;

                case ProductTypeEnum.Packaging:
                    var packagingProductResults = _productsService.GetPackagingProducts(includeInactive: true);
                    packagingProductResults.EnsureSuccessWithHttpResponseException();
                    var packagingProduct = packagingProductResults.ResultingObject.ToList().FirstOrDefault(p => p.ProductKey == id);
                    if(packagingProduct == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    return packagingProduct;

                default:
                    var productResults = _productsService.GetProducts(productType, true);
                    productResults.EnsureSuccessWithHttpResponseException();
                    var product = productResults.ResultingObject.ToList().FirstOrDefault(p => p.ProductKey == id);
                    if(product == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    return product;
            }
        }

        // GET: api/products/
        public async Task<HttpResponseMessage> Post([FromBody]CreateProductDto values)
        {
            if(!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            
            _userIdentityProvider.SetUserIdentity(values);

            Task<IResult<string>> task;
            switch (values.ProductType)
            {
                case ProductTypeEnum.Additive:
                    task = Task.Run(() => _productsService.CreateAdditiveProduct(values));
                    break;

                case ProductTypeEnum.Chile:
                    task = Task.Run(() => _productsService.CreateChileProduct(values));
                    break;

                case ProductTypeEnum.Packaging:
                    task = Task.Run(() => _productsService.CreatePackagingProduct(values));
                    break;

                case ProductTypeEnum.NonInventory:
                    task = Task.Run(() => _productsService.CreateNonInventoryProduct(values));
                    break;

                default: throw new NotSupportedException(string.Format("The ProductTypeEnum '{0}' is not supported.", values.ProductType));
            }

            await Task.WhenAll(new Task[] { task });

            return task.Result.ToHttpResponseMessage(HttpVerbs.Post);
        }

        // PUB: api/products/1
        [System.Web.Http.HttpPut, System.Web.Http.Route("{id}", Order=0)]
        public async Task<HttpResponseMessage> Put(string id, [FromBody]UpdateProductDto values)
        {
            if (!ModelState.IsValid) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            values.ProductKey = id;
            _userIdentityProvider.SetUserIdentity(values);

            Task<IResult> task;
            switch (values.ProductType)
            {
                case ProductTypeEnum.Additive:
                    task = Task.Run(() => _productsService.UpdateAdditiveProduct(values));
                    break;

                case ProductTypeEnum.Chile:
                    task = Task.Run(() => _productsService.UpdateChileProduct(values));
                    break;

                case ProductTypeEnum.Packaging:
                    task = Task.Run(() => _productsService.UpdatePackagingProduct(values));
                    break;

                case ProductTypeEnum.NonInventory:
                    task = Task.Run(() => _productsService.UpdateNonInventoryProduct(values));
                    break;

                default: throw new NotSupportedException(string.Format("The ProductType '{0}' is not supported.", values.ProductType));
            }

            await Task.WhenAll(new Task[] {task});

            var response = task.Result.ToHttpResponseMessage(HttpVerbs.Put);
            return response;
        }
    }
}