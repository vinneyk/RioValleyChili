using System;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Company
{
    internal class CreateCompanyCommand : CompanyCommandBase
    {
        internal CreateCompanyCommand(ICompanyUnitOfWork companyUnitOfWork) : base(companyUnitOfWork) { }

        internal IResult<Data.Models.Company> Execute(DateTime timeStamp, CreateCompanyCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var nextId = new EFUnitOfWorkHelper(CompanyUnitOfWork).GetNextSequence<Data.Models.Company>(c => true, c => c.Id);
            var company = CompanyUnitOfWork.CompanyRepository.Add(new Data.Models.Company
                {
                    TimeStamp = timeStamp,
                    Id = nextId,

                    Name = parameters.Parameters.CompanyName,
                    Active = parameters.Parameters.Active
                });

            return SetCompany(company, timeStamp, parameters);
        }
    }
}