var CompanyContactsViewModel = (function ($, ko) {
    return {
        init: init,
    };

    function init(opts) {
        var self = new CompanyContacts(opts);
        return self;
    }
    
    function saveContact(contact, options) {
        var self = this,
            contactKey = ko.utils.unwrapObservable(contact.ContactKey),
            model = contact.toDto();

        if (contact.isNew()) post(model);
        else put(model);

        function put(dataModel) {
            $.ajax("/api/contacts/" + contactKey, {
                type: "PUT",
                contentType: "application/json",
                data: ko.toJSON(dataModel),
                success: options.successCallback,
                error: function (xhr) {
                    if (options.errorCallback) options.errorCallback(xhr);
                },
                complete: options.completeCallback,
            });
        }

        function post(dataModel) {
            $.ajax("/api/contacts/", {
                type: 'POST',
                data: ko.toJSON(dataModel),
                contentType: 'application/json',
                success: function (data) {
                    self.ContactKey(data);
                    if (options.successCallback) options.successCallback();
                },
                error: function (xhr) {
                    if (options.errorCallback) options.errorCallback(xhr);
                },
                complete: options.completeCallback,
            });
        }
    }
    function deleteContact(contact, options) {
        var self = this;
        showUserMessage("Are you sure you want to delete this contact?", {
            description: "This operation cannot be undone. Click \"Yes\" to delete or \"No\" or \"Cancel\" to cancel without deleting the contact.",
            autoClose: false,
            type: "yesnocancel",
            onYesClick: executeDeletion,
            onNoClick: options.completeCallback,
            onCancelClick: options.completeCallback,
        });

        function executeDeletion() {
            options = options || {};
            $.ajax("/api/contacts/" + ko.utils.unwrapObservable(contact.ContactKey), {
                type: "DELETE",
                success: function () {
                    self.deleteContactCommand.pushSuccess("Contact \"" + contact.Name() + "\" has been deleted.");
                    self.contacts.remove(contact);
                    if (self.currentContact() === contact) {
                        self.currentContact(self.contacts().length ? self.contacts()[0] : null);
                    }
                    if (options.successCallback) options.successCallback();
                },
                error: function (data) {
                    self.deleteContactCommand.pushError("Error deleting contact. Details: " + data.xhr.responseText);
                    if (options.errorCallback) options.errorCallback();
                },
                complete: options.completeCallback,
            });
        };
    }

    // types
    function CompanyContacts(input) {
        var me = this;
        input = ko.toJS(input);
        me.companyKey = input.CompanyKey;
        me.contacts = ko.observableArray(
            ko.utils.arrayMap(input.Contacts, mapContacts()) || []
        );
        me.currentContact = ko.observable(me.contacts().length > 0 ? me.contacts()[0] : undefined);
        
        // commands
        me.addNewContact = ko.command({
            execute: function () {
                var newContact = new Contact({CompanyKey: input.CompanyKey });
                me.contacts.push(newContact);
                me.selectContact(newContact);
                newContact.beginEditingCommand.execute();
            }
        });
        me.deleteContactCommand = ko.composableCommand({
            execute: function (complete) {
                deleteContact.call(me, me.currentContact(), { completeCallback: complete });
            },
            canExecute: function (isExecuting) {
                return !isExecuting && me.currentContact();
            }
        });

        // functions
        me.selectContact = function (contact) {
            if (!contact || contact === me.currentContact()) return;
            var valid = ko.utils.arrayFirst(me.contacts(), function (c) {
                return contact === c;
            }) !== null;
            if (!valid) throw new Error("The supplied contact was not valid because it is not a member of the contacts collection.");
            
            me.currentContact(contact);
        };
        
        return me;
        
        function mapContacts() {
            return function(c) {
                return new Contact(c);
            };
        }
    }
    function Contact(data) {
        var me = this;
        if (!data.CompanyKey) throw new Error("The contact must specify a company association (CompanyKey).");

        me.ContactKey = ko.observable(data.ContactKey);
        me.Name = ko.observable(data.Name);
        me.PhoneNumber = ko.observable(data.PhoneNumber);
        me.EMailAddress = ko.observable(data.EMailAddress);
        me.Addresses = ko.observableArray(mapAddresses());
        
        var esm = new EsmHelper(me, {
            customMappings: {
                Addresses: mapAddresses,
            },
            ignore: ['ContactKey'],
            // ContactKey should only be updated after a new contact is created.
        });

        // computed properties
        me.emailUrl = ko.computed(function () {
            var email = ko.utils.unwrapObservable(this.EMailAddress);
            return email ? 'mailto:' + email : '';
        }, me);
        me.displayEmailAddress = ko.computed(function () {
            return ko.utils.unwrapObservable(this.EMailAddress) && true;
        }, me);
        me.isNew = ko.computed(function () {
            return !this.ContactKey();
        }, me);
        me.nameDisplay = ko.computed(function () {
            var name = this.Name() || '[Unnamed]';
            return this.hasChanges() || this.isNew() ? '* ' + name : name;
        }, me);

        // functions
        me.toDto = toDto;
        
        var saveCommandChildren = ko.computed(function() {
            var addressCommands = ko.utils.arrayMap(me.Addresses(), function (addr) {
                return addr.saveEditsCommand;
            });
            addressCommands.push(esm.saveEditsCommand);
            return addressCommands;
        });

        // commands
        me.saveCommand = ko.composableCommand({
            children: saveCommandChildren,
            execute: function (contact, complete) {
                saveContact.call(me, contact, {
                    completeCallback: complete,
                    successCallback: function () {
                        me.saveCommand.pushSuccess("Contact saved successfully");
                    },
                    errorCallback: function (xhr) {
                        me.saveCommand.pushError("Failed to save contact. " + (xhr.responseText ? "Details: " + xhr.responseText : "" ));
                    }
                });
            },
            canExecute: function (isExecuting) {
                return !isExecuting;
            },
            moduleDependency: ko.composableCommand.moduleDependency.atLeastOneModuleRequired,
            log_name: 'SaveContactCommand'
        });
        me.newAddressCommand = ko.command({
            execute: function() {
                initializeNewAddress.call(me);
            }
        });
        me.removeAddressCommand = ko.command({
            execute: function(addressToRemove) {
                removeAddress.call(me, addressToRemove);
            }
        });
        
        return me;
        
        function mapAddresses() {
            return ko.utils.arrayMap(data.Addresses, function(address) {
                return new ContactAddress(address);
            }) || [new ContactAddress()];
        }
        function initializeNewAddress() {
            var vm = this;
            var newAddress = new ContactAddress();
            newAddress.beginEditingCommand.execute();
            var baseCancelCommand = newAddress.cancelEditsCommand;
            newAddress.cancelEditsCommand = ko.composableCommand({
                children: [baseCancelCommand],
                execute: function() {
                    removeAddress.call(vm, newAddress);
                }
            });
            this.Addresses.push(newAddress);
        }
        function removeAddress(addressToRemove) {
            this.Addresses.remove(addressToRemove);
        };
        function toDto() {
            return {
                ContactKey: this.ContactKey(),
                CompanyKey: data.CompanyKey,
                Name: this.Name(),
                PhoneNumber: this.PhoneNumber(),
                EmailAddress: this.EMailAddress(),
                Addresses: ko.utils.arrayMap(this.Addresses(), function (addr) {
                    return addr.toDto();
                })
            };
        }
    }
    function ContactAddress(data) {
        data = ko.utils.unwrapObservable(data) || { };
        var me = new Address(data.Address);
        me.ContactAddressKey = ko.observable(data.ContactAddressKey);
        me.Description = ko.observable(data.AddressDescription);

        var esm = new EsmHelper(me, {
            name: 'ContactAddress',
            ignore: [ 'ContactAddressKey']
        });
        
        // functions
        me.addressToDto = me.toDto;
        me.toDto = toDto;

        return me;
        
        function toDto() {
            return {
                ContactAddressKey: this.ContactAddressKey,
                AddressDescription: this.Description(),
                Address: me.addressToDto(),
            };
        }
    }
})(jQuery, ko);