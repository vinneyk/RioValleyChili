using System;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetShippingInstructions : ISetShippingInstructions
    {
        public DateTime? RequiredDeliveryDateTime { get; set; }
        public DateTime? ShipmentDate { get; set; }

        [StringLength(Constants.StringLengths.ShipmentInformationNotes)]
        public string InternalNotes { get; set; }
        [StringLength(Constants.StringLengths.ShipmentInformationNotes)]
        public string ExternalNotes { get; set; }
        [StringLength(Constants.StringLengths.ShipmentInformationNotes)]
        public string SpecialInstructions { get; set; }

        public ShippingLabel ShipFromOrSoldTo { get; set; }
        public ShippingLabel ShipTo { get; set; }
        public ShippingLabel FreightBillTo { get; set; }
    }
}