using System;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using Solutionhead.EntityKey;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using Solutionhead.Services;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases
{
    [TestFixture]
    public abstract class SetShipmentTestsBase<TService, TOrder, TOrderKey> : ServiceIntegrationTestBase<TService>
        where TService : class
        where TOrder : class
        where TOrderKey : EntityKeyBase, new()
    {
        protected abstract TOrder SetupOrder();
        protected abstract TOrderKey CreateKeyFromOrder(TOrder order);
        protected abstract ShipmentInformationKey GetShipmentInformationKey(TOrder order);
        protected abstract IResult GetResult(string key, Shipment shipment);

        [Test]
        public void Returns_non_successful_result_if_Order_Key_could_not_be_parsed()
        {
            //Act
            var result = GetResult("Nonononono... it's just no good.", new Shipment());

            //Assert
            result.AssertNotSuccess();
        }

        [Test]
        public void Returns_non_successful_result_if_Order_could_not_be_found()
        {
            //Arrange
            var key = new TOrderKey();

            //Act
            var result = GetResult(key.KeyValue, new Shipment());

            //Assert
            result.AssertNotSuccess();
        }

        [Test]
        public void Sets_ShipmentInformation_record_as_expected()
        {
            //Arrange
            const int expectedPalletQuantity = 42;
            const string expectedShipToAddressLine1 = "Address line 1";
            const string expectedComments = "Oh, to be a comment!";
            var expectedRequiredDelivery = new DateTime(2013, 3, 29);

            var order = SetupOrder();
            var orderKey = CreateKeyFromOrder(order);
            var shipment = new Shipment
                {
                    PalletQuantity = expectedPalletQuantity,
                    ShippingInstructions = new ShippingInstructions
                        {
                            RequiredDeliveryDateTime = expectedRequiredDelivery,
                            ExternalNotes = expectedComments,
                            ShipToShippingLabel = new ShippingLabel
                                {
                                    Address = new Address
                                        {
                                            AddressLine1 = expectedShipToAddressLine1
                                        }
                                },
                            ShipFromOrSoldToShippingLabel = new ShippingLabel(),
                            FreightBillToShippingLabel = new ShippingLabel()
                        }
                };

            //Act
            var result = GetResult(orderKey.KeyValue, shipment);

            //Assert
            result.AssertSuccess();
            var shipmentInfo = RVCUnitOfWork.ShipmentInformationRepository.FindByKey(GetShipmentInformationKey(order));
            Assert.AreEqual(expectedComments, shipmentInfo.ExternalNotes);
            Assert.AreEqual(expectedPalletQuantity, shipmentInfo.PalletQuantity);
            Assert.AreEqual(expectedShipToAddressLine1, shipmentInfo.ShipTo.Address.AddressLine1);
            Assert.AreEqual(expectedRequiredDelivery, shipmentInfo.RequiredDeliveryDate);
        }

        [Test]
        public void Sets_TransitInformation_to_null_in_database_if_null_is_supplied()
        {
            //Arrange
            var order = SetupOrder();
            var orderKey = CreateKeyFromOrder(order);

            var shipmentInfo = RVCUnitOfWork.ShipmentInformationRepository.FindByKey(GetShipmentInformationKey(order));
            Assert.IsNotNull(shipmentInfo.FreightBillType);
            Assert.IsNotNull(shipmentInfo.DriverName);
            Assert.IsNotNull(shipmentInfo.CarrierName);
            Assert.IsNotNull(shipmentInfo.TrailerLicenseNumber);
            Assert.IsNotNull(shipmentInfo.ContainerSeal);

            var shipment = new Shipment
            {
                ShippingInstructions = TestHelper.CreateObjectGraph<ShippingInstructions>(),
                TransitInformation = null
            };

            //Act
            var result = GetResult(orderKey.KeyValue, shipment);

            //Assert
            result.AssertSuccess();
            ResetUnitOfWork();
            shipmentInfo = RVCUnitOfWork.ShipmentInformationRepository.FindByKey(GetShipmentInformationKey(order));
            Assert.IsNull(shipmentInfo.FreightBillType);
            Assert.IsNull(shipmentInfo.DriverName);
            Assert.IsNull(shipmentInfo.CarrierName);
            Assert.IsNull(shipmentInfo.TrailerLicenseNumber);
            Assert.IsNull(shipmentInfo.ContainerSeal);
        }

        [Test]
        public void Sets_ShippingInstructions_to_null_in_database_if_null_is_supplied()
        {
            //Arrange
            var order = SetupOrder();
            var orderKey = CreateKeyFromOrder(order);

            var shipmentInfo = RVCUnitOfWork.ShipmentInformationRepository.FindByKey(GetShipmentInformationKey(order));
            Assert.IsNotNull(shipmentInfo.RequiredDeliveryDate);
            Assert.IsNotNull(shipmentInfo.ExternalNotes);
            AssertShippingLabelIsNotNull(shipmentInfo.ShipFrom);
            AssertShippingLabelIsNotNull(shipmentInfo.ShipTo);
            AssertShippingLabelIsNotNull(shipmentInfo.FreightBill);

            var shipment = new Shipment
                {
                    ShippingInstructions = null,
                    TransitInformation = TestHelper.CreateObjectGraph<TestableTransitInformation>()
                };

            //Act
            var result = GetResult(orderKey.KeyValue, shipment);

            //Assert
            result.AssertSuccess();

            ResetUnitOfWork();
            shipmentInfo = RVCUnitOfWork.ShipmentInformationRepository.FindByKey(GetShipmentInformationKey(order));
            Assert.IsNull(shipmentInfo.RequiredDeliveryDate);
            Assert.IsNull(shipmentInfo.ExternalNotes);
            AssertShippingLabelIsNull(shipmentInfo.ShipFrom);
            AssertShippingLabelIsNull(shipmentInfo.ShipTo);
            AssertShippingLabelIsNull(shipmentInfo.FreightBill);
        }

        private static void AssertShippingLabelIsNotNull(ShippingLabel label)
        {
            if(label == null) throw new ArgumentNullException("label");

            Assert.IsNotNull(label.Phone);
            Assert.IsNotNull(label.EMail);
            Assert.IsNotNull(label.Fax);
            Assert.IsNotNull(label.Name);
            Assert.IsNotNull(label.Address.AddressLine1);
            Assert.IsNotNull(label.Address.AddressLine2);
            Assert.IsNotNull(label.Address.AddressLine3);
            Assert.IsNotNull(label.Address.City);
            Assert.IsNotNull(label.Address.PostalCode);
            Assert.IsNotNull(label.Address.State);
        }

        private static void AssertShippingLabelIsNull(ShippingLabel label)
        {
            if(label == null) throw new ArgumentNullException("label");

            Assert.IsNull(label.Phone);
            Assert.IsNull(label.EMail);
            Assert.IsNull(label.Fax);
            Assert.IsNull(label.Name);
            Assert.IsNull(label.Address.AddressLine1);
            Assert.IsNull(label.Address.AddressLine2);
            Assert.IsNull(label.Address.AddressLine3);
            Assert.IsNull(label.Address.City);
            Assert.IsNull(label.Address.PostalCode);
            Assert.IsNull(label.Address.State);
        }

        private class TestableTransitInformation : ITransitInformation
        {
            [StringLength(25)]
            public string ShipmentMethod { get; set; }

            [StringLength(25)]
            public string FreightType { get; set; }

            [StringLength(25)]
            public string DriverName { get; set; }

            [StringLength(25)]
            public string CarrierName { get; set; }

            [StringLength(25)]
            public string TrailerLicenseNumber { get; set; }

            [StringLength(25)]
            public string ContainerSeal { get; set; }
        }
    }
}