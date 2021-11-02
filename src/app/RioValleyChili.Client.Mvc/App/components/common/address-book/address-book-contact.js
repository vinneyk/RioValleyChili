define(['ko', 'services/directoryService', './address-book-contact.html', './address-book-contact.css'],
function (ko, dirService, htmlMarkup) {

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
});