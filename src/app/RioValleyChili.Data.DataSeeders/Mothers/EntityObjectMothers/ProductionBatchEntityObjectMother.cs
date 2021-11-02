using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ProductionBatchEntityObjectMother : EntityMotherLogBase<ProductionBatch, ProductionBatchEntityObjectMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;
        private readonly SerializedData _serializedData;
        private readonly PickedInventoryItemLocationHelper _pickedInventoryItemLocationHelper;
        private readonly ProductionResultHelper _productionResultHelper;
        private readonly NotebookFactory _notebookFactory;

        public ProductionBatchEntityObjectMother(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }

            _newContextHelper = new NewContextHelper(newContext);
            _serializedData = new SerializedData(oldContext);
            _productionResultHelper = new ProductionResultHelper(_newContextHelper, _serializedData);
            _pickedInventoryItemLocationHelper = new PickedInventoryItemLocationHelper(_newContextHelper);
            _notebookFactory = NotebookFactory.Create(newContext);
        }

        public enum EntityTypes
        {
            ProductionBatch,
            ChileLotProduction,
            LotProductionResults,
            LotProductionResultItem,
            PickedInventory,
            PickedInventoryItem,
            ProductionBatchInstructionNotebook
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<ProductionBatch> BirthRecords()
        {
            _loadCount.Reset();
            var processedLotKeys = new Dictionary<LotKey, int>();
            foreach(var packSchedule in SelectProductionBatchesToLoad(OldContext))
            {
                var packSchId = packSchedule.PackSchID;
                var employeeIdentifiableByLot = GetEmployeeIdentifiableByLot(packSchedule);

                foreach(var oldBatch in packSchedule.ProductionBatches)
                {
                    _loadCount.AddRead(EntityTypes.ProductionBatch);

                    var invalidBatchItems = oldBatch.BatchItems.Where(b => b.PackSchID == null).ToList();
                    if(invalidBatchItems.Any())
                    {
                        invalidBatchItems.ForEach(b => Log(new CallbackParameters(CallbackReason.NullPackSchID)
                            {
                                BatchItem = b
                            }));
                        continue;
                    }

                    invalidBatchItems = oldBatch.BatchItems.Where(b => b.NewLot == null).ToList();
                    if(invalidBatchItems.Any())
                    {
                        invalidBatchItems.ForEach(b => Log(new CallbackParameters(CallbackReason.NullNewLot)
                            {
                                BatchItem = b
                            }));
                        continue;
                    }

                    invalidBatchItems = oldBatch.BatchItems.Where(b => b.ResultLot == null).ToList();
                    if(invalidBatchItems.Any())
                    {
                        invalidBatchItems.ForEach(b => Log(new CallbackParameters(CallbackReason.NullResultLot)
                            {
                                BatchItem = b
                            }));
                        continue;
                    }

                    LotKey lotKey;
                    if(!LotNumberParser.ParseLotNumber(oldBatch.ResultLot.Lot, out lotKey))
                    {
                        Log(new CallbackParameters(CallbackReason.InvalidLotNumber)
                            {
                                Batch = oldBatch
                            });
                        continue;
                    }

                    int previousLotNumber;
                    if(processedLotKeys.TryGetValue(lotKey, out previousLotNumber))
                    {
                        Log(new CallbackParameters(CallbackReason.LotAlreadyProcessed)
                            {
                                Batch = oldBatch
                            });
                        continue;
                    }
                    processedLotKeys.Add(lotKey, oldBatch.ResultLot.Lot);

                    var newPackSchedule = _newContextHelper.GetPackSchedule(packSchId);
                    if(newPackSchedule == null)
                    {
                        Log(new CallbackParameters(CallbackReason.PackScheduleNotLoaded)
                            {
                                Batch = oldBatch,
                                PackSchedule = packSchId
                            });
                        continue;
                    }

                    Tuple<DateTime, int> employeeIdentifiable;
                    if(!employeeIdentifiableByLot.TryGetValue(oldBatch.ResultLot.Lot, out employeeIdentifiable))
                    {
                        employeeIdentifiable = new Tuple<DateTime, int>(oldBatch.ResultLot.EntryDate.Value, oldBatch.ResultLot.EmployeeID.Value);
                    }

                    var chileLotProduction = CreateChileLotProduction(lotKey, oldBatch, newPackSchedule, employeeIdentifiable.Item1, employeeIdentifiable.Item2);
                    if(chileLotProduction == null)
                    {
                        continue;
                    }
                    
                    var productionHasBeenCompleted = false;
                    if(oldBatch.ResultLot != null)
                    {
                        productionHasBeenCompleted = oldBatch.ResultLot.BatchStatID == (int)BatchStatID.Produced;
                    }

                    _loadCount.AddRead(EntityTypes.ProductionBatchInstructionNotebook);
                    var notebook = _notebookFactory.BirthNext(oldBatch.ResultLot.EntryDate.Value, oldBatch.ResultLot.EmployeeID.Value);

                    var productionBatch = new ProductionBatch
                        {
                            TimeStamp = employeeIdentifiable.Item1,
                            EmployeeId = employeeIdentifiable.Item2,

                            LotDateCreated = chileLotProduction.LotDateCreated,
                            LotDateSequence = chileLotProduction.LotDateSequence,
                            LotTypeId = chileLotProduction.LotTypeId,

                            PackScheduleDateCreated = newPackSchedule.DateCreated,
                            PackScheduleSequence = newPackSchedule.SequentialNumber,

                            InstructionNotebookDateCreated = notebook.Date,
                            InstructionNotebookSequence = notebook.Sequence,

                            ProductionHasBeenCompleted = productionHasBeenCompleted,

                            TargetParameters = new ProductionBatchTargetParameters(oldBatch.ResultLot),

                            Production = chileLotProduction,
                            InstructionNotebook = notebook
                        };

                    _loadCount.AddLoaded(EntityTypes.ChileLotProduction);
                    _loadCount.AddLoaded(EntityTypes.ProductionBatch);
                    if(productionBatch.Production.Results != null)
                    {
                        _loadCount.AddLoaded(EntityTypes.LotProductionResults);
                        _loadCount.AddLoaded(EntityTypes.LotProductionResultItem, (uint) productionBatch.Production.Results.ResultItems.Count);
                    }
                    _loadCount.AddLoaded(EntityTypes.PickedInventory);
                    _loadCount.AddLoaded(EntityTypes.PickedInventoryItem, (uint) productionBatch.Production.PickedInventory.Items.Count);
                    _loadCount.AddLoaded(EntityTypes.ProductionBatchInstructionNotebook);

                    yield return productionBatch;
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private static Dictionary<int, Tuple<DateTime, int>> GetEmployeeIdentifiableByLot(PackScheduleDTO packSchedule)
        {
            if(packSchedule == null || string.IsNullOrWhiteSpace(packSchedule.Serialized))
            {
                return new Dictionary<int, Tuple<DateTime, int>>();
            }

            try
            {
                var lotKeyParser = new LotKey();
                var employeeKey = new EmployeeKey();
                return SerializablePackSchedule.Deserialize(packSchedule.Serialized).Batches.ToDictionary(b =>
                {
                    var lotKey = lotKeyParser.Parse(b.LotKey);
                    return LotNumberBuilder.BuildLotNumber(lotKey).LotNumber;
                }, b => new Tuple<DateTime, int>(b.TimeStamp, employeeKey.Parse(b.EmployeeKey).EmployeeKey_Id));
            }
            catch(Exception ex)
            {
                throw new Exception("Error parsing PackSchedule deserialized data.", ex);
            }
        }

        private ChileLotProduction CreateChileLotProduction(LotKey lotKey, ProductionBatchDTO oldBatch, PackSchedule newPackSchedule, DateTime timeStamp, int employeeId)
        {
            _loadCount.AddRead(EntityTypes.ChileLotProduction);

            var chileLot = _newContextHelper.GetChileLot(lotKey);
            if(chileLot == null)
            {
                Log(new CallbackParameters(CallbackReason.OutputChileLotNotLoaded)
                    {
                        Batch = oldBatch
                    });
                return null;
            }

            var pickedInventory = CreateNewPickedInventory(oldBatch);

            LotProductionResults productionResults;
            _productionResultHelper.CreateNewProductionResult(newPackSchedule, chileLot, oldBatch.ResultLot, out productionResults);
            if(productionResults != null)
            {
                _loadCount.AddRead(EntityTypes.LotProductionResults);
                productionResults.ResultItems = _productionResultHelper.CreateNewProductionResultItems(productionResults, oldBatch.ResultLot)
                    .Where(r =>
                        {
                            _loadCount.AddRead(EntityTypes.LotProductionResultItem);
                            return r.Result == ProductionResultHelper.CreateNewProductionResultItemResult.Success;
                        })
                    .Select(i => i.ResultItem).ToList();
            }

            var deserializedProduction = _serializedData.GetDeserialized<SerializableEmployeeIdentifiable>(SerializableType.ChileLotProduction, oldBatch.ResultLot.Lot.ToString());
            pickedInventory.Archived = productionResults != null;

            return new ChileLotProduction
                {
                    TimeStamp = deserializedProduction == null ? timeStamp : deserializedProduction.TimeStamp,
                    EmployeeId = deserializedProduction == null ? employeeId : deserializedProduction.EmployeeKey.EmployeeKeyId,

                    LotDateCreated = chileLot.LotDateCreated,
                    LotDateSequence = chileLot.LotDateSequence,
                    LotTypeId = chileLot.LotTypeId,

                    PickedInventoryDateCreated = pickedInventory.DateCreated,
                    PickedInventorySequence = pickedInventory.Sequence,

                    ProductionType = ProductionType.ProductionBatch,

                    PickedInventory = pickedInventory,
                    Results = productionResults
                };
        }

        private PickedInventory CreateNewPickedInventory(ProductionBatchDTO batch)
        {
            _loadCount.AddRead(EntityTypes.PickedInventory);

            var dateCreated = batch.BatchItems.Min(b => b.EntryDate.Date);
            var sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);
            var latest = batch.BatchItems.OrderByDescending(b => b.EntryDate).First();
            var pickedInventory = new PickedInventory
                {
                    DateCreated = dateCreated,
                    Sequence = sequence,

                    EmployeeId = latest.EmployeeID.Value,
                    TimeStamp = latest.EntryDate.ConvertLocalToUTC(),

                    Archived = false,
                    PickedReason = PickedReason.Production
                };

            pickedInventory.Items = CreatePickedInventoryItems(pickedInventory, batch.BatchItems).ToList();

            return pickedInventory;
        }

        private IEnumerable<PickedInventoryItem> CreatePickedInventoryItems(PickedInventory pickedInventory, IEnumerable<tblBatchItemDTO> batchItems)
        {
            var itemSequence = 1;

            foreach(var batchItem in batchItems)
            {
                _loadCount.AddRead(EntityTypes.PickedInventoryItem);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(batchItem.Lot, out lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemInvalidLotNumber)
                        {
                            BatchItem = batchItem
                        });
                    continue;
                }

                if(!_newContextHelper.LotLoaded(lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemLotNotLoaded)
                        {
                            BatchItem = batchItem
                        });
                    continue;
                }

                var packaging = batchItem.PTypeID == (int?)LotTypeEnum.Packaging ? _newContextHelper.NoPackagingProduct : _newContextHelper.GetPackagingProduct(batchItem.PkgID);
                if(packaging == null)
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemPackagingNotLoaded)
                        {
                            BatchItem = batchItem
                        });
                    continue;
                }

                var treatment = _newContextHelper.GetInventoryTreatment(batchItem.TrtmtID);
                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemTreatmentNotLoaded)
                        {
                            BatchItem = batchItem
                        });
                    continue;
                }

                int? currentLocationId = null;
                var productionLocation = _newContextHelper.GetProductionLocation(batchItem.tblLocation);
                if(productionLocation != null)
                {
                    currentLocationId = productionLocation.Id;
                }
                else
                {
                    var warehouseLocation = _newContextHelper.GetLocation(batchItem.LocID);
                    if(warehouseLocation != null)
                    {
                        currentLocationId = warehouseLocation.Id;
                    }
                }

                if(currentLocationId == null)
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemCurrentLocationCouldnotBeDetermined)
                        {
                            BatchItem = batchItem
                        });
                    continue;
                }

                int? warehouseLocationId;
                var warehouseLocationResult = _pickedInventoryItemLocationHelper.DeterminePickedFromLocation(batchItem, out warehouseLocationId);
                if(warehouseLocationResult == PickedInventoryItemLocationHelper.Result.UnableToDetermine)
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemDefaultPickedLocation)
                        {
                            BatchItem = batchItem
                        });
                    warehouseLocationId = _newContextHelper.UnknownFacilityLocation.Id;
                }

                if(warehouseLocationId == null)
                {
                    Log(new CallbackParameters(CallbackReason.BatchItemPickedLocationNotDetermined)
                        {
                            BatchItem = batchItem
                        });
                    continue;
                }

                yield return new PickedInventoryItem
                    {
                        DateCreated = pickedInventory.DateCreated,
                        Sequence = pickedInventory.Sequence,
                        ItemSequence = itemSequence,

                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,

                        PackagingProductId = packaging.Id,
                        FromLocationId = warehouseLocationId.Value,
                        TreatmentId = treatment.Id,
                        CurrentLocationId = currentLocationId.Value,
                        ToteKey = new string((batchItem.Tote ?? "").Take(Constants.StringLengths.ToteKeyLength).ToArray()),
                        Quantity = (int) (batchItem.Quantity ?? 0)
                    };

                itemSequence += 1;
            }
        }

        private static List<PackScheduleDTO> SelectProductionBatchesToLoad(ObjectContext objectContext)
        {
            var packSchedules = objectContext.CreateObjectSet<tblPackSch>().AsNoTracking();
            return objectContext.CreateObjectSet<tblBatchItem>().AsNoTracking().Select(b => new tblBatchItemDTO
                {
                    RowId = b.RowId,
                    PackSchID = b.PackSchID,
                    PSNum = b.tblPackSch != null ? b.tblPackSch.PSNum : (int?)null,
                    Lot = b.Lot,
                    PTypeID = b.tblLot.PTypeID,
                    PkgID = b.PkgID,
                    TrtmtID = b.TrtmtID,
                    LocID = b.LocID,
                    NewLot = b.NewLot,
                    EntryDate = b.EntryDate,
                    EmployeeID = b.EmployeeID,
                    Tote = b.Tote,
                    Quantity = b.Quantity,

                    tblLot = new[] { b.tblLot }.Select(l => new tblLotSourceDTO
                        {
                            tblOutgoings = l.tblOutgoings
                                .Where(o => o.BatchLot == b.NewLot)
                                .Select(o => new BatchItemSourceLotOutgoing
                                {
                                    BatchLot = o.BatchLot,
                                    EntryDate = o.EntryDate,
                                    LocID = o.LocID
                                })
                        }).FirstOrDefault(),

                    ResultLot = new[] { b.resultLot }.Where(l => l != null).Select(resultLot => new tblLotResultDTO
                        {
                            Lot = resultLot.Lot,
                            Shift = resultLot.Shift,
                            BatchBegTime = resultLot.BatchBegTime,
                            BatchEndTime = resultLot.BatchEndTime,
                            ProductionDate = resultLot.ProductionDate,
                            EntryDate = resultLot.EntryDate,
                            EmployeeID = resultLot.EmployeeID,
                            ProductionLine = resultLot.ProductionLine,
                            BatchStatID = resultLot.BatchStatID,
                            TargetWgt = resultLot.TargetWgt,
                            TgtAsta = resultLot.TgtAsta,
                            TgtScan = resultLot.TgtScan,
                            TgtScov = resultLot.TgtScov,

                            tblIncomings = resultLot.tblIncomings.Where(i => i.Quantity >= 1).Select(i => new tblIncomingDTO
                                {
                                    ID = i.ID,
                                    TTypeID = i.TTypeID,
                                    PkgID = i.PkgID,
                                    LocID = i.LocID,
                                    TrtmtID = i.TrtmtID,
                                    Quantity = i.Quantity
                                })
                        }).FirstOrDefault(),

                    tblLocation = new[] { b.tblLocation }.Select(l => new tblLocationDTO
                        {
                            Street = l.Street,
                            Row = l.Row
                        }).FirstOrDefault()
                })
                .GroupBy(b => b.PackSchID)
                .Select(batchItems => new PackScheduleDTO
                    {
                        PackSchID = batchItems.Key,
                        Serialized = packSchedules.Where(p => p.PackSchID == batchItems.Key).Select(p => p.Serialized).FirstOrDefault(),
                        BatchItems = batchItems
                    })
                .ToList();
        }

        #region DTOs

        public class PackScheduleDTO
        {
            public DateTime? PackSchID { get; set; }

            public string Serialized { get; set; }

            public IEnumerable<tblBatchItemDTO> BatchItems { get; set; }

            public List<ProductionBatchDTO> ProductionBatches
            {
                get
                {
                    return _productionBatches ?? (_productionBatches = BatchItems.GroupBy(b => b.NewLot).Select(b => new ProductionBatchDTO
                        {
                            BatchItems = b
                        }).ToList());
                }
            }
            private List<ProductionBatchDTO> _productionBatches;
        }

        public class ProductionBatchDTO
        {
            public IEnumerable<tblBatchItemDTO> BatchItems { get; set; }

            public tblLotResultDTO ResultLot
            {
                get
                {
                    if(!_gotResultLot)
                    {
                        var firstBatchItem = _FirstBatchItem;
                        _resultLot = firstBatchItem != null ? firstBatchItem.ResultLot : null;
                        _gotResultLot = true;
                    }
                    return _resultLot;
                }
            }
            private tblLotResultDTO _resultLot;
            private bool _gotResultLot = false;

            private List<tblBatchItemDTO> _BatchItems
            {
                get { return _batchItems ?? (_batchItems = BatchItems.ToList()); }
            }
            private List<tblBatchItemDTO> _batchItems;

            private tblBatchItemDTO _FirstBatchItem
            {
                get
                {
                    if(!_gotFirstBatchItem)
                    {
                        _firstBatchItem = _BatchItems.FirstOrDefault();
                        _gotFirstBatchItem = true;
                    }
                    return _firstBatchItem;
                }
            }
            private tblBatchItemDTO _firstBatchItem;
            private bool _gotFirstBatchItem = false;
        }

        public class tblBatchItemDTO
        {
            public int? NewLot { get; set; }

            public DateTime EntryDate { get; set; }

            public int? EmployeeID { get; set; }

            public int Lot { get; set; }

            public int? PTypeID { get; set; }

            public int PkgID { get; set; }

            public int TrtmtID { get; set; }

            public tblLocationDTO tblLocation { get; set; }

            public string Tote { get; set; }

            public decimal? Quantity { get; set; }

            public tblLotSourceDTO tblLot { get; set; }

            public tblLotResultDTO ResultLot { get; set; }

            public IEnumerable<tblBatchInstrDTO> Instructions { get; set; }

            public int LocID { get; set; }

            public int RowId { get; set; }

            public DateTime? PackSchID { get; set; }

            public int? PSNum { get; set; }
        }

        public class tblBatchInstrDTO 
        {
            public int? Step { get; set; }

            public string Action { get; set; }

            public DateTime EntryDate { get; set; }
        }

        public class tblLotSourceDTO
        {
            public IEnumerable<BatchItemSourceLotOutgoing> tblOutgoings { get; set; }
        }

        public class BatchItemSourceLotOutgoing
        {
            public int? BatchLot { get; set; }

            public DateTime EntryDate { get; set; }

            public int LocID { get; set; }
        }

        public class tblLotResultDTO : IProductionBatchTargetParameters
        {
            public int Lot { get; set; }
            public string Shift { get; set; }
            public DateTime? BatchBegTime { get; set; }
            public DateTime? BatchEndTime { get; set; }
            public int? EmployeeID { get; set; }
            public DateTime? EntryDate { get; set; }
            public DateTime? ProductionDate { get; set; }
            public int? ProductionLine { get; set; }
            public int? BatchStatID { get; set; }
            public decimal? TargetWgt { get; set; }
            public decimal? TgtAsta { get; set; }
            public decimal? TgtScan { get; set; }
            public decimal? TgtScov { get; set; }

            public IEnumerable<tblIncomingDTO> tblIncomings { get; set; }

            #region IEqualityComparer

            public static IEqualityComparer<tblLotResultDTO> BatchItemResultLotComparer
            {
                get { return BatchItemResultLotComparerInstance; }
            }

            private sealed class BatchItemResultLotEqualityComparer : IEqualityComparer<tblLotResultDTO>
            {
                public bool Equals(tblLotResultDTO x, tblLotResultDTO y)
                {
                    if(ReferenceEquals(x, y)) return true;
                    if(x == null || y == null)
                    {
                        if(x != null || y != null)
                        {
                            return false;
                        }
                    }
                    return string.Equals(x.Shift, y.Shift) &&
                        x.BatchBegTime.Equals(y.BatchBegTime) &&
                           x.EntryDate.Equals(y.EntryDate) &&
                           x.BatchEndTime.Equals(y.BatchEndTime) &&
                           x.ProductionLine == y.ProductionLine &&
                           x.BatchStatID == y.BatchStatID;
                }

                public int GetHashCode(tblLotResultDTO obj)
                {
                    unchecked
                    {
                        var hashCode = (obj.Shift != null ? obj.Shift.GetHashCode() : 0);
                        hashCode = (hashCode * 397) ^ obj.BatchBegTime.GetHashCode();
                        hashCode = (hashCode * 397) ^ obj.EntryDate.GetHashCode();
                        hashCode = (hashCode * 397) ^ obj.BatchEndTime.GetHashCode();
                        hashCode = (hashCode * 397) ^ obj.ProductionLine.GetHashCode();
                        hashCode = (hashCode * 397) ^ obj.BatchStatID.GetHashCode();
                        return hashCode;
                    }
                }
            }

            private static readonly IEqualityComparer<tblLotResultDTO> BatchItemResultLotComparerInstance =
                new BatchItemResultLotEqualityComparer();

            #endregion

            double IProductionBatchTargetParameters.BatchTargetWeight { get { return (double) (TargetWgt ?? 0); } }
            double IProductionBatchTargetParameters.BatchTargetAsta { get { return (double) (TgtAsta ?? 0); } }
            double IProductionBatchTargetParameters.BatchTargetScan { get { return (double) (TgtScan ?? 0); } }
            double IProductionBatchTargetParameters.BatchTargetScoville { get { return (double) (TgtScov ?? 0); } }
        }

        public class tblIncomingDTO
        {
            public int ID { get; set; }
            public int? TTypeID { get; set; }
            public int PkgID { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
            public decimal? Quantity { get; set; }
        }

        public class tblLocationDTO : ILocation
        {
            public string Street { get; set; }
            public int? Row { get; set; }
        }

        #endregion

        public enum CallbackReason
        {
            Exception,
            Summary,
            NullPackSchID,
            NullNewLot,
            NullResultLot,
            InvalidLotNumber,
            PackScheduleNotLoaded,
            OutputChileLotNotLoaded,
            BatchItemInvalidLotNumber,
            BatchItemPackagingNotLoaded,
            BatchItemTreatmentNotLoaded,
            BatchItemCurrentLocationCouldnotBeDetermined,
            BatchItemDefaultPickedLocation,
            BatchItemPickedLocationNotDetermined,
            BatchItemLotNotLoaded,
            LotAlreadyProcessed,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public ProductionBatchDTO Batch { get; set; }

            public DateTime? PackSchedule { get; set; }

            public tblBatchItemDTO BatchItem { get; set; }

            public LotKey LotKey { get; set; }

            protected override CallbackReason ExceptionReason { get { return ProductionBatchEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ProductionBatchEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ProductionBatchEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ProductionBatchEntityObjectMother.CallbackReason.Exception:
                    case ProductionBatchEntityObjectMother.CallbackReason.LotAlreadyProcessed:
                        return ReasonCategory.Error;

                    case ProductionBatchEntityObjectMother.CallbackReason.NullPackSchID:
                    case ProductionBatchEntityObjectMother.CallbackReason.NullNewLot:
                    case ProductionBatchEntityObjectMother.CallbackReason.NullResultLot:
                    case ProductionBatchEntityObjectMother.CallbackReason.InvalidLotNumber:
                    case ProductionBatchEntityObjectMother.CallbackReason.PackScheduleNotLoaded:
                    case ProductionBatchEntityObjectMother.CallbackReason.OutputChileLotNotLoaded:
                    case ProductionBatchEntityObjectMother.CallbackReason.BatchItemInvalidLotNumber:
                    case ProductionBatchEntityObjectMother.CallbackReason.BatchItemPackagingNotLoaded:
                    case ProductionBatchEntityObjectMother.CallbackReason.BatchItemTreatmentNotLoaded:
                    case ProductionBatchEntityObjectMother.CallbackReason.BatchItemCurrentLocationCouldnotBeDetermined:
                    case ProductionBatchEntityObjectMother.CallbackReason.BatchItemPickedLocationNotDetermined:
                    case ProductionBatchEntityObjectMother.CallbackReason.BatchItemLotNotLoaded:
                        return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}