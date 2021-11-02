using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class PackScheduleEntityObjectMother : EntityMotherLogBase<PackScheduleEntityObjectMother.PackScheduleResult, PackScheduleEntityObjectMother.CallbackParameters>
    {
        private readonly PackSchedulePackagingHelper _packagingHelper;
        private readonly NewContextHelper _newContextHelper;
        private readonly NotebookFactory _notebookFactory;
        private readonly SerializedData _serializedData;
        private Dictionary<int, List<DateTime?>> _batchItemPackSchIds;

        public PackScheduleEntityObjectMother(RioAccessSQLEntities oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
            _packagingHelper = new PackSchedulePackagingHelper(oldContext.CreateObjectSet<tblPackaging>().ToList());
            _notebookFactory = NotebookFactory.Create(newContext);
            _serializedData = new SerializedData(oldContext);
        }

        private enum EntityTypes
        {
            PackSchedule,
            ProductionBatch,
            Notebook,
            PickedInventory,
            ChileLot,
            Lot
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();
        private PackScheduleKeyReservationHelper _keyReservation;
        
        protected override IEnumerable<PackScheduleResult> BirthRecords()
        {
            _loadCount.Reset();
            _keyReservation = new PackScheduleKeyReservationHelper();

            _batchItemPackSchIds = OldContext.CreateObjectSet<tblBatchItem>()
                .Where(i => i.BatchLot != null)
                .Select(i => new
                    {
                        BatchLot = i.BatchLot.Value,
                        i.PackSchID
                    })
                .GroupBy(i => i.BatchLot)
                .ToDictionary(i => i.Key, i => i.Select(g => g.PackSchID).ToList());
            var oldPackSchedules = SelectPackSchedulesToLoad(OldContext);
            foreach(var oldPackSchedule in oldPackSchedules)
            {
                var key = _keyReservation.ParseAndRegisterKey(oldPackSchedule.SerializedKey);
                var requiresKeySync = key == null;
                _loadCount.AddRead(EntityTypes.PackSchedule);

                if(oldPackSchedule.ProdID == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullProduct)
                        {
                            PackSchedule = oldPackSchedule
                        });
                    continue;
                }

                if(oldPackSchedule.Product.PTypeID == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullProductType)
                        {
                            PackSchedule = oldPackSchedule
                        });
                    continue;
                }

                if(oldPackSchedule.PackSchDate == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullPackSchDate)
                        {
                            PackSchedule = oldPackSchedule
                        });
                    continue;
                }

                if(oldPackSchedule.BatchTypeID == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullBatchTypeID)
                        {
                            PackSchedule = oldPackSchedule
                        });
                    continue;
                }

                var chileProduct = _newContextHelper.GetChileProduct(oldPackSchedule.ProdID);
                if(chileProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.ChileProductNotLoaded)
                        {
                            PackSchedule = oldPackSchedule
                        });
                    continue;
                }

                int? line;
                var lineResult = PackScheduleProductionLineHelper.DetermineProductionLine(oldPackSchedule, out line);
                if(lineResult != PackScheduleProductionLineHelper.ProductionLineResult.FromPackSchedule)
                {
                    Log(new CallbackParameters(lineResult.ToCallbackReason())
                    {
                        PackSchedule = oldPackSchedule
                    });
                }
                if(line == null)
                {
                    continue;
                }

                var productionLine = _newContextHelper.GetProductionLine(line);
                if(productionLine == null)
                {
                    Log(new CallbackParameters(CallbackReason.ProductionLineNotLoaded)
                        {
                            PackSchedule = oldPackSchedule,
                            Line = line
                        });
                    continue;
                }

                Models.Company customer = null;
                if(!string.IsNullOrWhiteSpace(oldPackSchedule.Company_IA))
                {
                    customer = _newContextHelper.GetCompany(oldPackSchedule.Company_IA, CompanyType.Customer);
                    if(customer == null)
                    {
                        Log(new CallbackParameters(CallbackReason.CustomerNotLoaded)
                            {
                                PackSchedule = oldPackSchedule
                            });
                        continue;
                    }
                }
                
                key = key ?? _keyReservation.GetNextAndRegisterKey(new PackScheduleKeyReservationHelper.Key
                    {
                        PackScheduleKey_DateCreated = oldPackSchedule.PackSchID.Date,
                        PackScheduleKey_DateSequence = 0
                    });
                var packSchedule = new PackScheduleResult
                    {
                        RequiresKeySync = requiresKeySync,
                        PackSchID = oldPackSchedule.PackSchID,
                        PSNum = oldPackSchedule.PSNum,
                        DateCreated = key.PackScheduleKey_DateCreated,
                        SequentialNumber = key.PackScheduleKey_DateSequence,

                        EmployeeId = oldPackSchedule.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId,
                        TimeStamp = oldPackSchedule.PackSchID.ConvertLocalToUTC(),

                        ChileProductId = chileProduct.Id,

                        ProductionLineLocationId = productionLine.Id,
                        ProductionDeadline = oldPackSchedule.ProductionDeadline.GetDate(),
                        ScheduledProductionDate = oldPackSchedule.PackSchDate.Value.Date,

                        DefaultBatchTargetParameters = new ProductionBatchTargetParameters(oldPackSchedule),
                        SummaryOfWork = oldPackSchedule.PackSchDesc,
                        
                        CustomerId = customer == null ? (int?)null : customer.Id,
                        OrderNumber = oldPackSchedule.OrderNumber
                    };

                bool isSerialized;
                if(!ProcessSerializedPackSchedule(packSchedule, oldPackSchedule, out isSerialized))
                {
                    if(isSerialized)
                    {
                        continue;
                    }
                    if(!ProcessUnserializedPackSchedule(packSchedule, oldPackSchedule))
                    {
                        continue;
                    }
                }
                
                if(packSchedule.EmployeeId == _newContextHelper.DefaultEmployee.EmployeeId)
                {
                    Log(new CallbackParameters(CallbackReason.DefaultEmployee)
                        {
                            PackSchedule = oldPackSchedule,
                            DefaultEmployeeId = packSchedule.EmployeeId
                        });
                }

                _loadCount.AddLoaded(EntityTypes.PackSchedule);
                packSchedule.ProductionBatches.ForEach(b =>
                    {
                        _loadCount.AddLoaded(EntityTypes.ProductionBatch);
                        _loadCount.AddLoaded(EntityTypes.Notebook);
                        _loadCount.AddLoaded(EntityTypes.PickedInventory);
                        _loadCount.AddLoaded(EntityTypes.ChileLot);
                        _loadCount.AddLoaded(EntityTypes.Lot);
                    });

                yield return packSchedule;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        public static IEnumerable<PackScheduleDTO> SelectPackSchedulesToLoad(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblPackSch>().AsNoTracking().Select(p => new PackScheduleDTO
                    {
                        Serialized = p.Serialized,
                        SerializedKey = p.SerializedKey,
                        EmployeeID = p.EmployeeID,
                        PackSchID = p.PackSchID,
                        PSNum = p.PSNum,
                        ProdID = p.ProdID,
                        BatchTypeID = p.BatchTypeID,
                        PackSchDate = p.PackSchDate,
                        ProductionDeadline = p.ProductionDeadline,
                        TargetWgt = p.TargetWgt,
                        TgtAsta = p.TgtAsta,
                        TgtScan = p.TgtScan,
                        TgtScov = p.TgtScov,
                        PackSchDesc = p.PackSchDesc,
                        ProductionLine = p.ProductionLine,
                        Company_IA = p.Company_IA,
                        OrderNumber = p.OrderNumber,

                        Product = new[] { p.tblProduct }.Where(r => r != null).Select(r => new PackScheduleProductDTO
                            {
                                PTypeID = r.PTypeID,
                                Mesh = r.Mesh
                            }).FirstOrDefault(),

                        BatchLots = p.tblLots.Select(b => new BatchLotDTO
                            {
                                Lot = ((int?)b.Lot) ?? -1,
                                BatchTypeID = b.BatchTypeID,
                                ProductionLine = b.ProductionLine,
                                Incoming = b.tblIncomings.Select(i =>
                                    new BatchItemResultLotIncomingDTO
                                    {
                                        PkgID = (int?)i.PkgID ?? 0,
                                        TtlWgt = i.TtlWgt
                                    }),
                                Inventory = b.ViewInventory.Select(i =>
                                    new BatchItemResultLotInventoryDTO
                                    {
                                        PkgID = (int?)i.PkgID ?? 0,
                                        NetWgt = i.NetWgt
                                    })
                            }),

                        BatchItems = p.tblBatchItems.Select(b => new BatchItemDTO
                            {
                                Quantity = b.Quantity,
                                BatchLot = b.BatchLot,
                                SourceLot = new BatchItemSourceLotDTO
                                    {
                                        PTypeID = b.tblLot.PTypeID,
                                        Product = new BatchItemSourceLotProductDTO
                                            {
                                                PkgID = b.tblLot.tblProduct.PkgID,
                                                Packaging = new BatchItemLotProductPackagingDTO
                                                    {
                                                        PkgID = (int?)b.tblLot.tblProduct.tblPackaging.PkgID ?? 0,
                                                        NetWgt = b.tblLot.tblProduct.tblPackaging.NetWgt
                                                    }
                                            }
                                    },

                                Packaging = new BatchItemPackagingDTO
                                    {
                                        NetWgt = b.tblPackaging.NetWgt,
                                        PkgID = (int?)b.tblPackaging.PkgID ?? 0
                                    }
                            })
                    })
                    .OrderByDescending(p => p.SerializedKey != null)
                    .ToList();
        }

        private int? GetSingleBatchTypeID(PackScheduleDTO oldPackSchedule)
        {
            if(oldPackSchedule.BatchLots != null && oldPackSchedule.BatchLots.Any())
            {
                var batchTypeIds = oldPackSchedule.BatchLots.Select(b => b.BatchTypeID).Distinct().Where(t => t != null).ToList();
                if(batchTypeIds.Count == 1)
                {
                    return batchTypeIds[0];
                }
            }
            return null;
        }

        private bool ProcessSerializedPackSchedule(PackSchedule packSchedule, PackScheduleDTO oldPackSchedule, out bool isSerialized)
        {
            isSerialized = true;
            int? pkgID;
            if(!SerializablePackSchedule.DeserializeIntoPackSchedule(packSchedule, oldPackSchedule.Serialized, out pkgID))
            {
                isSerialized = false;
                return false;
            }

            int? batchTypeId;
            if(oldPackSchedule.BatchLots != null && oldPackSchedule.BatchLots.Any())
            {
                batchTypeId = GetSingleBatchTypeID(oldPackSchedule);
                if(batchTypeId == null)
                {
                    Log(new CallbackParameters(CallbackReason.NoSingleBatchType)
                        {
                            PackSchedule = oldPackSchedule
                        });
                    return false;
                }
            }
            else
            {
                batchTypeId = oldPackSchedule.BatchTypeID.Value;
            }
            packSchedule.WorkTypeId = WorkTypeFactory.BuildWorkTypeFromBatchTypeID(batchTypeId.Value).Id;

            var packagingProduct = _newContextHelper.GetPackagingProduct(pkgID);
            if(packagingProduct != null)
            {
                packSchedule.PackagingProductId = packagingProduct.Id;
            }
            else if(!SetDeterminedPackaging(packSchedule, oldPackSchedule))
            {
                return false;
            }
            
            
            var productionBatches = packSchedule.ProductionBatches.ToList();
            productionBatches.RemoveAll(batch =>
                {
                    var lotKey = new LotKey(batch);
                    var lotNumber = LotNumberParser.BuildLotNumber(lotKey);

                    List<DateTime?> ids;
                    if(_batchItemPackSchIds.TryGetValue(lotNumber, out ids))
                    {
                        if(ids.Any())
                        {
                            if(ids.Any(i => i != oldPackSchedule.PackSchID))
                            {
                                Log(new CallbackParameters(CallbackReason.MismatchedBatchItemPackSchID)
                                    {
                                        PackSchedule = oldPackSchedule,
                                        BatchNumber = lotNumber
                                    });
                            }
                            return true;
                        }
                    }

                    _loadCount.AddRead(EntityTypes.ProductionBatch);
                    _loadCount.AddRead(EntityTypes.Notebook);
                    _loadCount.AddRead(EntityTypes.PickedInventory);
                    _loadCount.AddRead(EntityTypes.ChileLot);
                    _loadCount.AddRead(EntityTypes.Lot);

                    if(!_newContextHelper.LotLoaded(lotKey))
                    {
                        Log(new CallbackParameters(CallbackReason.BatchLotNotLoaded)
                            {
                                PackSchedule = oldPackSchedule,
                                LotKey = lotKey
                            });
                        return true;
                    }

                    var dateCreated = batch.LotDateCreated.Date;
                    var sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);

                    batch.Production.PickedInventoryDateCreated = batch.Production.PickedInventory.DateCreated = dateCreated;
                    batch.Production.PickedInventorySequence = batch.Production.PickedInventory.Sequence = sequence;

                    var notebook = _notebookFactory.BirthNext(batch.TimeStamp);
                    batch.InstructionNotebook = notebook;
                    batch.InstructionNotebookDateCreated = notebook.Date;
                    batch.InstructionNotebookSequence = notebook.Sequence;

                    var deserializedProduction = _serializedData.GetDeserialized<SerializableEmployeeIdentifiable>(SerializableType.ChileLotProduction, lotNumber.ToString());
                    if(deserializedProduction != null)
                    {
                        batch.Production.EmployeeId = deserializedProduction.EmployeeKey.EmployeeKeyId;
                        batch.Production.TimeStamp = deserializedProduction.TimeStamp;
                    }

                    return false;
                });
            packSchedule.ProductionBatches = productionBatches;

            return true;
        }

        private bool ProcessUnserializedPackSchedule(PackSchedule packSchedule, PackScheduleDTO oldPackSchedule)
        {
            if(!oldPackSchedule.BatchItems.Any())
            {
                Log(new CallbackParameters(CallbackReason.NoBatchItems)
                {
                    PackSchedule = oldPackSchedule
                });
                return false;
            }

            var batchTypeId = GetSingleBatchTypeID(oldPackSchedule);
            if(batchTypeId == null)
            {
                Log(new CallbackParameters(CallbackReason.NoSingleBatchType)
                    {
                        PackSchedule = oldPackSchedule
                    });
                return false;
            }
            packSchedule.WorkTypeId = WorkTypeFactory.BuildWorkTypeFromBatchTypeID(batchTypeId.Value).Id;

            if(!SetDeterminedPackaging(packSchedule, oldPackSchedule))
            {
                return false;
            }

            packSchedule.ProductionBatches = new List<ProductionBatch>();

            return true;
        }

        private bool SetDeterminedPackaging(PackSchedule packSchedule, PackScheduleDTO oldPackSchedule)
        {
            int? pkgId;
            tblPackaging packaging;
            var packagingResult = _packagingHelper.DeterminePackaging(oldPackSchedule, out pkgId, out packaging);
            if(packagingResult != PackSchedulePackagingHelper.PackagingResult.FromSinglePickedPackaging)
            {
                Log(new CallbackParameters(packagingResult.ToCallbackReason())
                {
                    PackSchedule = oldPackSchedule,
                    Packaging = packaging
                });
            }
            if(pkgId == null)
            {
                return false;
            }

            var packagingProduct = _newContextHelper.GetPackagingProduct(pkgId);
            if(packagingProduct == null)
            {
                Log(new CallbackParameters(CallbackReason.PackagingNotLoaded)
                {
                    PackSchedule = oldPackSchedule,
                    PkgID = pkgId
                });
                return false;
            }

            packSchedule.PackagingProductId = packagingProduct.Id;

            return true;
        }

        #region DTOs

        public class PackScheduleDTO : IProductionBatchTargetParameters
        {
            public string Serialized { get; set; }
            public string SerializedKey { get; set; }
            public int? EmployeeID { get; set; }
            public DateTime PackSchID { get; set; }
            public int? PSNum { get; set; }
            public int? ProdID { get; set; }
            public DateTime? PackSchDate { get; set; }
            public int? BatchTypeID { get; set; }
            public decimal? TargetWgt { get; set; }
            public decimal? TgtAsta { get; set; }
            public decimal? TgtScan { get; set; }
            public decimal? TgtScov { get; set; }
            public string PackSchDesc { get; set; }
            public DateTime? ProductionDeadline { get; set; }
            public int? ProductionLine { get; set; }
            public string Company_IA { get; set; }
            public string OrderNumber { get; set; }

            public PackScheduleProductDTO Product { get; set; }
            public IEnumerable<BatchItemDTO> BatchItems { get; set; }
            public IEnumerable<BatchLotDTO> BatchLots { get; set; }

            double IProductionBatchTargetParameters.BatchTargetWeight { get { return (double) (TargetWgt ?? 0); } }
            double IProductionBatchTargetParameters.BatchTargetAsta { get { return (double) (TgtAsta ?? 0); } }
            double IProductionBatchTargetParameters.BatchTargetScan { get { return (double) (TgtScan ?? 0); } }
            double IProductionBatchTargetParameters.BatchTargetScoville { get { return (double) (TgtScov ?? 0); } }
        }

        public class PackScheduleProductDTO
        {
            public int? PTypeID { get; set; }
            public decimal? Mesh { get; set; }
        }

        public class BatchItemDTO
        {
            public decimal? Quantity { get; set; }
            public int? BatchLot { get; set; }

            public BatchItemSourceLotDTO SourceLot { get; set; }
            public BatchItemPackagingDTO Packaging { get; set; }
        }

        public class BatchItemSourceLotDTO
        {
            public int? PTypeID { get; set; }
            public BatchItemSourceLotProductDTO Product { get; set; }
        }

        public class BatchItemSourceLotProductDTO
        {
            public int? PkgID { get; set; }
            public BatchItemLotProductPackagingDTO Packaging { get; set; }
        }

        public class BatchItemLotProductPackagingDTO
        {
            public decimal? NetWgt { get; set; }
            public int PkgID { get; set; }
        }

        public class BatchLotDTO
        {
            public int Lot { get; set; }
            public int? ProductionLine { get; set; }
            public int? BatchTypeID { get; set; }

            public IEnumerable<BatchItemResultLotIncomingDTO> Incoming { get; set; }
            public IEnumerable<BatchItemResultLotInventoryDTO> Inventory { get; set; }
        }

        public class BatchItemResultLotIncomingDTO
        {
            public int PkgID { get; set; }
            public decimal? TtlWgt { get; set; }
        }

        public class BatchItemResultLotInventoryDTO
        {
            public int PkgID { get; set; }
            public decimal? NetWgt { get; set; }
        }

        public class BatchItemPackagingDTO
        {
            public decimal? NetWgt { get; set; }
            public int PkgID { get; set; }
        }

        #endregion

        public class PackScheduleResult : PackSchedule
        {
            public bool RequiresKeySync;
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            PackagingNotLoaded,
            ChileProductNotLoaded,
            ProductionLineNotLoaded,

            Packaging_CouldNotDetermine,
            Packaging_ResolvedFromMultiplePickedPackaging,
            Packaging_DeterminedFromResultingLotIncoming,
            Packaging_DeterminedFromRelabelInputs,
            Packaging_DeterminedFromRelabelInputsFromDescription,
            Packaging_ToteInDescription,
            Packaging_DrumInDescriptionMesh20,
            Packaging_DrumInDescriptionNotMesh20,
            Packaging_BagInDescription,
            Packaging_BoxInDescription,
            Packaging_DeterminedFromResultingLotInventory,
            Packaging_DeterminedFromReworkInputs,

            Line_CouldNotDetermine,
            Line_DeterminedFromResultingLots,
            Line_DeterminedFromDescription,
            Line_DeterminedFromBatchType,
            NullProduct,
            NullProductType,
            NullPackSchDate,
            NullBatchTypeID,
            NoBatchItems,
            DefaultEmployee,
            BatchLotNotLoaded,
            NoSingleBatchType,
            CustomerNotLoaded,
            StringTruncated,
            MismatchedBatchItemPackSchID
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public PackScheduleDTO PackSchedule { get; set; }
            public int? PkgID { get; set; }
            public int? Line { get; set; }
            public int DefaultEmployeeId { get; set; }
            public tblPackaging Packaging { get; set; }
            public LotKey LotKey { get; set; }
            public int BatchNumber { get; set; }

            protected override CallbackReason ExceptionReason { get { return PackScheduleEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return PackScheduleEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return PackScheduleEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case PackScheduleEntityObjectMother.CallbackReason.Exception: return ReasonCategory.Error;
                    
                    case PackScheduleEntityObjectMother.CallbackReason.NullProduct:
                    case PackScheduleEntityObjectMother.CallbackReason.NullProductType:
                    case PackScheduleEntityObjectMother.CallbackReason.NullPackSchDate:
                    case PackScheduleEntityObjectMother.CallbackReason.NullBatchTypeID:
                    case PackScheduleEntityObjectMother.CallbackReason.NoBatchItems:
                    case PackScheduleEntityObjectMother.CallbackReason.PackagingNotLoaded:
                    case PackScheduleEntityObjectMother.CallbackReason.ChileProductNotLoaded:
                    case PackScheduleEntityObjectMother.CallbackReason.ProductionLineNotLoaded:
                    case PackScheduleEntityObjectMother.CallbackReason.Line_CouldNotDetermine:
                    case PackScheduleEntityObjectMother.CallbackReason.BatchLotNotLoaded:
                    case PackScheduleEntityObjectMother.CallbackReason.NoSingleBatchType:
                    case PackScheduleEntityObjectMother.CallbackReason.CustomerNotLoaded:
                        return ReasonCategory.RecordSkipped;

                    case PackScheduleEntityObjectMother.CallbackReason.DefaultEmployee: return ReasonCategory.Informational;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}
