define(['services/serviceCore'], function(core) {
    return {
        createDefect: createDefect,
        saveLotAttributes: saveLotAttributes,
        removeLotHold: removeLotHold,
        setLotQualityStatus: setLotQualityStatus,
        deleteLotDefectResolution: deleteLotDefectResolution,
        setLotHold: setLotHold,
        getLotByKey: getLotByKey,

        //#region customer product specs
        getCustomerProductSpec: getCustomerProductSpec,
        saveCustomerProductSpec: saveCustomerProductSpec,
        deleteCustomerProductSpec: deleteCustomerProductSpec,
        //#endregion customer product specs

        // Sample Matching
        buildSampleRequestsPager: buildSampleRequestsPager,
        getSampleRequestDetails: getSampleRequestDetails,
        createSampleRequest: createSampleRequest,
        updateSampleRequest: updateSampleRequest,
        deleteSampleRequest: deleteSampleRequest,
        setCustomerProductSpecs: setCustomerProductSpecs,
        setLabResults: setLabResults,
        createJournalEntry: createJournalEntry,
        updateJournalEntry: updateJournalEntry,
        deleteJournalEntry: deleteJournalEntry,
        getCustomerProductNames: getCustomerProductNames
    };

    function createDefect(data, callbackOptions) {
        return core.ajaxPost('/api/defects', data, callbackOptions);
    }
    function getLotByKey(lotKey, options) {
        return core.ajax("/api/lots/" + lotKey, options);
    }
    function saveLotAttributes(lotKey, data, options) {
        return core.ajaxPut("/api/lots/" + lotKey, data, options);
    }
    function removeLotHold(lotKey, optionsCallback) {
        return core.ajaxPut("/api/lots/" + lotKey + "/holds", null, optionsCallback);
    }
    function setLotQualityStatus(lotKey, qualityStatus, optionsCallback) {
        var status = qualityStatus;
        if (qualityStatus && typeof qualityStatus === "object") { status = qualityStatus.key; }
        if (status == undefined) throw new Error('Status value is required');
        return core.ajaxPut("/api/lots/" + lotKey + "/qualityStatus", { status: status }, optionsCallback);
    }
    function deleteLotDefectResolution(id, callbackOptions) {
        return core.ajaxDelete("/api/defects/" + id + "/resolution", callbackOptions);
    }
    function setLotHold(lotKey, data, optionsCallback) {
        return core.ajaxPut("/api/lots/" + lotKey + "/holds", data, optionsCallback);
    }

    //#region customer product specs
    function getCustomerProductSpec(customerKey, productKey) {
        return core.ajax(buildCustomerProductSpecUrl(customerKey, productKey));
    }
    function saveCustomerProductSpec(customerKey, productKey, values) {
        return core.ajaxPost(buildCustomerProductSpecUrl(customerKey, productKey), values);
    }
    function deleteCustomerProductSpec(customerKey, productKey, attributeKey) {
        return core.ajaxDelete(buildCustomerProductSpecUrl(customerKey, productKey) + '/' + attributeKey);
    }
    function buildCustomerProductSpecUrl(customerKey, productKey) {
        return ['/api/customers/', customerKey, '/products/', productKey, '/specs'].join('');
    }
    //#endregion

    // Sample Matching
    function buildSampleRequestsPager( options ) {
      var _options = options || {};
      return core.pagedDataHelper.init({
        urlBase: "/api/samplerequests",
        pageSize: _options.pageSize || 50,
        parameters: _options.parameters,
        resultCounter: function ( data ) {
          return data.length;
        },
        onNewPageSet: _options.onNewPageSet
      });
    }
    function getSampleRequestDetails( sampleKey ) {
      return core.ajax( '/api/samplerequests/' + sampleKey );
    }
    function createSampleRequest( sampleData ) {
      return core.ajaxPost( '/api/samplerequests', sampleData );
    }
    function updateSampleRequest( sampleKey, sampleData ) {
      return core.ajaxPut( '/api/samplerequests/' + sampleKey, sampleData );
    }
    function deleteSampleRequest( sampleKey ) {
      return core.ajaxDelete( '/api/samplerequests/' + sampleKey );
    }
    function setCustomerProductSpecs( sampleKey, itemKey, specs ) {
      return core.ajaxPut( '/api/samplerequests/' + sampleKey + '/items/' + itemKey + '/customerspecs', specs );
    }
    function setLabResults( sampleKey, itemKey, specs ) {
      return core.ajaxPut( '/api/samplerequests/' + sampleKey + '/items/' + itemKey + '/labresults', specs );
    }
    function createJournalEntry( sampleKey, journal ) {
      return core.ajaxPost( '/api/samplerequests/' + sampleKey + '/journals', journal );
    }
    function updateJournalEntry( sampleKey, journalKey, journal ) {
      return core.ajaxPut( '/api/samplerequests/' + sampleKey + '/journals/' + journalKey, journal );
    }
    function deleteJournalEntry( sampleKey, journalKey ) {
      return core.ajaxDelete( '/api/samplerequests/' + sampleKey + '/journals/' + journalKey );
    }
    function getCustomerProductNames() {
      return core.ajax('/api/samplerequests/customerproductnames');
    }
});
