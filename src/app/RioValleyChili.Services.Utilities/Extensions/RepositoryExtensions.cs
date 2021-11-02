using System.Linq;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.Extensions
{
    public static class RepositoryExtensions
    {
        public static IQueryable<AdditiveLot> FilterByProductKey(this IRepository<AdditiveLot> lotRepository, IAdditiveProductKey productKey)
        {
            if(productKey == null)
            {
                return lotRepository.All();
            }
            return lotRepository.Filter(l => l.AdditiveProductId == productKey.AdditiveProductKey_Id);
        }
        
        public static IQueryable<Inventory> FilterAvailable(this IQueryable<Inventory> inventoryRepository)
        {
            return inventoryRepository.Where(i => i.Quantity > 0).Select(i => i);
        }

        public static IQueryable<Inventory> FilterAvailable(this IRepository<Inventory> inventoryRepository)
        {
            return inventoryRepository.All().FilterAvailable();
        }

        public static IQueryable<Inventory> FilterByLot(this IQueryable<Inventory> chileLots, ILotKey chileLotKey)
        {
            return chileLots.Where(c => c.LotDateCreated == chileLotKey.LotKey_DateCreated &&
                                        c.LotDateSequence == chileLotKey.LotKey_DateSequence &&
                                        c.LotTypeId == chileLotKey.LotKey_LotTypeId)
                            .Select(c => c);
        }


        public static IQueryable<ChileLot> FilterAvailable(this IQueryable<ChileLot> chileLots)
        {
            return chileLots.Where(LotPredicates.AvailableChileLotsFilter).Select(c => c);
        }

        public static IQueryable<ChileLot> FilterAvailable(this IRepository<ChileLot> chileLotRepository)
        {
            return chileLotRepository.All().FilterAvailable();
        }

        public static IQueryable<ChileLot> FilterByLot(this IQueryable<ChileLot> chileLots, ILotKey chileLotKey)
        {
            return chileLots.Where(c => c.LotDateCreated == chileLotKey.LotKey_DateCreated &&
                                        c.LotDateSequence == chileLotKey.LotKey_DateSequence &&
                                        c.LotTypeId == chileLotKey.LotKey_LotTypeId)
                            .Select(c => c);
        }


        public static IQueryable<PackagingLot> FilterAvailable(this IQueryable<PackagingLot> packagingLots)
        {
            return packagingLots.Where(LotPredicates.AvailablePackagingLotsFilter).Select(p => p);
        }

        public static IQueryable<PackagingLot> FilterAvailable(this IRepository<PackagingLot> packagingLotRepository)
        {
            return packagingLotRepository.All().FilterAvailable();
        }


        public static IQueryable<AdditiveLot> FilterAvailable(this IQueryable<AdditiveLot> additiveLots)
        {
            return additiveLots.Where(LotPredicates.AvailableAdditiveLotsFilter).Select(a => a);
        }

        public static IQueryable<AdditiveLot> FilterAvailable(this IRepository<AdditiveLot> additiveLotRepository)
        {
            return additiveLotRepository.All().FilterAvailable();
        }
    }
}