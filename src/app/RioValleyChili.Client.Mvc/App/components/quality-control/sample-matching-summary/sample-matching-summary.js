function SampleMatchingSummaryVM( params ) {
  if ( !(this instanceof SampleMatchingSummaryVM) ) { return new SampleMatchingSummaryVM( params ); }

  var self = this;

  this.samples = params.input;

  // Behaviors
  this.selected = params.selected || ko.observable();
  // Select a sample for details view
  this.selectSample = function( data, element ) {
    // Check for nearby sample-summary element
    var $tr = $( element.target ).closest('tr')[0];

    // If sample is found
    if ( $tr ) {
      // Get context for element
      var sample = ko.contextFor( $tr ).$data;

      // Assign data to selected observable
      self.selected( sample );
    }
  };

  // Update sample list with new data
  function updateSample( newData ) {
    var sampleKey = newData.SampleRequestKey;
    var _samples = self.samples();

    var matchedSample = ko.utils.arrayFirst( _samples, function( sample ) {
      return sample.SampleRequestKey === sampleKey;
    });

    // If sample list has item with a matching key
    if ( matchedSample ) {
      // Replace item with new data
      var sampleIndex = _samples.indexOf( matchedSample );

      self.samples.splice( sampleIndex, 1, newData );

    // Else, append to the top of the samples list
    } else {
      self.samples.splice( 0, 0, newData );
    }
  }

  function removeSample( sampleKey ) {
    var _samples = self.samples();

    var matchedSample = ko.utils.arrayFirst( _samples, function( sample ) {
      return sample.SampleRequestKey === sampleKey;
    });

    if ( matchedSample ) {
      var sampleIndex = _samples.indexOf( matchedSample );

      self.samples.splice( sampleIndex, 1 );
    }
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      updateSample: updateSample,
      removeSample: removeSample
    });
  }

  return this;
}

module.exports = {
  viewModel: SampleMatchingSummaryVM,
  template: require('./sample-matching-summary.html')
};
