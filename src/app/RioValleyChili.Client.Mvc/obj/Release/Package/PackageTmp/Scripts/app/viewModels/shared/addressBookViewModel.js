var AddressBookViewModel = (function () {
    var self = {};    // options
    self.options = {
        onclose: null,
        companySelection: true,
        subscribeAddress: null,
        subscribeContact: null,
        subscribeCustomer: null
    }
    self.cache = null;
    self.ESM = ko.observable();
    self.isNew = ko.observable(false);
    self.isEditing = ko.observable(false);
    self.isVisible = ko.observable(false);
    self.companies = ko.observableArray([]);
    self.selectedCompany = ko.observable();

    self.contacts = ko.observableArray([]);
    self.selectedContact = ko.observable();

    self.addresses = ko.observableArray([]);
    self.selectedAddress = ko.observable();

    var gettingContacts = false;


    // setters
    self.setCompany = function (key) {
        var sub = null;
        var todo = function () {
            self.selectedCompany(getCompanyByKey(key));
            if(sub) sub.dispose();
        };
        if (self.companies().length < 1) sub = self.companies.subscribe(todo);
        else todo();
    }
    self.setContact = function (key) {
        var sub = null;
        var innerSub = null;
        var todo = function () {
            var setContact = function () {
                self.selectedContact(getContactByKey(key));
                if(innerSub) innerSub.dispose();
            }
            if (gettingContacts) innerSub = self.contacts.subscribe(setContact);
            else setContact();
            if(sub) sub.dispose();
        }
        if (!self.selectedCompany()) sub = self.selectedCompany.subscribe(todo)
        else todo();
    }
    // computed getters
    self.isEditing = ko.computed({
        read: function () {
            return self.isNew() || self.ESM() && self.ESM().isEditing();
        },
        write: function (val) {
            if (self.ESM()) self.ESM().isEditing(val);
        }
    });
    self.isDirty = ko.computed(function () {
        return self.isNew() || self.ESM() && self.ESM().isDirty();
    });
    // public functions
    self.hide = function () {
        self.isVisible(false);
        if (typeof self.options.onclose === 'function')
            self.options.onclose(self.selectedAddress(), self.selectedContact(), self.selectedCompany());
    }
    self.show = function () {
        self.companies([]);
        self.selectedCompany(null);
        getCompanies();
        self.isVisible(true);
    }
    self.reset = function () {
        self.selectedAddress(null);
        self.selectedContact(null);
        self.selectedCompany(null);
    }
    self.undo = function () {
        self.ESM().resetEditsCommand.execute();
    }
    self.edit = function () {
        self.isEditing(!self.isEditing());
        if (self.isEditing() && self.selectedAddress()) self.selectedAddress().selected(false);
    }
    self.cancelNew = function () {
        if (self.isNew()) {
            self.contacts.remove(self.selectedContact());
            self.selectedContact(null);
            self.isNew(false);
        }
    }
    self.newContact = function () {
        self.isNew(true);
        var c = {
            Name: ko.observable(),
            PhoneNumber: ko.observable(),
            EMailAddress: ko.observable()
        };
        self.contacts.unshift(c);
        self.selectedContact(c);
        self.addresses([new Address()]);
    }
    self.delContact = function () {
        var onYes = function () {
            $.ajax("/api/contacts/" + self.selectedContact().ContactKey(), {
                type: "DELETE",
                success: function () {
                    showUserMessage("Contact deleted.");
                    self.contacts.remove(self.selectedContact());
                    self.selectedContact(null);
                    if (self.isEditing()) self.isEditing(false);
                    if (self.isNew()) self.isNew(false);
                },
                error: function (data) {
                    console.log(data);
                    showUserMessage("Error delete contact.", {
                        description: data.responseText || data.statusText
                    });
                }
            });
        }
        showUserMessage("Are you sure you want to delete this contact?", {
            description: "This cannot be undone.",
            autoClose: false,
            type: "yesnocancel",
            onYesClick: onYes
        });
    }
    self.addAddress = function () {
        self.addresses.unshift(new Address());
    }
    self.removeAddress = function (a) {
        self.addresses.remove(a);
    }
    self.save = function () {
        var details = self.selectedContact();
        var model = {
            Name: details.Name(),
            PhoneNumber: details.PhoneNumber(),
            EmailAddress: details.EMailAddress(),
            Addresses: ko.utils.arrayMap(self.addresses(), function (addr) {
                return { Address: ko.toJS(addr.Address), ContactAddressKey: addr.ContactAddressKey() };
            })
        };
        if (self.isNew()) createSave(model);
        else updateSave(model);
    }


    // computeds
    self.canSave = ko.computed(function () {
        return self.isEditing();
    });

    // private functions
    function setESM() {
        var model = {
            addresses: self.addresses
        };
        for (var prop in self.selectedContact())
            model[prop] = self.selectedContact()[prop];

        self.ESM(new ko.EditStateManager(model, {
            customMappings: {
                addresses: function (addrs) {
                    return ko.utils.arrayMap(addrs, function (address) {
                        return addressMap(address);
                    });
                },
                contact: function (contact) {
                    return ko.mapping.fromJS(contact);
                }
            }
        }));
    }
    function updateSave(dataModel) {
        $.ajax("/api/contacts/" + self.selectedContact().ContactKey(), {
            type: "PUT",
            contentType: "application/json",
            data: ko.toJSON(dataModel),
            success: function (data) {
                console.log(data);
                self.isEditing(false);
                self.ESM().saveEditsCommand.execute();
                showUserMessage("Updated contact successfully!");
            },
            error: function (data) {
                console.log(data);
                showUserMessage("Error updating contact.", {
                    description: data.responseText || data.statusText
                });
            }
        });
    }
    function createSave(dataModel) {
        dataModel.CompanyKey = self.selectedCompany().CompanyKey;
        console.log(dataModel);
        $.ajax("/api/contacts/", {
            type: 'POST',
            data: ko.toJSON(dataModel),
            contentType: 'application/json',
            success: function (data) {
                console.log(data);
                self.isNew(false);
                self.selectedContact().ContactKey = ko.observable(data);
                self.selectedContact.notifySubscribers(self.selectedContact());
                showUserMessage("Created contact successfully!");
            },
            error: function (data) {
                console.log(data);
                showUserMessage("Error creating contact.", {
                    description: data.statusText || data.responseText
                });
            }
        });
    }
    function getCompanies() {
        $.ajax("/api/companies", {
            success: function (data) {
                self.companies(data);
            },
            error: function (data) {
                console.log(data);
                showUserMessage("Error getting customers.", {
                    description: data.statusText || data.responseText
                });
            }
        });
    }
    function getContacts() {
        if (!self.selectedCompany()) return;
        gettingContacts = true;
        $.ajax("/api/contacts?companyKey=" + self.selectedCompany().CompanyKey, {
            success: function (data) {
                self.contacts(ko.utils.arrayMap(data, function(contact){
                    return ko.mapping.fromJS(contact);
                }));
                gettingContacts = false;
            },
            error: function (data) {
                console.log(data);
                showUserMessage("Error getting contacts for customer <b>" + self.selectedCompany().Name + "</b>.", {
                    description: data.statusText || data.responseText
                });
            }
        });
    }
    function getAddresses() {
        if (!self.selectedCompany() || !self.selectedContact() || self.isNew()) return;
        var compKey = self.selectedCompany().CompanyKey;
        var contactKey = self.selectedContact().ContactKey();
        if (!compKey || !contactKey) return;



        $.ajax("/api/companies/" + compKey + "/contacts/" + contactKey, {
            success: function (data) {
                self.selectedAddress(null);
                self.addresses(ko.utils.arrayMap(data.Addresses, function (address) {
                    return addressMap(address);
                }));
                setESM();
            },
            error: function (data) {
                console.log(data);
                showUserMessage("Error getting addresses for contact <b>" + self.selectedContact().Name() + "</b>.", {
                    description: data.statusText || data.responseText
                });
            },
        });
    }
    function addressMap(address) {
        address.selected = ko.observable(false);
        address.selectMe = function (addr, evt) {
            if (self.isEditing()) return;
            if (self.selectedAddress() === address)
                self.selectedAddress(null);
            else {
                if (self.selectedAddress()) self.selectedAddress().selected(false);
                self.selectedAddress(address);
            }
            address.selected(!address.selected());
            return false;
        }
        // null fields => empty fields
        $.each(address.Address, function (prop, val) {
            if (val == null) address.Address[prop] = '';
        });
        return ko.mapping.fromJS(address);
    }
    function getContactByKey(key) {
        return ko.utils.arrayFirst(self.contacts(), function (c) {
            return c.ContactKey() === key;
        }) || undefined;
    }
    function getCompanyByKey(key) {
        return ko.utils.arrayFirst(self.companies(), function (c) {
            return c.CompanyKey === key;
        }) || undefined;
    }
    function Address() {
        var me = {};
        me.Address = {
            AddressLine1: ko.observable(''),
            AddressLine2: ko.observable(''),
            AddressLine3: ko.observable(''),
            City: ko.observable(''),
            State: ko.observable(''),
            PostalCode: ko.observable(''),
            Country: ko.observable('')
        };
        me.ContactAddressKey = undefined;
        return addressMap(me);
    }
    // subscribers
    self.selectedCompany.subscribe(getContacts);
    self.selectedContact.subscribe(getAddresses);
    self.init = function (opts) {
        $.extend(self.options, opts);
        if (self.options.subscribeAddress) self.selectedAddress.subscribe(self.options.subscribeAddress);
        if (self.options.subscribeContact) self.selectedContact.subscribe(self.options.subscribeContact);
        if (self.options.subscribeCustomer) self.selectedCompany.subscribe(self.options.subscribeCustomer);

        return self;
    }
    return self;
})();