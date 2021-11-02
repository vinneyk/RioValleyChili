function DehydratedItemReceived( input ) {
  if ( !(this instanceof DehydratedItemReceived) ) { return new DehydratedItemReceived( input ); }

  var self = this;

  self.Tote = ko.observable( input.Tote );
  self.Variety = ko.observable( input.Variety );
  self.Grower = ko.observable( input.Grower );
  self.Location = ko.observable( input.Location );
  self.Quantity = ko.observable( input.Quantity );
  self.PackagingProduct = ko.observable( input.PackagingProduct );

  // computeds
  self.Weight = ko.pureComputed(function () {
    var quantity = self.Quantity();
    var packaging = self.PackagingProduct();
    return packaging && !isNaN( packaging.Weight )
      ? ( quantity > 0 ? quantity * packaging.Weight : 0 )
      : 0;
  });
  self.LocationDescription = ko.pureComputed(function () {
    var location = self.Location() || {};
    return location.Description;
  });
  self.PackagingName = ko.pureComputed(function () {
    var packaging = self.PackagingProduct() || {};
    return packaging.ProductName;
  });

  // validation and other extensions
  self.Tote.extend({
    toteKey: true,
    required: { onlyIf: isRequired }
  });
  self.Quantity.extend({
    required: {
      message: "This field is required.",
      onlyIf: function () {
        return self.Tote() != undefined;
      }
    }
  });

  self.Variety.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });
  self.Grower.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });
  self.Location.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });
  self.PackagingProduct.extend({
    required: {
      message: "This field is required.",
      onlyIf: isRequired
    }
  });

  function isRequired() {
    return self.Quantity() > 0;
  }
}

module.exports = DehydratedItemReceived;
