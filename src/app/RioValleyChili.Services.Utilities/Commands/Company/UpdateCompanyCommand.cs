using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal class UpdateCompanyCommand : CompanyCommandBase
    {
        internal UpdateCompanyCommand(ICompanyUnitOfWork companyUnitOfWork) : base(companyUnitOfWork) { }

        internal IResult Execute(DateTime timestamp, UpdateCompanyCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var company = CompanyUnitOfWork.CompanyRepository.FindByKey(parameters.CompanyKey,
                c => c.Customer,
                c => c.CompanyTypes);
            if(company == null)
            {
                return new InvalidResult(string.Format(UserMessages.CompanyNotFound, parameters.CompanyKey.KeyValue));
            }

            return SetCompany(company, timestamp, parameters);
        }
    }
}