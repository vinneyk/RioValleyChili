using System;

namespace RioValleyChili.Core
{
    public enum BatchTypeID
    {
        New = 1,
        Rework = 2,
        RePack = 3,
        ReLabel = 4
    }

    public static class BatchTypeIDHelper
    {
        public static BatchTypeID GetBatchTypeID(int batchTypeID)
        {
            switch(batchTypeID)
            {
                case 1: return BatchTypeID.New;
                case 2: return BatchTypeID.Rework;
                case 3: return BatchTypeID.RePack;
                case 4: return BatchTypeID.ReLabel;

                default: throw new ArgumentOutOfRangeException("batchTypeID");
            }
        }

        public static BatchTypeID GetBatchTypeID(string workTypeDescription)
        {
            if(!string.IsNullOrWhiteSpace(workTypeDescription))
            {
                workTypeDescription = workTypeDescription.ToUpper();
                switch(workTypeDescription)
                {
                    case "NEW": return BatchTypeID.New;
                    case "REWORK": return BatchTypeID.Rework;
                    case "REPACK": return BatchTypeID.RePack;
                    case "RELABEL": return BatchTypeID.ReLabel;
                }
            }

            throw new Exception(string.Format("Could not determined BatchTypeID from batchType[{0}].", workTypeDescription));
        }
    }
}