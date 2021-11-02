using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Products
{
    internal class UpdateProductCommand
    {
        private readonly IProductUnitOfWork _productUnitOfWork;

        internal UpdateProductCommand(IProductUnitOfWork productUnitOfWork)
        {
            if(productUnitOfWork == null) { throw new ArgumentNullException("productUnitOfWork"); }
            _productUnitOfWork = productUnitOfWork;
        }

        internal IResult<AdditiveProduct> UpdateAdditiveProduct(UpdateAdditiveProductParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) {  throw new ArgumentNullException("parameters"); }

            var additiveProduct = _productUnitOfWork.AdditiveProductRepository.FindByKey(parameters.ProductKey, p => p.Product);
            if(additiveProduct == null)
            {
                return new InvalidResult<AdditiveProduct>(null, string.Format(UserMessages.AdditiveProductNotFound, parameters.ProductKey.KeyValue));
            }

            var additiveType = _productUnitOfWork.AdditiveTypeRepository.FindByKey(parameters.AdditiveTypeKey);
            if(additiveType == null)
            {
                return new InvalidResult<AdditiveProduct>(null, string.Format(UserMessages.AdditiveTypeNotFound, parameters.AdditiveTypeKey.KeyValue));
            }

            var update = UpdateProduct(additiveProduct.Product, parameters.Parameters);
            if(!update.Success)
            {
                return update.ConvertTo<AdditiveProduct>();
            }
            additiveProduct.AdditiveTypeId = additiveType.Id;

            return new SuccessResult<AdditiveProduct>(additiveProduct);
        }

        internal IResult<ChileProduct> UpdateChileProduct(UpdateChileProductParameters parameters, DateTime timeStamp, out List<AdditiveTypeKey> deletedIngredients)
        {
            deletedIngredients = null;
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileProduct = _productUnitOfWork.ChileProductRepository.FindByKey(parameters.ProductKey, c => c.Product,
                c => c.Ingredients,
                c => c.ProductAttributeRanges);
            if(chileProduct == null)
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.ChileProductNotFound, parameters.ProductKey.KeyValue));
            }

            var chileType = _productUnitOfWork.ChileTypeRepository.FindByKey(parameters.ChileTypeKey);
            if(chileType == null)
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.ChileTypeNotFound, parameters.ChileTypeKey.KeyValue));
            }

            var update = UpdateProduct(chileProduct.Product, parameters.Parameters);
            if(!update.Success)
            {
                return update.ConvertTo<ChileProduct>();
            }

            chileProduct.ChileTypeId = chileType.Id;
            chileProduct.Mesh = parameters.Parameters.Mesh;
            chileProduct.IngredientsDescription = parameters.Parameters.IngredientsDescription;

            var setRangesResult = new SetChileProductAttributeRangesCommand(_productUnitOfWork).Execute(chileProduct, parameters.Parameters, timeStamp, parameters.AttributeRanges);
            if(!setRangesResult.Success)
            {
                return setRangesResult.ConvertTo<ChileProduct>();
            }

            return new SetChileProductIngredientsCommand(_productUnitOfWork).Execute(chileProduct, parameters.Parameters, timeStamp, parameters.Ingredients, out deletedIngredients);
        }

        internal IResult<PackagingProduct> UpdatePackagingProduct(UpdatePackagingProductParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Parameters.Weight < 0)
            {
                return new InvalidResult<PackagingProduct>(null, UserMessages.NegativeWeight);
            }

            if(parameters.Parameters.PackagingWeight < 0)
            {
                return new InvalidResult<PackagingProduct>(null, UserMessages.NegativeWeight);
            }

            if(parameters.Parameters.PalletWeight < 0)
            {
                return new InvalidResult<PackagingProduct>(null, UserMessages.NegativePalletWeight);
            }

            var packagingProduct = _productUnitOfWork.PackagingProductRepository.FindByKey(parameters.ProductKey, p => p.Product);
            if(packagingProduct == null)
            {
                return new InvalidResult<PackagingProduct>(null, string.Format(UserMessages.PackagingProductNotFound, parameters.ProductKey.KeyValue));
            }

            var update = UpdateProduct(packagingProduct.Product, parameters.Parameters);
            if(!update.Success)
            {
                return update.ConvertTo<PackagingProduct>();
            }

            packagingProduct.Weight = parameters.Parameters.Weight;
            packagingProduct.PackagingWeight = parameters.Parameters.PackagingWeight;
            packagingProduct.PalletWeight = parameters.Parameters.PalletWeight;

            return new SuccessResult<PackagingProduct>(packagingProduct);
        }

        internal IResult<Product> UpdateProduct(Product product, IUpdateProductParameters parameters)
        {
            if(product == null) {  throw new ArgumentNullException("product"); }
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            product.Name = parameters.ProductName;
            product.ProductCode = parameters.ProductCode;
            product.IsActive = parameters.IsActive;

            return new SuccessResult<Product>(product);
        }
    }
}