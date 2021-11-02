using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetSampleOrderParameters
    {
        internal ISetSampleOrderParameters Parameters;

        internal SampleOrderKey SampleOrderKey;
        internal CustomerKey RequestCustomerKey;
        internal CompanyKey BrokerKey;

        internal List<SetSampleOrderItemParameters> Items;
    }
}