var ShipmentViewModelFactory = (function(ko, shippingInstructionsViewModel, undefined) {
    return {
        init: init
    };
    
    function init(options) {
        if (!options) throw new Error("Options required for initialization of new ShipmentViewModel.");
        if (!options.target) throw new Error("Target option is required.");
        //if (!options.ShipmentKey) throw new Error("The ShipmentKey option must be defined.");
        //var data = options.data || {};

        if (!options.data) {
            assignTarget(null);
            return;
        }
        
        var data = options.data;

        var viewModel = {
            isNew: data.ShipmentKey == undefined,
            Status: ko.numericObservable(data.Status, 0).extend({ shipmentStatusType: true }),
            PalletWeight: ko.numericObservable(data.PalletWeight, 2).extend({ min: 0 }),
            PalletQuantity: ko.numericObservable(data.PalletQuantity, 0).extend({ min: 0 }),
            TransitInformation: new TransitInfo(data.TransitInformation),
            
            //methods
            toDto: toDto,
        };


        var shippingInstructionsVm = shippingInstructionsViewModel.init({
            target: viewModel,
            data: data.ShippingInstructions,
        });

        var esm = new EsmHelper(viewModel, {
            customMappings: {
                TransitInformation: function(data) {
                    return new TransitInfo(data);
                }
            },
            beginEditingCallback: function() {
                shippingInstructionsVm.beginEditingCommand.execute();
            },
            endEditingCallback: function() {
                shippingInstructionsVm.endEditingCommand.execute();
            },
            revertChangesCallback: function () {
                shippingInstructionsVm.cancelEditsCommand.execute();
            },
            hasUntrackedChanges: function() {
                return shippingInstructionsVm.hasChanges();
            },
            commitChangesCallback: function() {
                shippingInstructionsVm.saveEditsCommand.execute();
            },
            include: ['TransitInformation'],
            enableLogging: true
        });
        
        // commands
        viewModel.saveCommand = ko.composableCommand({            
            execute: function (complete) {
                viewModel.saveCommand.indicateWorking();
                saveShipment({
                    successCallback: function () {
                        viewModel.saveEditsCommand.execute();
                        viewModel.saveCommand.pushSuccess("Shipment information saved successfully.");
                        viewModel.saveCommand.indicateSuccess();
                    },
                    errorCallback: function (xhr) {
                        viewModel.saveCommand.pushError("Shipment information failed to save." + (xhr ? "Description: " + xhr.responseText : ""));
                        viewModel.saveCommand.indicateFailure();
                    },
                    completeCallback: complete,
                });
            },
            canExecute: function(isExecuting) {
                return !isExecuting;
            },
            shouldExecute: function() {
                return viewModel.hasChanges();
            }
        });

        ajaxStatusHelper.init(viewModel.saveCommand);
        assignTarget(viewModel);
        return viewModel;
        
        // private functions
        function toDto() {
            return buildShipmentDto.call(this, data.InventoryOrderEnum, shippingInstructionsVm);
        }
        function saveShipment(opts) {
            api.shipments.saveShipment(options.ShipmentKey, viewModel.toDto(), opts);
        }
        function assignTarget(value) {
            options.target.ShipmentViewModel = value;
        }
    }
    
    function buildShipmentDto(shipmentType, shippingInstructions) {
        return {
            InventoryOrderEnum: shipmentType,
            PalletWeight: this.PalletWeight(),
            PalletQuantity: this.PalletQuantity(),
            Status: this.Status(),
            ShippingInstructions: shippingInstructions.toDto(),
            TransitInformation: this.TransitInformation
        };
    }
    
    function TransitInfo(data) {
        data = data || {};
        return {
            DriverName: ko.observable(data.DriverName),
            CarrierName: ko.observable(data.CarrierName),
            TrailerLicenseNumber: ko.observable(data.TrailerLicenseNumber),
            ContainerSeal: ko.observable(data.ContainerSeal),
            FreightType: ko.observable(data.FreightType),
        };
    }
}(ko, ShippingInstructionsViewModel));
