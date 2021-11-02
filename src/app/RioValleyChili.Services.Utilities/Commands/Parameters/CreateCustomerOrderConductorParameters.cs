using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CustomerService;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateCustomerOrderConductorParameters : ISetCustomerOrderCommandParameters
    {
        public ISetCustomerOrderParameters Parameters { get; set; }
        internal CustomerKey CustomerKey { get; set; }
        public FacilityKey ShipFromFacilityKey { get; set; }

        internal List<SetCustomerOrderItemParameters> OrderItems { get; set; }
    }
}