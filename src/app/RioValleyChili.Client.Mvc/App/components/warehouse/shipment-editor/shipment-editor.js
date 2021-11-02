/**
  * @param {Object} input - Observable, input data to initialize editor
  * @param {Object} exports - Observable, container for editor return
  * @param {boolean} [soldTo=false] - Replace "Ship From" with "Sold To"
  */

var warehouseService = require('services/warehouseService');

function ShipmentEditorViewModel(params) {
    var self = this,
        input = ko.toJS(params.input) || {},
        transit = input.Transit || {},
        shippingInstructions = input.ShippingInstructions || {};

    self.isLocked = params.locked || null;
    self.isInit = ko.observable(false);
    self.isSoldTo = ko.unwrap( params.soldTo );

    // Data
    self.PalletWeight = ko.observable(input.PalletWeight);
    self.PalletQuantity = ko.numericObservable(input.PalletQuantity);
    self.Status = ko.observable(input.Status || 0).extend({ shipmentStatusType: true });
    self.FreightType = ko.observable(transit.FreightType);
    self.DriverName = ko.observable(transit.DriverName);
    self.CarrierName = ko.observable(transit.CarrierName);
    self.TrailerLicenseNumber = ko.observable(transit.TrailerLicenseNumber);
    self.ContainerSeal = ko.observable(transit.ContainerSeal);
    self.ShippingInstructions = {
        ShipTo: ko.observable(shippingInstructions.ShipTo),
        ShipFrom: ko.observable(shippingInstructions.ShipFromOrSoldTo || shippingInstructions.ShipFrom),
        FreightBill: ko.observable(shippingInstructions.FreightBill),
    };

    self.FreightBillTypes = [
        'Prepaid',
        'FOB',
        'Collect',
        'COD',
        '3rd Party',
        'Delivered',
        'Other'
    ];

    // Exports from address editor component
    self.ShipFromExport = ko.observable();
    self.ShipToExport = ko.observable();
    self.FreightBillExport = ko.observable();

    self.isInit(true);

    // Behaviors
    function setShipToAddress(values) {
        self.ShippingInstructions.ShipTo(values);
    }
    function setShipFromAddress(values) {
        self.ShippingInstructions.ShipFrom(values);
    }
    function setFreightBillAddress(values) {
        self.ShippingInstructions.FreightBill(values);
    }
    function toDto() {
        return ko.toJS({
            PalletWeight: self.PalletWeight,
            PalletQuantity: self.PalletQuantity,
            Status: self.Status,
            Transit: {
                FreightBillType: self.FreightType,
                DriverName: self.DriverName,
                CarrierName: self.CarrierName,
                TrailerLicenseNumber: self.TrailerLicenseNumber,
                ContainerSeal: self.ContainerSeal,
            },
            ShippingInstructions: {
                ShipFromOrSoldTo: self.ShipFromExport,
                ShipTo: self.ShipToExport,
                FreightBill: self.FreightBillExport
            }
        });
    }

    // Output
    if (params && params.exports) {
      params.exports({
          isInit: self.isInit,
          toDto: toDto,
          setShipToAddress: setShipToAddress,
          setShipFromAddress: setShipFromAddress,
          setFreightBillAddress: setFreightBillAddress,
      });
    }

    return this;
}

ko.components.register('contact-label-editor', require('App/components/warehouse/contact-label-editor/contact-label-editor'));

module.exports = {
    viewModel: ShipmentEditorViewModel,
    template: require('./shipment-editor.html')
};
