using System;
using System.Globalization;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Models.Helpers;
using Solutionhead.EntityKey;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class ProductionScheduleKey : EntityKey<IProductionScheduleKey>.With<DateTime, int>, IKey<ProductionSchedule>, IProductionScheduleKey
    {
        public ProductionScheduleKey() : this(DataConstants.SqlMinDate, 0) { }

        public ProductionScheduleKey(IProductionScheduleKey productionScheduleKey)
            : this(productionScheduleKey.ProductionScheduleKey_ProductionDate, productionScheduleKey.LocationKey_Id) { }

        private ProductionScheduleKey(DateTime productionDate, int lineLocationId)
        {
            Field0 = productionDate;
            Field1 = lineLocationId;
        }

        protected override IProductionScheduleKey ConstructKey(DateTime field0, int field1)
        {
            return new ProductionScheduleKey(field0, field1);
        }

        protected override With<DateTime, int> DeconstructKey(IProductionScheduleKey key)
        {
            return new ProductionScheduleKey(key.ProductionScheduleKey_ProductionDate, key.LocationKey_Id);
        }

        protected override string DateTimeToString(DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        protected override bool TryParseDateTime(string s, out object result)
        {
            DateTime dateTime;
            var tryParse = DateTime.TryParseExact(s, "yyyyMMdd", new DateTimeFormatInfo(), DateTimeStyles.None, out dateTime);
            result = dateTime;
            return tryParse;
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidProductionScheduleKey, inputValue);
        }

        public Expression<Func<ProductionSchedule, bool>> FindByPredicate { get { return p => p.ProductionDate == Field0 && p.ProductionLineLocationId == Field1; } }
        public DateTime ProductionScheduleKey_ProductionDate { get { return Field0; } }
        public int LocationKey_Id { get { return Field1; } }
    }

    public static class ProductionScheduleKeyExtensions
    {
        public static ProductionScheduleKey ToProductionScheduleKey(this IProductionScheduleKey k)
        {
            return new ProductionScheduleKey(k);
        }
    }
}