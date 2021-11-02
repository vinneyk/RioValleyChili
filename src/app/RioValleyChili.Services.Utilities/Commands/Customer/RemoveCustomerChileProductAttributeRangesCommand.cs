using System;
using System.Linq;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Customer
{
    internal class RemoveCustomerChileProductAttributeRangesCommand
    {
        private readonly ISalesUnitOfWork _salesUnitOfWork;

        public RemoveCustomerChileProductAttributeRangesCommand(ISalesUnitOfWork salesUnitOfWork)
        {
            if(salesUnitOfWork == null) { throw new ArgumentNullException("salesUnitOfWork"); }
            _salesUnitOfWork = salesUnitOfWork;
        }

        public IResult Execute(ICustomerKey customerKey, IChileProductKey chileProductKey, out int? prodId, out string companyIA)
        {
            prodId = null;
            companyIA = null;

            var predicate = CustomerProductAttributeRangePredicates.ByCustomerProduct(customerKey, chileProductKey);
            foreach(var attributeRange in _salesUnitOfWork.CustomerProductAttributeRangeRepository.Filter(predicate,
                r => r.Customer.Company,
                r => r.ChileProduct.Product).ToList())
            {
                int parsed;
                if(int.TryParse(attributeRange.ChileProduct.Product.ProductCode, out parsed))
                {
                    prodId = parsed;
                }

                companyIA = !string.IsNullOrWhiteSpace(attributeRange.Customer.Company.Name) ? attributeRange.Customer.Company.Name : companyIA;

                _salesUnitOfWork.CustomerProductAttributeRangeRepository.Remove(attributeRange);
            }

            return new SuccessResult();
        }
    }
}