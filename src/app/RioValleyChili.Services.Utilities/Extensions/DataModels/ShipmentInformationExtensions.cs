using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;

namespace RioValleyChili.Services.Utilities.Extensions.DataModels
{
    internal static class ShipmentInformationExtensions
    {
        internal static void SetShipmentInformation(this ShipmentInformation shipmentInformation, IShipmentDetailReturn shipment)
        {
            if(shipmentInformation == null) { throw new ArgumentNullException("shipmentInformation"); }

            if(shipmentInformation.ShipFrom == null)
            {
                shipmentInformation.ShipFrom = new ShippingLabel();
            }
            if(shipmentInformation.ShipTo == null)
            {
                shipmentInformation.ShipTo = new ShippingLabel();
            }
            if(shipmentInformation.FreightBill == null)
            {
                shipmentInformation.FreightBill = new ShippingLabel();
            }

            if(shipment == null)
            {
                shipmentInformation.Status = ShipmentStatus.Unscheduled;
                shipmentInformation.PalletWeight = 0.0;
                shipmentInformation.PalletQuantity = 0;

                shipmentInformation.SetShippingInstructions(null);
                shipmentInformation.SetTransitInformation((ITransitInformation) null);
            }
            else
            {
                shipmentInformation.Status = shipment.Status;
                shipmentInformation.PalletWeight = shipment.PalletWeight;
                shipmentInformation.PalletQuantity = shipment.PalletQuantity;

                shipmentInformation.SetShippingInstructions(shipment.ShippingInstructions);
                shipmentInformation.SetTransitInformation(shipment.TransitInformation);
            }
        }

        internal static ShippingLabel SetShippingLabel(this ShippingLabel shippingLabel, ShippingLabel shippingLabelSource)
        {
            if(shippingLabel == null) { throw new ArgumentNullException("shippingLabel"); }

            if(shippingLabelSource != null)
            {
                shippingLabel.Name = shippingLabelSource.Name;
                shippingLabel.Phone = shippingLabelSource.Phone;
                shippingLabel.EMail = shippingLabelSource.EMail;
                shippingLabel.Fax = shippingLabelSource.Fax;
                shippingLabel.Address = shippingLabelSource.Address ?? new Address();
            }
            else
            {
                shippingLabel.Name = null;
                shippingLabel.Phone = null;
                shippingLabel.EMail = null;
                shippingLabel.Fax = null;
                shippingLabel.Address = new Address();
            }

            return shippingLabel;
        }

        internal static void SetShipmentInformation(this ShipmentInformation shipmentInformation, ISetShipmentInformationWithStatus shipment)
        {
            if(shipmentInformation == null) { throw new ArgumentNullException("shipmentInformation"); }

            shipmentInformation.SetShipmentInformation(shipment as ISetShipmentInformation);
            shipmentInformation.Status = shipment == null ? ShipmentStatus.Unscheduled : shipment.ShipmentStatus;
        }

        internal static void SetShipmentInformation(this ShipmentInformation shipmentInformation, ISetShipmentInformation shipment, bool setShipFrom = true)
        {
            if(shipmentInformation == null) { throw new ArgumentNullException("shipmentInformation"); }

            if(shipment == null)
            {
                shipmentInformation.Status = ShipmentStatus.Unscheduled;
                shipmentInformation.PalletWeight = null;
                shipmentInformation.PalletQuantity = 0;
                shipmentInformation.ExternalNotes = null;
                shipmentInformation.InternalNotes = null;
                shipmentInformation.SpecialInstructions = null;

                shipmentInformation.SetShippingInstructions(null, setShipFrom);
                shipmentInformation.SetTransitInformation((ISetTransitInformation) null);
            }
            else
            {
                shipmentInformation.PalletWeight = shipment.PalletWeight;
                shipmentInformation.PalletQuantity = shipment.PalletQuantity;
                
                shipmentInformation.SetShippingInstructions(shipment.ShippingInstructions, setShipFrom);
                shipmentInformation.SetTransitInformation(shipment.TransitInformation);
            }
        }

        private static void SetTransitInformation(this ShipmentInformation shipmentInformation, ITransitInformation transitInformation)
        {
            if(transitInformation == null)
            {
                shipmentInformation.FreightBillType = null;
                shipmentInformation.DriverName = null;
                shipmentInformation.CarrierName = null;
                shipmentInformation.TrailerLicenseNumber = null;
                shipmentInformation.ContainerSeal = null;
            }
            else
            {
                shipmentInformation.FreightBillType = transitInformation.FreightType;
                shipmentInformation.DriverName = transitInformation.DriverName;
                shipmentInformation.CarrierName = transitInformation.CarrierName;
                shipmentInformation.TrailerLicenseNumber = transitInformation.TrailerLicenseNumber;
                shipmentInformation.ContainerSeal = transitInformation.ContainerSeal;
            }
        }

