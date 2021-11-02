using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class ProductService : IProductService
    {
        #region fields and constructors

        private readonly ProductServiceProvider _productServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public ProductService(ProductServiceProvider productServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(productServiceProvider == null) { throw new ArgumentNullException("productServiceProvider"); }
            _productServiceProvider = productServiceProvider;

            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }
            _exceptionLogger = exceptionLogger;
        }
        
        #endregion

        #region additive product methods

        public IResult<string> CreateAdditiveProduct(ICreateAdditiveProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.CreateAdditiveProduct(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateAdditiveProduct(IUpdateAdditiveProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.UpdateAdditiveProduct(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IAdditiveProductReturn>> GetAdditiveProducts(bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            try
            {
                return _productServiceProvider.GetAdditiveProductSummaries(includeInactive, filterProductsWithInventory);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IAdditiveProductReturn>>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<IAdditiveTypeReturn>> GetAdditiveTypes()
        {
            try
            {
                return _productServiceProvider.GetAdditiveTypes();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<IAdditiveTypeReturn>>(null, ex.Message);
            }
        }

        #endregion

        #region chile product methods

        public IResult<string> CreateChileProduct(ICreateChileProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.CreateChileProduct(parameters);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateChileProduct(IUpdateChileProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.UpdateChileProduct(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<IChileProductDetailReturn> GetChileProductDetail(string keyValue)
        {
            try
            {
                return _productServiceProvider.GetChileProductDetail(keyValue);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IChileProductDetailReturn>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IChileProductReturn>> GetChileProducts(ChileStateEnum? chileState = null, bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            try
            {
                return _productServiceProvider.GetChileProducts(chileState, includeInactive, filterProductsWithInventory);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return null;
            }
        }

        public IResult<IQueryable<IChileTypeSummaryReturn>> GetChileTypeSummaries()
        {
            try
            {
                return _productServiceProvider.GetChileTypeSummaries();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IChileTypeSummaryReturn>>(null, ex.Message);
            }
        }

        public IResult<IEnumerable<string>> GetChileAttributeNames()
        {
            try
            {
                return _productServiceProvider.GetChileAttributeNames();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IEnumerable<string>>(null, ex.Message);
            }
        }

        public IResult SetChileProductAttributeRanges(ISetChileProductAttributeRangesParameters parameters)
        {
            try
            {
                return _productServiceProvider.SetChileProductAttributeRange(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult SetChileProductIngredients(ISetChileProductIngredientsParameters parameters)
        {
            try
            {
                return _productServiceProvider.SetChileProductIngredients(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        #endregion

        #region packaging product methods

        public IResult<string> CreatePackagingProduct(ICreatePackagingProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.CreatePackagingProduct(parameters);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdatePackagingProduct(IUpdatePackagingProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.UpdatePackagingProduct(parameters);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult<IQueryable<IPackagingProductReturn>> GetPackagingProducts(bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            try
            {
                return _productServiceProvider.GetPackagingProducts(includeInactive, filterProductsWithInventory);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IPackagingProductReturn>>(null, ex.Message);
            }
        }

        #endregion

        #region non-inventory product methods

        public IResult<string> CreateNonInventoryProduct(ICreateProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.CreateNonInventoryProduct(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        public IResult UpdateNonInventoryProduct(IUpdateProductParameters parameters)
        {
            try
            {
                return _productServiceProvider.UpdateNonInventoryProduct(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.Message);
            }
        }

        #endregion

        public IResult<IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<string>>>> GetProductSubTypes()
        {
            try
            {
                return _productServiceProvider.GetProductSubTypes();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return null;
            }
        }

        public IResult<IQueryable<IProductReturn>> GetProducts(ProductTypeEnum? productType, bool includeInactive)
        {
            try
            {
                return _productServiceProvider.GetProducts(productType, includeInactive);
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return null;
            }
        }
    }
}
