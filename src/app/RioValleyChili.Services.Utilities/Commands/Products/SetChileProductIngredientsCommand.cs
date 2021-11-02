using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Products
{
    internal class SetChileProductIngredientsCommand
    {
        private readonly IProductUnitOfWork _productUnitOfWork;

        internal SetChileProductIngredientsCommand(IProductUnitOfWork productUnitOfWork)
        {
            if(productUnitOfWork == null) { throw new ArgumentNullException("productUnitOfWork"); }
            _productUnitOfWork = productUnitOfWork;
        }

        internal IResult<ChileProduct> Execute(ChileProduct chileProduct, IUserIdentifiable user, DateTime timeStamp, IEnumerable<SetChileProductIngredientCommandParameters> ingredients, out List<AdditiveTypeKey> deletedIngredients)
        {
            deletedIngredients = new List<AdditiveTypeKey>();

            var employeeResult = new GetEmployeeCommand(_productUnitOfWork).GetEmployee(user);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ChileProduct>();
            }

            AdditiveTypeKey[] missingAdditiveTypes;
            _productUnitOfWork.AdditiveTypeRepository.FindByKeys(out missingAdditiveTypes, ingredients.Select(i => i.AdditiveTypeKey).ToArray());
            if(missingAdditiveTypes.Any())
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.AdditiveTypeNotFound, missingAdditiveTypes.First()));
            }

            foreach(var match in (chileProduct.Ingredients ?? new List<ChileProductIngredient>()).BestMatches(ingredients,
                (e, n) => e.AdditiveTypeId == n.AdditiveTypeKey.AdditiveTypeKey_AdditiveTypeId,
                (e, n) => e.Percentage == n.Parameters.Percentage))
            {
                if(match.Item2 == null)
                {
                    deletedIngredients.Add(match.Item1.ToAdditiveTypeKey());
                    _productUnitOfWork.ChileProductIngredientRepository.Remove(match.Item1);
                }
                else
                {
                    bool update;
                    var ingredient = match.Item1;
                    if(ingredient == null || ingredient.AdditiveTypeId != match.Item2.AdditiveTypeKey.AdditiveTypeKey_AdditiveTypeId)
                    {
                        if(ingredient != null)
                        {
                            deletedIngredients.Add(ingredient.ToAdditiveTypeKey());
                            _productUnitOfWork.ChileProductIngredientRepository.Remove(ingredient);
                        }

                        update = true;
                        ingredient = _productUnitOfWork.ChileProductIngredientRepository.Add(new ChileProductIngredient
                        {
                            ChileProductId = chileProduct.Id,
                            AdditiveTypeId = match.Item2.AdditiveTypeKey.AdditiveTypeKey_AdditiveTypeId
                        });
                    }
                    else
                    {
                        update = ingredient.Percentage != match.Item2.Parameters.Percentage;
                    }

                    if(update)
                    {
                        ingredient.EmployeeId = employeeResult.ResultingObject.EmployeeId;
                        ingredient.TimeStamp = timeStamp;
                        ingredient.Percentage = match.Item2.Parameters.Percentage;
                    }
                }
            }

            return new SuccessResult().ConvertTo(chileProduct);
        }

        internal IResult Execute(DateTime timestamp, SetChileProductIngredientsCommandParameters parameters, out List<AdditiveTypeKey> deletedIngredients)
        {
            deletedIngredients = null;

            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileProduct = _productUnitOfWork.ChileProductRepository.FindByKey(parameters.ChileProductKey,
                c => c.Ingredients);
            if(chileProduct == null)
            {
                return new InvalidResult(string.Format(UserMessages.ChileProductNotFound, parameters.ChileProductKey));
            }

            return Execute(chileProduct, parameters.Parameters, timestamp, parameters.Ingredients, out deletedIngredients);
        }
    }
}