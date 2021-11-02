using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializablePackSchedule
    {
        public static string Serialize(PackSchedule packSchedule)
        {
            return JsonConvert.SerializeObject(new SerializablePackSchedule(packSchedule), Formatting.None);
        }

        public static bool DeserializeIntoPackSchedule(PackSchedule packSchedule, string serializedPackSchedule, out int? pkgID)
        {
            pkgID = null;
            var deserialized = Deserialize(serializedPackSchedule);
            if(deserialized == null)
            {
                return false;
            }
            
            var employeeKeyParser = new EmployeeKey();
            var lotKeyParser = new LotKey();
            pkgID = deserialized.PkgID;

            IEmployeeKey employeeKey;
            if(employeeKeyParser.TryParse(deserialized.EmployeeKey, out employeeKey))
            {
                packSchedule.EmployeeId = employeeKey.EmployeeKey_Id;
            }
            packSchedule.TimeStamp = deserialized.TimeStamp;

            packSchedule.DefaultBatchTargetParameters = new ProductionBatchTargetParameters(deserialized.TargetParameters);
            packSchedule.ProductionBatches = deserialized.Batches.Select(b =>
                {
                    var lotKey = lotKeyParser.Parse(b.LotKey);
                    var batchEmployeeKey = employeeKeyParser.Parse(b.EmployeeKey);
                    var pickedEmployeeKey = employeeKeyParser.Parse(b.PickedInventory.EmployeeKey);
                    
                    var productionBatch = new ProductionBatch
                        {
                            EmployeeId = batchEmployeeKey.EmployeeKey_Id,
                            TimeStamp = b.TimeStamp,

                            LotDateCreated = lotKey.LotKey_DateCreated,
                            LotDateSequence = lotKey.LotKey_DateSequence,
                            LotTypeId = lotKey.LotKey_LotTypeId,

                            PackScheduleDateCreated = packSchedule.DateCreated,
                            PackScheduleSequence = packSchedule.SequentialNumber,

                            ProductionHasBeenCompleted = false,
                            TargetParameters = new ProductionBatchTargetParameters(b.TargetParameters),

                            Production = new ChileLotProduction
                                {
                                    EmployeeId = batchEmployeeKey.EmployeeKey_Id,
                                    TimeStamp = b.TimeStamp,

                                    LotDateCreated = lotKey.LotKey_DateCreated,
                                    LotDateSequence = lotKey.LotKey_DateSequence,
                                    LotTypeId = lotKey.LotKey_LotTypeId,

                                    ProductionType = ProductionType.ProductionBatch,

                                    PickedInventory = new Models.PickedInventory
                                        {
                                            EmployeeId = pickedEmployeeKey.EmployeeKey_Id,
                                            TimeStamp = b.PickedInventory.TimeStamp,
                                            PickedReason = PickedReason.Production,
                                            Archived = false,
                                            Items = new List<PickedInventoryItem>()
                                        }
                                }
                        };
                    return productionBatch;
                }).ToList();
            
            return true;
        }

        private SerializablePackSchedule(PackSchedule packSchedule)
        {
            PkgID = null;
            var productCode = packSchedule.PackagingProduct.Product.ProductCode;
            if(!string.IsNullOrEmpty(productCode))
            {
                int result;
                if(int.TryParse(productCode, out result))
                {
                    PkgID = result;
                }
            }

            EmployeeKey = new EmployeeKey(packSchedule);
            TimeStamp = packSchedule.TimeStamp;
            TargetParameters = new BatchTargetParameters(packSchedule.DefaultBatchTargetParameters);
            Batches = packSchedule.ProductionBatches.Select(b => new Batch
                {
                    LotKey = new LotKey(b),
                    EmployeeKey = new EmployeeKey(b),
                    TimeStamp = b.TimeStamp,

                    TargetParameters = new BatchTargetParameters(b.TargetParameters),
                    PickedInventory = new PickedInventory
                        {
                            EmployeeKey = new EmployeeKey(b.Production.PickedInventory),
                            TimeStamp = b.Production.PickedInventory.TimeStamp
                        }
                }).ToList();
        }

        public static SerializablePackSchedule Deserialize(string serializedPackSchedule)
        {
            return string.IsNullOrWhiteSpace(serializedPackSchedule) ? null : JsonConvert.DeserializeObject<SerializablePackSchedule>(serializedPackSchedule);
        }

        public int? PkgID;
        public string EmployeeKey;
        public DateTime TimeStamp;

        public List<Batch> Batches;
        public BatchTargetParameters TargetParameters;

        public class Batch
        {
            public string LotKey;
            public string EmployeeKey;
            public DateTime TimeStamp;

            public BatchTargetParameters TargetParameters;
            public PickedInventory PickedInventory;
        }

        public class PickedInventory
        {
            public string EmployeeKey;
            public DateTime TimeStamp;
        }

        [JsonObject(MemberSerialization.Fields)]
        public class BatchTargetParameters : IProductionBatchTargetParameters
        {
            public double BatchTargetWeight;
            public double BatchTargetAsta;
            public double BatchTargetScan;
            public double BatchTargetScoville;

            public BatchTargetParameters(IProductionBatchTargetParameters parameters)
            {
                BatchTargetWeight = parameters.BatchTargetWeight;
                BatchTargetAsta = parameters.BatchTargetAsta;
                BatchTargetScan = parameters.BatchTargetScan;
                BatchTargetScoville = parameters.BatchTargetScoville;
            }

            double IProductionBatchTargetParameters.BatchTargetWeight { get { return BatchTargetWeight; } }
            double IProductionBatchTargetParameters.BatchTargetAsta { get { return BatchTargetAsta; } }
            double IProductionBatchTargetParameters.BatchTargetScan { get { return BatchTargetScan; } }
            double IProductionBatchTargetParameters.BatchTargetScoville { get { return BatchTargetScoville; } }
        }
    }
}