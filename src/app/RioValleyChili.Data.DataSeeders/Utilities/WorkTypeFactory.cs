using System;
using RioValleyChili.Data.DataSeeders.Mothers.PocoMothers;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    class WorkTypeFactory
    {
        internal static WorkType BuildWorkTypeFromBatchTypeID(int batchTypeID)
        {
            switch(batchTypeID)
            {
                case 1: return WorkTypeMother.NewProduct;
                case 2: return WorkTypeMother.Rework;
                case 3: return WorkTypeMother.Repack;
                case 4: return WorkTypeMother.Relabel;

                default: throw new InvalidOperationException(string.Format("'{0}' is not a valid batch type ID.", batchTypeID));
            }
        }
    }
}
