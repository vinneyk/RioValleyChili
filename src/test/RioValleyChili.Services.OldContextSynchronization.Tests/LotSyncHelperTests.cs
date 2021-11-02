using System;
using System.Collections.Generic;
using NUnit.Framework;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.OldContextSynchronization.Utilities;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class LotSyncHelperTests
    {
        [TestFixture]
        public class SetTestDateTests : LotSyncHelperTests
        {
            [Test]
            public void SetsTestDateFromMaxDateOfAttributes()
            {
                // arrange
                var lot = new tblLot();
                var minDate = DateTime.Now.AddDays(-5).Date;
                var expectedDate = DateTime.Now.Date;
                var asta = BuildLotAttribute(StaticAttributeNames.Asta, 1.0, minDate);
                var scoville = BuildLotAttribute(StaticAttributeNames.Scoville, 2.0f, minDate);
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0, expectedDate);
                var newLot = new Lot
                {
                    Attributes = new[] { asta, scoville, scan }
                };

                // act
                LotSyncHelper.SetTestData(lot, newLot);

                // assert
                Assert.True(lot.TestDate.HasValue);
                Assert.AreEqual(expectedDate, lot.TestDate);
            }

            [Test]
            public void IfAllAttributeTestDatesAreMinDateValue_TestDateIsNull()
            {
                // arrange
                var lot = new tblLot();
                var asta = BuildLotAttribute(StaticAttributeNames.Asta, 1.0, DateTime.MinValue);
                var scoville = BuildLotAttribute(StaticAttributeNames.Scoville, 2.0f, DateTime.MinValue);
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0, DateTime.MinValue);
                var newLot = new Lot
                {
                    Attributes = new[] { asta, scoville, scan }
                };

                // act
                LotSyncHelper.SetTestData(lot, newLot);

                // assert
                Assert.AreEqual(null, lot.TestDate);
            }

            [Test]
            public void SetsTesterIdFromLatestAttributeEmployeeId()
            {
                // arrange
                var lot = new tblLot();
                var asta = BuildLotAttribute(StaticAttributeNames.Asta, 1.0, DateTime.MinValue);
                var scoville = BuildLotAttribute(StaticAttributeNames.Scoville, 2.0f, new DateTime(2015, 12, 12));
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0, new DateTime(2016, 1, 1), employee: new EmployeeKeyReturn { EmployeeKey_Id = 123 });
                var newLot = new Lot
                {
                    Attributes = new[] { asta, scoville, scan }
                };

                // act
                LotSyncHelper.SetTestData(lot, newLot);

                // assert
                Assert.AreEqual(123, lot.TesterID);
            }

            [Test]
            public void TesterIdWillBeNullIfNoActualAttributesExist()
            {
                // arrange
                var lot = new tblLot();
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0, new DateTime(2016, 1, 1), true, new EmployeeKeyReturn { EmployeeKey_Id = 123 });
                var newLot = new Lot
                {
                    Attributes = new[] { scan }
                };

                // act
                LotSyncHelper.SetTestData(lot, newLot);

                // assert
                Assert.AreEqual(null, lot.TesterID);
            }
        }

        [TestFixture]
        public class SetLotAttributesTests : LotSyncHelperTests
        {
            [Test]
            public void Sets_lot_attribute_values_as_expected()
            {
                //Arrange
                var lot = new tblLot();
                var asta = BuildLotAttribute(StaticAttributeNames.Asta, 1.0);
                var scoville = BuildLotAttribute(StaticAttributeNames.Scoville, 2.0f);
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0);

                //Act
                LotSyncHelper.SetLotAttributes(lot, new List<LotAttribute>
                    {
                        asta, scoville, scan
                    });

                //Assert
                Assert.AreEqual(asta.AttributeValue, lot.AvgAsta);
                Assert.AreEqual(scoville.AttributeValue, lot.AvgScov);
                Assert.AreEqual(scan.AttributeValue, lot.Scan);
            }

            [Test]
            public void Clears_missing_attribute_values()
            {
                //Arrange
                var lot = new tblLot
                    {
                        Ash = 11,
                        AToxin = 22,
                        Gluten = 33
                    };
                var asta = BuildLotAttribute(StaticAttributeNames.Asta, 1.0);
                var scoville = BuildLotAttribute(StaticAttributeNames.Scoville, 2.0f);
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0);

                //Act
                LotSyncHelper.SetLotAttributes(lot, new List<LotAttribute>
                    {
                        asta, scoville, scan
                    });

                //Assert
                Assert.IsNotNull(lot.AvgAsta);
                Assert.IsNotNull(lot.AvgScov);
                Assert.IsNotNull(lot.Scan);
                Assert.IsNull(lot.Ash);
                Assert.IsNull(lot.AToxin);
                Assert.IsNull(lot.Gluten);
            }

            [Test]
            public void Does_not_override_lot_attributes_with_computed_values()
            {
                //Arrange
                var lot = new tblLot
                    {
                        AvgAsta = 10
                    };
                var asta = BuildLotAttribute(StaticAttributeNames.Asta, 1.0, null, true);
                var scoville = BuildLotAttribute(StaticAttributeNames.Scoville, 2.0f);
                var scan = BuildLotAttribute(StaticAttributeNames.Scan, 3.0);

                //Act
                LotSyncHelper.SetLotAttributes(lot, new List<LotAttribute>
                    {
                        asta, scoville, scan
                    });

                //Assert
                Assert.AreEqual(scoville.AttributeValue, lot.AvgScov);
                Assert.AreEqual(scan.AttributeValue, lot.Scan);
                Assert.AreEqual(10, lot.AvgAsta);
            }
        }

        [TestFixture]
        public class SetProductSpecDefectTests : LotSyncHelperTests
        {
            [Test]
            public void Returns_false_and_LotStat_will_not_have_been_set_if_no_LotAttributes_or_defects_exist()
            {
                var oldLot = new tblLot();
                Assert.IsFalse(LotSyncHelper.SetProductSpecDefectStat(oldLot, null, null, null));
                Assert.IsNull(oldLot.LotStat);
            }

            [Test]
            public void Returns_false_and_LotStat_will_not_have_been_set_if_no_defects_of_ProductSpec_type_exist()
            {
                var oldLot = new tblLot();
                Assert.IsFalse(LotSyncHelper.SetProductSpecDefectStat(oldLot, null, null, new List<IAttributeDefect>
                    {
                        new LotAttributeDefect
                            {
                                LotDefect = new LotDefect { DefectType = DefectTypeEnum.BacterialContamination },
                            }
                    }));
                Assert.IsNull(oldLot.LotStat);
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_as_expected_for_ProductSpec_defect()
            {
                var oldLot = new tblLot();
                Assert.IsTrue(LotSyncHelper.SetProductSpecDefectStat(oldLot, null, null, new List<IAttributeDefect>
                    {
                        GetLotStatHelperTests.BuildProductSpecDefect(StaticAttributeNames.H2O, 1, 2, 3, false)
                    }));
                Assert.AreEqual((int)LotStat.Low_Water, oldLot.LotStat);
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_if_attribute_is_out_of_range_of_a_customer_spec()
            {
                var oldLot = new tblLot();
                Assert.IsTrue(LotSyncHelper.SetProductSpecDefectStat(oldLot, new List<LotAttribute>
                    {
                        new LotAttribute
                            {
                                AttributeValue = 1,
                                AttributeShortName = StaticAttributeNames.H2O.ShortName
                            }
                    },
                    new List<CustomerProductAttributeRange>
                        {
                            new CustomerProductAttributeRange
                                {
                                    RangeMin = 2,
                                    RangeMax = 3,
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName
                                }
                        }, null));
                Assert.AreEqual((int)LotStat.Low_Water, oldLot.LotStat);
            }

            [Test, Issue("Should handle an attribute having multiple defects. -RI 8/1/2016",
                References = new[] { "RVCADMIN-1203" })]
            public void Returns_true_and_LotStat_will_have_been_set_given_multiple_unresolved_defects_for_an_attribute()
            {
                var oldLot = new tblLot();
                Assert.IsTrue(LotSyncHelper.SetProductSpecDefectStat(oldLot, new List<LotAttribute>
                    {
                        new LotAttribute
                            {
                                AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                AttributeValue = 1
                            },
                    },
                    new List<CustomerProductAttributeRange>
                        {
                            new CustomerProductAttributeRange
                                {
                                    RangeMin = 2,
                                    RangeMax = 3,
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName
                                }
                        },
                    new List<IAttributeDefect>
                        {
                            new LotAttributeDefect
                                {
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                    LotDefect = new LotDefect
                                        {
                                            DefectType = DefectTypeEnum.ProductSpec
                                        }
                                },
                            new LotAttributeDefect
                                {
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                    LotDefect = new LotDefect
                                        {
                                            DefectType = DefectTypeEnum.ProductSpec
                                        }
                                },
                        }));
                Assert.AreEqual((int)LotStat.Low_Water, oldLot.LotStat);
            }

            [Test, Issue("Should handle an attribute having multiple defects. -RI 8/1/2016",
                References = new[] { "RVCADMIN-1203" })]
            public void Returns_true_and_LotStat_will_have_been_set_if_a_single_unresolved_defect_exists_for_an_attribute()
            {
                var oldLot = new tblLot();
                Assert.IsTrue(LotSyncHelper.SetProductSpecDefectStat(oldLot, new List<LotAttribute>
                    {
                        new LotAttribute
                            {
                                AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                AttributeValue = 1
                            },
                    },
                    new List<CustomerProductAttributeRange>
                        {
                            new CustomerProductAttributeRange
                                {
                                    RangeMin = 2,
                                    RangeMax = 3,
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName
                                }
                        },
                    new List<IAttributeDefect>
                        {
                            new LotAttributeDefect
                                {
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                    LotDefect = new LotDefect
                                        {
                                            DefectType = DefectTypeEnum.ProductSpec,
                                            Resolution = new LotDefectResolution()
                                        }
                                },
                            new LotAttributeDefect
                                {
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                    LotDefect = new LotDefect
                                        {
                                            DefectType = DefectTypeEnum.ProductSpec
                                        }
                                },
                        }));
                Assert.AreEqual((int)LotStat.Low_Water, oldLot.LotStat);
            }

            [Test, Issue("Should handle an attribute having multiple defects. -RI 8/1/2016",
                References = new[] {"RVCADMIN-1203"})]
            public void Returns_false_and_LotStat_will_not_have_been_set_if_all_defects_for_an_attribute_are_resolved()
            {
                var oldLot = new tblLot();
                Assert.False(LotSyncHelper.SetProductSpecDefectStat(oldLot, new List<LotAttribute>
                    {
                        new LotAttribute
                            {
                                AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                AttributeValue = 1
                            },
                    },
                    new List<CustomerProductAttributeRange>
                        {
                            new CustomerProductAttributeRange
                                {
                                    RangeMin = 2,
                                    RangeMax = 3,
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName
                                }
                        },
                    new List<IAttributeDefect>
                        {
                            new LotAttributeDefect
                                {
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                    LotDefect = new LotDefect
                                        {
                                            DefectType = DefectTypeEnum.ProductSpec,
                                            Resolution = new LotDefectResolution()
                                        }
                                },
                            new LotAttributeDefect
                                {
                                    AttributeShortName = StaticAttributeNames.H2O.ShortName,
                                    LotDefect = new LotDefect
                                        {
                                            DefectType = DefectTypeEnum.ProductSpec,
                                            Resolution = new LotDefectResolution()
                                        }
                                },
                        }));
                Assert.IsNull(oldLot.LotStat);
            }
        }

        [TestFixture]
        public class SetInHouseContaminationTests : LotSyncHelperTests
        {
            [Test]
            public void Returns_false_and_no_LotStat_will_have_been_set_it_no_unresolved_InHouseContamination_defects_exist()
            {
                //Arrange
                var lotWithNoDefects = new Lot
                    {
                        LotDefects = new LotDefect[0]
                    };
                var lotWithResolvedDefects = new Lot
                    {
                        LotDefects = new[]
                            {
                                BuildLotDefect(DefectTypeEnum.InHouseContamination, true),
                                BuildLotDefect(DefectTypeEnum.InHouseContamination, true)
                            }
                    };
                var oldLot = new tblLot
                    {
                        LotStat = null
                    };

                //Act-Assert
                Assert.IsFalse(LotSyncHelper.SetInHouseContamination(oldLot, lotWithNoDefects));
                Assert.IsNull(oldLot.LotStat);

                Assert.IsFalse(LotSyncHelper.SetInHouseContamination(oldLot, lotWithResolvedDefects));
                Assert.IsNull(oldLot.LotStat);
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_as_expected_for_description_containing_Dark_Specs()
            {
                TestInHouseContamination(LotStat.Dark_Specs, "DARKSPEC");
                TestInHouseContamination(LotStat.Dark_Specs, "DaRk SpEC");
                TestInHouseContamination(LotStat.Dark_Specs, "This lot contains dark things called 'specs'");
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_as_expected_for_description_containing_Hard_BB()
            {
                TestInHouseContamination(LotStat.Hard_BBs, "hardbb");
                TestInHouseContamination(LotStat.Hard_BBs, "HarD BBs");
                TestInHouseContamination(LotStat.Hard_BBs, "We found HARD things; BB-like things");
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_as_expected_for_description_containing_Soft_BB()
            {
                TestInHouseContamination(LotStat.Soft_BBs, "SOFTBB");
                TestInHouseContamination(LotStat.Soft_BBs, "sOft Bbs");
                TestInHouseContamination(LotStat.Soft_BBs, "Oh the softness of BBs!");
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_as_expected_for_description_containing_Smoke_Cont()
            {
                TestInHouseContamination(LotStat.Smoke_Cont, "SMOKECONT");
                TestInHouseContamination(LotStat.Smoke_Cont, "SmOkE cOnT");
                TestInHouseContamination(LotStat.Smoke_Cont, "there was smoke in the contents or something");
            }

            [Test]
            public void Returns_true_and_LotStat_will_have_been_set_with_Notes_as_expected_for_an_unmatched_description()
            {
                tblLot oldLot;
                Lot newLot;

                const string description = "dark hard stuff with soft smoke";
                ArrangeLots(description, out oldLot, out  newLot);
                Assert.IsTrue(LotSyncHelper.SetInHouseContamination(oldLot, newLot));
                Assert.AreEqual((int)LotStat.See_Desc, oldLot.LotStat);
                Assert.AreEqual(description, oldLot.Notes);
            }

            private static void TestInHouseContamination(LotStat expected, string description)
            {
                tblLot oldLot;
                Lot newLot;

                ArrangeLots(description, out oldLot, out  newLot);
                Assert.IsTrue(LotSyncHelper.SetInHouseContamination(oldLot, newLot));
                Assert.AreEqual((int)expected, oldLot.LotStat);
            }

            private static void ArrangeLots(string description, out tblLot oldLot, out Lot newLot)
            {
                newLot = new Lot
                    {
                        LotDefects = new[]
                                {
                                    BuildLotDefect(DefectTypeEnum.InHouseContamination, false, description)
                                }
                    };

                oldLot = new tblLot
                    {
                        LotStat = null
                    };
            }
        }

        protected static LotAttribute BuildLotAttribute(AttributeName attributeName, double value, DateTime? testDate = null, bool computed = false, IEmployeeKey employee = null)
        {
            return new LotAttribute
                {
                    AttributeName = attributeName,
                    AttributeShortName = attributeName.AttributeNameKey_ShortName,
                    AttributeValue = value,
                    AttributeDate = testDate ?? DateTime.MinValue,
                    TimeStamp = testDate ?? DateTime.MinValue,
                    Computed = computed,
                    EmployeeId = employee == null ? 0 : employee.EmployeeKey_Id
                };
        }

        protected static LotDefect BuildLotDefect(DefectTypeEnum defectType, bool withResolution, string description = null)
        {
            return new LotDefect
                {
                    DefectType = defectType,
                    Description = description,
                    Resolution = withResolution ? new LotDefectResolution() : null
                };
        }
    }
}