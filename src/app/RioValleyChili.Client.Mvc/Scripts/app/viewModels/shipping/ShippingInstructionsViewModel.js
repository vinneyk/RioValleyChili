var ShippingInstructionsViewModel = (function(ko, shippingLabelViewModelFactory) {

    return {
        init: init
    };

    function init(options) {
        if (!options.target) throw new Error("Requires target option");
        var data = options.data || {};

        var self = {
            shipFromRincon: ko.observable(true),
            billFreightToRvc: ko.observable(true),
            RequiredDeliveryDateTime: ko.observable(data.RequiredDeliveryDateTime).extend({ isoDate: 'm/d/yyyy' }),
            Comments: ko.observable(data.Comments),
            ShipToShippingLabel: shippingLabelViewModelFactory.init(data.ShipToShippingLabel),
            ShipFromShippingLabel: shippingLabelViewModelFactory.init(data.ShipFromShippingLabel),
            FreightBillToShippingLabel: shippingLabelViewModelFactory.init(data.FreightBillToShippingLabel),
        
            // methods
            setShipToShippingLabel: setShipToShippingLabel,
            toDto: getShippingInstructionsDto,
        };

        var esm = new EsmHelper(self, {
            initializeAsEditing: data.InitializedAsEmpty === true,
            ignore: ['ShipToShippingLabel', 'ShipFromShippingLabel', 'FreightBillToShippingLabel'],
            name: "ShippingInstructionsViewModel.esm",
            hasUntrackedChanges: function () {
                return self.ShipFromShippingLabel.hasChanges()
                    || self.ShipToShippingLabel.hasChanges()
                    || self.FreightBillToShippingLabel.hasChanges();
            },
            endEditingCallback: endEditing.bind(self),
            revertChangesCallback: revertChanges.bind(self),
        });

        // register subscribers
        self.shipFromRincon.subscribe(function (newVal) {
            if (newVal === true) {
                self.ShipFromShippingLabel.update(self.RvcRinconAddress);
            } else {
                self.ShipFromShippingLabel.clear();
            }
        }, self);
        self.billFreightToRvc.subscribe(function (newVal) {
            if (newVal === true) {
                self.FreightBillToShippingLabel.update(self.RvcRinconAddress);
            } else {
                self.FreightBillToShippingLabel.clear();
            }
        }, self);

        options.target.ShippingInstructions = self;
        return self;
    }
    
    // private functions
    function setShipToShippingLabel(data) {
        this.ShipToShippingLabel.update(data);
    }    
    function getShippingInstructionsDto() {
        var model = this;
        return {
            ShipFromShippingLabel: model.ShipFromShippingLabel.toDto(),
            ShipToShippingLabel: model.ShipToShippingLabel.toDto(),
            FreightBillToShippingLabel: model.FreightBillToShippingLabel.toDto(),
            RequiredDeliveryDateTime: model.RequiredDeliveryDateTime,
            Comments: model.Comments,
        };
    }
    function revertChanges() {
        this.ShipToShippingLabel.cancelEditsCommand.execute();
        this.ShipFromShippingLabel.cancelEditsCommand.execute();
        this.FreightBillToShippingLabel.cancelEditsCommand.execute();
    }
    function endEditing() {
        this.ShipToShippingLabel.endEditingCommand.execute();
        this.ShipFromShippingLabel.endEditingCommand.execute();
        this.FreightBillToShippingLabel.endEditingCommand.execute();
    }
    
}(window.ko, ShippingLabelViewModel))