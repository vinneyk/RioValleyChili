using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableLot
    {
        public LotHoldType? HoldType;
        public string HoldDescription;
        [Obsolete("Use old context ReceivedPkgID instead.")]
        public string ReceivedPackagingKey;
        public string ReceivedPkgID;
        public SerializableEmployeeIdentifiable LotIdentifiable;
        public List<Attribute> Attributes;
        public List<Defect> Defects;
        public List<SerializableNote> Notes;
        public List<CustomerAllowance> CustomerAllowances;
        public List<ContractAllowance> ContractAllowances;
        public List<CustomerOrderAllowance> CustomerOrderAllowances;

        private SerializableLot(Lot lot)
        {
            HoldType = lot.Hold;
            HoldDescription = lot.HoldDescription;
            ReceivedPackagingKey = new PackagingProductKey(lot);
            ReceivedPkgID = lot.ReceivedPackaging.Product.ProductCode;

            var keyedAttributeDefects = lot.AttributeDefects.ToDictionary(d => new LotDefectKey(d), d => d);

            Attributes = (lot.Attributes ?? new LotAttribute[0]).Select(a => new Attribute
                {
                    EmployeeKey = new EmployeeKey(a),
                    TimeStamp = a.TimeStamp,
                    NameKey = new AttributeNameKey(a),

                    Value = a.AttributeValue,
                    DateTested = a.AttributeDate,
                    Computed = a.Computed
                }).ToList();

            Defects = (lot.LotDefects ?? new LotDefect[0]).Select(d => new
                {
                    defect = d,
                    resolution = d.Resolution,
                    attributeDefect = keyedAttributeDefects.Where(k => k.Key.Equals(d)).Select(k => k.Value).FirstOrDefault()
                }).Select(d => new Defect
                {
                    AttributeDefect = d.attributeDefect == null ? null : new AttributeDefect
                        {
                            NameKey = new AttributeNameKey(d.attributeDefect),
                            Value = d.attributeDefect.OriginalAttributeValue,
                            Min = d.attributeDefect.OriginalAttributeMinLimit,
                            Max = d.attributeDefect.OriginalAttributeMaxLimit
                        },

                    DefectResolution = d.resolution == null ? null : new DefectResolution
                        {
                            ResolutionType = d.resolution.ResolutionType.ToString(),
                            Description = d.resolution.Description,

                            EmployeeKey = new EmployeeKey(d.resolution),
                            TimeStamp = d.resolution.TimeStamp
                        },
                        
                    DefectType = d.defect.DefectType.ToString(),
                    Description = d.defect.Description
                }).ToList();

            LotIdentifiable = new SerializableEmployeeIdentifiable(lot);

            CustomerAllowances = (lot.CustomerAllowances ?? new List<LotCustomerAllowance>())
                .Select(c => new CustomerAllowance
                    {
                        CompanyName = c.Customer.Company.Name
                    })
                .ToList();

            ContractAllowances = (lot.ContractAllowances ?? new List<LotContractAllowance>())
                .Where(c => c.Contract.ContractId != null)
                .Select(c => new ContractAllowance
                    {
                        ContractId = c.Contract.ContractId.Value
                    })
                .ToList();

            CustomerOrderAllowances = (lot.SalesOrderAllowances ?? new List<LotSalesOrderAllowance>())
                .Where(c => c.SalesOrder.InventoryShipmentOrder.MoveNum != null)
                .Select(c => new CustomerOrderAllowance
                    {
                        OrderNum = c.SalesOrder.InventoryShipmentOrder.MoveNum.Value
                    })
                .ToList();
        }

        public static string Serialize(Lot lot)
        {
            return JsonConvert.SerializeObject(new SerializableLot(lot), Formatting.None);
        }

        public static bool UpdateLot(Lot newLot, SerializableLot deserialized, bool tested, out List<LotAttributeDefect> attributeDefects)
        {
            if(deserialized == null)
            {
                newLot.Attributes = new List<LotAttribute>();
                newLot.LotDefects = new List<LotDefect>();
                attributeDefects = new List<LotAttributeDefect>();
                return false;
            }

            if(deserialized.LotIdentifiable != null)
            {
                newLot.EmployeeId = deserialized.LotIdentifiable.EmployeeKey.EmployeeKeyId;
                newLot.TimeStamp = deserialized.LotIdentifiable.TimeStamp;
            }

            var newAttributeDefects = new List<LotAttributeDefect>();
            var attributeNameKeyParser = new AttributeNameKey();
            var employeeKeyParser = new EmployeeKey();

            newLot.Hold = deserialized.HoldType;
            newLot.HoldDescription = deserialized.HoldDescription;
            newLot.Attributes = deserialized.Attributes.Select(a =>
                {
                    var nameKey = attributeNameKeyParser.Parse(a.NameKey);
                    var employeeKey = employeeKeyParser.Parse(a.EmployeeKey);

                    return new LotAttribute
                        {
                            Lot = newLot,
                            TimeStamp = a.TimeStamp,
                            EmployeeId = employeeKey.EmployeeKey_Id,
                            LotDateCreated = newLot.LotDateCreated,
                            LotDateSequence = newLot.LotDateSequence,
                            LotTypeId = newLot.LotTypeId,
                            AttributeShortName = nameKey.AttributeNameKey_ShortName,

                            AttributeValue = a.Value,
                            AttributeDate = a.DateTested.Date,
                            Computed = a.Computed ?? !tested
                        };
                }).ToList();

            var defectId = 0;
            newLot.LotDefects = deserialized.Defects.Select(d =>
                {
                    DefectTypeEnum defectType;
                    if(!DefectTypeEnum.TryParse(d.DefectType, out defectType))
                    {
                        throw new Exception(string.Format("Could not parse DefectTypeEnum[{0}]", d.DefectType));
                    }

                    var resolutionType = default(ResolutionTypeEnum);
                    if(d.DefectResolution != null)
                    {
                        if(!ResolutionTypeEnum.TryParse(d.DefectResolution.ResolutionType, out resolutionType))
                        {
                            throw new Exception(string.Format("Could not parse ResolutionTypeEnum[{0}]", d.DefectResolution.ResolutionType));
                        }
                    }

                    defectId += 1;
                    var defect = new LotDefect
                        {
                            LotDateCreated = newLot.LotDateCreated,
                            LotDateSequence = newLot.LotDateSequence,
                            LotTypeId = newLot.LotTypeId,
                            DefectId = defectId,

                            DefectType = defectType,
                            Description = d.Description,
                            Resolution = d.DefectResolution == null ? null : new LotDefectResolution
                                {
                                    EmployeeId = employeeKeyParser.Parse(d.DefectResolution.EmployeeKey).EmployeeKey_Id,
                                    TimeStamp = d.DefectResolution.TimeStamp,

                                    LotDateCreated = newLot.LotDateCreated,
                                    LotDateSequence = newLot.LotDateSequence,
                                    LotTypeId = newLot.LotTypeId,
                                    DefectId = defectId,

                                    ResolutionType = resolutionType,
                                    Description = d.DefectResolution.Description,
                                }
                        };

                    if(d.AttributeDefect != null)
                    {
                        var nameKey = attributeNameKeyParser.Parse(d.AttributeDefect.NameKey);

                        newAttributeDefects.Add(new LotAttributeDefect
                            {
                                LotDateCreated = newLot.LotDateCreated,
                                LotDateSequence = newLot.LotDateSequence,
                                LotTypeId = newLot.LotTypeId,
                                DefectId = defectId,
                                AttributeShortName = nameKey.AttributeNameKey_ShortName,

                                OriginalAttributeValue = d.AttributeDefect.Value,
                                OriginalAttributeMinLimit = d.AttributeDefect.Min,
                                OriginalAttributeMaxLimit = d.AttributeDefect.Max,

                                LotDefect = defect
                            });
                    }

                    return defect;
                }).ToList();

            attributeDefects = newAttributeDefects;
            return true;
        }

        public static SerializableLot Deserialize(string serializedLot)
        {
            if(string.IsNullOrWhiteSpace(serializedLot))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<SerializableLot>(serializedLot);
        }

        public class Attribute : EmployeeIdentifiable
        {
            public string NameKey;
            public double Value;
            public DateTime DateTested;
            public bool? Computed;
        }

        public class Defect
        {
            public AttributeDefect AttributeDefect;
            public DefectResolution DefectResolution;
            public string DefectType;
            public string Description;
        }

        public class AttributeDefect
        {
            public string NameKey;
            public double Value;
            public double Min;
            public double Max;
        }

        public class DefectResolution : EmployeeIdentifiable
        {
            public string ResolutionType;
            public string Description;
        }

        public abstract class EmployeeIdentifiable
        {
            public string EmployeeKey;
            public DateTime TimeStamp;
        }

        public class CustomerAllowance
        {
            public string CompanyName;
        }

        public class ContractAllowance
        {
            public int ContractId;
        }

        public class CustomerOrderAllowance
        {
            public int OrderNum;
        }
    }
}