function CustomerMaintenanceSummaryVM( params ) {
  if ( !(this instanceof CustomerMaintenanceSummaryVM) ) { return new CustomerMaintenanceSummaryVM( params ); }

  var self = this;

  // Data
  this.companies = params.input;

  this.selected = params.selected;
  this.selectCompany = function( data, element ) {
    var $tr = $( element.target ).closest('tr')[0];

    // End if tr element does not exist
    if ( !$tr ) {
      return;
    }

    // Set company as selected
    var company = ko.contextFor( $tr ).$data;

    this.selected( company );
  };

  function addCompany( companyData ) {
    self.companies.splice( 0, 0, companyData );
  }

  function updateCompany( companyKey, companyData ) {
    var _companies = self.companies();
    var _company = ko.utils.arrayFirst( _companies, function( company ) {
      return company.CompanyKey === companyKey;
    });
    var companyIndex = _companies.indexOf( _company );

    if ( companyIndex > -1 ) {
      self.companies.splice( companyIndex, 1, companyData );
    } else {
      self.companies.splice( 0, 0, companyData );
    }
  }

  // Exports
  if ( params && params.exports ) {
    params.exports({
      addCompany: addCompany,
      updateCompany: updateCompany
    });
  }

  return this;
}

module.exports = {
  viewModel: CustomerMaintenanceSummaryVM,
  template: require('./customer-maintenance-summary.html')
};
