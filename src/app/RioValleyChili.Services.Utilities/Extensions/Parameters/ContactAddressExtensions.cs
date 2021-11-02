using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ContactAddressExtensions
    {
        internal static IResult<SetContactAddressParameters> ToParsedParameters(this IContactAddressReturn contactAddress)
        {
            if(contactAddress == null) { throw new ArgumentNullException("contactAddress"); }

            ContactAddressKey contactAddressKey = null;
            if(contactAddress.ContactAddressKey != null)
            {
                var contactAddressKeyResult = KeyParserHelper.ParseResult<IContactAddressKey>(contactAddress.ContactAddressKey);
                if(!contactAddressKeyResult.Success)
                {
                    return contactAddressKeyResult.ConvertTo<SetContactAddressParameters>();
                }
                contactAddressKey = contactAddressKeyResult.ResultingObject.ToContactAddressKey();
            }

            return new SuccessResult<SetContactAddressParameters>(new SetContactAddressParameters
                {
                    ContactAddress = contactAddress,
                    ContactAddressKey = contactAddressKey
                });
        }
    }
}