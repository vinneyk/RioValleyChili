using System.Linq;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class PackScheduleProductionLineHelper
    {
        public enum ProductionLineResult
        {
            CouldNotDetermine,
            FromPackSchedule,
            DeterminedFromResultingLots,
            DeterminedFromDescription,
            DeterminedFromBatchType
        }

        public static ProductionLineResult DetermineProductionLine(PackScheduleEntityObjectMother.PackScheduleDTO packSchedule, out int? line)
        {
            line = null;
            if(packSchedule.ProductionLine != null)
            {
                line = packSchedule.ProductionLine;
                return ProductionLineResult.FromPackSchedule;
            }

            var lines = packSchedule.BatchLots.Where(b => b.ProductionLine != null).Select(b => b.ProductionLine).Distinct().ToList();
            if(lines.Count == 1)
            {
                line = lines.Single();
                return ProductionLineResult.DeterminedFromResultingLots;
            }

            if(packSchedule.PackSchDesc != null)
            {
                if(packSchedule.PackSchDesc.ToUpper().Contains("LINE #3"))
                {
                    line = 3;
                    return ProductionLineResult.DeterminedFromDescription;
                }
            }

            switch(packSchedule.BatchTypeID)
            {
                case 2:
                    line = 3;
                    return ProductionLineResult.DeterminedFromBatchType;

                case 3:
                    line = 4;
                    return ProductionLineResult.DeterminedFromBatchType;

                case 4:
                    line = 5;
                    return ProductionLineResult.DeterminedFromBatchType;

                default: return ProductionLineResult.CouldNotDetermine;
            }
        }
    }
}