/* File Created: August 2, 2012 */

var ProductionScheduleDetailsViewModel = function(itemSorterViewModel, values, opts) {
    var self = this;

    self.productionScheduleKey = values.productionScheduleKey;
    self.productionDate = ko.observable(values.productionDate);
    self.productionLine = ko.observable(values.productionLine);
    self.editable = values.isEditable;
    self.canDelete = values.canBeDeleted;
              
    self.save = function() {
        var productionSchedule = {
            ProductionLine: self.productionLine(),
            ProductionDate: self.productionDate(),
            ProductionScheduleItems: itemSorterViewModel.getOrderedItems()
        };
              
        $.ajax(
            {
                url: opts.saveUrl,
                data: JSON.stringify(productionSchedule),
                contentType: 'application/json',
                dataType: 'json',
                type: 'PUT',
                //type: 'POST',
                statusCode: {
                    200: function () {
                        if(opts.saveSuccessUrl == '') {
                            alert("Changes saved successfully.");
                            return;
                        }
                        
                        window.location = opts.saveSuccessUrl;
                    }
                },
                error: function () { alert('error'); } //todo: replace with JS function to display error
            });
    };

    self.canSave = ko.computed(function() {
        return self.editable;
    });

    self.callDelete = function() {
        $.ajax(
        {
            url: opts.deleteUrl,
            data: ko.toJSON(self.productionScheduleKey),
            contentType: 'application/json',
            dataType: 'json',
            type: 'DELETE',
            statusCode: { 
                200: function () {
                    if(opts.deleteSuccessUrl == '') {
                        alert("Production Schedule deleted successfully.");
                        return;
                    }
                    window.location = opts.deleteSuccessUrl;
                    // todo: implement mechanism to notify user that the record was deleted
                },
                400: function () { alert('bad request'); }, //todo: replace with JS function to display error
                500: function () { alert('internal error'); } //todo: replace with JS function to display error
            }
        });
    };
};