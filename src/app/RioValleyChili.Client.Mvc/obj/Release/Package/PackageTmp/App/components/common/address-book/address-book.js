define(['ko', 'services/directoryService', './address-book.html',
        './address-book.css', './address-book-contact'],
function (ko, dirService, htmlMarkup) {

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
});
