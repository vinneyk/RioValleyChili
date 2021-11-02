using System.Collections.Generic;
using NUnit.Framework;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.OldContextSynchronization.Utilities;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class GetLotStatHelperTests
    {
        [Test]
        public void Returns_false_and_no_LotStat_will_have_been_set_if_no_unresolved_ProductSpec_defects_exist()
        {
            Assert.IsNull(GetLotStatHelper.GetProductSpecDefectStat(new List<IAttributeDefect>()));
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_Granulation()
        {
            TestProductSpecDefect(LotStat.Granulation, StaticAttributeNames.Granularity);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_High_Water()
        {
            TestProductSpecDefect(LotStat.High_Water, StaticAttributeNames.H2O);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_Low_Water()
        {
            TestProductSpecDefect(LotStat.Low_Water, StaticAttributeNames.H2O, false);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_Scan()
        {
            TestProductSpecDefect(LotStat.Scan, StaticAttributeNames.Scan, false);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_Asta()
        {
            TestProductSpecDefect(LotStat.Asta, StaticAttributeNames.Asta, false);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_AB()
        {
            TestProductSpecDefect(LotStat.A_B, StaticAttributeNames.AB, false);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_Scoville()
        {
            TestProductSpecDefect(LotStat.Scov, StaticAttributeNames.Scoville, false);
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_unmatched_attribute()
        {
            TestProductSpecDefect(LotStat.See_Desc, StaticAttributeNames.Gluten, false, "Gluten too low");
            TestProductSpecDefect(LotStat.See_Desc, StaticAttributeNames.Yeast, true, "Yeast too high");
        }

        [Test]
        public void Returns_true_and_LotStat_would_have_been_set_as_expected_for_defect_with_highest_relative_deviation()
        {
            Assert.AreEqual(LotStat.Low_Water, GetLotStatHelper.GetProductSpecDefectStat(new[]
                {
                    BuildProductSpecDefect(StaticAttributeNames.Scan, 10000, 0, 0, false),
                    BuildProductSpecDefect(StaticAttributeNames.H2O, 1, 2, 3, false),
                    BuildProductSpecDefect(StaticAttributeNames.Asta, 100, 98, 99, false)
                }).LotStat);
        }

        private static void TestProductSpecDefect(LotStat lotStat, AttributeName attributeName, bool tooHigh = true, string description = null)
        {
            var stat = GetLotStatHelper.GetProductSpecDefectStat(new[]
                {
                    BuildProductSpecDefect(attributeName, tooHigh ? 4 : 1, 2, 3, false)
                });
            Assert.AreEqual(lotStat, stat.LotStat);
            Assert.AreEqual(description, stat.Description);
        }

        public static IAttributeDefect BuildProductSpecDefect(IAttributeNameKey nameKey, double value, double min, double max, bool resolution)
        {
            return new AttributeDefect
                {
                    HasResolution = resolution,
                    AttributeNameKey_ShortName = nameKey.AttributeNameKey_ShortName,
                    Value = value,
                    RangeMin = min,
                    RangeMax = max,
                    DefectType = DefectTypeEnum.ProductSpec
                };
        }

        private class AttributeDefect : IAttributeDefect
        {
            public string AttributeNameKey_ShortName { get; set; }
            public double RangeMin { get; set; }
            public double RangeMax { get; set; }
            public double Value { get; set; }
            public DefectTypeEnum DefectType { get; set; }
            public bool HasResolution { get; set; }
        }
    }
}