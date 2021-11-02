using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataTests
{
    [TestFixture]
    public class CreateChileLotHelperTests
    {
        [TestFixture]
        public class LotContainsSerializedDefects : CreateChileLotHelperTests
        {
            [Test]
            public void InHouseContamination_defects_are_preserved()
            {
                //Arrange
                var chileProduct = CreateChileProduct();
                var openDefect = new Defect(DefectTypeEnum.InHouseContamination, "test");
                var closedDefect = new Defect(DefectTypeEnum.InHouseContamination, "test2", true);

                List<LotAttributeDefect> lotAttributeDefects;
                var lot = CreateLot(out lotAttributeDefects, new Defects
                    {
                        openDefect,
                        closedDefect
                    });

                var tblLot = CreateTblLot(lot, lotAttributeDefects);

                //Act
                List<LotAttributeDefect> attributeDefects;
                CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                //Assert
                openDefect.AssertExpected(lot.LotDefects.Single(d => d.DefectType == DefectTypeEnum.InHouseContamination && d.Resolution == null));
                closedDefect.AssertExpected(lot.LotDefects.Single(d => d.DefectType == DefectTypeEnum.InHouseContamination && d.Resolution != null));
            }

            [Test]
            public void Removing_an_attribute_from_old_context_will_close_open_defects_and_remove_attribute_from_Lot()
            {
                var chileProduct = CreateChileProduct(new AttributeRanges
                    {
                        { StaticAttributeNames.Asta, 0, 1 },
                        { StaticAttributeNames.Scan, 0, 1 }
                    });
                var astaDefect = new Defect(StaticAttributeNames.Asta, 2, 0, 1);
                var scanDefect = new Defect(StaticAttributeNames.Scan, 3, 0, 1);

                List<LotAttributeDefect> lotAttributeDefects;
                var lot = CreateLot(out lotAttributeDefects,
                    new Defects
                        {
                            astaDefect,
                            scanDefect
                        },
                    new Attributes
                        {
                            { StaticAttributeNames.Asta, 2 },
                            { StaticAttributeNames.Scan, 3 }
                        });

                var tblLot = CreateTblLot(lot, lotAttributeDefects);

                //Act
                List<LotAttributeDefect> attributeDefects;
                var chileLot = CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                //Assert
                astaDefect.Resolution = scanDefect.Resolution = true;
                astaDefect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                scanDefect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Scan.ShortName));
                Assert.IsEmpty(chileLot.Lot.Attributes);
            }

            [Test]
            public void Removing_an_attribute_from_old_context_will_preserve_closed_defects_and_remove_attribute_from_Lot()
            {
                var chileProduct = CreateChileProduct(new AttributeRanges
                    {
                        { StaticAttributeNames.Asta, 0, 1 },
                        { StaticAttributeNames.Scan, 0, 1 }
                    });
                var astaDefect = new Defect(StaticAttributeNames.Asta, 2, 0, 1, true);
                var scanDefect = new Defect(StaticAttributeNames.Scan, 3, 0, 1, true);

                List<LotAttributeDefect> lotAttributeDefects;
                var lot = CreateLot(out lotAttributeDefects,
                    new Defects
                        {
                            astaDefect,
                            scanDefect
                        },
                    new Attributes
                        {
                            { StaticAttributeNames.Asta, 2 },
                            { StaticAttributeNames.Scan, 3 }
                        });

                var tblLot = CreateTblLot(lot, lotAttributeDefects);

                //Act
                List<LotAttributeDefect> attributeDefects;
                var chileLot = CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                //Assert
                astaDefect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                scanDefect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Scan.ShortName));
                Assert.IsEmpty(chileLot.Lot.Attributes);
            }

            [TestFixture]
            public class LotAttributeDefects : LotContainsSerializedDefects
            {
                [Test]
                public void Defect_is_as_expected_when_old_context_attribute_value_is_equal_to_deserialized_attribute_value()
                {
                    //Arrange
                    var chileProduct = CreateChileProduct(new AttributeRanges
                        {
                            { StaticAttributeNames.Asta, 0, 1}
                        });
                    var defect = new Defect(StaticAttributeNames.Asta, 2, 0, 1);

                    List<LotAttributeDefect> lotAttributeDefects;
                    var lot = CreateLot(out lotAttributeDefects,
                        new Defects
                            {
                                defect
                            },
                        new Attributes
                            {
                                { StaticAttributeNames.Asta, 2 }
                            });

                    var tblLot = CreateTblLot(lot, lotAttributeDefects, new Attributes
                        {
                            { StaticAttributeNames.Asta, 2 }
                        });

                    //Act
                    List<LotAttributeDefect> attributeDefects;
                    CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                    //Assert
                    defect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                }

                [Test]
                public void New_LotAttributeDefects_are_created_if_old_context_values_are_out_of_range()
                {
                    //Arrange
                    var expectedDefect = new Defect(StaticAttributeNames.Asta, 3, 0, 1);
                    var chileProduct = CreateChileProduct(new AttributeRanges
                        {
                            { StaticAttributeNames.Asta, 0, 1}
                        });

                    List<LotAttributeDefect> lotAttributeDefects;
                    var lot = CreateLot(out lotAttributeDefects);

                    var newAttribute = new Attribute(StaticAttributeNames.Asta, 3);
                    var tblLot = CreateTblLot(lot, lotAttributeDefects, new Attributes { newAttribute });

                    //Act
                    List<LotAttributeDefect> attributeDefects;
                    var chileLot = CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                    //Assert
                    expectedDefect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                    newAttribute.AssertExpected(chileLot.Lot);
                }

                [Test]
                public void Attribute_value_is_updated_and_existing_resolved_defect_is_loaded()
                {
                    //Arrange
                    var chileProduct = CreateChileProduct(new AttributeRanges
                        {
                            { StaticAttributeNames.Asta, 0, 1}
                        });
                    var defect = new Defect(StaticAttributeNames.Asta, 2, 0, 1, true);

                    List<LotAttributeDefect> lotAttributeDefects;
                    var lot = CreateLot(out lotAttributeDefects,
                        new Defects
                            {
                                defect
                            },
                        new Attributes
                            {
                                { StaticAttributeNames.Asta, 0.5 }
                            });

                    var newAttribute = new Attribute(StaticAttributeNames.Asta, 0.75);
                    var tblLot = CreateTblLot(lot, lotAttributeDefects, new Attributes
                        {
                            newAttribute
                        });

                    //Act
                    List<LotAttributeDefect> attributeDefects;
                    var chileLot = CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                    //Assert
                    defect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                    newAttribute.AssertExpected(chileLot.Lot);
                }

                [Test]
                public void Open_defect_value_is_updated_when_old_context_attribute_is_different_and_out_of_range()
                {
                    //Arrange
                    var chileProduct = CreateChileProduct(new AttributeRanges
                        {
                            { StaticAttributeNames.Asta, 0, 1}
                        });
                    var defect = new Defect(StaticAttributeNames.Asta, 2, 0, 1);

                    List<LotAttributeDefect> lotAttributeDefects;
                    var lot = CreateLot(out lotAttributeDefects,
                        new Defects
                            {
                                defect
                            },
                        new Attributes
                            {
                                { StaticAttributeNames.Asta, 2 }
                            });

                    var tblLot = CreateTblLot(lot, lotAttributeDefects, new Attributes
                        {
                            { StaticAttributeNames.Asta, 3 }
                        });

                    //Act
                    List<LotAttributeDefect> attributeDefects;
                    CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                    //Assert
                    defect.Value = 3;
                    defect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                }

                [Test]
                public void Open_defect_is_closed_when_old_context_attribute_is_in_range()
                {
                    //Arrange
                    var chileProduct = CreateChileProduct(new AttributeRanges
                        {
                            { StaticAttributeNames.Asta, 0, 1}
                        });
                    var defect = new Defect(StaticAttributeNames.Asta, 2, 0, 1);

                    List<LotAttributeDefect> lotAttributeDefects;
                    var lot = CreateLot(out lotAttributeDefects,
                        new Defects
                            {
                                defect
                            },
                        new Attributes
                            {
                                { StaticAttributeNames.Asta, 2 }
                            });

                    var newAttribute = new Attribute(StaticAttributeNames.Asta, 0.5);
                    var tblLot = CreateTblLot(lot, lotAttributeDefects, new Attributes
                        {
                            newAttribute
                        });

                    //Act
                    List<LotAttributeDefect> attributeDefects;
                    var chileLot = CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                    //Assert
                    defect.Resolution = true;
                    defect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName));
                    newAttribute.AssertExpected(chileLot.Lot);
                }

                [Test]
                public void Closed_defect_is_loaded_and_new_open_defect_is_created_if_value_is_out_of_range()
                {
                    //Arrange
                    var chileProduct = CreateChileProduct(new AttributeRanges
                        {
                            { StaticAttributeNames.Asta, 0, 1}
                        });
                    var defect = new Defect(StaticAttributeNames.Asta, 2, 0, 1, true);

                    List<LotAttributeDefect> lotAttributeDefects;
                    var lot = CreateLot(out lotAttributeDefects,
                        new Defects
                            {
                                defect
                            },
                        new Attributes
                            {
                                { StaticAttributeNames.Asta, 0.5 }
                            });

                    var newAttribute = new Attribute(StaticAttributeNames.Asta, 3);
                    var tblLot = CreateTblLot(lot, lotAttributeDefects, new Attributes
                        {
                            newAttribute
                        });

                    //Act
                    List<LotAttributeDefect> attributeDefects;
                    var chileLot = CreateChileLotHelper.CreateChileLot(tblLot, new Lot(), chileProduct, out attributeDefects);

                    //Assert
                    defect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName && a.LotDefect.Resolution != null));

                    defect.Resolution = false;
                    defect.Value = 3;
                    defect.AssertExpected(attributeDefects.Single(a => a.AttributeShortName == StaticAttributeNames.Asta.ShortName && a.LotDefect.Resolution == null));
                    newAttribute.AssertExpected(chileLot.Lot);
                }
            }
        }

        protected CreateChileLotHelper CreateChileLotHelper
        {
            get { return _createChileLotHelper ?? (_createChileLotHelper = CreateCreateChileLotHelper()); }
        }
        private CreateChileLotHelper _createChileLotHelper;
        
        private CreateChileLotHelper CreateCreateChileLotHelper()
        {
            var lotMotherMock = new Mock<ILotMother>();
            lotMotherMock.SetupGet(m => m.DefaultEmployee).Returns(new Employee
                {
                    EmployeeId = 0,
                    UserName = "TestUser"
                });
            lotMotherMock.SetupGet(m => m.AttributeNames).Returns(StaticAttributeNames.AttributeNames.ToDictionary(a => new AttributeNameKey(a).KeyValue, a => a));

            var lotStatuses = Enum.GetValues(typeof(LotStat)).Cast<LotStat>().Select(s => new tblLotStatu
                {
                    LotStatID = (int) s,
                    LotStatDesc = s.ToString()
                });
            return new CreateChileLotHelper(lotMotherMock.Object, lotStatuses);
        }

        protected ChileProduct CreateChileProduct(AttributeRanges ranges = null)
        {
            return new ChileProduct
            {
                ProductAttributeRanges = (ranges ?? new AttributeRanges()).Select(r => new ChileProductAttributeRange
                {
                    AttributeShortName = r.AttributeNameKey.AttributeNameKey_ShortName,
                    RangeMin = r.Min,
                    RangeMax = r.Max
                }).ToList()
            };
        }

        protected Lot CreateLot(out List<LotAttributeDefect> lotAttributeDefects, Defects defects = null, Attributes attributes = null)
        {
            lotAttributeDefects = new List<LotAttributeDefect>();
            var lot = new Lot
                {
                    LotDefects = new List<LotDefect>(),
                    Attributes = (attributes ?? new Attributes()).Where(a => a.Value != null).Select(a => new LotAttribute
                        {
                            AttributeShortName = a.NameKey.AttributeNameKey_ShortName,
                            AttributeValue = a.Value.Value
                        }).ToList()
                };
            
            foreach(var defect in defects ?? new Defects())
            {
                LotDefect lotDefect;
                if(defect.Name != null)
                {
                    lotDefect = lot.AddNewDefect(defect.Name.DefectType, defect.Name.Name);
                    lotAttributeDefects.Add(new LotAttributeDefect
                        {
                            AttributeShortName = defect.Name.ShortName,
                            DefectId = lotDefect.DefectId,
                            OriginalAttributeValue = defect.Value,
                            OriginalAttributeMinLimit = defect.Min,
                            OriginalAttributeMaxLimit = defect.Max
                        });
                }
                else
                {
                    lotDefect = lot.AddNewDefect(defect.DefectType, defect.Description);
                }

                if(defect.Resolution)
                {
                    lotDefect.Resolution = new LotDefectResolution
                        {
                            ResolutionType = ResolutionTypeEnum.DataEntryCorrection,
                            Description = "resolution"
                        };
                }
            }

            return lot;
        }

        protected LotEntityObjectMother.LotDTO CreateTblLot(Lot serializedLot, IEnumerable<LotAttributeDefect> attributeDefects = null, Attributes attributes = null)
        {
            var tblLot = new LotEntityObjectMother.LotDTO
                {
                    EntryDate = DateTime.UtcNow.ConvertUTCToLocal(),
                    Serialized = SerializableLot.Serialize(serializedLot, attributeDefects ?? new LotAttributeDefect[0])
                };

            foreach(var attribute in attributes ?? new Attributes())
            {
                var set = tblLot.AttributeSet(attribute.NameKey);
                if(set != null)
                {
                    set((decimal?) attribute.Value);
                }
            }

            return tblLot;
        }

        protected class Attribute
        {
            public IAttributeNameKey NameKey;
            public double? Value;

            public Attribute(IAttributeNameKey nameKey, double value)
            {
                NameKey = nameKey;
                Value = value;
            }

            public void AssertExpected(Lot lot)
            {
                var attribute = lot.Attributes.FirstOrDefault(a => a.AttributeShortName == NameKey.AttributeNameKey_ShortName);
                if(Value == null)
                {
                    Assert.IsNull(attribute);
                }
                else
                {
                    if(attribute == null)
                    {
                        Assert.Fail("Could not find LotAttribute for AttributeName[{0}]", new AttributeNameKey(NameKey));
                    }
                    Assert.AreEqual(Value, attribute.AttributeValue);
                }
            }
        }

        protected class Attributes : List<Attribute>
        {
            public void Add(IAttributeNameKey nameKey, double value)
            {
                Add(new Attribute(nameKey, value));
            }
        }

        protected class Defect
        {
            public AttributeName Name;
            public double Value;
            public double Min;
            public double Max;

            public bool Resolution;
            public DefectTypeEnum DefectType;
            public string Description;

            public Defect(AttributeName name, double value, double min, double max, bool resolution = false)
            {
                Name = name;
                Value = value;
                Min = min;
                Max = max;
                Resolution = resolution;
            }

            public Defect(DefectTypeEnum defectType, string description, bool resolution = false)
            {
                DefectType = defectType;
                Description = description;
                Resolution = resolution;
            }

            public void AssertExpected(LotAttributeDefect defect)
            {
                Assert.AreEqual(Name.ShortName, defect.AttributeShortName);
                Assert.AreEqual(Value, defect.OriginalAttributeValue);
                Assert.AreEqual(Min, defect.OriginalAttributeMinLimit);
                Assert.AreEqual(Max, defect.OriginalAttributeMaxLimit);
                if(Resolution)
                {
                    Assert.IsNotNull(defect.LotDefect.Resolution);
                }
                else
                {
                    Assert.IsNull(defect.LotDefect.Resolution);
                }
            }

            public void AssertExpected(LotDefect defect)
            {
                Assert.AreEqual(DefectType, defect.DefectType);
                Assert.AreEqual(Description, defect.Description);
                if(Resolution)
                {
                    Assert.IsNotNull(defect.Resolution);
                }
                else
                {
                    Assert.IsNull(defect.Resolution);
                }
            }
        }

        protected class Defects : List<Defect>
        {
            public void Add(AttributeName name, double value, double min, double max, bool resolution = false)
            {
                Add(new Defect(name, value, min, max, resolution));
            }

            public void Add(DefectTypeEnum defectType, string description, bool resolution = false)
            {
                Add(new Defect(defectType, description, resolution));
            }
        }

        protected class AttributeRange
        {
            public IAttributeNameKey AttributeNameKey;
            public double Min;
            public double Max;

            public AttributeRange(IAttributeNameKey attributeNameKey, double min, double max)
            {
                AttributeNameKey = attributeNameKey;
                Min = min;
                Max = max;
            }
        }

        protected class AttributeRanges : List<AttributeRange>
        {
            public void Add(IAttributeNameKey nameKey, double min, double max)
            {
                Add(new AttributeRange(nameKey, min, max));
            }
        }
    }
}
