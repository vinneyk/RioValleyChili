using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class UpdateProductionSchedulePackSchedulesCommandParameters
    {
        public ProductionScheduleKey ProductionScheduleKey { get; private set; }

        public List<ProductionSchedulePackScheduleParameter> PackSchedules { get; set; }

        public UpdateProductionSchedulePackSchedulesCommandParameters(IProductionScheduleKey productionScheduleKey)
        {
            ProductionScheduleKey = new ProductionScheduleKey(productionScheduleKey);
        }
    }

    public class ProductionSchedulePackScheduleParameter
    {
        public PackScheduleKey PackScheduleKey { get; private set; }

        public int Order { get; set; }

        public ProductionSchedulePackScheduleParameter(IPackScheduleKey packScheduleKey)
        {
            PackScheduleKey = new PackScheduleKey(packScheduleKey);
        }
    }
}