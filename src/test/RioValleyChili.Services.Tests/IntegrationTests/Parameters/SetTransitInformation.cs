using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetTransitInformation : ISetTransitInformation
    {
        [StringLength(Constants.StringLengths.FreightBillType)]
        public string FreightBillType { get; set; }

        [StringLength(Constants.StringLengths.ShipmentMethod)]
        public string ShipmentMethod { get; set; }

        [StringLength(Constants.StringLengths.DriverName)]
        public string DriverName { get; set; }

        [StringLength(Constants.StringLengths.CarrierName)]
        public string CarrierName { get; set; }

        [StringLength(Constants.StringLengths.TrailerLicenseNumber)]
        public string TrailerLicenseNumber { get; set; }

        [StringLength(Constants.StringLengths.ContainerSeal)]
        public string ContainerSeal { get; set; }
    }
}