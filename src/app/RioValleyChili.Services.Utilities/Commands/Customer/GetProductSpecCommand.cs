using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class GetProductSpecCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        public GetProductSpecCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        public IResult Execute(IChileProductKey product, ICustomerKey customer, out IDictionary<AttributeNameKey, ChileProductAttributeRange> productSpec, out IDictionary<AttributeNameKey, CustomerProductAttributeRange> customerSpec)
        {
            productSpec = null;
            customerSpec = null;

            var customerPredicate = customer == null ? c => false : customer.ToCustomerKey().FindByPredicate;
            var chileProduct = _lotUnitOfWork.ChileProductRepository.Filter(product.ToChileProductKey().FindByPredicate)
                .AsExpandable()
                .Select(c => new
                    {
                        attributeRanges = c.ProductAttributeRanges,
                        customerRanges = c.CustomerProductAttributeRanges.Where(r => customerPredicate.Invoke(r.Customer) && r.Active)
                    })
                .FirstOrDefault();
            if(chileProduct == null)
            {
                return new InvalidResult<IDictionary<IAttributeNameKey, IAttributeRange>>(null, string.Format(UserMessages.ChileProductNotFound, product.ToChileProductKey()));
            }

            productSpec = chileProduct.attributeRanges.ToDictionary(r => r.ToAttributeNameKey(), r => r);
            customerSpec = chileProduct.customerRanges.ToDictionary(r => r.ToAttributeNameKey(), r => r);

            return new SuccessResult();
        }
    }
}