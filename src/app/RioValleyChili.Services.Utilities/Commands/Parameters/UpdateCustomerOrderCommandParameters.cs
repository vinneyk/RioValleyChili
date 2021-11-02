using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.CustomerService;
using RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class UpdateCustomerOrderCommandParameters : ISetCustomerOrderCommandParameters
    {
        public ISetCustomerOrderParameters Parameters { get; set; }
        public CustomerOrderKey CustomerOrderKey { get; set; }
        public FacilityKey ShipFromFacilityKey { get; set; }
    }
}