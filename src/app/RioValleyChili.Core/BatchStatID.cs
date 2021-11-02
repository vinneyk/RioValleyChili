using System;

namespace RioValleyChili.Core
{
    public enum BatchStatID
    {
        Scheduled = 1,
        Picked = 2,
        Produced = 3
    }

    public static class BatchStatIDHelper
    {
        public static BatchStatID? GetBatchStatID(int? batchStatID)
        {
            if(batchStatID == null)
            {
                return null;
            }

            switch(batchStatID)
            {
                case 1: return BatchStatID.Scheduled;
                case 2: return BatchStatID.Picked;
                case 3: return BatchStatID.Produced;

                default: throw new ArgumentOutOfRangeException("batchStatID");
            }
        }

        public static int? GetBatchStatID(BatchStatID? batchStatID)
        {
            if(batchStatID == null)
            {
                return null;
            }

            return (int) batchStatID.Value;
        }
    }
}