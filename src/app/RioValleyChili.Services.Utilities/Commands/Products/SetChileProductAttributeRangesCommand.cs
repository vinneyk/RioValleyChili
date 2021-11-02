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
    internal class SetChileProductAttributeRangesCommand
    {
        private readonly IProductUnitOfWork _productUnitOfWork;

        internal SetChileProductAttributeRangesCommand(IProductUnitOfWork productUnitOfWork)
        {
            if(productUnitOfWork == null) { throw new ArgumentNullException("productUnitOfWork"); }
            _productUnitOfWork = productUnitOfWork;
        }

        internal IResult<ChileProduct> Execute(ChileProduct chileProduct, IUserIdentifiable user, DateTime timeStamp, IEnumerable<SetChileProductAttributeRangeParameters> ranges)
        {
            var employeeResult = new GetEmployeeCommand(_productUnitOfWork).GetEmployee(user);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ChileProduct>();
            }

            AttributeNameKey[] notFound;
            var attributeNames = _productUnitOfWork.AttributeNameRepository.FindByKeys(out notFound, ranges.Select(r => r.AttributeNameKey).ToArray());
            if(notFound.Any())
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.AttributeNameNotFound, notFound.First()));
            }

            var invalidForChile = attributeNames.FirstOrDefault(n => !n.ValidForChileInventory);
            if(invalidForChile != null)
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.AttributeNameNotValidForChile, invalidForChile.ToAttributeNameKey()));
            }

            foreach(var match in (chileProduct.ProductAttributeRanges ?? new List<ChileProductAttributeRange>()).BestMatches(ranges,
                (e, n) => e.AttributeShortName == n.AttributeNameKey.AttributeNameKey_ShortName,
                (e, n) => e.RangeMin == n.Parameters.RangeMin,
                (e, n) => e.RangeMax == n.Parameters.RangeMax))
            {
                if(match.Item2 == null)
                {
                    _productUnitOfWork.ChileProductAttributeRangeRepository.Remove(match.Item1);
                }
                else
                {
                    bool update;
                    var range = match.Item1;
                    if(range == null || !match.Item2.AttributeNameKey.Equals(range))
                    {
                        if(range != null)
                        {
                            _productUnitOfWork.ChileProductAttributeRangeRepository.Remove(range);
                        }

                        update = true;
                        range = _productUnitOfWork.ChileProductAttributeRangeRepository.Add(new ChileProductAttributeRange
                            {
                                ChileProductId = chileProduct.Id,
                                AttributeShortName = match.Item2.AttributeNameKey.AttributeNameKey_ShortName
                            });
                    }
                    else
                    {
                        update = range.RangeMin != match.Item2.Parameters.RangeMin || range.RangeMax != match.Item2.Parameters.RangeMax;
                    }

                    if(update)
                    {
                        range.EmployeeId = employeeResult.ResultingObject.EmployeeId;
                        range.TimeStamp = timeStamp;
                        range.RangeMin = match.Item2.Parameters.RangeMin;
                        range.RangeMax = match.Item2.Parameters.RangeMax;
                    }
                }
            }

            return new SuccessResult<ChileProduct>(chileProduct);
        }

        internal IResult<ChileProduct> Execute(SetChileProductAttributeRangesCommandParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var chileProduct = _productUnitOfWork.ChileProductRepository.FindByKey(parameters.ChileProductKey, c => c.ProductAttributeRanges);
            if(chileProduct == null)
            {
                return new InvalidResult<ChileProduct>(null, string.Format(UserMessages.ChileProductNotFound, parameters.ChileProductKey.KeyValue));
            }

            return Execute(chileProduct, parameters.Parameters, timeStamp, parameters.AttributeRanges);
        }
    }
}