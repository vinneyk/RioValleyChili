using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class ChileProductIngredientsMother : EntityMotherLogBase<ChileProductIngredient, ChileProductIngredientsMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public ChileProductIngredientsMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        private enum EntityTypes
        {
            ChileProductIngredient
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<ChileProductIngredient> BirthRecords()
        {
            _loadCount.Reset();

            var ingredients = SelectProductIngredientsToLoad(OldContext);
            foreach(var ingredient in ingredients)
            {
                _loadCount.AddRead(EntityTypes.ChileProductIngredient);

                if(ingredient.Ingredient == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullIngredient)
                        {
                            Ingredient = ingredient
                        });
                    continue;
                }

                if(ingredient.Product == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullProduct)
                        {
                            Ingredient = ingredient
                        });
                    continue;
                }

                var chileProduct = _newContextHelper.GetChileProduct(ingredient.Product.ProdID);
                if(chileProduct == null)
                {
                    Log(new CallbackParameters(CallbackReason.ChileProductNotLoaded)
                        {
                            Ingredient = ingredient
                        });
                    continue;
                }

                var additiveType = _newContextHelper.GetAdditiveType(ingredient.Ingredient.IngrDesc);
                if(additiveType == null)
                {
                    Log(new CallbackParameters(CallbackReason.AdditiveTypeNotLoaded)
                        {
                            Ingredient = ingredient
                        });
                    continue;
                }

                _loadCount.AddLoaded(EntityTypes.ChileProductIngredient);

                yield return new ChileProductIngredient
                    {
                        EmployeeId = ingredient.EmployeeID ?? _newContextHelper.DefaultEmployee.EmployeeId,
                        TimeStamp = ingredient.EntryDate.ConvertLocalToUTC(),

                        ChileProductId = chileProduct.Id,
                        AdditiveTypeId = additiveType.Id,

                        Percentage = (double) (ingredient.Percentage ?? default(decimal))
                    };
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        public List<tblProductIngrDTO> SelectProductIngredientsToLoad(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblProductIngr>().Select(i => new tblProductIngrDTO
                {
                    EmployeeID = i.EmployeeID,
                    EntryDate = i.EntryDate,
                    Percentage = i.Percentage,
                    Ingredient = new[] { i.tblIngredient }.Where(n => n != null).Select(n => new tblIngredientDTO
                        {
                            IngrDesc = n.IngrDesc
                        }).FirstOrDefault(),
                    Product = new[] { i.tblProduct }.Where(n => n != null).Select(p => new tblProductDTO
                        {
                            ProdID = p.ProdID,
                            ProductName = p.Product
                        }).FirstOrDefault()
                }).ToList();
        }

        #region DTOs

        public class tblProductIngrDTO
        {
            public int? EmployeeID { get; set; }

            public decimal? Percentage { get; set; }

            public tblIngredientDTO Ingredient { get; set; }

            public tblProductDTO Product { get; set; }

            public DateTime EntryDate { get; set; }
        }

        public class tblIngredientDTO
        {
            public string IngrDesc { get; set; }
        }

        public class tblProductDTO
        {
            public int ProdID { get; set; }

            public string ProductName { get; set; }
        }

        #endregion

        public enum CallbackReason
        {
            Exception,
            Summary,
            NullIngredient,
            NullProduct,
            ChileProductNotLoaded,
            AdditiveTypeNotLoaded,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public tblProductIngrDTO Ingredient { get; set; }

            protected override CallbackReason ExceptionReason { get { return ChileProductIngredientsMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return ChileProductIngredientsMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return ChileProductIngredientsMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case ChileProductIngredientsMother.CallbackReason.Exception: return ReasonCategory.Error;
                        
                    case ChileProductIngredientsMother.CallbackReason.NullIngredient:
                    case ChileProductIngredientsMother.CallbackReason.NullProduct:
                    case ChileProductIngredientsMother.CallbackReason.ChileProductNotLoaded:
                    case ChileProductIngredientsMother.CallbackReason.AdditiveTypeNotLoaded: return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}