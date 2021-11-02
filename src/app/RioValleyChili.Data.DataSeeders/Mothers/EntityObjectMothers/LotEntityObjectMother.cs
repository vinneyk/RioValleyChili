using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Helpers;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class LotEntityObjectMother : EntityMotherLogBase<LotEntityObjectMother.LotResults, LotEntityObjectMother.CallbackParameters>, ILotMother
    {
        private readonly NewContextHelper _newContextHelper;
        private readonly CreateChileLotHelper _createChileLotHelper;

        public LotEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
            _createChileLotHelper = new CreateChileLotHelper(this, oldContext.CreateObjectSet<tblLotStatu>());
        }

        public enum EntityTypes
        {
            Lot,
            ChileLot,
            AdditiveLot,
            PackagingLot,
            LotAttribute,
            LotDefect,
            LotAttributeDefect,
            LotDefectResolution
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<LotResults> BirthRecords()
        {
            _loadCount.Reset();
            var lotKeys = new Dictionary<LotKey, int>();
            foreach(var lot in SelectLotsToLoad(OldContext))
            {
                _loadCount.AddRead(EntityTypes.Lot);

                LotKey lotKey;
                if(!LotNumberParser.ParseLotNumber(lot.Lot, out lotKey))
                {
                    Log(new CallbackParameters(lot, CallbackReason.InvalidLotNumber));
                    continue;
                }

                if(_newContextHelper.LotLoaded(lotKey))
                {
                    continue;
                }

                if(lotKeys.ContainsKey(lotKey))
                {
                    Log(new CallbackParameters(lot, CallbackReason.DuplicateLotNumber)
                        {
                            ExistingLotNumber = lotKeys[lotKey]
                        });
                    continue;
                }
                lotKeys.Add(lotKey, lot.Lot);

                if(lot.EntryDate == null)
                {
                    Log(new CallbackParameters(lot, CallbackReason.EntryDateNull));
                    continue;
                }

                if(lot.tblProduct == null)
                {
                    Log(new CallbackParameters(lot, CallbackReason.NullProductReference));
                    continue;
                }

                var lotType = lotKey.LotKey_LotTypeId.ToLotType();
                if(lotType == null)
                {
                    Log(new CallbackParameters(lot, CallbackReason.InvalidLotType)
                        {
                            LotKey = lotKey
                        });
                    continue;
                }

                var deserialized = SerializableLot.Deserialize(lot.Serialized);
                var packagingReceived = GetPackagingReceived(lot, deserialized);
                if(packagingReceived == null)
                {
                    continue;
                }
                
                Models.Company vendor = null;
                if(!string.IsNullOrWhiteSpace(lot.Company_IA))
                {
                    vendor = _newContextHelper.GetCompany(lot.Company_IA);
                    if(vendor == null)
                    {
                        Log(new CallbackParameters(lot, CallbackReason.CompanyNotLoaded));
                    }
                }
                
                var newLot = new Lot
                    {
                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,

                        EmployeeId = lot.DeterminedEmployeeId != null ? lot.DeterminedEmployeeId.Value : _newContextHelper.DefaultEmployee.EmployeeId,
                        TimeStamp = lot.DeterminedTimestamp.ConvertLocalToUTC() ?? lotKey.LotKey_DateCreated,
                        
                        ReceivedPackagingProductId = packagingReceived.PackagingProductKey_ProductId,
                        VendorId = vendor == null ? (int?)null : vendor.Id,

                        PurchaseOrderNumber = lot.PurchOrder,
                        ShipperNumber = lot.ShipperNum,

                        Notes = lot.Notes
                    };

                LotResults results = null;
                switch(lotType.Value.ToProductType())
                {
                    case ProductTypeEnum.Additive: results = GetAdditiveLotResults(lot, newLot); break;
                    case ProductTypeEnum.Chile: results = GetChileLotResults(lot, newLot, deserialized); break;
                    case ProductTypeEnum.Packaging: results = GetPackagingLotResults(lot, newLot); break;
                }

                if(results != null)
                {
                    _loadCount.AddLoaded(EntityTypes.Lot);

                    if(results.ChileLot != null)
                    {
                        _loadCount.AddLoaded(EntityTypes.ChileLot);
                        _loadCount.AddLoaded(EntityTypes.LotAttribute, (uint) results.ChileLot.Lot.Attributes.Count);
                        _loadCount.AddLoaded(EntityTypes.LotDefect, (uint) (results.ChileLot.Lot.LotDefects == null ? 0 : results.ChileLot.Lot.LotDefects.Count));
                        _loadCount.AddLoaded(EntityTypes.LotDefectResolution, (uint) (results.ChileLot.Lot.LotDefects == null ? 0 : results.ChileLot.Lot.LotDefects.Count(d => d.Resolution != null)));
                        _loadCount.AddLoaded(EntityTypes.LotAttributeDefect, (uint)(results.LotAttributeDefects == null ? 0 : results.LotAttributeDefects.Count));
                    }

                    if(results.AdditiveLot != null)
                    {
                        _loadCount.AddLoaded(EntityTypes.AdditiveLot);
                        _loadCount.AddLoaded(EntityTypes.LotAttribute, (uint)results.AdditiveLot.Lot.Attributes.Count);
                    }

                    _loadCount.AddLoaded(EntityTypes.PackagingLot, (uint)(results.PackagingLot != null ? 1 : 0));

                    yield return results;
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private IPackagingProductKey GetPackagingReceived(LotDTO lot, SerializableLot deserializedLot)
        {
            if(deserializedLot != null && !string.IsNullOrWhiteSpace(deserializedLot.ReceivedPkgID))
            {
                var packagingProduct = _newContextHelper.GetPackagingProduct(deserializedLot.ReceivedPkgID);
                if(packagingProduct == null)
                {
                    Log(new CallbackParameters(lot, CallbackReason.SerializedReceivedPackagingNotLoaded)
                        {
                            ReceivedPkgID = deserializedLot.ReceivedPkgID
                        });
                    return null;
                }
                return packagingProduct;
            }

            var packagingIds = lot.tblIncomings.Select(i => i.PkgID).Distinct().ToList();
            switch(packagingIds.Count)
            {
                case 0: return _newContextHelper.NoPackagingProduct;
                case 1: return _newContextHelper.GetPackagingProduct(packagingIds.Single());
                default:
                {
                    var packagingInputs = lot.tblIncomings.GroupBy(dto => dto.PkgID, dto => new { Qty = dto.Quantity })
                        .Select(group => new
                        {
                            PkgId = group.Key,
                            Qty = group.Sum(g => g.Qty ?? 0),
                            PackagingProduct =  _newContextHelper.GetPackagingProduct(group.Key)
                        });

                    var pkg = packagingInputs.Select(p => new
                    {
                        TotalWeight = p.PackagingProduct.Weight*(double) p.Qty,
                        Packaging = p.PackagingProduct
                    }).OrderByDescending(p => p.TotalWeight).FirstOrDefault();
                    return pkg == null ? _newContextHelper.NoPackagingProduct : pkg.Packaging;
                }
            }
        }

        private LotResults GetChileLotResults(LotDTO oldLot, Lot newLot, SerializableLot deserializedLot)
        {
            _loadCount.AddRead(EntityTypes.ChileLot);

            var product = _newContextHelper.GetProduct(oldLot.tblProduct.ProdID);
            if(product == null)
            {
                Log(new CallbackParameters(oldLot, CallbackReason.ChileProductNotLoaded));
                return null;
            }
            if(product.ChileProduct == null)
            {
                if(product.AdditiveProduct != null)
                {
                    Log(new CallbackParameters(oldLot, CallbackReason.ExpectedChileProductButFoundAdditive)
                        {
                            Product = product
                        });
                }
                else if(product.PackagingProduct != null)
                {
                    Log(new CallbackParameters(oldLot, CallbackReason.ExpectedChileProductButFoundPackaging)
                        {
                            Product = product
                        });
                }
                return null;
            }

            var chileProduct = _newContextHelper.GetChileProduct(oldLot.tblProduct.ProdID);
            if(chileProduct == null)
            {
                throw new Exception("Could not find ChileProduct with AttributeRanges.");
            }

            List<LotAttributeDefect> lotAttributeDefects;
            var newChileLot = _createChileLotHelper.CreateChileLot(oldLot, newLot, deserializedLot, chileProduct, _newContextHelper.AttributesNames.Values, out lotAttributeDefects);
            if(newChileLot == null)
            {
                return null;
            }

            return new LotResults
                {
                    ChileLot = newChileLot,
                    LotAttributeDefects = lotAttributeDefects
                };
        }

        private static void SetLotHoldStatus(LotStat? lotStat, Lot newLot)
        {
            string description;
            var hold = LotHelper.GetHoldStatus(lotStat, out description);
            if(newLot.Hold != hold)
            {
                newLot.Hold = hold;
                newLot.HoldDescription = description;
            }
        }

        private LotResults GetAdditiveLotResults(LotDTO oldLot, Lot newLot)
        {
            _loadCount.AddRead(EntityTypes.AdditiveLot);

            var product = _newContextHelper.GetProduct(oldLot.tblProduct.ProdID);
            if(product == null)
            {
                Log(new CallbackParameters(oldLot, CallbackReason.AdditiveProductNotLoaded)
                    {
                        Product = product
                    });
                return null;
            }
            if(product.AdditiveProduct == null)
            {
                if(product.ChileProduct != null)
                {
                    Log(new CallbackParameters(oldLot, CallbackReason.ExpectedAdditiveProductButFoundChile)
                        {
                            Product = product
                        });
                }
                else if(product.PackagingProduct != null)
                {
                    Log(new CallbackParameters(oldLot, CallbackReason.ExpectedAdditiveProductButFoundPackaging)
                        {
                            Product = product
                        });
                }
                return null;
            }

            newLot.ProductionStatus = LotProductionStatus.Produced;
            newLot.QualityStatus = LotQualityStatus.Released;
            newLot.ProductSpecComplete = true;
            newLot.ProductSpecOutOfRange = false;
            SetLotHoldStatus(oldLot.LotStat, newLot);
            SetIdentifiable(newLot, oldLot);
            
            return new LotResults
                {
                    AdditiveLot = LoadAttributes(new AdditiveLot
                        {
                            LotDateCreated = newLot.LotDateCreated,
                            LotDateSequence = newLot.LotDateSequence,
                            LotTypeId = newLot.LotTypeId,
                            Lot = newLot,
                            AdditiveProductId = product.AdditiveProduct.Id
                        }, oldLot)
                };
        }

        private AdditiveLot LoadAttributes(AdditiveLot newLot, LotDTO oldLot)
        {
            newLot.Lot.Attributes = new List<LotAttribute>();

            var attributeData = new CreateChileLotHelper.AttributeCommonData(oldLot, this);
            foreach(var attributeName in StaticAttributeNames.AttributeNames)
            {
                var value = oldLot.AttributeGet(attributeName)();
                if(value != null)
                {
                    newLot.Lot.Attributes.Add(new LotAttribute
                        {
                            LotDateCreated = newLot.LotDateCreated,
                            LotDateSequence = newLot.LotDateSequence,
                            LotTypeId = newLot.LotTypeId,
                            AttributeShortName = attributeName.ShortName,

                            AttributeValue = (double) value,
                            AttributeDate = attributeData.DeterminedTestDate,

                            EmployeeId = attributeData.TesterId,
                            TimeStamp = attributeData.EntryDate.Value,
                            Computed = attributeData.NullTestDate
                        });
                }
            }
            
            return newLot;
        }

        private LotResults GetPackagingLotResults(LotDTO oldLot, Lot newLot)
        {
            _loadCount.AddRead(EntityTypes.PackagingLot);

            if(oldLot.tblProduct.PkgID == null)
            {
                Log(new CallbackParameters(oldLot, CallbackReason.PackagingProductNullPackagingId));
                return null;
            }

            var packagingProduct = _newContextHelper.GetPackagingProduct(oldLot.tblProduct.PkgID.Value);
            if(packagingProduct == null)
            {
                Log(new CallbackParameters(oldLot, CallbackReason.PackagingProductNotLoaded));
                return null;
            }

            newLot.ProductionStatus = LotProductionStatus.Produced;
            newLot.QualityStatus = LotQualityStatus.Released;
            newLot.ProductSpecComplete = true;
            newLot.ProductSpecOutOfRange = false;
            SetLotHoldStatus(oldLot.LotStat, newLot);
            SetIdentifiable(newLot, oldLot);
            
            return new LotResults
            {
                PackagingLot = new PackagingLot
                    {
                        LotDateCreated = newLot.LotDateCreated,
                        LotDateSequence = newLot.LotDateSequence,
                        LotTypeId = newLot.LotTypeId,
                        Lot = newLot,

                        PackagingProductId = packagingProduct.Id
                    }
            };
        }

        private static void SetIdentifiable(Lot newLot, LotDTO oldLot)
        {
            var deserialized = SerializableLot.Deserialize(oldLot.Serialized);
            if(deserialized != null)
            {
                if(deserialized.LotIdentifiable != null)
                {
                    newLot.EmployeeId = deserialized.LotIdentifiable.EmployeeKey.EmployeeKeyId;
                    newLot.TimeStamp = deserialized.LotIdentifiable.TimeStamp;
                }
            }
        }

        private static List<LotDTO> SelectLotsToLoad(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblLot>().AsNoTracking()
                .Select(l => new LotDTO
                {
                    Lot = l.Lot,
                    EmployeeID = l.EmployeeID,
                    TesterID = l.TesterID,
                    TestDate = l.TestDate,
                    ProductionDate = l.ProductionDate,
                    EntryDate = l.EntryDate,
                    _BatchStatID = l.BatchStatID,
                    _LotStat = l.LotStat,

                    PurchOrder = l.PurchOrder,
                    ShipperNum = l.ShipperNum,
                    Company_IA = l.Company_IA,

                    AoverB = l.AoverB,
                    AvgAsta = l.AvgAsta,
                    Coli = l.Coli,
                    EColi = l.EColi,
                    Gran = l.Gran,
                    H2O = l.H2O,
                    InsPrts = l.InsPrts,
                    Lead = l.Lead,
                    Mold = l.Mold,
                    RodHrs = l.RodHrs,
                    Sal = l.Sal,
                    Scan = l.Scan,
                    AvgScov = l.AvgScov,
                    TPC = l.TPC,
                    Yeast = l.Yeast,
                    Ash = l.Ash,
                    AIA = l.AIA,
                    Ethox = l.Ethox,
                    AToxin = l.AToxin,
                    BI = l.BI,
                    Gluten = l.Gluten,

                    Notes = l.Notes,
                    Serialized = l.Serialized,

                    DeterminedTimestamp = l.tblLotAttributeHistory.OrderByDescending(h => h.ArchiveDate).Take(1).Select(h => (DateTime?)h.ArchiveDate).DefaultIfEmpty(l.EntryDate).FirstOrDefault(),
                    DeterminedEmployeeId = l.tblLotAttributeHistory.Where(h => h.EmployeeID != null).OrderByDescending(h => h.ArchiveDate).Take(1).Select(h => h.EmployeeID).DefaultIfEmpty(l.EmployeeID).FirstOrDefault(),

                    OriginalHistory = l.tblLotAttributeHistory.Where(h => h.TestDate == null).OrderBy(h => h.ArchiveDate).Select(h => new LotHistory
                        {
                            Lot = h.Lot,
                            TesterID = h.TesterID,
                            TestDate = h.TestDate,
                            EntryDate = h.EntryDate,
                            AoverB = h.AoverB,
                            AvgAsta = h.AvgAsta,
                            Coli = h.Coli,
                            EColi = h.EColi,
                            Gran = h.Gran,
                            H2O = h.H2O,
                            InsPrts = h.InsPrts,
                            Lead = h.Lead,
                            Mold = h.Mold,
                            RodHrs = h.RodHrs,
                            Sal = h.Sal,
                            Scan = h.Scan,
                            AvgScov = h.AvgScov,
                            TPC = h.TPC,
                            Yeast = h.Yeast,
                            Ash = h.Ash,
                            AIA = h.AIA,
                            Ethox = h.Ethox,
                            AToxin = h.AToxin,
                            Gluten = h.Gluten,
                            BI = null, //Not in history records - RI 2016/1/26
                        }).FirstOrDefault(),

                    tblProduct = new[] { l.tblProduct }.Where(p => p != null).Select(p => new ProductDTO
                        {
                            ProdID = p.ProdID,
                            PkgID = p.PkgID
                        }).FirstOrDefault(),
                    tblIncomings = l.tblIncomings.Where(i => i.TTypeID == (int?)TransType.Ingredients || i.TTypeID == (int?)TransType.Other).Select(i => new IncomingDTO
                        {
                            PkgID = i.PkgID,
                            Quantity = i.Quantity ?? 0
                        })
                }).ToList();
        }

        #region ILotMother implementation

        IDictionary<string, AttributeName> ILotMother.AttributeNames { get { return _newContextHelper.AttributesNames; } }

        Employee ILotMother.DefaultEmployee { get { return _newContextHelper.DefaultEmployee; } }

        public void AddRead(EntityTypes entityType)
        {
            _loadCount.AddRead(entityType);
        }

        void ILotMother.Log(CallbackParameters callbackParameteres)
        {
            Log(callbackParameteres);
        }

        void ILotMother.SetLotHoldStatus(LotStat? lotStat, Lot newLot)
        {
            SetLotHoldStatus(lotStat, newLot);
        }

        #endregion

        public class LotDTO : ILotAttributes
        {
            public int Lot { get; set; }
            public int? EmployeeID { get; set; }
            public ProductDTO tblProduct { get; set; }

            public int? TesterID { get; set; }
            public DateTime? TestDate { get; set; }
            public DateTime? ProductionDate { get; set; }
            public DateTime? EntryDate { get; set; }
            public int? _BatchStatID { get; set; }
            public int? _LotStat { get; set; }

            public string PurchOrder { get; set; }
            public string ShipperNum { get; set; }
            public string Company_IA { get; set; }

            public decimal? AoverB { get; set; }
            public decimal? AvgAsta { get; set; }
            public decimal? Coli { get; set; }
            public decimal? EColi { get; set; }
            public decimal? Gran { get; set; }
            public decimal? H2O { get; set; }
            public decimal? InsPrts { get; set; }
            public decimal? Lead { get; set; }
            public decimal? Mold { get; set; }
            public decimal? RodHrs { get; set; }
            public decimal? Sal { get; set; }
            public decimal? Scan { get; set; }
            public decimal? AvgScov { get; set; }
            public decimal? TPC { get; set; }
            public decimal? Yeast { get; set; }
            public decimal? Ash { get; set; }
            public decimal? AIA { get; set; }
            public decimal? Ethox { get; set; }
            public decimal? AToxin { get; set; }
            public decimal? BI { get; set; }
            public decimal? Gluten { get; set; }

            public string Notes { get; set; }
            public string Serialized { get; set; }

            public DateTime? DeterminedTimestamp { get; set; }
            public int? DeterminedEmployeeId { get; set; }

            public LotHistory OriginalHistory { get; set; }

            public BatchStatID? BatchStat
            {
                get { return BatchStatIDHelper.GetBatchStatID(_BatchStatID); }
                set { _BatchStatID = BatchStatIDHelper.GetBatchStatID(value); }
            }

            public LotStat? LotStat
            {
                get { return LotStatHelper.GetLotStat(_LotStat); }
                set { _LotStat = LotStatHelper.GetLotStat(value); }
            }

            public IEnumerable<IncomingDTO> tblIncomings { get; set; }
        }

        public class LotHistory : ILotAttributes
        {
            public int Lot { get; set; }
            public int? TesterID { get; set; }
            public DateTime? TestDate { get; set; }
            public DateTime? EntryDate { get; set; }
            public decimal? AoverB { get; set; }
            public decimal? AvgAsta { get; set; }
            public decimal? Coli { get; set; }
            public decimal? EColi { get; set; }
            public decimal? Gran { get; set; }
            public decimal? H2O { get; set; }
            public decimal? InsPrts { get; set; }
            public decimal? Lead { get; set; }
            public decimal? Mold { get; set; }
            public decimal? RodHrs { get; set; }
            public decimal? Sal { get; set; }
            public decimal? Scan { get; set; }
            public decimal? AvgScov { get; set; }
            public decimal? TPC { get; set; }
            public decimal? Yeast { get; set; }
            public decimal? Ash { get; set; }
            public decimal? AIA { get; set; }
            public decimal? Ethox { get; set; }
            public decimal? AToxin { get; set; }
            public decimal? BI { get; set; }
            public decimal? Gluten { get; set; }
        }

        public class ProductDTO
        {
            public int ProdID { get; set; }
            public int? PkgID { get; set; }
        }

        public class IncomingDTO
        {
            public int PkgID { get; set; }
            public decimal? Quantity { get; set; }
        }

        public class LotResults
        {
            public AdditiveLot AdditiveLot;
            public ChileLot ChileLot;
            public PackagingLot PackagingLot;

            public List<LotAttributeDefect> LotAttributeDefects;

            public Lot Lot
            {
                get
                {
                    if(AdditiveLot != null)
                    {
                        return AdditiveLot.Lot;
                    }

                    if(ChileLot != null)
                    {
                        return ChileLot.Lot;
                    }

                    if(PackagingLot != null)
                    {
                        return PackagingLot.Lot;
                    }

                    return null;
                }
            }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            NullProductReference,
            InvalidLotNumber,
            DuplicateLotNumber,
            InvalidLotType,
            ChileProductNotLoaded,
            AdditiveProductNotLoaded,
            PackagingProductNullPackagingId,
            PackagingProductNotLoaded,
            ExpectedChileProductButFoundAdditive,
            ExpectedChileProductButFoundPackaging,
            ExpectedAdditiveProductButFoundChile,
            ExpectedAdditiveProductButFoundPackaging,
            UnableToDetermineContamination,
            TesterIDNullUsedDefault,
            TestDateNullCurrentDateUsed,
            EntryDateNull,
            ChileLotCompletedButMissingAttributes,
            ChileLotCompletedButMissingBacterialAttributes,
            ChileLotStatusConflict,
            StringTruncated,
            MultipleIncomingPackagings,
            SerializedReceivedPackagingNotLoaded,
            CompanyNotLoaded
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public LotDTO Lot { get; set; }
            public int ExistingLotNumber { get; set; }
            public LotKey LotKey { get; set; }
            public NewContextHelper.ProductResult Product { get; set; }
            public int DefaultEmployeeID { get; set; }
            public List<string> MissingAttributes { get; set; }
            public ChileLot ChileLot { get; set; }
            public string ReceivedPkgID { get; set; }

            protected override CallbackReason ExceptionReason { get { return LotEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return LotEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return LotEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(LotDTO lot, CallbackReason callbackReason) : base(callbackReason)
            {
                Lot = lot;
            }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case LotEntityObjectMother.CallbackReason.Exception:
                        return ReasonCategory.Error;
                        
                    case LotEntityObjectMother.CallbackReason.NullProductReference:
                    case LotEntityObjectMother.CallbackReason.InvalidLotNumber:
                    case LotEntityObjectMother.CallbackReason.DuplicateLotNumber:
                    case LotEntityObjectMother.CallbackReason.InvalidLotType:
                    case LotEntityObjectMother.CallbackReason.ExpectedChileProductButFoundAdditive:
                    case LotEntityObjectMother.CallbackReason.ExpectedChileProductButFoundPackaging:
                    case LotEntityObjectMother.CallbackReason.ChileProductNotLoaded:
                    case LotEntityObjectMother.CallbackReason.ExpectedAdditiveProductButFoundChile:
                    case LotEntityObjectMother.CallbackReason.ExpectedAdditiveProductButFoundPackaging:
                    case LotEntityObjectMother.CallbackReason.AdditiveProductNotLoaded:
                    case LotEntityObjectMother.CallbackReason.PackagingProductNullPackagingId:
                    case LotEntityObjectMother.CallbackReason.PackagingProductNotLoaded:
                    case LotEntityObjectMother.CallbackReason.UnableToDetermineContamination:
                    case LotEntityObjectMother.CallbackReason.EntryDateNull:
                    case LotEntityObjectMother.CallbackReason.ChileLotStatusConflict:
                        return ReasonCategory.RecordSkipped;

                    case LotEntityObjectMother.CallbackReason.TestDateNullCurrentDateUsed:
                    case LotEntityObjectMother.CallbackReason.TesterIDNullUsedDefault:
                    case LotEntityObjectMother.CallbackReason.ChileLotCompletedButMissingAttributes:
                    case LotEntityObjectMother.CallbackReason.ChileLotCompletedButMissingBacterialAttributes:
                    case LotEntityObjectMother.CallbackReason.SerializedReceivedPackagingNotLoaded:
                    case LotEntityObjectMother.CallbackReason.CompanyNotLoaded:
                        return ReasonCategory.Informational;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}