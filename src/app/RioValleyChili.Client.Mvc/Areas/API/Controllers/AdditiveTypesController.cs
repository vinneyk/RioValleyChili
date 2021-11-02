using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Areas.API.Controllers
{
    [Obsolete("Use IngredientsController instead.")]
    public class AdditiveTypesController : ApiController
    {
        private readonly IProductService _productService;

        public AdditiveTypesController(IProductService productService)
        {
            if (productService == null) throw new ArgumentNullException("productService");
            _productService = productService;
        }

        public IEnumerable<AdditiveTypeDto> Get()
        {
            var result = _productService.GetAdditiveTypes();
            result.EnsureSuccessWithHttpResponseException();
            return result.ResultingObject
                .OrderBy(r => r.AdditiveTypeDescription).ToList()
                .Select(a => new AdditiveTypeDto
                {
                    Key = a.AdditiveTypeKey,
                    Description = a.AdditiveTypeDescription,
                });
        }
    }
}
