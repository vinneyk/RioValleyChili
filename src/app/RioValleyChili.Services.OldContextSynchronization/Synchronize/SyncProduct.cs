using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.Product)]
    public class SyncProduct : SyncCommandBase<IProductUnitOfWork, SyncProductParameters>
    {
        public SyncProduct(IProductUnitOfWork unitOfWork) : base(unitOfWork) {}

        public override void Synchronize(Func<SyncProductParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var parameters = getInput();
            var product = UnitOfWork.ProductRepository.FindByKey(parameters.ProductKey);
            int? id = null;
            var message = "";
            switch(product.ProductType)
            {
                case ProductTypeEnum.Additive:
                    id = SyncAdditiveProduct(product).ProdID;
                    message = ConsoleOutput.SynchedTblProduct;
                    break;

                case ProductTypeEnum.Chile:
                    id = SyncChileProduct(product, parameters.DeletedIngredients).ProdID;
                    message = ConsoleOutput.SynchedTblProduct;
                    break;

                case ProductTypeEnum.Packaging:
                    id = SyncPackagingProduct(product).PkgID;
                    message = ConsoleOutput.SynchedTblPackaging;
                    break;

                case ProductTypeEnum.NonInventory:
                    id = SyncNonInventoryProduct(product).ProdID;
                    message = ConsoleOutput.SynchedTblProduct;
                    break;
            }

            OldContext.SaveChanges();

            Console.WriteLine(message, id);
        }

        private tblProduct SyncChileProduct(Product product, List<AdditiveTypeKey> deletedIngredients)
        {
            var chileProduct = UnitOfWork.ChileProductRepository.FindByKey(product.ToProductKey(),
                c => c.ProductAttributeRanges,
                c => c.Ingredients);

            tblProduct oldProduct;
            int prodId;
            var syncIngredients = new SyncChileProductHelper(OldContext);
            if(int.TryParse(product.ProductCode, out prodId))
            {
                oldProduct = syncIngredients.GetProduct(prodId);
            }
            else
            {
                throw new Exception("OldContextSync - ProductCode must be numeric.");
            }

            if(oldProduct == null)
            {
                oldProduct = new tblProduct
                    {
                        ProdID = prodId,
                        ProdGrpID = chileProduct.ChileTypeId,
                        PTypeID = (int) chileProduct.ChileState,
                        EmployeeID = 100,
                        EntryDate = DateTime.Now,
                        TrtmtID = StaticInventoryTreatments.NoTreatment.Id,
                        tblProductIngrs = new EntityCollection<tblProductIngr>(),
                        s_GUID = Guid.NewGuid()
                    };
                OldContext.tblProducts.AddObject(oldProduct);
            }

            oldProduct.Product = product.Name;
            oldProduct.InActive = !product.IsActive;
            oldProduct.Mesh = (decimal?) chileProduct.Mesh;
            oldProduct.IngrDesc = chileProduct.IngredientsDescription;

            SetChileProductSpec(chileProduct, oldProduct);
            syncIngredients.SyncIngredients(chileProduct, oldProduct, deletedIngredients);

            return oldProduct;
        }

        private tblProduct SyncAdditiveProduct(Product product)
        {
            var additiveProduct = UnitOfWork.AdditiveProductRepository.FindByKey(product.ToProductKey());

            tblProduct oldProduct;
            int prodId;
            if(int.TryParse(product.ProductCode, out prodId))
            {
                oldProduct = OldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
            }
            else
            {
                throw new Exception("OldContextSync - ProductCode must be numeric.");
            }

            if(oldProduct == null)
            {
                oldProduct = new tblProduct
                    {
                        ProdID = prodId,
                        ProdGrpID = 99,
                        PTypeID = 4,
                        EmployeeID = 100,
                        EntryDate = DateTime.Now,
                        TrtmtID = StaticInventoryTreatments.NoTreatment.Id,

                        s_GUID = Guid.NewGuid()
                    };
                OldContext.tblProducts.AddObject(oldProduct);
            }

            oldProduct.Product = product.Name;
            oldProduct.InActive = !product.IsActive;
            oldProduct.IngrID = additiveProduct.AdditiveTypeId;

            return oldProduct;
        }

        private tblPackaging SyncPackagingProduct(Product product)
        {
            var packagingProduct = UnitOfWork.PackagingProductRepository.FindByKey(product.ToProductKey());

            tblPackaging oldPackaging;
            int pkgId;
            if(int.TryParse(product.ProductCode, out pkgId))
            {
                oldPackaging = OldContext.tblPackagings.FirstOrDefault(p => p.PkgID == pkgId);
            }
            else
            {
                throw new Exception("OldContextSync - ProductCode must be numeric.");
            }

            if(oldPackaging == null)
            {
                oldPackaging = new tblPackaging
                    {
                        PkgID = pkgId,
                        EmployeeID = 100,
                        EntryDate = DateTime.Now,

                        s_GUID = Guid.NewGuid()
                    };
                OldContext.tblPackagings.AddObject(oldPackaging);
            }

            oldPackaging.Packaging = product.Name;
            oldPackaging.InActive = !product.IsActive;
            oldPackaging.NetWgt = (decimal?) packagingProduct.Weight;
            oldPackaging.PkgWgt = (decimal?) packagingProduct.PackagingWeight;
            oldPackaging.PalletWgt = (decimal?) packagingProduct.PalletWeight;

            return oldPackaging;
        }

        private tblProduct SyncNonInventoryProduct(Product product)
        {
            tblProduct oldProduct;
            int prodId;
            if(int.TryParse(product.ProductCode, out prodId))
            {
                oldProduct = OldContext.tblProducts.FirstOrDefault(p => p.ProdID == prodId);
            }
            else
            {
                throw new Exception("OldContextSync - ProductCode must be numeric.");
            }

            if(oldProduct == null)
            {
                oldProduct = new tblProduct
                    {
                        ProdID = prodId,
                        ProdGrpID = 7,
                        PTypeID = 3,
                        EmployeeID = 100,
                        EntryDate = DateTime.Now,
                        TrtmtID = StaticInventoryTreatments.NoTreatment.Id,

                        s_GUID = Guid.NewGuid()
                    };
                OldContext.tblProducts.AddObject(oldProduct);
            }

            oldProduct.Product = product.Name;
            oldProduct.InActive = !product.IsActive;

            return oldProduct;
        }

        private static void SetChileProductSpec(ChileProduct chileProduct, tblProduct oldProduct)
        {
            foreach(var setRange in new List<SetRange>
                {
                    new SetRange(StaticAttributeNames.Granularity, p => p.GranStart, p => p.GranEnd),
                    new SetRange(StaticAttributeNames.Asta, p => p.AstaStart, p => p.AstaEnd),
                    new SetRange(StaticAttributeNames.AB, p => p.ABStart, p => p.ABEnd),
                    new SetRange(StaticAttributeNames.Scoville, p => p.ScovStart, p => p.ScovEnd),
                    new SetRange(StaticAttributeNames.H2O, p => p.H2OStart, p => p.H2OEnd),
                    new SetRange(StaticAttributeNames.Scan, p => p.ScanStart, p => p.ScanEnd),
                })
            {
                setRange.Set(chileProduct.ProductAttributeRanges.FirstOrDefault(r => r.AttributeShortName == setRange.Attribute.AttributeNameKey_ShortName), oldProduct);
            }
        }

        private class SetRange
        {
            public readonly IAttributeNameKey Attribute;
            private readonly PropertyInfo _min;
            private readonly PropertyInfo _max;

            public SetRange(IAttributeNameKey attribute, Expression<Func<tblProduct, decimal?>> selectMin, Expression<Func<tblProduct, decimal?>> selectMax)
            {
                Attribute = attribute;

                var memberExpression = selectMin.Body as MemberExpression;
                _min = memberExpression.Member as PropertyInfo;

                memberExpression = selectMax.Body as MemberExpression;
                _max = memberExpression.Member as PropertyInfo;
            }

            public void Set(IAttributeRange range, tblProduct oldProduct)
            {
                _min.SetValue(oldProduct, range == null ? null : (decimal?)range.RangeMin);
                _max.SetValue(oldProduct, range == null ? null : (decimal?)range.RangeMax);
            }
        }
    }
}