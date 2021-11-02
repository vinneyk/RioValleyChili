function Address(data) {
    data = ko.utils.unwrapObservable(data) || { };
    var me = this;
    me.AddressLine1 = ko.observable(data.AddressLine1);
    me.AddressLine2 = ko.observable(data.AddressLine2);
    me.AddressLine3 = ko.observable(data.AddressLine3);
    me.City = ko.observable(data.City);
    me.State = ko.observable(data.State);
    me.PostalCode = ko.observable(data.PostalCode);
    me.Country = ko.observable(data.Country);

    return me;
}