using System;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Models.Parameters
{
    public class SetShippingInstructionsParameters : ISetShippingInstructions
    {
        public SetShippingInstructionsParameters()
        {
            ShipFromOrSoldTo = new ShippingLabel();
            ShipTo = new ShippingLabel();
            FreightBillTo = new ShippingLabel();
        }

        public SetShippingInstructionsParameters(IShippingInstructions shippingInstructions)
        {
            if(shippingInstructions != null)
            {
                RequiredDeliveryDateTime = shippingInstructions.RequiredDeliveryDateTime;
                ShipmentDate = shippingInstructions.ShipmentDate;

                InternalNotes = shippingInstructions.InternalNotes;
                ExternalNotes = shippingInstructions.ExternalNotes;
                SpecialInstructions = shippingInstructions.SpecialInstructions;

                ShipFromOrSoldTo = shippingInstructions.ShipFromOrSoldToShippingLabel;
                ShipTo = shippingInstructions.ShipToShippingLabel;
                FreightBillTo = shippingInstructions.FreightBillToShippingLabel;
            }
        }
        
        public DateTime? RequiredDeliveryDateTime { get; set; }
        public DateTime? ShipmentDate { get; set; }

        public string InternalNotes { get; set; }
        public string ExternalNotes { get; set; }
        public string SpecialInstructions { get; set; }

        public ShippingLabel ShipFromOrSoldTo { get; set; }
        public ShippingLabel ShipTo { get; set; }
        public ShippingLabel FreightBillTo { get; set; }
    }
}