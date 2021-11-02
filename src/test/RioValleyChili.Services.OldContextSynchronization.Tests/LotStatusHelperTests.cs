using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class LotStatusHelperTests
    {
        [TestFixture]
        public class LotHasUnresolvedDefects : LotSyncHelperTests
        {
            [Test]
            public void Returns_true_if_Lot_contains_BacterialContamination_defects_without_resolutions()
            {
                //Arrange
                var lot = new Lot
                {
                    LotDefects = new[]
                            {
                                BuildLotDefect(DefectTypeEnum.BacterialContamination, false),
                                BuildLotDefect(DefectTypeEnum.BacterialContamination, true)
                            }
                };

                //Act-Assert
                Assert.IsTrue(LotStatusHelper.LotHasUnresolvedDefects(lot, DefectTypeEnum.BacterialContamination));
            }

            [Test]
            public void Returns_false_if_Lot_contains_no_BacterialContamination_defects_without_resolutions()
            {
                //Arrange
                var lotWithNoDefects = new Lot
                {
                    LotDefects = new LotDefect[0]
                };

                var lotWithResolvedDefect = new Lot
                {
                    LotDefects = new[]
                            {
                                BuildLotDefect(DefectTypeEnum.BacterialContamination, true)
                            }
                };

                //Act-Assert
                Assert.IsFalse(LotStatusHelper.LotHasUnresolvedDefects(lotWithNoDefects, DefectTypeEnum.BacterialContamination));
                Assert.IsFalse(LotStatusHelper.LotHasUnresolvedDefects(lotWithResolvedDefect, DefectTypeEnum.BacterialContamination));
            }
        }

        [TestFixture]
        public class ChileLotAllProductSpecsEntered : LotSyncHelperTests
        {
            [Test]
            public void Returns_true_if_ChileLot_contains_LotAttributes_for_all_ChileProductAttributeRanges()
            {
                Assert.IsTrue(LotStatusHelper.ChileLotAllProductSpecsEntered(new ChileLot
                    {
                        Lot = new Lot
                            {
                                Attributes = BuildLotAttributes(StaticAttributeNames.Asta, StaticAttributeNames.Scan, StaticAttributeNames.Scoville)

                            },
                        ChileProduct = new ChileProduct
                            {
                                ProductAttributeRanges = BuildAttributeRanges(StaticAttributeNames.Asta, StaticAttributeNames.Scan, StaticAttributeNames.Scoville)
                            }
                    }));
            }

            [Test]
            public void Returns_false_if_ChileLot_is_missing_any_LotAttribute_for_a_ChileProductRange()
            {
                Assert.IsFalse(LotStatusHelper.ChileLotAllProductSpecsEntered(new ChileLot
                    {
                        Lot = new Lot
                            {
                                Attributes = BuildLotAttributes(StaticAttributeNames.Asta, StaticAttributeNames.Scoville)
                            },
                        ChileProduct = new ChileProduct
                            {
                                ProductAttributeRanges = BuildAttributeRanges(StaticAttributeNames.Asta, StaticAttributeNames.Scan, StaticAttributeNames.Scoville)
                            }
                    }));
            }

            private static ICollection<LotAttribute> BuildLotAttributes(params AttributeName[] attributes)
            {
                return attributes.Select(a => new LotAttribute
                    {
                        AttributeShortName = a.ShortName,
                        AttributeName = a
                    }).ToArray();
            }

            private static ICollection<ChileProductAttributeRange> BuildAttributeRanges(params AttributeName[] attributes)
            {
                return attributes.Select(a => new ChileProductAttributeRange
                    {
                        AttributeShortName = a.ShortName,
                        AttributeName = a
                    }).ToArray();
            }
        }
    }
}