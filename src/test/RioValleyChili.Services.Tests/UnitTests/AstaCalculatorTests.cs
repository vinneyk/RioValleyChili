using System;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;

namespace RioValleyChili.Services.Tests.UnitTests
{
    [TestFixture]
    public class AstaCalculatorTests
    {
        private static readonly DateTime startingDate = new DateTime(1985, 3, 29);
        private const double EPSILON = 0.0000000005;

        [Test]
        public void CalculateRatio_values_are_as_expected()
        {
            TestRatio(0, 1.0);
            TestRatio(1, 0.9995);
            TestRatio(2, 0.99900025);
            TestRatio(5, 0.997502499);
            TestRatio(10, 0.995011235);
            TestRatio(20, 0.990047358);
            TestRatio(50, 0.975303814);
            TestRatio(100, 0.95121753);
            TestRatio(200, 0.90481479);
            TestRatio(500, 0.778752093);
            TestRatio(1000, 0.606454823);
            TestRatio(2000, 0.367787452);
            TestRatio(5000, 0.082033694);
        }

        [Test]
        public void CalculateAsta_values_are_as_expected()
        {
            TestAsta(0, 120.0, 120);
            TestAsta(1, 120.0, 120);
            TestAsta(10, 120.0, 119);
            TestAsta(50, 120.0, 117);
            TestAsta(100, 120.0, 114);
            TestAsta(200, 120.0, 109);
            TestAsta(1000, 120.0, 73);
            TestAsta(5000, 120.0, 10);
        }

        [Test]
        public void CalculateAsta_value_is_as_expected_when_value_would_have_been_entered_several_days_after_production()
        {
            var productionDate = new DateTime(2012, 3, 29);

            const int testedAsta = 73;
            var testedDate = productionDate.AddDays(1000);

            const int expectedCurrentAsta = 10;
            var currentDate = productionDate.AddDays(5000);

            Assert.AreEqual(expectedCurrentAsta, AstaCalculator.CalculateAsta(testedAsta, testedDate, productionDate, currentDate));
        }

        private void TestRatio(int daysElapsed, double expectedValue)
        {
            var result = AstaCalculator.CalculateRatio(startingDate, startingDate.AddDays(daysElapsed));
            var absoluteDifference = Math.Abs(expectedValue - result);
            if(absoluteDifference > EPSILON)
            {
                Assert.Fail("Expected '{0}' but received '{1}' on '{2}' days elapsed. Difference of '{3}' is greater than tolerance of '{4}'.", expectedValue, result, daysElapsed, absoluteDifference, EPSILON);
            }
        }

        private void TestAsta(int daysElapsed, double originalAsta, int expectedAsta)
        {
            var result = AstaCalculator.CalculateAsta(originalAsta, startingDate, startingDate, startingDate.AddDays(daysElapsed));
            if(result != expectedAsta)
            {
                Assert.Fail("Expected '{0}' but received '{1}' on originalAsta of '{2}' after '{3}' daysElapsed.", expectedAsta, result, originalAsta, daysElapsed);
            }
        }
    }
}
