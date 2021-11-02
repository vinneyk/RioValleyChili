define(['services/serviceCore', 'app'], function(core, app) {
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
});
