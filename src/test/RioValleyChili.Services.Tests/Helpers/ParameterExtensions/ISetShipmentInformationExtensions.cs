using NUnit.Framework;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ISetShipmentInformationExtensions
    {
        internal static void AssertEqual(this ISetShipmentInformation expected, ShipmentInformation result, ShippingLabel soldTo = null)
        {
            expected = expected ?? new SetShipmentInformationWithStatus();
            Assert.AreEqual(expected.PalletWeight, result.PalletWeight);
            Assert.AreEqual(expected.PalletQuantity, result.PalletQuantity);
            expected.ShippingInstructions.AssertEqual(result, soldTo);
            expected.TransitInformation.AssertEqual(result);
        }

        internal static void AssertEqual(this ISetShippingInstructions expected, ShipmentInformation result, ShippingLabel soldTo = null)
        {
            expected = expected ?? new SetShippingInstructions();

            Assert.AreEqual(expected.RequiredDeliveryDateTime, result.RequiredDeliveryDate);
            Assert.AreEqual(expected.ShipmentDate, result.ShipmentDate);
            Assert.AreEqual(expected.InternalNotes, result.InternalNotes);
            Assert.AreEqual(expected.ExternalNotes, result.ExternalNotes);
            Assert.AreEqual(expected.SpecialInstructions, result.SpecialInstructions);
            expected.ShipFromOrSoldTo.AssertEquivalent(soldTo ?? result.ShipFrom);
            expected.ShipTo.AssertEquivalent(result.ShipTo);
            expected.FreightBillTo.AssertEquivalent(result.FreightBill);
        }

        internal static void AssertEqual(this ISetTransitInformation expected, ShipmentInformation result)
        {
            expected = expected ?? new SetTransitInformation();

            Assert.AreEqual(expected.FreightBillType, result.FreightBillType);
            Assert.AreEqual(expected.ShipmentMethod, result.ShipmentMethod);
            Assert.AreEqual(expected.DriverName, result.DriverName);
            Assert.AreEqual(expected.CarrierName, result.CarrierName);
            Assert.AreEqual(expected.TrailerLicenseNumber, result.TrailerLicenseNumber);
            Assert.AreEqual(expected.ContainerSeal, result.ContainerSeal);
        }
    }
}