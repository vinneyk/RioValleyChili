using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CompanyService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateContactCommandParameters : ContactCommandParametersBase
    {
        internal ICreateContactParameters Parameters { get; set; }

        internal CompanyKey CompanyKey { get; set; }

        internal override IContactParameters BaseParameters { get { return Parameters; } }
    }

    internal class UpdateContactCommandParameters : ContactCommandParametersBase
    {
        internal IUpdateContactParameters Parameters { get; set; }

        internal ContactKey ContactKey { get; set; }

        internal override IContactParameters BaseParameters { get { return Parameters; } }
    }

    internal abstract class ContactCommandParametersBase
    {
        internal abstract IContactParameters BaseParameters { get; }

        internal List<SetContactAddressParameters> SetAddresses { get; set; }
    }
}