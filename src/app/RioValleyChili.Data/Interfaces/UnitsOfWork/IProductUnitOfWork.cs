using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IProductUnitOfWork : ICoreUnitOfWork
    {
        IRepository<Product> ProductRepository { get; }

        IRepository<AdditiveProduct> AdditiveProductRepository { get; }

        IRepository<AdditiveType> AdditiveTypeRepository { get; }
            
        IRepository<ChileProduct> ChileProductRepository { get; }

        IRepository<ChileType> ChileTypeRepository { get; }

        IRepository<ChileProductAttributeRange> ChileProductAttributeRangeRepository { get; }

        IRepository<ChileProductIngredient> ChileProductIngredientRepository { get; }
            
        IRepository<AttributeName> AttributeNameRepository { get; }
            
        IRepository<PackagingProduct> PackagingProductRepository { get; }

        IRepository<ChileLot> ChileLotRepository { get; }

        IRepository<PackagingLot> PackagingLotRepository { get; }

        IRepository<AdditiveLot> AdditiveLotRepository { get; }
    }
}