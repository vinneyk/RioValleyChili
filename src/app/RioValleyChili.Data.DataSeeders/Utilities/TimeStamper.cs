using System;
using Solutionhead.Core;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    static class TimeStampHelper
    {
        internal static ITimeStamper Current
        {
            get
            {
                return new TimeStamper();
            }
        }
    }

    class TimeStamper : ITimeStamper
    {
        public DateTime CurrentTimeStamp
        {
            get { return DateTime.UtcNow; }
        }
    }
}
