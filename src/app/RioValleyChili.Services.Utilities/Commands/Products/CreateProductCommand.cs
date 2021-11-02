using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.ProductService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Products
{
    internal class CreateProductCommand
    {
        private readonly IProductUnitOfWork _productUnitOfWork;

        internal CreateProductCommand(IProductUnitOfWork productUnitOfWork)
        {
            if(productUnitOfWork == null) { throw new ArgumentNullException("productUnitOfWork"); }
            _productUnitOfWork = productUnitOfWork;
        }

        internal IResult<AdditiveProduct> CreateAdditiveProduct(CreateAdditiveProductParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var additiveType = _productUnitOfWork.AdditiveTypeRepository.FindByKey(parameters.AdditiveTypeKey);
            if(additiveType == null)
            {
                return new InvalidResult<AdditiveProduct>(null, string.Format(UserMessages.AdditiveTypeNotFound, parameters.AdditiveTypeKey.KeyValue));
            }

            var productResult = CreateProduct(parameters.Parameters, ProductTypeEnum.Additive);
            if(!productResult.Success)
            {
                return productResult.ConvertTo((AdditiveProduct) null);
            }
            var product = productResult.ResultingObject;

            var additiveProduct = new AdditiveProduct
                {
                    Id = product.Id,
                    AdditiveTypeId = parameters.AdditiveTypeKey.AdditiveTypeKey_AdditiveTypeId,

                    Product = product
                };

            additiveProduct = _productUnitOfWork.AdditiveProductRepository.Add(additiveProduct);

            return new SuccessResult<AdditiveProduct>(additiveProduct);
        }

        internal IResult<ChileProduct> CreateChileProduct(CreateChileProductParameters parameters, DateTime timeStamp, out List<AdditiveTypeKey> deletedIngredients)
        {
            deletedIngredients = null;
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileType = _productUnitOfWork.ChileTypeRepository.FindByKey(parameters.ChileTypeKey);
            if(chileType == null)
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.ChileTypeNotFound, parameters.ChileTypeKey.KeyValue));
            }

            var productResult = CreateProduct(parameters.Parameters, ProductTypeEnum.Chile);
            if(!productResult.Success)
            {
                return productResult.ConvertTo((ChileProduct) null);
            }
            var product = productResult.ResultingObject;

            var chileProduct = _productUnitOfWork.ChileProductRepository.Add(new ChileProduct
                {
                    Id = product.Id,
                    ChileTypeId = parameters.ChileTypeKey.ChileTypeKey_ChileTypeId,

                    ChileState = parameters.Parameters.ChileState,
                    Mesh = parameters.Parameters.Mesh,
                    IngredientsDescription = parameters.Parameters.IngredientsDescription,
                    Product = product
                });

            var setRangesResult = new SetChileProductAttributeRangesCommand(_productUnitOfWork).Execute(chileProduct, parameters.Parameters, timeStamp, parameters.AttributeRanges);
            if(!setRangesResult.Success)
            {
                return setRangesResult.ConvertTo<ChileProduct>();
            }

            return new SetChileProductIngredientsCommand(_productUnitOfWork).Execute(chileProduct, parameters.Parameters, timeStamp, parameters.Ingredients, out deletedIngredients);
        }

        internal IResult<PackagingProduct> CreatePackagingProduct(CreatePackagingProductParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Parameters.Weight < 0)
            {
                return new InvalidResult<PackagingProduct>(null, UserMessages.NegativeWeight);
            }

            if(parameters.Parameters.PackagingWeight < 0)
            {
                return new InvalidResult<PackagingProduct>(null, UserMessages.NegativePackagingWeight);
            }

            if(parameters.Parameters.PalletWeight < 0)
            {
                return new InvalidResult<PackagingProduct>(null, UserMessages.NegativePalletWeight);
            }

            var productResult = CreateProduct(parameters.Parameters, ProductTypeEnum.Packaging);
            if(!productResult.Success)
            {
                return productResult.ConvertTo((PackagingProduct) null);
            }
            var product = productResult.ResultingObject;

            var packagingProduct = new PackagingProduct
                {
                    Id = product.Id,
                    Weight = parameters.Parameters.Weight,
                    PackagingWeight = parameters.Parameters.PackagingWeight,
                    PalletWeight = parameters.Parameters.PalletWeight,

                    Product = product
                };

            packagingProduct = _productUnitOfWork.PackagingProductRepository.Add(packagingProduct);

            return new SuccessResult<PackagingProduct>(packagingProduct);
        }

        internal IResult<Product> CreateProduct(ICreateProductParameters parameters, ProductTypeEnum productType)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var nextId = new EFUnitOfWorkHelper(_productUnitOfWork).GetNextSequence<Product>(p => true, p => p.Id);
            var product = _productUnitOfWork.ProductRepository.Add(new Product
                {
                    Id = nextId,
                    Name = parameters.ProductName,
                    ProductCode = parameters.ProductCode,
                    IsActive = true,
                    ProductType = productType
                });
            
            return new SuccessResult<Product>(product);
        }
    }
}