        private static void SetShippingInstructions(this ShipmentInformation shipmentInformation, IShippingInstructions shippingInstructions)
        {
            if(shipmentInformation == null) { throw new ArgumentNullException("shipmentInformation"); }

            ShippingLabel shipFrom = null;
            ShippingLabel shipTo = null;
            ShippingLabel freightBill = null;

            if(shippingInstructions == null)
            {
                shipmentInformation.RequiredDeliveryDate = null;
                shipmentInformation.InternalNotes = null;
                shipmentInformation.ExternalNotes = null;
                shipmentInformation.SpecialInstructions = null;
            }
            else
            {
                shipFrom = shippingInstructions.ShipFromOrSoldToShippingLabel;
                shipTo = shippingInstructions.ShipToShippingLabel;
                freightBill = shippingInstructions.FreightBillToShippingLabel;

                shipmentInformation.RequiredDeliveryDate = shippingInstructions.RequiredDeliveryDateTime.HasValue ? shippingInstructions.RequiredDeliveryDateTime.Value.Date : (DateTime?)null;
                shipmentInformation.InternalNotes = shippingInstructions.InternalNotes;
                shipmentInformation.ExternalNotes = shippingInstructions.ExternalNotes;
                shipmentInformation.SpecialInstructions = shippingInstructions.SpecialInstructions;
            }

            shipmentInformation.ShipFrom.SetShippingLabel(shipFrom);
            shipmentInformation.ShipTo.SetShippingLabel(shipTo);
            shipmentInformation.FreightBill.SetShippingLabel(freightBill);
        }

        private static void SetShippingInstructions(this ShipmentInformation shipInfo, ISetShippingInstructions shippingInstructions, bool setShipFrom)
        {
            if(shipInfo == null) { throw new ArgumentNullException("shipInfo"); }

            if(shippingInstructions == null)
            {
                shipInfo.RequiredDeliveryDate = null;
                shipInfo.ShipmentDate = null;

                shipInfo.InternalNotes = null;
                shipInfo.ExternalNotes = null;
                shipInfo.SpecialInstructions = null;

                shipInfo.ShipFrom = new ShippingLabel();
                shipInfo.ShipTo = new ShippingLabel();
                shipInfo.FreightBill = new ShippingLabel();
            }
            else
            {
                shipInfo.RequiredDeliveryDate = shippingInstructions.RequiredDeliveryDateTime;
                shipInfo.ShipmentDate = shippingInstructions.ShipmentDate;

                shipInfo.InternalNotes = shippingInstructions.InternalNotes;
                shipInfo.ExternalNotes = shippingInstructions.ExternalNotes;
                shipInfo.SpecialInstructions = shippingInstructions.SpecialInstructions;

                shipInfo.ShipFrom = (setShipFrom ? shippingInstructions.ShipFromOrSoldTo : null) ?? new ShippingLabel();
                shipInfo.ShipTo = shippingInstructions.ShipTo ?? new ShippingLabel();
                shipInfo.FreightBill = shippingInstructions.FreightBillTo ?? new ShippingLabel();
            }
        }

        private static void SetTransitInformation(this ShipmentInformation shipInfo, ISetTransitInformation transitInformation)
        {
            if(shipInfo == null) { throw new ArgumentNullException("shipInfo"); }

            if(transitInformation == null)
            {
                shipInfo.FreightBillType = null;
                shipInfo.ShipmentMethod = null;
                shipInfo.DriverName = null;
                shipInfo.CarrierName = null;
                shipInfo.TrailerLicenseNumber = null;
                shipInfo.ContainerSeal = null;
            }
            else
            {
                shipInfo.FreightBillType = transitInformation.FreightBillType;
                shipInfo.ShipmentMethod = transitInformation.ShipmentMethod;
                shipInfo.DriverName = transitInformation.DriverName;
                shipInfo.CarrierName = transitInformation.CarrierName;
                shipInfo.TrailerLicenseNumber = transitInformation.TrailerLicenseNumber;
                shipInfo.ContainerSeal = transitInformation.ContainerSeal;
            }
        }
    }
}