using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Helpers;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Mothers.PocoMothers;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ChileProductEntityObjectMother : EntityMotherLogBase<ChileProduct, ChileProductEntityObjectMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;
        private readonly RioValleyChiliDataContext _newContext;

        public ChileProductEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
            _newContextHelper = new NewContextHelper(newContext);
        }

        private int _productId;

        private enum EntityType
        {
            ChileProduct,
            Product,
            ChileProductAttributeRange
        }

        private readonly MotherLoadCount<EntityType> _loadCount = new MotherLoadCount<EntityType>();
        private static readonly ChileTypeKey PaprikaKey = new ChileTypeKey(ChileTypeMother.Paprika);

        protected override IEnumerable<ChileProduct> BirthRecords()
        {
            _loadCount.Reset();
            
            _productId = _newContext.NextIdentity<Product>(p => p.Id);

            foreach(var product in SelectChileProductsToLoad())
            {
                _loadCount.AddRead(EntityType.ChileProduct);
                _loadCount.AddRead(EntityType.Product);

                if(string.IsNullOrWhiteSpace(product.Product))
                {
                    Log(new CallbackParameters(CallbackReason.NoProductName)
                        {
                            Product = product
                        });
                    continue;
                }

#warning Default range values are being created. Need to determine the validity of this. VK 6/25
                var attributes = new List<ChileProductAttributeRange>
                    {
                        BuildChileProductAttributeRange(product, StaticAttributeNames.ColiForms, 0, 2300),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.EColi, 0, 3),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Mold, 0, 3000),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Salmonella, 0, 0),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.TPC, 0, 3000000),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Yeast, 0, 3000),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.RodentHairs, 0, 6),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Lead, 0, 1000),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.AToxin, 0, 20),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Gluten, 0, 20),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.AB, product.ABStart, product.ABEnd),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Asta, product.AstaStart, product.AstaEnd),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Granularity, product.GranStart, product.GranEnd),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Scan, product.ScanStart, product.ScanEnd),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.Scoville, product.ScovStart, product.ScovEnd),
                        BuildChileProductAttributeRange(product, StaticAttributeNames.H2O, product.H2OStart, product.H2OEnd)
                    };

                var chileType = ChileTypeHelper.GetChileType(product.ProdGrpID, product.Product);
                if(chileType == null)
                {
                    chileType = ChileTypeMother.Other;
                    Log(new CallbackParameters(CallbackReason.UnmappedChileType)
                        {
                            Product = product,
                            ChileType = chileType
                        });
                }

                attributes.Add(BuildChileProductAttributeRange(product, StaticAttributeNames.InsectParts, 0, PaprikaKey.Equals(chileType) ? 75 : 50));
                attributes.RemoveAll(i => i == null);

                var chileProduct = new ChileProduct
                    {
                        Id = _productId,
                        ChileTypeId = chileType.ChileTypeKey_ChileTypeId,
                        ChileState = ChileStateHelper.GetChileState((int)product.PTypeID),
                        Mesh = (double?) product.Mesh,
                        IngredientsDescription = product.IngrDesc,
                        Product = new Product
                            {
                                Id = _productId,
                                IsActive = product.InActive == false,
                                Name = product.Product,
                                ProductCode = product.ProdID.ToString(CultureInfo.InvariantCulture),
                                ProductType = ProductTypeEnum.Chile
                            },
                        ProductAttributeRanges = attributes
                    };

                _loadCount.AddLoaded(EntityType.ChileProduct);
                _loadCount.AddLoaded(EntityType.Product);
                _loadCount.AddLoaded(EntityType.ChileProductAttributeRange, (uint) chileProduct.ProductAttributeRanges.Count);
                yield return chileProduct;

                _productId += 1;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private ChileProductAttributeRange BuildChileProductAttributeRange(tblProduct product, AttributeName attributeName, decimal? minRange, decimal? maxRange)
        {
            _loadCount.AddRead(EntityType.ChileProductAttributeRange);

            if(product.EntryDate == null)
            {
                Log(new CallbackParameters(CallbackReason.NullEntryDate)
                    {
                        Product = product
                    });
                return null;
            }
            var entryDate = product.EntryDate.Value.ConvertLocalToUTC();

            if(!minRange.HasValue && !maxRange.HasValue)
            {
                Log(new CallbackParameters(CallbackReason.MissingAttributeData)
                    {
                        Product = product,
                        AttributeName = attributeName
                    });
                return null;
            }

            if(!minRange.HasValue || !maxRange.HasValue)
            {
                Log(new CallbackParameters(CallbackReason.InvalidAttributeData)
                    {
                        Product = product,
                        AttributeName = attributeName
                    });

                minRange = minRange ?? maxRange;
                maxRange = maxRange ?? minRange;
            }

            if(minRange > maxRange)
            {
                var tempMax = minRange;
                minRange = maxRange;
                maxRange = tempMax;
            }

            return new ChileProductAttributeRange
                       {
                           ChileProductId = _productId,
                           AttributeShortName = attributeName.ShortName,
                           RangeMin = (double)minRange,
                           RangeMax = (double)maxRange,
                           EmployeeId = product.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId,
                           TimeStamp = entryDate
                       };
        }

        private List<tblProduct> SelectChileProductsToLoad()
        {
            return OldContext.CreateObjectSet<tblProduct>()
                .Include(p => p.tblProductGrp)
                .Where(p =>
                    p.ProdGrpID != null &&
                    p.ProdGrpID != 7 &&
                    (
                        p.PTypeID == (int)LotTypeEnum.Raw ||
                        p.PTypeID == (int)LotTypeEnum.DeHydrated ||
                        p.PTypeID == (int)LotTypeEnum.WIP ||
                        p.PTypeID == (int)LotTypeEnum.FinishedGood ||
                        p.ProdGrpID == 11 ||
                        p.ProdGrpID == 12
                    ))
                .ToList();
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            NoProductName,
            InvalidAttributeData,
            MissingAttributeData,
            NullEntryDate,
            UnmappedChileType,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public tblProduct Product { get; set; }
            public AttributeName AttributeName { get; set; }
            public ChileType ChileType { get; set; }

            protected override CallbackReason ExceptionReason { get { return ChileProductEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ChileProductEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ChileProductEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ChileProductEntityObjectMother.CallbackReason.Exception:
                        return ReasonCategory.Error;

                    case ChileProductEntityObjectMother.CallbackReason.NullEntryDate:
                    case ChileProductEntityObjectMother.CallbackReason.NoProductName:
                        return ReasonCategory.RecordSkipped;

                    case ChileProductEntityObjectMother.CallbackReason.MissingAttributeData:
                    case ChileProductEntityObjectMother.CallbackReason.UnmappedChileType:
                        return ReasonCategory.Informational;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}
