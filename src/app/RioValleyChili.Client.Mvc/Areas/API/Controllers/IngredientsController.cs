using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Production;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    public class IngredientsController : ApiController
    {
        private readonly IProductService _productService;

        public IngredientsController(IProductService productService)
        {
            if(productService == null) throw new ArgumentNullException("productService");
            _productService = productService;
        }

        [OutputCache(Duration=600)]
        public IDictionary<ProductTypeEnum, IEnumerable<Ingredient>> Get()
        {
            var dictionary = new Dictionary<ProductTypeEnum, IEnumerable<Ingredient>>();

            var result = _productService.GetAdditiveTypes();
            result.EnsureSuccessWithHttpResponseException();
            dictionary.Add(ProductTypeEnum.Additive, result.ResultingObject.Project().To<Ingredient>());
            dictionary.Add(ProductTypeEnum.Chile, new Ingredient[0]);
            dictionary.Add(ProductTypeEnum.Packaging, new Ingredient[0]);

            return dictionary;
        } 
    }
}
