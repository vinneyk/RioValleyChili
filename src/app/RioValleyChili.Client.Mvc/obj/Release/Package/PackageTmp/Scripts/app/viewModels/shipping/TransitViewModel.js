var TransitViewModel = (function (ko) {
    return {
        init: init
    };
    
    function init(options) {
        if (!options.target) throw new Error("Target option is required.");
        var data = options.data || {};
        
        var model = {
            DriverName: ko.observable(data.DriverName),
            CarrierName: ko.observable(data.CarrierName),
            TrailerLicenseNumber: ko.observable(data.TrailerLicenseNumber),
            ContainerSeal: ko.observable(data.ContainerSeal),
            
            // functions
            toDto: function() {
                return getTransitDto.call(model);
            }
        };
        
        //var esm = new EsmHelper(model, {
        //    name: 'TransitViewModel.esm',
        //});

        options.target.TransitViewModel = model;
        return model;
    };
    
    function getTransitDto() {
        var model = this;
        return {
            FreightType: model.FreightType,
            DriverName: model.DriverName,
            CarrierName: model.CarrierName,
            TrailerLicenseNumber: model.TrailerLicenseNumber,
            ContainerSeal: model.ContainerSeal,
        };
    }

}(ko));