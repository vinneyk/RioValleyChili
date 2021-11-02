var warehouseService = require('services/warehouseService');

/**
  * @param {Object} input - Observable, Contact label data
  * @param {boolean} [locked=false] - Disables editing of contact
  * @param {Object} exports - Observable, container for exposed methods and properties
  */
function ContactLabelEditorViewModel(params) {
    var self = this,
        input = ko.toJS(params.input) || {};

    self.disposables = [];

    self.isLocked = params.locked || null;

    self.Name = ko.observable(input.Name);
    self.Phone = ko.observable(input.Phone);
    self.EMail = ko.observable(input.EMail);
    self.Fax = ko.observable(input.Fax);
    self.Address = ko.observable(input.Address);

    self.addressExports = ko.observable();

    if (ko.isObservable(params.input)) {
      self.disposables.push([
        params.input.subscribe(function(values) {
          input = ko.toJS(values || {});

          if (input.hasOwnProperty('Address')) {
            self.Name(input.Name);
            self.Phone(input.Phone);
            self.EMail(input.EMail);
            self.Fax(input.Fax);
            self.Address(input.Address);
          } else {
            self.Address(input);
          }
        })
      ]);
    }

  // Output
    if ( params && params.exports ) {
      params.exports({
          Name: self.Name,
          Phone: self.Phone,
          EMail: self.EMail,
          Fax: self.Fax,
          Address: self.addressExports
      });
    }
}

ko.utils.extend(ContactLabelEditorViewModel, {
    dispose: function () {
        ko.utils.arrayForEach(this.disposables, this.disposeOne);
        ko.utils.objectForEach(this, this.disposeOne);
    },

    disposeOne: function(propOrValue, value) {
        var disposable = value || propOrValue;

        if (disposable && typeof disposable.dispose === "function") {
            disposable.dispose();
        }
    }
});

ko.components.register('address-editor', require('App/components/warehouse/address-editor/address-editor'));

module.exports = {
    viewModel: ContactLabelEditorViewModel,
    template: require('./contact-label-editor.html')
};
