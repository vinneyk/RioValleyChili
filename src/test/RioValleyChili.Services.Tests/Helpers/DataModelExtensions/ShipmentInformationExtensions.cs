using System;
using NUnit.Framework;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class ShipmentInformationExtensions
    {
        internal static void AssertEqual(this ShipmentInformation expected, ISetShipmentInformationWithStatus setShipmentInformation)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            expected.AssertEqual((ISetShipmentInformation)setShipmentInformation);
            if(setShipmentInformation == null)
            {
                Assert.AreEqual(ShipmentStatus.Unscheduled, expected.Status);
            }
            else
            {
                Assert.AreEqual(setShipmentInformation.ShipmentStatus, expected.Status);
            }
        }

        internal static void AssertEqual(this ShipmentInformation expected, ISetShipmentInformation setShipmentInformation, ShippingLabel soldTo = null)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            if(setShipmentInformation == null)
            {
                Assert.AreEqual(ShipmentStatus.Unscheduled, expected.Status);
                Assert.AreEqual(0.0, expected.PalletWeight);
                Assert.AreEqual(0, expected.PalletQuantity);
                Assert.IsNull(expected.InternalNotes);
                Assert.IsNull(expected.ExternalNotes);
                Assert.IsNull(expected.SpecialInstructions);

                expected.AssertEqual((ISetTransitInformation)null);
                expected.AssertEqual((ISetShippingInstructions)null, soldTo);
            }
            else
            {
                Assert.AreEqual(setShipmentInformation.PalletWeight, expected.PalletWeight);
                Assert.AreEqual(setShipmentInformation.PalletQuantity, expected.PalletQuantity);

                expected.AssertEqual(setShipmentInformation.TransitInformation);
                expected.AssertEqual(setShipmentInformation.ShippingInstructions, soldTo);
            }
        }

        internal static void AssertEqual(this ShipmentInformation expected, IShipmentInformationReturn result, ShippingLabel soldTo = null)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            if(result == null)
            {
                Assert.AreEqual(ShipmentStatus.Unscheduled, expected.Status);
                Assert.AreEqual(0.0, expected.PalletWeight);
                Assert.AreEqual(0, expected.PalletQuantity);
                Assert.IsNull(expected.InternalNotes);
                Assert.IsNull(expected.ExternalNotes);
                Assert.IsNull(expected.SpecialInstructions);

                expected.AssertEqual((ISetTransitInformation)null);
                expected.AssertEqual((ISetShippingInstructions)null, soldTo);
            }
            else
            {
                Assert.AreEqual(result.PalletWeight, expected.PalletWeight);
                Assert.AreEqual(result.PalletQuantity, expected.PalletQuantity);

                expected.AssertEqual(result.TransitInformation);
                expected.AssertEqual(result.ShippingInstructions, soldTo);
            }
        }

        internal static void AssertEqual(this ShipmentInformation expected, ITransitInformation result)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            if(result == null)
            {
                Assert.IsNull(expected.FreightBillType);
                Assert.IsNull(expected.ShipmentMethod);
                Assert.IsNull(expected.DriverName);
                Assert.IsNull(expected.CarrierName);
                Assert.IsNull(expected.TrailerLicenseNumber);
                Assert.IsNull(expected.ContainerSeal);
            }
            else
            {
                Assert.AreEqual(result.FreightType, expected.FreightBillType);
                Assert.AreEqual(result.ShipmentMethod, expected.ShipmentMethod);
                Assert.AreEqual(result.DriverName, expected.DriverName);
                Assert.AreEqual(result.CarrierName, expected.CarrierName);
                Assert.AreEqual(result.TrailerLicenseNumber, expected.TrailerLicenseNumber);
                Assert.AreEqual(result.ContainerSeal, expected.ContainerSeal);
            }
        }

        internal static void AssertEqual(this ShipmentInformation expected, IShippingInstructions shippingInstructions, ShippingLabel soldTo = null)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            if(shippingInstructions == null)
            {
                Assert.IsNull(expected.RequiredDeliveryDate);
                Assert.IsNull(expected.ShipmentDate);
                Assert.IsNull(expected.InternalNotes);
                Assert.IsNull(expected.ExternalNotes);
                Assert.IsNull(expected.SpecialInstructions);

                expected.ShipFrom.AssertEqual(null);
                expected.ShipTo.AssertEqual(null);
                expected.FreightBill.AssertEqual(null);
            }
            else
            {
                Assert.AreEqual(shippingInstructions.RequiredDeliveryDateTime, expected.RequiredDeliveryDate);
                Assert.AreEqual(shippingInstructions.ShipmentDate, expected.ShipmentDate);
                Assert.AreEqual(shippingInstructions.InternalNotes, expected.InternalNotes);
                Assert.AreEqual(shippingInstructions.ExternalNotes, expected.ExternalNotes);
                Assert.AreEqual(shippingInstructions.SpecialInstructions, expected.SpecialInstructions);

                if(soldTo != null)
                {
                    soldTo.AssertEqual(shippingInstructions.ShipFromOrSoldToShippingLabel);
                }
                else
                {
                    expected.ShipFrom.AssertEqual(shippingInstructions.ShipFromOrSoldToShippingLabel);
                }
                expected.ShipTo.AssertEqual(shippingInstructions.ShipToShippingLabel);
                expected.FreightBill.AssertEqual(shippingInstructions.FreightBillToShippingLabel);
            }
        }

        internal static void AssertEqual(this ShipmentInformation expected, ISetTransitInformation transitInformation)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            if(transitInformation == null)
            {
                Assert.IsNull(expected.FreightBillType);
                Assert.IsNull(expected.ShipmentMethod);
                Assert.IsNull(expected.DriverName);
                Assert.IsNull(expected.CarrierName);
                Assert.IsNull(expected.TrailerLicenseNumber);
                Assert.IsNull(expected.ContainerSeal);
            }
            else
            {
                Assert.AreEqual(transitInformation.FreightBillType, expected.FreightBillType);
                Assert.AreEqual(transitInformation.ShipmentMethod, expected.ShipmentMethod);
                Assert.AreEqual(transitInformation.DriverName, expected.DriverName);
                Assert.AreEqual(transitInformation.CarrierName, expected.CarrierName);
                Assert.AreEqual(transitInformation.TrailerLicenseNumber, expected.TrailerLicenseNumber);
                Assert.AreEqual(transitInformation.ContainerSeal, expected.ContainerSeal);
            }
        }

        internal static void AssertEqual(this ShipmentInformation expected, ISetShippingInstructions shippingInstructions, ShippingLabel soldTo = null)
        {
            if(expected == null) { throw new ArgumentNullException("expected"); }

            if(shippingInstructions == null)
            {
                Assert.IsNull(expected.RequiredDeliveryDate);
                Assert.IsNull(expected.ShipmentDate);
                Assert.IsNull(expected.InternalNotes);
                Assert.IsNull(expected.ExternalNotes);
                Assert.IsNull(expected.SpecialInstructions);

                expected.ShipFrom.AssertEqual(null);
                expected.ShipTo.AssertEqual(null);
                expected.FreightBill.AssertEqual(null);
            }
            else
            {
                Assert.AreEqual(shippingInstructions.RequiredDeliveryDateTime, expected.RequiredDeliveryDate);
                Assert.AreEqual(shippingInstructions.ShipmentDate, expected.ShipmentDate);
                Assert.AreEqual(shippingInstructions.InternalNotes, expected.InternalNotes);
                Assert.AreEqual(shippingInstructions.ExternalNotes, expected.ExternalNotes);
                Assert.AreEqual(shippingInstructions.SpecialInstructions, expected.SpecialInstructions);

                if(soldTo != null)
                {
                    soldTo.AssertEqual(shippingInstructions.ShipFromOrSoldTo);
                }
                else
                {
                    expected.ShipFrom.AssertEqual(shippingInstructions.ShipFromOrSoldTo);
                }
                expected.ShipTo.AssertEqual(shippingInstructions.ShipTo);
                expected.FreightBill.AssertEqual(shippingInstructions.FreightBillTo);
            }
        }

        internal static void AssertEqual(this ShippingLabel shippingLabel, ShippingLabel label)
        {
            if(shippingLabel == null) { throw new ArgumentNullException("shippingLabel"); }

            if(label == null)
            {
                Assert.IsNull(shippingLabel.Name);
                Assert.IsNull(shippingLabel.Phone);
                Assert.IsNull(shippingLabel.EMail);
                Assert.IsNull(shippingLabel.Fax);

                shippingLabel.Address.AssertEqual(null);
            }
            else
            {
                Assert.AreEqual(label.Name, shippingLabel.Name);
                Assert.AreEqual(label.Phone, shippingLabel.Phone);
                Assert.AreEqual(label.EMail, shippingLabel.EMail);
                Assert.AreEqual(label.Fax, shippingLabel.Fax);

                shippingLabel.Address.AssertEqual(label.Address);
            }
        }

        internal static void AssertEqual(this Address address, Address otherAddress)
        {
            if(address == null) { throw new ArgumentNullException("address"); }

            if(otherAddress == null)
            {
                Assert.IsNull(address.AddressLine1);
                Assert.IsNull(address.AddressLine2);
                Assert.IsNull(address.AddressLine3);
                Assert.IsNull(address.City);
                Assert.IsNull(address.State);
                Assert.IsNull(address.PostalCode);
                Assert.IsNull(address.Country);
            }
            else
            {
                Assert.AreEqual(otherAddress.AddressLine1, address.AddressLine1);
                Assert.AreEqual(otherAddress.AddressLine2, address.AddressLine2);
                Assert.AreEqual(otherAddress.AddressLine3, address.AddressLine3);
                Assert.AreEqual(otherAddress.City, address.City);
                Assert.AreEqual(otherAddress.State, address.State);
                Assert.AreEqual(otherAddress.PostalCode, address.PostalCode);
                Assert.AreEqual(otherAddress.Country, address.Country);
            }
        }

        internal static SetShipmentInformationWithStatus ToSetShipmentInformationWithStatus(this ShipmentInformation information, Action<SetShipmentInformationWithStatus> initialize = null)
        {
            var parameters = new SetShipmentInformationWithStatus
                {
                    ShipmentStatus = information.Status,
                    PalletWeight = information.PalletWeight,
                    PalletQuantity = information.PalletQuantity,

                    ShippingInstructions = new SetShippingInstructions
                        {
                            ShipFromOrSoldTo = information.ShipFrom,
                            ShipTo = information.ShipTo,
                            FreightBillTo = information.FreightBill,
                            RequiredDeliveryDateTime = information.RequiredDeliveryDate,
                            ShipmentDate = information.ShipmentDate,
                            InternalNotes = information.InternalNotes,
                            ExternalNotes = information.ExternalNotes,
                            SpecialInstructions = information.SpecialInstructions
                        },

                    TransitInformation = new SetTransitInformation
                        {
                            FreightBillType = information.FreightBillType,
                            ShipmentMethod = information.ShipmentMethod,
                            DriverName = information.DriverName,
                            CarrierName = information.CarrierName,
                            TrailerLicenseNumber = information.TrailerLicenseNumber,
                            ContainerSeal = information.ContainerSeal
                        }
                };

            if(initialize != null)
            {
                initialize(parameters);
            }

            return parameters;
        }
    }
}