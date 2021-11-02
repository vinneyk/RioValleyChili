using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands.Products;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class ProductServiceProvider: IUnitOfWorkContainer<IProductUnitOfWork>
    {
        #region Fields and Constructors.

        private readonly IProductUnitOfWork _productUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public ProductServiceProvider(IProductUnitOfWork productUnitOfWork, ITimeStamper timeStamper)
        {
            if(productUnitOfWork == null) { throw new ArgumentNullException("productUnitOfWork"); }
            _productUnitOfWork = productUnitOfWork;

            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }
            _timeStamper = timeStamper;
        }

        #endregion

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult<string> CreateAdditiveProduct(ICreateAdditiveProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedResults = parameters.ToParsedParameters();
            if(!parsedResults.Success)
            {
                return parsedResults.ConvertTo<string>();
            }

            var commandResult = new CreateProductCommand(_productUnitOfWork).CreateAdditiveProduct(parsedResults.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(commandResult.ResultingObject.ToAdditiveProductKey()),
                new SyncProductParameters
                    {
                        ProductKey = commandResult.ResultingObject.ToProductKey()
                    });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult UpdateAdditiveProduct(IUpdateAdditiveProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedResults = parameters.ToParsedParameters();
            if(!parsedResults.Success)
            {
                return parsedResults.ConvertTo<string>();
            }

            var commandResult = new UpdateProductCommand(_productUnitOfWork).UpdateAdditiveProduct(parsedResults.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(commandResult.ResultingObject.ToAdditiveProductKey()),
                new SyncProductParameters
                    {
                        ProductKey = commandResult.ResultingObject.ToProductKey()
                    });
        }

        public IResult<IQueryable<IAdditiveProductReturn>> GetAdditiveProductSummaries(bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            var additiveProduct = ProductProjectors.SelectAdditiveProduct();

            Expression<Func<AdditiveProduct, bool>> predicate = p => includeInactive || p.Product.IsActive;
            if(filterProductsWithInventory)
            {
                var additiveLots = _productUnitOfWork.AdditiveLotRepository.All();
                predicate = predicate.And(a => additiveLots.Any(l => l.AdditiveProductId == a.Id && l.Lot.Inventory.Any()));
            }

            var result = _productUnitOfWork.AdditiveProductRepository.All().AsExpandable()
                .Where(predicate)
                .Select(a => additiveProduct.Invoke(a));
            return new SuccessResult<IQueryable<IAdditiveProductReturn>>(result);
        }

        public IResult<IEnumerable<IAdditiveTypeReturn>> GetAdditiveTypes()
        {
            var select = AdditiveTypeProjectors.Select();
            return new SuccessResult<IEnumerable<IAdditiveTypeReturn>>(_productUnitOfWork.AdditiveTypeRepository.All().AsExpandable().Select(select).ToList());
        }

        public IResult<IQueryable<IChileProductReturn>> GetChileProducts(ChileStateEnum? chileState = null, bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            var chileProduct = ProductProjectors.SelectChileProductSummary();

            Expression<Func<ChileProduct, bool>> predicate = p => (chileState == null || p.ChileState == chileState) && (includeInactive || p.Product.IsActive);
            if(filterProductsWithInventory)
            {
                var chileLots = _productUnitOfWork.ChileLotRepository.All();
                predicate = predicate.And(p => chileLots.Any(c => c.ChileProductId == p.Id && c.Lot.Inventory.Any()));
            }

            var query =  _productUnitOfWork.ChileProductRepository
                .All().AsExpandable()
                .Where(predicate)
                .Select(c => chileProduct.Invoke(c));

            return new SuccessResult<IQueryable<IChileProductReturn>>(query);
        }

        public IResult<IChileProductDetailReturn> GetChileProductDetail(string keyValue)
        {
            if(keyValue == null) { throw new ArgumentNullException("keyValue"); }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(keyValue);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<IChileProductDetailReturn>();
            }
            var predicate = new ChileProductKey(chileProductKeyResult.ResultingObject).FindByPredicate;
            var selector = ProductProjectors.SelectChileProductDetail();
            
            var chileProduct = _productUnitOfWork.ChileProductRepository.Filter(predicate).AsExpandable().Select(selector).FirstOrDefault();
            if(chileProduct == null)
            {
                return new FailureResult<IChileProductDetailReturn>(null, string.Format(UserMessages.ChileProductNotFound, new ChileProductKey(chileProductKeyResult.ResultingObject).KeyValue));
            }

            return new SuccessResult<IChileProductDetailReturn>(chileProduct);
        }

        public IResult<IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<string>>>> GetProductSubTypes()
        {
            var result = new Dictionary<ProductTypeEnum, IEnumerable<string>>
                {
                    { ProductTypeEnum.Chile, _productUnitOfWork.ChileTypeRepository.All().Distinct().Select(c => c.Description).ToList() },
                    { ProductTypeEnum.Additive, _productUnitOfWork.AdditiveTypeRepository.All().Distinct().Select(a => a.Description).ToList() },
                    { ProductTypeEnum.Packaging, new List<string>() }
                };

            return new SuccessResult<IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<string>>>>(result);
        }

        public IResult<IQueryable<IPackagingProductReturn>> GetPackagingProducts(bool includeInactive = false, bool filterProductsWithInventory = false)
        {
            var select = ProductProjectors.SelectPackagingProduct();

            Expression<Func<PackagingProduct, bool>> predicate = p => includeInactive || p.Product.IsActive;
            if(filterProductsWithInventory)
            {
                var packagingLots = _productUnitOfWork.PackagingLotRepository.All();
                predicate = predicate.And(p => packagingLots.Any(l => l.PackagingProductId == p.Id && l.Lot.Inventory.Any()));
            }
            var query = _productUnitOfWork.PackagingProductRepository.All().AsExpandable()
                .Where(predicate)
                .Select(select);
            return new SuccessResult<IQueryable<IPackagingProductReturn>>(query);
        }

        public IResult<IQueryable<IChileTypeSummaryReturn>> GetChileTypeSummaries()
        {
            var select = ChileTypeProjectors.SelectSummary();

            var query = _productUnitOfWork.ChileTypeRepository.All().AsExpandable().Select(select);
            return new SuccessResult<IQueryable<IChileTypeSummaryReturn>>(query);
        }

        public IResult<IEnumerable<string>> GetChileAttributeNames()
        {
            var chileAttributeNames = _productUnitOfWork.AttributeNameRepository.All().Where(a => a.Active && a.ValidForChileInventory).Select(n => n.Name).ToList();
            return new SuccessResult<IEnumerable<string>>(chileAttributeNames);
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult SetChileProductAttributeRange(ISetChileProductAttributeRangesParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedParameters = parameters.ToParsedParameters();
            if(!parsedParameters.Success)
            {
                return parsedParameters.ConvertTo<string>();
            }

            var setResult = new SetChileProductAttributeRangesCommand(_productUnitOfWork).Execute(parsedParameters.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!setResult.Success)
            {
                return setResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(),
                new SyncProductParameters
                {
                    ProductKey = setResult.ResultingObject.ToProductKey()
                });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult SetChileProductIngredients(ISetChileProductIngredientsParameters parameters)
        {
            var parametersResult = parameters.ToParsedParameters();
            if(!parametersResult.Success)
            {
                return parametersResult.ConvertTo<string>();
            }

            List<AdditiveTypeKey> deletedIngredients;
            var commandResult = new SetChileProductIngredientsCommand(_productUnitOfWork).Execute(_timeStamper.CurrentTimeStamp, parametersResult.ResultingObject, out deletedIngredients);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncProductParameters
                {
                    ProductKey = new ProductKey(parametersResult.ResultingObject.ChileProductKey),
                    DeletedIngredients = deletedIngredients
                });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult<string> CreateChileProduct(ICreateChileProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedResults = parameters.ToParsedParameters();
            if(!parsedResults.Success)
            {
                return parsedResults.ConvertTo<string>();
            }

            List<AdditiveTypeKey> deletedIngredients;
            var commandResult = new CreateProductCommand(_productUnitOfWork).CreateChileProduct(parsedResults.ResultingObject, _timeStamper.CurrentTimeStamp, out deletedIngredients);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(commandResult.ResultingObject.ToChileProductKey()),
                new SyncProductParameters
                    {
                        ProductKey = commandResult.ResultingObject.ToProductKey(),
                        DeletedIngredients = deletedIngredients
                    });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult UpdateChileProduct(IUpdateChileProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedResults = parameters.ToParsedParameters();
            if(!parsedResults.Success)
            {
                return parsedResults.ConvertTo<string>();
            }

            List<AdditiveTypeKey> deletedIngredients;
            var commandResult = new UpdateProductCommand(_productUnitOfWork).UpdateChileProduct(parsedResults.ResultingObject, _timeStamper.CurrentTimeStamp, out deletedIngredients);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(),
                new SyncProductParameters
                    {
                        ProductKey = commandResult.ResultingObject.ToProductKey(),
                        DeletedIngredients = deletedIngredients
                    });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult<string> CreatePackagingProduct(ICreatePackagingProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedResults = parameters.ToParsedParameters();
            if(!parsedResults.Success)
            {
                return parsedResults.ConvertTo<string>();
            }

            var commandResult = new CreateProductCommand(_productUnitOfWork).CreatePackagingProduct(parsedResults.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(commandResult.ResultingObject.ToPackagingProductKey()),
                new SyncProductParameters
                    {
                        ProductKey = commandResult.ResultingObject.ToProductKey()
                    });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult UpdatePackagingProduct(IUpdatePackagingProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var parsedResults = parameters.ToParsedParameters();
            if(!parsedResults.Success)
            {
                return parsedResults.ConvertTo<string>();
            }

            var commandResult = new UpdateProductCommand(_productUnitOfWork).UpdatePackagingProduct(parsedResults.ResultingObject, _timeStamper.CurrentTimeStamp);
            if(!commandResult.Success)
            {
                return commandResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(),
                new SyncProductParameters
                    {
                        ProductKey = commandResult.ResultingObject.ToProductKey()
                    });
        }

        IProductUnitOfWork IUnitOfWorkContainer<IProductUnitOfWork>.UnitOfWork { get { return _productUnitOfWork; } }

        public IResult<IQueryable<IProductReturn>> GetProducts(ProductTypeEnum? productType, bool includeInactive)
        {
            var select = ProductProjectors.SelectProduct();

            Expression<Func<Product, bool>> predicate = p => includeInactive || p.IsActive;
            if(productType.HasValue)
            {
                predicate = predicate.And(p => p.ProductType == productType.Value);
            }

            var query = _productUnitOfWork.ProductRepository.All().AsExpandable()
                .Where(predicate)
                .Select(select);

            return new SuccessResult<IQueryable<IProductReturn>>(query);
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult<string> CreateNonInventoryProduct(ICreateProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var createResult = new CreateProductCommand(_productUnitOfWork).CreateProduct(parameters, ProductTypeEnum.NonInventory);
            if(!createResult.Success)
            {
                return createResult.ConvertTo<string>();
            }

            _productUnitOfWork.Commit();

            var productKey = createResult.ResultingObject.ToProductKey();
            return SyncParameters.Using(new SuccessResult<string>(productKey), new SyncProductParameters { ProductKey = productKey });
        }

        [SynchronizeOldContext(NewContextMethod.Product)]
        public IResult UpdateNonInventoryProduct(IUpdateProductParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var productKeyResult = KeyParserHelper.ParseResult<IProductKey>(parameters.ProductKey);
            if(!productKeyResult.Success)
            {
                return productKeyResult;
            }
            var productKey = productKeyResult.ResultingObject.ToProductKey();

            var product = _productUnitOfWork.ProductRepository.FindByKey(productKey);
            if(product == null || product.ProductType != ProductTypeEnum.NonInventory)
            {
                return new InvalidResult<string>(null, string.Format(UserMessages.NonInventoryProductNotFound, productKey));
            }

            var updateResult = new UpdateProductCommand(_productUnitOfWork).UpdateProduct(product, parameters);
            if(!updateResult.Success)
            {
                return updateResult;
            }

            _productUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncProductParameters { ProductKey = productKey });
        }
    }
}