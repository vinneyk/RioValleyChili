using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal class GetCompanyCommand
    {
        private ICompanyUnitOfWork _companyUnitOfWork;

        internal GetCompanyCommand(ICompanyUnitOfWork companyUnitOfWork)
        {
            if(companyUnitOfWork == null) {  throw new ArgumentNullException("companyUnitOfWork"); }
            _companyUnitOfWork = companyUnitOfWork;
        }

        internal IResult<Data.Models.Company> Execute(ICompanyKey companyKey, CompanyType? companyType = null)
        {
            if(companyKey == null) { throw new ArgumentNullException("companyKey"); }
            var parsedKey = new CompanyKey(companyKey);

            var company = _companyUnitOfWork.CompanyRepository.FindByKey(parsedKey, c => c.CompanyTypes);
            if(company == null)
            {
                return new InvalidResult<Data.Models.Company>(null, string.Format(UserMessages.CompanyNotFound, parsedKey));
            }

            if(companyType != null)
            {
                if(company.CompanyTypes.All(t => t.CompanyTypeEnum != companyType))
                {
                    return new InvalidResult<Data.Models.Company>(null, string.Format(UserMessages.CompanyNotOfType, companyKey, companyType));
                }
            }

            return new SuccessResult<Data.Models.Company>(company);
        }
    }
}