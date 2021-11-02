webpackJsonp([1],[
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(26), __webpack_require__(40),
	    __webpack_require__(41),
	    __webpack_require__(46),
	    __webpack_require__(69),
	    __webpack_require__(37),
	    __webpack_require__(71), __webpack_require__(45)], __WEBPACK_AMD_DEFINE_RESULT__ = function (ko, page, salesService, contractSummariesComponent, contractDetailsComponent, contractShipmentSummariesComponent) {
	      __webpack_require__(18);

	      ko.punches.enableAll();

	        return init();

	    function init() {
	        var contractKeyFilter = ko.observable();

	        var vm = {
	            selectedContract: ko.observable(),
	            contractDetails: ko.observable(),
	            cloneContractKey: ko.observable(),
	            pastContractsForCustomer: ko.observableArray([]),
	            searchContractKey: ko.observable(),
	            selectedCustomer: ko.observable(),
	            customerFilterOptions: ko.observableArray([]),
	        };

	        var contractSummariesVm = new contractSummariesComponent.viewModel({
	            customerFilter: vm.selectedCustomer,
	            contractKeyFilter: contractKeyFilter,
	        });

	        ko.components.register('customer-contract-summaries', {
	            viewModel: { instance: contractSummariesVm },
	            template: contractSummariesComponent.template,
	        });

	        var contractDetailsComponentVm = new contractDetailsComponent.viewModel({
	            input: vm,
	            contract: vm.contractDetails,
	            contractKey: vm.selectedContract
	        });
	        ko.components.register('customer-contract-details', {
	            viewModel: { instance: contractDetailsComponentVm },
	            template: contractDetailsComponent.template,
	        });

	        ko.components.register('customer-contract-shipment-summaries', contractShipmentSummariesComponent);

	        vm.isUpdating = false;
	        vm.isAdding = false;

	        vm.isAddrVisible = contractDetailsComponentVm.addressVisible;
	        vm.loadingContractDetails = contractDetailsComponentVm.loadingContractDetails;
	        vm.loadCustomerContractsCommand = contractSummariesVm.loadCustomerContractsCommand;
	        vm.closeAddrCommand = ko.command({
	          execute: function() {
	            vm.isAddrVisible( false );
	          },
	          canExecute: function() {
	            return true;
	          }
	        });

	        //#region observables
	        vm.showShipmentSummariesForContracts = vm.pastContractsForCustomer.filter(function(c) {
	            return c.showDraw() === true;
	        }).map(function(c) {
	            return c.contract;
	        });
	        vm.isClone = false;
	        //#endregion

	        vm.loadContractByKeyCommand = ko.asyncCommand({
	            execute: function (complete) {
	                contractDetailsComponentVm.loadContractByKey(vm.searchContractKey())
	                    .always(complete);
	            },
	            canExecute: function(isExecuting) {
	                return !isExecuting && vm.searchContractKey() && true;
	            }
	        });
	        vm.createNewCustomerContractCommand = ko.command({
	            execute: function () {
	              this.selectedContract('new');
	              vm.goToContract('new');
	            }
	        });
	        vm.closeContractCommand = ko.command({
	            execute: function() {
	                contractDetailsComponentVm.clearContractSelection();
	                vm.contractDetails(null);
	                vm.selectedContract(null);
	                vm.pastContractsForCustomer([]);
	                vm.isClone = false;
	                vm.goToContract();
	            },
	            canExecute: function() {
	                return contractDetailsComponentVm.contract() && true;
	            }
	        });
	        vm.saveContractCommand = ko.command({
	            execute: function () {
	                var isNew = !ko.unwrap(contractDetailsComponentVm.contract().CustomerContractKey);

	                if (isNew) {
	                  vm.isAdding = true;
	                } else {
	                  vm.isUpdating = true;
	                }

	                return contractDetailsComponentVm.saveContractCommand.execute().then(
	                function(data, textStatus, jqXHR) {
	                  var key = contractDetailsComponentVm.contract().CustomerContractKey;

	                  vm.goToContract(key);
	                },
	                function(jqXHR, textStatus, errorThrown) {
	                   // Fail
	                });
	            },
	            canExecute: function () {
	                return contractDetailsComponentVm.saveContractCommand.canExecute() &&
	                  vm.contractDetails() &&
	                  true;
	            }
	        });
	        vm.cloneContractCommand = ko.asyncCommand({
	            execute: function (complete) {
	                var key = vm.cloneContractKey() || vm.selectedContract();
	                if (!key) throw new Error("Missing Contract Key");

	                var doClone = function () {
	                    contractDetailsComponentVm.findContractByKey(key)
	                    .then(function (contract) {
	                      vm.isClone = true;
	                      contractDetailsComponentVm.contract(contract.createNewFromCopy());
	                      vm.goToContract('new');
	                    })
	                    .fail(function (xhr, status, message) {
	                        showUserMessage("Unable to copy contract", { description: "The copy failed due to an error retrieving the contract from the database." });
	                    })
	                    .always(complete);
	                };

	                // Check if dirty
	                var contract = contractDetailsComponentVm.contract();
	                if (contract && contract.hasChanges()) {
	                    showUserMessage("Save changes before copying?", {
	                        description: "The current contract has unsaved changes. Would you like to save before we copy the contract? Click <strong>Yes</strong> to save the changes and then copy the saved contract. Click <strong>No</strong> to discard the changes and copy the contract without the changes. Click <strong>Cancel</strong> to cancel the copy operation.",
	                        type: 'yesnocancel',
	                        onYesClick: function () {
	                            vm.saveContractCommand.execute().then(doClone);
	                        },
	                        onNoClick: doClone,
	                        onCancelClick: complete
	                    });
	                } else doClone();
	            },
	            canExecute: function (isExecuting) {
	                var isNew = vm.contractDetails() ? vm.contractDetails().isNew : false;
	                return !isExecuting && (vm.cloneContractKey() || vm.selectedContract()) && !isNew;
	            }
	        });
	        vm.deleteContractCommand = ko.asyncCommand({
	            execute: function (complete) {
	                if (showUserMessage("Are you sure you want to delete this contract?", {
	                    description: "This operation cannot be undone. To continue with the deletion of the contract, click \"Yes\" otherwise, click \"No\" to abort the deletion.",
	                    type: "yesno",
	                    onYesClick: function() {
	                        salesService.deleteContract(contractDetailsComponentVm.contract().CustomerContractKey)
	                            .then(function() {
	                                vm.closeContractCommand.execute();
	                                showUserMessage("Contract deleted successfully.");
	                                //todo: remove summary item
	                            })
	                            .always(complete);
	                    },
	                    onNoClick: complete
	                }));
	            },
	            canExecute: function (isExecuting) {
	                return !isExecuting && vm.contractDetails() && !vm.contractDetails().isNew && true;
	            }
	        });

	        //#region subscribers
	        var contractSubscriptions = [];
	        contractDetailsComponentVm.contract.subscribe(function (contract) {
	            if (contractSubscriptions.length) {
	                ko.utils.arrayForEach(contractSubscriptions, function (sub) {
	                  var disposeResult = sub.dispose && sub.dispose();
	                });
	                contractSubscriptions = [];
	            }

	            if (!contract) {
	                vm.pastContractsForCustomer([]);
	                return;
	            }

	            if (contract.CustomerKey()) loadRecentContractsForCustomer.call(vm, contract.CustomerKey(), contract.copiedFrom);
	            else vm.pastContractsForCustomer([]);

	            contractSubscriptions.push(contract.CustomerKey.subscribe(function(customerKey) {
	                loadRecentContractsForCustomer.call(vm, customerKey);
	            }));
	        });

	        vm.selectedCustomer.subscribe(function () {
	            vm.loadCustomerContractsCommand.execute();
	        });
	        //#endregion
	        vm.contractClicked = contractClicked.bind(vm);
	        loadCustomers.call(vm);

	        vm.goToContract = function(key) {
	          if (key === undefined || key === null) {
	            page('/');
	          } else if (key === 'new') {
	            page('/new');
	          } else {
	            page('/' + key);
	          }
	        };
	        // Routing
	        vm.previousKey = '';
	        page.base('/Customers/Contracts');
	        page('/:key?', navigateToKey);

	        function navigateToKey(ctx) {
	          var key = ctx.params.key;

	          if (key === undefined) {
	            document.title = 'Customer Contracts';
	            vm.selectedContract();
	          } else if (key === 'new') {
	            document.title = 'New Contract';
	            vm.selectedContract();
	            if (!vm.isClone) {
	              contractDetailsComponentVm.clearContractSelection();
	            } else {
	              vm.isClone = false;
	            }
	          } else {
	            if (vm.isUpdating || vm.isAdding) {
	              contractDetailsComponentVm.loadContractByKey(key)
	              .done(function(data, textStatus, jqXHR) {
	                if (vm.isUpdating) {
	                  contractSummariesVm.updateContractSummaryItem(key, toContractSummaryItem.call(data));
	                } else {
	                  contractSummariesVm.addContractSummaryItem(toContractSummaryItem.call(data));
	                }
	              })
	              .fail(function(jqXHR, textStatus, errorThrown) {
	              })
	              .always(function() {
	                vm.isUpdating = false;
	                vm.isAdding = false;
	              });
	            } else {
	              vm.selectedContract(key);
	            }
	          }

	          vm.previousKey = key;

	          return;
	        }

	        vm.contractDetails.subscribe(function (x) {
	              var title = 'Customer Contracts';

	              if (x && x.CustomerContractKey && x.CustomerContractKey !== 'new'){
	                title = 'Contract ' + x.CustomerContractKey + ' - ' + x.CustomerName();
	                document.title = title;
	              }
	        });

	        ko.applyBindings(vm);
	        page();
	    }

	    function contractClicked(context, args) {
	        if (!args.originalEvent) throw new Error("Invalid argument \"args\". Missing event information.");
	        var srcElement = args.originalEvent.srcElement || {};
	        var $tr = $(srcElement).closest('tr');
	        if (!$tr.length) throw new Error("Unable to find \"tr\" element for source element.");

	        var selectedContract = ko.contextFor($tr[0]).$data;
	        if (selectedContract) {
	          this.selectedContract(selectedContract.CustomerContractKey);
	          this.goToContract(selectedContract.CustomerContractKey);
	        }
	    }

	    function toContractSummaryItem() {
	        return ko.toJS({
	            CustomerContractKey: this.CustomerContractKey,
	            ContractNumber: this.ContractNumber,
	            ContractDate: this.ContractDate,
	            TermBegin: this.TermBegin,
	            TermEnd: this.TermEnd,
	            CustomerPurchaseOrder: this.CustomerPurchaseOrder,
	            ContractType: this.ContractType,
	            ContractStatus: this.ContractStatus,
	            CustomerKey: this.CustomerKey,
	            CustomerName: this.CustomerName,
	            ContactName: this.ContactName,
	            BrokerCompanyKey: this.BrokerCompanyKey,
	            BrokerCompanyName: this.BrokerCompanyName,
	            DistributionWarehouseKey: this.DistributionWarehouseKey,
	            DistributionWarehouseName: this.DistributionWarehouseName,
	            AverageBasePrice: this.averageBasePrice,
	            AverageTotalPrice: this.averageTotalPrice,
	            SumQuantity: this.totalQuantityOnContract,
	            SumWeight: this.totalPoundsOnContract,
	        });
	    }

	    function loadRecentContractsForCustomer(key, copyee) {
	        var vm = this;
	        salesService.getContractsForCustomer(key, 3)
	            .then(function(data) {
	                vm.pastContractsForCustomer(ko.utils.arrayMap(data, function(c) {
	                    return {
	                        contract: c,
	                        showDraw: ko.observable(c.CustomerContractKey == copyee),
	                    };
	                }));
	            });
	    }

	    function loadCustomers() {
	        var vm = this;
	        salesService.getCustomers()
	            .then(vm.customerFilterOptions);
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 1 */,
/* 2 */,
/* 3 */,
/* 4 */,
/* 5 */,
/* 6 */,
/* 7 */,
/* 8 */,
/* 9 */,
/* 10 */,
/* 11 */,
/* 12 */,
/* 13 */,
/* 14 */,
/* 15 */
/***/ (function(module, exports, __webpack_require__) {

	var directoryService = __webpack_require__(16);

	/**
	  * @param {string} companyKey - Target company key
	  * @param {Object} options - Contact data w/ addresses for display
	  * @param {Object} selected - Observable, container for selected address
	  */
	function AddressBookVM( params ) {
	  if ( !(this instanceof AddressBookVM) ) {
	    return new AddressBookVM( params );
	  }

	  var self = this;

	  // Data
	  var selectedContact = params.selected || ko.observable( null );
	  var companyKey = params.companyKey;

	  this.contacts = ko.observableArray( [] );
	  if ( ko.isObservable( params.options ) ) {
	    params.options.subscribe(function( opts ) {
	      self.contacts( mapContacts( opts ) );
	    });
	  }

	  function mapContacts( contacts ) {
	    var _contacts = ko.unwrap( contacts );

	    return ko.utils.arrayMap( _contacts, function( contact ) {
	      ko.utils.arrayForEach( contact.Addresses, function( addr ) {
	        addr.ContactKey = contact.ContactKey;
	        addr.CompanyKey = contact.CompanyKey;
	        addr.Name = contact.Name;
	        addr.Phone = contact.PhoneNumber || contact.Phone;
	        addr.Fax = contact.Fax;
	        addr.EMail = contact.EMailAddress || contact.EMail;
	        addr.Address.CityStatePost = "".concat( addr.Address.City, ', ', addr.Address.State, ' ', addr.Address.PostalCode );
	        addr.isSelected = ko.pureComputed(function() {
	          var selected = selectedContact();

	          return addr.Address === (selected && selected.Address);
	        });
	      });

	      return contact;
	    });
	  }

	  // Behaviors
	  this.select = function( data, element ) {
	    if ( data.hasOwnProperty('Address') ) {
	      var contactData = data;

	      selectedContact( contactData );
	    }
	  };

	  this.contacts( mapContacts( params.options ) );

	  // Contact editing
	  function Address( addr ) {
	    var _address = addr.Address || {};

	    this.ContactAddressKey = addr.ContactAddressKey;
	    this.AddressDescription = ko.observable( addr.AddressDescription );
	    this.Address = {
	      AddressLine1: ko.observable( _address.AddressLine1 ),
	      AddressLine2: ko.observable( _address.AddressLine2 ),
	      AddressLine3: ko.observable( _address.AddressLine3 ),
	      City: ko.observable( _address.City ),
	      State: ko.observable( _address.State ),
	      Country: ko.observable( _address.Country ),
	      PostalCode: ko.observable( _address.PostalCode ),
	    };

	    this.Address.CityStatePost = ko.pureComputed(function() {
	      return '' + this.Address.City() + ', ' + this.Address.State() + ' ' + this.Address.PostalCode();

	    }, this);
	  }

	  function ContactInfo( contact ) {
	    this.isNew = !contact.ContactKey;

	    this.ContactKey = contact.ContactKey;
	    this.CompanyKey = companyKey;
	    this.Name = ko.observable( contact.Name );
	    this.Phone = ko.observable( contact.Phone );
	    this.EMail = ko.observable( contact.EMail );

	    this.selectedAddress = ko.observable();

	    var mapAddr = function( addr ) {
	      var mappedAddr = new Address( addr );
	      mappedAddr.isSelected = ko.pureComputed(function() {
	        var selected = this.selectedAddress();

	        return mappedAddr === selected;
	      }, this);

	      return mappedAddr;
	    }.bind( this );

	    var _contactAddresses = ko.utils.arrayFirst( self.contacts(), function( contact ) {
	      return contact.ContactKey === this.ContactKey;
	    }, this);
	    var _addressesCache = JSON.parse( ko.toJSON( _contactAddresses && _contactAddresses.Addresses ) );
	    var _addresses = ko.utils.arrayMap( _addressesCache, mapAddr);

	    this.Addresses = ko.observableArray( _addresses );

	    this.selectAddress = function( data, element ) {
	      if ( this.selectedAddress() === data ) {
	        this.selectedAddress( null );
	      } else {
	        this.selectedAddress( data );
	      }
	    }.bind( this );

	    if ( contact.selectedAddress ) {
	      var _initalAddress = ko.utils.arrayFirst( this.Addresses(), function( addr ) {
	        return addr.ContactAddressKey === contact.selectedAddress;
	      });

	      this.selectedAddress( _initalAddress );
	    }

	    this.addAddress = function() {
	      var _newAddr = mapAddr( {} );

	      this.Addresses.push( _newAddr );
	      this.selectedAddress( _newAddr );
	    }.bind( this );
	  }

	  ContactInfo.prototype.toDto = function() {
	    var _data = {
	      Name: this.Name,
	      PhoneNumber: this.Phone,
	      EmailAddress: this.EMail,
	      Addresses: this.Addresses,
	      ContactKey: this.ContactKey
	    };

	    var _addr = ko.toJS( this.Address );
	    _data.CompanyKey = companyKey;

	    return ko.toJS( _data );
	  };

	  this.editorData = ko.observable( null );

	  this.startNewContact = function Name( Parameters ) {
	    self.editorData( new ContactInfo({
	      CompanyKey: companyKey
	    }) );
	  };

	  this.editContact = function( contact, element ) {
	    var _parentData = ko.toJS( ko.contextFor( element.target ).$parent );

	    var _contactData = {};
	    _contactData.CompanyKey = companyKey;
	    _contactData.ContactKey = _parentData.ContactKey;
	    _contactData.Name = _parentData.Name;
	    _contactData.Phone = _parentData.Phone;
	    _contactData.EMail = _parentData.EMail;
	    _contactData.Addresses = _parentData.Addresses;
	    _contactData.selectedAddress = _parentData.ContactAddressKey;

	    self.editorData( new ContactInfo( _contactData ) );
	  };

	  function createContact( companyKey, contactData ) {
	    return directoryService.createContact( companyKey, contactData ).then(
	    function( data, textStatus, jqXHR ) {
	      return data;
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not save new contact', {
	        description: errorThrown
	      });
	    });
	  }

	  function updateContact( contactKey, contactData ) {
	    return directoryService.updateContact( contactKey, contactData ).then(
	    function( data, textStatus, jqXHR ) {
	      return data;
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not update contact info', {
	        description: errorThrown
	      });
	    });
	  }

	  function deleteContact( contactKey ) {
	    return directoryService.deleteContact( contactKey ).then(
	    function( data, textStatus, jqXHR ) {
	      return data;
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not remove contact', {
	        description: errorThrown
	      });
	    });
	  }

	  this.saveContact = ko.asyncCommand({
	    execute: function( complete ) {
	      var _editor = self.editorData();
	      var _data = _editor.toDto();
	      var _contactKey = _data.ContactKey;
	      var _companyKey = _data.CompanyKey;

	      var save;
	      if ( _editor.isNew ) {
	        save = createContact( _companyKey, _data ).then( null );
	      } else {
	        save = updateContact( _contactKey, _data ).then( null );
	      }

	      var getContacts = save.then(
	      function( data, textStatus, jqXHR ) {
	        return directoryService.getContacts( _companyKey ).then(
	        function( data, textStatus, jqXHR ) {
	          self.contacts( mapContacts( data ) );

	          // Close editor
	          self.editorData( null );
	        });
	      }).always( complete );

	      return getContacts;
	    }
	  });

	  this.removeContact = ko.asyncCommand({
	    execute: function( complete ) {
	      var _data = self.editorData();
	      var _contactKey = _data.ContactKey;

	      showUserMessage( 'Delete contact address?', {
	        description: 'Are you sure you want to remove this address? This action cannot be undone.',
	        type: 'yesno',
	        onYesClick: function() {
	          var remove = deleteContact( _contactKey ).then( null );

	          var getContacts = remove.then(
	            function( data, textStatus, jqXHR ) {
	            return directoryService.getContacts( _companyKey ).then(
	              function( data, textStatus, jqXHR ) {
	              self.contacts( data );

	              // Close editor
	              self.editorData( null );
	            });
	          }).always( complete );
	        },
	        onNoClick: function() { complete(); }
	      });
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting;
	    }
	  });

	  this.cancelEdit = function() {
	    self.editorData( null );
	  };

	  return this;
	}

	module.exports = {
	  viewModel: AddressBookVM,
	  template: __webpack_require__(17)
	};


/***/ }),
/* 16 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function(core, app) {
	    return {
	        getCompanies: curryGetCompaniesDelegate(),
	        getCompaniesDataPager: getCompaniesDataPager,
	        getCompanyDetails: curryGetCompanyByKeyDelegate,
	        getCompanyData: getCompanyData,
	        getCustomers: curryGetCompaniesDelegate(app.lists.companyTypes.Customer.key),
	        getDehydrators: curryGetCompaniesDelegate(app.lists.companyTypes.Dehydrator.key),
	        getBrokers: curryGetCompaniesDelegate(app.lists.companyTypes.Broker.key),
	        getVendors: function (vendorType) { return core.ajax(getCompaniesUrlBuilder(vendorType)); },
	        getVendorDetails: curryGetCompanyByKeyDelegate,
	        createVendor: createVendor,
	        createCompany: createCompany,
	        updateCompany: updateCompany,
	        getContacts: getContacts,
	        createContact: createContact,
	        updateContact: updateContact,
	        deleteContact: deleteContact,
	        getNoteTypes: getNoteTypes,
	        createNote: createNote,
	        updateNote: updateNote,
	        deleteNote: deleteNote
	    };

	    function createCompany( companyData ) {
	      return core.ajaxPost( '/api/companies/', companyData );
	    }

	    function updateCompany( companyKey, companyData ) {
	      return core.ajaxPut( '/api/companies/' + companyKey, companyData );
	    }

	    function createVendor( data ) {
	      var _data = data;

	      // "1" = Supplier
	      _data.VendorTypes = [1];

	      return core.ajaxPost( '/api/vendors/', _data );
	    }

	    function getCompanyData( companyKey ) {
	      return core.ajax( '/api/companies/' + companyKey );
	    }

	    function getCompaniesDataPager( options ) {
	      options = options || {};
	      return core.pagedDataHelper.init({
	        urlBase: "/api/companies",
	        pageSize: options.pageSize || 50,
	        parameters: options.parameters,
	        resultCounter: function (data) {
	          return data.length;
	        },
	        onNewPageSet: options.onNewPageSet
	      });
	    }
	    function curryGetCompaniesDelegate (companyType) {
	        return function () { return core.ajax(getCompaniesUrlBuilder(companyType)); };
	    }

	    function curryGetCompanyByKeyDelegate(companyKey) {
	        return function () {
	          return core.ajax(getCompanyByKeyUrlBuilder(companyKey));
	        };
	    }
	    function getCompanyByKeyUrlBuilder(companyKey) {
	        return function () {
	            return '/api/companies/' + companyKey;
	        };
	    }
	    function getCompaniesUrlBuilder(companyType) {
	        return function () {
	            return '/api/companies' + (companyType == null ? "" : "?companyType=" + companyType);
	        };
	    }

	    function getContacts( companyKey ) {
	      return core.ajax( '/api/companies/' + companyKey + '/contacts' );
	    }

	    function createContact( companyKey, contactData ) {
	      return core.ajaxPost('/api/companies/' + companyKey + '/contacts', contactData );
	    }

	    function updateContact( contactKey, contactData ) {
	      return core.ajaxPut('/api/contacts/' + contactKey, contactData );
	    }

	    function deleteContact( contactKey ) {
	      return core.ajaxDelete('/api/contacts/' + contactKey );
	    }

	    function getNoteTypes() {
	      return core.ajax('/api/profilenotes/types');
	    }
	    function createNote( companyKey, note ) {
	      return core.ajaxPost( '/api/companies/' + companyKey + '/notes/', note );
	    }
	    function updateNote( companyKey, noteId, note ) {
	      return core.ajaxPut( '/api/companies/' + companyKey + '/notes/' + noteId, note );
	    }
	    function deleteNote( companyKey, noteId ) {
	      return core.ajaxDelete( '/api/companies/' + companyKey + '/notes/' + noteId );
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 17 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"panel panel-primary\">\n  <div class=\"panel-heading\">\n    <h4 class=\"panel-title\" data-bind=\"text: editorData() ? 'Edit a contact' : 'Contacts'\">Contacts</h4>\n  </div>\n  <div class=\"panel-body\">\n    <div class=\"address-book\">\n      <div data-bind=\"template: editorData() ? 'address-book-editor' : 'address-book-contact'\">\n      </div>\n    </div>\n  </div>\n  <div class=\"panel-footer\">\n    <div class=\"text-right\" data-bind=\"visible: editorData\">\n      <button class=\"btn btn-default\" data-bind=\"click: cancelEdit\">Cancel</button>\n      <button class=\"btn btn-primary\" data-bind=\"command: saveContact\">Save</button>\n    </div>\n  </div>\n</div>\n\n<script id=\"address-book-editor\" type=\"text/html\">\n  <!-- ko with: editorData -->\n  <section>\n    <button class=\"btn btn-danger btn-sm pull-right\" data-bind=\"command: $parent.removeContact\"><i class=\"fa fa-trash\"></i> Delete Contact</button>\n    <h4>Contact Info</h4>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-line-1\">Name</label>\n      <input class=\"form-control\" id=\"editor-address-line-1\" type=\"text\" data-bind=\"textinput: Name\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-line-1\">Phone</label>\n      <input class=\"form-control\" id=\"editor-address-line-1\" type=\"text\" data-bind=\"textinput: Phone\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-line-1\">Email</label>\n      <input class=\"form-control\" id=\"editor-address-line-1\" type=\"text\" data-bind=\"textinput: EMail\">\n    </div>\n  </section>\n  <hr>\n  <section>\n    <h4>Addresses</h4>\n    <div>\n        <div class=\"address-list row\">\n            <!-- ko foreach: Addresses -->\n            <div class=\"col-md-6 col-lg-4\">\n              <a href=\"#\" class=\"contact-address btn btn-block\" data-bind=\"css: { 'btn-primary': isSelected, 'btn-default': !isSelected() }, click: $parent.selectAddress, with: Address\">\n                <p data-bind=\"text: $parent.AddressDescription, visible: $parent.AddressDescription\"></p>\n                <span class=\"center-block\" data-bind=\"text: AddressLine1\"></span>\n                <span class=\"center-block\" data-bind=\"text: AddressLine2\"></span>\n                <span class=\"center-block\" data-bind=\"text: AddressLine3\"></span>\n                <span class=\"center-block\" data-bind=\"text: CityStatePost\"></span>\n              </a>\n            </div>\n            <!-- /ko -->\n        </div>\n        <div class=\"address-list row\">\n            <div class=\"col-md-6 col-lg-4\">\n              <button class=\"contact-address btn btn-default btn-block\" data-bind=\"click: addAddress, visible: isNew\">\n                <p><i class=\"fa fa-plus-square\"></i> New Address</p>\n              </button>\n            </div>\n        </div>\n    </div>\n  </section>\n  <section data-bind=\"with: selectedAddress\">\n    <hr>\n    <!-- ko with: Address -->\n    <h4>Address Details</h4>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-line-1\">Address Line 1</label>\n      <input class=\"form-control\" id=\"editor-address-line-1\" type=\"text\" data-bind=\"textinput: AddressLine1\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-line-2\">Address Line 2</label>\n      <input class=\"form-control\" id=\"editor-address-line-2\" type=\"text\" data-bind=\"textinput: AddressLine2\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-line-3\">Address Line 3</label>\n      <input class=\"form-control\" id=\"editor-address-line-3\" type=\"text\" data-bind=\"textinput: AddressLine3\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-city\">City</label>\n      <input class=\"form-control\" id=\"editor-city\" type=\"text\" data-bind=\"textinput: City\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-state\">State</label>\n      <input class=\"form-control\" id=\"editor-state\" type=\"text\" data-bind=\"textinput: State\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-postal-code\">Postal Code</label>\n      <input class=\"form-control\" id=\"editor-postal-code\" type=\"text\" data-bind=\"textinput: PostalCode\">\n    </div>\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-country\">Country</label>\n      <input class=\"form-control\" id=\"editor-country\" type=\"text\" data-bind=\"textinput: Country\">\n    </div>\n    <!-- /ko -->\n\n    <div class=\"form-group\">\n      <label class=\"control-label\" for=\"editor-address-description\">Description</label>\n      <textarea class=\"form-control vertical-resize\" id=\"editor-address-description\" data-bind=\"textinput: AddressDescription\"></textarea>\n    </div>\n  </section>\n  <!-- /ko -->\n</script>\n<script id=\"address-book-contact\" type=\"text/html\">\n<button class=\"btn btn-default btn-sm pull-right\" data-bind=\"click: startNewContact\"><i class=\"fa fa-plus\"></i> Add Contact</button>\n<div class=\"well\" data-bind=\"visible: contacts().length === 0\">No contacts for the selected company</div>\n<!-- ko foreach: contacts -->\n<section class=\"address-book-contact container-fluid\">\n    <h4 class=\"contact-name\" data-bind=\"text: Name || '( No name )'\"></h4>\n\n    <div class=\"address-list row\" data-bind=\"foreach: Addresses\">\n        <div class=\"col-md-6 col-lg-4\">\n          <a href=\"#\" class=\"contact-address btn btn-block\" data-bind=\"css: { 'btn-primary': isSelected, 'btn-default': !isSelected() }, click: $parents[1].select, with: Address\">\n            <button class=\"pull-right btn btn-link btn-sm\" data-bind=\"click: $parents[2].editContact\">\n              <i class=\"fa fa-edit fa-lg\"></i>\n            </button>\n              <p data-bind=\"text: $parent.AddressDescription, visible: $parent.AddressDescription\"></p>\n              <span class=\"center-block\" data-bind=\"text: AddressLine1\"></span>\n              <span class=\"center-block\" data-bind=\"text: AddressLine2\"></span>\n              <span class=\"center-block\" data-bind=\"text: AddressLine3\"></span>\n              <span class=\"center-block\" data-bind=\"text: CityStatePost\"></span>\n          </a>\n        </div>\n    </div>\n</section>\n<!-- /ko -->\n</script>\n"

/***/ }),
/* 18 */
/***/ (function(module, exports, __webpack_require__) {

	var rvc = __webpack_require__(8);

	var koPunches = (function () {
	  var self = this;

	  // Behaviors
	  ko.filters.toDate = function (value) {
	    if ( value == null ) {
	      return null;
	    }

	    var dateStr = null,
	      input = new Date(ko.unwrap(value));

	      var month = (input.getUTCMonth() + 1).toString();
	      month = month.length === 2 ? month : "0" + month;

	      var day = input.getUTCDate().toString();
	      day = day.length === 2 ? day : "0" + day;

	    dateStr = month + '/' + day + '/' + input.getUTCFullYear();

	    return dateStr;
	  };

	  ko.filters.toDateTime = function ( value, format ) {
	    var dateObj;

	    if (typeof value === "string") {
	      dateObj = new Date( value );
	    }

	    if ( !(dateObj instanceof Date) ) {
	      throw new Error( 'Invalid input. Expected date but encountered ' + (typeof input) + '.' );
	    }

	    return dateObj.format( format || 'm/d/yyyy hh:MM TT' );
	  };

	  ko.filters.lotKey = function (input) {
	    var value = ko.unwrap(input);

	    if (value == undefined) {
	      return '';
	    }

	    var key = value.toString().replace(/ /g, '');
	    var keyLength = key.length;

	    if (keyLength === 0) {
	      return '';
	    } else if (keyLength <= 2) {
	      return key;
	    } else if (keyLength <= 4) {
	      return [key.substr(0, 2), key.substr(2)].join(' ');
	    } else if (keyLength <= 7) {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3)].join(' ');
	    } else {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4,3), key.substr(7)].join(' ');
	    }
	  };

	  ko.filters.toteKey = function (input) {
	    var value = ko.unwrap(input);

	    if (value == undefined) {
	      return '';
	    }

	    var key = value.toString().replace(/ /g, '');
	    if (key.length === 0) {
	      return '';
	    } else if (key.length <= 2) {
	      return key;
	    } else if (key.length <= 4) {
	      return [key.substr(0, 2), key.substr(2)].join(' ');
	    } else {
	      return [key.substr(0, 2), key.substr(2, 2), key.substr(4)].join(' ');
	    }
	  };

	    ko.filters.secToHrMin = function(value) {
	      var valueNum = +ko.unwrap(value);
	      var isNegative = valueNum < 0;

	      // Parse as positive number, Math.floor rounds negative numbers down
	      // Ex: -0.2 = -1
	      if (!isNaN(valueNum)) {
	        var secondsTotal = isNegative ? -valueNum : valueNum;
	        var hours = Math.floor(secondsTotal / 3600);
	        var minutes = Math.floor((secondsTotal - (3600 * hours)) / 60);

	        return isNegative ?
	          "".concat('-', hours, 'h ', minutes, 'm') :
	          "".concat(hours, 'h ', minutes, 'm');
	      } else {
	        return '0m';
	      }
	    };

	  ko.filters.USD = function(value) {
	    var amt = parseFloat(ko.unwrap(value));

	    return typeof amt === 'number' ?
	      '$' + amt.toFixed(2) :
	      '';
	  };

	  ko.filters.toFixed = function(value, numOfDigits) {
	    var amt = parseFloat(ko.unwrap(value));

	    return typeof amt === 'number' ?
	      amt.toFixed(numOfDigits || 2) :
	      '';
	  };

	  ko.filters.toNumber = function( value ) {
	    var numValue = value != null ? +value : null;

	    if ( numValue != null ) {
	      return numValue.toLocaleString();
	    } else {
	      return null;
	    }
	  };

	  ko.filters.contractStatus = function ( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.contractStatuses;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.orderStatus = function ( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.orderStatus;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.sampleStatus = function( value ) {
	    var input = ko.unwrap( value );
	    var statuses = rvc.lists.sampleStatusTypes;
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statuses[ statusKey ].value;
	  };

	  ko.filters.statusType = function( value, statusName ) {
	    var input = ko.unwrap( value );

	    if ( input == null ) {
	      return;
	    }

	    var statuses = rvc.lists[ statusName ];
	    var statusKey = ko.utils.arrayFirst( Object.keys( statuses ), function( status ) {
	      return statuses[ status ].key === input;
	    });

	    return statusKey != null ? statuses[ statusKey ].value : null;
	  };

	  ko.filters.name = function( value ) {
	    var input = ko.unwrap( value );

	    return input && input.Name;
	  };

	  ko.filters.length = function( value ) {
	    if ( typeof value === 'string' || Array.isArray( value ) ) {
	      return value.length;
	    }

	    return 0;
	  };

	  // Exports
	  return this;
	})();

	module.exports = koPunches;


/***/ }),
/* 19 */,
/* 20 */,
/* 21 */,
/* 22 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/*** IMPORTS FROM imports-loader ***/
	var require = false;

	/* http://www.reddnet.net/knockout-js-extender-for-dates-in-iso-8601-format/ 
	 * Knockout extender for dates that are round-tripped in ISO 8601 format
	 *  Depends on knockout.js and date.format.js 
	 *  Includes extensions for the date object that: 
	 *      add Date.toISOString() for browsers that do not nativly implement it
	 *      replaces Date.parse() with version to supports ISO 8601 (IE and Safari do not)
	 *  Includes example of how to use the extended binding
	 */

	(function () {
	    if (require) {
	        !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_FACTORY__ = (extendKo), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	    } else {
	        extendKo(ko);
	    }
	    
	    function extendKo(ko) {
	        ko.extenders.isoDate = function (target, formatString) {
	            target.formattedDate = ko.computed({
	                read: function () {
	                    if (!target()) {
	                        return;
	                    }
	                    var dt = new Date(Date.parse(target()));
	                    // Modified to not change to UTC. Should not effect 
	                    // anything NOT using time formatting.
	                    return dt.format(formatString);
	                    //return dt.format(formatString, true);
	                },
	                write: function (value) {
	                    // Modified from original to enable setting value to null
	                    // previously, when the value was "falsey", assignment was 
	                    // skipped all together. -- VK 5/27/13
	                    if (!value) {
	                        target(null);
	                    } else {
	                        target(new Date(Date.parse(value)).toISOString());
	                    }
	                }
	            });

	            target.asDate = ko.computed(function () {
	                return new Date(target.formattedDate());
	            });

	            //initialize with current value
	            target.formattedDate(target());

	            //return the computed observable
	            return target;
	        };
	    }
	}());


	/** from the mozilla documentation (before they implemented the function in the browser)
	 * https://developer.mozilla.org/index.php?title=en/JavaScript/Reference/Global_Objects/Date&revision=65
	 */
	(function(Date) {
	    if (!Date.prototype.toISOString) {
	        Date.prototype.toISOString = function() {
	            function pad(n) {
	                return n < 10 ? '0' + n : n;
	            }
	            return this.getUTCFullYear() + '-' + pad(this.getUTCMonth() + 1) + '-' + pad(this.getUTCDate()) + 'T' + pad(this.getUTCHours()) + ':' + pad(this.getUTCMinutes()) + ':' + pad(this.getUTCSeconds()) + 'Z';
	        };
	    }
	}(Date));

	/**
	 * Date.parse with progressive enhancement for ISO 8601 <https://github.com/csnover/js-iso8601>
	 * © 2011 Colin Snover <http://zetafleet.com>
	 * Released under MIT license.
	 */
	(function(Date) {
	    var origParse = Date.parse,
	        numericKeys = [1, 4, 5, 6, 7, 10, 11];
	    Date.parse = function(date) {
	        var timestamp, struct, minutesOffset = 0;

	        // ES5 §15.9.4.2 states that the string should attempt to be parsed as a Date Time String Format string
	        // before falling back to any implementation-specific date parsing, so that’s what we do, even if native
	        // implementations could be faster
	        //              1 YYYY                2 MM       3 DD           4 HH    5 mm       6 ss        7 msec        8 Z 9 ±    10 tzHH    11 tzmm
	        if ((struct = /^(\d{4}|[+\-]\d{6})(?:-(\d{2})(?:-(\d{2}))?)?(?:T(\d{2}):(\d{2})(?::(\d{2})(?:\.(\d{3}))?)?(?:(Z)|([+\-])(\d{2})(?::(\d{2}))?)?)?$/.exec(date))) {
	            // avoid NaN timestamps caused by “undefined” values being passed to Date.UTC
	            for (var i = 0, k;
	            (k = numericKeys[i]); ++i) {
	                struct[k] = +struct[k] || 0;
	            }

	            // allow undefined days and months
	            struct[2] = (+struct[2] || 1) - 1;
	            struct[3] = +struct[3] || 1;

	            if (struct[8] !== 'Z' && struct[9] !== 'undefined') {
	                minutesOffset = struct[10] * 60 + struct[11];

	                if (struct[9] === '+') {
	                    minutesOffset = 0 - minutesOffset;
	                }
	            }

	            timestamp = Date.UTC(struct[1], struct[2], struct[3], struct[4], struct[5] + minutesOffset, struct[6], struct[7]);
	        }
	        else {
	            timestamp = origParse ? origParse(date) : NaN;
	        }

	        return timestamp;
	    };
	}(Date));


/***/ }),
/* 23 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function($) {(function() {
	  ko.bindingHandlers.datePicker = {
	    init: function (element, valueAccessor, allBindings) {
	      $(element).wrap('<div class="input-group"></div>');
	      $(element).datepicker({
	        showOn: 'button',
	        buttonText: '<i class="fa fa-calendar"></i>',
	        changeMonth: true,
	        changeYear: true
	      }).next(".ui-datepicker-trigger")
	          .addClass("btn btn-default")
	          .attr( 'tabindex', '-1' )
	          .wrap('<span class="input-group-btn"></span>');

	      ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	        //todo: cleanup wrapper element
	        $(element).datepicker('destroy');
	      });

	      var value = valueAccessor();
	      if (ko.isObservable(value)) {
	        ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
	      }
	    }
	  };
	}());

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 24 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(8)], __WEBPACK_AMD_DEFINE_RESULT__ = function(core, app) {
	    function getProductsByInventoryType(inventoryType, options) {
	      options = options || {};
	      options = $.extend({
	        filterProductsWithInventory: false,
	        includeInactive: false
	      }, options);

	      var url = ['/api/products/', inventoryType].join('');

	      var qs = [];
	      if (options.lotType != null) {
	        qs.push('lotType=' + options.lotType);
	      }
	      if (options.filterProductsWithInventory) {
	        qs.push('filterProductsWithInventory=true');
	      }
	      if (options.includeInactive) {
	        qs.push('includeInactive=true');
	      }

	      if (qs.length > 0) {
	        url += '?' + qs.join('&');
	      }

	      return core.ajax(url, options);
	    }

	    function getProductsByLotType(lotType, options) {
	      var inventoryType = app.lists.lotTypes.findByKey(lotType).inventoryType.key;
	      options = options || {};
	      options.lotType = lotType;
	      return getProductsByInventoryType(inventoryType, options);
	    }

	    return {
	        getChileProducts: function (chileState) {
	            if (chileState && typeof chileState === "object") chileState = chileState.key;
	            return core.ajax(core.buildUrl(buildChileProductsUrl, chileState));
	        },
	        getPackagingProducts: function (options) { return getProductsByLotType(app.lists.lotTypes.Packaging.key, options); },
	        getProductTypeAttributes: function () {
	            return core.ajax("/api/productTypeAttributes");
	        },
	        getCustomerProducts: function( customerKey ) {
	          return core.ajax( '/api/customers/' + customerKey + '/productspecs' );
	        },
	        getCustomerProductDetails: function( customerKey, productKey ) {
	          return core.ajax( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
	        },
	        createCustomerProductOverride: function( customerKey, productKey, overrideData ) {
	          return core.ajaxPost( '/api/customers/' + customerKey + '/productSpecs/' + productKey, overrideData );
	        },
	        deleteCustomerProductOverride: function( customerKey, productKey ) {
	          return core.ajaxDelete( '/api/customers/' + customerKey + '/productSpecs/' + productKey );
	        },
	        getProductDetails: core.setupFn(getProductDetails, buildProductUrl),
	        getProductsByLotType: getProductsByLotType,
	        getProductsByInventoryType: getProductsByInventoryType,
	        getChileVarieties: function() {
	          return core.ajax('/api/chilevarities');
	        },
	        getChileTypes: function () {
	            return core.ajax("/api/chileTypes");
	        },
	        getAdditiveTypes: function () {
	            return core.ajax("/api/additiveTypes");
	        },
	        getProductionLocations: function() {
	          return core.ajax('/api/productionlines');
	        },
	        createProduct: function( data ) {
	          return core.ajaxPost( '/api/products', data );
	        },
	        updateProduct: function( productCode, data ) {
	          return core.ajaxPut( '/api/products/' + productCode, data );
	        },
	        setProductIngredients: function( productKey, data ) {
	          return core.ajaxPost( '/api/products/' + productKey + '/ingredients', data );
	        },
	        setProductAttributes: function( productKey, data ) {
	          return core.ajaxPost( '/api/products/' + productKey + '/specs', data );
	        },
	        getProductionSchedulesDataPager: function( options ) {
	          options = options || {};

	          return core.pagedDataHelper.init({
	              urlBase: options.baseUrl || "/api/productionschedules",
	              pageSize: options.pageSize || 50,
	              parameters: options.parameters,
	              onNewPageSet: options.onNewPageSet,
	              onEndOfResults: options.onEndOfResults
	          });
	        },
	        getProductionScheduleDetails: function( key ) {
	          return core.ajax( '/api/productionschedules/' + key );
	        },
	        createProductionSchedule: function( data ) {
	          return core.ajaxPost( '/api/productionschedules/', data );
	        },
	        updateProductionSchedule: function( key, data ) {
	          return core.ajaxPut( '/api/productionschedules/' + key, data );
	        },
	        deleteProductionSchedule: function( key ) {
	          return core.ajaxDelete( '/api/productionschedules/' + key );
	        }
	    };

	    //#region function delegates
	    function getProductDetails(lotType, key) {
	        return core.ajax(buildProductUrl(lotType, key));
	    }
	    //#endregion

	    function buildProductUrl(lotType, key) {
	        key = key || '';
	        return '/api/products/' + lotType + (key ? '/' + key : '');
	    }
	    function buildChileProductsUrl(chileState) {
	        return '/api/chileproducts?chileState=' + chileState;
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 25 */,
/* 26 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(process) {  /* globals require, module */

	  'use strict';

	  /**
	   * Module dependencies.
	   */

	  var pathtoRegexp = __webpack_require__(28);

	  /**
	   * Module exports.
	   */

	  module.exports = page;

	  /**
	   * Detect click event
	   */
	  var clickEvent = ('undefined' !== typeof document) && document.ontouchstart ? 'touchstart' : 'click';

	  /**
	   * To work properly with the URL
	   * history.location generated polyfill in https://github.com/devote/HTML5-History-API
	   */

	  var location = ('undefined' !== typeof window) && (window.history.location || window.location);

	  /**
	   * Perform initial dispatch.
	   */

	  var dispatch = true;


	  /**
	   * Decode URL components (query string, pathname, hash).
	   * Accommodates both regular percent encoding and x-www-form-urlencoded format.
	   */
	  var decodeURLComponents = true;

	  /**
	   * Base path.
	   */

	  var base = '';

	  /**
	   * Running flag.
	   */

	  var running;

	  /**
	   * HashBang option
	   */

	  var hashbang = false;

	  /**
	   * Previous context, for capturing
	   * page exit events.
	   */

	  var prevContext;

	  /**
	   * Register `path` with callback `fn()`,
	   * or route `path`, or redirection,
	   * or `page.start()`.
	   *
	   *   page(fn);
	   *   page('*', fn);
	   *   page('/user/:id', load, user);
	   *   page('/user/' + user.id, { some: 'thing' });
	   *   page('/user/' + user.id);
	   *   page('/from', '/to')
	   *   page();
	   *
	   * @param {string|!Function|!Object} path
	   * @param {Function=} fn
	   * @api public
	   */

	  function page(path, fn) {
	    // <callback>
	    if ('function' === typeof path) {
	      return page('*', path);
	    }

	    // route <path> to <callback ...>
	    if ('function' === typeof fn) {
	      var route = new Route(/** @type {string} */ (path));
	      for (var i = 1; i < arguments.length; ++i) {
	        page.callbacks.push(route.middleware(arguments[i]));
	      }
	      // show <path> with [state]
	    } else if ('string' === typeof path) {
	      page['string' === typeof fn ? 'redirect' : 'show'](path, fn);
	      // start [options]
	    } else {
	      page.start(path);
	    }
	  }

	  /**
	   * Callback functions.
	   */

	  page.callbacks = [];
	  page.exits = [];

	  /**
	   * Current path being processed
	   * @type {string}
	   */
	  page.current = '';

	  /**
	   * Number of pages navigated to.
	   * @type {number}
	   *
	   *     page.len == 0;
	   *     page('/login');
	   *     page.len == 1;
	   */

	  page.len = 0;

	  /**
	   * Get or set basepath to `path`.
	   *
	   * @param {string} path
	   * @api public
	   */

	  page.base = function(path) {
	    if (0 === arguments.length) return base;
	    base = path;
	  };

	  /**
	   * Bind with the given `options`.
	   *
	   * Options:
	   *
	   *    - `click` bind to click events [true]
	   *    - `popstate` bind to popstate [true]
	   *    - `dispatch` perform initial dispatch [true]
	   *
	   * @param {Object} options
	   * @api public
	   */

	  page.start = function(options) {
	    options = options || {};
	    if (running) return;
	    running = true;
	    if (false === options.dispatch) dispatch = false;
	    if (false === options.decodeURLComponents) decodeURLComponents = false;
	    if (false !== options.popstate) window.addEventListener('popstate', onpopstate, false);
	    if (false !== options.click) {
	      document.addEventListener(clickEvent, onclick, false);
	    }
	    if (true === options.hashbang) hashbang = true;
	    if (!dispatch) return;
	    var url = (hashbang && ~location.hash.indexOf('#!')) ? location.hash.substr(2) + location.search : location.pathname + location.search + location.hash;
	    page.replace(url, null, true, dispatch);
	  };

	  /**
	   * Unbind click and popstate event handlers.
	   *
	   * @api public
	   */

	  page.stop = function() {
	    if (!running) return;
	    page.current = '';
	    page.len = 0;
	    running = false;
	    document.removeEventListener(clickEvent, onclick, false);
	    window.removeEventListener('popstate', onpopstate, false);
	  };

	  /**
	   * Show `path` with optional `state` object.
	   *
	   * @param {string} path
	   * @param {Object=} state
	   * @param {boolean=} dispatch
	   * @param {boolean=} push
	   * @return {!Context}
	   * @api public
	   */

	  page.show = function(path, state, dispatch, push) {
	    var ctx = new Context(path, state);
	    page.current = ctx.path;
	    if (false !== dispatch) page.dispatch(ctx);
	    if (false !== ctx.handled && false !== push) ctx.pushState();
	    return ctx;
	  };

	  /**
	   * Goes back in the history
	   * Back should always let the current route push state and then go back.
	   *
	   * @param {string} path - fallback path to go back if no more history exists, if undefined defaults to page.base
	   * @param {Object=} state
	   * @api public
	   */

	  page.back = function(path, state) {
	    if (page.len > 0) {
	      // this may need more testing to see if all browsers
	      // wait for the next tick to go back in history
	      history.back();
	      page.len--;
	    } else if (path) {
	      setTimeout(function() {
	        page.show(path, state);
	      });
	    }else{
	      setTimeout(function() {
	        page.show(base, state);
	      });
	    }
	  };


	  /**
	   * Register route to redirect from one path to other
	   * or just redirect to another route
	   *
	   * @param {string} from - if param 'to' is undefined redirects to 'from'
	   * @param {string=} to
	   * @api public
	   */
	  page.redirect = function(from, to) {
	    // Define route from a path to another
	    if ('string' === typeof from && 'string' === typeof to) {
	      page(from, function(e) {
	        setTimeout(function() {
	          page.replace(/** @type {!string} */ (to));
	        }, 0);
	      });
	    }

	    // Wait for the push state and replace it with another
	    if ('string' === typeof from && 'undefined' === typeof to) {
	      setTimeout(function() {
	        page.replace(from);
	      }, 0);
	    }
	  };

	  /**
	   * Replace `path` with optional `state` object.
	   *
	   * @param {string} path
	   * @param {Object=} state
	   * @param {boolean=} init
	   * @param {boolean=} dispatch
	   * @return {!Context}
	   * @api public
	   */


	  page.replace = function(path, state, init, dispatch) {
	    var ctx = new Context(path, state);
	    page.current = ctx.path;
	    ctx.init = init;
	    ctx.save(); // save before dispatching, which may redirect
	    if (false !== dispatch) page.dispatch(ctx);
	    return ctx;
	  };

	  /**
	   * Dispatch the given `ctx`.
	   *
	   * @param {Context} ctx
	   * @api private
	   */
	  page.dispatch = function(ctx) {
	    var prev = prevContext,
	      i = 0,
	      j = 0;

	    prevContext = ctx;

	    function nextExit() {
	      var fn = page.exits[j++];
	      if (!fn) return nextEnter();
	      fn(prev, nextExit);
	    }

	    function nextEnter() {
	      var fn = page.callbacks[i++];

	      if (ctx.path !== page.current) {
	        ctx.handled = false;
	        return;
	      }
	      if (!fn) return unhandled(ctx);
	      fn(ctx, nextEnter);
	    }

	    if (prev) {
	      nextExit();
	    } else {
	      nextEnter();
	    }
	  };

	  /**
	   * Unhandled `ctx`. When it's not the initial
	   * popstate then redirect. If you wish to handle
	   * 404s on your own use `page('*', callback)`.
	   *
	   * @param {Context} ctx
	   * @api private
	   */
	  function unhandled(ctx) {
	    if (ctx.handled) return;
	    var current;

	    if (hashbang) {
	      current = base + location.hash.replace('#!', '');
	    } else {
	      current = location.pathname + location.search;
	    }

	    if (current === ctx.canonicalPath) return;
	    page.stop();
	    ctx.handled = false;
	    location.href = ctx.canonicalPath;
	  }

	  /**
	   * Register an exit route on `path` with
	   * callback `fn()`, which will be called
	   * on the previous context when a new
	   * page is visited.
	   */
	  page.exit = function(path, fn) {
	    if (typeof path === 'function') {
	      return page.exit('*', path);
	    }

	    var route = new Route(path);
	    for (var i = 1; i < arguments.length; ++i) {
	      page.exits.push(route.middleware(arguments[i]));
	    }
	  };

	  /**
	   * Remove URL encoding from the given `str`.
	   * Accommodates whitespace in both x-www-form-urlencoded
	   * and regular percent-encoded form.
	   *
	   * @param {string} val - URL component to decode
	   */
	  function decodeURLEncodedURIComponent(val) {
	    if (typeof val !== 'string') { return val; }
	    return decodeURLComponents ? decodeURIComponent(val.replace(/\+/g, ' ')) : val;
	  }

	  /**
	   * Initialize a new "request" `Context`
	   * with the given `path` and optional initial `state`.
	   *
	   * @constructor
	   * @param {string} path
	   * @param {Object=} state
	   * @api public
	   */

	  function Context(path, state) {
	    if ('/' === path[0] && 0 !== path.indexOf(base)) path = base + (hashbang ? '#!' : '') + path;
	    var i = path.indexOf('?');

	    this.canonicalPath = path;
	    this.path = path.replace(base, '') || '/';
	    if (hashbang) this.path = this.path.replace('#!', '') || '/';

	    this.title = document.title;
	    this.state = state || {};
	    this.state.path = path;
	    this.querystring = ~i ? decodeURLEncodedURIComponent(path.slice(i + 1)) : '';
	    this.pathname = decodeURLEncodedURIComponent(~i ? path.slice(0, i) : path);
	    this.params = {};

	    // fragment
	    this.hash = '';
	    if (!hashbang) {
	      if (!~this.path.indexOf('#')) return;
	      var parts = this.path.split('#');
	      this.path = parts[0];
	      this.hash = decodeURLEncodedURIComponent(parts[1]) || '';
	      this.querystring = this.querystring.split('#')[0];
	    }
	  }

	  /**
	   * Expose `Context`.
	   */

	  page.Context = Context;

	  /**
	   * Push state.
	   *
	   * @api private
	   */

	  Context.prototype.pushState = function() {
	    page.len++;
	    history.pushState(this.state, this.title, hashbang && this.path !== '/' ? '#!' + this.path : this.canonicalPath);
	  };

	  /**
	   * Save the context state.
	   *
	   * @api public
	   */

	  Context.prototype.save = function() {
	    history.replaceState(this.state, this.title, hashbang && this.path !== '/' ? '#!' + this.path : this.canonicalPath);
	  };

	  /**
	   * Initialize `Route` with the given HTTP `path`,
	   * and an array of `callbacks` and `options`.
	   *
	   * Options:
	   *
	   *   - `sensitive`    enable case-sensitive routes
	   *   - `strict`       enable strict matching for trailing slashes
	   *
	   * @constructor
	   * @param {string} path
	   * @param {Object=} options
	   * @api private
	   */

	  function Route(path, options) {
	    options = options || {};
	    this.path = (path === '*') ? '(.*)' : path;
	    this.method = 'GET';
	    this.regexp = pathtoRegexp(this.path,
	      this.keys = [],
	      options);
	  }

	  /**
	   * Expose `Route`.
	   */

	  page.Route = Route;

	  /**
	   * Return route middleware with
	   * the given callback `fn()`.
	   *
	   * @param {Function} fn
	   * @return {Function}
	   * @api public
	   */

	  Route.prototype.middleware = function(fn) {
	    var self = this;
	    return function(ctx, next) {
	      if (self.match(ctx.path, ctx.params)) return fn(ctx, next);
	      next();
	    };
	  };

	  /**
	   * Check if this route matches `path`, if so
	   * populate `params`.
	   *
	   * @param {string} path
	   * @param {Object} params
	   * @return {boolean}
	   * @api private
	   */

	  Route.prototype.match = function(path, params) {
	    var keys = this.keys,
	      qsIndex = path.indexOf('?'),
	      pathname = ~qsIndex ? path.slice(0, qsIndex) : path,
	      m = this.regexp.exec(decodeURIComponent(pathname));

	    if (!m) return false;

	    for (var i = 1, len = m.length; i < len; ++i) {
	      var key = keys[i - 1];
	      var val = decodeURLEncodedURIComponent(m[i]);
	      if (val !== undefined || !(hasOwnProperty.call(params, key.name))) {
	        params[key.name] = val;
	      }
	    }

	    return true;
	  };


	  /**
	   * Handle "populate" events.
	   */

	  var onpopstate = (function () {
	    var loaded = false;
	    if ('undefined' === typeof window) {
	      return;
	    }
	    if (document.readyState === 'complete') {
	      loaded = true;
	    } else {
	      window.addEventListener('load', function() {
	        setTimeout(function() {
	          loaded = true;
	        }, 0);
	      });
	    }
	    return function onpopstate(e) {
	      if (!loaded) return;
	      if (e.state) {
	        var path = e.state.path;
	        page.replace(path, e.state);
	      } else {
	        page.show(location.pathname + location.hash, undefined, undefined, false);
	      }
	    };
	  })();
	  /**
	   * Handle "click" events.
	   */

	  function onclick(e) {

	    if (1 !== which(e)) return;

	    if (e.metaKey || e.ctrlKey || e.shiftKey) return;
	    if (e.defaultPrevented) return;



	    // ensure link
	    // use shadow dom when available
	    var el = e.path ? e.path[0] : e.target;
	    while (el && 'A' !== el.nodeName) el = el.parentNode;
	    if (!el || 'A' !== el.nodeName) return;



	    // Ignore if tag has
	    // 1. "download" attribute
	    // 2. rel="external" attribute
	    if (el.hasAttribute('download') || el.getAttribute('rel') === 'external') return;

	    // ensure non-hash for the same path
	    var link = el.getAttribute('href');
	    if (!hashbang && el.pathname === location.pathname && (el.hash || '#' === link)) return;



	    // Check for mailto: in the href
	    if (link && link.indexOf('mailto:') > -1) return;

	    // check target
	    if (el.target) return;

	    // x-origin
	    if (!sameOrigin(el.href)) return;



	    // rebuild path
	    var path = el.pathname + el.search + (el.hash || '');

	    // strip leading "/[drive letter]:" on NW.js on Windows
	    if (typeof process !== 'undefined' && path.match(/^\/[a-zA-Z]:\//)) {
	      path = path.replace(/^\/[a-zA-Z]:\//, '/');
	    }

	    // same page
	    var orig = path;

	    if (path.indexOf(base) === 0) {
	      path = path.substr(base.length);
	    }

	    if (hashbang) path = path.replace('#!', '');

	    if (base && orig === path) return;

	    e.preventDefault();
	    page.show(orig);
	  }

	  /**
	   * Event button.
	   */

	  function which(e) {
	    e = e || window.event;
	    return null === e.which ? e.button : e.which;
	  }

	  /**
	   * Check if `href` is the same origin.
	   */

	  function sameOrigin(href) {
	    var origin = location.protocol + '//' + location.hostname;
	    if (location.port) origin += ':' + location.port;
	    return (href && (0 === href.indexOf(origin)));
	  }

	  page.sameOrigin = sameOrigin;

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(27)))

/***/ }),
/* 27 */
/***/ (function(module, exports) {

	// shim for using process in browser
	var process = module.exports = {};

	// cached from whatever global is present so that test runners that stub it
	// don't break things.  But we need to wrap it in a try catch in case it is
	// wrapped in strict mode code which doesn't define any globals.  It's inside a
	// function because try/catches deoptimize in certain engines.

	var cachedSetTimeout;
	var cachedClearTimeout;

	function defaultSetTimout() {
	    throw new Error('setTimeout has not been defined');
	}
	function defaultClearTimeout () {
	    throw new Error('clearTimeout has not been defined');
	}
	(function () {
	    try {
	        if (typeof setTimeout === 'function') {
	            cachedSetTimeout = setTimeout;
	        } else {
	            cachedSetTimeout = defaultSetTimout;
	        }
	    } catch (e) {
	        cachedSetTimeout = defaultSetTimout;
	    }
	    try {
	        if (typeof clearTimeout === 'function') {
	            cachedClearTimeout = clearTimeout;
	        } else {
	            cachedClearTimeout = defaultClearTimeout;
	        }
	    } catch (e) {
	        cachedClearTimeout = defaultClearTimeout;
	    }
	} ())
	function runTimeout(fun) {
	    if (cachedSetTimeout === setTimeout) {
	        //normal enviroments in sane situations
	        return setTimeout(fun, 0);
	    }
	    // if setTimeout wasn't available but was latter defined
	    if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
	        cachedSetTimeout = setTimeout;
	        return setTimeout(fun, 0);
	    }
	    try {
	        // when when somebody has screwed with setTimeout but no I.E. maddness
	        return cachedSetTimeout(fun, 0);
	    } catch(e){
	        try {
	            // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
	            return cachedSetTimeout.call(null, fun, 0);
	        } catch(e){
	            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
	            return cachedSetTimeout.call(this, fun, 0);
	        }
	    }


	}
	function runClearTimeout(marker) {
	    if (cachedClearTimeout === clearTimeout) {
	        //normal enviroments in sane situations
	        return clearTimeout(marker);
	    }
	    // if clearTimeout wasn't available but was latter defined
	    if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
	        cachedClearTimeout = clearTimeout;
	        return clearTimeout(marker);
	    }
	    try {
	        // when when somebody has screwed with setTimeout but no I.E. maddness
	        return cachedClearTimeout(marker);
	    } catch (e){
	        try {
	            // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
	            return cachedClearTimeout.call(null, marker);
	        } catch (e){
	            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
	            // Some versions of I.E. have different rules for clearTimeout vs setTimeout
	            return cachedClearTimeout.call(this, marker);
	        }
	    }



	}
	var queue = [];
	var draining = false;
	var currentQueue;
	var queueIndex = -1;

	function cleanUpNextTick() {
	    if (!draining || !currentQueue) {
	        return;
	    }
	    draining = false;
	    if (currentQueue.length) {
	        queue = currentQueue.concat(queue);
	    } else {
	        queueIndex = -1;
	    }
	    if (queue.length) {
	        drainQueue();
	    }
	}

	function drainQueue() {
	    if (draining) {
	        return;
	    }
	    var timeout = runTimeout(cleanUpNextTick);
	    draining = true;

	    var len = queue.length;
	    while(len) {
	        currentQueue = queue;
	        queue = [];
	        while (++queueIndex < len) {
	            if (currentQueue) {
	                currentQueue[queueIndex].run();
	            }
	        }
	        queueIndex = -1;
	        len = queue.length;
	    }
	    currentQueue = null;
	    draining = false;
	    runClearTimeout(timeout);
	}

	process.nextTick = function (fun) {
	    var args = new Array(arguments.length - 1);
	    if (arguments.length > 1) {
	        for (var i = 1; i < arguments.length; i++) {
	            args[i - 1] = arguments[i];
	        }
	    }
	    queue.push(new Item(fun, args));
	    if (queue.length === 1 && !draining) {
	        runTimeout(drainQueue);
	    }
	};

	// v8 likes predictible objects
	function Item(fun, array) {
	    this.fun = fun;
	    this.array = array;
	}
	Item.prototype.run = function () {
	    this.fun.apply(null, this.array);
	};
	process.title = 'browser';
	process.browser = true;
	process.env = {};
	process.argv = [];
	process.version = ''; // empty string to avoid regexp issues
	process.versions = {};

	function noop() {}

	process.on = noop;
	process.addListener = noop;
	process.once = noop;
	process.off = noop;
	process.removeListener = noop;
	process.removeAllListeners = noop;
	process.emit = noop;
	process.prependListener = noop;
	process.prependOnceListener = noop;

	process.listeners = function (name) { return [] }

	process.binding = function (name) {
	    throw new Error('process.binding is not supported');
	};

	process.cwd = function () { return '/' };
	process.chdir = function (dir) {
	    throw new Error('process.chdir is not supported');
	};
	process.umask = function() { return 0; };


/***/ }),
/* 28 */
/***/ (function(module, exports, __webpack_require__) {

	var isarray = __webpack_require__(29)

	/**
	 * Expose `pathToRegexp`.
	 */
	module.exports = pathToRegexp
	module.exports.parse = parse
	module.exports.compile = compile
	module.exports.tokensToFunction = tokensToFunction
	module.exports.tokensToRegExp = tokensToRegExp

	/**
	 * The main path matching regexp utility.
	 *
	 * @type {RegExp}
	 */
	var PATH_REGEXP = new RegExp([
	  // Match escaped characters that would otherwise appear in future matches.
	  // This allows the user to escape special characters that won't transform.
	  '(\\\\.)',
	  // Match Express-style parameters and un-named parameters with a prefix
	  // and optional suffixes. Matches appear as:
	  //
	  // "/:test(\\d+)?" => ["/", "test", "\d+", undefined, "?", undefined]
	  // "/route(\\d+)"  => [undefined, undefined, undefined, "\d+", undefined, undefined]
	  // "/*"            => ["/", undefined, undefined, undefined, undefined, "*"]
	  '([\\/.])?(?:(?:\\:(\\w+)(?:\\(((?:\\\\.|[^()])+)\\))?|\\(((?:\\\\.|[^()])+)\\))([+*?])?|(\\*))'
	].join('|'), 'g')

	/**
	 * Parse a string for the raw tokens.
	 *
	 * @param  {String} str
	 * @return {Array}
	 */
	function parse (str) {
	  var tokens = []
	  var key = 0
	  var index = 0
	  var path = ''
	  var res

	  while ((res = PATH_REGEXP.exec(str)) != null) {
	    var m = res[0]
	    var escaped = res[1]
	    var offset = res.index
	    path += str.slice(index, offset)
	    index = offset + m.length

	    // Ignore already escaped sequences.
	    if (escaped) {
	      path += escaped[1]
	      continue
	    }

	    // Push the current path onto the tokens.
	    if (path) {
	      tokens.push(path)
	      path = ''
	    }

	    var prefix = res[2]
	    var name = res[3]
	    var capture = res[4]
	    var group = res[5]
	    var suffix = res[6]
	    var asterisk = res[7]

	    var repeat = suffix === '+' || suffix === '*'
	    var optional = suffix === '?' || suffix === '*'
	    var delimiter = prefix || '/'
	    var pattern = capture || group || (asterisk ? '.*' : '[^' + delimiter + ']+?')

	    tokens.push({
	      name: name || key++,
	      prefix: prefix || '',
	      delimiter: delimiter,
	      optional: optional,
	      repeat: repeat,
	      pattern: escapeGroup(pattern)
	    })
	  }

	  // Match any characters still remaining.
	  if (index < str.length) {
	    path += str.substr(index)
	  }

	  // If the path exists, push it onto the end.
	  if (path) {
	    tokens.push(path)
	  }

	  return tokens
	}

	/**
	 * Compile a string to a template function for the path.
	 *
	 * @param  {String}   str
	 * @return {Function}
	 */
	function compile (str) {
	  return tokensToFunction(parse(str))
	}

	/**
	 * Expose a method for transforming tokens into the path function.
	 */
	function tokensToFunction (tokens) {
	  // Compile all the tokens into regexps.
	  var matches = new Array(tokens.length)

	  // Compile all the patterns before compilation.
	  for (var i = 0; i < tokens.length; i++) {
	    if (typeof tokens[i] === 'object') {
	      matches[i] = new RegExp('^' + tokens[i].pattern + '$')
	    }
	  }

	  return function (obj) {
	    var path = ''
	    var data = obj || {}

	    for (var i = 0; i < tokens.length; i++) {
	      var token = tokens[i]

	      if (typeof token === 'string') {
	        path += token

	        continue
	      }

	      var value = data[token.name]
	      var segment

	      if (value == null) {
	        if (token.optional) {
	          continue
	        } else {
	          throw new TypeError('Expected "' + token.name + '" to be defined')
	        }
	      }

	      if (isarray(value)) {
	        if (!token.repeat) {
	          throw new TypeError('Expected "' + token.name + '" to not repeat, but received "' + value + '"')
	        }

	        if (value.length === 0) {
	          if (token.optional) {
	            continue
	          } else {
	            throw new TypeError('Expected "' + token.name + '" to not be empty')
	          }
	        }

	        for (var j = 0; j < value.length; j++) {
	          segment = encodeURIComponent(value[j])

	          if (!matches[i].test(segment)) {
	            throw new TypeError('Expected all "' + token.name + '" to match "' + token.pattern + '", but received "' + segment + '"')
	          }

	          path += (j === 0 ? token.prefix : token.delimiter) + segment
	        }

	        continue
	      }

	      segment = encodeURIComponent(value)

	      if (!matches[i].test(segment)) {
	        throw new TypeError('Expected "' + token.name + '" to match "' + token.pattern + '", but received "' + segment + '"')
	      }

	      path += token.prefix + segment
	    }

	    return path
	  }
	}

	/**
	 * Escape a regular expression string.
	 *
	 * @param  {String} str
	 * @return {String}
	 */
	function escapeString (str) {
	  return str.replace(/([.+*?=^!:${}()[\]|\/])/g, '\\$1')
	}

	/**
	 * Escape the capturing group by escaping special characters and meaning.
	 *
	 * @param  {String} group
	 * @return {String}
	 */
	function escapeGroup (group) {
	  return group.replace(/([=!:$\/()])/g, '\\$1')
	}

	/**
	 * Attach the keys as a property of the regexp.
	 *
	 * @param  {RegExp} re
	 * @param  {Array}  keys
	 * @return {RegExp}
	 */
	function attachKeys (re, keys) {
	  re.keys = keys
	  return re
	}

	/**
	 * Get the flags for a regexp from the options.
	 *
	 * @param  {Object} options
	 * @return {String}
	 */
	function flags (options) {
	  return options.sensitive ? '' : 'i'
	}

	/**
	 * Pull out keys from a regexp.
	 *
	 * @param  {RegExp} path
	 * @param  {Array}  keys
	 * @return {RegExp}
	 */
	function regexpToRegexp (path, keys) {
	  // Use a negative lookahead to match only capturing groups.
	  var groups = path.source.match(/\((?!\?)/g)

	  if (groups) {
	    for (var i = 0; i < groups.length; i++) {
	      keys.push({
	        name: i,
	        prefix: null,
	        delimiter: null,
	        optional: false,
	        repeat: false,
	        pattern: null
	      })
	    }
	  }

	  return attachKeys(path, keys)
	}

	/**
	 * Transform an array into a regexp.
	 *
	 * @param  {Array}  path
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function arrayToRegexp (path, keys, options) {
	  var parts = []

	  for (var i = 0; i < path.length; i++) {
	    parts.push(pathToRegexp(path[i], keys, options).source)
	  }

	  var regexp = new RegExp('(?:' + parts.join('|') + ')', flags(options))

	  return attachKeys(regexp, keys)
	}

	/**
	 * Create a path regexp from string input.
	 *
	 * @param  {String} path
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function stringToRegexp (path, keys, options) {
	  var tokens = parse(path)
	  var re = tokensToRegExp(tokens, options)

	  // Attach keys back to the regexp.
	  for (var i = 0; i < tokens.length; i++) {
	    if (typeof tokens[i] !== 'string') {
	      keys.push(tokens[i])
	    }
	  }

	  return attachKeys(re, keys)
	}

	/**
	 * Expose a function for taking tokens and returning a RegExp.
	 *
	 * @param  {Array}  tokens
	 * @param  {Array}  keys
	 * @param  {Object} options
	 * @return {RegExp}
	 */
	function tokensToRegExp (tokens, options) {
	  options = options || {}

	  var strict = options.strict
	  var end = options.end !== false
	  var route = ''
	  var lastToken = tokens[tokens.length - 1]
	  var endsWithSlash = typeof lastToken === 'string' && /\/$/.test(lastToken)

	  // Iterate over the tokens and create our regexp string.
	  for (var i = 0; i < tokens.length; i++) {
	    var token = tokens[i]

	    if (typeof token === 'string') {
	      route += escapeString(token)
	    } else {
	      var prefix = escapeString(token.prefix)
	      var capture = token.pattern

	      if (token.repeat) {
	        capture += '(?:' + prefix + capture + ')*'
	      }

	      if (token.optional) {
	        if (prefix) {
	          capture = '(?:' + prefix + '(' + capture + '))?'
	        } else {
	          capture = '(' + capture + ')?'
	        }
	      } else {
	        capture = prefix + '(' + capture + ')'
	      }

	      route += capture
	    }
	  }

	  // In non-strict mode we allow a slash at the end of match. If the path to
	  // match already ends with a slash, we remove it for consistency. The slash
	  // is valid at the end of a path match, not in the middle. This is important
	  // in non-ending mode, where "/test/" shouldn't match "/test//route".
	  if (!strict) {
	    route = (endsWithSlash ? route.slice(0, -2) : route) + '(?:\\/(?=$))?'
	  }

	  if (end) {
	    route += '$'
	  } else {
	    // In non-ending mode, we need the capturing groups to match as much as
	    // possible by using a positive lookahead to the end or next path segment.
	    route += strict && endsWithSlash ? '' : '(?=\\/|$)'
	  }

	  return new RegExp('^' + route, flags(options))
	}

	/**
	 * Normalize the given path string, returning a regular expression.
	 *
	 * An empty array can be passed in for the keys, which will hold the
	 * placeholder key descriptions. For example, using `/user/:id`, `keys` will
	 * contain `[{ name: 'id', delimiter: '/', optional: false, repeat: false }]`.
	 *
	 * @param  {(String|RegExp|Array)} path
	 * @param  {Array}                 [keys]
	 * @param  {Object}                [options]
	 * @return {RegExp}
	 */
	function pathToRegexp (path, keys, options) {
	  keys = keys || []

	  if (!isarray(keys)) {
	    options = keys
	    keys = []
	  } else if (!options) {
	    options = {}
	  }

	  if (path instanceof RegExp) {
	    return regexpToRegexp(path, keys, options)
	  }

	  if (isarray(path)) {
	    return arrayToRegexp(path, keys, options)
	  }

	  return stringToRegexp(path, keys, options)
	}


/***/ }),
/* 29 */
/***/ (function(module, exports) {

	module.exports = Array.isArray || function (arr) {
	  return Object.prototype.toString.call(arr) == '[object Array]';
	};


/***/ }),
/* 30 */,
/* 31 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(8), __webpack_require__(32), __webpack_require__(34), __webpack_require__(22), __webpack_require__(35)], __WEBPACK_AMD_DEFINE_RESULT__ = function(ko, app) {
	    ko.extenders.timeEntry = function (target) {
	        var pattern = /^(\d)?(\d)?:?(\d)?(\d)?$/;
	        target.formattedTime = ko.computed({
	            read: function () {
	                if (!target()) return '00:00';
	                var val = target();
	                var formatted = val;
	                switch (val.length) {
	                    case 1:
	                        formatted = val.replace(pattern, "0$1");
	                    case 2:
	                        formatted += ":00";
	                        break;
	                    case 3:
	                        formatted = val.replace(pattern, "0$1:$2$3");
	                        break;
	                    case 4:
	                        formatted = val.replace(pattern, "$1$2:$3$4");
	                        break;
	                }
	                return formatted;
	            },
	            write: function (value) {
	                if (typeof value === "string") {
	                    var parsed = Date.parse(value);
	                    if (parsed) {
	                        var d = new Date(parsed);
	                        var hours = d.getHours();
	                        hours = hours < 10 ? ('0' + hours) : hours;
	                        var minutes = d.getMinutes();
	                        minutes = minutes < 10 ? ('0' + minutes) : minutes;
	                        value = hours + ":" + minutes;
	                    }
	                }
	                target(value);
	            }
	        });

	        target.extend({ pattern: { message: "Invalid Date", params: /^([01]\d|2[0-3]):?([0-5]\d)$/ } });
	        target.Hours = ko.computed(function () {
	            if (!target.formattedTime()) return 0;
	            return target.formattedTime().split(":")[0];
	        });
	        target.Mins = ko.computed(function () {
	            if (!target.formattedTime()) return 0;
	            return target.formattedTime().split(":")[1];
	        });
	        target.formattedTime(target());
	        return target;
	    };

	    ko.extenders.toteKey = function (target, callback) {
	        var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{4})?$/;
	        var isComplete = ko.observable(false);
	        target.formattedTote = ko.computed({
	            read: function () {
	                var value = target();
	                return formatTote(value);
	            },
	            write: function (input) {
	                var value = cleanInput(input);
	                if (target() === value) return;

	                target(value);
	                if (value && value.match(pattern)) {
	                    var formatted = formatTote(value);
	                    if (formatted.length === 10) {
	                        isComplete(true);
	                        if (typeof callback === "function") callback(formatted);
	                    }
	                }
	            },
	        });
	        target.isComplete = ko.computed(function () {
	            return isComplete();
	        });
	        target.getNextTote = function () {
	            var formatted = target.formattedTote();
	            var sequence = parseSequence();
	            if (isNaN(sequence)) return null;
	            sequence++;

	            var sequenceString = formatSequence();
	            return formatted.replace(pattern, '0$2 $4 ' + sequenceString);

	            function parseSequence() {
	                var sections = formatted.split(" ");
	                if (sections.length !== 3) return null;
	                return parseInt(sections[2]);
	            }
	            function formatSequence() {
	                var val = sequence.toString();
	                while (val.length < 4) {
	                    val = "0" + val;
	                }
	                return val;
	            }
	        };
	        target.isMatch = function (val) {
	            var formattedVal = formatTote(ko.utils.unwrapObservable(val));
	            if (!formattedVal) return false;
	            var p = new RegExp("^" + target.formattedTote() + "$");
	            return formattedVal.match(p);
	        };

	        target.extend({ throttle: 800 });
	        target.formattedTote(target());
	        return target;

	        function formatTote(input) {
	            if (input == undefined) return '';
	            if (!input.match(pattern)) return input;
	            input = input.trim();
	            return input.replace(pattern, '0$2 $4 $6').trim().replace("  ", " ");
	        }
	        function cleanInput(input) {
	            if (typeof input == "number") input = input.toString();
	            if (typeof input !== "string") return undefined;
	            return input.replace(/\s/g, '');
	        }
	    };
	    
	    ko.extenders.contractType = function (target) {
	        return new TypeExtension(target, app.lists.contractTypes.toDictionary());
	    };

	    ko.extenders.contractStatus = function (target) {
	        return new TypeExtension(target, app.lists.contractStatuses.toDictionary());
	    };

	    ko.extenders.defectResolutionType = function (target) {
	        return new TypeExtension(target, app.lists.defectResolutionTypes.toDictionary());
	    };

	    ko.extenders.defectType = function (target) {
	        return new TypeExtension(target, app.lists.defectTypes.toDictionary());
	    };
	    
	    ko.extenders.facilityType = function (target) {
	        return new TypeExtension(target, app.lists.facilityTypes.toDictionary());
	    };

	    ko.extenders.inventoryType = function (target) {
	        return new TypeExtension(target, app.lists.inventoryTypes.toObjectDictionary());
	    };

	    ko.extenders.productType = function (target) {
	      var extension = new TypeExtension(target, app.lists.inventoryTypes.toDictionary());
	      extension.trueValue = ko.pureComputed(function() {
	        var raw = target();
	        if (raw == null) return null;
	        return parseInt(raw);
	      });
	      return extension;
	    };

	    ko.extenders.locationStatusType = function (target) {
	        return new TypeExtension(target, app.lists.locationStatusTypes.toDictionary());
	    }

	    ko.extenders.lotHoldType = function (target) {
	        return new TypeExtension(target, app.lists.lotHoldTypes.toDictionary());
	    };

	    ko.extenders.lotQualityStatusType = function (target) {
	        return new TypeExtension(target, app.lists.lotQualityStatusTypes.toDictionary());
	    };

	    ko.extenders.lotType = function (target) {
	        return new TypeExtension(target, app.lists.lotTypes.toDictionary());
	    };

	    ko.extenders.lotType2 = function (target) {
	        return new TypeExtension(target, app.lists.lotTypes.toObjectDictionary());
	    };

	    ko.extenders.productionStatusType = function (target) {
	        return new TypeExtension(target, app.lists.productionStatusTypes.toDictionary());
	    };

	    ko.extenders.chileType = function (target) {
	        var options = {
	            0: 'Other Raw',
	            1: 'Dehydrated',
	            2: 'WIP',
	            3: 'Finished Goods'
	        };
	        return new TypeExtension(target, options);
	    };

	    ko.extenders.treatmentType = function (target) {
	        return new TypeExtension(target, app.lists.treatmentTypes.toDictionary());
	    };

	    ko.extenders.shipmentStatusType = function (target) {
	        return new TypeExtension(target, app.lists.shipmentStatus.toDictionary());
	    };
	    ko.extenders.orderStatusType = function (target) {
	        return new TypeExtension(target, app.lists.orderStatus.toDictionary());
	    };
	    ko.extenders.customerOrderStatusType = function (target) {
	        return new TypeExtension(target, app.lists.customerOrderStatus.toDictionary());
	    };

	    ko.extenders.movementTypes = function (target) {
	        var options = {
	            0: 'Same Warehouse',
	            1: 'Between Warehouses',
	        };
	        return new TypeExtension(target, options);
	    };

	    ko.extenders.inventoryOrderTypes = function (target, defaultOption) {
	        return new TypeExtension(target, app.lists.inventoryOrderTypes, defaultOption);
	    };

	    // Data input binding extension. Converts input to numeric values.
	    ko.extenders.numeric = function (target, precision) {
	        console.warn('Replace numeric binding extender with numericObservable object');
	        var mode = 'readonly', isWriteable = false;
	        if (!ko.isWriteableObservable(target)) {
	            mode = 'writeable';
	            isWriteable = true;
	            //throw new Error('Object must be a writableObservable in order to be used with the numeric binding. For read-only binding, use formatNumber instead.');
	        }

	        target.numericMode = mode;
	        if (isWriteable) return writable();
	        else return readonly();

	        function writable() {
	            applyFormatting(target());
	            target.subscribe(applyFormatting, target);
	            return target;

	            function applyFormatting(value) {
	                value = formatValue(value);
	                if (value === target()) return;
	                setValue(value);
	            }
	            function setValue(value) {
	                target(value);
	            }
	        }

	        function readonly() {
	            target.formattedNumber = ko.computed({
	                read: function () {
	                    return formatValue(target()) || undefined;
	                },
	                write: function (val) {
	                    target(formatValue(val) || undefined);
	                }
	            }, target);
	            return target;
	        }

	        function formatValue(input) {
	            var numVal = parseFloat(input);
	            if (isNaN(numVal)) return undefined;
	            else return parseFloat(numVal.toFixed(precision));
	        }
	    };

	    //Read-only binding for displaying numeric values with a specific decimal precision.
	    //For numeric input bindings, use the numeric binding instead.
	    ko.extenders.formatNumber = function (target, precision) {
	        function formatValue(input) {
	            precision = parseInt(precision) || 0;
	            return precision > 0 ? parseFloat(input).toFixed(precision) : parseInt(input);
	        }

	        target.formattedNumber = ko.computed(function () {
	            return formatValue(target()) || 0;
	        }, target);
	        return target;
	    };

	    //******************************
	    // MAPPING HELPERS

	    ko.mappings = ko.mappings || {};
	    ko.mappings.formattedDate = function (options, format) {
	        var dateString = options.data;
	        var date = null;
	        if (typeof dateString == "string" && dateString.length > 0) {
	            if (dateString.match(/^\/Date\(\d*\)\/$/)) {
	                dateString = dateString.replace(/[^0-9 +]/g, '');
	                dateString = parseInt(dateString);
	            }
	            date = new Date(dateString).toISOString();
	        }
	        var result = ko.observable(date).extend({ isoDate: format || 'm/d/yyyy' });
	        return result;
	    };

	    //****************************************
	    // validation rules
	    ko.validation.rules['isUnique'] = {
	        validator: function (newVal, options) {
	            if (options.predicate && typeof options.predicate !== "function")
	                throw new Error("Invalid option for isUnique validator. The 'predicate' option must be a function.");

	            var array = options.array || options;
	            var count = 0;
	            ko.utils.arrayMap(ko.utils.unwrapObservable(array), function (existingVal) {
	                if (equalityDelegate()(existingVal, newVal)) count++;
	            });
	            return count < 2;

	            function equalityDelegate() {
	                return options.predicate ? options.predicate : function (v1, v2) { return v1 === v2; };
	            }
	        },
	        message: 'This value is a duplicate',
	    };

	    /*
	     * Determines if a field is required or not based on a function or value
	     * Parameter: boolean function, or boolean value
	     * Example
	     *
	     * viewModel = {
	     *   var vm = this;     
	     *   vm.isRequired = ko.observable(false);
	     *   vm.requiredField = ko.observable().extend({ conditional_required: vm.isRequired});
	     * }   
	    */
	    ko.validation.rules['conditional_required'] = {
	        validator: function (val, condition) {
	            var required;
	            if (typeof condition == 'function') {
	                required = condition();
	            } else {
	                required = condition;
	            }

	            if (required) {
	                return !(val == undefined || val.length == 0);
	            } else {
	                return true;
	            }
	        },
	        message: "Field is required"
	    };

	    ko.validation.rules['doesNotEqual'] = {
	        validator: function (v1, v2) {
	            ko.validation.rules['doesNotEqual'].message = "\"" + v1 + "\" is not valid.";
	            return v1 !== v2;
	        },
	    };

	    ko.validation.rules['isValidTreatment'] = {
	        validator: function (val) {
	            return val !== app.lists.treatmentTypes.NotTreated.key
	                && val !== app.lists.treatmentTypes.LowBac.key;
	        },
	        message: "Invalid Treatment"
	    };

	    ko.validation.rules['isTrue'] = {
	        validator: function (value, fnInvalid) {
	            return fnInvalid.apply(value) === true;
	        },
	        message: "The new location is the same as the previous location. There is no need to create a movement if the items don't change location.",
	    };

	    ko.validation.registerExtenders();


	    //******************************************
	    // private functions

	    function TypeExtension(target, options, defaultOption) {
	        if (defaultOption === undefined && options.length) defaultOption = options[0];
	        target.displayValue = ko.computed({
	            read: function () {
	                if (target() == undefined) return '';
	                return getTypeOption(target()) || defaultOption;
	            }
	        });
	        target.options = buildSelectListOptions(options);
	        return target;

	        function buildSelectListOptions(source) {
	            var selectListOptions = [];
	            for (var opt in source) {
	                selectListOptions.push({
	                    key: opt,
	                    value: source[opt]
	                });
	            }
	            return selectListOptions;
	        }
	        function getTypeOption(val) {
	            switch (typeof val) {
	                case "string": return fromString(val);
	                case "number": return fromNumber(val);
	                case "object": return fromObject(val);
	                default: return undefined;
	            }

	            function fromString(s) {
	                return fromNumber(parseInt(s))
	                    || findOptionByName();

	                function findOptionByName() {
	                    for (var prop in options) {
	                        if (options[prop] === s) {
	                            return fromString(prop);
	                        }
	                    }
	                    return undefined;
	                }
	            }
	            function fromNumber(num) {
	                if (isNaN(num)) return undefined;
	                return options[num + ''];
	            }
	            function fromObject(o) {
	                if (!o || o.value == undefined) return undefined;
	                return o.value;
	            }
	        }
	    }

	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 32 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {__webpack_require__(33);
	__webpack_require__(23);

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function(ko) {
	    ko.bindingHandlers.hidden = {
	        update: function (element, valueAccessor) {
	            ko.bindingHandlers.visible.update(element, function () {
	                return !ko.utils.unwrapObservable(valueAccessor());
	            });
	        }
	    }
	    ko.bindingHandlers.preventBubble = {
	        init: function (element, valueAccessor) {
	            var eventName = ko.utils.unwrapObservable(valueAccessor());
	            ko.utils.registerEventHandler(element, eventName, function (event) {
	                event.cancelBubble = true;
	                if (event.stopPropagation) {
	                    event.stopPropagation();
	                }
	            });
	        }
	    };

	    ko.bindingHandlers.fixed = {
	      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var options = ko.unwrap( allBindings().decimalOptions ) || {};
	        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;

	        if ( $( element ).is('input') ) {
	          var hiddenObservable = valueAccessor();

	          if ( ko.isObservable( hiddenObservable ) && hiddenObservable() === '' ) {
	            hiddenObservable( null );
	          }

	          var transform = ko.pureComputed({
	            read: hiddenObservable,
	            write: function( value ) {
	              if ( value === '' ) {
	                hiddenObservable( null );
	              } else {
	                var num = parseFloat( value.replace( /[^\d\.\-]/g, '' ) );
	                hiddenObservable( num.toFixed( precision ) );
	              }
	            }
	          });

	          ko.bindingHandlers.value.init( element, function() { return transform; }, allBindings );
	        }
	      },
	      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var value = ko.unwrap( valueAccessor() );
	        var options = ko.unwrap( allBindings().decimalOptions );
	        var precision = options.precision || ko.bindingHandlers.fixed.defaultPrecision;
	        var formattedValue = parseFloat( value ).toFixed( precision );

	        if ( !isNaN( formattedValue ) ) {
	          $( element ).val( options.commas ? ko.bindingHandlers.fixed.withComma( formattedValue ) : formattedValue );
	        } else {
	          $( element ).val( '' );
	        }
	      },
	      defaultPrecision: 2,
	      withComma: function( value ) {
	        var vals = value.split('.');
	        vals[0] = Number( vals[0] ).toLocaleString();

	        return vals.join('.');
	      }
	    };

	    /**
	      * Bootstrap Modal Binding
	      * Bind to modal wrapper with class `.modal`
	      * Refer to http://getbootstrap.com/javascript/#modals for modal structure
	      *
	      * @param {boolean} valueAccessor - Toggles modal visibility
	      */
	    ko.bindingHandlers.modal = {
	      init: function (element, valueAccessor) {
	        $(body).append($(element));
	        $(element).remove();
	        $(element).modal({
	          show: false
	        });

	        var value = valueAccessor();
	        if (ko.isObservable(value)) {
	          $(element).on('hide.bs.modal', function() {
	            value(false);
	          });
	        }
	      },
	      update: function (element, valueAccessor) {
	        var value = valueAccessor();
	        if (ko.utils.unwrapObservable(value)) {
	          $(element).modal('show');
	        } else {
	          $(element).modal('hide');
	        }
	      }
	    };

	    /**
	      * @param {Object} valueAccessor - Value to monitor for update function
	      */
	    ko.bindingHandlers.floatThead = {
	      init: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        if ( element.nodeName.toLowerCase() !== "table" ) {
	          throw new Error('The floatThead binding must be set on a table element');
	        }

	        var $tableSelector = $( element );
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap( value );

	        $tableSelector.parent().addClass('sticky-head-container');
	        $tableSelector.addClass('sticky-head');

	        if ( ko.isObservable( value ) && (valueUnwrapped == null || (Array.isArray( valueUnwrapped) && value.length === 0)) ) {
	          var valueSubscription = value.subscribe( function( newValue ) {
	            floatThead( $tableSelector );
	            valueSubscription.dispose();
	          });
	        } else {
	          floatThead( $tableSelector );
	        }

	        ko.utils.domNodeDisposal.addDisposeCallback( element, function() {
	          $tableSelector.floatThead('destroy');
	        });

	        function floatThead( $tableSelector ) {
	          $tableSelector.floatThead({
	            scrollContainer: function ( $table ) {
	              return $table.closest('.sticky-head-container');
	            }
	          });
	        }
	      },
	      update: function( element, valueAccessor, allBindings, viewModel, bindingContext ) {
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap(value);

	        $( element ).floatThead('reflow');
	      }
	    };

	    ko.bindingHandlers.loadingMessage = {
	      init: function (element, valueAccessor) {
	        $('body').append($(element));
	        $(element).hide();
	      },
	      update: function(element, valueAccessor) {
	        var value = valueAccessor();
	        var valueUnwrapped = ko.unwrap(value);

	        if (!!valueUnwrapped) {
	          $(element).fadeIn();
	        } else {
	          $(element).fadeOut();
	        }
	      }
	    };

	    /**
	      * @deprecated Use modal instead
	      */
	    ko.bindingHandlers.dialog = {
	        init: function (element, valueAccessor, allBindings, bindingContext) {
	            console.debug('dialog has been deprecated, use modal instead');
	            var defaultConfig = {
	                    modal: true,
	                },
	                $element = $(element),
	                value = valueAccessor(),
	                commands = parseCommands();

	            initDialog();


	            // prevent duplicate binding error?
	            ko.cleanNode($element);
	            $element.removeAttr('data-bind');
	            $element.children(function() {
	                this.removeAttr('data-bind');
	            });

	            var dialogDestroyed = false;
	            if (ko.isObservable(value)) {
	                var valueSubscriber = value.subscribe(function (val) {
	                    if (!dialogDestroyed) $element.dialog(val ? "open" : "close");
	                });

	                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                    if ($element.dialog('option')) $element.dialog("destroy");
	                    dialogDestroyed = true;
	                    valueSubscriber.dispose && valueSubscriber.dispose();
	                });
	            }

	            attachKoCommands();

	            function initDialog() {
	                // Creates empty function for any configured buttons.
	                // This will cause the buttons to be displayed while allowing
	                // execution to be deferred to the supplied command object.

	                var options = ko.utils.extend(allBindings() || {}, defaultConfig);

	                var config = {
	                    modal: options.modal,
	                    height: options.height,
	                    width: options.width,
	                    position: options.position,
	                    buttons: { },
	                    close: options.close || options.cancelCommand,
	                    title: options.title,
	                    autoOpen: ko.utils.peekObservable(value) && true || false,
	                    dialogClass: options.cancelCommand ? 'no-close' : '',
	                };

	                for (var prop in commands) {
	                    if (commands.hasOwnProperty(prop))
	                        config.buttons[prop] = empty;
	                }

	                $element.dialog(config);

	                function empty() { }
	            }

	            function parseCommands() {
	                var exports = {},
	                    bindings = allBindings() || {};

	                parseCommand(bindings['cancelCommand'], 'cancelCommand', 'Cancel');
	                parseCommand(bindings['okCommand'], 'okCommand', 'Ok');

	                var customCommands = getCustomCommands();
	                for (var prop in customCommands) {
	                    if (customCommands.hasOwnProperty(prop))
	                        parseCommand(customCommands[prop], prop, prop);
	                }
	                return exports;

	                function getCustomCommands() {
	                    return allBindings().customCommands || allBindings().customCommand || [];
	                }
	                function parseCommand(cmd, bindingName, mapToButtonName) {
	                    if (!cmd) return;
	                    if (!cmd.execute) {
	                        cmd = ko.command({
	                            execute: cmd
	                        });
	                    }
	                    exports[mapToButtonName || bindingName] = cmd;
	                }


	            }

	            function attachKoCommands() {
	                var buttonFunctions = $element.dialog("option", "buttons");
	                var newButtonsConfig = [];
	                for (var funcName in buttonFunctions) {
	                    for (var cmdName in commands) {
	                        if (cmdName == funcName) {
	                            var buttons = $(".ui-dialog-buttonpane button:contains('" + cmdName + "')");

	                            $.each(buttons, function (index) {
	                                var command = commands[cmdName];

	                                ko.bindingHandlers.command.init(
	                                    buttons[index],
	                                    ko.utils.wrapAccessor(command),
	                                    allBindings,
	                                    null,
	                                    bindingContext);

	                                // remove click functionality from the jQuery UI element
	                                newButtonsConfig.push({
	                                    text: cmdName,
	                                    click: empty,
	                                });
	                            });
	                            break;
	                        }
	                    }
	                }

	                function empty() { }
	            }
	        },
	    };

	    ko.bindingHandlers.cancelKey = {
	        init: function (element, valueAccessor, allBindings, viewModel) {
	            var delegate = ko.utils.unwrapObservable(valueAccessor());

	            if (delegate && typeof delegate !== 'function' && typeof delegate.execute === "function") {
	                delegate = delegate.execute;
	            }
	            if (delegate == undefined) return;

	            var cancelKeyCode = 27;
	            var elementToRegister = element;

	            if (attachToWindow()) {
	                var conditionFn = function () { return $(element).is(':visible'); };
	                elementToRegister = window;
	            }
	            ko.utils.registerEventHandler(elementToRegister, 'keydown', buildEventHandler(conditionFn));

	            function attachToWindow() {
	                var bindings = ko.utils.unwrapObservable(allBindings);
	                return bindings && ko.utils.unwrapObservable(bindings.attachToWindow) === true;
	            }

	            function buildEventHandler(conditionalFn) {
	                conditionalFn = conditionalFn || function () { return true; };
	                return function (event) {
	                    if (event.keyCode == cancelKeyCode && conditionalFn()) {
	                        executeCancel(event);
	                        event.cancelBubble = true;
	                        if (event.stopPropagation) event.stopPropagation();
	                    }
	                };
	            }

	            function executeCancel(event) {
	                event.preventDefault();
	                event.target.blur();
	                delegate.call(viewModel);
	            }
	        }
	    };

	    ko.bindingHandlers.onblur = {
	        init: function (element, valueAccessor) {
	            var fn = valueAccessor();
	            if (fn && fn.execute) fn = commandWrapper.bind(fn);
	            ko.utils.registerEventHandler(element, 'blur', fn);

	            function commandWrapper() {
	                this.execute();
	            }
	        }
	    };

	    ko.bindingHandlers.growToWindowHeight = {
	        init: function (element, valueAccessor) {
	            var $element = $(element);
	            var bindings = $.extend({}, ko.bindingHandlers.growToWindowHeight.DEFAULT_OPTIONS, ko.unwrap(valueAccessor()) || {});

	            var $windowHeight = $(window).height();
	            var viewportHeight = document.body && document.body.clientHeight
	                ? Math.min(document.body.clientHeight, $windowHeight)
	                : $windowHeight;
	            var windowHeight = viewportHeight - bindings.offset;
	            $element.height(windowHeight);
	            $element.css('overflow', 'auto');
	        },
	        DEFAULT_OPTIONS: {
	            offset: 0
	        }
	    };

	    ko.bindingHandlers.maxHeight = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            $element.addClass('fullWindowHeight');
	            constrainHeight.call($element);

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                var sub = value.subscribe(function() {
	                    setupStickyTableElements();
	                    sub.dispose();
	                });
	            } else setupStickyTableElements();

	            function setupStickyTableElements() {
	                prepareTables(element, valueAccessor(), allBindings());
	            }
	        },
	    };

	    function constrainHeight() {
	        var windowHeight = $(window).height();
	        this.height(windowHeight);
	        //this.css('max-height', windowHeight);
	        this.css('overflow-y', 'scroll');
	        this.css('overflow-x', 'scroll');
	    }

	    function prepareTables(element, value, allBindings) {
	        var $element = $(element);
	        var opts = allBindings || {};

	        if ($element.is('table')) {
	            $element = $(element).wrap("<div><div>").parent();
	        }

	        initStickyTableBinding($element, value, opts, 'stickyTableHeaders');
	        //initStickyTableBinding($element, value, opts, 'stickyTableFooters');
	    }

	    function initStickyTableBinding(element, value, opts, bindingName) {
	        var valueAccessor = opts[bindingName]
	                ? ko.utils.wrapAccessor(opts[bindingName])
	                : ko.utils.wrapAccessor(value || true);

	        var $element = $(element);

	        var template = getTemplatedChild();
	        if (template) {
	            if (!opts[bindingName]) return;
	            opts.dependsOn = template;
	            var bindings = ko.utils.wrapAccessor(opts);
	            ko.bindingHandlers[bindingName].init(template, valueAccessor, bindings);
	        } else {
	            opts.parent = $element;
	            $element.find(opts[bindingName] || 'table').each(function () {
	                var $this = $(this);
	                ko.bindingHandlers[bindingName].init($this, valueAccessor, ko.utils.wrapAccessor(opts));
	                removeBinding($this, bindingName);
	            });
	        }

	        function getTemplatedChild() {
	            var child = getChild();
	            if (!child) return null;
	            var childContext = ko.contextFor(child);
	            if (!childContext) return null;

	            var childBindings = ko.bindingProvider.instance.getBindings(child, childContext);
	            return childBindings && childBindings.template ? child : null;

	            function getChild() {
	                return $element.children(':first')[0]
	                    || getVirtualElementChild();
	            }
	            function getVirtualElementChild() {
	                var vChild = ko.virtualElements.firstChild($element[0]);
	                return vChild && ko.virtualElements.nextSibling(vChild);
	            }
	        }

	        function removeBinding(table, binding) {
	            var dataBind = table.attr('data-bind');
	            if (dataBind) {
	                var regex = new RegExp(binding + "\:\s?\w+\W?\s?", "i");
	                dataBind = dataBind.replace(regex, "");
	                table.attr('data-bind', dataBind);
	            }
	        }
	    }

	    ko.bindingHandlers.fixCvpOverlay = {
	        init: function (element, valueAccessor) {
	            var $container = $(element).wrap('<div />').parent();
	            //ko.bindingHandlers.fixCvpOverlay.update(element,valueAccessor);
	        },
	        update: function (element, valueAccessor) {
	            valueAccessor().notifySubscribers(); // fix initial overlay
	            ko.utils.unwrapObservable(valueAccessor());
	            var $cvp = $("#cvp");
	            var cvpWidth = $cvp.outerWidth();
	            var $element = $(element);
	            var inventoryTableWidth = $element.width();

	            // When element contains an enumerated child (such as a foreach binding), the
	            // width function returns 0. This hacky little fix will set a default width.
	            if (inventoryTableWidth == 0) {
	                inventoryTableWidth = 5000; // default width
	            }

	            var $container = $element.parent();
	            $container.width(inventoryTableWidth).css({ "padding-right": cvpWidth + 85 });
	        }
	    };

	    ko.bindingHandlers.undo = {
	        init: function (element, valueAccessor, allBindings, viewModel) {
	            var bindings = {};
	            var trackedBindingNames = ['value'];
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });
	            var elementBindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	            ko.utils.arrayForEach(trackedBindingNames, function (binding) {
	                if (elementBindings[binding]) {
	                    bindings[binding] = elementBindings[binding];
	                }
	            });

	            for (var boundProp in bindings) {
	                initializeTracking(bindings[boundProp]);
	            }

	            function revert(propAccessor) {
	                var initalValue = propAccessor.changeHistory()[0];
	                propAccessor(initalValue);
	            }

	            function initializeTracking(propAccessor) {
	                propAccessor.changeHistory = ko.observableArray([propAccessor()]);

	                propAccessor.subscribe(function (newVal) {
	                    propAccessor.changeHistory.push(newVal);
	                });

	                setupRevertTrigger(propAccessor);
	            }

	            isEditing.subscribe(function (newVal) {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.visible.update(
	                        button,
	                        ko.utils.wrapAccessor(newVal),
	                        allBindings,
	                        data
	                    );
	                });
	            });

	            function setupRevertTrigger(propAccessor) {
	                // eventually, we'll enable actual undo/redo stepping but for now, we just
	                // handle both as a revert function.
	                var revertCommand = ko.command({
	                    execute: function () {
	                        revert(propAccessor);
	                    },
	                    canExecute: function () {
	                        return propAccessor.changeHistory().length > 1;
	                    }
	                });
	                propAccessor.revertCommand = revertCommand;
	                var trigger = allBindings().undoTrigger || allBindings().revertTrigger;

	                $(trigger).each(function (index, button) {
	                    ko.bindingHandlers.command.init(
	                        button,
	                        ko.utils.wrapAccessor(revertCommand),
	                        allBindings,
	                        viewModel
	                    );
	                });
	            }
	        }
	    };

	    ko.bindingHandlers.pageData = {
	        update: function (element, valueAccessor) {
	            ko.utils.unwrapObservable(valueAccessor());
	            $(element).hide().fadeIn(500);
	        }
	    };

	    ko.bindingHandlers.editableContent = {
	        init: function (element, valueAccessor, allBindings, data) {
	            var savedState = ko.observable();
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });
	            var $element = $(element);
	            var $cancelButton = $(allBindings().cancelTrigger);
	            var $masterCancelButton = $(allBindings().masterCancelTrigger);

	            if (!isEditing()) { $element.attr("readonly", "readonly"); }

	            ko.bindingHandlers.undo.init(
	                element,
	                ko.utils.wrapAccessor(function () { return true; }),
	                ko.utils.wrapAccessor({ revertTrigger: $cancelButton ? $cancelButton[0] : undefined }),
	                data);

	            ko.bindingHandlers.click.init(
	                element,
	                ko.utils.wrapAccessor(beginEdit),
	                ko.utils.wrapAccessor({}),
	                data);

	            //todo: handle blur events (and allow disabling the blur handlers)

	            //todo: 1. prevent bubbling, 2. enable canceling when cancel button is not supplied
	            ko.bindingHandlers.cancelKey.init(
	                element,
	                ko.utils.wrapAccessor(function () { $cancelButton.click(); }),
	                ko.utils.wrapAccessor({ keydownBubble: false }),
	                data
	            );


	            setupCancelButtons();

	            function setupCancelButtons() {
	                var cancelCommand = allBindings().cancelEditsCommand;
	                if ($cancelButton.length > 0) {
	                    $cancelButton.each(function (index, button) {
	                        ko.bindingHandlers.command.init(
	                            button,
	                            ko.utils.wrapAccessor(cancelCommand),
	                            function () { return { clickBubble: false }; },
	                            data);
	                    });
	                }

	                $masterCancelButton.each(function (index, button) {
	                    var context = ko.contextFor(button);
	                    if (context) {
	                        var commandBinding = ko.bindingProvider.instance.getBindings(button, context).command;
	                        if (commandBinding) {
	                            if (typeof commandBinding.addCommand != "function") {
	                                throw new Error('The masterCancelCommand is only supported with a composableCommand instance.');
	                            }

	                            commandBinding.addCommand(ko.command({
	                                execute: function () { $cancelButton.click(); }
	                            }));
	                        }
	                    }
	                });
	            }

	            function beginEdit() {
	                if (isEditing()) return;
	                valueAccessor()(true);
	                var bindings = ko.bindingProvider.instance.getBindings(element, ko.contextFor(element));
	                savedState(ko.utils.unwrapObservable(bindings.value));
	            }
	        },
	        update: function (element, valueAccessor) {
	            var isEditing = ko.utils.unwrapObservable(valueAccessor());
	            if (isEditing === false) {
	                $(element).attr("readonly", "readonly");
	            } else {
	                $(element).removeAttr("readonly");
	            }
	        }
	    };

	    ko.bindingHandlers.editableContentArea = {
	        init: function (element, valueAccessor, allBindings, data) {
	            var $element = $(element);
	            var isEditing = ko.computed(function () {
	                return ko.utils.unwrapObservable(valueAccessor());
	            });

	            function setIsEditingValue(val) {
	                valueAccessor()(val);
	            }
	            var inputElements = $('input', $element).not('[type="button"], [type="submit"]');
	            var allChildrenEmpty = ko.computed(function () {
	                var firstDefinedValue = ko.utils.arrayFirst(inputElements, function (e) {
	                    var ctx = ko.contextFor(e);
	                    var binding = ctx ? ko.bindingProvider.instance.getBindings(e, ctx) : undefined;
	                    var value = binding ? binding.value() : undefined;
	                    return value != undefined;
	                });
	                return firstDefinedValue === null;
	            });
	            var isCancelVisible = ko.computed(function () {
	                return isEditing() && !allChildrenEmpty();
	            });

	            valueAccessor().__editableContentArea__inputElements = inputElements;

	            var cancelValueAccessor = ko.utils.wrapAccessor(function () { return false; });
	            cancelValueAccessor().__editableContentArea__inputElements = inputElements;

	            var cancelCommand = ko.command({
	                execute: function () {
	                    setIsEditingValue(false);
	                },
	            });

	            // currently requires cancelTrigger binding to be provided
	            var $cancelButton = $element.find(allBindings().cancelTrigger);

	            // initialize visibility
	            updateCancelButtonVisibility();

	            // update visibility
	            isCancelVisible.subscribe(function () {
	                updateCancelButtonVisibility();
	            });

	            allBindings().cancelTrigger = $cancelButton ? $cancelButton[0] : undefined;
	            allBindings().cancelEditsCommand = cancelCommand;

	            $.each(inputElements, function (index, elem) {
	                ko.bindingHandlers.editableContent.init(elem, valueAccessor, allBindings, data);
	            });

	            function updateCancelButtonVisibility() {
	                $cancelButton.each(function (index, button) {
	                    ko.bindingHandlers.visible.update(
	                        button,
	                        function () { return isCancelVisible(); },
	                        allBindings,
	                        data
	                    );
	                });
	            }
	        },
	        update: function (element, valueAccessor, allBindings, data) {
	            var inputElements = valueAccessor().__editableContentArea__inputElements;
	            var isEditing = ko.utils.unwrapObservable(valueAccessor());

	            $.each(inputElements, function (index, elem) {
	                ko.bindingHandlers.editableContent.update(
	                    elem,
	                    ko.utils.wrapAccessor(isEditing),
	                    allBindings,
	                    data
	                );
	            });
	        },
	    };



	    function initVisibleToggle(element, dataValue, isNot) {
	        var $el = $(element);
	        if (isNot ? !dataValue : (dataValue && true)) {
	            $el.hide();
	            $el.slideDown();
	        } else {
	            $el.slideUp();
	        }

	        $el = null;
	    }

	    function updateVisibleToggle(element, dataValue, opts, isNot) {
	        var defaults = {
	            showDuration: "slow",
	            hideDuration: "slow",
	            speed: false,
	            direction: "down",
	        };

	        var options = $.extend({}, defaults, opts);
	        var $el = $(element);
	        if (options && options.speed) options.showDuration = options.hideDuration = options.speed;

	        if (isNot ? !dataValue : (dataValue && true)) {
	            $el.slideDown(options.showDuration);
	        } else {
	            $el.slideUp(options.hideDuration);
	        }
	    }

	    ko.bindingHandlers.slideVisible = {
	        init: function (element, valueAccessor) {
	            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), false);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), false);
	        }
	    };

	    ko.bindingHandlers.slideCollapsed = {
	        init: function(element, valueAccessor) {
	            initVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), true);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            updateVisibleToggle(element, ko.utils.unwrapObservable(valueAccessor()), allBindings(), true);
	        }
	    }

	    ko.bindingHandlers.popup = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            $element.addClass('popupWindow');

	            var defaults = {
	                attachCancelCommandToWindow: true,
	            };
	            var options = $.extend({}, defaults, allBindings());
	            var borderWidth = parseInt($element.css('border-left-width'), 10) || 10; // parseInt trims the 'px' and returns base-10 value

	            $(element).on('click', onCloseEvent);

	            if (options.closePopupCommand) {
	                var cancelKeyOptions = options;
	                cancelKeyOptions.attachToWindow = options.attachCancelCommandToWindow;
	                ko.bindingHandlers.cancelKey.init(element, ko.utils.wrapAccessor(options.closePopupCommand), ko.utils.wrapAccessor(cancelKeyOptions));
	            }

	            ko.bindingHandlers.slideIn.init(element, valueAccessor);

	            // handle cleanup
	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.off('click', onCloseEvent);
	            });

	            // private functions
	            function onCloseEvent(e) {
	                var hitAreaX = borderWidth + $element.position().left;
	                // Reece - Replaced pageX with screenX b/c when
	                // clicking a selectBox,  pageX value is relative to
	                // selectbox -- not the page.
	                if (e.pageX && e.pageX <= hitAreaX) {
	                    if (options.closePopupCommand && typeof options.closePopupCommand.execute == "function") {
	                        options.closePopupCommand.execute();
	                        return;
	                    }
	                    if (ko.isWriteableObservable(valueAccessor())) {
	                        valueAccessor()(false);
	                        return;
	                    }
	                    ko.bindingHandlers.popup.update(element, ko.utils.wrapAccessor(false), allBindings);
	                }
	            }
	        },
	        update: function (element, valueAccessor, allBindings) {
	            ko.bindingHandlers.slideIn.update(element, valueAccessor, allBindings);
	        }
	    };

	    ko.bindingHandlers.slideIn = {
	        init: function (element, valueAccessor) {
	            var display = ko.utils.unwrapObservable(valueAccessor());

	            var $element = $(element);
	            $element.show();
	            if (!display) {
	                $element.hide();
	                //$element.css({ left: $(window).width() });
	                $element.css({ left: "100%" });
	            }
	        },
	        update: function (element, valueAccessor) {
	            var $element = $(element);
	            var display = ko.utils.unwrapObservable(valueAccessor());
	            if (display) {
	                $element.show();
	                $element.animate({ left: 0 });
	            } else {
	                $element.animate({ left: "100%" });
	                $element.hide();
	            }
	        }
	    };

	    ko.bindingHandlers.fadeVisible = {
	        init: function (element) {
	            $(element).hide();
	        },
	        update: function (element, valueAccessor) {
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            if (value) $(element).fadeIn();
	            else $(element).fadeOut();
	        }
	    };

	    ko.bindingHandlers.stickyTableHeaders = {
	        init: function (element, valueAccessor, allBindings) {
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            var bindings = allBindings();
	            var $table;
	            var options = {};
	            var completed = false;
	            var $element = $(element);

	            options.tabs = bindings.tabbedParent;
	            options.myTab = bindings.myTab;

	            if (typeof value === "string") {
	                $table = $element.find(value);
	                options.parent = $element;
	            } else {
	                $table = $(element);
	                options.parent = bindings.parent;
	            }

	            if ($table == undefined) throw new Error("The table element can not be found. Selector: '" + value + "'.");

	            bindTable();

	            function bindTable() {
	                //Enables the jQuery transformation to be deferred until after the dependent object has data
	                var dependsOn = bindings['dependsOn'];
	                if (dependsOn && deferToDependency()) {
	                    return;
	                }
	                stickyHeaders($table, options);

	                function deferToDependency() {
	                    var $dependency = typeof dependsOn === "string"
	                        ? $element.children(':first')
	                        : $(dependsOn);
	                    if (!$dependency) return false;

	                    var dependencyElement = $dependency[0];
	                    var dependencyContext = ko.contextFor(dependencyElement);
	                    var dependencyBindings = ko.bindingProvider.instance.getBindings(dependencyElement, dependencyContext);

	                    if (dependencyHasTemplate()) {
	                        var fnName = '__stickyTableHeaders__updateHeaders__';
	                        if (isVirtualElement()) {
	                            dependsOn.data = attachAfterRenderBinding.call(dependsOn.data);
	                            dependencyContext.$data[fnName] = function () {
	                                var table = typeof (value) === "string"
	                                    ? $(arguments[0]).filter(value) || $element.find(value)
	                                    : value;

	                                if (!table.length) {
	                                    console.error("The table element could not be found. When attaching stickyTableHeaders within template, the value parameter should contain a selector for the table.");
	                                    return;
	                                }

	                                var context = ko.contextFor(dependsOn);
	                                var theadDependency = bindings.stickyTableHeaderDependency;
	                                if (typeof theadDependency === "string") theadDependency = context.$data[theadDependency];
	                                options.parent = table;
	                                if (ko.isObservable(theadDependency)) {
	                                    theadDependency.subscribe(function () {
	                                        stickyHeaders(table, options);
	                                    });
	                                } else {
	                                    stickyHeaders(table, options);
	                                }
	                            };
	                        } else {
	                            var binding = attachAfterRenderBinding.call($dependency.attr('data-bind'));
	                            $dependency.attr('data-bind', binding);
	                            dependencyContext.$data[fnName] = function () {
	                                stickyHeaders($element.find(value), options);
	                            };
	                        }
	                        return true;

	                    }

	                    return false;

	                    function isVirtualElement() {
	                        return dependencyElement.nodeType === 8;
	                            //virtualNoteBindingValue is apparently only available to the debug version of KO.
	                            //&& ko.virtualElements.virtualNodeBindingValue
	                            //&& ko.virtualElements.virtualNodeBindingValue(dependsOn);
	                    }
	                    function dependencyHasTemplate() {
	                        return dependencyBindings && dependencyBindings.template;
	                    }
	                    function attachAfterRenderBinding() {
	                        return this.replace(/(template\:\s?\{)/, "$1" + 'afterRender:' + fnName + ',');
	                    }
	                }
	            }

	            function stickyHeaders(table, opts) {
	                table.each(function () {
	                    if (!this.tagName || this.tagName.toLowerCase() !== 'table') {
	                        throw new Error("The bound element is not a table element. Element selector: '" + value + "'");
	                    }
	                });

	                opts.floatingElementId = 'stickyTableHeader';
	                opts.target = 'thead:first';

	                table.stickyTableHeaders(opts);

	                table.each(function () { rebind.call(this, opts); });

	                var valueSubscription;
	                if (ko.isObservable(valueAccessor())) {
	                    valueSubscription = valueAccessor().subscribe(function () {
	                        table.stickyTableHeaders('option', 'format');
	                    });
	                }

	                var rebuildSubscription;
	                if (bindings.rebuildTrigger) {
	                    if (!ko.isObservable(bindings.rebuildTrigger))
	                        throw new Error("Invalid binding: \"rebuildTrigger\". Must be observable object.");

	                    rebuildSubscription = bindings.rebuildTrigger.subscribe(function () {
	                        table.stickyTableHeaders("option", "refresh");
	                        table.each(function () { rebind.call(this, opts); });
	                    });
	                }

	                ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                    table.stickyTableHeaders('destroy');
	                    valueSubscription && valueSubscription.dispose();
	                    rebuildSubscription && rebuildSubscription.dispose();
	                });

	                completed = true;
	            }

	            function rebind(opts) {
	                var floatingElements = $('.' + opts.floatingElementId, this);
	                var floatingClone = floatingElements[0];

	                var context = ko.contextFor(this);
	                if (!context || !floatingClone) return;

	                ko.cleanNode(floatingClone);
	                ko.applyBindings(context.$data, floatingClone);

	                var $clone = $(floatingClone);
	                var bindings = ko.bindingProvider.instance.getBindings(this, ko.contextFor(this));

	                // reformat elements if the clone was templated
	                if (bindings.template) {
	                    $element.stickyTableHeaders('option', 'format');
	                }

	                if (bindings.sortableTable) {
	                    ko.bindingHandlers.sortableTable.init($clone.parent()[0], ko.utils.wrapAccessor(bindings.sortableTable), ko.utils.wrapAccessor(bindings));
	                }

	            }
	        },
	    };

	    var templateRegEx = /(?:^|,|\s)template\s*:\s*(?:(?:(?:'|\")([^(?:'|"|\s|\{)]+)\s*(?:'|"))|(?:\{.*name\s*:\s*(?:(?:'|\")([^(?:'|"|\s|\{)]+)(?:'|"|\s))))/i;
	    ko.bindingHandlers.stickyTableFooters = {
	        init: function (element, valueAccessor, allBindingsAccessor) {
	            var opts = ko.utils.unwrapObservable(allBindingsAccessor());
	            opts.floatingElementId = 'stickyTableFooter';
	            opts.target = 'tfoot:first';

	            var $element = $(element);

	            $element.stickyTableFooters(opts);

	            var table = element;

	            $(opts.target, table).each(function () {
	                var floatingElements = $('.' + opts.floatingElementId, table);
	                if (!floatingElements.length) return;
	                var floatingClone = floatingElements[0];


	                var context = ko.contextFor(this);
	                if (!context) return;

	                ko.cleanNode(floatingClone);
	                ko.applyBindings(context.$data, floatingClone);

	                // reformat elements if the clone was templated
	                var $clone = $(floatingClone);
	                var dataBind = $clone.attr('data-bind');
	                if (dataBind) {
	                    var matches = dataBind.match(templateRegEx);
	                    if (matches && matches.length) {
	                        $element.stickyTableFooters('option', 'format');
	                    }
	                }
	            });

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                value.subscribe(function() {
	                    $element.stickyTableFooters('option', 'format');
	                });
	            }

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.stickyTableFooters('destroy');
	            });
	        },
	    };

	    ko.bindingHandlers.tooltip = {
	        init: function (element, bindingAccessor, allBindings) {
	            var value = ko.utils.unwrapObservable(bindingAccessor()),
	                bindings = allBindings && allBindings() || {};

	            if (typeof value == "number") value = value.toString();
	            if (!value || value.length == 0) return;

	            var $element = $(element);
	            $element.attr('title', value);
	            $element.tooltip({
	                track: bindings.tooltipTrack,
	            });
	            //todo: enable updates to the tooltip value
	        },
	    };

	    ko.bindingHandlers.datePickerSm = {
	        init: function (element, valueAccessor, allBindings) {
	            $(element).wrap('<div class="input-group input-group-sm"></div>');
	            $(element).datepicker({
	                showOn: 'button',
	                buttonText: '<i class="fa fa-calendar"></i>',
	                changeMonth: true,
	                changeYear: true
	            }).next(".ui-datepicker-trigger")
	                .addClass("btn btn-default")
	                .wrap('<span class="input-group-btn"></span>');

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                //todo: cleanup wrapper element
	                $(element).datepicker('destroy');
	            });

	            var value = valueAccessor();
	            if (ko.isObservable(value)) {
	                ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
	            }
	        }
	    };

	    ko.bindingHandlers.autoHeightTextarea = {
	        init: function (element, valueAccessor) {
	        },
	        update: function (element, valueAccessor) {
	            element.style.height = '0';
	            element.style.height = element.scrollHeight + 'px';
	        }
	    };

	    // autocomplete: listOfCompletions
	    ko.bindingHandlers.autocomplete = {
	        init: function (element, valueAccessor) {
	            var disposables = [];
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            var opts = {
	                //minLength: 0,
	                change: onChange
	            };
	            $( element ).wrap('<div class="ui-front"></div>');

	            function buildSourceOptions( value ) {
	              if ( value.length ) {
	                opts.source = ko.utils.arrayMap( value, function(c) {
	                  if ( c.Name && !c.label ) {
	                    c.label = c.Name;
	                  }

	                  return c;
	                });
	              } else {
	                opts = $.extend( opts, value );

	                if ( !value.source ) {
	                  console.log("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
	                  return;

	                  //the following line was causing an error when closing a pack schedule after it's been in edit mode.
	                  throw new Error("Invalid parameters for the autocomplete binding. Value must be either an array or and object with a \"source\" property containing an array.");
	                }

	                if ( value.label || value.value ) {
	                  var labelProjector = buildProjector( value.label ),
	                  valueProjector = value.value ? buildProjector( value.value ) : function() { return value; };

	                  opts.source = ko.utils.arrayMap( ko.utils.unwrapObservable( value.source ), function ( item ) {
	                    return {
	                      label: labelProjector(item),
	                      value: valueProjector(item),
	                    };
	                  });
	                } else {
	                  opts.source = ko.utils.unwrapObservable(value.source);
	                }
	              }
	            }

	            function buildProjector( src ) {
	              var prop = ko.utils.unwrapObservable(src);

	              if (prop == undefined) {
	                throw new Error("Projector property is undefined.");
	              }

	              return typeof prop === "function" ?
	                function (object) { return prop(object); } :
	                function(object) { return object[prop]; };
	            }

	            if ( ko.isObservable( value ) ) {
	              disposables.push( value.subscribe(function( optionsSource ) {
	                buildSourceOptions( optionsSource );
	                $( element ).autocomplete( opts );
	              }));
	            } else if ( ko.isObservable( value.source ) ) {
	              disposables.push( value.source.subscribe(function( optionsSource ) {
	                buildSourceOptions( optionsSource );
	                $( element ).autocomplete( opts );
	              }));
	            }

	            buildSourceOptions( value );
	            $( element ).autocomplete( opts );

	            function onChange (e, ui) {
	                var bindingContext = ko.contextFor(element);
	                if (!bindingContext) return;
	                var bindings = ko.bindingProvider.instance.getBindings(element, bindingContext) || {};
	                if (!bindings.value) return;

	                if (ui.item && ui.item.value) {
	                    bindings.value(ui.item.value);
	                }
	                    // enable new elements to be added to the list
	                else if (opts.allowNewValues) bindings.value($(this).val());
	                else {
	                    bindings.value(null);
	                    if (ko.DEBUG) {
	                        console.log('The selected value was not found in the options list. To allow new values, include the \"allowNewValues=\'true\'\" value in the \"autocompleteOptions\" binding attribute.');
	                    }
	                }
	            }

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	              $( element ).autocomplete( 'destroy' );
	              ko.utils.arrayForEach( disposables, function( disposable ) {
	                disposable.dispose();
	              });
	            });
	        },
	    };

	    ko.bindingHandlers.tabs = {
	        init: function (element, valueAccessor, allBindings) {
	            var $element = $(element);
	            var value = ko.utils.unwrapObservable(valueAccessor());
	            $(element).val(value);

	            var options = ko.utils.unwrapObservable(allBindings().tabOptions) || {};

	            $element.on("tabsactivate", onTabActivate);
	            $element.on("tabscreate", onTabCreate);

	            $element.tabs(options);

	            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
	                $element.tabs("destroy");
	                $element.off('tabsactivate', onTabActivate);
	                $element.off('tabscreate', onTabCreate);
	            });


	            function onTabActivate(event, ui) {
	                bindValueFromUI(ui.newTab.text(), ui.newPanel[0]);
	            }

	            function onTabCreate(event, ui) {
	                bindValueFromUI(ui.tab.text(), ui.panel[0]);
	            }

	            function bindValueFromUI(name, panel) {
	                if (!ko.isObservable(valueAccessor())) return;
	                valueAccessor()({
	                    name: name,
	                    data: getDataBoundObjectFor(panel)
	                });
	            }

	            function getDataBoundObjectFor(tabPanel) {
	                if (!tabPanel) return undefined;
	                var panelContext = ko.contextFor(tabPanel);
	                if (!panelContext) return undefined;
	                var bindings = ko.bindingProvider.instance.getBindings(tabPanel, panelContext) || {};
	                return bindings.with || panelContext.$data;
	            }
	        },
	    };

	    ko.bindingHandlers.ajaxStatus = {
	        init: function (element, valueAccessor) {
	            var value = valueAccessor();
	            if (value.ajaxSuccess == undefined
	                || value.ajaxFailure == undefined
	                || value.ajaxWorking == undefined) throw new Error("The bound value is not valid for use with the ajaxStatus binding.");

	            ko.applyBindingsToNode(element, {
	                css: {
	                    working: value.ajaxWorking,
	                    success: value.ajaxSuccess,
	                    fail: value.ajaxFailure,
	                    ajaxStatus: true,
	                }
	            });
	        }
	    };


	    // Dragons be here...
	    // allows up/down arrows, mouse-click dragging,
	    // and mouse-click wheel
	    // accepts property 'negative'in allBindings to allow negative numbers
	    ko.bindingHandlers.numValue = {
	        init: function (element, valueAccessor, allBindings) {
	            console.warn("numValue binding handler is being used! This should be replaced with the numericObservable.");
	            var num = valueAccessor();
	            var bindings = ko.utils.unwrapObservable(allBindings());
	            var isChar = function (key) { return key >= 65 && key <= 90; };
	            var up = 38, down = 40;
	            $(element).keydown(function (evt) {
	                var key = evt.keyCode;
	                var iVal = parseInt(element.value);
	                if (key === up || key === down) {
	                    if (key === up) iVal++;
	                    else if (key == down && (bindings.negative || iVal > 0)) iVal--;
	                }
	                else if (isChar(key) && !evt.ctrlKey) evt.preventDefault();
	                if (!isNaN(iVal) && iVal != null) num(iVal);
	                else num(null);

	                return true;
	            });
	            var isDown = false;
	            var lastY = 0;
	            var buffer = 10;
	            $(element).mousedown(function (e) { isDown = true; return true; });
	            $(document).mouseup(function (e) { isDown = false; return true; });
	            $(document).mousemove(function (e) {
	                if (isDown) {
	                    var y = e.pageY;
	                    if (!lastY) lastY = y;
	                    if (y > lastY + buffer && (bindings.negative || num() > 0)) {
	                        num(num() - 1);
	                        lastY = y;
	                    }
	                    else if (y + buffer < lastY) {
	                        num(num() + 1);
	                        lastY = y;
	                    }
	                }
	            });
	            $(document).on("mousewheel", function (evt) {
	                if (isDown) {
	                    var delta = evt.originalEvent.wheelDelta;
	                    if (delta > 0) {
	                        num(num() + 1);
	                    }
	                    else if (delta < 0 && (bindings.negative || num() > 0)) {
	                        num(num() - 1);
	                    }
	                }
	            });

	            // show validations as well
	            return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindings);
	        },
	        update: function (element, valueAccessor, allBindings) {
	            var val = ko.utils.unwrapObservable(valueAccessor());
	            if (!isNaN(val)) element.value = val;
	        }
	    };

	    ko.bindingHandlers.resizable = {
	        init: function (element, valueAccessor) {
	            var alsoResizeSelector = ko.unwrap(valueAccessor());
	            if (typeof alsoResizeSelector != "string") alsoResizeSelector = '';
	            $(element).resizable({
	                alsoResize: alsoResizeSelector,
	                minWidth: 300,
	                minHeight: 100
	            });
	        }
	    };

	    ko.bindingHandlers.accordion = {
	        init: function (element, valueAccessor) {
	        },
	        update: function (element, valueAccessor) {
	            var opts = ko.utils.unwrapObservable(valueAccessor());
	            $(element).accordion(opts);
	        }
	    };

	    ko.bindingHandlers.slimscroll = {
	        init: function (element) {
	            var $el = $(element);
	            $el.slimscroll({
	                //alwaysVisible: true,
	                railColor: '#222',
	                height: "100%"
	                //railVisible: true
	            });
	        }
	    };

	    // Focuses next .form-control when Enter is pressed
	    ko.bindingHandlers.tabOnEnter = {
	        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	            var allBindings = allBindingsAccessor();
	            $(element).keypress(function (event) {
	                var keyCode = (event.which ? event.which : event.keyCode);
	                if (keyCode === 13) {
	                    var index = $('.form-control').index(event.target) + 1;
	                    var $next = $('.form-control').eq(index);

	                    $next.focus();
	                    $next.select();
	                    return false;
	                }
	                return true;
	            });
	        }
	    };

	    /** Trigger valueAccessor on Enter keypress
	      * @param {function} valueAccessor - Function to call
	      */
	    ko.bindingHandlers.onEnter = {
	      init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
	        var allBindings = allBindingsAccessor();
	        var value = valueAccessor();
	        $(element).keypress(function (event) {
	          var keyCode = (event.which ? event.which : event.keyCode);
	          if (keyCode === 13) {
	            value.call(viewModel);
	            return false;
	          }
	          return true;
	        });
	      }
	    };
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 33 */
/***/ (function(module, exports, __webpack_require__) {

	/* WEBPACK VAR INJECTION */(function(jQuery) {// @preserve jQuery.floatThead 1.4.5 - http://mkoryak.github.io/floatThead/ - Copyright (c) 2012 - 2016 Misha Koryak
	// @license MIT

	/* @author Misha Koryak
	 * @projectDescription lock a table header in place while scrolling - without breaking styles or events bound to the header
	 *
	 * Dependencies:
	 * jquery 1.9.0 + [required] OR jquery 1.7.0 + jquery UI core
	 *
	 * http://mkoryak.github.io/floatThead/
	 *
	 * Tested on FF13+, Chrome 21+, IE8, IE9, IE10, IE11
	 *
	 */
	(function( $ ) {
	  /**
	   * provides a default config object. You can modify this after including this script if you want to change the init defaults
	   * @type {Object}
	   */
	  $.floatThead = $.floatThead || {};
	  $.floatThead.defaults = {
	    headerCellSelector: 'tr:visible:first>*:visible', //thead cells are this.
	    zIndex: 1001, //zindex of the floating thead (actually a container div)
	    position: 'auto', // 'fixed', 'absolute', 'auto'. auto picks the best for your table scrolling type.
	    top: 0, //String or function($table) - offset from top of window where the header should not pass above
	    bottom: 0, //String or function($table) - offset from the bottom of the table where the header should stop scrolling
	    scrollContainer: function($table) { // or boolean 'true' (use offsetParent) | function -> if the table has horizontal scroll bars then this is the container that has overflow:auto and causes those scroll bars
	      return $([]);
	    },
	    responsiveContainer: function($table) { // only valid if scrollContainer is not used (ie window scrolling). this is the container which will control y scrolling at some mobile breakpoints
	      return $([]);
	    },
	    getSizingRow: function($table, $cols, $fthCells){ // this is only called when using IE,
	      // override it if the first row of the table is going to contain colgroups (any cell spans greater than one col)
	      // it should return a jquery object containing a wrapped set of table cells comprising a row that contains no col spans and is visible
	      return $table.find('tbody tr:visible:first>*:visible');
	    },
	    floatTableClass: 'floatThead-table',
	    floatWrapperClass: 'floatThead-wrapper',
	    floatContainerClass: 'floatThead-container',
	    copyTableClass: true, //copy 'class' attribute from table into the floated table so that the styles match.
	    autoReflow: false, //(undocumented) - use MutationObserver api to reflow automatically when internal table DOM changes
	    debug: false, //print possible issues (that don't prevent script loading) to console, if console exists.
	    support: { //should we bind events that expect these frameworks to be present and/or check for them?
	      bootstrap: true,
	      datatables: true,
	      jqueryUI: true,
	      perfectScrollbar: true
	    }
	  };

	  var util = window._;

	  var canObserveMutations = typeof MutationObserver !== 'undefined';


	  //browser stuff
	  var ieVersion = function(){for(var a=3,b=document.createElement("b"),c=b.all||[];a = 1+a,b.innerHTML="<!--[if gt IE "+ a +"]><i><![endif]-->",c[0];);return 4<a?a:document.documentMode}();
	  var isFF = /Gecko\//.test(navigator.userAgent);
	  var isWebkit = /WebKit\//.test(navigator.userAgent);

	  if(!(ieVersion || isFF || isWebkit)){
	    ieVersion = 11; //yey a hack!
	  }

	  //safari 7 (and perhaps others) reports table width to be parent container's width if max-width is set on table. see: https://github.com/mkoryak/floatThead/issues/108
	  var isTableWidthBug = function(){
	    if(isWebkit) {
	      var $test = $('<div>').css('width', 0).append(
	        $('<table>').css('max-width', '100%').append(
	          $('<tr>').append(
	            $('<th>').append(
	              $('<div>').css('min-width', 100).text('X')
	            )
	          )
	        )
	      );
	      $("body").append($test);
	      var ret = ($test.find("table").width() == 0);
	      $test.remove();
	      return ret;
	    }
	    return false;
	  };

	  var createElements = !isFF && !ieVersion; //FF can read width from <col> elements, but webkit cannot

	  var $window = $(window);

	  if(!window.matchMedia) {
	    var _beforePrint = window.onbeforeprint;
	    var _afterPrint = window.onafterprint;
	    window.onbeforeprint = function () {
	      _beforePrint && _beforePrint();
	      $window.triggerHandler("beforeprint");
	    };
	    window.onafterprint = function () {
	      _afterPrint && _afterPrint();
	      $window.triggerHandler("afterprint");
	    };
	  }

	  /**
	   * @param debounceMs
	   * @param cb
	   */
	  function windowResize(eventName, cb){
	    if(ieVersion == 8){ //ie8 is crap: https://github.com/mkoryak/floatThead/issues/65
	      var winWidth = $window.width();
	      var debouncedCb = util.debounce(function(){
	        var winWidthNew = $window.width();
	        if(winWidth != winWidthNew){
	          winWidth = winWidthNew;
	          cb();
	        }
	      }, 1);
	      $window.on(eventName, debouncedCb);
	    } else {
	      $window.on(eventName, util.debounce(cb, 1));
	    }
	  }

	  function getClosestScrollContainer($elem) {
	    var elem = $elem[0];
	    var parent = elem.parentElement;

	    do {
	      var pos = window
	          .getComputedStyle(parent)
	          .getPropertyValue('overflow');

	      if (pos != 'visible') break;

	    } while (parent = parent.parentElement);

	    if(parent == document.body){
	      return $([]);
	    }
	    return $(parent);
	  }


	  function debug(str){
	    window && window.console && window.console.error && window.console.error("jQuery.floatThead: " + str);
	  }

	  //returns fractional pixel widths
	  function getOffsetWidth(el) {
	    var rect = el.getBoundingClientRect();
	    return rect.width || rect.right - rect.left;
	  }

	  /**
	   * try to calculate the scrollbar width for your browser/os
	   * @return {Number}
	   */
	  function scrollbarWidth() {
	    var $div = $('<div>').css({ //borrowed from anti-scroll
	      'width': 50,
	      'height': 50,
	      'overflow-y': 'scroll',
	      'position': 'absolute',
	      'top': -200,
	      'left': -200
	    }).append(
	      $('<div>').css({
	        'height': 100,
	        'width': '100%'
	      })
	    );
	    $('body').append($div);
	    var w1 = $div.innerWidth();
	    var w2 = $('div', $div).innerWidth();
	    $div.remove();
	    return w1 - w2;
	  }
	  /**
	   * Check if a given table has been datatableized (http://datatables.net)
	   * @param $table
	   * @return {Boolean}
	   */
	  function isDatatable($table){
	    if($table.dataTableSettings){
	      for(var i = 0; i < $table.dataTableSettings.length; i++){
	        var table = $table.dataTableSettings[i].nTable;
	        if($table[0] == table){
	          return true;
	        }
	      }
	    }
	    return false;
	  }

	  function tableWidth($table, $fthCells, isOuter){
	    // see: https://github.com/mkoryak/floatThead/issues/108
	    var fn = isOuter ? "outerWidth": "width";
	    if(isTableWidthBug && $table.css("max-width")){
	      var w = 0;
	      if(isOuter) {
	        w += parseInt($table.css("borderLeft"), 10);
	        w += parseInt($table.css("borderRight"), 10);
	      }
	      for(var i=0; i < $fthCells.length; i++){
	        w += $fthCells.get(i).offsetWidth;
	      }
	      return w;
	    } else {
	      return $table[fn]();
	    }
	  }
	  $.fn.floatThead = function(map){
	    map = map || {};
	    if(!util){ //may have been included after the script? lets try to grab it again.
	      util = window._ || $.floatThead._;
	      if(!util){
	        throw new Error("jquery.floatThead-slim.js requires underscore. You should use the non-lite version since you do not have underscore.");
	      }
	    }

	    if(ieVersion < 8){
	      return this; //no more crappy browser support.
	    }

	    var mObs = null; //mutation observer lives in here if we can use it / make it

	    if(util.isFunction(isTableWidthBug)) {
	      isTableWidthBug = isTableWidthBug();
	    }

	    if(util.isString(map)){
	      var command = map;
	      var args = Array.prototype.slice.call(arguments, 1);
	      var ret = this;
	      this.filter('table').each(function(){
	        var $this = $(this);
	        var opts = $this.data('floatThead-lazy');
	        if(opts){
	          $this.floatThead(opts);
	        }
	        var obj = $this.data('floatThead-attached');
	        if(obj && util.isFunction(obj[command])){
	          var r = obj[command].apply(this, args);
	          if(r !== undefined){
	            ret = r;
	          }
	        }
	      });
	      return ret;
	    }
	    var opts = $.extend({}, $.floatThead.defaults || {}, map);

	    $.each(map, function(key, val){
	      if((!(key in $.floatThead.defaults)) && opts.debug){
	        debug("Used ["+key+"] key to init plugin, but that param is not an option for the plugin. Valid options are: "+ (util.keys($.floatThead.defaults)).join(', '));
	      }
	    });
	    if(opts.debug){
	      var v = $.fn.jquery.split(".");
	      if(parseInt(v[0], 10) == 1 && parseInt(v[1], 10) <= 7){
	        debug("jQuery version "+$.fn.jquery+" detected! This plugin supports 1.8 or better, or 1.7.x with jQuery UI 1.8.24 -> http://jqueryui.com/resources/download/jquery-ui-1.8.24.zip")
	      }
	    }

	    this.filter(':not(.'+opts.floatTableClass+')').each(function(){
	      var floatTheadId = util.uniqueId();
	      var $table = $(this);
	      if($table.data('floatThead-attached')){
	        return true; //continue the each loop
	      }
	      if(!$table.is('table')){
	        throw new Error('jQuery.floatThead must be run on a table element. ex: $("table").floatThead();');
	      }
	      canObserveMutations = opts.autoReflow && canObserveMutations; //option defaults to false!
	      var $header = $table.children('thead:first');
	      var $tbody = $table.children('tbody:first');
	      if($header.length == 0 || $tbody.length == 0){
	        $table.data('floatThead-lazy', opts);
	        $table.unbind("reflow").one('reflow', function(){
	          $table.floatThead(opts);
	        });
	        return;
	      }
	      if($table.data('floatThead-lazy')){
	        $table.unbind("reflow");
	      }
	      $table.data('floatThead-lazy', false);

	      var headerFloated = true;
	      var scrollingTop, scrollingBottom;
	      var scrollbarOffset = {vertical: 0, horizontal: 0};
	      var scWidth = scrollbarWidth();
	      var lastColumnCount = 0; //used by columnNum()

	      if(opts.scrollContainer === true){
	        opts.scrollContainer = getClosestScrollContainer;
	      }

	      var $scrollContainer = opts.scrollContainer($table) || $([]); //guard against returned nulls
	      var locked = $scrollContainer.length > 0;
	      var $responsiveContainer = locked ? $([]) : opts.responsiveContainer($table) || $([]);
	      var responsive = isResponsiveContainerActive();

	      var useAbsolutePositioning = null;
	      if(typeof opts.useAbsolutePositioning !== 'undefined'){
	        opts.position = 'auto';
	        if(opts.useAbsolutePositioning){
	          opts.position = opts.useAbsolutePositioning ? 'absolute' : 'fixed';
	        }
	        debug("option 'useAbsolutePositioning' has been removed in v1.3.0, use `position:'"+opts.position+"'` instead. See docs for more info: http://mkoryak.github.io/floatThead/#options")
	      }
	      if(typeof opts.scrollingTop !== 'undefined'){
	        opts.top = opts.scrollingTop;
	        debug("option 'scrollingTop' has been renamed to 'top' in v1.3.0. See docs for more info: http://mkoryak.github.io/floatThead/#options");
	      }
	      if(typeof opts.scrollingBottom !== 'undefined'){
	        opts.bottom = opts.scrollingBottom;
	        debug("option 'scrollingBottom' has been renamed to 'bottom' in v1.3.0. See docs for more info: http://mkoryak.github.io/floatThead/#options");
	      }


	      if (opts.position == 'auto') {
	        useAbsolutePositioning = null;
	      } else if (opts.position == 'fixed') {
	        useAbsolutePositioning = false;
	      } else if (opts.position == 'absolute'){
	        useAbsolutePositioning = true;
	      } else if (opts.debug) {
	        debug('Invalid value given to "position" option, valid is "fixed", "absolute" and "auto". You passed: ', opts.position);
	      }

	      if(useAbsolutePositioning == null){ //defaults: locked=true, !locked=false
	        useAbsolutePositioning = locked;
	      }
	      var $caption = $table.find("caption");
	      var haveCaption = $caption.length == 1;
	      if(haveCaption){
	        var captionAlignTop = ($caption.css("caption-side") || $caption.attr("align") || "top") === "top";
	      }

	      var $fthGrp = $('<fthfoot>').css({
	        'display': 'table-footer-group',
	        'border-spacing': 0,
	        'height': 0,
	        'border-collapse': 'collapse',
	        'visibility': 'hidden'
	      });

	      var wrappedContainer = false; //used with absolute positioning enabled. did we need to wrap the scrollContainer/table with a relative div?
	      var $wrapper = $([]); //used when absolute positioning enabled - wraps the table and the float container
	      var absoluteToFixedOnScroll = ieVersion <= 9 && !locked && useAbsolutePositioning; //on IE using absolute positioning doesn't look good with window scrolling, so we change position to fixed on scroll, and then change it back to absolute when done.
	      var $floatTable = $("<table/>");
	      var $floatColGroup = $("<colgroup/>");
	      var $tableColGroup = $table.children('colgroup:first');
	      var existingColGroup = true;
	      if($tableColGroup.length == 0){
	        $tableColGroup = $("<colgroup/>");
	        existingColGroup = false;
	      }
	      var $fthRow = $('<fthtr>').css({ //created unstyled elements (used for sizing the table because chrome can't read <col> width)
	        'display': 'table-row',
	        'border-spacing': 0,
	        'height': 0,
	        'border-collapse': 'collapse'
	      });
	      var $floatContainer = $('<div>').css('overflow', 'hidden').attr('aria-hidden', 'true');
	      var floatTableHidden = false; //this happens when the table is hidden and we do magic when making it visible
	      var $newHeader = $("<thead/>");
	      var $sizerRow = $('<tr class="size-row" aria-hidden="true"/>');
	      var $sizerCells = $([]);
	      var $tableCells = $([]); //used for sizing - either $sizerCells or $tableColGroup cols. $tableColGroup cols are only created in chrome for borderCollapse:collapse because of a chrome bug.
	      var $headerCells = $([]);
	      var $fthCells = $([]); //created elements

	      $newHeader.append($sizerRow);
	      $table.prepend($tableColGroup);
	      if(createElements){
	        $fthGrp.append($fthRow);
	        $table.append($fthGrp);
	      }

	      $floatTable.append($floatColGroup);
	      $floatContainer.append($floatTable);
	      if(opts.copyTableClass){
	        $floatTable.attr('class', $table.attr('class'));
	      }
	      $floatTable.attr({ //copy over some deprecated table attributes that people still like to use. Good thing people don't use colgroups...
	                         'cellpadding': $table.attr('cellpadding'),
	                         'cellspacing': $table.attr('cellspacing'),
	                         'border': $table.attr('border')
	                       });
	      var tableDisplayCss = $table.css('display');
	      $floatTable.css({
	                        'borderCollapse': $table.css('borderCollapse'),
	                        'border': $table.css('border'),
	                        'display': tableDisplayCss
	                      });
	      if(!locked){
	        $floatTable.css('width', 'auto');
	      }
	      if(tableDisplayCss == 'none'){
	        floatTableHidden = true;
	      }

	      $floatTable.addClass(opts.floatTableClass).css({'margin': 0, 'border-bottom-width': 0}); //must have no margins or you won't be able to click on things under floating table

	      if(useAbsolutePositioning){
	        var makeRelative = function($container, alwaysWrap){
	          var positionCss = $container.css('position');
	          var relativeToScrollContainer = (positionCss == "relative" || positionCss == "absolute");
	          var $containerWrap = $container;
	          if(!relativeToScrollContainer || alwaysWrap){
	            var css = {"paddingLeft": $container.css('paddingLeft'), "paddingRight": $container.css('paddingRight')};
	            $floatContainer.css(css);
	            $containerWrap = $container.data('floatThead-containerWrap') || $container.wrap(
	              $('<div>').addClass(opts.floatWrapperClass).css({
	                'position': 'relative',
	                'clear': 'both'
	              })
	            ).parent();
	            $container.data('floatThead-containerWrap', $containerWrap); //multiple tables inside one scrolling container - #242
	            wrappedContainer = true;
	          }
	          return $containerWrap;
	        };
	        if(locked){
	          $wrapper = makeRelative($scrollContainer, true);
	          $wrapper.prepend($floatContainer);
	        } else {
	          $wrapper = makeRelative($table);
	          $table.before($floatContainer);
	        }
	      } else {
	        $table.before($floatContainer);
	      }


	      $floatContainer.css({
	                            position: useAbsolutePositioning ? 'absolute' : 'fixed',
	                            marginTop: 0,
	                            top:  useAbsolutePositioning ? 0 : 'auto',
	                            zIndex: opts.zIndex,
	                            willChange: 'transform'
	                          });
	      $floatContainer.addClass(opts.floatContainerClass);
	      updateScrollingOffsets();

	      var layoutFixed = {'table-layout': 'fixed'};
	      var layoutAuto = {'table-layout': $table.css('tableLayout') || 'auto'};
	      var originalTableWidth = $table[0].style.width || ""; //setting this to auto is bad: #70
	      var originalTableMinWidth = $table.css('minWidth') || "";

	      function eventName(name){
	        return name+'.fth-'+floatTheadId+'.floatTHead'
	      }

	      function setHeaderHeight(){
	        var headerHeight = 0;
	        $header.children("tr:visible").each(function(){
	          headerHeight += $(this).outerHeight(true);
	        });
	        if($table.css('border-collapse') == 'collapse') {
	          var tableBorderTopHeight = parseInt($table.css('border-top-width'), 10);
	          var cellBorderTopHeight = parseInt($table.find("thead tr:first").find(">*:first").css('border-top-width'), 10);
	          if(tableBorderTopHeight > cellBorderTopHeight) {
	            headerHeight -= (tableBorderTopHeight / 2); //id love to see some docs where this magic recipe is found..
	          }
	        }
	        $sizerRow.outerHeight(headerHeight);
	        $sizerCells.outerHeight(headerHeight);
	      }


	      function setFloatWidth(){
	        var tw = tableWidth($table, $fthCells, true);
	        var $container = responsive ? $responsiveContainer : $scrollContainer;
	        var width = $container.width() || tw;
	        var floatContainerWidth = $container.css("overflow-y") != 'hidden' ? width - scrollbarOffset.vertical : width;
	        $floatContainer.width(floatContainerWidth);
	        if(locked){
	          var percent = 100 * tw / (floatContainerWidth);
	          $floatTable.css('width', percent+'%');
	        } else {
	          $floatTable.outerWidth(tw);
	        }
	      }

	      function updateScrollingOffsets(){
	        scrollingTop = (util.isFunction(opts.top) ? opts.top($table) : opts.top) || 0;
	        scrollingBottom = (util.isFunction(opts.bottom) ? opts.bottom($table) : opts.bottom) || 0;
	      }

	      /**
	       * get the number of columns and also rebuild resizer rows if the count is different than the last count
	       */
	      function columnNum(){
	        var count;
	        var $headerColumns = $header.find(opts.headerCellSelector);
	        if(existingColGroup){
	          count = $tableColGroup.find('col').length;
	        } else {
	          count = 0;
	          $headerColumns.each(function () {
	            count += parseInt(($(this).attr('colspan') || 1), 10);
	          });
	        }
	        if(count != lastColumnCount){
	          lastColumnCount = count;
	          var cells = [], cols = [], psuedo = [], content;
	          for(var x = 0; x < count; x++){
	            content = $headerColumns.eq(x).text();
	            cells.push('<th class="floatThead-col" aria-label="'+content+'"/>');
	            cols.push('<col/>');
	            psuedo.push(
	              $('<fthtd>').css({
	                'display': 'table-cell',
	                'height': 0,
	                'width': 'auto'
	              })
	            );
	          }

	          cols = cols.join('');
	          cells = cells.join('');

	          if(createElements){
	            $fthRow.empty();
	            $fthRow.append(psuedo);
	            $fthCells = $fthRow.find('fthtd');
	          }

	          $sizerRow.html(cells);
	          $sizerCells = $sizerRow.find("th");
	          if(!existingColGroup){
	            $tableColGroup.html(cols);
	          }
	          $tableCells = $tableColGroup.find('col');
	          $floatColGroup.html(cols);
	          $headerCells = $floatColGroup.find("col");

	        }
	        return count;
	      }

	      function refloat(){ //make the thing float
	        if(!headerFloated){
	          headerFloated = true;
	          if(useAbsolutePositioning){ //#53, #56
	            var tw = tableWidth($table, $fthCells, true);
	            var wrapperWidth = $wrapper.width();
	            if(tw > wrapperWidth){
	              $table.css('minWidth', tw);
	            }
	          }
	          $table.css(layoutFixed);
	          $floatTable.css(layoutFixed);
	          $floatTable.append($header); //append because colgroup must go first in chrome
	          $tbody.before($newHeader);
	          setHeaderHeight();
	        }
	      }
	      function unfloat(){ //put the header back into the table
	        if(headerFloated){
	          headerFloated = false;
	          if(useAbsolutePositioning){ //#53, #56
	            $table.width(originalTableWidth);
	          }
	          $newHeader.detach();
	          $table.prepend($header);
	          $table.css(layoutAuto);
	          $floatTable.css(layoutAuto);
	          $table.css('minWidth', originalTableMinWidth); //this looks weird, but it's not a bug. Think about it!!
	          $table.css('minWidth', tableWidth($table, $fthCells)); //#121
	        }
	      }
	      var isHeaderFloatingLogical = false; //for the purpose of this event, the header is/isnt floating, even though the element
	                                           //might be in some other state. this is what the header looks like to the user
	      function triggerFloatEvent(isFloating){
	        if(isHeaderFloatingLogical != isFloating){
	          isHeaderFloatingLogical = isFloating;
	          $table.triggerHandler("floatThead", [isFloating, $floatContainer])
	        }
	      }
	      function changePositioning(isAbsolute){
	        if(useAbsolutePositioning != isAbsolute){
	          useAbsolutePositioning = isAbsolute;
	          $floatContainer.css({
	                                position: useAbsolutePositioning ? 'absolute' : 'fixed'
	                              });
	        }
	      }
	      function getSizingRow($table, $cols, $fthCells, ieVersion){
	        if(createElements){
	          return $fthCells;
	        } else if(ieVersion) {
	          return opts.getSizingRow($table, $cols, $fthCells);
	        } else {
	          return $cols;
	        }
	      }

	      /**
	       * returns a function that updates the floating header's cell widths.
	       * @return {Function}
	       */
	      function reflow(){
	        var i;
	        var numCols = columnNum(); //if the tables columns changed dynamically since last time (datatables), rebuild the sizer rows and get a new count

	        return function(){
	          //Cache the current scrollLeft value so that it can be reset post reflow
	          var scrollLeft = $floatContainer.scrollLeft();
	          $tableCells = $tableColGroup.find('col');
	          var $rowCells = getSizingRow($table, $tableCells, $fthCells, ieVersion);

	          if($rowCells.length == numCols && numCols > 0){
	            if(!existingColGroup){
	              for(i=0; i < numCols; i++){
	                $tableCells.eq(i).css('width', '');
	              }
	            }
	            unfloat();
	            var widths = [];
	            for(i=0; i < numCols; i++){
	              widths[i] = getOffsetWidth($rowCells.get(i));
	            }
	            for(i=0; i < numCols; i++){
	              $headerCells.eq(i).width(widths[i]);
	              $tableCells.eq(i).width(widths[i]);
	            }
	            refloat();
	          } else {
	            $floatTable.append($header);
	            $table.css(layoutAuto);
	            $floatTable.css(layoutAuto);
	            setHeaderHeight();
	          }
	          //Set back the current scrollLeft value on floatContainer
	          $floatContainer.scrollLeft(scrollLeft);
	          $table.triggerHandler("reflowed", [$floatContainer]);
	        };
	      }

	      function floatContainerBorderWidth(side){
	        var border = $scrollContainer.css("border-"+side+"-width");
	        var w = 0;
	        if (border && ~border.indexOf('px')) {
	          w = parseInt(border, 10);
	        }
	        return w;
	      }

	      function isResponsiveContainerActive(){
	        return $responsiveContainer.css("overflow-x") == 'auto';
	      }
	      /**
	       * first performs initial calculations that we expect to not change when the table, window, or scrolling container are scrolled.
	       * returns a function that calculates the floating container's top and left coords. takes into account if we are using page scrolling or inner scrolling
	       * @return {Function}
	       */
	      function calculateFloatContainerPosFn(){
	        var scrollingContainerTop = $scrollContainer.scrollTop();

	        //this floatEnd calc was moved out of the returned function because we assume the table height doesn't change (otherwise we must reinit by calling calculateFloatContainerPosFn)
	        var floatEnd;
	        var tableContainerGap = 0;
	        var captionHeight = haveCaption ? $caption.outerHeight(true) : 0;
	        var captionScrollOffset = captionAlignTop ? captionHeight : -captionHeight;

	        var floatContainerHeight = $floatContainer.height();
	        var tableOffset = $table.offset();
	        var tableLeftGap = 0; //can be caused by border on container (only in locked mode)
	        var tableTopGap = 0;
	        if(locked){
	          var containerOffset = $scrollContainer.offset();
	          tableContainerGap = tableOffset.top - containerOffset.top + scrollingContainerTop;
	          if(haveCaption && captionAlignTop){
	            tableContainerGap += captionHeight;
	          }
	          tableLeftGap = floatContainerBorderWidth('left');
	          tableTopGap = floatContainerBorderWidth('top');
	          tableContainerGap -= tableTopGap;
	        } else {
	          floatEnd = tableOffset.top - scrollingTop - floatContainerHeight + scrollingBottom + scrollbarOffset.horizontal;
	        }
	        var windowTop = $window.scrollTop();
	        var windowLeft = $window.scrollLeft();
	        var scrollContainerLeft = (
	            isResponsiveContainerActive() ?  $responsiveContainer :
	            (locked ? $scrollContainer : $window)
	        ).scrollLeft();

	        return function(eventType){
	          responsive = isResponsiveContainerActive();

	          var isTableHidden = $table[0].offsetWidth <= 0 && $table[0].offsetHeight <= 0;
	          if(!isTableHidden && floatTableHidden) {
	            floatTableHidden = false;
	            setTimeout(function(){
	              $table.triggerHandler("reflow");
	            }, 1);
	            return null;
	          }
	          if(isTableHidden){ //it's hidden
	            floatTableHidden = true;
	            if(!useAbsolutePositioning){
	              return null;
	            }
	          }

	          if(eventType == 'windowScroll'){
	            windowTop = $window.scrollTop();
	            windowLeft = $window.scrollLeft();
	          } else if(eventType == 'containerScroll'){
	            if($responsiveContainer.length){
	              if(!responsive){
	                return; //we dont care about the event if we arent responsive right now
	              }
	              scrollContainerLeft = $responsiveContainer.scrollLeft();
	            } else {
	              scrollingContainerTop = $scrollContainer.scrollTop();
	              scrollContainerLeft = $scrollContainer.scrollLeft();
	            }
	          } else if(eventType != 'init') {
	            windowTop = $window.scrollTop();
	            windowLeft = $window.scrollLeft();
	            scrollingContainerTop = $scrollContainer.scrollTop();
	            scrollContainerLeft =  (responsive ? $responsiveContainer : $scrollContainer).scrollLeft();
	          }
	          if(isWebkit && (windowTop < 0 || windowLeft < 0)){ //chrome overscroll effect at the top of the page - breaks fixed positioned floated headers
	            return;
	          }

	          if(absoluteToFixedOnScroll){
	            if(eventType == 'windowScrollDone'){
	              changePositioning(true); //change to absolute
	            } else {
	              changePositioning(false); //change to fixed
	            }
	          } else if(eventType == 'windowScrollDone'){
	            return null; //event is fired when they stop scrolling. ignore it if not 'absoluteToFixedOnScroll'
	          }

	          tableOffset = $table.offset();
	          if(haveCaption && captionAlignTop){
	            tableOffset.top += captionHeight;
	          }
	          var top, left;
	          var tableHeight = $table.outerHeight();

	          if(locked && useAbsolutePositioning){ //inner scrolling, absolute positioning
	            if (tableContainerGap >= scrollingContainerTop) {
	              var gap = tableContainerGap - scrollingContainerTop + tableTopGap;
	              top = gap > 0 ? gap : 0;
	              triggerFloatEvent(false);
	            } else {
	              top = wrappedContainer ? tableTopGap : scrollingContainerTop;
	              //headers stop at the top of the viewport
	              triggerFloatEvent(true);
	            }
	            left = tableLeftGap;
	          } else if(!locked && useAbsolutePositioning) { //window scrolling, absolute positioning
	            if(windowTop > floatEnd + tableHeight + captionScrollOffset){
	              top = tableHeight - floatContainerHeight + captionScrollOffset; //scrolled past table
	            } else if (tableOffset.top >= windowTop + scrollingTop) {
	              top = 0; //scrolling to table
	              unfloat();
	              triggerFloatEvent(false);
	            } else {
	              top = scrollingTop + windowTop - tableOffset.top + tableContainerGap + (captionAlignTop ? captionHeight : 0);
	              refloat(); //scrolling within table. header floated
	              triggerFloatEvent(true);
	            }
	            left =  scrollContainerLeft;
	          } else if(locked && !useAbsolutePositioning){ //inner scrolling, fixed positioning
	            if (tableContainerGap > scrollingContainerTop || scrollingContainerTop - tableContainerGap > tableHeight) {
	              top = tableOffset.top - windowTop;
	              unfloat();
	              triggerFloatEvent(false);
	            } else {
	              top = tableOffset.top + scrollingContainerTop  - windowTop - tableContainerGap;
	              refloat();
	              triggerFloatEvent(true);
	              //headers stop at the top of the viewport
	            }
	            left = tableOffset.left + scrollContainerLeft - windowLeft;
	          } else if(!locked && !useAbsolutePositioning) { //window scrolling, fixed positioning
	            if(windowTop > floatEnd + tableHeight + captionScrollOffset){
	              top = tableHeight + scrollingTop - windowTop + floatEnd + captionScrollOffset;
	              //scrolled past the bottom of the table
	            } else if (tableOffset.top > windowTop + scrollingTop) {
	              top = tableOffset.top - windowTop;
	              refloat();
	              triggerFloatEvent(false); //this is a weird case, the header never gets unfloated and i have no no way to know
	              //scrolled past the top of the table
	            } else {
	              //scrolling within the table
	              top = scrollingTop;
	              triggerFloatEvent(true);
	            }
	            left = tableOffset.left + scrollContainerLeft - windowLeft;
	          }
	          return {top: Math.round(top), left: Math.round(left)};
	        };
	      }
	      /**
	       * returns a function that caches old floating container position and only updates css when the position changes
	       * @return {Function}
	       */
	      function repositionFloatContainerFn(){
	        var oldTop = null;
	        var oldLeft = null;
	        var oldScrollLeft = null;
	        return function(pos, setWidth, setHeight){
	          if(pos != null && (oldTop != pos.top || oldLeft != pos.left)){
	            if(ieVersion === 8){
	              $floatContainer.css({
	                top: pos.top,
	                left: pos.left
	              });
	            } else {
	              var transform = 'translateX(' + pos.left + 'px) translateY(' + pos.top + 'px)';
	              $floatContainer.css({
	                '-webkit-transform' : transform,
	                '-moz-transform'    : transform,
	                '-ms-transform'     : transform,
	                '-o-transform'      : transform,
	                'transform'         : transform,
	                'top': 0,
	                'left': 0
	              });
	            }
	            oldTop = pos.top;
	            oldLeft = pos.left;
	          }
	          if(setWidth){
	            setFloatWidth();
	          }
	          if(setHeight){
	            setHeaderHeight();
	          }
	          var scrollLeft = (responsive ? $responsiveContainer : $scrollContainer).scrollLeft();
	          if(!useAbsolutePositioning || oldScrollLeft != scrollLeft){
	            $floatContainer.scrollLeft(scrollLeft);
	            oldScrollLeft = scrollLeft;
	          }
	        }
	      }

	      /**
	       * checks if THIS table has scrollbars, and finds their widths
	       */
	      function calculateScrollBarSize(){ //this should happen after the floating table has been positioned
	        if($scrollContainer.length){
	          if(opts.support && opts.support.perfectScrollbar && $scrollContainer.data().perfectScrollbar){
	            scrollbarOffset = {horizontal:0, vertical:0};
	          } else {
	            if($scrollContainer.css('overflow-x') == 'scroll'){
	              scrollbarOffset.horizontal = scWidth;
	            } else {
	              var sw = $scrollContainer.width(), tw = tableWidth($table, $fthCells);
	              var offsetv = sh < th ? scWidth : 0;
	              scrollbarOffset.horizontal = sw - offsetv < tw ? scWidth : 0;
	            }
	            if($scrollContainer.css('overflow-y') == 'scroll'){
	              scrollbarOffset.vertical = scWidth;
	            } else {
	              var sh = $scrollContainer.height(), th = $table.height();
	              var offseth = sw < tw ? scWidth : 0;
	              scrollbarOffset.vertical = sh - offseth < th ? scWidth : 0;
	            }
	          }
	        }
	      }
	      //finish up. create all calculation functions and bind them to events
	      calculateScrollBarSize();

	      var flow;

	      var ensureReflow = function(){
	        flow = reflow();
	        flow();
	      };

	      ensureReflow();

	      var calculateFloatContainerPos = calculateFloatContainerPosFn();
	      var repositionFloatContainer = repositionFloatContainerFn();

	      repositionFloatContainer(calculateFloatContainerPos('init'), true); //this must come after reflow because reflow changes scrollLeft back to 0 when it rips out the thead

	      var windowScrollDoneEvent = util.debounce(function(){
	        repositionFloatContainer(calculateFloatContainerPos('windowScrollDone'), false);
	      }, 1);

	      var windowScrollEvent = function(){
	        repositionFloatContainer(calculateFloatContainerPos('windowScroll'), false);
	        if(absoluteToFixedOnScroll){
	          windowScrollDoneEvent();
	        }
	      };
	      var containerScrollEvent = function(){
	        repositionFloatContainer(calculateFloatContainerPos('containerScroll'), false);
	      };


	      var windowResizeEvent = function(){
	        if($table.is(":hidden")){
	          return;
	        }
	        updateScrollingOffsets();
	        calculateScrollBarSize();
	        ensureReflow();
	        calculateFloatContainerPos = calculateFloatContainerPosFn();
	        repositionFloatContainer = repositionFloatContainerFn();
	        repositionFloatContainer(calculateFloatContainerPos('resize'), true, true);
	      };
	      var reflowEvent = util.debounce(function(){
	        if($table.is(":hidden")){
	          return;
	        }
	        calculateScrollBarSize();
	        updateScrollingOffsets();
	        ensureReflow();
	        calculateFloatContainerPos = calculateFloatContainerPosFn();
	        repositionFloatContainer(calculateFloatContainerPos('reflow'), true);
	      }, 1);

	      /////// printing stuff
	      var beforePrint = function(){
	        unfloat();
	      };
	      var afterPrint = function(){
	        refloat();
	      };
	      var printEvent = function(mql){
	        //make printing the table work properly on IE10+
	        if(mql.matches) {
	          beforePrint();
	        } else {
	          afterPrint();
	        }
	      };

	      var matchMediaPrint;
	      if(window.matchMedia && window.matchMedia('print').addListener){
	        matchMediaPrint = window.matchMedia("print");
	        matchMediaPrint.addListener(printEvent);
	      } else {
	        $window.on('beforeprint', beforePrint);
	        $window.on('afterprint', afterPrint);
	      }
	      ////// end printing stuff


	      if(locked){ //internal scrolling
	        if(useAbsolutePositioning){
	          $scrollContainer.on(eventName('scroll'), containerScrollEvent);
	        } else {
	          $scrollContainer.on(eventName('scroll'), containerScrollEvent);
	          $window.on(eventName('scroll'), windowScrollEvent);
	        }
	      } else { //window scrolling
	        $responsiveContainer.on(eventName('scroll'), containerScrollEvent);
	        $window.on(eventName('scroll'), windowScrollEvent);
	      }

	      $window.on(eventName('load'), reflowEvent); //for tables with images

	      windowResize(eventName('resize'), windowResizeEvent);
	      $table.on('reflow', reflowEvent);
	      if(opts.support && opts.support.datatables && isDatatable($table)){
	        $table
	            .on('filter', reflowEvent)
	            .on('sort',   reflowEvent)
	            .on('page',   reflowEvent);
	      }

	      if(opts.support && opts.support.bootstrap) {
	        $window.on(eventName('shown.bs.tab'), reflowEvent); // people cant seem to figure out how to use this plugin with bs3 tabs... so this :P
	      }
	      if(opts.support && opts.support.jqueryUI) {
	        $window.on(eventName('tabsactivate'), reflowEvent); // same thing for jqueryui
	      }


	      if (canObserveMutations) {
	        var mutationElement = null;
	        if(util.isFunction(opts.autoReflow)){
	          mutationElement = opts.autoReflow($table, $scrollContainer)
	        }
	        if(!mutationElement) {
	          mutationElement = $scrollContainer.length ? $scrollContainer[0] : $table[0]
	        }
	        mObs = new MutationObserver(function(e){
	          var wasTableRelated = function(nodes){
	            return nodes && nodes[0] && (nodes[0].nodeName == "THEAD" || nodes[0].nodeName == "TD"|| nodes[0].nodeName == "TH");
	          };
	          for(var i=0; i < e.length; i++){
	            if(!(wasTableRelated(e[i].addedNodes) || wasTableRelated(e[i].removedNodes))){
	              reflowEvent();
	              break;
	            }
	          }
	        });
	        mObs.observe(mutationElement, {
	          childList: true,
	          subtree: true
	        });
	      }

	      //attach some useful functions to the table.
	      $table.data('floatThead-attached', {
	        destroy: function(){
	          var ns = '.fth-'+floatTheadId;
	          unfloat();
	          $table.css(layoutAuto);
	          $tableColGroup.remove();
	          createElements && $fthGrp.remove();
	          if($newHeader.parent().length){ //only if it's in the DOM
	            $newHeader.replaceWith($header);
	          }
	          triggerFloatEvent(false);
	          if(canObserveMutations){
	            mObs.disconnect();
	            mObs = null;
	          }
	          $table.off('reflow reflowed');
	          $scrollContainer.off(ns);
	          $responsiveContainer.off(ns);
	          if (wrappedContainer) {
	            if ($scrollContainer.length) {
	              $scrollContainer.unwrap();
	            }
	            else {
	              $table.unwrap();
	            }
	          }
	          if(locked){
	            $scrollContainer.data('floatThead-containerWrap', false);
	          } else {
	            $table.data('floatThead-containerWrap', false);
	          }
	          $table.css('minWidth', originalTableMinWidth);
	          $floatContainer.remove();
	          $table.data('floatThead-attached', false);
	          $window.off(ns);
	          if (matchMediaPrint) {
	            matchMediaPrint.removeListener(printEvent);
	          }
	          beforePrint = afterPrint = function(){};

	          return function reinit(){
	            return $table.floatThead(opts);
	          }
	        },
	        reflow: function(){
	          reflowEvent();
	        },
	        setHeaderHeight: function(){
	          setHeaderHeight();
	        },
	        getFloatContainer: function(){
	          return $floatContainer;
	        },
	        getRowGroups: function(){
	          if(headerFloated){
	            return $floatContainer.find('>table>thead').add($table.children("tbody,tfoot"));
	          } else {
	            return $table.children("thead,tbody,tfoot");
	          }
	        }
	      });
	    });
	    return this;
	  };
	})((function(){
	  var $ = window.jQuery;
	  if(typeof module !== 'undefined' && module.exports && !$) {
	    // only use cjs if they dont have a jquery for me to use, and we have commonjs
	    $ = __webpack_require__(1);
	  }
	  return $;
	})());

	/* jQuery.floatThead.utils - http://mkoryak.github.io/floatThead/ - Copyright (c) 2012 - 2016 Misha Koryak
	 * License: MIT
	 *
	 * This file is required if you do not use underscore in your project and you want to use floatThead.
	 * It contains functions from underscore that the plugin uses.
	 *
	 * YOU DON'T NEED TO INCLUDE THIS IF YOU ALREADY INCLUDE UNDERSCORE!
	 *
	 */

	(function($){

	  $.floatThead = $.floatThead || {};

	  $.floatThead._  = window._ || (function(){
	    var that = {};
	    var hasOwnProperty = Object.prototype.hasOwnProperty, isThings = ['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp'];
	    that.has = function(obj, key) {
	      return hasOwnProperty.call(obj, key);
	    };
	    that.keys = function(obj) {
	      if (obj !== Object(obj)) throw new TypeError('Invalid object');
	      var keys = [];
	      for (var key in obj) if (that.has(obj, key)) keys.push(key);
	      return keys;
	    };
	    var idCounter = 0;
	    that.uniqueId = function(prefix) {
	      var id = ++idCounter + '';
	      return prefix ? prefix + id : id;
	    };
	    $.each(isThings, function(){
	      var name = this;
	      that['is' + name] = function(obj) {
	        return Object.prototype.toString.call(obj) == '[object ' + name + ']';
	      };
	    });
	    that.debounce = function(func, wait, immediate) {
	      var timeout, args, context, timestamp, result;
	      return function() {
	        context = this;
	        args = arguments;
	        timestamp = new Date();
	        var later = function() {
	          var last = (new Date()) - timestamp;
	          if (last < wait) {
	            timeout = setTimeout(later, wait - last);
	          } else {
	            timeout = null;
	            if (!immediate) result = func.apply(context, args);
	          }
	        };
	        var callNow = immediate && !timeout;
	        if (!timeout) {
	          timeout = setTimeout(later, wait);
	        }
	        if (callNow) result = func.apply(context, args);
	        return result;
	      };
	    };
	    return that;
	  })();
	})(jQuery);


	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 34 */
/***/ (function(module, exports) {

	/*** IMPORTS FROM imports-loader ***/
	var require = false;
	var module = false;

	/*=============================================================================
		Author:			Eric M. Barnard - @ericmbarnard								
		License:		MIT (http://opensource.org/licenses/mit-license.php)		
																					
		Description:	Validation Library for KnockoutJS							
		Version:		2.0.2											
	===============================================================================
	*/
	/*globals require: false, exports: false, define: false, ko: false */

	(function (factory) {
		// Module systems magic dance.

		if (typeof require === "function" && typeof exports === "object" && typeof module === "object") {
			// CommonJS or Node: hard-coded dependency on "knockout"
			factory(require("knockout"), exports);
		} else if (typeof define === "function" && define["amd"]) {
			// AMD anonymous module with hard-coded dependency on "knockout"
			define(["knockout", "exports"], factory);
		} else {
			// <script> tag: use the global `ko` object, attaching a `mapping` property
			factory(ko, ko.validation = {});
		}
	}(function ( ko, exports ) {

		if (typeof (ko) === 'undefined') {
			throw new Error('Knockout is required, please ensure it is loaded before loading this validation plug-in');
		}

		// create our namespace object
		ko.validation = exports;

		var kv = ko.validation,
			koUtils = ko.utils,
			unwrap = koUtils.unwrapObservable,
			forEach = koUtils.arrayForEach,
			extend = koUtils.extend;
	;/*global ko: false*/

	var defaults = {
		registerExtenders: true,
		messagesOnModified: true,
		errorsAsTitle: true,            // enables/disables showing of errors as title attribute of the target element.
		errorsAsTitleOnModified: false, // shows the error when hovering the input field (decorateElement must be true)
		messageTemplate: null,
		insertMessages: true,           // automatically inserts validation messages as <span></span>
		parseInputAttributes: false,    // parses the HTML5 validation attribute from a form element and adds that to the object
		writeInputAttributes: false,    // adds HTML5 input validation attributes to form elements that ko observable's are bound to
		decorateInputElement: false,         // false to keep backward compatibility
		decorateElementOnModified: true,// true to keep backward compatibility
		errorClass: null,               // single class for error message and element
		errorElementClass: 'validationElement',  // class to decorate error element
		errorMessageClass: 'validationMessage',  // class to decorate error message
		allowHtmlMessages: false,		// allows HTML in validation messages
		grouping: {
			deep: false,        //by default grouping is shallow
			observable: true,   //and using observables
			live: false		    //react to changes to observableArrays if observable === true
		},
		validate: {
			// throttle: 10
		}
	};

	// make a copy  so we can use 'reset' later
	var configuration = extend({}, defaults);

	configuration.html5Attributes = ['required', 'pattern', 'min', 'max', 'step'];
	configuration.html5InputTypes = ['email', 'number', 'date'];

	configuration.reset = function () {
		extend(configuration, defaults);
	};

	kv.configuration = configuration;
	;kv.utils = (function () {
		var seedId = new Date().getTime();

		var domData = {}; //hash of data objects that we reference from dom elements
		var domDataKey = '__ko_validation__';

		return {
			isArray: function (o) {
				return o.isArray || Object.prototype.toString.call(o) === '[object Array]';
			},
			isObject: function (o) {
				return o !== null && typeof o === 'object';
			},
			isNumber: function(o) {
				return !isNaN(o);	
			},
			isObservableArray: function(instance) {
				return !!instance &&
						typeof instance["remove"] === "function" &&
						typeof instance["removeAll"] === "function" &&
						typeof instance["destroy"] === "function" &&
						typeof instance["destroyAll"] === "function" &&
						typeof instance["indexOf"] === "function" &&
						typeof instance["replace"] === "function";
			},
			values: function (o) {
				var r = [];
				for (var i in o) {
					if (o.hasOwnProperty(i)) {
						r.push(o[i]);
					}
				}
				return r;
			},
			getValue: function (o) {
				return (typeof o === 'function' ? o() : o);
			},
			hasAttribute: function (node, attr) {
				return node.getAttribute(attr) !== null;
			},
			getAttribute: function (element, attr) {
				return element.getAttribute(attr);
			},
			setAttribute: function (element, attr, value) {
				return element.setAttribute(attr, value);
			},
			isValidatable: function (o) {
				return !!(o && o.rules && o.isValid && o.isModified);
			},
			insertAfter: function (node, newNode) {
				node.parentNode.insertBefore(newNode, node.nextSibling);
			},
			newId: function () {
				return seedId += 1;
			},
			getConfigOptions: function (element) {
				var options = kv.utils.contextFor(element);

				return options || kv.configuration;
			},
			setDomData: function (node, data) {
				var key = node[domDataKey];

				if (!key) {
					node[domDataKey] = key = kv.utils.newId();
				}

				domData[key] = data;
			},
			getDomData: function (node) {
				var key = node[domDataKey];

				if (!key) {
					return undefined;
				}

				return domData[key];
			},
			contextFor: function (node) {
				switch (node.nodeType) {
					case 1:
					case 8:
						var context = kv.utils.getDomData(node);
						if (context) { return context; }
						if (node.parentNode) { return kv.utils.contextFor(node.parentNode); }
						break;
				}
				return undefined;
			},
			isEmptyVal: function (val) {
				if (val === undefined) {
					return true;
				}
				if (val === null) {
					return true;
				}
				if (val === "") {
					return true;
				}
			},
			getOriginalElementTitle: function (element) {
				var savedOriginalTitle = kv.utils.getAttribute(element, 'data-orig-title'),
					currentTitle = element.title,
					hasSavedOriginalTitle = kv.utils.hasAttribute(element, 'data-orig-title');

				return hasSavedOriginalTitle ?
					savedOriginalTitle : currentTitle;
			},
			async: function (expr) {
				if (window.setImmediate) { window.setImmediate(expr); }
				else { window.setTimeout(expr, 0); }
			},
			forEach: function (object, callback) {
				if (kv.utils.isArray(object)) {
					return forEach(object, callback);
				}
				for (var prop in object) {
					if (object.hasOwnProperty(prop)) {
						callback(object[prop], prop);
					}
				}
			}
		};
	}());;var api = (function () {

		var isInitialized = 0,
			configuration = kv.configuration,
			utils = kv.utils;

		function cleanUpSubscriptions(context) {
			forEach(context.subscriptions, function (subscription) {
				subscription.dispose();
			});
			context.subscriptions = [];
		}

		function dispose(context) {
			if (context.options.deep) {
				forEach(context.flagged, function (obj) {
					delete obj.__kv_traversed;
				});
				context.flagged.length = 0;
			}

			if (!context.options.live) {
				cleanUpSubscriptions(context);
			}
		}

		function runTraversal(obj, context) {
			context.validatables = [];
			cleanUpSubscriptions(context);
			traverseGraph(obj, context);
			dispose(context);
		}

		function traverseGraph(obj, context, level) {
			var objValues = [],
				val = obj.peek ? obj.peek() : obj;

			if (obj.__kv_traversed === true) {
				return;
			}

			if (context.options.deep) {
				obj.__kv_traversed = true;
				context.flagged.push(obj);
			}

			//default level value depends on deep option.
			level = (level !== undefined ? level : context.options.deep ? 1 : -1);

			// if object is observable then add it to the list
			if (ko.isObservable(obj)) {
				// ensure it's validatable but don't extend validatedObservable because it
				// would overwrite isValid property.
				if (!obj.errors && !utils.isValidatable(obj)) {
					obj.extend({ validatable: true });
				}
				context.validatables.push(obj);

				if (context.options.live && utils.isObservableArray(obj)) {
					context.subscriptions.push(obj.subscribe(function () {
						context.graphMonitor.valueHasMutated();
					}));
				}
			}

			//get list of values either from array or object but ignore non-objects
			// and destroyed objects
			if (val && !val._destroy) {
				if (utils.isArray(val)) {
					objValues = val;
				}
				else if (utils.isObject(val)) {
					objValues = utils.values(val);
				}
			}

			//process recursively if it is deep grouping
			if (level !== 0) {
				utils.forEach(objValues, function (observable) {
					//but not falsy things and not HTML Elements
					if (observable && !observable.nodeType && (!ko.isComputed(observable) || observable.rules)) {
						traverseGraph(observable, context, level + 1);
					}
				});
			}
		}

		function collectErrors(array) {
			var errors = [];
			forEach(array, function (observable) {
				// Do not collect validatedObservable errors
				if (utils.isValidatable(observable) && !observable.isValid()) {
					// Use peek because we don't want a dependency for 'error' property because it
					// changes before 'isValid' does. (Issue #99)
					errors.push(observable.error.peek());
				}
			});
			return errors;
		}

		return {
			//Call this on startup
			//any config can be overridden with the passed in options
			init: function (options, force) {
				//done run this multiple times if we don't really want to
				if (isInitialized > 0 && !force) {
					return;
				}

				//because we will be accessing options properties it has to be an object at least
				options = options || {};
				//if specific error classes are not provided then apply generic errorClass
				//it has to be done on option so that options.errorClass can override default
				//errorElementClass and errorMessage class but not those provided in options
				options.errorElementClass = options.errorElementClass || options.errorClass || configuration.errorElementClass;
				options.errorMessageClass = options.errorMessageClass || options.errorClass || configuration.errorMessageClass;

				extend(configuration, options);

				if (configuration.registerExtenders) {
					kv.registerExtenders();
				}

				isInitialized = 1;
			},

			// resets the config back to its original state
			reset: kv.configuration.reset,

			// recursively walks a viewModel and creates an object that
			// provides validation information for the entire viewModel
			// obj -> the viewModel to walk
			// options -> {
			//	  deep: false, // if true, will walk past the first level of viewModel properties
			//	  observable: false // if true, returns a computed observable indicating if the viewModel is valid
			// }
			group: function group(obj, options) { // array of observables or viewModel
				options = extend(extend({}, configuration.grouping), options);

				var context = {
					options: options,
					graphMonitor: ko.observable(),
					flagged: [],
					subscriptions: [],
					validatables: []
				};

				var result = null;

				//if using observables then traverse structure once and add observables
				if (options.observable) {
					result = ko.computed(function () {
						context.graphMonitor(); //register dependency
						runTraversal(obj, context);
						return collectErrors(context.validatables);
					});
				}
				else { //if not using observables then every call to error() should traverse the structure
					result = function () {
						runTraversal(obj, context);
						return collectErrors(context.validatables);
					};
				}

				result.showAllMessages = function (show) { // thanks @heliosPortal
					if (show === undefined) {//default to true
						show = true;
					}

					result.forEach(function (observable) {
						if (utils.isValidatable(observable)) {
							observable.isModified(show);
						}
					});
				};

				result.isAnyMessageShown = function () {
					var invalidAndModifiedPresent;

					invalidAndModifiedPresent = !!result.find(function (observable) {
						return utils.isValidatable(observable) && !observable.isValid() && observable.isModified();
					});
					return invalidAndModifiedPresent;
				};

				result.filter = function(predicate) {
					predicate = predicate || function () { return true; };
					// ensure we have latest changes
					result();

					return koUtils.arrayFilter(context.validatables, predicate);
				};

				result.find = function(predicate) {
					predicate = predicate || function () { return true; };
					// ensure we have latest changes
					result();

					return koUtils.arrayFirst(context.validatables, predicate);
				};

				result.forEach = function(callback) {
					callback = callback || function () { };
					// ensure we have latest changes
					result();

					forEach(context.validatables, callback);
				};

				result.map = function(mapping) {
					mapping = mapping || function (item) { return item; };
					// ensure we have latest changes
					result();

					return koUtils.arrayMap(context.validatables, mapping);
				};

				/**
				 * @private You should not rely on this method being here.
				 * It's a private method and it may change in the future.
				 *
				 * @description Updates the validated object and collects errors from it.
				 */
				result._updateState = function(newValue) {
					if (!utils.isObject(newValue)) {
						throw new Error('An object is required.');
					}
					obj = newValue;
					if (options.observable) {
						context.graphMonitor.valueHasMutated();
					}
					else {
						runTraversal(newValue, context);
						return collectErrors(context.validatables);
					}
				};
				return result;
			},

			formatMessage: function (message, params, observable) {
				if (utils.isObject(params) && params.typeAttr) {
					params = params.value;
				}
				if (typeof (message) === 'function') {
					return message(params, observable);
				}
				var replacements = unwrap(params) || [];
				if (!utils.isArray(replacements)) {
					replacements = [replacements];
				}
				return message.replace(/{(\d+)}/gi, function(match, index) {
					if (typeof replacements[index] !== 'undefined') {
						return replacements[index];
					}
					return match;
				});
			},

			// addRule:
			// This takes in a ko.observable and a Rule Context - which is just a rule name and params to supply to the validator
			// ie: kv.addRule(myObservable, {
			//		  rule: 'required',
			//		  params: true
			//	  });
			//
			addRule: function (observable, rule) {
				observable.extend({ validatable: true });

				var hasRule = !!koUtils.arrayFirst(observable.rules(), function(item) {
					return item.rule && item.rule === rule.rule;
				});

				if (!hasRule) {
					//push a Rule Context to the observables local array of Rule Contexts
					observable.rules.push(rule);
				}
				return observable;
			},

			// addAnonymousRule:
			// Anonymous Rules essentially have all the properties of a Rule, but are only specific for a certain property
			// and developers typically are wanting to add them on the fly or not register a rule with the 'kv.rules' object
			//
			// Example:
			// var test = ko.observable('something').extend{(
			//	  validation: {
			//		  validator: function(val, someOtherVal){
			//			  return true;
			//		  },
			//		  message: "Something must be really wrong!',
			//		  params: true
			//	  }
			//  )};
			addAnonymousRule: function (observable, ruleObj) {
				if (ruleObj['message'] === undefined) {
					ruleObj['message'] = 'Error';
				}

				//make sure onlyIf is honoured
				if (ruleObj.onlyIf) {
					ruleObj.condition = ruleObj.onlyIf;
				}

				//add the anonymous rule to the observable
				kv.addRule(observable, ruleObj);
			},

			addExtender: function (ruleName) {
				ko.extenders[ruleName] = function (observable, params) {
					//params can come in a few flavors
					// 1. Just the params to be passed to the validator
					// 2. An object containing the Message to be used and the Params to pass to the validator
					// 3. A condition when the validation rule to be applied
					//
					// Example:
					// var test = ko.observable(3).extend({
					//	  max: {
					//		  message: 'This special field has a Max of {0}',
					//		  params: 2,
					//		  onlyIf: function() {
					//					  return specialField.IsVisible();
					//				  }
					//	  }
					//  )};
					//
					if (params && (params.message || params.onlyIf)) { //if it has a message or condition object, then its an object literal to use
						return kv.addRule(observable, {
							rule: ruleName,
							message: params.message,
							params: utils.isEmptyVal(params.params) ? true : params.params,
							condition: params.onlyIf
						});
					} else {
						return kv.addRule(observable, {
							rule: ruleName,
							params: params
						});
					}
				};
			},

			// loops through all kv.rules and adds them as extenders to
			// ko.extenders
			registerExtenders: function () { // root extenders optional, use 'validation' extender if would cause conflicts
				if (configuration.registerExtenders) {
					for (var ruleName in kv.rules) {
						if (kv.rules.hasOwnProperty(ruleName)) {
							if (!ko.extenders[ruleName]) {
								kv.addExtender(ruleName);
							}
						}
					}
				}
			},

			//creates a span next to the @element with the specified error class
			insertValidationMessage: function (element) {
				var span = document.createElement('SPAN');
				span.className = utils.getConfigOptions(element).errorMessageClass;
				utils.insertAfter(element, span);
				return span;
			},

			// if html-5 validation attributes have been specified, this parses
			// the attributes on @element
			parseInputValidationAttributes: function (element, valueAccessor) {
				forEach(kv.configuration.html5Attributes, function (attr) {
					if (utils.hasAttribute(element, attr)) {

						var params = element.getAttribute(attr) || true;

						if (attr === 'min' || attr === 'max')
						{
							// If we're validating based on the min and max attributes, we'll
							// need to know what the 'type' attribute is set to
							var typeAttr = element.getAttribute('type');
							if (typeof typeAttr === "undefined" || !typeAttr)
							{
								// From http://www.w3.org/TR/html-markup/input:
								//   An input element with no type attribute specified represents the
								//   same thing as an input element with its type attribute set to "text".
								typeAttr = "text";
							}
							params = {typeAttr: typeAttr, value: params};
						}

						kv.addRule(valueAccessor(), {
							rule: attr,
							params: params
						});
					}
				});

				var currentType = element.getAttribute('type');
				forEach(kv.configuration.html5InputTypes, function (type) {
					if (type === currentType) {
						kv.addRule(valueAccessor(), {
							rule: (type === 'date') ? 'dateISO' : type,
							params: true
						});
					}
				});
			},

			// writes html5 validation attributes on the element passed in
			writeInputValidationAttributes: function (element, valueAccessor) {
				var observable = valueAccessor();

				if (!observable || !observable.rules) {
					return;
				}

				var contexts = observable.rules(); // observable array

				// loop through the attributes and add the information needed
				forEach(kv.configuration.html5Attributes, function (attr) {
					var ctx = koUtils.arrayFirst(contexts, function (ctx) {
						return ctx.rule && ctx.rule.toLowerCase() === attr.toLowerCase();
					});

					if (!ctx) {
						return;
					}

					// we have a rule matching a validation attribute at this point
					// so lets add it to the element along with the params
					ko.computed({
						read: function() {
							var params = ko.unwrap(ctx.params);

							// we have to do some special things for the pattern validation
							if (ctx.rule === "pattern" && params instanceof RegExp) {
								// we need the pure string representation of the RegExpr without the //gi stuff
								params = params.source;
							}

							element.setAttribute(attr, params);
						},
						disposeWhenNodeIsRemoved: element
					});
				});

				contexts = null;
			},

			//take an existing binding handler and make it cause automatic validations
			makeBindingHandlerValidatable: function (handlerName) {
				var init = ko.bindingHandlers[handlerName].init;

				ko.bindingHandlers[handlerName].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

					init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);

					return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
				};
			},

			// visit an objects properties and apply validation rules from a definition
			setRules: function (target, definition) {
				var setRules = function (target, definition) {
					if (!target || !definition) { return; }

					for (var prop in definition) {
						if (!definition.hasOwnProperty(prop)) { continue; }
						var ruleDefinitions = definition[prop];

						//check the target property exists and has a value
						if (!target[prop]) { continue; }
						var targetValue = target[prop],
							unwrappedTargetValue = unwrap(targetValue),
							rules = {},
							nonRules = {};

						for (var rule in ruleDefinitions) {
							if (!ruleDefinitions.hasOwnProperty(rule)) { continue; }
							if (kv.rules[rule]) {
								rules[rule] = ruleDefinitions[rule];
							} else {
								nonRules[rule] = ruleDefinitions[rule];
							}
						}

						//apply rules
						if (ko.isObservable(targetValue)) {
							targetValue.extend(rules);
						}

						//then apply child rules
						//if it's an array, apply rules to all children
						if (unwrappedTargetValue && utils.isArray(unwrappedTargetValue)) {
							for (var i = 0; i < unwrappedTargetValue.length; i++) {
								setRules(unwrappedTargetValue[i], nonRules);
							}
							//otherwise, just apply to this property
						} else {
							setRules(unwrappedTargetValue, nonRules);
						}
					}
				};
				setRules(target, definition);
			}
		};

	}());

	// expose api publicly
	extend(ko.validation, api);
	;//Validation Rules:
	// You can view and override messages or rules via:
	// kv.rules[ruleName]
	//
	// To implement a custom Rule, simply use this template:
	// kv.rules['<custom rule name>'] = {
	//      validator: function (val, param) {
	//          <custom logic>
	//          return <true or false>;
	//      },
	//      message: '<custom validation message>' //optionally you can also use a '{0}' to denote a placeholder that will be replaced with your 'param'
	// };
	//
	// Example:
	// kv.rules['mustEqual'] = {
	//      validator: function( val, mustEqualVal ){
	//          return val === mustEqualVal;
	//      },
	//      message: 'This field must equal {0}'
	// };
	//
	kv.rules = {};
	kv.rules['required'] = {
		validator: function (val, required) {
			var testVal;

			if (val === undefined || val === null) {
				return !required;
			}

			testVal = val;
			if (typeof (val) === 'string') {
				if (String.prototype.trim) {
					testVal = val.trim();
				}
				else {
					testVal = val.replace(/^\s+|\s+$/g, '');
				}
			}

			if (!required) {// if they passed: { required: false }, then don't require this
				return true;
			}

			return ((testVal + '').length > 0);
		},
		message: 'This field is required.'
	};

	function minMaxValidatorFactory(validatorName) {
	    var isMaxValidation = validatorName === "max";

	    return function (val, options) {
	        if (kv.utils.isEmptyVal(val)) {
	            return true;
	        }

	        var comparisonValue, type;
	        if (options.typeAttr === undefined) {
	            // This validator is being called from javascript rather than
	            // being bound from markup
	            type = "text";
	            comparisonValue = options;
	        } else {
	            type = options.typeAttr;
	            comparisonValue = options.value;
	        }

	        // From http://www.w3.org/TR/2012/WD-html5-20121025/common-input-element-attributes.html#attr-input-min,
	        // if the value is parseable to a number, then the minimum should be numeric
	        if (!isNaN(comparisonValue) && !(comparisonValue instanceof Date)) {
	            type = "number";
	        }

	        var regex, valMatches, comparisonValueMatches;
	        switch (type.toLowerCase()) {
	            case "week":
	                regex = /^(\d{4})-W(\d{2})$/;
	                valMatches = val.match(regex);
	                if (valMatches === null) {
	                    throw new Error("Invalid value for " + validatorName + " attribute for week input.  Should look like " +
	                        "'2000-W33' http://www.w3.org/TR/html-markup/input.week.html#input.week.attrs.min");
	                }
	                comparisonValueMatches = comparisonValue.match(regex);
	                // If no regex matches were found, validation fails
	                if (!comparisonValueMatches) {
	                    return false;
	                }

	                if (isMaxValidation) {
	                    return (valMatches[1] < comparisonValueMatches[1]) || // older year
	                        // same year, older week
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] <= comparisonValueMatches[2]));
	                } else {
	                    return (valMatches[1] > comparisonValueMatches[1]) || // newer year
	                        // same year, newer week
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] >= comparisonValueMatches[2]));
	                }
	                break;

	            case "month":
	                regex = /^(\d{4})-(\d{2})$/;
	                valMatches = val.match(regex);
	                if (valMatches === null) {
	                    throw new Error("Invalid value for " + validatorName + " attribute for month input.  Should look like " +
	                        "'2000-03' http://www.w3.org/TR/html-markup/input.month.html#input.month.attrs.min");
	                }
	                comparisonValueMatches = comparisonValue.match(regex);
	                // If no regex matches were found, validation fails
	                if (!comparisonValueMatches) {
	                    return false;
	                }

	                if (isMaxValidation) {
	                    return ((valMatches[1] < comparisonValueMatches[1]) || // older year
	                        // same year, older month
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] <= comparisonValueMatches[2])));
	                } else {
	                    return (valMatches[1] > comparisonValueMatches[1]) || // newer year
	                        // same year, newer month
	                        ((valMatches[1] === comparisonValueMatches[1]) && (valMatches[2] >= comparisonValueMatches[2]));
	                }
	                break;

	            case "number":
	            case "range":
	                if (isMaxValidation) {
	                    return (!isNaN(val) && parseFloat(val) <= parseFloat(comparisonValue));
	                } else {
	                    return (!isNaN(val) && parseFloat(val) >= parseFloat(comparisonValue));
	                }
	                break;

	            default:
	                if (isMaxValidation) {
	                    return val <= comparisonValue;
	                } else {
	                    return val >= comparisonValue;
	                }
	        }
	    };
	}

	kv.rules['min'] = {
		validator: minMaxValidatorFactory("min"),
		message: 'Please enter a value greater than or equal to {0}.'
	};

	kv.rules['max'] = {
		validator: minMaxValidatorFactory("max"),
		message: 'Please enter a value less than or equal to {0}.'
	};

	kv.rules['minLength'] = {
		validator: function (val, minLength) {
			if(kv.utils.isEmptyVal(val)) { return true; }
			var normalizedVal = kv.utils.isNumber(val) ? ('' + val) : val;
			return normalizedVal.length >= minLength;
		},
		message: 'Please enter at least {0} characters.'
	};

	kv.rules['maxLength'] = {
		validator: function (val, maxLength) {
			if(kv.utils.isEmptyVal(val)) { return true; }
			var normalizedVal = kv.utils.isNumber(val) ? ('' + val) : val;
			return normalizedVal.length <= maxLength;
		},
		message: 'Please enter no more than {0} characters.'
	};

	kv.rules['pattern'] = {
		validator: function (val, regex) {
			return kv.utils.isEmptyVal(val) || val.toString().match(regex) !== null;
		},
		message: 'Please check this value.'
	};

	kv.rules['step'] = {
		validator: function (val, step) {

			// in order to handle steps of .1 & .01 etc.. Modulus won't work
			// if the value is a decimal, so we have to correct for that
			if (kv.utils.isEmptyVal(val) || step === 'any') { return true; }
			var dif = (val * 100) % (step * 100);
			return Math.abs(dif) < 0.00001 || Math.abs(1 - dif) < 0.00001;
		},
		message: 'The value must increment by {0}.'
	};

	kv.rules['email'] = {
		validator: function (val, validate) {
			if (!validate) { return true; }

			//I think an empty email address is also a valid entry
			//if one want's to enforce entry it should be done with 'required: true'
			return kv.utils.isEmptyVal(val) || (
				// jquery validate regex - thanks Scott Gonzalez
				validate && /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i.test(val)
			);
		},
		message: 'Please enter a proper email address.'
	};

	kv.rules['date'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && !/Invalid|NaN/.test(new Date(value)));
		},
		message: 'Please enter a proper date.'
	};

	kv.rules['dateISO'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^\d{4}[-/](?:0?[1-9]|1[012])[-/](?:0?[1-9]|[12][0-9]|3[01])$/.test(value));
		},
		message: 'Please enter a proper date.'
	};

	kv.rules['number'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(value));
		},
		message: 'Please enter a number.'
	};

	kv.rules['digit'] = {
		validator: function (value, validate) {
			if (!validate) { return true; }
			return kv.utils.isEmptyVal(value) || (validate && /^\d+$/.test(value));
		},
		message: 'Please enter a digit.'
	};

	kv.rules['phoneUS'] = {
		validator: function (phoneNumber, validate) {
			if (!validate) { return true; }
			if (kv.utils.isEmptyVal(phoneNumber)) { return true; } // makes it optional, use 'required' rule if it should be required
			if (typeof (phoneNumber) !== 'string') { return false; }
			phoneNumber = phoneNumber.replace(/\s+/g, "");
			return validate && phoneNumber.length > 9 && phoneNumber.match(/^(1-?)?(\([2-9]\d{2}\)|[2-9]\d{2})-?[2-9]\d{2}-?\d{4}$/);
		},
		message: 'Please specify a valid phone number.'
	};

	kv.rules['equal'] = {
		validator: function (val, params) {
			var otherValue = params;
			return val === kv.utils.getValue(otherValue);
		},
		message: 'Values must equal.'
	};

	kv.rules['notEqual'] = {
		validator: function (val, params) {
			var otherValue = params;
			return val !== kv.utils.getValue(otherValue);
		},
		message: 'Please choose another value.'
	};

	//unique in collection
	// options are:
	//    collection: array or function returning (observable) array
	//              in which the value has to be unique
	//    valueAccessor: function that returns value from an object stored in collection
	//              if it is null the value is compared directly
	//    external: set to true when object you are validating is automatically updating collection
	kv.rules['unique'] = {
		validator: function (val, options) {
			var c = kv.utils.getValue(options.collection),
				external = kv.utils.getValue(options.externalValue),
				counter = 0;

			if (!val || !c) { return true; }

			koUtils.arrayFilter(c, function (item) {
				if (val === (options.valueAccessor ? options.valueAccessor(item) : item)) { counter++; }
			});
			// if value is external even 1 same value in collection means the value is not unique
			return counter < (!!external ? 1 : 2);
		},
		message: 'Please make sure the value is unique.'
	};


	//now register all of these!
	(function () {
		kv.registerExtenders();
	}());
	;// The core binding handler
	// this allows us to setup any value binding that internally always
	// performs the same functionality
	ko.bindingHandlers['validationCore'] = (function () {

		return {
			init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
				var config = kv.utils.getConfigOptions(element);
				var observable = valueAccessor();

				// parse html5 input validation attributes, optional feature
				if (config.parseInputAttributes) {
					kv.utils.async(function () { kv.parseInputValidationAttributes(element, valueAccessor); });
				}

				// if requested insert message element and apply bindings
				if (config.insertMessages && kv.utils.isValidatable(observable)) {

					// insert the <span></span>
					var validationMessageElement = kv.insertValidationMessage(element);

					// if we're told to use a template, make sure that gets rendered
					if (config.messageTemplate) {
						ko.renderTemplate(config.messageTemplate, { field: observable }, null, validationMessageElement, 'replaceNode');
					} else {
						ko.applyBindingsToNode(validationMessageElement, { validationMessage: observable });
					}
				}

				// write the html5 attributes if indicated by the config
				if (config.writeInputAttributes && kv.utils.isValidatable(observable)) {

					kv.writeInputValidationAttributes(element, valueAccessor);
				}

				// if requested, add binding to decorate element
				if (config.decorateInputElement && kv.utils.isValidatable(observable)) {
					ko.applyBindingsToNode(element, { validationElement: observable });
				}
			}
		};

	}());

	// override for KO's default 'value', 'checked', 'textInput' and selectedOptions bindings
	kv.makeBindingHandlerValidatable("value");
	kv.makeBindingHandlerValidatable("checked");
	if (ko.bindingHandlers.textInput) {
		kv.makeBindingHandlerValidatable("textInput");
	}
	kv.makeBindingHandlerValidatable("selectedOptions");


	ko.bindingHandlers['validationMessage'] = { // individual error message, if modified or post binding
		update: function (element, valueAccessor) {
			var obsv = valueAccessor(),
				config = kv.utils.getConfigOptions(element),
				val = unwrap(obsv),
				msg = null,
				isModified = false,
				isValid = false;

			if (obsv === null || typeof obsv === 'undefined') {
				throw new Error('Cannot bind validationMessage to undefined value. data-bind expression: ' +
					element.getAttribute('data-bind'));
			}

			isModified = obsv.isModified && obsv.isModified();
			isValid = obsv.isValid && obsv.isValid();

			var error = null;
			if (!config.messagesOnModified || isModified) {
				error = isValid ? null : obsv.error;
			}

			var isVisible = !config.messagesOnModified || isModified ? !isValid : false;
			var isCurrentlyVisible = element.style.display !== "none";

			if (config.allowHtmlMessages) {
				koUtils.setHtml(element, error);
			} else {
				ko.bindingHandlers.text.update(element, function () { return error; });
			}

			if (isCurrentlyVisible && !isVisible) {
				element.style.display = 'none';
			} else if (!isCurrentlyVisible && isVisible) {
				element.style.display = '';
			}
		}
	};

	ko.bindingHandlers['validationElement'] = {
		update: function (element, valueAccessor, allBindingsAccessor) {
			var obsv = valueAccessor(),
				config = kv.utils.getConfigOptions(element),
				val = unwrap(obsv),
				msg = null,
				isModified = false,
				isValid = false;

			if (obsv === null || typeof obsv === 'undefined') {
				throw new Error('Cannot bind validationElement to undefined value. data-bind expression: ' +
					element.getAttribute('data-bind'));
			}

			isModified = obsv.isModified && obsv.isModified();
			isValid = obsv.isValid && obsv.isValid();

			// create an evaluator function that will return something like:
			// css: { validationElement: true }
			var cssSettingsAccessor = function () {
				var css = {};

				var shouldShow = ((!config.decorateElementOnModified || isModified) ? !isValid : false);

				// css: { validationElement: false }
				css[config.errorElementClass] = shouldShow;

				return css;
			};

			//add or remove class on the element;
			ko.bindingHandlers.css.update(element, cssSettingsAccessor, allBindingsAccessor);
			if (!config.errorsAsTitle) { return; }

			ko.bindingHandlers.attr.update(element, function () {
				var
					hasModification = !config.errorsAsTitleOnModified || isModified,
					title = kv.utils.getOriginalElementTitle(element);

				if (hasModification && !isValid) {
					return { title: obsv.error, 'data-orig-title': title };
				} else if (!hasModification || isValid) {
					return { title: title, 'data-orig-title': null };
				}
			});
		}
	};

	// ValidationOptions:
	// This binding handler allows you to override the initial config by setting any of the options for a specific element or context of elements
	//
	// Example:
	// <div data-bind="validationOptions: { insertMessages: true, messageTemplate: 'customTemplate', errorMessageClass: 'mySpecialClass'}">
	//      <input type="text" data-bind="value: someValue"/>
	//      <input type="text" data-bind="value: someValue2"/>
	// </div>
	ko.bindingHandlers['validationOptions'] = (function () {
		return {
			init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
				var options = unwrap(valueAccessor());
				if (options) {
					var newConfig = extend({}, kv.configuration);
					extend(newConfig, options);

					//store the validation options on the node so we can retrieve it later
					kv.utils.setDomData(element, newConfig);
				}
			}
		};
	}());
	;// Validation Extender:
	// This is for creating custom validation logic on the fly
	// Example:
	// var test = ko.observable('something').extend{(
	//      validation: {
	//          validator: function(val, someOtherVal){
	//              return true;
	//          },
	//          message: "Something must be really wrong!',
	//          params: true
	//      }
	//  )};
	ko.extenders['validation'] = function (observable, rules) { // allow single rule or array
		forEach(kv.utils.isArray(rules) ? rules : [rules], function (rule) {
			// the 'rule' being passed in here has no name to identify a core Rule,
			// so we add it as an anonymous rule
			// If the developer is wanting to use a core Rule, but use a different message see the 'addExtender' logic for examples
			kv.addAnonymousRule(observable, rule);
		});
		return observable;
	};

	//This is the extender that makes a Knockout Observable also 'Validatable'
	//examples include:
	// 1. var test = ko.observable('something').extend({validatable: true});
	// this will ensure that the Observable object is setup properly to respond to rules
	//
	// 2. test.extend({validatable: false});
	// this will remove the validation properties from the Observable object should you need to do that.
	ko.extenders['validatable'] = function (observable, options) {
		if (!kv.utils.isObject(options)) {
			options = { enable: options };
		}

		if (!('enable' in options)) {
			options.enable = true;
		}

		if (options.enable && !kv.utils.isValidatable(observable)) {
			var config = kv.configuration.validate || {};
			var validationOptions = {
				throttleEvaluation : options.throttle || config.throttle
			};

			observable.error = ko.observable(null); // holds the error message, we only need one since we stop processing validators when one is invalid

			// observable.rules:
			// ObservableArray of Rule Contexts, where a Rule Context is simply the name of a rule and the params to supply to it
			//
			// Rule Context = { rule: '<rule name>', params: '<passed in params>', message: '<Override of default Message>' }
			observable.rules = ko.observableArray(); //holds the rule Contexts to use as part of validation

			//in case async validation is occurring
			observable.isValidating = ko.observable(false);

			//the true holder of whether the observable is valid or not
			observable.__valid__ = ko.observable(true);

			observable.isModified = ko.observable(false);

			// a semi-protected observable
			observable.isValid = ko.computed(observable.__valid__);

			//manually set error state
			observable.setError = function (error) {
				var previousError = observable.error.peek();
				var previousIsValid = observable.__valid__.peek();

				observable.error(error);
				observable.__valid__(false);

				if (previousError !== error && !previousIsValid) {
					// if the observable was not valid before then isValid will not mutate,
					// hence causing any grouping to not display the latest error.
					observable.isValid.notifySubscribers();
				}
			};

			//manually clear error state
			observable.clearError = function () {
				observable.error(null);
				observable.__valid__(true);
				return observable;
			};

			//subscribe to changes in the observable
			var h_change = observable.subscribe(function () {
				observable.isModified(true);
			});

			// we use a computed here to ensure that anytime a dependency changes, the
			// validation logic evaluates
			var h_obsValidationTrigger = ko.computed(extend({
				read: function () {
					var obs = observable(),
						ruleContexts = observable.rules();

					kv.validateObservable(observable);

					return true;
				}
			}, validationOptions));

			extend(h_obsValidationTrigger, validationOptions);

			observable._disposeValidation = function () {
				//first dispose of the subscriptions
				observable.isValid.dispose();
				observable.rules.removeAll();
				h_change.dispose();
				h_obsValidationTrigger.dispose();

				delete observable['rules'];
				delete observable['error'];
				delete observable['isValid'];
				delete observable['isValidating'];
				delete observable['__valid__'];
				delete observable['isModified'];
	            delete observable['setError'];
	            delete observable['clearError'];
	            delete observable['_disposeValidation'];
			};
		} else if (options.enable === false && observable._disposeValidation) {
			observable._disposeValidation();
		}
		return observable;
	};

	function validateSync(observable, rule, ctx) {
		//Execute the validator and see if its valid
		if (!rule.validator(observable(), (ctx.params === undefined ? true : unwrap(ctx.params)))) { // default param is true, eg. required = true

			//not valid, so format the error message and stick it in the 'error' variable
			observable.setError(kv.formatMessage(
						ctx.message || rule.message,
						unwrap(ctx.params),
						observable));
			return false;
		} else {
			return true;
		}
	}

	function validateAsync(observable, rule, ctx) {
		observable.isValidating(true);

		var callBack = function (valObj) {
			var isValid = false,
				msg = '';

			if (!observable.__valid__()) {

				// since we're returning early, make sure we turn this off
				observable.isValidating(false);

				return; //if its already NOT valid, don't add to that
			}

			//we were handed back a complex object
			if (valObj['message']) {
				isValid = valObj.isValid;
				msg = valObj.message;
			} else {
				isValid = valObj;
			}

			if (!isValid) {
				//not valid, so format the error message and stick it in the 'error' variable
				observable.error(kv.formatMessage(
					msg || ctx.message || rule.message,
					unwrap(ctx.params),
					observable));
				observable.__valid__(isValid);
			}

			// tell it that we're done
			observable.isValidating(false);
		};

		//fire the validator and hand it the callback
		rule.validator(observable(), unwrap(ctx.params || true), callBack);
	}

	kv.validateObservable = function (observable) {
		var i = 0,
			rule, // the rule validator to execute
			ctx, // the current Rule Context for the loop
			ruleContexts = observable.rules(), //cache for iterator
			len = ruleContexts.length; //cache for iterator

		for (; i < len; i++) {

			//get the Rule Context info to give to the core Rule
			ctx = ruleContexts[i];

			// checks an 'onlyIf' condition
			if (ctx.condition && !ctx.condition()) {
				continue;
			}

			//get the core Rule to use for validation
			rule = ctx.rule ? kv.rules[ctx.rule] : ctx;

			if (rule['async'] || ctx['async']) {
				//run async validation
				validateAsync(observable, rule, ctx);

			} else {
				//run normal sync validation
				if (!validateSync(observable, rule, ctx)) {
					return false; //break out of the loop
				}
			}
		}
		//finally if we got this far, make the observable valid again!
		observable.clearError();
		return true;
	};
	;
	var _locales = {};
	var _currentLocale;

	kv.defineLocale = function(name, values) {
		if (name && values) {
			_locales[name.toLowerCase()] = values;
			return values;
		}
		return null;
	};

	kv.locale = function(name) {
		if (name) {
			name = name.toLowerCase();

			if (_locales.hasOwnProperty(name)) {
				kv.localize(_locales[name]);
				_currentLocale = name;
			}
			else {
				throw new Error('Localization ' + name + ' has not been loaded.');
			}
		}
		return _currentLocale;
	};

	//quick function to override rule messages
	kv.localize = function (msgTranslations) {
		var rules = kv.rules;

		//loop the properties in the object and assign the msg to the rule
		for (var ruleName in msgTranslations) {
			if (rules.hasOwnProperty(ruleName)) {
				rules[ruleName].message = msgTranslations[ruleName];
			}
		}
	};

	// Populate default locale (this will make en-US.js somewhat redundant)
	(function() {
		var localeData = {};
		var rules = kv.rules;

		for (var ruleName in rules) {
			if (rules.hasOwnProperty(ruleName)) {
				localeData[ruleName] = rules[ruleName].message;
			}
		}
		kv.defineLocale('en-us', localeData);
	})();

	// No need to invoke locale because the messages are already defined along with the rules for en-US
	_currentLocale = 'en-us';
	;/**
	 * Possible invocations:
	 * 		applyBindingsWithValidation(viewModel)
	 * 		applyBindingsWithValidation(viewModel, options)
	 * 		applyBindingsWithValidation(viewModel, rootNode)
	 *		applyBindingsWithValidation(viewModel, rootNode, options)
	 */
	ko.applyBindingsWithValidation = function (viewModel, rootNode, options) {
		var node = document.body,
			config;

		if (rootNode && rootNode.nodeType) {
			node = rootNode;
			config = options;
		}
		else {
			config = rootNode;
		}

		kv.init();

		if (config) {
			config = extend(extend({}, kv.configuration), config);
			kv.utils.setDomData(node, config);
		}

		ko.applyBindings(viewModel, node);
	};

	//override the original applyBindings so that we can ensure all new rules and what not are correctly registered
	var origApplyBindings = ko.applyBindings;
	ko.applyBindings = function (viewModel, rootNode) {

		kv.init();

		origApplyBindings(viewModel, rootNode);
	};

	ko.validatedObservable = function (initialValue, options) {
		if (!options && !kv.utils.isObject(initialValue)) {
			return ko.observable(initialValue).extend({ validatable: true });
		}

		var obsv = ko.observable(initialValue);
		obsv.errors = kv.group(kv.utils.isObject(initialValue) ? initialValue : {}, options);
		obsv.isValid = ko.observable(obsv.errors().length === 0);

		if (ko.isObservable(obsv.errors)) {
			obsv.errors.subscribe(function(errors) {
				obsv.isValid(errors.length === 0);
			});
		}
		else {
			ko.computed(obsv.errors).subscribe(function (errors) {
				obsv.isValid(errors.length === 0);
			});
		}

		obsv.subscribe(function(newValue) {
			if (!kv.utils.isObject(newValue)) {
				/*
				 * The validation group works on objects.
				 * Since the new value is a primitive (scalar, null or undefined) we need
				 * to create an empty object to pass along.
				 */
				newValue = {};
			}
			// Force the group to refresh
			obsv.errors._updateState(newValue);
			obsv.isValid(obsv.errors().length === 0);
		});

		return obsv;
	};
	;}));


/***/ }),
/* 35 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;var rvc = __webpack_require__(8);
	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(22)], __WEBPACK_AMD_DEFINE_RESULT__ = function () {
	    ko.extenders.lotKey = function (target, options) {
	        var matchCallback, changedCallback;

	        if (typeof options === "function") {
	            matchCallback = options;
	            changedCallback = null;
	        } else if(options != undefined && typeof options === "object") {
	            matchCallback = options.matchCallback;
	            changedCallback = typeof options.changedCallback === "function" ? options.changedCallback : undefined;
	        }

		    var pattern = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)(\d{2}\d*)?$/,
		        newPattern = /^([01])?([0-9]{1})\s{0,}(\d{1,2})?\s{0,}(\d{1,3})?\s{0,}(\d{1,})?(.*)$/,
	            completePattern = /^(\d{2})(\s)(\d{2})(\s)(\d{3})(\s)(\d{2}\d*)$/,
	            isComplete = ko.observable(false);

		    target.formattedLot = ko.pureComputed({
				read: function () {
					return target();
				},
				write: function (value) {
					value = cleanInput(value);
					if (target.peek() === value) return;

					var formatted = formatAsLot(value);
					target(formatted);
				    if (formatted && formatted.match(completePattern)) {
				        isComplete(true);
				        if (typeof matchCallback === "function") matchCallback(formatted);
				        if (typeof changedCallback === "function") changedCallback(value, formatted);
				    } else changedCallback && changedCallback(value, undefined); // the second argument of the changedCallback is only returned when the lot key is complete

				}
			});
	        
			function cleanInput(input) {
				if (typeof input == "number") input = input.toString();
				if (typeof input !== "string") return undefined;
				return input.trim();
			}
			function formatAsLot(input) {
	            var re = /\d+/g,
	            newInput = input.match(re);

				if (newInput === undefined || newInput === null) { return; }
				newInput = newInput.join('');

				return newInput.replace(newPattern, function(match, p1, p2, p3, p4, p5, p6) {
				    if (p1) {
				        return [String(p1) + p2, p3, p4, p5].join(' ');
	                } else if (!p1 && (p2 === "0" || p2 === "1")) {
	                    return [p2, p3, p4, p5].join(' ');
	                } else {
	                    return [String(0) + p2, p3, p4, p5].join(' ');
	                }
	            }).trim();
			}

			target.match = function (valueToCompare) {
				var partialPattern = new RegExp('^' + target.formattedLot() + '$');
				return valueToCompare.match(partialPattern);
			};
			target.isComplete = ko.pureComputed(function () {
				return isComplete();
			}, target);
			target.Date = ko.pureComputed(function () {
			    var formattedLot = this.formattedLot();
				if (formattedLot) {
					var sections = formattedLot.split(" ");
					var days = parseInt(sections[2]);
					var defDate = "1/1/" + (parseInt(sections[1]) >= 90 ? "19" : "20");
					var date = new Date(defDate + sections[1]).addDays(days - 1);
					date.addMinutes(date.getTimezoneOffset());

					return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
				}
			}, target);
			target.formattedDate = ko.pureComputed(function () {
				var date = this();
				if (date && date != 'Invalid Date') return date.format("UTC:m/d/yyyy");
				return '';
			}, target.Date);
			target.LotType = ko.pureComputed(function () {
			  var lot = target.formattedLot();
			  if( lot ) {
			    var sections = lot.split(" ");
			    return Number(sections[0]);
			  }
			});
			target.InventoryTypeKey = ko.pureComputed(function () {
			  var lotType = target.LotType();
	      switch (lotType) {
	        case 1:
	        case 2:
	        case 3:
	        case 11:
	        case 12:
	          return rvc.lists.inventoryTypes.Chile.key;
	        case 4:
	          return rvc.lists.inventoryTypes.Additive.key;
	        case 5:
	          return rvc.lists.inventoryTypes.Packaging.key;
	        default:
	          return null;
			  }
			});
			target.Sequence = ko.pureComputed({
				read: function () {
					if (this.formattedLot()) {
						var sections = this.formattedLot().split(" ");
						if (sections.length === 4)
							return sections[3];
					}
				},
				write: function (newSeq) {
					var val = '';
					if (isComplete()) {
						var reg = /^(0)?([1-9])(\s?)(\d{2})?(\s)?(\d{3})?(\s?)/;
						val = this.formattedLot().match(reg)[0];
						val += newSeq < 10 ? '0' : '';
						val += newSeq;
						this.formattedLot(val);
					}
				}
			}, target);
			target.getNextLot = function () {
				var sequence = parseInt(target.Sequence());
				sequence++;
				if (sequence < 10) sequence = '0' + sequence;
				return target.formattedLot().replace(pattern, '0$2 $4 $6 ' + sequence);
			};

			target.extend({ throttle: 800 });

			target.formattedLot(target.peek());
			return target;
		};
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 36 */
/***/ (function(module, exports, __webpack_require__) {

	(function (ko) {
	    ko.validation.init({
	        insertMessages: false,
	        decorateInputElement: true,
	        errorElementClass: 'has-error',
	        errorMessageClass: 'help-block'
	    });
	}(__webpack_require__(9)));

/***/ }),
/* 37 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {__webpack_require__(37);

	!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(38), __webpack_require__(39), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function (tableHeaderClickHelper, propertyGetter, ko) {
	    var sortOption = {};

	    ko.bindingHandlers.sortableTable = {
	        init: function (element, valueAccessor, allBindingsAccessor) {
	            var allBindings = allBindingsAccessor();
	            allBindings.enableClick = canBeSorted;

	            ko.bindingHandlers.clickableTableHeaders.init(
	                element,
	                ko.utils.wrapAccessor(sort),
	                ko.utils.wrapAccessor(allBindings));

	            function sort(th) {
	                $(element).find('thead .' + ko.bindingHandlers.sortableTable.options.sortedCssClass)
	                    .removeClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
	                        ko.bindingHandlers.sortableTable.options.sortedAscCssClass + ' ' +
	                        ko.bindingHandlers.sortableTable.options.sortedDescCssClass);

	                sortData(th, valueAccessor());
	            }
	        }
	    }

	    ko.bindingHandlers.sortableTable.options = {
	        sortedCssClass: 'sorted',
	        sortedAscCssClass: 'asc',
	        sortedDescCssClass: 'desc'
	    }

	    function sortData(thElement, data) {
	        var $th = $(thElement);
	        var sort = $th.attr('data-sort');
	        if (!sort) return;

	        var previousSort = sortOption;
	        var sortDirection = previousSort && previousSort.propertyName === sort
	            ? previousSort.direction * -1
	            : 1;

	        sortOption = {
	            propertyName: sort,
	            direction: sortDirection
	        };

	        // todo: get context for table's tbody in order to prevent duplicate declaration source property as valueAccessor for the body and clickable header

	        var sortFn = dynamicSortFn(sort, sortDirection);
	        if (ko.isObservable(data)) {
	          var dataCache = data();

	            data(dataCache.sort(sortFn));
	        }

	        $th.addClass(ko.bindingHandlers.sortableTable.options.sortedCssClass + ' ' +
	            (sortDirection > 0
	                ? ko.bindingHandlers.sortableTable.options.sortedAscCssClass
	                : ko.bindingHandlers.sortableTable.options.sortedDescCssClass));
	    };

	    function dynamicSortFn( sort, direction ) {
	        if ( !sort ) { return null; }

	        // 1 = Ascending, -1 = Descending
	        if (direction != -1) { direction = 1; }

	        var lt = -1 * direction,
	            gt = 1 * direction;

	        function sortFn( a, b ) {
	            var _a = ko.utils.unwrapObservable( propertyGetter.getValue( a, sort ) );
	            var _b = ko.utils.unwrapObservable( propertyGetter.getValue( b, sort ) );

	            if ( _a < _b || _a == null ) {
	              return lt;
	            } else if ( _a > _b ) {
	              return gt;
	            }

	            return 0;
	        }

	        return sortFn;
	    }

	    function canBeSorted(element) {
	        var $th = $(element);
	        var sort = $th.attr('data-sort');
	        return sort && true;
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__))

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 38 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function($, ko) {
	    ko.bindingHandlers.clickableTableHeaders = {
	        init: function (element, valueAccessor, allBindings) {
	            setTimeout(function () { // allow templated bindings to be rendered
	                setupHandlers(element, valueAccessor, allBindings);
	            }, 0);
	        }
	    }

	    function setupHandlers(element, valueAccessor, allBindings) {
	        setupTableClickElements(element, valueAccessor, allBindings);
	        setupRebindTrigger(element, valueAccessor, allBindings);
	    }

	    var defaultEnableClick = function () { return true; };

	    function setupTableClickElements(element, valueAccessor, allBindings) {
	        var options = allBindings() || {};
	        var enableClick = options.enableClick || defaultEnableClick;
	        var $table = $(element);

	        $table.find('thead th').each(function (index, thElem) {
	            thElem.clickEnabled = enableClick(thElem);
	            
	            if (thElem.clickEnabled) {
	                var $th = $(thElem);
	                $th.css({
	                    cursor: 'pointer'
	                });
	            }

	            thElem = null;
	            $th = null;
	        });

	        var $thead = $table.find('thead');
	        $thead.off('click');
	        $thead.click(function (args) {
	            if (args.target.nodeType !== 1 || args.target.nodeName !== 'TH') return;
	            if (args.target.clickEnabled) valueAccessor()(args.target);
	            args.stopPropagation();
	        });

	        ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
	            $thead.off('click');
	            $thead = null;
	            $table = null;
	        });
	    }
	    function setupRebindTrigger(element, valueAccessor, allBindings) {
	        var options = allBindings() || {};
	        if (options.rebindTrigger) {
	            if (!ko.isObservable(options.rebindTrigger)) throw new Error("The \"rebindTrigger\" binding option is invalid. Expected observable.");
	            options.rebindTrigger.subscribe(function () {
	                setupTableClickElements(element, valueAccessor, allBindings);
	                //todo: clean up old bindings?
	            });
	        }
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

/***/ }),
/* 39 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_RESULT__ = function() {
	    var arrayIndexRegex = /^(\w+)?\[(\d+)\]\.?(.*)/;

	    //Object.prototype.deepSearch = function(propertyName) {
	    //    return getValueFromPropertyName(this, propertyName);
	    //}

	    return {
	        getValue: getValueFromPropertyName
	    }

	    function getValueFromPropertyName(obj, propName) {
	        if (obj == undefined) return undefined;
	        var matches = arrayIndexRegex.exec(propName);
	        if (matches && matches.length) {
	            var index = parseInt(matches[2]);
	            obj = matches[1]
	                ? obj[matches[1]][index]
	                : obj[index];

	            return matches[3]
	                ? getValueFromPropertyName(obj, matches[3])
	                : obj;
	        }

	        var paths = propName.split('.');
	        if (paths.length > 1) {
	            var p = paths.shift();
	            return getValueFromPropertyName(obj[p], paths.join('.'));
	        }

	        return obj[propName];
	    }
	}.call(exports, __webpack_require__, exports, module), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__))


/***/ }),
/* 40 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(16), __webpack_require__(1)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, directoryService, $) {
	    var service = {
	        getContractSummariesDataPager: getContractSummariesDataPager,
	        getContractDetails: getContractDetails,
	        getSalesOrders: getSalesOrders,
	        getShipmentMethods: getShipmentMethods,
	        getSalesOrdersDataPager: getSalesOrdersDataPager,
	        getSalesOrder: getSalesOrder,
	        createSalesOrder: createSalesOrder,
	        updateSalesOrder: updateSalesOrder,
	        deleteSalesOrder: deleteSalesOrder,
	        postSalesInvoice: postSalesInvoice,
	        postSalesOrder: postSalesOrder,
	        getShipmentSummaryForContract: getShipmentSummaryForContract,
	        getPaymentTermOptions: getPaymentTermOptions,
	        getWarehouses: getWarehouses,
	        createContract: createContract,
	        deleteContract: deleteContract,
	        updateContract: updateContract,
	        getContractsForCustomer: getContractsForCustomer,
	        getQuotesDataPager: getQuotesDataPager,
	        getQuoteDetails: getQuoteDetails,
	        createQuote: createQuote,
	        updateQuote: updateQuote
	    };

	    var BASE_URLS = {
	      QUOTES: '/api/quotes/'
	    };

	    return $.extend({}, service, directoryService);

	    function getContractSummariesDataPager(args) {
	        var options = args || {};
	        return core.pagedDataHelper.init({
	            urlBase: "/api/contracts",
	            pageSize: options.pageSize || 100,
	            parameters: options.parameters,
	            onNewPageSet: options.onNewPageSet,
	            resultCounter: function(response) {
	                return response.Data.length;
	            }
	        });
	    }
	    function getSalesOrdersDataPager(options) {
	        var opts = options || {};
	        return core.pagedDataHelper.init({
	            urlBase: "/api/salesorders",
	            pageSize: opts.pageSize || 100,
	            parameters: opts.parameters,
	            onNewPageSet: opts.onNewPageSet,
	            resultCounter: function(response) {
	                return response.length;
	            }
	        });
	    }
	    function getContractsForCustomer(key, maxRecords) {
	        return core.ajax("/api/customers/" + key + "/contracts?take=" + maxRecords);
	    }
	    function getContractDetails(key) {
	        return core.ajax(buildContractRoute(key));
	    }
	    function getShipmentSummaryForContract(contractKey) {
	        return core.ajax(buildContractRoute(contractKey) + '/shipments');
	    }
	    function getShipmentMethods() {
	      return core.ajax('/api/shipmentMethods');
	    }
	    function getPaymentTermOptions() {
	        return core.ajax("/api/paymentterms");
	    }
	    function getWarehouses() {
	        return core.ajax("/api/facilities");
	    }
	    function getSalesOrders(filters) {
	      if (filters) {
	        return core.ajax("/api/salesorders/?" + filters);
	      } else {
	        return core.ajax("/api/salesorders/");
	      }
	    }
	    function getSalesOrder(key) {
	        return core.ajax("/api/salesorders/" + key);
	    }
	    function createSalesOrder( data ) {
	      return core.ajaxPost('/api/salesorders/', data);
	    }
	    function updateSalesOrder( key, data ) {
	      return core.ajaxPut('/api/salesorders/' + key, data);
	    }
	    function deleteSalesOrder( key ) {
	      return core.ajaxDelete('/api/salesorders/' + key);
	    }
	    function postSalesOrder( key, data ) {
	      return core.ajaxPost('/api/InventoryShipmentOrders/' + key + '/PostAndClose', data);
	    }
	    function postSalesInvoice( key ) {
	      return core.ajaxPost('/api/salesorders/' + key + '/postinvoice');
	    }
	    function createContract(values) {
	        return core.ajaxPost(buildContractRoute(), values);
	    }
	    function deleteContract(key) {
	        return core.ajaxDelete(buildContractRoute(key));
	    }
	    function updateContract(contractKey, values) {
	        return core.ajaxPut(buildContractRoute(contractKey), values);
	    }

	    function buildContractRoute(key) {
	        return "/api/contracts/" + (key || "");
	    }

	    function getQuotesDataPager( options ) {
	      var opts = options || {};

	      return core.pagedDataHelper.init({
	        urlBase: BASE_URLS.QUOTES,
	        pageSize: opts.pageSize || 20,
	        parameters: opts.parameters,
	        onNewPageSet: opts.onNewPageSet,
	        resultCounter: function(response) {
	          return response.length;
	        }
	      });
	    }

	    function getQuoteDetails( quoteKey ) {
	      if ( quoteKey == null ) { throw new Error('Quote Details fetching requires a key'); }

	      return core.ajax( BASE_URLS.QUOTES + quoteKey );
	    }

	    function createQuote( quoteData ) {
	      return core.ajaxPost( BASE_URLS.QUOTES, quoteData );
	    }

	    function updateQuote( quoteKey, quoteData ) {
	      return core.ajaxPut( BASE_URLS.QUOTES + quoteKey, quoteData );
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 41 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(42), __webpack_require__(40), __webpack_require__(8), __webpack_require__(9),
	    __webpack_require__(43), __webpack_require__(44), __webpack_require__(45), __webpack_require__(31)], __WEBPACK_AMD_DEFINE_RESULT__ = function (templateMarkup, salesService, app, ko) {

	        //#region model definitions

	        function CustomerContractSummary(input) {
	            var values = input || {};
	            var currencyFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });

	            this.CustomerContractKey = values.CustomerContractKey;
	            this.ContractNumber = values.ContractNumber;
	            this.ContractDate = new Date(values.ContractDate);
	            this.TermBegin = values.TermBegin && new Date(values.TermBegin).toString('M/d/yyyy');
	            this.TermEnd = values.TermEnd && new Date(values.TermEnd).toString('M/d/yyyy');
	            this.CustomerPurchaseOrder = values.CustomerPurchaseOrder;
	            this.ContractType = app.lists.contractTypes.findByKey(values.ContractType || 0).value;
	            this.ContractStatus = app.lists.contractStatuses.findByKey(values.ContractStatus || 0).value;
	            this.CustomerKey = values.CustomerKey;
	            this.CustomerName = values.CustomerName;
	            this.ContactName = values.ContactName;
	            this.BrokerCompanyKey = values.BrokerCompanyKey;
	            this.BrokerCompanyName = values.BrokerCompanyName;
	            this.DistributionWarehouseKey = values.DistributionWarehouseKey;
	            this.DistributionWarehouseName = values.DistributionWarehouseName;
	            this.AverageBasePrice = currencyFormat.format(values.AverageBasePrice || 0);
	            this.AverageTotalPrice = currencyFormat.format(values.AverageTotalPrice || 0);
	            this.SumQuantity = values.SumQuantity || 0;
	            this.SumWeight = values.SumWeight || 0;
	        }

	        CustomerContractSummary.prototype.defaultValues = {
	            AverageBasePrice: 0,
	            AverageTotalPrice: 0,
	            SumQuantity: 0,
	            SumWeight: 0,
	        };

	        function CustomerContracts(params) {

	            this.customerFilter = ko.isObservable(params.customerFilter) ?
	              params.customerFilter :
	              ko.observable(params.customerFilter);

	            var vm = this,
	                contractsDataPager = salesService.getContractSummariesDataPager({
	                    parameters: { customerKey: vm.customerFilter },
	                    onNewPageSet: function() {
	                        vm.customerContracts([]);
	                    }
	                });


	            this.contractKeyFilter = ko.isObservable(params.contractKeyFilter) ?
	              params.contractKeyFilter :
	              ko.observable(params.contractKeyFilter);

	            this.customerContracts = ko.observableArray([]);
	            this.totalPages = ko.observable();
	            this.currentPageNumber = ko.observable();

	            // commands
	            this.loadCustomerContractsCommand = ko.asyncCommand({
	                execute: function (complete) {
	                    contractsDataPager.nextPage()
	                        .then(loadCustomerContractsSuccess)
	                        .fail(loadCustomerContractsFailure)
	                        .always(complete);
	                },
	                canExecute: function(isExecuting) {
	                    return !isExecuting;
	                }
	            }, this);

	            this.loadCustomerContractsCommand.execute();


	            function loadCustomerContractsSuccess(response) {
	                ko.utils.arrayPushAll(vm.customerContracts(), ko.utils.arrayMap(response.Data, mapContractSummary));
	                vm.customerContracts.notifySubscribers(vm.customerContracts());
	                vm.totalPages(response.TotalPageCount);
	                vm.currentPageNumber(response.CurrentPage);
	            }
	            function loadCustomerContractsFailure(xhr, status, message)
	            {
	                showUserMessage("Failed to load customer contracts", { description: message });
	            }
	        }

	        CustomerContracts.prototype.findContractSummaryItemByKey = function (key) {
	            if (!key) return null;
	            return ko.utils.arrayFirst(this.customerContracts(), function(c) {
	                return c.CustomerContractKey === key;
	            });
	        };
	        CustomerContracts.prototype.addContractSummaryItem = function (values) {
	            if (!values) return null;
	            var newItem = values instanceof CustomerContractSummary ? values : new CustomerContractSummary(ko.toJS(values));
	            this.customerContracts.unshift(newItem);
	            return newItem;
	        };
	        CustomerContracts.prototype.updateContractSummaryItem = function (key, newValues) {
	            var contract = this.findContractSummaryItemByKey(key);
	            var newContract = new CustomerContractSummary(ko.toJS(newValues));
	            var index = ko.utils.arrayIndexOf(this.customerContracts(), contract);
	            if (index > -1) {
	                this.customerContracts.splice(index, 1, newContract);
	                return newContract;
	            } else return this.addContractSummaryItem(newContract);
	        };

	        //#endregion
	        
	        return { viewModel: CustomerContracts, template: templateMarkup };

	        //#region private methods
	        function mapContractSummary(values) {
	            return new CustomerContractSummary(values);
	        }
	        //#endregion
	    }.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 42 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"floatThead-wrap-height\">\r\n  <div class=\"table-responsive\">\r\n    <table class=\"reset table table-striped table-condensed table-bordered\" data-bind=\"sortableTable: customerContracts, floatThead: customerContracts\">\r\n      <thead>\r\n        <tr>\r\n          <th data-sort=\"CustomerContractKey\">ID</th>\r\n          <th data-sort=\"ContractDate\">Date</th>\r\n          <th data-sort=\"CustomerName\">Customer</th>\r\n          <th data-sort=\"ContactName\">Contact</th>\r\n          <th data-sort=\"TermBegin\">Term Start</th>\r\n          <th data-sort=\"TermEnd\">Term End</th>\r\n          <th data-sort=\"CustomerPurchaseOrder\">PO Number</th>\r\n          <th data-sort=\"ContractType\">Contract Type</th>\r\n          <th data-sort=\"ContractStatus\">Status</th>\r\n          <th data-sort=\"BrokerCompanyName\">Broker</th>\r\n          <th data-sort=\"DistributionWarehouseName\">Warehouse</th>\r\n          <th data-sort=\"AverageBasePrice\" class=\"calc numeric\">Avg Base Price</th>\r\n          <th data-sort=\"AverageTotalPrice\" class=\"calc numeric\">Avg Total Price</th>\r\n          <th data-sort=\"SumQuantity\" class=\"calc numeric\">Total Qty</th>\r\n          <th data-sort=\"SumWeight\" class=\"calc numeric\">Total Weight</th>\r\n        </tr>\r\n      </thead>\r\n\r\n      <tbody data-bind=\"foreach: customerContracts\">\r\n        <tr>\r\n          <td data-bind=\"text: CustomerContractKey\" class=\"no-wrap\"></td>\r\n          <td data-bind=\"text: ContractDate.toString('M/d/yyyy')\"></td>\r\n          <td class=\"truncate\" data-bind=\"text: CustomerName\"></td>\r\n          <td class=\"truncate no-wrap\" data-bind=\"text: ContactName\"></td>\r\n          <td data-bind=\"text: TermBegin\"></td>\r\n          <td data-bind=\"text: TermEnd\"></td>\r\n          <td data-bind=\"text: CustomerPurchaseOrder\"></td>\r\n          <td data-bind=\"text: ContractType\"></td>\r\n          <td data-bind=\"text: ContractStatus\"></td>\r\n          <td class=\"truncate\" data-bind=\"text: BrokerCompanyName\"></td>\r\n          <td data-bind=\"text: DistributionWarehouseName\"></td>\r\n          <td data-bind=\"text: AverageBasePrice\" class=\"calc numeric\"></td>\r\n          <td data-bind=\"text: AverageTotalPrice\" class=\"calc numeric\"></td>\r\n          <td data-bind=\"text: SumQuantity.toLocaleString()\" class=\"calc numeric\"></td>\r\n          <td data-bind=\"text: SumWeight.toLocaleString()\" class=\"calc numeric\"></td>\r\n        </tr>\r\n      </tbody>\r\n    </table>\r\n  </div>\r\n</div>\r\n"

/***/ }),
/* 43 */,
/* 44 */,
/* 45 */,
/* 46 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(54), __webpack_require__(40), __webpack_require__(24), __webpack_require__(55), __webpack_require__(8), __webpack_require__(9), __webpack_require__(8), __webpack_require__(47), __webpack_require__(57), __webpack_require__(59)], __WEBPACK_AMD_DEFINE_RESULT__ = function (templateMarkup, salesService, productsService, helpers, app, ko, rvc, notebookControlComponent) {

	      __webpack_require__(36);
	        var currencyFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });
	        var avgCostFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 3 });

	        //#region model definitions

	        function Address(input) {
	            if (!(this instanceof Address)) return new Address(input);
	            var values = input || {};

	            this.AddressLine1 = ko.observable(values.AddressLine1 || "");
	            this.AddressLine2 = ko.observable(values.AddressLine2 || "");
	            this.AddressLine3 = ko.observable(values.AddressLine3 || "");
	            this.City = ko.observable(values.City || "");
	            this.State = ko.observable(values.State || "");
	            this.PostalCode = ko.observable(values.PostalCode || "");
	            this.Country = ko.observable(values.Country || "");
	        }
	        // update values
	        Address.prototype.update = function (val) {
	            ['AddressLine1', 'AddressLine2', 'AddressLine3', 'City', 'State', 'PostalCode', 'Country']
	            .forEach(function (prop) {
	                if (prop in val)
	                    this[prop](val[prop]);
	            }, this);
	        };

	        Address.prototype.buildDtoObject = function() {
	            return ko.toJS(this);
	        };

	        function CustomerContract(input) {
	            if (!(this instanceof CustomerContract)) return new CustomerContract(input);
	            var values = input || {};
	            var _customer = ko.observable(),
	                _broker = ko.observable();

	            var vm = this;

	            this.CustomerContractKey = values.CustomerContractKey;
	            this.ContractNumber = values.ContractNumber;
	            this.ContractDate = ko.observableDate(values.ContractDate || Date.now());
	            this.TermBegin = ko.observableDate(values.TermBegin);
	            this.TermEnd = ko.observableDate(values.TermEnd);
	            this.CustomerPurchaseOrder = ko.observable(values.CustomerPurchaseOrder);
	            this.ContractType = ko.observable(values.ContractType + '').extend({ contractType: true });
	            this.ContractStatus = ko.observable(values.ContractStatus + '').extend({ contractStatus: true });
	            this.CustomerKey = ko.observable(values.CustomerKey);
	            this.CustomerName = ko.observable(values.CustomerName);
	            this.ContactName = ko.observable(values.ContactName);
	            this.BrokerCompanyKey = ko.observable(values.BrokerCompanyKey);
	            this.BrokerCompanyName = ko.observable(values.BrokerCompanyName);
	            this.PaymentTerms = ko.observable(values.PaymentTerms);
	            this.FOB = ko.observable(values.FOB);
	            this.NotesToPrint = ko.observable(values.NotesToPrint);
	            this.CommentsNotebookKey = values.CommentsNotebookKey;

	            this.DistributionWarehouseKey = values.DistributionWarehouseKey;
	            this.DistributionWarehouseName = values.DistributionWarehouseName;
	            this.ContactAddress = new Address(values.ContactAddress);
	            this.ContractItems = ko.observableArray(ko.utils.arrayMap(values.ContractItems, ContractLineItem));

	            values.Links = values.Links || {};
	            this.contractDrawSummaryReportUrl = values.Links["contract-draw-summary-report"] && values.Links["contract-draw-summary-report"].HRef;
	            this.contractReportUrl = values.Links["contract-report"] && values.Links["contract-report"].HRef;

	            this.customer = ko.computed({
	                read: function () {
	                  return _customer();
	                },
	                write: function (value) {
	                    if (value === undefined) {
	                        _customer(null);
	                        return;
	                    }
	                    vm.CustomerKey((value && value.CompanyKey) || null);
	                    vm.CustomerName((value && value.Name) || null);
	                    _customer(value);
	                },
	                owner: this
	            });

	            this.broker = ko.computed({
	                read: _broker,
	                write: function(value) {
	                    if (value === undefined) {
	                        _broker(null);
	                        return;
	                    }

	                    vm.BrokerCompanyKey((value && value.CompanyKey) || null);
	                    vm.BrokerCompanyName((value && value.Name) || null);
	                    _broker(value);
	                }
	            });
	            this.totalQuantityOnContract = ko.pureComputed(function () {
	                var sum = 0;
	                ko.utils.arrayMap(this.ContractItems(), function(i) {
	                    sum += i.Quantity() || 0;
	                });
	                return sum;
	            }, this);
	            this.totalPoundsOnContract = ko.pureComputed(function () {
	                var sum = 0;
	                ko.utils.arrayMap(this.ContractItems(), function (i) {
	                    sum += i.totalWeight() || 0;
	                });
	                return sum;
	            }, this);
	            this.averageBasePricePerPound = ko.pureComputed(function () {
	                var sum = 0, items = this.ContractItems();
	                ko.utils.arrayMap(items, function (i) {
	                    sum += (i.PriceBase() || 0) * (i.totalWeight() || 0);
	                });
	                var totalPounds = this.totalPoundsOnContract();
	                return totalPounds ? sum / totalPounds : 0;
	            }, this);
	            this.averageBasePricePerPound.formattedValue = ko.pureComputed(function () {
	                return avgCostFormat.format(this());
	            }, this.averageBasePricePerPound);
	            this.averageNetPricePerPound = ko.pureComputed(function () {
	                var sum = 0, items = this.ContractItems();
	                ko.utils.arrayMap(items, function (i) {
	                    sum += (i.totalPricePerPound() || 0) * (i.totalWeight() || 0);
	                });
	                var totalPounds = this.totalPoundsOnContract();
	                return totalPounds ? sum / totalPounds : 0;
	            }, this);
	            this.averageNetPricePerPound.formattedValue = ko.pureComputed(function () {
	                return avgCostFormat.format(this());
	            }, this.averageNetPricePerPound);
	            this.totalValueOfContract = ko.pureComputed(function () {
	                var sum = 0;
	                ko.utils.arrayMap(this.ContractItems(), function (i) {
	                    sum += i.totalValue() || 0;
	                });
	                return sum;
	            }, this);
	            this.totalValueOfContract.formattedValue = ko.pureComputed(function() {
	                return currencyFormat.format(this());
	            }, this.totalValueOfContract);

	            this.addContractItemCommand = ko.command({
	                execute: function() {
	                    vm.ContractItems.push(new ContractLineItem());
	                },
	            });
	            this.removeContractItemCommand = ko.command({
	                execute: function(item) {
	                    if (!(item instanceof ContractLineItem)) throw new Error("Invalid arguement. Expected ContractLineItem instance.");
	                    ko.utils.arrayRemoveItem(vm.ContractItems(), item);
	                    vm.ContractItems.notifySubscribers(vm.ContractItems());
	                }
	            });

	            this.isNew = !values.CustomerContractKey;

	            this.validation = ko.validatedObservable({
	              date: vm.ContractDate.extend({ required: true }),
	              customer: vm.customer.extend({ required: true })
	            });
	        }

	        CustomerContract.prototype.updateContractAsync = function() {

	        };
	        CustomerContract.prototype.adjustBasePriceOfItems = function (adjustment, filter) {
	            var adjValue = parseFloat(adjustment);
	            if (isNaN(adjValue)) throw new Error("Invalid argument \"adjustment\". Expected numeric value.");

	            var items = typeof filter === "function" ?
	              ko.utils.arrayFilter(this.ContractItems.peek(), filter) :
	              this.ContractItems.peek();

	            items.forEach(function (item) { item.PriceBase((parseFloat(item.PriceBase()) || 0) + adjValue); });
	        };
	        CustomerContract.prototype.buildDtoObject = function() {
	            return {
	                ContractDate: this.ContractDate(),
	                TermBegin: this.TermBegin(),
	                TermEnd: this.TermEnd(),
	                CustomerKey: this.CustomerKey(),
	                ContractType: this.ContractType(),
	                ContractStatus: this.ContractStatus(),
	                ContactName: this.ContactName(),
	                BrokerKey: this.BrokerCompanyKey(),
	                ContactAddress: this.ContactAddress.buildDtoObject(),
	                FOB: this.FOB(),
	                DistributionWarehouseKey: this.DistributionWarehouseKey,
	                PaymentTerms: this.PaymentTerms(),
	                CustomerPurchaseOrder: this.CustomerPurchaseOrder(),
	                NotesToPrint: this.NotesToPrint(),
	                ContractItems: ko.utils.arrayMap(this.ContractItems(), function(item) { return item.buildDtoObject(); }),
	            };
	        };
	        CustomerContract.prototype.toContractSummaryItem = function() {
	            return ko.toJS({
	                CustomerContractKey: this.CustomerContractKey,
	                ContractNumber: this.ContractNumber,
	                ContractDate: this.ContractDate,
	                TermBegin: this.TermBegin,
	                TermEnd: this.TermEnd,
	                CustomerPurchaseOrder: this.CustomerPurchaseOrder,
	                ContractType: this.ContractType,
	                ContractStatus: this.ContractStatus,
	                CustomerKey: this.CustomerKey,
	                CustomerName: this.CustomerName,
	                ContactName: this.ContactName,
	                BrokerCompanyKey: this.BrokerCompanyKey,
	                BrokerCompanyName: this.BrokerCompanyName,
	                DistributionWarehouseKey: this.DistributionWarehouseKey,
	                DistributionWarehouseName: this.DistributionWarehouseName,
	                AverageBasePrice: this.averageBasePricePerPound,
	                AverageTotalPrice: this.averageNetPricePerPound,
	                SumQuantity: this.totalQuantityOnContract,
	                SumWeight: this.totalPoundsOnContract,
	            });
	        };
	        CustomerContract.prototype.createNewFromCopy = function() {
	            var copy = new CustomerContract(ko.toJS(this));
	            copy.copiedFrom = this.CustomerContractKey;
	            copy.CustomerContractKey = null;
	            copy.ContractNumber = null;
	            copy.CustomerPurchaseOrder(null);
	            copy.ContractStatus(app.lists.contractStatuses.Pending.key);
	            //copy.NotesToPrint(null);
	            copy.CommentsNotebookKey = null;
	            copy.ContractDate(Date.now());

	            var termBegin = copy.TermBegin();
	            if (termBegin) {
	                if (!(termBegin instanceof Date)) termBegin = new Date(termBegin);
	                copy.TermBegin(new Date(termBegin.getFullYear() + 1, termBegin.getMonth(), termBegin.getDate()));
	            }

	            var termEnd = copy.TermEnd();
	            if (termEnd) {
	                if (!(termEnd instanceof Date)) termEnd = new Date(termEnd);
	                copy.TermEnd(new Date(termEnd.getFullYear() + 1, termEnd.getMonth(), termEnd.getDate()));
	            }

	            copy.isNew = true;

	            return copy;
	        };

	        function ContractLineItem(input) {
	            if (!(this instanceof ContractLineItem)) return new ContractLineItem(input);
	            var values = input || {};

	            var self = this;
	            var _chileProduct = ko.observable();
	            var _packagingProduct = ko.observable();

	            this.ChileProductClassificationKey = values.ChileProductClassificationKey;
	            this.ContractItemKey = values.ContractItemKey;
	            this.ChileProductKey = ko.observable(values.ChileProductKey);
	            this.ChileProductName = ko.observable(values.ChileProductName);
	            this.PackagingProductKey = ko.observable(values.PackagingProductKey);
	            this.PackagingProductName = ko.observable(values.PackagingProductName);
	            this.TreatmentKey = ko.observable(values.TreatmentKey).extend({ treatmentType: true });
	            this.UseCustomerSpec = ko.observable(values.UseCustomerSpec || false);
	            this.CustomerProductCode = ko.observable(values.CustomerProductCode);
	            this.Quantity = ko.numericObservable(values.Quantity);
	            // this.PriceBase = ko.numericObservable(values.PriceBase, 2);
	            this.PriceBase = preciseObservable(values.PriceBase, 2);
	            this.PriceFreight = preciseObservable(values.PriceFreight || 0, 2);
	            this.PriceTreatment = preciseObservable(values.PriceTreatment || 0, 2);
	            this.PriceWarehouse = preciseObservable(values.PriceWarehouse || 0, 2);
	            this.PriceRebate = preciseObservable(values.PriceRebate || 0, 2);

	            /**
	              * @param {(number|observable)} input - Initial value
	              * @param {number} precision - Sets precision of output number
	              */
	            function preciseObservable(input, precision) {
	              var value = ko.observable(parseFloat(input) || 0);
	              var isPrecise = typeof precision === 'number';

	              if (isPrecise) {
	                value(value().toFixed(precision));
	              }

	              var comp = ko.computed({
	                read: function() {
	                  return value();
	                },
	                write: function(val) {
	                  newVal = parseFloat(val);

	                  if (isPrecise) {
	                    newVal = newVal.toFixed(precision);
	                  }

	                  value(newVal);
	                  comp.notifySubscribers();
	                },
	                owner: this
	              });

	              return comp;
	            }

	            this.chileProduct = ko.computed({
	                read: _chileProduct,
	                write: function (value) {
	                    var val = value || {};
	                    self.ChileProductKey(val.ProductKey || null);
	                    self.ChileProductName(val.ProductNameFull || null);
	                    self.ChileProductClassificationKey = val.ChileTypeKey;
	                    _chileProduct(val);
	                }
	            });

	            this.packagingProduct = ko.computed({
	                read: _packagingProduct,
	                write: function (value) {
	                    var val = value || {};
	                    self.PackagingProductKey(val.ProductKey || null);
	                    self.PackagingProductName(val.ProductName || null);
	                    _packagingProduct(value);
	                }
	            });

	            this.totalWeight = ko.pureComputed(function () {
	                var packaging = self.packagingProduct() || {};
	                return (self.Quantity() * packaging.Weight) || 0;
	            }, this);

	            this.totalPricePerPound = ko.pureComputed(function () {
	              var base = parseFloat(self.PriceBase() || 0);
	              var freight = parseFloat(self.PriceFreight() || 0);
	              var treatment = parseFloat(self.PriceTreatment() || 0);
	              var warehouse = parseFloat(self.PriceWarehouse() || 0);
	              var rebate = parseFloat(self.PriceRebate() || 0);

	              return base + freight + treatment + warehouse - rebate;
	            }, this);
	            this.totalPricePerPound.displayValue = ko.pureComputed(function() {
	                return currencyFormat.format(self.totalPricePerPound());
	            });

	            this.totalValue = ko.pureComputed(function () {
	                return self.totalPricePerPound() * self.totalWeight() || 0;
	            }, this);
	            this.totalValue.displayValue = ko.pureComputed(function() {
	                return currencyFormat.format(self.totalValue());
	            });
	        }

	        ContractLineItem.prototype.buildDtoObject = function () {
	            return ko.toJS({
	                ChileProductKey: this.ChileProductKey,
	                PackagingProductKey: this.PackagingProductKey,
	                TreatmentKey: this.TreatmentKey,
	                UseCustomerSpec: this.UseCustomerSpec,
	                CustomerProductCode: this.CustomerProductCode,
	                Quantity: this.Quantity,
	                PriceBase: this.PriceBase,
	                PriceFreight: this.PriceFreight,
	                PriceTreatment: this.PriceTreatment,
	                PriceWarehouse: this.PriceWarehouse,
	                PriceRebate: this.PriceRebate,
	            });
	        };


	        function CustomerContractDetailsViewModel(params) {

	            var vm = this,
	                _selectedContract = ko.observable();

	            // Async Initialization
	            this.isInit = ko.observable(false);

	            vm.initDef = $.when(salesService.getCustomers(),
	                salesService.getBrokers(),
	                salesService.getPaymentTermOptions(),
	                salesService.getWarehouses(),
	                productsService.getChileProducts(app.lists.chileTypes.FinishedGoods.key),
	                productsService.getPackagingProducts())
	                .done(function (customers, brokers, paymentTermOptions, warehouses, products, packaging) {
	                  if (customers[1] !== "success" ||
	                      brokers[1] !== "success" ||
	                      paymentTermOptions[1] !== "success" ||
	                      warehouses[1] !== "success" ||
	                      products[1] !== "success" ||
	                      packaging[1] !== "success") {
	                    throw new Error('Failed to load lookup data.');
	                  }
	                    var opts,
	                        contract;

	                    // Customers
	                    vm.customerOptions(customers[0]);
	                    setInitiallySelectedCusomerForContract.call(vm);

	                    // Brokers
	                    vm.brokerOptions(brokers[0]);
	                    setInitiallySelectedBrokerForContract.call(vm);

	                    // Payment Term Options
	                    vm.paymentTermOptions(paymentTermOptions[0]);

	                    // Warehouses
	                    vm.fobOptions(ko.utils.arrayMap(warehouses[0], function(item) { return item.WarehouseName; }));

	                    // Packaging
	                    vm.packagingProductOptions(packaging[0]);

	                    // Products
	                    opts = ko.utils.arrayMap(products[0], function (product) {
	                        product.ProductCodeAndName = product.ProductCode + ' - ' + product.ProductName;
	                        return product;
	                    });
	                    vm.chileProductOptions(opts.sort(function (a, b) {
	                        return a.ProductCode < b.ProductCode ? -1
	                            : a.ProductCode === b.ProductCode ? 0 : 1;
	                        })
	                    );

	                    vm.isInit(true);
	            });

	            this.subscriptions = [];

	            var addrVisible = ko.observable(false);

	            // Passed into address-book
	            var setContactAddress = function( contactInfo ) {
	              var contractData = this.contract();
	              var contact = contractData.ContactAddress;

	              contractData.ContactName( contactInfo.Name );
	              contact.AddressLine1( contactInfo.Address.AddressLine1 );
	              contact.AddressLine2( contactInfo.Address.AddressLine2 );
	              contact.AddressLine3( contactInfo.Address.AddressLine3 );
	              contact.City( contactInfo.Address.City );
	              contact.State( contactInfo.Address.State );
	              contact.PostalCode( contactInfo.Address.PostalCode );
	              contact.Country( contactInfo.Address.Country );
	            }.bind(this);

	            this.contactLabelData = {
	              visible: addrVisible,
	              buttons: [{
	                callback: function( contact ) {
	                  setContactAddress( contact );
	                  addrVisible( false );
	                }.bind(this),
	                text: 'Select Contact'
	              }]
	            };

	            this.templateMode = ko.observable("edit-view");
	            this.currentTab = ko.observable();
	            this.customerOptions = ko.observableArray([]);
	            this.brokerOptions = ko.observableArray([]);
	            this.paymentTermOptions = ko.observableArray([]);
	            this.fobOptions = ko.observableArray([]);
	            this.chileProductOptions = ko.observableArray([]);
	            this.packagingProductOptions = ko.observableArray([]);
	            this.loadingContractDetails = ko.observable(false);
	            this.basePriceAdjust = ko.numericObservable(0, 2);
	            this.productToAdjust = ko.observable();
	            this.chileClassifications = rvc.lists.chileClassifications.buildSelectListOptions();

	            this.contract = ko.computed({
	                read: _selectedContract,
	                write: function(value) {
	                    if (!value) return _selectedContract(null);
	                    if (!(value instanceof CustomerContract)) {
	                        value = new CustomerContract(value);
	                    }
	                    addrVisible(false);
	                    setInitialOptionSectionsForContract.call(vm, value);
	                    setInitiallySelectedCusomerForContract.call(vm, value);
	                    setInitiallySelectedBrokerForContract.call(vm, value);
	                    value.__esm = helpers.esmHelper(value, {
	                        customMappings: {
	                            ContractItems: function(vals) {
	                                return ko.utils.arrayMap(vals, function(item) {
	                                    var mapped = new ContractLineItem(item);
	                                    setInitiallySelectedOptionsForContractItem.call(vm, mapped);
	                                    return mapped;
	                                });
	                            },
	                        },
	                        customRevertFunctions: {
	                            ContactAddress: function (val, target) {
	                                target.update(val);
	                            },
	                            CustomerKey: function (val) {
	                                value.customer(findCustomerOptionByKey.call(vm, val));
	                                return val;
	                            },
	                        },
	                        include: ['ContactAddress']
	                    });
	                    vm.initDef.then(function () {
	                        setTimeout(function () { _selectedContract(value); }, 1000);
	                    });
	                    return value;
	                }
	            });

	            this.showContacts = ko.command({
	              execute: function() {
	                addrVisible.call(this, true);
	              },
	              canExecute: function() {
	                var contract = vm.contract();
	                return contract && ko.unwrap(contract.customer);
	              }
	            });
	            this.hideContacts = addrVisible.bind(this, false);
	            this.addressVisible = ko.computed({
	              read: function isAddrVisible() {
	                return addrVisible() && this.contract();
	              },
	              write: function setVisibility( val ) {
	                if ( !val ) {
	                  this.hideContacts();
	                }
	              },
	              owner: this
	            });

	            if (params.contract && ko.isObservable(params.contract)) {
	                vm.subscriptions.push(this.contract.subscribe(params.contract));
	            }
	            if (params.contractKey && ko.isObservable(params.contractKey)) {
	                vm.subscriptions.push(params.contractKey.subscribe(function (key) {
	                  var input = params;

	                    if (key === 'new') {
	                      if (params.input && params.input.isClone === true) {
	                        return;
	                      } else {
	                        vm.contract(new CustomerContract());
	                      }
	                    } else if (!key) {
	                        vm.contract(null);
	                    } else {
	                        vm.loadContractByKey.call(vm, key);
	                    }
	                }));
	            }

	            this.saveContractCommand = ko.asyncCommand({
	                execute: function (complete) {
	                  var contract = vm.contract();
	                    if (contract.validation.isValid()) {
	                      var contract = vm.contract();
	                      var dto = contract.buildDtoObject();
	                      var dfd;

	                      if (contract.isNew) {
	                          dfd = salesService.createContract(dto);
	                          dfd.then(function (data) {
	                                  vm.contract().CustomerContractKey = data;
	                                  showUserMessage("Contract Created Successfully");
	                              })
	                              .fail(function (xhr, status, message) { showUserMessage("Failed to create contract.", { description: "An error occurred while attempting to save the contract. " + message }); });
	                      } else {
	                          dfd = salesService.updateContract(contract.CustomerContractKey, dto);
	                          dfd.then(function () { showUserMessage("Contract Saved Successfully"); })
	                              .fail(function (xhr, status, message) { showUserMessage("Failed to Update Contract", { description: "An error occurred while attempting to save the contract. " + message }); });
	                      }

	                      dfd.always(complete);

	                      return dfd;
	                    } else {
	                      var rejectSave = $.Deferred();

	                      rejectSave.then(
	                      null,
	                      function(jqXHR, textStatus, errorThrown) {
	                        showUserMessage("Could not save contract", { description: errorThrown });
	                      });

	                      rejectSave.reject(null, null, 'Please fill in all required fields');

	                      contract.validation.errors.showAllMessages();
	                      complete();

	                      return rejectSave;
	                    }
	                },
	                canExecute: function(isExecuting) {
	                    return !isExecuting && vm.contract() && true;
	                }
	            });

	            this.applyAdjustmentsCommand = ko.command({
	                execute: function () {
	                    var adjustmentAmount = vm.basePriceAdjust();
	                    var productType = vm.productToAdjust();
	                    var productTypeKey = productType && productType.key;

	                    vm.contract().adjustBasePriceOfItems(adjustmentAmount, function (item) {
	                        return !productTypeKey || item.ChileProductClassificationKey == productTypeKey;
	                    });
	                },
	                canExecute: function() {
	                    return vm.contract() && vm.basePriceAdjust();
	                }
	            });
	        }

	        CustomerContractDetailsViewModel.prototype.findContractByKey = function(key) {
	            var dfd = $.Deferred();
	            salesService.getContractDetails(key)
	                .then(function (data) {
	                    var contract = new CustomerContract(data);
	                    dfd.resolve(contract);
	                })
	                .fail(function(xhr, status, message) {
	                    showUserMessage("Failed to get contract details.", { description: message });
	                    dfd.reject.call(null, arguments);
	                });
	            return dfd;
	        };
	        CustomerContractDetailsViewModel.prototype.loadContractByKey = function (key) {
	            var vm = this;
	            vm.loadingContractDetails(true);

	            return CustomerContractDetailsViewModel.prototype.findContractByKey(key)
	                .then(vm.contract)
	                .always(function () { vm.loadingContractDetails(false); });
	        };
	        CustomerContractDetailsViewModel.prototype.clearContractSelection = function() {
	            this.contract(null);
	        };

	        CustomerContractDetailsViewModel.prototype.dispose = function() {
	            var subscriptions = this.subscriptions || [];
	            ko.utils.arrayForEach(subscriptions, function (s) { s && s.dispose && s.dispose(); });

	            var selectedContract = this.contract();
	            if (selectedContract) {
	                selectedContract.__esm.dispose();
	            }
	        };

	        function setInitialOptionSectionsForContract(contract) {
	            var vm = this;
	            ko.utils.arrayForEach(contract.ContractItems(), function (item) {
	                setInitiallySelectedOptionsForContractItem.call(vm, item);
	            });
	        }
	        function setInitiallySelectedOptionsForContractItem(item) {
	            var vm = this;
	            var selectedChileProduct = ko.utils.arrayFirst(vm.chileProductOptions(), function (p) {
	                return p.ProductKey === item.ChileProductKey.peek();
	            });
	            item.chileProduct(selectedChileProduct);

	            var selectedPackagingProduct = ko.utils.arrayFirst(vm.packagingProductOptions(), function (p) {
	                return p.ProductKey === item.PackagingProductKey.peek();
	            });
	            item.packagingProduct(selectedPackagingProduct);
	        }
	        function setInitiallySelectedCusomerForContract(contract) {
	            if (!contract) return;
	            var vm = this;
	            var customerKey = contract.CustomerKey();
	            if (customerKey) {
	                if (vm.customerOptions().length) {
	                    setCustomer(customerKey);
	                } else {
	                    var sub = vm.customerOptions.subscribe(function() {
	                        setCustomer(customerKey, sub);
	                        sub = null;
	                    });
	                    vm.subscriptions.push(sub);
	                }
	            } else contract.customer(null);

	            function setCustomer(keyValue, subscription) {
	                var selected = contract.customer() && contract.customer().CompanyKey === keyValue ?
	                  contract.customer() :
	                  findCustomerOptionByKey.call(vm, keyValue);
	                contract.customer(selected || null);

	                if (subscription) {
	                    subscription.dispose();
	                }
	            }
	        }
	        function findCustomerOptionByKey(customerKey) {
	            var vm = this;
	            return ko.utils.arrayFirst(vm.customerOptions(), function (opt) { return opt.CompanyKey === customerKey; });
	        }
	        function setInitiallySelectedBrokerForContract(contract) {
	            if (!contract) return;
	            var vm = this;

	            var brokerKey = contract.BrokerCompanyKey();
	            if (brokerKey) {
	                if (vm.brokerOptions()) {
	                    setBroker(brokerKey);
	                } else {
	                    var sub = vm.brokerOptions().subscribe(function() {
	                        setBroker(brokerKey, sub);
	                        sub = null;
	                    });
	                    vm.subscriptions.push(sub);
	                }

	            } else contract.broker(null);

	            function setBroker(keyValue, subscription) {
	                var selected = contract.broker() && contract.broker().CompanyKey === brokerKey ?
	                  contract.broker() :
	                  ko.utils.arrayFirst(vm.brokerOptions(), function (opt) { return opt.CompanyKey === brokerKey; });
	                contract.broker(selected || null);

	                if (subscription) {
	                    subscription.dispose();
	                }
	            }
	        }

	        //#endregion

	        ko.components.register('notebook-control', notebookControlComponent);
	        ko.components.register( 'contact-address-label-helper', __webpack_require__(67) );

	        return { viewModel: CustomerContractDetailsViewModel, template: templateMarkup };
	    }.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 47 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(48), __webpack_require__(49), __webpack_require__(9), __webpack_require__(50)], __WEBPACK_AMD_DEFINE_RESULT__ = function (templateMarkup, notebooksService, ko) {
	        //document.insertStyleIntoHead(notebookCss);

	        function Note(input) {
	            if(!(this instanceof Note)) return new Note(input);
	            var values = input || {};

	            this.Text = ko.observable(values.Text);
	            this.CreatedByUser = ko.observable(values.CreatedByUser);
	            this.NoteDate = ko.observableDateTime(values.NoteDate || Date.now(), "m/d/yyyy h:MM tt");
	            this.NoteKey = values.NoteKey;
	        }

	        Note.prototype.toDto = function() {
	            return {
	                NoteKey: this.NoteKey,
	                Text: this.Text(),
	            }
	        }

	        function NotebookControlViewModel(params) {
	            params = params || {};

	            var vm = this;

	            this.notebookKey = ko.isObservable(params.notebookKey) ? params.notebookKey : ko.observable(params.notebookKey);
	            this.notes = ko.observableArray();
	            this.newNote = ko.observable();

	            this.insertNoteCommand = ko.asyncCommand({
	                execute: function (complete) {
	                    var newNote = new Note({
	                        Text: vm.newNote(),
	                    });

	                    return notebooksService.insertNote(vm.notebookKey(), newNote.toDto())
	                        .then(function(data) {
	                            vm.notes.push(new Note(data));
	                            vm.newNote(null);
	                        })
	                        .always(complete);
	                },
	                canExecute: function(isExecuting) {
	                    return !isExecuting && vm.newNote() && true;
	                },
	                owner: this,
	            });

	            this.loadNotebook();
	        }

	        NotebookControlViewModel.prototype.loadNotebook = function () {
	            var notes = this.notes;
	            notes([]);

	            var key = this.notebookKey();
	            if (!key) return;
	            
	            notebooksService.getNotebookByKey(key)
	                .then(function (data) {
	                    notes(ko.utils.arrayMap(data, function (item) { return new Note(item); }));
	                });
	        }
	        
	        return {
	            viewModel: NotebookControlViewModel,
	            template: templateMarkup
	        }
	    }.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));

/***/ }),
/* 48 */
/***/ (function(module, exports) {

	module.exports = "<section class=\"notebook\">\r\n    <!-- ko foreach: notes -->\r\n    <article>\r\n        <p data-bind=\"text: Text\"></p>\r\n        <footer>\r\n            <strong data-bind=\"text: CreatedByUser\"></strong>:\r\n            <span data-bind=\"text: NoteDate.formattedDate\"></span>\r\n        </footer>\r\n    </article>\r\n    <!-- /ko -->\r\n    \r\n    <div class=\"well\">\r\n      <div class=\"form-group\">\r\n        <label class=\"control-label\">Create New Note</label>\r\n        <textarea class=\"form-control\" data-bind=\"value: newNote, valueUpdate: 'input'\" rows=\"3\"></textarea>\r\n      </div>\r\n      <button type=\"button\" class=\"btn btn-primary\" data-bind=\"command: insertNoteCommand\">Save Note</button>\r\n    </div>\r\n</section>\r\n"

/***/ }),
/* 49 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(7), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function (core, ko) {
	    return {
	        getNotebookByKey: getNotebookByKey,
	        insertNote: insertNote,
	        updateNote: putNote,
	        updateNoteText: updateNoteText,
	        deleteNote: deleteNote,

	        // phase these methods out
	        putNote: putNote,
	        postNote: postNote,
	    };

	    //#region exports
	    function getNotebookByKey(key, options) {
	        return core.ajax("/api/notebooks/" + key + "/notes", options);
	    }

	    function putNote(note, options) {
	        options.data = ko.toJSON(note.toDto());
	        return core.ajaxPut("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, options);
	    }

	    function updateNoteText(note) {
	        var NoteKey = note.NoteKey;
	        var NotebookKey = note.NotebookKey;
	        var NoteText = note.toDto();

	        return core.ajaxPut("/api/notebooks/" + NotebookKey + "/notes/" + NoteKey, NoteText);
	    }

	    function insertNote(notebookKey, values) {
	        return core.ajaxPost("/api/notebooks/" + notebookKey + "/notes", values);
	    }
	    function postNote(note, options) {
	        console.warn("postNote is obsolete. Use insertNote instead.");
	        options.data = ko.toJSON(note.toDto ? note.toDto() : note);
	        return core.ajaxPost("/api/notebooks/" + note.NotebookKey + "/notes", options);
	    }

	    function deleteNote(note, options) {
	        return core.ajaxDelete("/api/notebooks/" + note.NotebookKey + "/notes/" + note.NoteKey, options);        
	    }
	    //#endregion
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 50 */
/***/ (function(module, exports, __webpack_require__) {

	// style-loader: Adds some css to the DOM by adding a <style> tag

	// load the styles
	var content = __webpack_require__(51);
	if(typeof content === 'string') content = [[module.id, content, '']];
	// add the styles to the DOM
	var update = __webpack_require__(53)(content, {});
	// Hot Module Replacement
	if(false) {
		// When the styles change, update the <style> tags
		module.hot.accept("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\Content\\notebook.css", function() {
			var newContent = require("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\Content\\notebook.css");
			if(typeof newContent === 'string') newContent = [[module.id, newContent, '']];
			update(newContent);
		});
		// When the module is disposed, remove the <style> tags
		module.hot.dispose(function() { update(); });
	}

/***/ }),
/* 51 */
/***/ (function(module, exports, __webpack_require__) {

	exports = module.exports = __webpack_require__(52)();
	exports.push([module.id, ".notebook {\r\n    border-bottom: 1px solid #ebeeef;\r\n    padding-bottom: 5px;\r\n    border-left: 3px solid transparent;\r\n    margin-left: -3px;\r\n}\r\n\r\n    .notebook article {\r\n        border-width: 0 0 0 3px;\r\n        border-color: transparent;\r\n        border-style: solid;\r\n        padding-left: 3px;\r\n        margin-left: -3px;\r\n        margin-top: 3px;\r\n        margin-bottom: 15px;\r\n    }\r\n\r\n    .notebook article > p {\r\n        margin: 0 0 4px 0;\r\n    }\r\n\r\n    .notebook article:hover {\r\n        border-color: #8ba5c5;\r\n    }\r\n\r\n    .notebook article > footer {\r\n        font-size: .7em;\r\n    }\r\n\r\n.notebook > .newNote {\r\n    margin: 10px 0;\r\n    background-color: #ebeeef;\r\n    padding: 2px;\r\n}\r\n\r\n    .notebook > .newNote textarea {\r\n        display: block;\r\n        width: 98%;\r\n        height: 80px;\r\n    }", ""]);

/***/ }),
/* 52 */
/***/ (function(module, exports) {

	module.exports = function() {
		var list = [];
		list.toString = function toString() {
			var result = [];
			for(var i = 0; i < this.length; i++) {
				var item = this[i];
				if(item[2]) {
					result.push("@media " + item[2] + "{" + item[1] + "}");
				} else {
					result.push(item[1]);
				}
			}
			return result.join("");
		};
		return list;
	}

/***/ }),
/* 53 */
/***/ (function(module, exports, __webpack_require__) {

	/*
		MIT License http://www.opensource.org/licenses/mit-license.php
		Author Tobias Koppers @sokra
	*/
	var stylesInDom = {},
		memoize = function(fn) {
			var memo;
			return function () {
				if (typeof memo === "undefined") memo = fn.apply(this, arguments);
				return memo;
			};
		},
		isIE9 = memoize(function() {
			return /msie 9\b/.test(window.navigator.userAgent.toLowerCase());
		}),
		getHeadElement = memoize(function () {
			return document.head || document.getElementsByTagName("head")[0];
		}),
		singletonElement = null,
		singletonCounter = 0;

	module.exports = function(list, options) {
		if(false) {
			if(typeof document !== "object") throw new Error("The style-loader cannot be used in a non-browser environment");
		}

		options = options || {};
		// Force single-tag solution on IE9, which has a hard limit on the # of <style>
		// tags it will allow on a page
		if (typeof options.singleton === "undefined") options.singleton = isIE9();

		var styles = listToStyles(list);
		addStylesToDom(styles, options);

		return function update(newList) {
			var mayRemove = [];
			for(var i = 0; i < styles.length; i++) {
				var item = styles[i];
				var domStyle = stylesInDom[item.id];
				domStyle.refs--;
				mayRemove.push(domStyle);
			}
			if(newList) {
				var newStyles = listToStyles(newList);
				addStylesToDom(newStyles, options);
			}
			for(var i = 0; i < mayRemove.length; i++) {
				var domStyle = mayRemove[i];
				if(domStyle.refs === 0) {
					for(var j = 0; j < domStyle.parts.length; j++)
						domStyle.parts[j]();
					delete stylesInDom[domStyle.id];
				}
			}
		};
	}

	function addStylesToDom(styles, options) {
		for(var i = 0; i < styles.length; i++) {
			var item = styles[i];
			var domStyle = stylesInDom[item.id];
			if(domStyle) {
				domStyle.refs++;
				for(var j = 0; j < domStyle.parts.length; j++) {
					domStyle.parts[j](item.parts[j]);
				}
				for(; j < item.parts.length; j++) {
					domStyle.parts.push(addStyle(item.parts[j], options));
				}
			} else {
				var parts = [];
				for(var j = 0; j < item.parts.length; j++) {
					parts.push(addStyle(item.parts[j], options));
				}
				stylesInDom[item.id] = {id: item.id, refs: 1, parts: parts};
			}
		}
	}

	function listToStyles(list) {
		var styles = [];
		var newStyles = {};
		for(var i = 0; i < list.length; i++) {
			var item = list[i];
			var id = item[0];
			var css = item[1];
			var media = item[2];
			var sourceMap = item[3];
			var part = {css: css, media: media, sourceMap: sourceMap};
			if(!newStyles[id])
				styles.push(newStyles[id] = {id: id, parts: [part]});
			else
				newStyles[id].parts.push(part);
		}
		return styles;
	}

	function createStyleElement() {
		var styleElement = document.createElement("style");
		var head = getHeadElement();
		styleElement.type = "text/css";
		head.appendChild(styleElement);
		return styleElement;
	}

	function addStyle(obj, options) {
		var styleElement, update, remove;

		if (options.singleton) {
			var styleIndex = singletonCounter++;
			styleElement = singletonElement || (singletonElement = createStyleElement());
			update = applyToSingletonTag.bind(null, styleElement, styleIndex, false);
			remove = applyToSingletonTag.bind(null, styleElement, styleIndex, true);
		} else {
			styleElement = createStyleElement();
			update = applyToTag.bind(null, styleElement);
			remove = function () {
				styleElement.parentNode.removeChild(styleElement);
			};
		}

		update(obj);

		return function updateStyle(newObj) {
			if(newObj) {
				if(newObj.css === obj.css && newObj.media === obj.media && newObj.sourceMap === obj.sourceMap)
					return;
				update(obj = newObj);
			} else {
				remove();
			}
		};
	}

	function replaceText(source, id, replacement) {
		var boundaries = ["/** >>" + id + " **/", "/** " + id + "<< **/"];
		var start = source.lastIndexOf(boundaries[0]);
		var wrappedReplacement = replacement
			? (boundaries[0] + replacement + boundaries[1])
			: "";
		if (source.lastIndexOf(boundaries[0]) >= 0) {
			var end = source.lastIndexOf(boundaries[1]) + boundaries[1].length;
			return source.slice(0, start) + wrappedReplacement + source.slice(end);
		} else {
			return source + wrappedReplacement;
		}
	}

	function applyToSingletonTag(styleElement, index, remove, obj) {
		var css = remove ? "" : obj.css;

		if(styleElement.styleSheet) {
			styleElement.styleSheet.cssText = replaceText(styleElement.styleSheet.cssText, index, css);
		} else {
			var cssNode = document.createTextNode(css);
			var childNodes = styleElement.childNodes;
			if (childNodes[index]) styleElement.removeChild(childNodes[index]);
			if (childNodes.length) {
				styleElement.insertBefore(cssNode, childNodes[index]);
			} else {
				styleElement.appendChild(cssNode);
			}
		}
	}

	function applyToTag(styleElement, obj) {
		var css = obj.css;
		var media = obj.media;
		var sourceMap = obj.sourceMap;

		if(sourceMap && typeof btoa === "function") {
			try {
				css += "\n/*# sourceMappingURL=data:application/json;base64," + btoa(JSON.stringify(sourceMap)) + " */";
				css = "@import url(\"data:text/css;base64," + btoa(css) + "\")";
			} catch(e) {}
		}

		if(media) {
			styleElement.setAttribute("media", media)
		}

		if(styleElement.styleSheet) {
			styleElement.styleSheet.cssText = css;
		} else {
			while(styleElement.firstChild) {
				styleElement.removeChild(styleElement.firstChild);
			}
			styleElement.appendChild(document.createTextNode(css));
		}
	}


/***/ }),
/* 54 */
/***/ (function(module, exports) {

	module.exports = "<!-- ko if: isInit -->\r\n<div class=\"container-fluid\">\r\n    <header class=\"contract-header\" data-bind=\"with: contract\">\r\n        <!-- ko ifnot: isNew -->\r\n        <h1>Contract No. <!-- ko text: CustomerContractKey --><!-- /ko --> (<!-- ko text: ContractNumber --><!-- /ko -->)</h1>        \r\n        <!-- /ko -->\r\n\r\n        <!-- ko if: isNew -->\r\n        <h1>New Contract</h1>\r\n        <!-- /ko -->\r\n\r\n        <div class=\"row\">\r\n          <div class=\"col-sm-6\">\r\n            <div class=\"form-group\" data-bind=\"validationElement: customer\">\r\n              <label class=\"control-label\">Customer:</label>\r\n              <select data-bind=\"value: customer, options: $parent.customerOptions, optionsText: 'Name', optionsCaption: ' ', enable: isNew\" class=\"form-control\"></select>\r\n            </div>\r\n          </div>\r\n          <div class=\"col-sm-4 col-sm-offset-2\">\r\n            <div class=\"form-group\" data-bind=\"validationElement: ContractDate\">\r\n              <label class=\"control-label\">Contract Date: </label>\r\n              <input type=\"text\" class=\"form-control\" data-bind=\"value: ContractDate, datePicker: true\" />\r\n            </div>\r\n          </div>\r\n        </div>\r\n    </header>\r\n    \r\n    <section data-bind=\"if: contract\">\r\n      <!-- ko with: contract -->\r\n        <contact-address-label-helper params=\"key: customer() && customer().CompanyKey,\r\n          companies: $parent.customerOptions,\r\n          buttons: $parent.contactLabelData.buttons,\r\n          visible: $parent.contactLabelData.visible\">\r\n        </contact-address-label-helper>\r\n      <!-- /ko -->\r\n\r\n        <div data-bind=\"tabs: currentTab\">\r\n            <ul>\r\n                <li><a href=\"#header\">Contract Information</a></li>\r\n                <li><a href=\"#lineItems\">Contract Line Items</a></li>\r\n            </ul>\r\n\r\n            <div id=\"header\">\r\n                <div data-bind=\"template: {name: 'header-' + templateMode(), data: contract }\"></div>\r\n\r\n                <div>\r\n                    <label class=\"control-label\">Comments:</label>\r\n                    <!-- ko ifnot: contract().isNew -->\r\n                    <notebook-control params=\"notebookKey: contract().CommentsNotebookKey\"></notebook-control>\r\n                    <!-- /ko -->\r\n                    <p data-bind=\"if: contract().isNew\">Comments will be available after the contract has been saved.</p>\r\n                </div>\r\n            </div>\r\n\r\n            <div id=\"lineItems\" data-bind=\"template: { name: 'line-items-' + templateMode(), data: contract }\"></div>\r\n\r\n        </div>\r\n    </section>\r\n    \r\n</div>\r\n<!-- /ko -->\r\n\r\n<script type=\"text/html\" id=\"header-edit-view\">\r\n    <fieldset class=\"row\">\r\n        <legend>Contract Editor</legend>\r\n                <div class=\"col-sm-6\">\r\n                        <div class=\"form-group\">\r\n                          <label class=\"control-label\">Contact</label>\r\n                          <div class=\"input-group\">\r\n                            <input type=\"text\" class=\"form-control\" data-bind=\"value: ContactName\" />\r\n                            <span class=\"input-group-btn\">\r\n                              <button class=\"btn btn-default\" data-bind=\"command: $parent.showContacts\">\r\n                                <i class=\"fa fa-book\"></i>\r\n                              </button>\r\n                            </span>\r\n                          </div>\r\n                        </div>\r\n                            <div data-bind=\"with: ContactAddress\">\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">Address Line 1</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: AddressLine1\" />\r\n                              </div>\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">Address Line 2</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: AddressLine2\" />\r\n                              </div>\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">Address Line 3</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: AddressLine3\" />\r\n                              </div>\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">City</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: City\"  />\r\n                              </div>\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">State</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: State\" />\r\n                              </div>\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">Zip</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: PostalCode\" />\r\n                              </div>\r\n                              <div class=\"form-group\">\r\n                                <label class=\"control-label\">Country</label>\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: Country\" />\r\n                              </div>\r\n                            </div>\r\n                </div>\r\n\r\n                <div class=\"col-sm-6\">\r\n                        <div class=\"form-group\">\r\n                          <label class=\"control-label\">FOB</label>\r\n                          <input type=\"text\" class=\"form-control\" data-bind=\"value: FOB, autocomplete: { source: $parent.fobOptions, allowNewValues: true}\" />\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                          <label class=\"control-label\">Contract Term</label>\r\n\r\n                          <div class=\"container-fluid\">\r\n                            <div class=\"row\">\r\n                              <div class=\"col-sm-6\">\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: TermBegin, datePicker: true\" />\r\n                              </div>\r\n                              <div class=\"col-sm-6\">\r\n                                <input type=\"text\" class=\"form-control\" data-bind=\"value: TermEnd, datePicker: true\" />\r\n                              </div>\r\n                            </div>\r\n                          </div>\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                        <label class=\"control-label\">Contract Type</label>\r\n                        <select class=\"form-control\" data-bind=\"value: ContractType, options: ContractType.options, optionsText: 'value', optionsValue: 'key'\"></select>\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                        <label class=\"control-label\">Contract Status</label>\r\n                        <select class=\"form-control\" data-bind=\"value: ContractStatus, options: ContractStatus.options, optionsText: 'value', optionsValue: 'key'\"></select>\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                        <label class=\"control-label\">Broker</label>\r\n                        <select class=\"form-control\" data-bind=\"value: broker, options: $parent.brokerOptions, optionsText: 'Name', optionsCaption: ' '\"></select>\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                          <label class=\"control-label\">Payment Terms</label>\r\n                          <input type=\"text\" class=\"form-control\" data-bind=\"autocomplete: { source: $parent.paymentTermOptions, allowNewValues: true}, value: PaymentTerms\" />\r\n                        </div>\r\n                        <div class=\"form-group\">\r\n                          <label class=\"control-label\">Customer PO Num.</label>\r\n                          <input type=\"text\" class=\"form-control\" data-bind=\"value: CustomerPurchaseOrder\" />\r\n                        </div>\r\n                      <div class=\"form-group\">\r\n                          <label class=\"control-label\" style=\"vertical-align: top\">Notes to Print</label>\r\n                          <textarea class=\"form-control\" data-bind=\"value: NotesToPrint\"></textarea>\r\n                        </div>\r\n                </div>\r\n    </fieldset>\r\n</script>\r\n\r\n<script type=\"text/html\" id=\"line-items-edit-view\">\r\n\r\n    <div class=\"bg-info\" style=\"padding: 0 5px 5px;\">\r\n        <h3>Adjust base price</h3>\r\n        <div class=\"row\">\r\n            <div class=\"col-sm-3 col-md-2\">\r\n                <div class=\"form-group\">\r\n                    <label>Items Affected:</label>\r\n                    <select class=\"form-control\" data-bind=\"value: $parent.productToAdjust, options: $parent.chileClassifications, optionsText: 'key', optionsValue: 'value', optionsCaption: 'All Items'\"></select>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-sm-3 col-md-2\">\r\n                <label>Adjust by amount:</label>\r\n                <div class=\"input-group\">\r\n                    <input type=\"number\" class=\"form-control\" data-bind=\"value: $parent.basePriceAdjust\" />\r\n                    <span class=\"input-group-btn\">\r\n                        <button type=\"button\" class=\"btn btn-default\" data-bind=\"command: $parent.applyAdjustmentsCommand\">Adjust</button>\r\n                    </span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\"table-responsive\">\r\n        <table class=\"reset table table-condensed table-striped table-bordered small\">\r\n            <thead class=\"small\">\r\n                <tr>\r\n                    <th></th>\r\n                    <th class=\"col-sm-3\">Product</th>\r\n                    <th>Packaging</th>\r\n                    <th>Treatment</th>\r\n                    <th>Customer Spec</th>\r\n                    <th class=\"text-nowrap\">Customer Code</th>\r\n                    <th>Quantity</th>\r\n                    <th class=\"calc numeric\">Total Weight</th>\r\n                    <th>Base Price</th>\r\n                    <th>Freight</th>\r\n                    <th>Treatment</th>\r\n                    <th>Warehouse</th>\r\n                    <th>Rebate</th>\r\n                    <th class=\"calc numeric\">Total</th>\r\n                    <th class=\"calc numeric\">Value</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody data-bind=\"foreach: ContractItems\">\r\n                <tr>\r\n                    <td>\r\n                        <button type=\"button\" class=\"btn btn-link\" data-bind=\"command: $parent.removeContractItemCommand\"><i class=\"fa fa-times\"></i></button>\r\n                    </td>\r\n                    <td>\r\n                        <select class=\"form-control product-select\" data-bind=\"value: chileProduct, options: $parents[1].chileProductOptions, optionsText: 'ProductCodeAndName', optionsCaption: ' '\"></select>\r\n                    </td>\r\n                    <td>\r\n                        <select class=\"form-control product-select\" data-bind=\"value: packagingProduct, options: $parents[1].packagingProductOptions, optionsText: 'ProductName', optionsCaption: ' '\"></select>\r\n                    </td>\r\n                    <td>\r\n                        <select class=\"form-control\" data-bind=\"value: TreatmentKey, options: TreatmentKey.options, optionsText: 'value', optionsValue: 'key'\"></select>\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"checkbox\" data-bind=\"checked: UseCustomerSpec\" />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" data-bind=\"value: CustomerProductCode\" class=\"small-medium\" />\r\n                    </td>\r\n                    <td class=\"numeric\">\r\n                        <input class=\"form-control\" data-bind=\"value: Quantity\" type=\"number\" />\r\n                    </td>\r\n                    <td class=\"info\" data-bind=\"text: totalWeight().toLocaleString()\"></td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" data-bind=\"value: PriceBase\" class=\"short\" />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" data-bind=\"value: PriceFreight\" class=\"short\" />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" data-bind=\"value: PriceTreatment\" class=\"short\" />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" data-bind=\"value: PriceWarehouse\" class=\"short\" />\r\n                    </td>\r\n                    <td>\r\n                        <input type=\"text\" class=\"form-control\" data-bind=\"value: PriceRebate\" class=\"short\" />\r\n                    </td>\r\n                    <td class=\"info\" data-bind=\"text: totalPricePerPound | USD\"></td>\r\n                    <td class=\"info\" data-bind=\"text: totalValue | USD\"></td>\r\n                </tr>\r\n            </tbody>\r\n            <tfoot>\r\n                <tr>\r\n                    <td colspan=\"6\"></td>\r\n                    <td data-bind=\"text: totalQuantityOnContract().toLocaleString()\" class=\"numeric\"></td>\r\n                    <td data-bind=\"text: totalPoundsOnContract().toLocaleString()\" class=\"info\"></td>\r\n                    <td data-bind=\"text: averageBasePricePerPound | USD\" class=\"numeric\"></td>\r\n                    <td colspan=\"4\"></td>\r\n                    <td class=\"info\" data-bind=\"text: averageNetPricePerPound | USD\"></td>\r\n                    <td class=\"info\" data-bind=\"text: totalValueOfContract | USD\"></td>\r\n                </tr>\r\n            </tfoot>\r\n        </table>\r\n    </div>\r\n\r\n    <button class=\"btn btn-primary\" data-bind=\"command: addContractItemCommand\">Add Line Item</button>\r\n</script>\r\n"

/***/ }),
/* 55 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(1), __webpack_require__(9), __webpack_require__(56)], __WEBPACK_AMD_DEFINE_RESULT__ = function ($, ko) {
	    var ajaxStatus = {
	        success: 2,
	        failure: -1,
	        working: 1,
	        none: 0,
	    };

	    setupKoEditStateManager();

	    return {
	        ajaxStatusHelper: initAjaxStatusHelper,
	        animateNewListElement: animateNewListElement,
	        esmHelper: initEsmHelper,
	        getDataForClickedElement: function (options) {
	            options = options || {};
	            options.isDesiredTarget = options.isDesiredTarget || function () { return true; }

	            var data = options.clickArguments[0];

	            if (options.isDesiredTarget(data)) return data;
	            if (options.clickArguments.length < 2) throw new Error("Incorrect number of arguments for click handler.");

	            var targetElement = options.clickArguments[1].originalEvent.target;

	            if (!targetElement) throw new Error("Target element could not be determined.");


	            var context = ko.contextFor(targetElement);
	            if (context && options.isDesiredTarget(context.$data)) {
	                return context.$data;
	            }

	            throw new Error("Unable to identify data for target element.");
	        },
	    };
	   

	    //#region ajaxStatusHelper
	    function initAjaxStatusHelper(target) {
	        if (target == undefined) throw new Error("Target cannot be undefined.");

	        target.ajaxStatus = ko.observable(ajaxStatus.none);
	        target.indicateSuccess = success.bind(target);
	        target.indicateWorking = working.bind(target);
	        target.indicateFailure = failure.bind(target);
	        target.clearStatus = clear.bind(target);

	        // computed properties
	        target.ajaxSuccess = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.success;
	        }, target);
	        target.ajaxFailure = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.failure;
	        }, target);
	        target.ajaxWorking = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.working;
	        }, target);
	        target.ajaxInactive = ko.computed(function () {
	            return this.ajaxStatus() === ajaxStatus.none;
	        }, target);

	        
	        return target;
	    }
	    
	    // functions
	    function clear() {
	        this.ajaxStatus(ajaxStatus.none);
	    }
	    function success() {
	        this.ajaxStatus(ajaxStatus.success);
	    }
	    function working() {
	        this.ajaxStatus(ajaxStatus.working);
	    }
	    function failure() {
	        this.ajaxStatus(ajaxStatus.failure);
	    }
	    //#endregion ajaxStatusHelper

	    //#region animateNewListItem
	    function animateNewListElement(options) {
	        options = options || {};
	        options.paddingTop = options.paddingTop == undefined ? 120 : options.paddingTop;
	        return function(elem) {
	            if (elem.nodeType === 1) {
	                var $elem = $(elem);
	                var origBg = $elem.css('background-color');


	                if (doScroll()) {
	                    var maxHeightContainer = $('.maxHeight-container');
	                    var floatingHeader = maxHeightContainer.find('.tableFloatingHeader');
	                    if (floatingHeader) {
	                        options.paddingTop = floatingHeader.height() + 100; // the 100 shouldn't be necessary but without it, the scrollTop goes off screen...
	                    }

	                    if (maxHeightContainer) {
	                        maxHeightContainer.animate({
	                            scrollTop: (maxHeightContainer.scrollTop() + $elem.position().top) - options.paddingTop // need to allow for floating headers
	                        }, 2000);
	                    } else {
	                        $('html, body').animate({
	                            scrollTop: $elem.offset().top - 100 // need to allow for floating headers
	                        }, 2000);
	                    }

	                    if (options.afterScrollCallback) options.afterScrollCallback();
	                }

	                $elem.css('opacity', 0);
	                $elem.animate({ backgroundColor: "#a6dbed", opacity: 1 }, 800)
	                    .delay(2500)
	                    .animate({ backgroundColor: origBg || 'rgb(255, 255, 255)' }, 1000, function() {
	                        $elem.css('background-color', ''); // remove style from element to allow css to regain control
	                    });
	            };
	        };

	        function doScroll() {
	            var scrollOption = options.scrollToItem;
	            return ko && ko.isObservable(scrollOption)
	                ? scrollOption()
	                : scrollOption;
	        }
	    }
	    //#endregion animateNewListItem

	    //#region Edit State Manager
	    function initEsmHelper(objectToTrack, options) {
	        if (!objectToTrack) throw new Error("Must provide an objectToTrack.");
	        return setup(options);

	        function setup() {
	            var esm = ko.EditStateManager(objectToTrack, options);
	            var propertiesToCopy = ['toggleEditingCommand', 'beginEditingCommand', 'endEditingCommand', 'revertEditsCommand', 'cancelEditsCommand', 'saveEditsCommand', 'isEditing', 'hasChanges'];
	            for (var prop in propertiesToCopy) {
	                if (propertiesToCopy.hasOwnProperty(prop)) {
	                    var propName = propertiesToCopy[prop];
	                    objectToTrack[propName] = esm[propName];
	                }
	            }
	            return esm;
	        }
	    }
	    function setupKoEditStateManager() {
	        ko.EditStateManager.defaultOptions = (function () {
	            var defaultOptions = {
	                include: [],
	                ignore: ['__ko_mapping__'],
	                initializeAsEditing: false,
	                isInitiallyDirty: false,
	                canSave: function () { return true; },
	                name: "[unnamed_esm]",
	                canEdit: function () { return true; },
	                canEndEditing: function () { return true; },
	            };

	            return defaultOptions;
	        })();
	    }
	    //#endregion
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 56 */
/***/ (function(module, exports) {

	/*** IMPORTS FROM imports-loader ***/
	var require = false;
	var module = false;

	(function() {
	    if (require) {
	        define(['ko'], extendKnockout);
	    } else extendKnockout(ko);

	    function extendKnockout(ko) {
	        ko.DEBUG = true;

	        ko.EditStateManager = function(objectToTrack, options) {
	            var defaultOptions = ko.EditStateManager.defaultOptions;

	            options = options || {};

	            var include = defaultOptions.include || [];
	            if (options.include && options.include.length > 0) {
	                ko.utils.arrayPushAll(include, options.include);
	            }

	            options.initializeAsEditing = options.initializeAsEditing || defaultOptions.initializeAsEditing;
	            options.isInitiallyDirty = options.isInitiallyDirty || defaultOptions.isInitiallyDirty;
	            options.canSave = options.canSave || defaultOptions.canSave;
	            options.canEdit = options.canEdit || defaultOptions.canEdit;
	            options.canEndEditing = options.canEndEditing || defaultOptions.canEndEditing;

	            var name = options.name || defaultOptions.name;

	            var cacheMapping = options.customMappings || {};
	            var customRevertFunctions = options.customRevertFunctions || {};
	            var ignore = defaultOptions.ignore.concat(options.ignore);

	            if (ko.isObservable(objectToTrack) && isArray(objectToTrack())) {
	                // object is an observableArray
	                objectToTrack = {
	                    array: objectToTrack
	                };
	            }

	            var trackedObject = buildTrackedObject();
	            var revertUntrackedChanges = options.revertUntrackedChanges || emptyFn;
	            var hasUntrackedChanges = options.hasUntrackedChanges || function() { return false; };
	            var commitUntrackedChanges = options.commitUntrackedChanges || emptyFn;

	            var revertChangesCallback = options.revertChangesCallback || emptyFn;
	            var commitChangesCallback = options.commitChangesCallback || emptyFn;
	            var beginEditingCallback = options.beginEditingCallback || emptyFn;
	            var endEditingCallback = options.endEditingCallback || emptyFn;

	            function emptyFn() {}

	            var result = function() {},
	                isInitiallyDirty = ko.observable(options.isInitiallyDirty),
	                cachedState = ko.observable(),
	                currentHash = ko.computed(function() {
	                    return serialize(trackedObject);
	                }),
	                preventDirtyCheck = ko.observable(false);

	            var isEditing = ko.observable(false);
	            var isDirty = ko.computed(function() {
	                return preventDirtyCheck() || (isInitiallyDirty() || cachedState() !== currentHash());
	            });

	            // computed properties
	            result.isEditing = ko.computed(function() {
	                return isEditing();
	            });
	            result.isReadonly = ko.computed(function() {
	                return !isEditing();
	            }, result);
	            result.hasChanges = ko.computed(function() {
	                return isDirty() || hasUntrackedChanges();
	            });
	            //#endregion

	            //#region commands
	            result.toggleEditingCommand = ko.command({
	                execute: function() {
	                    if (result.isEditing()) result.endEditingCommand.execute();
	                    else result.beginEditingCommand.execute();
	                }
	            });
	            result.beginEditingCommand = ko.command({
	                canExecute: function() { return options.canEdit() && !isEditing(); },
	                execute: function() {
	                    beginEditing();
	                    beginEditingCallback();
	                },
	                log_name: name + ".beginEditingCommand",
	            });
	            result.endEditingCommand = ko.command({
	                canExecute: function() { return options.canEndEditing() && isEditing(); },
	                execute: function() { endEditing(); },
	                log_name: name + ".endEditingCommand",
	            });
	            result.revertEditsCommand = ko.command({
	                execute: function() {
	                    rollbackEdits();
	                    revertChangesCallback();
	                },
	                log_name: name + ".revertEditsCommand",
	            });
	            result.cancelEditsCommand = ko.command({
	                execute: function() {
	                    result.revertEditsCommand.execute();
	                    result.endEditingCommand.execute();
	                },
	                canExecute: function() { return result.hasChanges() || isEditing(); },
	                log_name: name + ".cancelEditsCommand",
	            });
	            result.saveEditsCommand = ko.command({
	                canExecute: function() { return options.canSave(); },
	                execute: function() {
	                    commitEdits();
	                    commitChangesCallback();
	                },
	                log_name: name + ".saveEditsCommand",
	            });
	            //#endregion

	            result.refreshState = cacheState;
	            result.defaultOptions = defaultOptions;

	            //#region init
	            if (options.initializeAsEditing) {
	                beginEditing();
	            }
	            cacheState();
	            //#endregion

	            //#region debug
	            if (ko.DEBUG) {
	                result.LOG = ko.observable(options.enableLogging);

	                if (result.LOG()) {
	                    cachedState.subscribe(function() {
	                        console.log(name + ' > cache value changed.');
	                        console.log({
	                            cache: cachedState(),
	                            currentHash: currentHash(),
	                            isEditing: isEditing(),
	                            isDirty: isDirty(),
	                        });
	                    });
	                    currentHash.subscribe(function() {
	                        console.log(name + ' > current hash value changed.');
	                        console.log({
	                            cache: cachedState(),
	                            currentHash: currentHash(),
	                            isEditing: isEditing(),
	                            isDirty: isDirty(),
	                        });
	                    });
	                }
	            }
	            //#endregion

	            result.dispose = function() {
	                objectToTrack(null);
	                objectToTrack = null;
	                cacheMapping = null;
	                customRevertFunctions = null;
	                ignore = null;
	                trackedObject = null;
	                revertUntrackedChanges = null;
	                hasUntrackedChanges = null;
	                commitUntrackedChanges = null;
	                revertChangesCallback = null;
	                commitChangesCallback = null;
	                beginEditingCallback = null;
	                endEditingCallback = null;
	                result = null;
	                isInitiallyDirty = null;
	                cachedState = null;
	                currentHash = null;
	                preventDirtyCheck = null;

	                isEditing = null;
	                isDirty = null;
	            }

	            return result;

	            //#region private functions
	            function beginEditing() {
	                if (isEditing() === true) return;
	                isEditing(true);
	            }

	            function endEditing() {
	                if (!isEditing()) return;
	                isEditing(false);
	                endEditingCallback();
	            }

	            function commitEdits() {
	                commitUntrackedChanges();
	                cacheState();
	                endEditing();
	                isInitiallyDirty(false);
	            }

	            function rollbackEdits() {
	                preventDirtyCheck(true);
	                var cache = deserializeCache();
	                recursiveRollback(trackedObject, cache);

	                revertUntrackedChanges();
	                cacheState();
	                preventDirtyCheck(false);

	                function recursiveRollback(current, original) {
	                    for (var propName in current) {
	                        var currentProp = current[propName];
	                        if (isEditStateManager(currentProp)) continue;

	                        var currentValue = ko.utils.unwrapObservable(currentProp);
	                        var originalValue = ko.utils.unwrapObservable(original[propName]);

	                        if (typeof currentValue === "function" || currentValue === originalValue) continue;

	                        setValue(originalValue);

	                        if (ko.DEBUG) {
	                            var newValue = ko.utils.unwrapObservable(currentProp);
	                            if (original.hasOwnProperty(propName) && newValue !== originalValue && options.customRevertFunctions[propName] == undefined) {
	                                console.warn('Revert failure:');
	                                console.debug({ message: 'Revert property \"' + propName + '\" failed', 'expected': originalValue, 'actual': newValue });
	                            }
	                        }
	                    }

	                    function isEditStateManager(prop) {
	                        return prop === result;
	                    }

	                    function setValue(value) {
	                        var revertFn = customRevertFunctions[propName] || defaultRevertFn;

	                        if (revertFn.length > 1) revertFn(value, current[propName]);
	                        else revertFn(value);
	                        
	                        function defaultRevertFn(val) {
	                            if (ko.isObservable(currentProp)) currentProp(val);
	                            else current[propName] = val;
	                        }
	                    }
	                }
	            }

	            function cacheState() {
	                cachedState(serialize(trackedObject));
	            }

	            function serialize(cacheObject) {
	                return ko.toJSON(cacheObject);
	            }

	            function buildTrackedObject() {
	                // Only observable properties are tracked by default. 
	                // This may  be  overridden by supplying an 'include' option, however, 
	                // the non-observable objects will not trigger changes to the tracked object.
	                trackedObject = {};

	                for (var prop in objectToTrack) {
	                    if (!isExcluded(prop, objectToTrack) && (isObservable(objectToTrack[prop]) || isIncluded(prop))) {
	                        trackedObject[prop] = objectToTrack[prop];
	                    }
	                }

	                return trackedObject;
	            }

	            function deserializeCache() {
	                var cache = ko.utils.parseJson(cachedState());
	                var hydrated = {};

	                for (var prop in cache) {
	                    hydrated[prop] = deserializeProperty(prop, cache);
	                }

	                return hydrated;
	            }

	            function deserializeProperty(propName, object) {
	                return typeof cacheMapping[propName] == "function"
	                    ? cacheMapping[propName].call(null, object[propName])
	                    : object[propName];
	            }

	            function isObservable(prop) {
	                return ko.isWriteableObservable
	                    ? ko.isWriteableObservable(prop)
	                    : (typeof prop == "function" && prop.name == "observable");
	            }

	            function isIncluded(propName) {
	                return ko.utils.arrayFirst(include, function(p) {
	                    return p === propName;
	                }) !== null;
	            }

	            function isExcluded(propName, obj) {
	                var prop = obj[propName];
	                return ko.utils.arrayFirst(ignore, function(p) {
	                    return typeof p === "string"
	                        ? p === propName
	                        : p === prop;
	                }) !== null;
	            }

	            function isArray(obj) {
	                return obj instanceof Array;
	                // Reece -- this wasn't working for me.
	                //return Object.prototype.toString(obj) === '[object Array]';
	            }
	            
	//#endregion
	        };

	        ko.EditStateManager.defaultOptions = (function() {
	            var defaultOptions = {
	                include: [],
	                ignore: ['__ko_mapping__'],
	                initializeAsEditing: false,
	                isInitiallyDirty: false,
	                canSave: function() { return true; },
	                name: "[unnamed_esm]",
	                canEdit: function() { return true; },
	                canEndEditing: function() { return true; },
	            };

	            return defaultOptions;
	        })();

	    }
	}());


/***/ }),
/* 57 */
/***/ (function(module, exports, __webpack_require__) {

	// style-loader: Adds some css to the DOM by adding a <style> tag

	// load the styles
	var content = __webpack_require__(58);
	if(typeof content === 'string') content = [[module.id, content, '']];
	// add the styles to the DOM
	var update = __webpack_require__(53)(content, {});
	// Hot Module Replacement
	if(false) {
		// When the styles change, update the <style> tags
		module.hot.accept("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\Content\\customer-contracts.css", function() {
			var newContent = require("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\Content\\customer-contracts.css");
			if(typeof newContent === 'string') newContent = [[module.id, newContent, '']];
			update(newContent);
		});
		// When the module is disposed, remove the <style> tags
		module.hot.dispose(function() { update(); });
	}

/***/ }),
/* 58 */
/***/ (function(module, exports, __webpack_require__) {

	exports = module.exports = __webpack_require__(52)();
	exports.push([module.id, ".customer-selector {\r\n    font-size: 1.5em !important;\r\n}\r\n\r\n.contract-header {\r\n    margin-bottom: 20px;\r\n}\r\n\r\n.address-book-buttons input[type=\"submit\"]{\r\n    float: right;\r\n}\r\n", ""]);

/***/ }),
/* 59 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(16), __webpack_require__(60),
	        __webpack_require__(61), __webpack_require__(63)], __WEBPACK_AMD_DEFINE_RESULT__ = function (ko, dirService, htmlMarkup) {

	    ko.components.register('address-book', {
	        viewModel: AddressBook,
	        template: htmlMarkup
	    });


	    /** View model */
	    function AddressBook(params) {
	        if (!ko.isObservable(params.companyKey)) throw "Observable `companyKey` required for Address Book";
	        if (!ko.isObservable(params.selected))   throw "Observable `selected` required for Address Book";
	        var self = this;

	        this.companyKey = params.companyKey();
	        this.selectedContact = params.selected;
	        this.contacts = ko.observableArray([]);

	        // Toggle selected contact
	        this.select = function (x) {
	            if (self.isSelected(x)) {
	                self.selectedContact(null);
	            } else {
	                self.selectedContact(x);
	            }
	        };

	        self.isSelected = function (x) {
	            var activeKey = self.selectedContact() ? self.selectedContact().ContactKey : null
	            return activeKey == x.ContactKey
	        };

	        this.load = ko.asyncCommand({
	            execute: function (complete) {
	                dirService.getContacts(self.companyKey)
	                .done(function (result) {
	                    self.contacts(result);
	                })
	                .fail(function (err) {
	                    console.error(err);
	                    alert('Failed fetching contacts');
	                })
	                .always(complete);
	            }
	        });

	        this.load.execute();



	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 60 */
/***/ (function(module, exports) {

	module.exports = "<h3>Address Book</h3>\r\n\r\n<div class=\"address-book\">\r\n    <em data-bind=\"text: 'Company Key: ' + companyKey\"></em>\r\n    <div data-bind=\"foreach: contacts\">\r\n        <address-book-contact data-bind=\"css: {'active': $parent.isSelected($data)}\"\r\n                              params=\"onSelect: $parent.select, details: $data\"></address-book-contact>\r\n    </div>\r\n</div>\r\n\r\n<div class=\"modal-message\" data-bind=\"fadeVisible: load.isExecuting\">\r\n    <div>Loading Contacts...</div>\r\n</div>"

/***/ }),
/* 61 */
/***/ (function(module, exports, __webpack_require__) {

	// style-loader: Adds some css to the DOM by adding a <style> tag

	// load the styles
	var content = __webpack_require__(62);
	if(typeof content === 'string') content = [[module.id, content, '']];
	// add the styles to the DOM
	var update = __webpack_require__(53)(content, {});
	// Hot Module Replacement
	if(false) {
		// When the styles change, update the <style> tags
		module.hot.accept("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\App\\components\\common\\address-book\\address-book.css", function() {
			var newContent = require("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\App\\components\\common\\address-book\\address-book.css");
			if(typeof newContent === 'string') newContent = [[module.id, newContent, '']];
			update(newContent);
		});
		// When the module is disposed, remove the <style> tags
		module.hot.dispose(function() { update(); });
	}

/***/ }),
/* 62 */
/***/ (function(module, exports, __webpack_require__) {

	exports = module.exports = __webpack_require__(52)();
	exports.push([module.id, "address-book {\r\n    display: block;\r\n}\r\naddress-book input[type=\"submit\"] {\r\n    float: right;\r\n}", ""]);

/***/ }),
/* 63 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/* WEBPACK VAR INJECTION */(function($) {!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9), __webpack_require__(16), __webpack_require__(64), __webpack_require__(65)], __WEBPACK_AMD_DEFINE_RESULT__ = function (ko, dirService, htmlMarkup) {

	    ko.components.register('address-book-contact', {
	        viewModel: Contact,
	        template: htmlMarkup
	    });

	    /** 
	     * A single selectable address-book contact.
	     */
	    function Contact(params) {
	        if (!params.details) throw 'Address Book Contact expects "details"';
	        var self = this;


	        // Transfer details
	        ['CompanyKey', 'ContactKey', 'EMailAddress', 'Name', 'PhoneNumber']
	        .forEach(function (prop) {
	            this[prop] = params.details[prop];
	        }, this);

	        this.Addresses = params.details.Addresses.map(
	            function (addr, i) {
	                addr.selected = ko.observable(i === 0);
	                return addr;
	            });
	        
	        // Selected address
	        this.selectedAddr = ko.observable(this.Addresses[0]);

	        // Contact selected
	        this.onSelect = function () {
	            if (typeof params.onSelect === 'function') {
	                params.onSelect(self);
	            }
	        };

	        // Address selected
	        this.selectAddr = function (ctx, ev) {
	            var x = $(ev.target).closest('.contact-address');
	            if (x.length > 0) {
	                var data = ko.contextFor(x[0]);
	                self.selectedAddr().selected(false);
	                self.selectedAddr(data.$data);
	                self.selectedAddr().selected(true);
	            } else {
	                console.error('Couldn\'t locate address to select');
	            }
	        }
	    }
	}.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	/* WEBPACK VAR INJECTION */}.call(exports, __webpack_require__(1)))

/***/ }),
/* 64 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"address-book-contact\" data-bind=\"click: onSelect\">\r\n    <h3 class=\"contact-name\" data-bind=\"text: Name || '( No name )'\"></h3>\r\n\r\n    <div class=\"address-list\" data-bind=\"foreach: Addresses, click: selectAddr, clickBubble: !$parents[1].isSelected($data)\">\r\n        <div class=\"contact-address\" data-bind=\"css: {'active-addr': selected}\">\r\n            <div data-bind=\"text: Address.AddressLine1\"></div>\r\n            <div data-bind=\"text: Address.AddressLine2\"></div>\r\n            <div data-bind=\"text: Address.AddressLine3\"></div>\r\n            <div data-bind=\"text: Address.City + ', ' + Address.State + ' ' + Address.PostalCode\"></div>\r\n        </div>\r\n    </div>\r\n</div>"

/***/ }),
/* 65 */
/***/ (function(module, exports, __webpack_require__) {

	// style-loader: Adds some css to the DOM by adding a <style> tag

	// load the styles
	var content = __webpack_require__(66);
	if(typeof content === 'string') content = [[module.id, content, '']];
	// add the styles to the DOM
	var update = __webpack_require__(53)(content, {});
	// Hot Module Replacement
	if(false) {
		// When the styles change, update the <style> tags
		module.hot.accept("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\App\\components\\common\\address-book\\address-book-contact.css", function() {
			var newContent = require("!!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\node_modules\\css-loader\\index.js!C:\\ReposLocal\\RVC\\src\\app\\RioValleyChili.Client.Mvc\\App\\components\\common\\address-book\\address-book-contact.css");
			if(typeof newContent === 'string') newContent = [[module.id, newContent, '']];
			update(newContent);
		});
		// When the module is disposed, remove the <style> tags
		module.hot.dispose(function() { update(); });
	}

/***/ }),
/* 66 */
/***/ (function(module, exports, __webpack_require__) {

	exports = module.exports = __webpack_require__(52)();
	exports.push([module.id, "\r\naddress-book-contact {\r\n    display: block;\r\n    -moz-user-select: none;\r\n    -ms-user-select: none;\r\n    -webkit-user-select: none;\r\n    user-select: none;\r\n}\r\n\r\n    address-book-contact .address-book-contact {\r\n        border-bottom: 1px solid grey;\r\n        padding: 1em;\r\n    }\r\n\r\n    address-book-contact:hover {\r\n        cursor: pointer;\r\n    }\r\n\r\n    address-book-contact.active {\r\n        background-color: rgb(188, 231, 244);\r\n    }\r\n\r\n    address-book-contact .contact-name, address-book-contact .address-list {\r\n        text-align: center;\r\n    }\r\n    address-book-contact .contact-address {\r\n        display: inline-block;\r\n        border: 1px solid;\r\n        border-radius: 3px;\r\n        padding: 1em;\r\n        margin: 0.5em;\r\n    }\r\n    address-book-contact .address-list .active-addr {\r\n        color: green;\r\n        border-width: thick;\r\n    }", ""]);

/***/ }),
/* 67 */
/***/ (function(module, exports, __webpack_require__) {

	var directoryService = __webpack_require__(16);

	ko.components.register( 'contact-picker', __webpack_require__(15) );

	/**
	  * @param {boolean} visible - Observable, toggle visibility
	  * @param {string} key - Company key to fetch contacts for
	  * @param {Object[]} companies - Array of companies for display and selection
	  * @param {Object[]} buttons - Buttons to display on picker UI
	  * @param {Function} buttons.callback - Callback function to call after successful selection
	  * @param {string} buttons.text - Button label text
	  */
	function AddressBookControllerVM( params ) {
	  if ( !(this instanceof AddressBookControllerVM) ) { return new AddressBookControllerVM( params ); }

	  var self = this;

	  // Constructors
	  function Button( opts ) {
	    this.text = opts.text;
	    this.callback = function () {
	      var response = self.inputData.selected();
	      response.company = self.company();
	      opts.callback( response );
	    };
	  }

	  this.disposables = [];

	  // Data
	  this.isVisible = ko.isObservable( params.visible ) && params.visible.extend({ notify: 'always' });
	  this.isPicking = ko.observable( false );
	  this.isLoading = ko.observable( false );

	  this.companies = params.companies || ko.observableArray( [] );
	  this.company = ko.observable();
	  this.companyKey = ko.pureComputed(function() {
	    var c = self.company();
	    return c && ko.unwrap( c.CompanyKey );
	  });

	  this.inputData = {
	    opts: ko.observableArray( [] ),
	    companyKey: this.companyKey,
	    selected: ko.observable( null ).extend({ notify: 'always' })
	  };

	  if ( ko.isObservable( params.key )) {
	    this.disposables.push(params.key.subscribe(function(val) {
	      setSelectedCompanyByKey(val);
	    }));
	  }
	  if (ko.unwrap(params.key) != null) {
	    setTimeout(function() {
	      setSelectedCompanyByKey(ko.unwrap(params.key));
	    }, 0);
	  }
	  this.companyKey.subscribe( function( newKey ) {
	    if ( newKey != null && self.isPicking() ) {
	      // Search for company data
	      getContactsById( newKey );
	    }
	  });

	  this.buttons = ko.observableArray( ko.utils.arrayMap( params.buttons, function( btn ) {
	    return new Button( btn);
	  } ) );

	  // Behaviors
	  function setSelectedCompanyByKey(keyValue) {
	    keyValue = ko.unwrap(keyValue);
	    var selected = ko.utils.arrayFirst(self.companies(), function (c) { return c.CompanyKey === keyValue; });
	    self.company(selected);
	  }
	  function hideUI() {
	    self.isPicking( false );
	    self.isLoading( false );
	    self.inputData.selected( null );
	    setSelectedCompanyByKey( ko.unwrap(params.key) );
	  }

	  this.toggleUI = ko.asyncCommand({
	    execute: function( complete ) {
	      var getContacts = getContactsById( ko.unwrap( self.companyKey )).then(
	        function( data, textStatus, jqXHR ) {
	          self.isPicking( true );
	      }).always( complete );
	    },
	    canExecute: function( isExecuting ) {
	      return !isExecuting;
	    }
	  });

	  this.cancel = function() {
	    self.inputData.selected( null );
	    self.isPicking( false );
	  };

	  function getContactsById( id ) {
	    var getContacts = directoryService.getContacts( id ).then(
	    function( data, textStatus, jqXHR ) {
	      self.inputData.opts( data );
	    },
	    function( jqXHR, textStatus, errorThrown ) {
	      showUserMessage( 'Could not get contacts', { description: errorThrown } );
	    });

	    return getContacts;
	  }

	  var visibleSub = this.isVisible.subscribe(function( bool ) {
	    if ( bool ) {
	      self.isLoading( true );

	      var _companyKey = ko.unwrap( self.companyKey );
	      if ( _companyKey ) {
	        var getContacts = getContactsById( _companyKey ).then(
	        function( data, textStatus, jqXHR ) {
	          self.isPicking( true );
	        },
	        function( jqXHR, textStatus, errorThrown ) {
	          self.isPicking( false );
	          showUserMessage( errorThrown );
	        }).always(function() {
	          self.isLoading( false );
	        });
	      } else {
	        self.isPicking( true );
	      }
	    } else {
	      hideUI();
	    }
	  });

	  this.disposables.push( visibleSub );

	  return this;
	}

	ko.utils.extend( AddressBookControllerVM.prototype, {
	  dispose: function() {
	    ko.utils.arrayForEach( this.disposables, this.disposeOne );
	    ko.utils.objectForEach( this, this.disposeOne );
	  },

	  // little helper that handles being given a value or prop + value
	  disposeOne: function( propOrValue, value ) {
	    var disposable = value || propOrValue;

	    if ( disposable && typeof disposable.dispose === "function" ) {
	      disposable.dispose();
	    }
	  }
	} );

	module.exports = {
	  viewModel: AddressBookControllerVM,
	  template: __webpack_require__(68)
	};


/***/ }),
/* 68 */
/***/ (function(module, exports) {

	module.exports = "<section data-bind=\"popup: isPicking, if: isVisible() && isPicking()\">\r\n  <section class=\"panel panel-default\">\r\n    <section class=\"panel-heading\">\r\n      <h3>Address Book</h3>\r\n    </section>\r\n    <section class=\"panel-body\">\r\n      <div class=\"form-group\">\r\n        <label class=\"control-label\"></label>\r\n        <select class=\"form-control\" data-bind=\"value: company, options: companies, optionsText: 'Name', optionsCaption: ' '\"></select>\r\n      </div>\r\n\r\n      <section class=\"container-fluid\">\r\n        <div class=\"row\">\r\n          <div class=\"col-xs-12\" data-bind=\"foreach: buttons\">\r\n            <button type=\"button\" class=\"btn btn-primary\" data-bind=\"text: text, click: callback, enable: $parent.inputData.selected()\"></button>\r\n          </div>\r\n        </div>\r\n      </section>\r\n\r\n      <section data-bind=\"if: inputData.opts\">\r\n        <br>\r\n        <contact-picker\r\n          params=\"options: inputData.opts,\r\n            companyKey: inputData.companyKey,\r\n            selected: inputData.selected\">\r\n        </contact-picker>\r\n      </section>\r\n    </section>\r\n  </section>\r\n</section>\r\n\r\n<div class=\"modal-message\" data-bind=\"fadeVisible: !toggleUI.canExecute()\">\r\n    <div>Loading Contacts...</div>\r\n</div>\r\n\r\n\r\n"

/***/ }),
/* 69 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(70), __webpack_require__(40), __webpack_require__(55), __webpack_require__(9)], __WEBPACK_AMD_DEFINE_RESULT__ = function (templateMarkup, salesService, koHelpers, ko) {

	        var currencyFormat = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });

	        function CustomerContractShipmentSummariesViewModel(params) {
	            if (!(this instanceof CustomerContractShipmentSummariesViewModel)) {
	                return new CustomerContractShipmentSummariesViewModel(params);
	            }

	            var vm = this;

	            this.reportItems = ko.observableArray([]);
	            this._subscriptions = [];

	            var contractsToReport = getContractKeysFromParam();

	            if (ko.isObservable(params.contractsToReport)) {
	                vm._subscriptions.push(params.contractsToReport.subscribe(function (values) {

	                    // convert cache object into array
	                    var previousState = [];
	                    for (var p in contractsToReport) {
	                        if (!contractsToReport.hasOwnProperty(p)) continue;
	                        previousState.push(contractsToReport[p]);
	                        break;
	                    }
	                    

	                    var differences = ko.utils.compareArrays(previousState || [], values || []);
	                    ko.utils.arrayForEach(differences, function(diff) {
	                        var key = getContractKey(diff.value);
	                        switch(diff.status) {
	                            case 'deleted': stopReportingContract(key);
	                                break;
	                            case 'added' : startReportingContract(key, diff.value);
	                                break;
	                        }
	                    });
	                }));
	            }

	            return this;

	            function getContractKeysFromParam() {
	                return ko.unwrap(params.contractsToReport).toObj(getContractKey);
	            }
	            function getContractKey(target) {
	                var keyValue = typeof target === "string" ? target
	                        : target && ko.unwrap(target.CustomerContractKey);

	                if (!keyValue) throw new Error("Unable to determine contract key from parameter target. No CustomerContractKey property was found.");

	                return keyValue;
	            }
	            function stopReportingContract(key) {
	                var itemToRemove = ko.utils.arrayFirst(vm.reportItems(), function(item) {
	                    return item.ContractKey === key;
	                });
	                if (itemToRemove) {
	                    ko.utils.arrayRemoveItem(vm.reportItems(), itemToRemove);
	                    vm.reportItems.notifySubscribers(vm.reportItems());
	                }

	                delete contractsToReport[key];
	            }
	            function startReportingContract(key, value) {
	                if (contractsToReport[key]) return;

	                contractsToReport[key] = value;

	                var item = {
	                    ContractKey: key,
	                    ContractHeader: ko.observable(),
	                    shipmentSummaries: ko.observableArray([]),
	                    TotalContractPounds: ko.observable(0),
	                    TotalPoundsShipped: ko.observable(0),
	                    TotalPoundsPending: ko.observable(0),
	                    TotalPoundsRemaining: ko.observable(0),
	                    TotalContractValueFormatted: ko.observable(0),
	                };

	                koHelpers.ajaxStatusHelper(item);

	                salesService.getShipmentSummaryForContract(key)
	                    .then(function (data) {
	                        if (!(data && data.length)) return;
	                        item.ContractHeader({
	                            ContractKey: data[0].ContractKey,
	                            ContractNumber: data[0].ContractNumber,
	                            ContractStatus: data[0].ContractStatus,
	                            ContractBeginDate: data[0].ContractBeginDate,
	                            ContractEndDate: data[0].ContractEndDate,
	                            reportUrl: data[0].Links["self"] && data[0].Links["self"].HRef,
	                        });

	                        var totalContractValue = 0,
	                            totalContractPounds = 0,
	                            totalPoundsShipped = 0,
	                            totalPoundsPending = 0,
	                            totalPoundsRemaining = 0;

	                        ko.utils.arrayPushAll(item.shipmentSummaries(), ko.utils.arrayMap(data, function(i) {
	                            totalContractValue += i.ContractItemValue;
	                            totalContractPounds += i.ContractItemPounds;
	                            totalPoundsShipped += i.TotalPoundsShippedForContractItem;
	                            totalPoundsPending += i.TotalPoundsPendingForContractItem;
	                            totalPoundsRemaining += i.TotalPoundsRemainingForContractItem;

	                            return {
	                                ProductName: i.ProductName,
	                                PackagingName: i.PackagingName,
	                                Treatment: i.Treatment,
	                                ContractItemValue: currencyFormat.format(i.ContractItemValue),
	                                ContractItemPounds: i.ContractItemPounds.toLocaleString(),
	                                TotalPoundsShippedForContractItem: i.TotalPoundsShippedForContractItem.toLocaleString(),
	                                TotalPoundsPendingForContractItem: i.TotalPoundsPendingForContractItem.toLocaleString(),
	                                TotalPoundsRemainingForContractItem: i.TotalPoundsRemainingForContractItem.toLocaleString(),
	                                BasePrice: i.BasePrice,
	                                CustomerProductCode: i.CustomerProductCode,
	                            };
	                        }));

	                        item.TotalContractValueFormatted(currencyFormat.format(totalContractValue));
	                        item.TotalContractPounds(totalContractPounds.toLocaleString());
	                        item.TotalPoundsShipped(totalPoundsShipped.toLocaleString());
	                        item.TotalPoundsPending(totalPoundsPending.toLocaleString());
	                        item.TotalPoundsRemaining(totalPoundsRemaining.toLocaleString());
	                        

	                        item.shipmentSummaries.notifySubscribers(item.shipmentSummaries());
	                        console.log(item);
	                    });

	                vm.reportItems.push(item);
	            }
	        }

	        CustomerContractShipmentSummariesViewModel.prototype.dispose = function() {
	            ko.utils.arrayForEach(this._subscriptions || [], function(sub) { sub && sub.dispose && sub.dispose(); });
	        };

	        return { viewModel: CustomerContractShipmentSummariesViewModel, template: templateMarkup };
	    }.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));


/***/ }),
/* 70 */
/***/ (function(module, exports) {

	module.exports = "<div class=\"container-fluid\" data-bind=\"visible: reportItems().length > 0\">\r\n  <div class=\"panel panel-default\">\r\n    <div class=\"panel-heading\">\r\n      <h3>Contract Shipments</h3>\r\n    </div>\r\n    <ul class=\"list-group\" data-bind=\"foreach: reportItems\">\r\n      <li class=\"list-group-item\">\r\n        <article>\r\n          <!-- ko ifnot: ContractHeader -->\r\n          <h4 data-bind=\"text: ContractKey\"></h4>\r\n          <!-- /ko -->\r\n\r\n          <!-- ko if: ContractHeader -->\r\n          <h4>\r\n            <!-- ko text: ContractHeader().ContractKey --><!-- /ko -->\r\n            (<!-- ko text: ContractHeader().ContractNumber --><!-- /ko -->)\r\n          </h4>\r\n\r\n          <a class=\"btn btn-link\" data-bind=\"attr: { href: ContractHeader().reportUrl }\">Contract Draw Summary Report</a>\r\n\r\n          <table class=\"table table-condensed\" data-bind=\"with: ContractHeader\">\r\n            <tbody>\r\n              <tr>\r\n                <td>\r\n                  <label class=\"display-label\">Contract Dates</label>\r\n                  <div class=\"display-field\" data-bind=\"text: ContractBeginDate + ' - ' + ContractEndDate\"></div>\r\n                </td>\r\n              </tr>\r\n              <tr>\r\n                <td>\r\n                  <label class=\"display-label\">Contract Status</label>\r\n                  <div data-bind=\"text: ContractStatus\" class=\"display-field\"></div>\r\n                </td>\r\n              </tr>\r\n            </tbody>\r\n          </table>\r\n          <!-- /ko -->\r\n\r\n          <div data-bind=\"if: ajaxWorking\">\r\n            Loading...\r\n          </div>\r\n\r\n          <!-- ko ifnot: ajaxWorking -->\r\n          <div class=\"table-responsive\">\r\n            <table class=\"table table-condensed\">\r\n              <thead>\r\n                <tr>\r\n                  <th>Product</th>\r\n                  <th>Packaging</th>\r\n                  <th>Treatment</th>\r\n                  <th class=\"text-nowrap\">Customer Code</th>\r\n                  <th>Base Price</th>\r\n                  <th>Contract Value</th>\r\n                  <th>Contract Pounds</th>\r\n                  <th>Lbs. Shipped</th>\r\n                  <th>Lbs. Pending</th>\r\n                  <th>Lbs. Remaining</th>\r\n                </tr>\r\n              </thead>\r\n\r\n              <tbody data-bind=\"foreach: shipmentSummaries\">\r\n                <tr>\r\n                  <td data-bind=\"text: ProductName\" class=\"truncate\"></td>\r\n                  <td data-bind=\"text: PackagingName\" class=\"truncate\"></td>\r\n                  <td data-bind=\"text: Treatment\" ></td>\r\n                  <td data-bind=\"text: CustomerProductCode\" ></td>\r\n                  <td data-bind=\"text: BasePrice\" ></td>\r\n                  <td data-bind=\"text: ContractItemValue\" ></td>\r\n                  <td data-bind=\"text: ContractItemPounds\"></td>\r\n                  <td data-bind=\"text: TotalPoundsShippedForContractItem\" ></td>\r\n                  <td data-bind=\"text: TotalPoundsPendingForContractItem\" ></td>\r\n                  <td data-bind=\"text: TotalPoundsRemainingForContractItem\" ></td>\r\n                </tr>\r\n              </tbody>\r\n\r\n              <tfoot>\r\n                <tr>\r\n                  <td colspan=\"5\"></td>\r\n                  <td data-bind=\"text: TotalContractValueFormatted\"></td>\r\n                  <td data-bind=\"text: TotalContractPounds\"></td>\r\n                  <td data-bind=\"text: TotalPoundsShipped\"></td>\r\n                  <td data-bind=\"text: TotalPoundsPending\"></td>\r\n                  <td data-bind=\"text: TotalPoundsRemaining\"></td>\r\n                </tr>\r\n              </tfoot>\r\n            </table>\r\n          </div>\r\n          <!-- /ko -->\r\n        </article>\r\n      </li>\r\n    </ul>\r\n  </div>\r\n</div>\r\n"

/***/ }),
/* 71 */
/***/ (function(module, exports, __webpack_require__) {

	var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;/*! Knockout projections plugin - version 1.1.0
	------------------------------------------------------------------------------
	Copyright (c) Microsoft Corporation
	All rights reserved.
	Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
	THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
	See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.
	------------------------------------------------------------------------------
	*/
	!function (a) { "use strict"; function b(a, b, c, d, e, f, g) { this.inputItem = b, this.stateArrayIndex = c, this.mappingOptions = e, this.arrayOfState = f, this.outputObservableArray = g, this.outputArray = this.outputObservableArray.peek(), this.isIncluded = null, this.suppressNotification = !1, this.outputArrayIndex = a.observable(d), this.disposeFuncFromMostRecentMapping = null, this.mappedValueComputed = a.computed(this.mappingEvaluator, this), this.mappedValueComputed.subscribe(this.onMappingResultChanged, this), this.previousMappedValue = this.mappedValueComputed.peek() } function c(a, b) { if (!a) return null; switch (a.status) { case "added": return a.index; case "deleted": return a.index + b; default: throw new Error("Unknown diff status: " + a.status) } } function d(a, c, d, e, f, g, h, i, j) { var k = "number" == typeof c.moved, l = k ? d[c.moved] : new b(a, c.value, e, f, g, h, i); return h.splice(e, 0, l), l.isIncluded && j.splice(f, 0, l.mappedValueComputed.peek()), k && (l.stateArrayIndex = e, l.setOutputArrayIndexSilently(f)), l } function e(a, b, c, d, e) { var f = b.splice(c, 1)[0]; f.isIncluded && e.splice(d, 1), "number" != typeof a.moved && f.dispose() } function f(a, b, c) { return a.stateArrayIndex = b, a.setOutputArrayIndexSilently(c), c + (a.isIncluded ? 1 : 0) } function g(a, b) { for (var c = {}, d = 0; d < a.length; d++) { var e = a[d]; "added" === e.status && "number" == typeof e.moved && (c[e.moved] = b[e.moved]) } return c } function h(a, b, c) { return c.length && b[a.index] ? b[a.index].outputArrayIndex.peek() : c.length } function i(a, b, i, j, k, l) { return b.subscribe(function (b) { if (b.length) { for (var m = g(b, i), n = 0, o = b[0], p = 0, q = o && h(o, i, j), r = o.index; o || r < i.length; r++) if (c(o, p) === r) { switch (o.status) { case "added": var s = d(a, o, m, r, q, l, i, k, j); s.isIncluded && q++, p++; break; case "deleted": e(o, i, r, q, j), p--, r--; break; default: throw new Error("Unknown diff status: " + o.status) } n++, o = b[n] } else r < i.length && (q = f(i[r], r, q)); k.valueHasMutated() } }, null, "arrayChange") } function j(a, c) { var d = this, e = [], f = [], g = a.observableArray(f), h = d.peek(); if ("function" == typeof c && (c = { mapping: c }), c.mappingWithDisposeCallback) { if (c.mapping || c.disposeItem) throw new Error("'mappingWithDisposeCallback' cannot be used in conjunction with 'mapping' or 'disposeItem'.") } else if (!c.mapping) throw new Error("Specify either 'mapping' or 'mappingWithDisposeCallback'."); for (var j = 0; j < h.length; j++) { var k = h[j], l = new b(a, k, j, f.length, c, e, g), n = l.mappedValueComputed.peek(); e.push(l), l.isIncluded && f.push(n) } var o = i(a, d, e, f, g, c), p = a.computed(g).extend({ trackArrayChanges: !0 }), q = p.dispose; return p.dispose = function () { o.dispose(), a.utils.arrayForEach(e, function (a) { a.dispose() }), q.call(this, arguments) }, m(a, p), p } function k(a, b) { return j.call(this, a, function (a) { return b(a) ? a : p }) } function l(a) { function b(a, b) { return function () { return b.apply(this, [a].concat(Array.prototype.slice.call(arguments, 0))) } } a[q] = { map: b(a, j), filter: b(a, k) } } function m(a, b) { return a.utils.extend(b, a[q]), b } function n(a) { a.projections = { _exclusionMarker: p }, l(a), m(a, a.observableArray.fn) } function o() { if ("undefined" != typeof module && "undefined" != typeof module.exports) { var b = __webpack_require__(9); n(b), module.exports = b } else  true ? !(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(9)], __WEBPACK_AMD_DEFINE_FACTORY__ = (n), __WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ? (__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__), __WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__)) : "ko" in a && n(a.ko) } var p = {}; b.prototype.dispose = function () { this.mappedValueComputed.dispose(), this.disposeResultFromMostRecentEvaluation() }, b.prototype.disposeResultFromMostRecentEvaluation = function () { if (this.disposeFuncFromMostRecentMapping && (this.disposeFuncFromMostRecentMapping(), this.disposeFuncFromMostRecentMapping = null), this.mappingOptions.disposeItem) { var a = this.mappedValueComputed(); this.mappingOptions.disposeItem(a) } }, b.prototype.mappingEvaluator = function () { null !== this.isIncluded && this.disposeResultFromMostRecentEvaluation(); var a; if (this.mappingOptions.mapping) a = this.mappingOptions.mapping(this.inputItem, this.outputArrayIndex); else { if (!this.mappingOptions.mappingWithDisposeCallback) throw new Error("No mapping callback given."); var b = this.mappingOptions.mappingWithDisposeCallback(this.inputItem, this.outputArrayIndex); if (!("mappedValue" in b)) throw new Error("Return value from mappingWithDisposeCallback should have a 'mappedItem' property."); a = b.mappedValue, this.disposeFuncFromMostRecentMapping = b.dispose } var c = a !== p; return this.isIncluded !== c && (null !== this.isIncluded && this.moveSubsequentItemsBecauseInclusionStateChanged(c), this.isIncluded = c), a }, b.prototype.onMappingResultChanged = function (a) { a !== this.previousMappedValue && (this.isIncluded && this.outputArray.splice(this.outputArrayIndex.peek(), 1, a), this.suppressNotification || this.outputObservableArray.valueHasMutated(), this.previousMappedValue = a) }, b.prototype.moveSubsequentItemsBecauseInclusionStateChanged = function (a) { var b, c, d = this.outputArrayIndex.peek(); if (a) for (this.outputArray.splice(d, 0, null), b = this.stateArrayIndex + 1; b < this.arrayOfState.length; b++) c = this.arrayOfState[b], c.setOutputArrayIndexSilently(c.outputArrayIndex.peek() + 1); else for (this.outputArray.splice(d, 1), b = this.stateArrayIndex + 1; b < this.arrayOfState.length; b++) c = this.arrayOfState[b], c.setOutputArrayIndexSilently(c.outputArrayIndex.peek() - 1) }, b.prototype.setOutputArrayIndexSilently = function (a) { this.suppressNotification = !0, this.outputArrayIndex(a), this.suppressNotification = !1 }; var q = "_ko.projections.cache"; o() }(this);

/***/ })
]);