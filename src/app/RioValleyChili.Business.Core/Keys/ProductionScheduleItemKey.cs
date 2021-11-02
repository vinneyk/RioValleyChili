using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Helpers;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class ProductionScheduleItemKey : EntityKey<IProductionScheduleItemKey>.With<DateTime, int, int>, IKey<ProductionScheduleItem>, IProductionScheduleItemKey
    {
        public ProductionScheduleItemKey() : base(DataConstants.SqlMinDate, 0, 0) { }

        public ProductionScheduleItemKey(IProductionScheduleItemKey k) : base(k.ProductionScheduleKey_ProductionDate, k.LocationKey_Id, k.ProductionScheduleItemKey_Index) { }

        protected override IProductionScheduleItemKey ConstructKey(DateTime field0, int field1, int field2)
        {
            return new ProductionScheduleItemKey
                {
                    Field0 = field0,
                    Field1 = field1,
                    Field2 = field2
                };
        }

        protected override With<DateTime, int, int> DeconstructKey(IProductionScheduleItemKey key)
        {
            return new ProductionScheduleItemKey
                {
                    Field0 = key.ProductionScheduleKey_ProductionDate,
                    Field1 = key.LocationKey_Id,
                    Field2 = key.ProductionScheduleItemKey_Index
                };
        }

        protected override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidProductionScheduleItemKey, inputValue);
        }

        public Expression<Func<ProductionScheduleItem, bool>> FindByPredicate { get { return i => i.ProductionDate == Field0 && i.ProductionLineLocationId == Field1 && i.Index == Field2; } }

        public DateTime ProductionScheduleKey_ProductionDate { get { return Field0; } }
        public int LocationKey_Id { get { return Field1; } }
        public int ProductionScheduleItemKey_Index { get { return Field2; } }
    }

    public static class ProductionScheduleItemKeyExtensions
    {
        public static ProductionScheduleItemKey ToProductionScheduleItemKey(this IProductionScheduleItemKey k)
        {
            return new ProductionScheduleItemKey(k);
        }
    }
}