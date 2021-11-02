using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ContactExtensions
    {
        internal static IResult<CreateContactCommandParameters> ToParsedParameters(this ICreateContactParameters createContactParameters)
        {
            if(createContactParameters == null) { throw new ArgumentNullException("createContactParameters"); }

            var companyKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(createContactParameters.CompanyKey);
            if(!companyKeyResult.Success)
            {
                return companyKeyResult.ConvertTo<CreateContactCommandParameters>();
            }

            var setAddresses = new List<SetContactAddressParameters>();
            foreach(var address in createContactParameters.Addresses ?? new List<IContactAddressReturn>())
            {
                var addressResult = address.ToParsedParameters();
                if(!addressResult.Success)
                {
                    return addressResult.ConvertTo<CreateContactCommandParameters>();
                }
                setAddresses.Add(addressResult.ResultingObject);
            }

            return new SuccessResult<CreateContactCommandParameters>(new CreateContactCommandParameters
                {
                    Parameters = createContactParameters,
                    CompanyKey = companyKeyResult.ResultingObject.ToCompanyKey(),
                    SetAddresses = setAddresses
                });
        }

        internal static IResult<UpdateContactCommandParameters> ToParsedParameters(this IUpdateContactParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var contactKeyResult = KeyParserHelper.ParseResult<IContactKey>(parameters.ContactKey);
            if(!contactKeyResult.Success)
            {
                return contactKeyResult.ConvertTo<UpdateContactCommandParameters>();
            }

            var updateAddresses = new List<SetContactAddressParameters>();
            if(parameters.Addresses != null)
            {
                foreach(var address in parameters.Addresses)
                {
                    var addressResult = address.ToParsedParameters();
                    if(!addressResult.Success)
                    {
                        return addressResult.ConvertTo<UpdateContactCommandParameters>();
                    }
                    updateAddresses.Add(addressResult.ResultingObject);
                }
            }

            return new SuccessResult<UpdateContactCommandParameters>(new UpdateContactCommandParameters
                {
                    Parameters = parameters,
                    ContactKey = contactKeyResult.ResultingObject.ToContactKey(),
                    SetAddresses = updateAddresses
                });
        }
    }
}