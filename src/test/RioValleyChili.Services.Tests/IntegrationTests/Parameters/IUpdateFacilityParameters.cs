using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateFacilityParameters : IUpdateFacilityParameters
    {
        public string UserToken { get; set; }
        public FacilityType FacilityType { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string ShippingLabelName { get; set; }
        public string PhoneNumber { get; set; }
        public string EMailAddress { get; set; }
        public Address Address { get; set; }
        public string FacilityKey { get; set; }
    }
}