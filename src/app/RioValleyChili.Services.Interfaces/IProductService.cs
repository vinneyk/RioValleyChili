using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IProductService
    {
        #region additive product methods

        IResult<string> CreateAdditiveProduct(ICreateAdditiveProductParameters parameters);

        IResult UpdateAdditiveProduct(IUpdateAdditiveProductParameters parameters);

        IResult<IQueryable<IAdditiveProductReturn>> GetAdditiveProducts(bool includeInactive = false, bool filterProductsWithInventory = false);

        IResult<IEnumerable<IAdditiveTypeReturn>> GetAdditiveTypes();

        #endregion
        
        #region chile product methods

        IResult<string> CreateChileProduct(ICreateChileProductParameters parameters);

        IResult UpdateChileProduct(IUpdateChileProductParameters parameters);

        IResult<IChileProductDetailReturn> GetChileProductDetail(string keyValue);

        IResult<IQueryable<IChileProductReturn>> GetChileProducts(ChileStateEnum? chileState = null, bool includeInactive = false, bool filterProductsWithInventory = false);

        IResult<IQueryable<IChileTypeSummaryReturn>> GetChileTypeSummaries();

        IResult<IEnumerable<string>> GetChileAttributeNames();

        [Obsolete("Use UpdateChileProduct method instead. -RI 2016-09-05")]
        IResult SetChileProductAttributeRanges(ISetChileProductAttributeRangesParameters parameters);

        [Obsolete("Use UpdateChileProduct method instead. -RI 2016-09-05")]
        IResult SetChileProductIngredients(ISetChileProductIngredientsParameters parameters);

        #endregion

        #region packaging product methods

        IResult<string> CreatePackagingProduct(ICreatePackagingProductParameters parameters);

        IResult UpdatePackagingProduct(IUpdatePackagingProductParameters parameters);

        IResult<IQueryable<IPackagingProductReturn>> GetPackagingProducts(bool includeInactive = false, bool filterProductsWithInventory = false);

        #endregion

        #region non-inventory product methods

        IResult<string> CreateNonInventoryProduct(ICreateProductParameters parameters);

        IResult UpdateNonInventoryProduct(IUpdateProductParameters parameters);

        #endregion

        IResult<IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<string>>>> GetProductSubTypes();

        IResult<IQueryable<IProductReturn>> GetProducts(ProductTypeEnum? productType, bool includeInactive);
    }
}