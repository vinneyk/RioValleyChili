/** Required libraries */
var productsService = require('App/services/productsService');
var directoryService = require('App/services/directoryService');
var page = require('page');

/** Included components */
ko.components.register( 'customer-product-specs-editor', require('App/components/quality-control/customer-product-spec-editor/customer-product-spec-editor'));
ko.components.register( 'customer-product-specs-summary', require('App/components/quality-control/customer-product-spec-summary/customer-product-spec-summary'));

/** Customer Product Spec view model */
function CustomerProductSpecsVM() {
  if ( !(this instanceof CustomerProductSpecsVM) ) { return new CustomerProductSpecsVM( params ); }

  var self = this;

  // Data
  // Summary Data
  this.selectedProduct = ko.observable();
  this.summaryData = {
    input: null,
    selected: null,
    exports: ko.observable()
  };

  // Editor Data
  this.editorData = {
    input: null,
    options: null,
    exports: ko.observable()
  };

  // Behaviors
  // Save editor data
  this.saveCommand = ko.asyncCommand({
    execute: function( complete ) {
    // Build DTO and validate data
    // If DTO is invalid, stop saving and notify user of errors

    // If new
      // Call API to create new product spec
    // Else if existing
      // Call API to update target product spec

    // After calling the save API
      // On sucess:
      // Update summary table
      // Notify User of success

      // On failure:
      // Notify use of failed save
    },
    canExecute: function( isExecuting ) {
      return !isExecuting;
    }
  });

  // Close editor
  function closeEditor() {
    // Check if editor is dirty

    // If editor is dirty, confirm if user wants to save before closing
      // If yes, attempt to save and navigate if successful, else stop closing
      // If no, close UI and discard changes
      // If cancel, do nothing
    // Else, close UI
  }

  // Close editor view via command
  this.closeEditor = ko.command({
    execute: function() {
      // Navigate to summary view to trigger route
    }
  });

  // Page.js Routing
  page.base( '/QualityControl/CustomerSpecs' );

  // Check if editor is open and if it's dirty
  function checkIfDirty( ctx, next ) {
    // If dirty, confirm with user and offer to save
    // Else, continue with navigation
  }
  page( checkIfDirty );

  // Check if user provided key
  function getCustomerDetails( ctx, next ) {
    // If user provided 'new' key, display "Create" view
    // Else, if user provided an existing key, display "Edit" view
    // Else, continue to next route
  }
  page( '/:customerKey', getCustomerDetails );

  function displaySummaries( ctx, next ) {
    // If user did not provide key
      // Reset UI
      // Return to Summary view
  }
  page( displaySummaries );

  // Fetch options and ensure all data is ready before initializing UI
  function init() {
    // Get customers
    // Then get product options
  }

  // Exports
  return this;
}

var vm = new CustomerProductSpecsVM();

ko.applyBindings( vm );

module.exports = vm;
