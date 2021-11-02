using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models.KeyReturns
{
    internal class ProductionScheduleKeyReturn : IProductionScheduleKey
    {
        public DateTime ProductionScheduleKey_ProductionDate { get; internal set; }
        public int LocationKey_Id { get; internal set; }

        internal string ProductionScheduleKey { get { return new ProductionScheduleKey(this); } }
    }
}