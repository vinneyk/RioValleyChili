using System;
using NUnit.Framework;
using RioValleyChili.Core.Extensions;

namespace RioValleyChili.Services.Tests.Helpers
{
    public static class DateTimeExtensions
    {
        private const int MillisecondEpsilon = 5;

        public static void AssertUTCSameAsMST(this DateTime utc, DateTime mst)
        {
            var difference = utc - mst.ConvertLocalToUTC();
            Assert.Less(Math.Abs(difference.Milliseconds), MillisecondEpsilon);
        }
    }
}