using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public class SyncChileProductHelper
    {
        public SyncChileProductHelper(RioAccessSQLEntities oldContext)
        {
            _oldContext = oldContext;
        }

        public tblProduct GetProduct(int prodId)
        {
            return _oldContext.tblProducts.Select(p => new
                {
                    tblProduct = p,
                    productIngredients = p.tblProductIngrs.Select(i => new
                        {
                            ingredient = i,
                            i.tblIngredient
                        }),
                })
                              .Where(p => p.tblProduct.ProdID == prodId)
                              .ToList()
                              .Select(r => r.tblProduct)
                              .FirstOrDefault();
        }

        public void SyncIngredients(ChileProduct chileProduct, tblProduct oldProduct, List<AdditiveTypeKey> deletedIngredients)
        {
            var entryDate = DateTime.Now.RoundMillisecondsForSQL();
            foreach(var ingredient in chileProduct.Ingredients)
            {
                var oldIngredient = oldProduct.tblProductIngrs.FirstOrDefault(i => i.IngrID == ingredient.AdditiveTypeId);
                if(oldIngredient == null)
                {
                    oldIngredient = new tblProductIngr
                        {
                            EntryDate = entryDate,
                            EmployeeID = ingredient.EmployeeId,
                            ProdID = oldProduct.ProdID,
                            IngrID = ingredient.AdditiveTypeId,
                            s_GUID = Guid.NewGuid()
                        };
                    entryDate = entryDate.AddSeconds(1);
                    _oldContext.tblProductIngrs.AddObject(oldIngredient);
                }

                oldIngredient.Percentage = (decimal?)ingredient.Percentage;
            }

            foreach(var deleted in deletedIngredients ?? new List<AdditiveTypeKey>())
            {
                var oldIngredient = oldProduct.tblProductIngrs.FirstOrDefault(i => i.IngrID == deleted.AdditiveTypeKey_AdditiveTypeId);
                if(oldIngredient != null)
                {
                    _oldContext.tblProductIngrs.DeleteObject(oldIngredient);
                }
            }
        }

        private readonly RioAccessSQLEntities _oldContext;
    }
}