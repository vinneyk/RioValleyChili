"use strict";
var InventoryTreatments = (function(root, $) {
    var app = root.rvc,
        ko = root.ko;

    var pagedDataHelper = PagedDataHelper.init({ pageSize: 20 });

    var self = {
        loadData: loadTreatmentData,
        TreatmentOrders: ko.observableArray([]),
        IsLoading: ko.observable(false),
    };
    
    self.LoadButtonText = ko.computed(function() {
        return self.IsLoading() ? "Loading..." :
            pagedDataHelper.AllDataLoaded ? "All Data Loaded" : "Load More Treatments";
    }, self);

    //initialize 
    ko.applyBindings(self);
    loadTreatmentData();
        
    function loadTreatmentData() {
        self.IsLoading(true);
        pagedDataHelper.GetNextPage({
            urlBase: "/api/treatmentorders",
            successCallback: function (data) {
                self.IsLoading(false);
                var treatmentOrders = ko.utils.arrayMap(data, function(item) { return new TreatmentOrder(item); });
                ko.utils.arrayPushAll(self.TreatmentOrders(), treatmentOrders);
                self.TreatmentOrders.notifySubscribers();
            },
            errorCallback: function () {
                self.IsLoading(false);
                alert('error');
            },
        });
    }

    // *******************************
    // private types

    var TreatmentOrder = function(data) {
        var me = {
            DateCreated: ko.observable().extend({
                isoDate: 'm/d/yyyy hh:MM TT'
            })
        };

        ko.mapping.fromJS(data, {}, me);

        return me;
    };

}(window, jQuery, undefined));